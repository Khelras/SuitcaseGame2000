using Godot;
using System;

public partial class Followmouse : MeshInstance2D
{
	private Vector2 m_MousePos;
	private Vector2 newPosition;
	//Called every time the node is added to the scene
	public override void _Ready(){

		GD.Print("Hello world");


	}
	//Updates every frame
	public override void _Process(double delta){

		if (Input.IsMouseButtonPressed(MouseButton.Left)) {
            m_MousePos = GetGlobalMousePosition();

            newPosition = m_MousePos;

            Position = newPosition;
        }
		
		//GD.Print("mousePos X:", m_MousePos.X,"\n", "mousePos Y:", m_MousePos.Y, "\n");
    }
	public void OnAreaEntered(Area2D area) {

		if (area.IsInGroup("cursor")) {

			GD.Print("cursor enter");
		}
	}

	
}
