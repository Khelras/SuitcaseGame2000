using Godot;
using System;

public partial class MainMenu : Control //We use Control instead of Node2D because we want to use the UI system
{
	[Export] public Button[] menuButton;
	public override void _Ready() {
		
		foreach (Button button in menuButton) {
			
			button.MouseFilter = MouseFilterEnum.Ignore; // mouse won't work
			
		}
	}

    private void OnStartPressed()
	{
		GetTree().ChangeSceneToFile("res://scenes/Phase1.tscn"); //Change the scene to the stage select when the start button is pressed
	}

	private void OnExitPressed()
	{
		GetTree().Quit(); //Quit the game when the exit button is pressed
    }
	private void OnBackMenuPressed() {
        GetTree().ChangeSceneToFile("res://scenes/MainMenu.tscn");
    }
	private void OnCreditsPressed() {
        GetTree().ChangeSceneToFile("res://scenes/Credits.tscn");
    }

}
