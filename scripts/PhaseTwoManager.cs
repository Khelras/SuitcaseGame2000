using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class PhaseTwoManager : Node
{
	// Post-Migration Stats
	public const int maxHealth = 30;
	public const int maxShelter = 30;
	public const int maxBelonging = 30;
	private const int dailyDecrement = 10;

	// Days
	public const int maxDays = 3;
    public int day = 1;

    // Items
    public List<ItemID> survivalItems = new();
    public List<ItemID> sentimentalItems = new();
    public List<ItemID> culturalItems = new();
    public List<ItemID> functionalItems = new();

	// Game Manager
	private GameManager game;

	// Progress Bars
	[ExportGroup("Progress Bars")]
	[Export] ProgressBar healthBar;
	[Export] ProgressBar shelterBar;
	[Export] ProgressBar belongingBar;

	// Morning and Event
    [ExportGroup("Morning and Event")]
    [Export] PhaseTwoMorning phaseTwoMorning;
	[Export] PhaseTwoEvent phaseTwoEvent;

    private string resultString;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
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

        // DEBUG
        GD.Print(survivalItems.Count);
        GD.Print(sentimentalItems.Count);
        GD.Print(culturalItems.Count);
        GD.Print(functionalItems.Count);

        // Progress Bars
        healthBar.MaxValue = maxHealth;
        shelterBar.MaxValue = maxShelter;
        belongingBar.MaxValue = maxBelonging;
        healthBar.MinValue = 0;
        shelterBar.MinValue = 0;
        belongingBar.MinValue = 0;
        healthBar.Value = Math.Min(maxHealth, (maxHealth - dailyDecrement) + survivalItems.Count);
        shelterBar.Value = Math.Min(maxShelter, (maxShelter - dailyDecrement) + functionalItems.Count);
        belongingBar.Value = Math.Min(maxBelonging, (maxBelonging - dailyDecrement) + sentimentalItems.Count + culturalItems.Count);

        // Fade into Morning
        phaseTwoMorning.dayLabel.Text = $"Day {day}:\nNew Beginnings";
        phaseTwoMorning.FadeInMorning();
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void OnContinueButtonPressed()
	{
        // Fade Out
        phaseTwoMorning.FadeOutMorning();
	}

	private void OnMorningFadeOutComplete()
	{
        // Check if Today is the Last Day
        if (day > maxDays)
        {
            // Close Game
            GetTree().Quit();
            return;
        }

        // Start the Event
        phaseTwoEvent.StartEvent(day);
	}

    private void OnOptionA()
    {
        if (day == 1) // Day 1
        {
            // Offer to Pay More Rent
            resultString = "You offered to pay more rent..." +
                "\n-Health" +
                "\n+Shelter" +
                "\n-Belonging";

            healthBar.Value -= dailyDecrement;
            belongingBar.Value -= dailyDecrement;
        }
        else if (day == 2) // Day 2
        {
            // Accept more work
            bool coinFlip = GD.Randf() > 0.5f;
            if (coinFlip == true)
            {
                // Nothing Happened
                resultString = "You accepted the extra hours... and got that bread!" +
                    "\n+Health" +
                    "\n-Belonging";

                shelterBar.Value -= dailyDecrement / 2;
                belongingBar.Value -= dailyDecrement;
            }
            else if (coinFlip == false)
            {
                // You got yourself Injured
                resultString = "You accepted the extra hours... and you got injured..." +
                    "\n-Health" +
                    "\n-Shelter" +
                    "\n-Belonging";

                healthBar.Value -= dailyDecrement;
                shelterBar.Value -= dailyDecrement;
                belongingBar.Value -= dailyDecrement;
            }
        }
        else if (day == 3) // Day 3
        {
            // Ignore the Stranger
            resultString = "You quietly ignored the stranger..." +
                "\n-Belonging";

            healthBar.Value -= dailyDecrement / 2;
            shelterBar.Value -= dailyDecrement / 2;
            belongingBar.Value -= dailyDecrement;
        }

        // Fade Out Prompt
        phaseTwoEvent.PromptFadeOut();
    }

    private void OnOptionB()
    {
        if (day == 1) // Day 1
        {
            // Start looking for another place to live
            resultString = "You started to look for another place to live..." +
                    "\n-Shelter";

            healthBar.Value -= dailyDecrement / 2;
            shelterBar.Value -= dailyDecrement;
            belongingBar.Value -= dailyDecrement / 2;
        }
        else if (day == 2) // Day 2
        {
            // Reject the extra work
            resultString = "You rejected the extra work" +
                    "\n+Belonging";

            healthBar.Value -= dailyDecrement / 2;
            shelterBar.Value -= dailyDecrement / 2;
        }
        else if (day == 3) // Day 3
        {
            // Take comfort in an item
            bool hasComfortItem = false;
            string comfortString = string.Empty;
            foreach (ItemID item in sentimentalItems)
            {
                if (item.name.ToLower() == "engravedamulet")
                {
                    hasComfortItem = true;
                    comfortString = "You took comfort from your family's Engraved Amulet you brought with you...";
                    break;
                }

                if (item.name.ToLower() == "wristwatch")
                {
                    hasComfortItem = true;
                    comfortString = "You took comfort from your dad's old Wristwatch on your wrist...";
                    break;
                }

                if (item.name.ToLower() == "book")
                {
                    hasComfortItem = true;
                    comfortString = "You took comfort from the Novel your mom gave you on your 8th birthday...";
                    break;
                }
            }

            // Player has the Pocket Watch
            if (hasComfortItem == true)
            {
                resultString = comfortString +
                    "it reminds you of home..." +
                    "\n+Belonging";

                healthBar.Value -= dailyDecrement / 2;
                shelterBar.Value -= dailyDecrement / 2;
            }
            // Player DOES NOT have the Pocket Watch
            else if (hasComfortItem == false)
            {
                resultString = "You had nothing... You quietly ignored the stranger..." +
                    "\n-Belonging";

                healthBar.Value -= dailyDecrement / 2;
                shelterBar.Value -= dailyDecrement / 2;
                belongingBar.Value -= dailyDecrement;
            }
        }

        // Fade Out Prompt
        phaseTwoEvent.PromptFadeOut();
    }

	private void OnEventPromptFadeOutComplete()
	{
        // Show Result String
        phaseTwoEvent.ShowResult(resultString);
	}

    private void OnNextDayPressed()
	{
		// Fade Out Result
		phaseTwoEvent.ResultFadeOut();
    }

	private void OnEventResultFadeOutComplete()
	{
		// Make Event Invisible
		phaseTwoEvent.Visible = false;

        // Otherwise, Next Day
        day++;

        if (day == 2)
        {
            phaseTwoMorning.dayLabel.Text = $"Day {day}:\nI'll be okay";
        }
        else if (day == 3)
        {
            phaseTwoMorning.dayLabel.Text = $"Day {day}:\nI miss home";
        }
        else if (day > maxDays) // Check if Today is the Last Day
        {
            string endTitle = string.Empty;
            if (healthBar.Value >= shelterBar.Value && healthBar.Value >= belongingBar.Value)
            {
                endTitle = "At least I'm surviving.";
            }
            else if (shelterBar.Value >= healthBar.Value && shelterBar.Value >= belongingBar.Value)
            {
                endTitle = "At least I have shelter.";
            }
            else if (belongingBar.Value >= healthBar.Value && belongingBar.Value >= shelterBar.Value)
            {
                endTitle = "I'm doing just fine.";
            }
            else
            {
                endTitle = "I don't belong here...";
            }

            phaseTwoMorning.dayLabel.Text = "The End:\n" + endTitle;
            phaseTwoMorning.dayLabel.LabelSettings.FontSize = 60;
            phaseTwoMorning.continueButton.GetNode<Label>("Label").Text = "Exit";
            phaseTwoMorning.FadeInMorning();
            return;
        }

        // Fade into Morning
        phaseTwoMorning.FadeInMorning();
    }
}
