using Godot;
using System;
using System.Collections.Generic;
using System.Net.Quic;
using static Items;

public partial class GameItemManeger : Node
{
	private PackedScene scene;
    private Items items;
    private Button swapbutton;
    private DragAndDrop dragAndDrop;
    private Area2D area;

    private int currentItem;
    private bool itemSpawned = false;
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        string path = "res://scenes/Items.tscn";

        scene = GD.Load<PackedScene>(path); // Loading the entire scene
        //Instance the scene
       
        area = GetNode<Area2D>("Area2D");
        swapbutton = GetNode<Button>("Button");
        swapbutton.Pressed += OnSwapButtonPressed;
        area.AreaExited += OnAreaExited;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
    {   // only spawn once
        if (!itemSpawned) {

            SpawnNode(currentItem); // Spawns the sprite
            itemSpawned = true;
        } 
        
    }
    private void OnAreaExited(Area2D area) {
        GD.Print("Exit");
        items.RemoveItem(currentItem);
    }
    private void OnSwapButtonPressed() {
      if (items == null) return;
        currentItem = (currentItem + 1) % items.orderSprites.Count;
        
        items.SetItem(currentItem);

    }
	private void SpawnNode(int index) {
       if (items != null) return; // dont spawn if item does't exist
        // create a copy of the scene
        items = scene.Instantiate<Items>();

        AddChild(items);

        items.SetItem(index);
    }
    private void SetItemType(int index) {
        if (IsInstanceValid(items))
            items.SetItem(index);
    }

  
	
}
