using Godot;
using System;
using System.Collections.Generic;

public partial class Items : Node2D
{
  
   
   

    public List<Sprite2D> orderSprites;
    private int CurrentIndex;
    public override void _Ready()
    {
        //Gets the sprites into index order 
        orderSprites = new List<Sprite2D>();
        orderSprites.Add(GetNode<Sprite2D>("Photo"));
        orderSprites.Add(GetNode<Sprite2D>("FamilyPhoto"));
        orderSprites.Add(GetNode<Sprite2D>("Shirt"));

      
   
    }
  
    public void SetItem(int index) {
        //make all sprites not visible
        foreach (Sprite2D sprite in orderSprites)
        {
            sprite.Visible = false;
        }
        //looks in the which Index to be visble
        orderSprites[index].Visible = true; 
       

    }
    public void RemoveItem(int index) {
        //check if the index in valid
        if (index >= 0 && index < orderSprites.Count) {

            orderSprites[index].Visible = true;

            orderSprites.RemoveAt(index);
        }
    }
}
