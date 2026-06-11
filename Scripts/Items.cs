using Godot;
using System;
using System.Collections.Generic;

public partial class Items : Node2D
{
  
    private Dictionary<SetItems, Sprite2D> sprites;
    public enum SetItems {Box, Circle, Cat,  }
    public override void _Ready()
    {
        sprites = new Dictionary<SetItems, Sprite2D>
        {
            {SetItems.Box, GetNode<Sprite2D>("Box") },
            {SetItems.Circle, GetNode<Sprite2D>("Circle") },
            {SetItems.Cat, GetNode<Sprite2D>("Cat") },
        };
        foreach (Sprite2D sprite in sprites.Values) { 
            sprite.Visible = false;
        }
   
    }
  
    public void SetType(SetItems type) {
        //make all sprites not visible
        foreach (Sprite2D sprite in sprites.Values)
        {
            sprite.Visible = false;
        }
        // 
        sprites[type].Visible = true; 
       

    }
}
