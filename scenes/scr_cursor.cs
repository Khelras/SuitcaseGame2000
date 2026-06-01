using Godot;
using System;

public partial class scr_cursor : MeshInstance2D
{
    private Vector2 m_MousePos;
    private Vector2 newPosition;
    public override void _Process(double delta) {

        m_MousePos = GetGlobalMousePosition();

        newPosition = m_MousePos;

        Position = newPosition;


    }
    public void OnArea2DEntered(Area2D area) {

        GD.Print("collided");
    }
}
