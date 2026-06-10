using Godot;
using System;

/// <summary>
///     The Grid class represents a Grid in the game.
/// </summary>
public partial class Grid : Node2D
{
    [ExportGroup("Grid Configuration")]
    [Export] public Vector2 GridPositionOffset { get; set; }
	[Export] public Vector2I GridSize { get; set; }

    [ExportGroup("Grid References")]
    [Export] public TileMapLayer GridMap { get; set; }


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        // Check if TileMapLayer Reference is Set
        if (this.GridMap == null)
        {
            // DEBUG: If the TileMapLayer reference is not set, print a warning to the console
            GD.PushWarning("TileMapLayer reference is not set. Please assign it in the inspector.");
        }
        else
        {
            // DEBUG: Print the name of the TileMapLayer to confirm the reference is set correctly
            GD.Print("TileMapLayer reference is set successfully.");
        }

        // Set the Position of the Grid based on the set Grid Position
        this.GlobalPosition = this.GridPositionOffset;

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
}
