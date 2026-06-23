using Godot;
using System;

public partial class ItemHoverUI : Control
{
	[Export] public Label NameLabel;
	[Export] public Label CatagoryLabel;
	[Export] public Label DescriptionLabel;
    private AudioStreamPlayer2D sfxShowItem;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		Visible = false;
        sfxShowItem = GetNode<AudioStreamPlayer2D>("ShowItem");
		FontFile customFont = GD.Load<FontFile>("res://Fonts/KiriFont.otf");
		// Creating a new labelSetting instance and apply the font
        NameLabel.LabelSettings = new LabelSettings();
        CatagoryLabel.LabelSettings = new LabelSettings();
        DescriptionLabel.LabelSettings = new LabelSettings();
		NameLabel.LabelSettings.Font = customFont;
        CatagoryLabel.LabelSettings.Font = customFont;
        DescriptionLabel.LabelSettings.Font = customFont;
    }

	public void ShowItem(Item item)
	{
		NameLabel.Text = item.ItemName;
		CatagoryLabel.Text = item.Catagory.ToString();
		DescriptionLabel.Text = item.ItemDescription;
		sfxShowItem.Play();
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
