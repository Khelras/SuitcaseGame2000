using Godot;
using System;
using System.Collections.Generic;

public partial class Item : Sprite2D
{
    [ExportGroup("Item Configuration")]
    [Export] private Vector2I GridCellSize;
    [Export] private Vector2I ItemSizeByCell;

    // -- Drag and Drop State --
    private Grid grid;
    private Vector2 originalPosition;

    private bool dragging = false;
    private Vector2 offset;
    private Area2D area;
    private CollisionShape2D collisionShape;

    private List<Vector2I> currentCells = new List<Vector2I>();
    private List<Vector2I> previousCells = new List<Vector2I>();
    private bool hasPlaced = false;
    // -- //

    public override void _Ready()
    {
        // ---- Viewport picking config (topmost item wins) ----
        GetViewport().PhysicsObjectPickingSort = true;
        GetViewport().PhysicsObjectPickingFirstOnly = true;

        // ---- Grid cell size default ----
        if (GridCellSize.X == 0 && GridCellSize.Y == 0)
        {
            GD.PushWarning("[Item] Defaulting GridCellSize to 32x32.");
            GridCellSize = new Vector2I(32, 32);
        }

        if (ItemSizeByCell.X == 0 && ItemSizeByCell.Y == 0)
            GD.PushWarning("[Item] Remember to set ItemSizeByCell in the inspector.");

        if (Texture == null)
        {
            GD.PushWarning("[Item] No texture set.");
            return;
        }

        // ---- Scale sprite to match grid footprint ----
        Scale = (Vector2)(ItemSizeByCell * GridCellSize) / Texture.GetSize();

        // ---- Set up collision shape ----
        collisionShape = GetNode<CollisionShape2D>("Area2D/CollisionShape2D");
        if (collisionShape.Shape is RectangleShape2D rectShape)
            rectShape.Size = Texture.GetSize();

        // ---- Wire up drag input ----
        area = GetNode<Area2D>("Area2D");
        area.InputEvent += OnInputEvent;

        // ---- Find grid via group ----
        grid = GetTree().GetFirstNodeInGroup("grid") as Grid;
        if (grid == null)
            GD.PushWarning("[Item] No node found in 'grid' group.");
    }

    public override void _Process(double delta)
    {
        if (!dragging) return;

        GlobalPosition = GetGlobalMousePosition() - offset;

        Rect2 rect = GetWorldCollisionRect();
        currentCells = grid.GetCellsCoveredByRect(rect);
        grid.ShowPreviewCells(currentCells, AreCellsValid(currentCells));
    }

    private void OnInputEvent(Node viewport, InputEvent @event, long shapeIdx)
    {
        if (@event is not InputEventMouseButton mouseButton) return;
        if (mouseButton.ButtonIndex != MouseButton.Left) return;

        if (mouseButton.Pressed)
        {
            // ---- Pick up ----
            dragging = true;
            ZIndex = 10;
            Modulate = new Color(1, 1, 1, 0.5f);
            originalPosition = GlobalPosition;
            offset = GetGlobalMousePosition() - GlobalPosition;

            if (hasPlaced)
            {
                foreach (Vector2I cell in previousCells)
                    grid.FreeCell(cell);
            }
        }
        else
        {
            // ---- Drop ----
            dragging = false;
            ZIndex = 0;
            Modulate = Colors.White;

            Rect2 rect = GetWorldCollisionRect();
            currentCells = grid.GetCellsCoveredByRect(rect);

            if (AreCellsValid(currentCells))
            {
                // Snap the top-left corner of the item to the nearest cell
                Vector2I topLeftCell = grid.WorldToCell(rect.Position);
                topLeftCell = new Vector2I(
                    Mathf.Clamp(topLeftCell.X, 0, grid.GridSize.X - 1),
                    Mathf.Clamp(topLeftCell.Y, 0, grid.GridSize.Y - 1)
                );

                Vector2 snappedWorld = grid.CellToWorld(topLeftCell);
                GlobalPosition = snappedWorld + rect.Size / 2f;

                // Re-sample after snapping
                currentCells = grid.GetCellsCoveredByRect(GetWorldCollisionRect());
                foreach (Vector2I cell in currentCells)
                    grid.OccupyCell(cell);

                previousCells = new List<Vector2I>(currentCells);
                hasPlaced = true;
            }
            else
            {
                GlobalPosition = originalPosition;

                if (hasPlaced)
                {
                    foreach (Vector2I cell in previousCells)
                        grid.OccupyCell(cell);
                }
            }

            grid.HidePreviewCells();
        }
    }

    // ---- Helpers ----

    private Rect2 GetWorldCollisionRect()
    {
        if (collisionShape.Shape is not RectangleShape2D rectShape)
        {
            GD.PushWarning("[Item] CollisionShape2D is not a RectangleShape2D.");
            return new Rect2(GlobalPosition, Vector2.Zero);
        }

        Vector2 size = rectShape.Size * collisionShape.GlobalScale.Abs();
        Vector2 topLeft = collisionShape.GlobalPosition - size / 2f;
        return new Rect2(topLeft, size);
    }

    private bool AreCellsValid(List<Vector2I> cells)
    {
        if (cells.Count == 0) return false;
        foreach (Vector2I cell in cells)
        {
            if (!grid.IsCellInBounds(cell) || grid.IsCellOccupied(cell))
                return false;
        }
        return true;
    }

    public bool GetDragging() => dragging;
    public Vector2I GetItemSizeByCell() => ItemSizeByCell;
    public Vector2I GetGridCellSize() => GridCellSize;
}