using Godot;
using System;

public partial class PhaseTwoMorning : Control
{
    [Export] public Label dayLabel;
    [Export] public Button continueButton;

    // Custom Signals
    [Signal] public delegate void MorningFadeInCompleteEventHandler();
    [Signal] public delegate void MorningFadeOutCompleteEventHandler();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        // Start as Faded Out
        Modulate = new Color(1f, 1f, 1f, 0f);

        // Disable Continue Button
        continueButton.Disabled = true;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public async void FadeInMorning(double duration = 1f)
    {
        // Fade In
        Visible = true;
        Tween tween = CreateTween();
        tween.TweenProperty(this, "modulate:a", 1.0f, duration);
        await ToSignal(tween, Tween.SignalName.Finished);

        // Enable Button
        continueButton.Disabled = false;

        // Emit Signal
        EmitSignal(SignalName.MorningFadeInComplete);
    }

    public async void FadeOutMorning(double duration = 1f)
    {
        // Disable Button
        continueButton.Disabled = false;

        // Fade Out
        Tween tween = CreateTween();
        tween.TweenProperty(this, "modulate:a", 0.0f, duration);
        await ToSignal(tween, Tween.SignalName.Finished);
        Visible = false;

        // Emit Signal
        EmitSignal(SignalName.MorningFadeOutComplete);
    }
}
