using Godot;
using System.Collections.Generic;

public class ItemID
{
    public string name;
    public ItemCatagory catagory;
    public bool isPacked;
}

public enum ItemCatagory
{
    Survival,
    Sentimental,
    CulturalIdentity,
    Functional
}

/// <summary>
///     A draggable inventory item that snaps to a tile grid when dropped.
///     Handles its own drag input, cell occupancy, and placement validation.
/// </summary>
public partial class Item : Sprite2D
{
    [ExportGroup("Item Configuration")]
    [Export] public Vector2I ItemSizeByCell { get; private set; }
    public Vector2I GridCellSize { get; private set; } = new Vector2I(48, 48);

    private AudioStreamPlayer2D sfxGrab;
    private AudioStreamPlayer2D sfxRotate;
    private AudioStreamPlayer2D sfxDrop;
    private AudioStreamPlayer2D sfxInsideGrid;

    //Export ItemName, ItemDescription, and ItemCatagory for use in the UI when an item is selected
    [Export] public string ItemName { get; private set; } = "Unnamed Item";
    [Export(PropertyHint.MultilineText)] public string ItemDescription { get; private set; } = "No Description";
    [Export] public ItemCatagory Catagory { get; private set; }

    //Store if the item is packed, its packed if its placed
    public bool IsPacked => hasPlaced;

    // References
    private Grid grid;
    private Area2D area;
    private CollisionShape2D collisionShape;

    // Drag state
    public bool dragging { get; private set; } = false;
    private Vector2 offset;
    private Vector2 originalPosition;

    // Placement state
    private List<Vector2I> currentCells = new List<Vector2I>();
    private List<Vector2I> previousCells = new List<Vector2I>();
    private bool hasPlaced = false;
    private bool wasInsideGrid = false;
    private Vector2I currentCellPosition = Vector2I.Zero;

    // ==================================================
    //  Lifecycle
    // ==================================================

    public override void _Ready()
    {
        // Topmost Area2D under the mouse wins input which prevents multi-item drag
        GetViewport().PhysicsObjectPickingSort = true;
        GetViewport().PhysicsObjectPickingFirstOnly = true;

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

        // Scale the sprite so the texture fills exactly ItemSizeByCell * GridCellSize pixels
        Scale = (Vector2)(ItemSizeByCell * GridCellSize) / Texture.GetSize();

        // Duplicate the shape so each Item instance has its own copy (shapes are shared resources)
        collisionShape = GetNode<CollisionShape2D>("Area2D/CollisionShape2D");
        collisionShape.Shape = (Shape2D)collisionShape.Shape.Duplicate();
        if (collisionShape.Shape is RectangleShape2D rectShape)
            rectShape.Size = (Vector2)(ItemSizeByCell * GridCellSize) / Scale;

        area = GetNode<Area2D>("Area2D");
        area.InputEvent += OnInputEvent;
        area.MouseEntered += OnMouseEntered;
        area.MouseExited += OnMouseExited;

        sfxGrab = GetNode<AudioStreamPlayer2D>("SfxGrab");
        sfxRotate = GetNode<AudioStreamPlayer2D>("SfxRotate");
        sfxDrop = GetNode<AudioStreamPlayer2D>("SfxDrop");
        sfxInsideGrid = GetNode<AudioStreamPlayer2D>("SfxInsideTheGrid");

        // Grid is found via group so Items don't need a direct scene reference
        grid = GetTree().GetFirstNodeInGroup("grid") as Grid;
        if (grid == null)
            GD.PushWarning("[Item] No node found in 'grid' group.");
    }

    public override void _Process(double delta)
    {
        if (!dragging) return;

        GlobalPosition = GetGlobalMousePosition() - offset;

        Vector2 topLeft = GetWorldTopLeft();
        currentCells = grid.GetCellsForItem(topLeft, ItemSizeByCell);

        bool isInsideGird = currentCells.Count > 0;

        //Play only when Inside the cell and cell has changed
        if (isInsideGird && currentCells[0] != currentCellPosition) {
            sfxInsideGrid.Play();
            currentCellPosition = currentCells[0];
        }
        if (!isInsideGird) { //reset when its outside 

            currentCellPosition = Vector2I.Zero;
        }

        wasInsideGrid = isInsideGird;
        // Hide preview when outside the grid, show green/red when inside
        if (currentCells.Count == 0)
            grid.HidePreviewCells();
        else
            grid.ShowPreviewCells(currentCells, AreCellsValid(currentCells));
        
    }

    // ==================================================
    //  Input
    // ==================================================

    private void OnInputEvent(Node viewport, InputEvent @event, long shapeIdx)
    {
        if (@event is not InputEventMouseButton mouseButton) return;
        if (mouseButton.ButtonIndex != MouseButton.Left) return;

        if (mouseButton.Pressed)
        {
            PickUp();
            sfxGrab.Play();
        }
            
    }

