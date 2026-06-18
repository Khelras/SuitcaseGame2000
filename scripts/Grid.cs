using Godot;
using System.Collections.Generic;
using System;

/// <summary>
/// Manages a tile-based grid — cell creation, occupancy tracking,
/// item placement snapping, and drag preview highlighting.
/// </summary>
public partial class Grid : Node2D
{
    [ExportGroup("Grid Configuration")]
    [Export] public Vector2 GridPosition { get; set; }
    [Export] public Vector2I GridSize { get; set; }

    [ExportGroup("Grid References")]
    [Export] public TileMapLayer GridMap { get; set; }

    // Tracks which cells are currently occupied by placed items
    private HashSet<Vector2I> occupiedCells = new HashSet<Vector2I>();

    // Preview highlight state (drawn via _Draw)
    private bool showingPreview = false;
    private List<Vector2I> previewCells = new List<Vector2I>();
    private Color previewColor = Colors.Green;

    // ==================================================
    //  Lifecycle
    // ==================================================

    public override void _Ready()
    {
        ZAsRelative = false;
        ZIndex = 0;
        ShowBehindParent = false;

        if (GridMap == null)
        {
            GD.PushWarning("[Grid] TileMapLayer reference is not set. Please assign it in the inspector.");
            return;
        }

        // Render the tilemap behind everything else
        GridMap.ZAsRelative = true;
        GridMap.ZIndex = -10;
        GridMap.ShowBehindParent = true;

        GlobalPosition = GridPosition;

        // Populate the tilemap with cells
        for (int x = 0; x < GridSize.X; x++)
            for (int y = 0; y < GridSize.Y; y++)
                GridMap.SetCell(new Vector2I(x, y), 0, Vector2I.Zero, 0);
    }

    public override void _Process(double delta) { }

    // ==================================================
    //  Drawing
    // ==================================================

    public override void _Draw()
    {
        if (!showingPreview || GridMap == null || GridMap.TileSet == null) return;

        Vector2 tileSize = GridMap.TileSet.TileSize;

        foreach (Vector2I cell in previewCells)
        {
            // Convert cell to local draw space and draw an outline rect
            Vector2 localPos = ToLocal(CellToWorld(cell));
            Rect2 rect = new Rect2(localPos - tileSize / 2f, tileSize);
            DrawRect(rect, previewColor, false, 4f);
        }
    }

    // ==================================================
    //  Item Placement
    // ==================================================

    /// <summary>
    ///     Returns all cells covered by an item footprint starting from worldTopLeft.
    ///     Returns an empty list if any part of the footprint falls outside the grid.
    /// </summary>
    public List<Vector2I> GetCellsForItem(Vector2 worldTopLeft, Vector2I itemSizeByCell)
    {
        Vector2I originCell = WorldToCell(worldTopLeft);

        // Check both corners — if either is out of bounds the whole item is off-grid
        Vector2I bottomRightCell = new Vector2I(
            originCell.X + itemSizeByCell.X - 1,
            originCell.Y + itemSizeByCell.Y - 1
        );

        if (!IsCellInBounds(originCell) || !IsCellInBounds(bottomRightCell))
            return new List<Vector2I>();

        List<Vector2I> cells = new List<Vector2I>();
        for (int x = 0; x < itemSizeByCell.X; x++)
            for (int y = 0; y < itemSizeByCell.Y; y++)
                cells.Add(new Vector2I(originCell.X + x, originCell.Y + y));

        return cells;
    }

    /// <summary>
    ///     Returns the world position an item's center should snap to,
    ///     given its top-left world position and cell footprint.
    ///     MapToLocal returns cell centers, so we offset back to the top-left corner first.
    /// </summary>
    public Vector2 GetSnappedItemCenter(Vector2 worldTopLeft, Vector2I itemSizeByCell, Vector2I gridCellSize)
    {
        Vector2I originCell = WorldToCell(worldTopLeft);
        Vector2 tileSize = (Vector2)GridMap.TileSet.TileSize;
        Vector2 cellTopLeft = CellToWorld(originCell) - tileSize / 2f;
        Vector2 halfItemSize = (Vector2)(itemSizeByCell * gridCellSize) / 2f;

        return cellTopLeft + halfItemSize;
    }

    // ==================================================
    //  Preview Highlight
    // ==================================================

    /// <summary>
    ///     Highlights the given cells green (valid) or red (invalid) during a drag.
    /// </summary>
    public void ShowPreviewCells(List<Vector2I> cells, bool valid)
    {
        previewCells = cells;
        previewColor = valid ? Colors.Green : Colors.Red;
        showingPreview = true;
        QueueRedraw();
    }

    /// <summary>
    ///     Clears the drag preview highlight.
    /// </summary>
    public void HidePreviewCells()
    {
        showingPreview = false;
        previewCells.Clear();
        QueueRedraw();
    }

    // ==================================================
    //  Cell Utilities
    // ==================================================

    /// <summary>
    ///     Converts a world position to the nearest grid cell coordinate.
    /// </summary>
    public Vector2I WorldToCell(Vector2 worldPosition)
    {
        return GridMap.LocalToMap(GridMap.ToLocal(worldPosition));
    }

    /// <summary>
    ///     Converts a grid cell coordinate to its world position (cell center).
    /// </summary>
    public Vector2 CellToWorld(Vector2I cellPosition)
    {
        return GridMap.ToGlobal(GridMap.MapToLocal(cellPosition));
    }

    /// <summary>
    ///     Returns true if the cell is within the grid bounds.
    /// </summary>
    public bool IsCellInBounds(Vector2I cell)
    {
        return cell.X >= 0 && cell.X < GridSize.X &&
               cell.Y >= 0 && cell.Y < GridSize.Y;
    }

    /// <summary>
    ///     Returns true if the cell is already occupied by a placed item.
    /// </summary>
    public bool IsCellOccupied(Vector2I cell) => occupiedCells.Contains(cell);

    /// <summary>
    ///     Marks a cell as occupied.
    /// </summary>
    public void OccupyCell(Vector2I cell) => occupiedCells.Add(cell);

    /// <summary>
    ///     Marks a cell as free.
    /// </summary>
    public void FreeCell(Vector2I cell) => occupiedCells.Remove(cell);
}