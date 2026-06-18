using Godot;
using System;

public partial class Item : Sprite2D
{
    [ExportGroup("Item Configuration")]
    [Export] Vector2I GridCellSize;
    [Export] Vector2I ItemSizeByCell;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        // Check if GridCellSize was Ignored
        if (GridCellSize.X == 0 && GridCellSize.Y == 0)
        {
            // Warning Reminder to Set it
            GD.PushWarning("[Item] WARNING: Defaulting Grid Cell Size to 32x32.");
            GridCellSize = new Vector2I(32, 32);
        }

        // Check if ItemSizeByCell was Ignored
        if (ItemSizeByCell.X == 0 && ItemSizeByCell.Y == 0)
        {
            // Warning Reminder to Set it
            GD.PushWarning("[Item] WARNING: Remember to set a Item Size (by Grid Cells) in the inspector.");
        }

        // Ensure an Image Texture was Provided
        if (Texture == null)
        {
            GD.PushWarning("[Item] ERROR: Please set an Image Texture for this Item.");
            return;
        }

        // Scaling the Image Texture
        Scale = (ItemSizeByCell * GridCellSize) / Texture.GetSize();

        // Scaling the Collision Shape
        Shape2D shape = GetNode<CollisionShape2D>("Area2D/CollisionShape2D").Shape;
        if (shape is RectangleShape2D rectShape) rectShape.Size = Texture.GetSize();
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
