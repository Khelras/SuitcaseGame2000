using Godot;
using System;
using System.Threading;

public partial class LabelController : Button
{
    [Export] public Label label;
    [Export] public ColorRect colorRect;
    [Export] public AudioStreamPlayer2D selectMenu;
    [ExportGroup("Label Configuration")]
    [Export] public float ScaleUpSpeed = 0.0f;
    [Export] public float ScaleDownSpeed = 0.0f;
    [ExportGroup("ColorRect Configuration")]
    [Export] public float moveDistance = 0.0f;
    [Export] public float animationDuration = 0.0f;

    private int baseFontSize;
    private Vector2 colorRectStartPos;
    public override void _Ready()
    {
        

        //set the pivot on the label
        label.PivotOffset = label.Size * 0.5f;
        label.Scale = Vector2.One;

        baseFontSize = label.GetThemeFontSize("font_size");

        colorRectStartPos = colorRect.Position;

        FocusEntered += PunchIn;
        FocusExited += PunchOut;

    }
    public override void _Process(double delta)
    {
     
    }
    private void PunchIn() {
        // grow whem mouse enter and basically so that when the scale of the text change it wont ruin the resulotion
        Tween tween = label.CreateTween();
        int targetSize = Mathf.RoundToInt(baseFontSize * 1.5f);
        tween.TweenMethod(Callable.From<int>(SetFontSize), baseFontSize, targetSize, ScaleUpSpeed).SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.Out);

        Tween rectTween = label.CreateTween();
        rectTween.TweenProperty(colorRect, "position:x", colorRectStartPos.X + moveDistance, animationDuration).SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.Out); ;
        label.Set("theme_override_colors/font_color", new Color(0.0f, 0.0f, 0.0f, 1.0f));
        selectMenu.Play();
    }
    private void PunchOut() {

        //Shrink back
        Tween tween = label.CreateTween();
        int currentSize = label.GetThemeFontSize("font_size");
        tween.TweenMethod(Callable.From<int>(SetFontSize), currentSize, baseFontSize, ScaleDownSpeed).SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.In);

        Tween rectTween = label.CreateTween();
        rectTween.TweenProperty(colorRect, "position:x", colorRectStartPos.X, animationDuration).SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.In);
        label.Set("theme_override_colors/font_color", new Color(255.0f, 255.0f, 0.0f, 1.0f));
    }
    
    private void SetFontSize(int size) {

        label.AddThemeFontSizeOverride("font_size", size);
    }
}
