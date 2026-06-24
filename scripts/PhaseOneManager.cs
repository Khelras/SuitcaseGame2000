using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class PhaseOneManager : Node
{
    [Export] ItemsManager itemManager;
    [Export] public Control LeftBehindPanel;
    [Export] public TextureRect LeftBehindImage;
    [Export] public Label LeftBehindText;
    [Export] public Label LeftBehindDescription;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    private async void OnFinishedButtonPressed()
    {
        List<Item> packedItems = itemManager.GetPackedItems();
        List<Item> leftBehindItems = itemManager.GetLeftBehindItems();
        List<Item> emotionalItems = new();

        // List of ItemIDs
        List<ItemID> packedItemIDs = new();
        List<ItemID> leftBehindItemIDs = new();

        foreach (Item item in packedItems)
        {
            // Item ID
            ItemID itemID = new ItemID();
            itemID.name = item.Name;
            itemID.catagory = item.Catagory;
            itemID.isPacked = item.IsPacked;
            packedItemIDs.Add(itemID);
        }

        foreach (Item item in leftBehindItems)
        {
            // Item ID
            ItemID itemID = new ItemID();
            itemID.name = item.Name;
            itemID.catagory = item.Catagory;
            itemID.isPacked = item.IsPacked;
            leftBehindItemIDs.Add(itemID);

            if (item.Catagory == ItemCatagory.Sentimental ||
                item.Catagory == ItemCatagory.CulturalIdentity)
            {
                emotionalItems.Add(item);
            }
        }

        Random random = new Random();

        emotionalItems = emotionalItems.OrderBy(x => random.Next()).Take(3).ToList();

        LeftBehindPanel.Visible = true;
        LeftBehindPanel.Modulate = new Color(1, 1, 1, 0);

        LeftBehindImage.Modulate = new Color(1, 1, 1, 0);
        LeftBehindText.Modulate = new Color(1, 1, 1, 0);
        LeftBehindDescription.Modulate = new Color(1, 1, 1, 0);

        // Fade whole screen to black
        Tween screenTween = CreateTween();
        screenTween.TweenProperty(LeftBehindPanel, "modulate:a", 1.0, 1.0);
        await ToSignal(screenTween, "finished");

        // Loop through and show the Items the Player Left Behind
        foreach (Item item in emotionalItems)
        {
            LeftBehindImage.Texture = item.Texture;
            LeftBehindText.Text = "Left behind: " + item.ItemName;
            LeftBehindDescription.Text = item.ItemDescription;

            // Fade item/text in
            Tween fadeIn = CreateTween();
            fadeIn.TweenProperty(LeftBehindImage, "modulate:a", 1.0, 0.6);
            fadeIn.Parallel().TweenProperty(LeftBehindText, "modulate:a", 1.0, 0.6);
            fadeIn.Parallel().TweenProperty(LeftBehindDescription, "modulate:a", 1.0, 0.6);
            await ToSignal(fadeIn, "finished");

            await ToSignal(GetTree().CreateTimer(4.0), "timeout");

            // Fade item/text out
            Tween fadeOut = CreateTween();
            fadeOut.TweenProperty(LeftBehindImage, "modulate:a", 0.0, 0.6);
            fadeOut.Parallel().TweenProperty(LeftBehindText, "modulate:a", 0.0, 0.6);
            fadeOut.Parallel().TweenProperty(LeftBehindDescription, "modulate:a", 0.0, 0.6);
            await ToSignal(fadeOut, "finished");
        }

        // Send Items to Game Manager
        GameManager game = GetNode<GameManager>("/root/GameManager");
        game.packedItems = packedItemIDs;
        game.leftBehindItems = packedItemIDs;

        // Go to the Next Phase
        GetTree().ChangeSceneToFile("res://scenes/Phase2.tscn");
    }
}
