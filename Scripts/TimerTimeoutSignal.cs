using Godot;
using System;

public partial class TimerTimeoutSignal : Timer
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Timeout += OnTimerTimeoutSignal;
	}


	private void OnTimerTimeoutSignal() {

		GD.Print("Time out");
	}
}
