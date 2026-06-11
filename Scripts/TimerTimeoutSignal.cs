using Godot;
using System;

public partial class TimerTimeoutSignal : Timer
{
	private int TotalTimeInSec = 0;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//GetNode<Timer>("Timer").Start();
        Timeout += OnTimerTimeoutSignal;
    }


	private void OnTimerTimeoutSignal() {

		TotalTimeInSec++;

		int min = (int)(TotalTimeInSec / 60f);
		int sec = TotalTimeInSec - min * 60;
		GetNode<Label>("Label").Text = min.ToString("D2") + ":" + sec.ToString("D2");


		GD.Print("Time out");
		
	}
}
