using Godot;
using System;
using System.Net.Quic;
using static Items;

public partial class GameItemManeger : Node
{
	private PackedScene scene;
    private Items items;
    private Button swapbutton;
    private Items.SetItems currentItem = Items.SetItems.Box;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        string path = "res://scenes/Items.tscn";

        scene = GD.Load<PackedScene>(path); // Loading the entire scene
        swapbutton = GetNode<Button>("Button");
        swapbutton.Pressed += OnSwapButtonPressed;
        
        
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        SpawnNode(new Vector2(1055.0f, 573.0f), currentItem); // Spawns the sprite
    }
    private void OnSwapButtonPressed() {

        if (currentItem == Items.SetItems.Box)
        {
            currentItem = Items.SetItems.Circle;

            DeleteNode();
        }
        else if(currentItem == Items.SetItems.Circle)
        {
            currentItem = Items.SetItems.Cat;

            DeleteNode();
        }
        else if(currentItem == Items.SetItems.Cat)
        {
           currentItem = Items.SetItems.Box;

            DeleteNode();
        }


    }
	private void SpawnNode(Vector2 position, Items.SetItems type) {
        if (IsInstanceValid(items)) return;
        // create a copy of the scene
        items = scene.Instantiate<Items>(); 

        //Change the Position
        items.Position = position;

        // delay before the sprite spawn
        CallDeferred("add_child", items);

        // delay on which sprite to spawn
        CallDeferred(nameof(SetItemType), (int)type); 
    }
    private void SetItemType(Items.SetItems type) {
        if (IsInstanceValid(items))
            items.SetType(type);
    }
	private void DeleteNode() {
        if (IsInstanceValid(items)) {
            items.QueueFree();
            items = null;
        }
        
        
    }
  
	
}
