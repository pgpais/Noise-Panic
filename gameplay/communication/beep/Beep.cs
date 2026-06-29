using Godot;

namespace Gameplay.Communication.Beep;

public partial class Beep : Node3D
{
	[ExportGroup("Parameters")]
	[ExportSubgroup("Active")]
	[Export] Color beepActiveColor;
	[Export] float beepActiveEmission;
	[ExportSubgroup("Inactive")]
	[Export] Color beepInactiveColor;
	[Export] float beepInactiveEmission;
	[Export] float deactivationDuration = 0.5f;
	
	[ExportGroup("References")]
	[Export] AudioStreamPlayer audioStreamPlayer;
	[Export] StandardMaterial3D beepMaterial;

	private bool _isActive = false;

	private Callable setEmission;
	private Callable setEnergyMultiplier;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		beepMaterial.SetEmission(beepInactiveColor);
		beepMaterial.SetEmissionEnergyMultiplier(beepInactiveEmission);
		
		setEmission = Callable.From((Color color) => beepMaterial.Emission = color);
		setEnergyMultiplier = Callable.From((float energy) => beepMaterial.EmissionEnergyMultiplier = energy);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (!IsMultiplayerAuthority()) return;
		
		if (Input.IsActionJustPressed("beep"))
		{
			Activate();
		}
	}

	public void Activate()
	{
		if (!IsMultiplayerAuthority()) return;
		if (_isActive) return;
		
		audioStreamPlayer.Play();
		
		// var tween = ActivationTween();
		// tween.Finished += Deactivate;
		//
		// tween.Play();
		
		beepMaterial.Emission = beepActiveColor;
		beepMaterial.EmissionEnergyMultiplier = beepActiveEmission;
		
		_isActive = true;
		Deactivate();
	}

	public void Deactivate()
	{
		if (!IsMultiplayerAuthority()) return;
		if (!_isActive) return;
		
		var tween = DeactivationTween();
		tween.Play();
		_isActive = false;
	}

	private Tween ActivationTween()
	{
		var tween = CreateTween();
		tween.SetParallel();
		tween.TweenMethod(setEmission, beepMaterial.Emission, beepActiveColor, 0.2f);
		tween.TweenMethod(setEnergyMultiplier, beepMaterial.EmissionEnergyMultiplier, beepActiveEmission, 0.2f);
		tween.SetEase(Tween.EaseType.Out);
		tween.SetTrans(Tween.TransitionType.Cubic);

		return tween;
	} 
	
	private Tween DeactivationTween()
	{
		var tween = CreateTween();
		tween.SetParallel();
		tween.TweenMethod(setEmission, beepMaterial.Emission, beepInactiveColor, deactivationDuration);
		tween.TweenMethod(setEnergyMultiplier, beepMaterial.EmissionEnergyMultiplier, beepInactiveEmission, deactivationDuration);

		return tween;
	}
	
	
}