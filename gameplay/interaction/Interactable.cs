using Godot;

namespace WeWereDumb.Gameplay.Interaction;

[GlobalClass]
public partial class Interactable : Area3D
{
	const string HIGHLIGHT_MATERIAL_UID = "uid://buvy6b2l6l2sw";

	[Signal] public delegate void InteractionStartedEventHandler(Interactor interactor);
	[Signal] public delegate void InteractionStoppedEventHandler();

	[Export] bool enabled = true;
	[Export] bool instantUse = true;

	[ExportGroup("Highlighting")]
	[Export] Material highlightMaterial;
	[Export] MeshInstance3D mesh;

	public Interactor Interactor => interactor;
	Interactor interactor;

	public override void _Ready()
	{
		base._Ready();

		highlightMaterial ??= ResourceLoader.Load<Material>(HIGHLIGHT_MATERIAL_UID);
		mesh ??= Owner.GetNode<MeshInstance3D>("MeshInstance3D");
	}

	public virtual void ShowHighlight()
	{
		if (!enabled) return;

		if (mesh != null)
		{
			mesh.MaterialOverlay = highlightMaterial;
		}
	}

	public virtual void HideHighlight()
	{
		if (!enabled) return;

		if (mesh != null)
		{
			mesh.MaterialOverlay = null;
		}
	}

	public virtual void StartInteraction(Interactor interactor)
	{
		if (!enabled) return;

		this.interactor = interactor;
		EmitSignal(SignalName.InteractionStarted, interactor);
		if (instantUse)
		{
			StopInteraction();
		}
	}

	public virtual void StopInteraction()
	{
		if (!enabled) return;

		EmitSignal(SignalName.InteractionStopped);
	}

	public virtual void Enable()
	{
		ProcessMode = ProcessModeEnum.Inherit;
		enabled = true;
	}

	public virtual void Disable()
	{
		ProcessMode = ProcessModeEnum.Disabled;
		enabled = false;
	}
}
