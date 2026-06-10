using Godot;
using System;

public partial class MainMenu : Control //We use Control instead of Node2D because we want to use the UI system
{
	private void OnStartPressed()
	{
		GetTree().ChangeSceneToFile("res://scenes/StageSelect.tscn"); //Change the scene to the stage select when the start button is pressed
	}

	private void OnExitPressed()
	{
		GetTree().Quit(); //Quit the game when the exit button is pressed
    }
}
