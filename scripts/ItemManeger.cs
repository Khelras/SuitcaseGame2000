using Godot;
using System;
using System.Collections.Generic;
using System.Net.Quic;
using static Items;

public partial class ItemManeger : Node
{
    // Public Variables set Externally in the Editor
    [Export] public PackedScene itemsScene;

    private Items items;
    private Button swapbutton;
    private DragAndDrop dragAndDrop;
    private Area2D area;

    private int currentItem;
    private bool itemSpawned = false;
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Check if PackedScene Reference is Set
        if (this.itemsScene == null)
        {
            // DEBUG: If the TileMapLayer reference is not set, print a warning to the console
            GD.PushWarning("itemsScene reference is not set. Please assign it in the inspector.");
        }
        else
        {
            // DEBUG: Print the name of the TileMapLayer to confirm the reference is set correctly
            GD.Print("itemsScene reference is set successfully.");
        }
       
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
      if (items == null || items.orderSprites.Count <= 0) return;
        currentItem = (currentItem + 1) % items.orderSprites.Count;
        
        items.SetItem(currentItem);

    }
	private void SpawnNode(int index) {
       if (items != null) return; // dont spawn if item does't exist
        // create a copy of the scene
        items = itemsScene.Instantiate<Items>();

        AddChild(items);

        items.SetItem(index);
    }
    private void SetItemType(int index) {
        if (IsInstanceValid(items))
            items.SetItem(index);
    }

  
	
}
