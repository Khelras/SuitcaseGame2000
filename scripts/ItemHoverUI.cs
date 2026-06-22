using Godot;
using System;

public partial class ItemHoverUI : Control
{
	[Export] public Label NameLabel;
	[Export] public Label CatagoryLabel;
	[Export] public Label DescriptionLabel;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Visible = false;
	}

	public void ShowItem(Item item)
	{
		NameLabel.Text = item.ItemName;
		CatagoryLabel.Text = item.Catagory.ToString();
		DescriptionLabel.Text = item.ItemDescription;

		Visible = true;
	}

	public void HideItem(Item item)
	{
		Visible = false;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}
}
