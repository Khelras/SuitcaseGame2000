using Godot;
using System;
using System.Collections.Generic;

public partial class PhaseTwoManager : Node
{
	// Post-Migration Stats
	public const int maxHealth = 100;
	public const int maxShelter = 100;
	public const int maxBelonging = 100;
    public int health;
    public int shelter;
    public int belonging;

	// Days
	public const int totalDays = 3;
    public int day = 1;

    // Items
    public List<ItemID> survivalItems = new();
    public List<ItemID> sentimentalItems = new();
    public List<ItemID> culturalItems = new();
    public List<ItemID> functionalItems = new();

	// Game Manager
	private GameManager game;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        this.health = maxHealth;
		this.shelter = maxShelter;
		this.belonging = maxBelonging;

		// Global Game Manager
		game = GetNode<GameManager>("/root/GameManager");

		// Load the List of Items
		foreach (ItemID item in game.packedItems)
		{
			// Sort by Items
			switch (item.catagory)
			{
				case ItemCatagory.Survival: { survivalItems.Add(item); } break;
				case ItemCatagory.Sentimental: { sentimentalItems.Add(item); } break;
				case ItemCatagory.CulturalIdentity: { culturalItems.Add(item); } break;
				case ItemCatagory.Functional: { functionalItems.Add(item); } break;
			}
		}
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
