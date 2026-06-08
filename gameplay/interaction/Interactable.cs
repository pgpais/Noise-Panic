using Godot;

namespace WeWereDumb.Gameplay.Interaction;

[GlobalClass]
public partial class Interactable : Area3D
{
	const string HIGHLIGHT_MATERIAL_UID = "uid://buvy6b2l6l2sw";

	[Signal] public delegate void InteractionStartedEventHandler(Interactor interactor);
	[Signal] public delegate void InteractionStoppedEventHandler(Interactor interactor);

	[ExportGroup("Highlighting")]
	[Export] Material highlightMaterial;
	[Export] MeshInstance3D mesh;

	public override void _Ready()
	{
		base._Ready();
		if (highlightMaterial == null)
		{
			highlightMaterial = ResourceLoader.Load<Material>(HIGHLIGHT_MATERIAL_UID);
		}

		if (mesh == null)
		{
			mesh = Owner.GetNode<MeshInstance3D>("MeshInstance3D");
		}
	}

	public virtual void ShowHighlight()
	{
		if (mesh != null)
		{
			mesh.MaterialOverlay = highlightMaterial;
		}
	}

	public virtual void HideHighlight()
	{
		if (mesh != null)
		{
			mesh.MaterialOverlay = null;
		}
	}

	public virtual void StartInteraction(Interactor interactor)
	{
		EmitSignal(SignalName.InteractionStarted, interactor);
	}

	public virtual void StopInteraction(Interactor interactor)
	{
		EmitSignal(SignalName.InteractionStopped, interactor);
	}
}
