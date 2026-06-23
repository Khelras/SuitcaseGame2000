using Godot;
using System.Collections.Generic;

public partial class ItemsManager : Node
{
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
}
