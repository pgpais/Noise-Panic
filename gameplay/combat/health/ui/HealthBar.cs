using System;
using Godot;

/// <summary>
/// A ProgressBar kept in sync with a HealthComponent.
/// Value changes animate smoothly rather than jumping instantly.
/// </summary>
public partial class HealthBar : ProgressBar
{
	[Export] HealthComponent healthComponent;
	[Export] private ProgressBar _backgroundBar;

	private Tween _tween;

	public override void _Ready()
	{
		MaxValue = healthComponent.MaxHealth;
		Value = healthComponent.CurrentHealth;

		_backgroundBar.MaxValue = MaxValue;
		_backgroundBar.Value = Value;

		healthComponent.HealthChanged += OnHealthChanged;
	}

	private void OnHealthChanged(int newHealth)
	{
		MaxValue = healthComponent.MaxHealth; //TODO: also tween

		_tween?.Kill();
		_tween = CreateTween();

		var foregroundTween = CreateTween();
		foregroundTween.SetParallel(true);
		// Make the bar scale up slightly to emphasize the change, then spring back to normal.
		Scale = new Vector2(1.15f, 1.15f);
		foregroundTween.TweenProperty(this, "scale", Vector2.One, 0.3)
			.SetTrans(Tween.TransitionType.Spring)
			.SetEase(Tween.EaseType.Out);

		// Make the foreground bar tween to the new health value.
		foregroundTween.TweenProperty(this, "value", (double)newHealth, 0.15)
			.SetTrans(Tween.TransitionType.Sine)
			.SetEase(Tween.EaseType.Out);

		// also tween the background bar to create a "trailing" effect after some delay.
		var backgroundTween = CreateTween();
		backgroundTween.TweenInterval(1);
		backgroundTween.TweenProperty(_backgroundBar, "value", (double)newHealth,
			0.25)
			.SetTrans(Tween.TransitionType.Sine)
			.SetEase(Tween.EaseType.Out);

		_tween.TweenSubtween(foregroundTween);
		_tween.TweenSubtween(backgroundTween);
	}
}
