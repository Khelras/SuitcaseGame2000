using Godot;
using System;

public partial class DragAndDrop : Sprite2D
{
    private bool dragging = false;
    private Vector2 offset;
    private Area2D area;
    public override void _Ready() {
        area = GetNode<Area2D>("Area2D");
        area.InputEvent += OnInputsEvent;
    }
    public override void _Process(double delta) {
        if (dragging) {
            Position = GetGlobalMousePosition() - offset;
        }

    }
    private void OnInputsEvent(Node vieport, InputEvent @event, long shapeIdx) {
        if (@event is InputEventMouseButton mouseButton) {
            if (mouseButton.ButtonIndex == MouseButton.Left) {
                if (mouseButton.Pressed == true)
                {
                    dragging = true;
                    GD.Print("Drag Pressed.");

                    // Offset to Drag based from where the Mouse Initially Clicked instead of the Origin of the Sprite.
                    offset = GetGlobalMousePosition() - GlobalPosition;
                }
                else {
                    dragging = false;
                    GD.Print("Drag Released.");
                }
            }
        }
      

    }
    public bool Getdragging() {

        return dragging;
    }
  
 
}
