using Godot;
using System;

public partial class PhaseTwoEvent : Control
{
    [ExportGroup("Prompt")]
    [Export] Control prompt;
    [Export] Label promptLabel;
    [Export] Button optionA;
    [Export] Button optionB;

    [ExportGroup("Result")]
    [Export] Control result;
    [Export] Label resultLabel;
    [Export] public Button nextDayButton;

    // Signals
    [Signal] public delegate void EventPromptFadeInCompleteEventHandler();
    [Signal] public delegate void EventPromptFadeOutCompleteEventHandler();
    [Signal] public delegate void EventResultFadeInCompleteEventHandler();
    [Signal] public delegate void EventResultFadeOutCompleteEventHandler();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        // Visibility
        Visible = false;
        prompt.Visible = false;
        result.Visible = false;

        // Modulate
        prompt.Modulate = new Color(1f, 1f, 1f, 0f);
        result.Modulate = new Color(1f, 1f, 1f, 0f);
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void StartEvent(int day)
	{
        // Visibility
        Visible = true;
        prompt.Visible = true;
        result.Visible = false;

        // Prompt Text
        if (day == 1) // Day 1
        {
            promptLabel.Text = "Day 1 Prompt...";
        }
        else if (day == 2) // Day 2
        {
            promptLabel.Text = "Day 2 Prompt...";
        }
        else if (day == 3) // Day 3
        {
            promptLabel.Text = "Day 3 Prompt...";
        }

        // Fade Into Prompt
        PromptFadeIn();
    }

    public void ShowResult(int day)
    {
        // Result Text
        if (day == 1) // Day 1
        {
            resultLabel.Text = "Day 1 Result...";
        }
        else if (day == 2) // Day 2
        {
            resultLabel.Text = "Day 2 Result...";
        }
        else if (day == 3) // Day 3
        {
            resultLabel.Text = "Day 3 Result...";
        }

        // Fade Into Result
        ResultFadeIn();
    }

    public async void PromptFadeIn(float duration = 1f)
    {
        // Fade In
        prompt.Visible = true;
        Tween tween = CreateTween();
        tween.TweenProperty(prompt, "modulate:a", 1.0f, duration);
        await ToSignal(tween, Tween.SignalName.Finished);

        // Enable Buttons
        optionA.Disabled = false;
        optionB.Disabled = false;

        // Emit Signal
        EmitSignal(SignalName.EventPromptFadeInComplete);
    }

    public async void PromptFadeOut(float duration = 1f)
    {
        // Disable Buttons
        optionA.Disabled = true;
        optionB.Disabled = true;

        // Fade Out
        Tween tween = CreateTween();
        tween.TweenProperty(prompt, "modulate:a", 0.0f, duration);
        await ToSignal(tween, Tween.SignalName.Finished);
        prompt.Visible = false;

        // Emit Signal
        EmitSignal(SignalName.EventPromptFadeOutComplete);
    }

    public async void ResultFadeIn(float duration = 1f)
    {
        // Fade In
        result.Visible = true;
        Tween tween = CreateTween();
        tween.TweenProperty(result, "modulate:a", 1.0f, duration);
        await ToSignal(tween, Tween.SignalName.Finished);

        // Enable Buttons
        nextDayButton.Disabled = false;

        // Emit Signal
        EmitSignal(SignalName.EventResultFadeInComplete);
    }

    public async void ResultFadeOut(float duration = 1f)
    {
        // Fade Out
        Tween tween = CreateTween();
        tween.TweenProperty(result, "modulate:a", 0.0f, duration);
        await ToSignal(tween, Tween.SignalName.Finished);
        result.Visible = false;

        // Disable Buttons
        nextDayButton.Disabled = true;

        // Emit Signal
        EmitSignal(SignalName.EventResultFadeOutComplete);
    }
}
