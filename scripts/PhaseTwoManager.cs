using Godot;
using System;

public partial class PhaseTwoManager : Node
{
	// Post-Migration Stats
	const int maxHealth = 100;
	const int maxShelter = 100;
	const int maxBelonging = 100;
    int health = maxHealth;
    int shelter = maxShelter;
    int belonging = maxBelonging;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
