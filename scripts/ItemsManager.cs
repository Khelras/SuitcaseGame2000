using Godot;
using System.Linq;
using System;
using System.Collections.Generic;

public enum ItemCatagory
{
    Survival,
    Sentimental,
    CulturalIdentity,
    Functional
}

public class ItemID
{
	public string name;
	public ItemCatagory catagory;
	public bool isPacked;
}

public partial class ItemsManager : Node
{
    [Export] public Control LeftBehindPanel;
    [Export] public TextureRect LeftBehindImage;
    [Export] public Label LeftBehindText;
    [Export] public Label LeftBehindDescription;

    public List<Item> GetAllItems()
	{
		List<Item> items = new();

		foreach (Node node in GetTree().GetNodesInGroup("items"))
		{
			if (node is Item item)
			{
				items.Add(item);
			}
		}
		return items;
	}

	public List<Item> GetPackedItems()
	{
		List<Item> packedItems = new();

		foreach(Item item in GetAllItems()) 
		{
			if (item.IsPacked == true)
			{
				packedItems.Add(item);
			}
		}
		return packedItems;
    }

	public List<Item> GetLeftBehindItems()
	{
		List<Item> leftBehindItems = new();

		foreach (Item item in GetAllItems())
		{
			if (!item.IsPacked)
			{
				leftBehindItems.Add(item);
			}
		}
		return leftBehindItems;

	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private async void OnButtonPressed()
	{
        List<Item> leftBehindItems = GetLeftBehindItems();

        List<Item> emotionalItems = new();

        foreach (Item item in leftBehindItems)
        {
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

            await ToSignal(GetTree().CreateTimer(2.0), "timeout");

            // Fade item/text out
            Tween fadeOut = CreateTween();
            fadeOut.TweenProperty(LeftBehindImage, "modulate:a", 0.0, 0.6);
            fadeOut.Parallel().TweenProperty(LeftBehindText, "modulate:a", 0.0, 0.6);
            fadeOut.Parallel().TweenProperty(LeftBehindDescription, "modulate:a", 0.0, 0.6);
            await ToSignal(fadeOut, "finished");
        }

        LeftBehindPanel.Visible = false;

    }
}
