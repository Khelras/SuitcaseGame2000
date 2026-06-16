using Godot;
using System.Collections.Generic;
using System;

/// <summary>
///     The Grid class represents a Grid in the game.
/// </summary>
public partial class Grid : Node2D
{
    [ExportGroup("Grid Configuration")]
    [Export] public Vector2 GridPosition { get; set; }
    [Export] public Vector2I GridSize { get; set; }

    [ExportGroup("Grid References")]
    [Export] public TileMapLayer GridMap { get; set; }

    private HashSet<Vector2I> occupiedCells = new HashSet<Vector2I>();

    private bool showingPreview = false;
    private List<Vector2I> previewCells = new List<Vector2I>();
    private Color previewColor = Colors.Green;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        ZAsRelative = false;
        ZIndex = 0;
        ShowBehindParent = false;

        // Check if TileMapLayer Reference is Set
        if (GridMap == null)
        {
            // DEBUG: If the TileMapLayer reference is not set, print a warning to the console
            GD.PushWarning("TileMapLayer reference is not set. Please assign it in the inspector.");
            return;
        }

        GridMap.ZAsRelative = true;
        GridMap.ZIndex = -10;
        GridMap.ShowBehindParent = true;

        // Set the Position of the Grid based on the set Grid Position
        this.GlobalPosition = this.GridPosition;

        // Loop to create the Grid based on the set Grid Size
        for (int x = 0; x < this.GridSize.X; x++)
        {
            for (int y = 0; y < this.GridSize.Y; y++)
            {
                this.GridMap.SetCell(new Vector2I(x, y), 0, Vector2I.Zero, 0);
            }
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    // Method to Snap a World Position to the Grid
    public Vector2 SnapWorldPositionToGrid(Vector2 WorldPosition)
    {
        Vector2 LocalPosition = GridMap.ToLocal(WorldPosition);
        Vector2I CellPosition = GridMap.LocalToMap(LocalPosition);
        Vector2 SnappedLocalPosition = GridMap.MapToLocal(CellPosition);

        return GridMap.ToGlobal(SnappedLocalPosition);
    }

    // Utility Methods for Grid Management
    public Vector2I WorldToCell(Vector2 WorldPosition)
    {
        Vector2 LocalPosition = GridMap.ToLocal(WorldPosition);
        return GridMap.LocalToMap(LocalPosition);
    }
    public Vector2 CellToWorld(Vector2I CellPosition)
    {
        Vector2 LocalPosition = GridMap.MapToLocal(CellPosition);
        return GridMap.ToGlobal(LocalPosition);
    }
    public bool IsCellOccupied(Vector2I CellPosition)
    {
        return occupiedCells.Contains(CellPosition);
    }
    public void OccupyCell(Vector2I CellPosition)
    {
        occupiedCells.Add(CellPosition);
    }
    public void FreeCell(Vector2I CellPosition)
    {
        occupiedCells.Remove(CellPosition);
    }

    public bool IsCellInBounds(Vector2I CellPosition)
    {
        return CellPosition.X >= 0 && CellPosition.X < GridSize.X && CellPosition.Y >= 0 && CellPosition.Y < GridSize.Y;
    }

    public List<Vector2I> GetCellsCoveredByRect(Rect2 worldRect)
    {
        List<Vector2I> cells = new List<Vector2I>();

        Vector2 topLeftWorld = worldRect.Position;
        Vector2 bottomRightWorld = worldRect.End - new Vector2(1, 1); // Subtract a small value to ensure we get the correct cell for the bottom-right corner

        Vector2I topLeft = WorldToCell(topLeftWorld);
        Vector2I bottomRight = WorldToCell(bottomRightWorld);

        int minX = Math.Min(topLeft.X, bottomRight.X);
        int maxX = Math.Max(topLeft.X, bottomRight.X);
        int minY = Math.Min(topLeft.Y, bottomRight.Y);
        int maxY = Math.Max(topLeft.Y, bottomRight.Y);

        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                cells.Add(new Vector2I(x, y));
            }
        }

        return cells;
    }

    public void ShowPreviewCells(List<Vector2I> cells, bool valid)
    {
        previewCells = cells;
        previewColor = valid ? Colors.Green : Colors.Red;
        showingPreview = true;
        QueueRedraw();
    }

    public void HidePreviewCells()
    {
        showingPreview = false;
        previewCells.Clear();
        QueueRedraw();
    }

    public override void _Draw()
    {
        if (!showingPreview || GridMap == null || GridMap.TileSet == null)
        {
            return;
        }

        Vector2 tileSize = GridMap.TileSet.TileSize;

        foreach (Vector2I cell in previewCells)
        {
            Vector2 worldPos = CellToWorld(cell);
            Vector2 localPos = ToLocal(worldPos);

            Rect2 rect = new Rect2(localPos - tileSize / 2f, tileSize);

            DrawRect(rect, previewColor, false, 4f);
        }
    }
}
