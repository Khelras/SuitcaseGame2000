using Godot;
using System;

public partial class ItemHoverUI : Control
{
	[Export] public Label NameLabel;
	[Export] public Label CatagoryLabel;
	[Export] public Label DescriptionLabel;
    [Export] public AudioStreamPlayer2D sfxShowItem;

    // Track which item owns the display hover UI
    private Item currentItem = null; 

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
	}

	public void ShowItem(Item item)
	{
        currentItem = item;
        NameLabel.Text = item.ItemName;
		CatagoryLabel.Text = item.Catagory.ToString();
		DescriptionLabel.Text = item.ItemDescription;
		sfxShowItem.Play();
        Visible = true;
	}

	public void HideItem(Item item)
	{
        // Only hide if the item requesting the hide is still the active one.
        // If the mouse already moved to another item, currentItem has changed so don't hide.
        if (currentItem == item)
        {
            currentItem = null;
            Visible = false;
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}
}
