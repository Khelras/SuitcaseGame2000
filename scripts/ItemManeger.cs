using Godot;
using System;
using System.Collections.Generic;
using System.Net.Quic;
using static Items;

public partial class ItemManeger : Node
{
    private Items items;
    private int currentItem;
    private Button swapbutton;
    private Area2D area;
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Items
        items = GD.Load<PackedScene>("res://scenes/Items.tscn").Instantiate<Items>();
        AddChild(items);
        currentItem = 0; // First Item
        items.SetItem(0);
        
        // Item Swapping
        area = GetNode<Area2D>("Area2D");
        swapbutton = GetNode<Button>("Button");
        swapbutton.Pressed += OnSwapButtonPressed;
        area.AreaExited += OnAreaExited;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
    {
    }

    private void OnAreaExited(Area2D area) {
        GD.Print("Exit");
        items.RemoveItem(currentItem);
    }

    private void OnSwapButtonPressed() {
        // Ensure Items Exists
        if (items == null || items.orderSprites.Count <= 0) return;

        // Swap to the Next Item
        currentItem = (currentItem + 1) % items.orderSprites.Count;
        items.SetItem(currentItem);
    }
}