    public override void _Input(InputEvent @event)
    { 
        if (@event is InputEventMouseButton mouseButton)
        {
            // Left Mouse Button Released to Drop Item
            if (mouseButton.ButtonIndex == MouseButton.Left && mouseButton.Pressed == false)
            {
                // Only Drop an Item if in a Dragging State
                if (dragging == false) return;

                Drop();
                sfxDrop.Play();
            }

            // Right Click to Rotate
            if (mouseButton.ButtonIndex == MouseButton.Right && mouseButton.Pressed && dragging)
            {
                if (dragging == false) return;
                Rotate90();
                sfxRotate.Play();
                return;
            }
        }
    }

    private void PickUp()
    {
        dragging = true;
        ZIndex = 10; // render and pick above all siblings
        Modulate = new Color(1f, 1f, 1f, 0.5f);
        originalPosition = GlobalPosition;
        offset = GetGlobalMousePosition() - GlobalPosition;

        // Free previously occupied cells so they're available during the drag
        if (hasPlaced)
            foreach (Vector2I cell in previousCells)
                grid.FreeCell(cell);
    }

    private void Drop()
    {
        dragging = false;
        ZIndex = 0;
        Modulate = Colors.White;

        Vector2 topLeft = GetWorldTopLeft();
        currentCells = grid.GetCellsForItem(topLeft, ItemSizeByCell);

        bool onGrid = currentCells.Count > 0;
        
        bool valid = onGrid && AreCellsValid(currentCells);

        if (valid)
        {
            // Snap to grid and occupy the covered cells
            GlobalPosition = grid.GetSnappedItemCenter(topLeft, ItemSizeByCell, GridCellSize);
            currentCells = grid.GetCellsForItem(GetWorldTopLeft(), ItemSizeByCell);

            foreach (Vector2I cell in currentCells)
                grid.OccupyCell(cell);

            previousCells = new List<Vector2I>(currentCells);
            hasPlaced = true;
        }
        else if (onGrid)
        {
            // On grid but blocked — return to original position and restore occupied cells
            GlobalPosition = originalPosition;

            if (hasPlaced)
                foreach (Vector2I cell in previousCells)
                    grid.OccupyCell(cell);
        }
        else
        {
            // Outside grid — leave item where it was dropped, clear placement state
            hasPlaced = false;
            previousCells.Clear();
        }

        grid.HidePreviewCells();
    }

    // ==================================================
    //  Rotation
    // ==================================================

    /// <summary>
    ///     Rotates the item 90degs clockwise, swaps its cell footprint, and
    ///     updates the collision shape to match the new dimensions.
    /// </summary>
    private void Rotate90()
    {
        RotationDegrees += 90f;

        // Swap cell footprint so the grid knows the new orientation
        ItemSizeByCell = new Vector2I(ItemSizeByCell.Y, ItemSizeByCell.X);

        // Only update collision shape — sprite scale stays the same
        ApplyRotationToSizeAndShape();

        // Refresh grid preview
        Vector2 topLeft = GetWorldTopLeft();
        currentCells = grid.GetCellsForItem(topLeft, ItemSizeByCell);
        if (currentCells.Count == 0)
            grid.HidePreviewCells();
        else
            grid.ShowPreviewCells(currentCells, AreCellsValid(currentCells));
    }

    /// <summary>
    ///     Updates only the collision shape to match the current ItemSizeByCell.
    ///     Scale is intentionally NOT changed — the sprite texture rotates naturally
    ///     with RotationDegrees, so the visual shape is already correct.
    /// </summary>
    private void ApplyRotationToSizeAndShape()
    {
        if (collisionShape.Shape is RectangleShape2D rectShape)
        {
            // ItemSizeByCell is already swapped before this is called,
            // so this correctly sets the collision box to the new W x H
            Vector2 pixelSize = (Vector2)(ItemSizeByCell * GridCellSize);
            rectShape.Size = pixelSize / (pixelSize / Texture.GetSize());
            rectShape.Size = Texture.GetSize();
        }
    }


    // ==================================================
    //  Helpers
    // ==================================================

    /// <summary>
    ///     Returns the world-space top-left corner of this item's footprint.
    /// </summary>
    private Vector2 GetWorldTopLeft()
    {
        return GlobalPosition - (Vector2)(ItemSizeByCell * GridCellSize) / 2f;
    }

    /// <summary>
    ///     Returns true if all cells are in bounds and unoccupied
    /// </summary>
    private bool AreCellsValid(List<Vector2I> cells)
    {
        if (cells.Count == 0) return false;
        foreach (Vector2I cell in cells)
            if (!grid.IsCellInBounds(cell) || grid.IsCellOccupied(cell))
                return false;
        return true;
    }

    private void OnMouseEntered()
    {
        ItemHoverUI hoverUI = GetTree().GetFirstNodeInGroup("item_hover_ui") as ItemHoverUI;
        hoverUI?.ShowItem(this);
    }
    private void OnMouseExited()
    {
        ItemHoverUI hoverUI = GetTree().GetFirstNodeInGroup("item_hover_ui") as ItemHoverUI;
        hoverUI?.HideItem(this);
    }
}