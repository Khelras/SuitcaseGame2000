using Godot;
using System;
using System.Collections.Generic;

public partial class DragAndDrop : Sprite2D
{
    // -- Drag and Drop Properties -- //
    // Placement
    private Grid grid;
    private Vector2 originalPosition;

    // Draging
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
        area = GetNode<Area2D>("Area2D");
        area.InputEvent += OnInputsEvent;

        grid = GetTree().GetFirstNodeInGroup("grid") as Grid;
        collisionShape = GetNode<CollisionShape2D>("Area2D/CollisionShape2D");

        if (grid == null)
        {
            GD.PushWarning("Grid node not found in the scene. Please ensure there is a node with the 'grid' group.");
        }
    }
    public override void _Process(double delta)
    {
        if (dragging)
        {
            GlobalPosition = GetGlobalMousePosition() - offset;

            Rect2 rectangle = GetWorldCollisionRect();
            currentCells = grid.GetCellsCoveredByRect(rectangle);
            bool valid = AreCellsValid(currentCells);
            grid.ShowPreviewCells(currentCells, valid);

        }

    }
    private void OnInputsEvent(Node vieport, InputEvent @event, long shapeIdx)
    {
        if (@event is InputEventMouseButton mouseButton)
        {
            if (mouseButton.ButtonIndex == MouseButton.Left)
            {
                if (mouseButton.Pressed == true)
                {
                    dragging = true;
                    GD.Print("Drag Pressed.");

                    Modulate = new Color(1, 1, 1, 0.5f); // Make the sprite semi-transparent while dragging

                    originalPosition = GlobalPosition;
                    // Offset to Drag based from where the Mouse Initially Clicked instead of the Origin of the Sprite.
                    offset = GetGlobalMousePosition() - GlobalPosition;

                    if (hasPlaced)
                    {
                        foreach (Vector2I cell in previousCells)
                        {
                            grid.FreeCell(cell);
                        }
                    }

                }
                else
                {
                    dragging = false;
                    GD.Print("Drag Released.");

                    Modulate = Colors.White; // Reset the sprite's color when not dragging

                    Rect2 rectangle = GetWorldCollisionRect();
                    currentCells = grid.GetCellsCoveredByRect(rectangle);

                    if (AreCellsValid(currentCells))
                    {
                        Vector2I snapCell = grid.WorldToCell(GlobalPosition);
                        GlobalPosition = grid.CellToWorld(snapCell);

                        currentCells = grid.GetCellsCoveredByRect(GetWorldCollisionRect());

                        foreach (Vector2I cell in currentCells)
                        {
                            grid.OccupyCell(cell);
                        }

                        previousCells = new List<Vector2I>(currentCells);
                        hasPlaced = true;
                    }
                    else
                    {
                        GlobalPosition = originalPosition;
                        
                        if (hasPlaced)
                        {
                            foreach (Vector2I cell in previousCells)
                            {
                                grid.OccupyCell(cell);
                            }
                        }
                    }
                    grid.HidePreviewCells();
                }
            }
        }
    }
    public bool GetDragging()
    {
        return dragging;
    }

    private Rect2 GetWorldCollisionRect()
    {
        RectangleShape2D rectangleShape = collisionShape.Shape as RectangleShape2D;

        if (rectangleShape == null)
        {
            GD.PushWarning("CollisionShape2D does not have a RectangleShape2D. Please ensure the CollisionShape2D is set up correctly.");
            return new Rect2(GlobalPosition, Vector2.Zero);
        }

        Vector2 size = rectangleShape.Size * collisionShape.GlobalScale.Abs();
        Vector2 topLeft = collisionShape.GlobalPosition - (size / 2);

        return new Rect2(topLeft, size);
    }

    private bool AreCellsValid (List<Vector2I> cells)
    {
        foreach (Vector2I cell in cells)
        {
            if (!grid.IsCellInBounds(cell) || grid.IsCellOccupied(cell))
            {
                return false;
            }
        }
        return true;
    }   

}

