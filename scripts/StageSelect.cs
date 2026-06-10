using Godot;
using System;

public partial class StageSelect : Control //We use Control instead of Node2D because we want to use the UI system
{
    private void OnStage1Pressed()
    {
        GetTree().ChangeSceneToFile("res://scenes/Stage1Levels.tscn"); //Change the scene to Stage1 when the button is pressed
    }

    private void OnStage2Pressed()
    {
        GetTree().ChangeSceneToFile("res://scenes/Stage2Levels.tscn"); //Change the scene to Stage2 when the button is pressed
    }

    private void OnStage1Level1Pressed()
    {
        GetTree().ChangeSceneToFile("res://scenes/Stage1Levels/Stage1Level1.tscn"); //Change the scene to Stage1Level1 when the button is pressed
    }

    private void OnStage1Level2Pressed()
    {
        GetTree().ChangeSceneToFile("res://scenes/Stage1Levels/Stage1Level2.tscn"); //Change the scene to Stage1Level2 when the button is pressed
    }

    private void OnStage2Level1Pressed()
    {
        GetTree().ChangeSceneToFile("res://scenes/Stage2Levels/Stage2Level1.tscn"); //Change the scene to Stage2Level1 when the button is pressed
    }
    private void OnStage2Level2Pressed()
    {
        GetTree().ChangeSceneToFile("res://scenes/Stage2Levels/Stage2Level2.tscn"); //Change the scene to Stage2Level2 when the button is pressed
    }

    private void OnBackPressed()
    {
        GetTree().ChangeSceneToFile("res://scenes/MainMenu.tscn"); //Change the scene back to the main menu when the button is pressed
    }
}
