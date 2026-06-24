using Godot;
using System;

public partial class PhaseTwoEvent : Control
{
    [ExportGroup("Prompt")]
    [Export] public Control prompt;
    [Export] public Label promptLabel;
    [Export] public Button optionA;
    [Export] public Button optionB;

    [ExportGroup("PromptBackground")]
    [Export] public Sprite2D promptBackground;
    [Export] public Texture2D[] backgroundImages;

    [ExportGroup("Result")]
    [Export] public Control result;
    [Export] public Label resultLabel;
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
        promptBackground.Visible = false;
        result.Visible = false;

        // Modulate
        prompt.Modulate = new Color(1f, 1f, 1f, 0f);
        promptBackground.Modulate = new Color(1f, 1f, 1f, 0f);
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
        promptBackground.Visible = true;
        result.Visible = false;

        // Prompt Text
        if (day == 1) // Day 1
        {
            promptLabel.Text = "Your landlord has been hinting about kicking you out...\n" +
                "\nA: Offer to pay more." +
                "\nB: Start looking for another place.";
        }
        else if (day == 2) // Day 2
        {
            promptLabel.Text = "Someone at the factory says that you can work extra hours if you accept an unsafe task...\n" +
                "\nA: Accept offer, I need the money." +
                "\nB: Reject offer, it is too risky...";
        }
        else if (day == 3) // Day 3
        {
            promptLabel.Text = "On the bus ride home, a stranger yells at you saying to 'go back to your country'\n" +
                "\nA: Ignore them." +
                "\nB: Find an item to take comfort in.";
        }

        // Prompt Background
        int index = day - 1;
        if (index < backgroundImages.Length && index >= 0)
        {
            promptBackground.Texture = backgroundImages[index];
        }
        else
        {
            // Remove Background
            promptBackground.Texture = null;
        }

        // Fade Into Prompt
        PromptFadeIn();
    }

    public void ShowResult(string resultText)
    {
        // Result Text
        resultLabel.Text = resultText;

        // Fade Into Result
        ResultFadeIn();
    }

    public async void PromptFadeIn(float duration = 1f)
    {
        // Fade In Prompt Background
        promptBackground.Visible = true;
        Tween bgTween = CreateTween();
        bgTween.TweenProperty(promptBackground, "modulate:a", 1.0f, duration);
        await ToSignal(bgTween, Tween.SignalName.Finished);

        // Fade In Prompt Text
        prompt.Visible = true;
        Tween promptTween = CreateTween();
        promptTween.TweenProperty(prompt, "modulate:a", 1.0f, duration);
        await ToSignal(promptTween, Tween.SignalName.Finished);

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

        // Fade Out Prompt Text
        Tween promptTween = CreateTween();
        promptTween.TweenProperty(prompt, "modulate:a", 0.0f, duration);
        await ToSignal(promptTween, Tween.SignalName.Finished);
        prompt.Visible = false;

        // Fade Out Prompt Background
        Tween bgTween = CreateTween();
        bgTween.TweenProperty(promptBackground, "modulate:a", 0.0f, duration);
        await ToSignal(bgTween, Tween.SignalName.Finished);
        promptBackground.Visible = false;

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
        // Disable Buttons
        nextDayButton.Disabled = true;

        // Fade Out
        Tween tween = CreateTween();
        tween.TweenProperty(result, "modulate:a", 0.0f, duration);
        await ToSignal(tween, Tween.SignalName.Finished);
        result.Visible = false;

        // Emit Signal
        EmitSignal(SignalName.EventResultFadeOutComplete);
    }
}
