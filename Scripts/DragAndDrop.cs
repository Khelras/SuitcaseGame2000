using Godot;
using System;

public partial class DragAndDrop : Sprite2D
{
    private bool dragging = false;
    
    private Vector2 Offset;
    private Area2D area;
    public override void _Ready() {
        area = GetNode<Area2D>("Area2D");
        area.InputEvent += OnInputsEvent;
    }
    public override void _Process(double delta) {

        if (dragging) {
            Position = GetGlobalMousePosition() - Offset;
        }

    }
    private void OnInputsEvent(Node vieport, InputEvent @event, long shapeIdx) {
        if (@event is InputEventMouseButton mouseButton) {
            if (mouseButton.ButtonIndex == MouseButton.Left) {

                if (mouseButton.Pressed)
                {
                    dragging = true;
                    GD.Print("Clicking");
                    Offset = GetGlobalMousePosition() - GlobalPosition;
                }
                else {
                    dragging = false;
                    GD.Print("StoppedDragging");
                }
            }
        }
      

    }
    public bool Getdragging() {

        return dragging;
    }
  
 
}
