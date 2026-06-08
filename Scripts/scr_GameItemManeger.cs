using Godot;
using System;

public partial class scr_GameItemManeger : Node
{
	private PackedScene scene;
	private Node NodeBox;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        scene = GD.Load<PackedScene>("res://scenes/mesh_instance_2d.tscn"); // Getting the Node from the scene
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("SpawnObj")) {
            NodeBox = scene.Instantiate(); //Creates a node specificaly to the Item
			AddChild(NodeBox); // Spawns the obj
			GD.Print("Spanw");
		}
		if (Input.IsActionJustPressed("DeleteObj")) {
			NodeBox.QueueFree(); // Delete the specific Node
			GD.Print("Deleted");
		}
	}
}
