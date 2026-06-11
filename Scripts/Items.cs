using Godot;
using System;
using System.Collections.Generic;

public partial class Items : Node2D
{
  
   
   

    public List<Sprite2D> orderSprites;
    private int CurrentIndex;
    public override void _Ready()
    {
        // this gets the ket which is the enum and the value which retrive the Sprite2D from the scene

        orderSprites = new List<Sprite2D>();
        orderSprites.Add(GetNode<Sprite2D>("Box"));
        orderSprites.Add(GetNode<Sprite2D>("Circle"));
        orderSprites.Add(GetNode<Sprite2D>("Cat"));

      
   
    }
  
    public void SetType(int index) {
        //make all sprites not visible
        foreach (Sprite2D sprite in orderSprites)
        {
            sprite.Visible = false;
        }
        //looks in the dictionary which enum value and set it to Visible
        orderSprites[index].Visible = true; 
       

    }
}
