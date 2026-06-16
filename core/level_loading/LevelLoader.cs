using Godot;
using Godot.Collections;
using System;

[GlobalClass]
public partial class LevelLoader : Node
{
	[Signal] public delegate void LevelLoadedEventHandler(Node level);

	[Export] LevelLoadingMenu _levelLoadingMenu;
	[Export] Node3D levelsParent;

	private string currentLoadingLevelPath;

	private bool isLoading = false;
	private bool isSynced = true;

	private Dictionary<int, bool> peerReadyStates = [];

	override public void _Ready()
	{
		ClearLevels();
	}

	override public void _Process(double delta)
	{
		if (isLoading)
		{
			var progress = GetLoadProgress();
			GD.Print("[LevelLoader]" + Multiplayer.GetUniqueId() + ": " + "Load progress: " + progress);
			_levelLoadingMenu.SetProgress(progress);
			if (progress >= 1.0f)
			{
				OnLoadComplete();
			}
		}
	}

	public void LoadLevelRequest(string levelScenePath, bool clearLevels = true, bool isSynced = true)
	{
		this.isSynced = isSynced;
		GD.Print("[LevelLoader]" + Multiplayer.GetUniqueId() + ": " + "Requesting level: " + levelScenePath + " isSynced: " + isSynced);
		if (!isSynced)
		{
			LoadLevel(levelScenePath, clearLevels);
			return;
		}

		if (!IsMultiplayerAuthority()) return;

		foreach (var peerId in Multiplayer.GetPeers())
		{
			peerReadyStates[peerId] = false;
		}

		Rpc(MethodName.LoadLevel, levelScenePath, clearLevels);
	}

	[Rpc(CallLocal = true)]
	private void LoadLevel(string levelScenePath, bool clearLevels = true)
	{
		GD.Print("[LevelLoader]" + Multiplayer.GetUniqueId() + ": " + "Loading level: " + levelScenePath);
		if (clearLevels)
			ClearLevels();

		_levelLoadingMenu.Enable();
		isLoading = true;
		levelsParent.ProcessMode = ProcessModeEnum.Disabled;

		//TODO: a bit dumb to load the PackedScene again. But we can instead use multithreading so structure can be like this
		currentLoadingLevelPath = levelScenePath;
		ResourceLoader.LoadThreadedRequest(currentLoadingLevelPath);
	}

	public float GetLoadProgress()
	{
		var progress = new Godot.Collections.Array();
		var status = ResourceLoader.LoadThreadedGetStatus(currentLoadingLevelPath, progress);
		if (status == ResourceLoader.ThreadLoadStatus.InProgress)
		{
			return (float)progress[0];
		}
		return 1.0f;
	}

	public void OnLoadComplete()
	{
		if (ResourceLoader.LoadThreadedGet(currentLoadingLevelPath) is PackedScene packedScene)
		{
			GD.Print("[LevelLoader]" + Multiplayer.GetUniqueId() + ": " + "Load complete: " + currentLoadingLevelPath);
			var instance = packedScene.Instantiate();
			levelsParent?.AddChild(instance);
			EmitSignal(SignalName.LevelLoaded, instance);
			if (isSynced)
			{
				Rpc(MethodName.PeerReady, Multiplayer.GetUniqueId());
			}
			else
			{
				FinishLoad();
			}
		}
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	private void PeerReady(int peerId)
	{
		if (!IsMultiplayerAuthority()) return;

		peerReadyStates[peerId] = true;
		if (AreAllPeersReady())
		{
			GD.Print("[LevelLoader]" + Multiplayer.GetUniqueId() + ": " + "All peers ready: " + currentLoadingLevelPath);
			Rpc(MethodName.FinishLoad);
		}
	}

	private bool AreAllPeersReady()
	{
		foreach (bool ready in peerReadyStates.Values)
		{
			if (!ready)
				return false;
		}
		return true;
	}


	[Rpc(CallLocal = true)]
	private void FinishLoad()
	{
		GD.Print("[LevelLoader]" + Multiplayer.GetUniqueId() + ": " + "Finish load: " + currentLoadingLevelPath);
		_levelLoadingMenu.Disable();
		isLoading = false;
		levelsParent.ProcessMode = ProcessModeEnum.Inherit;
	}

	private void ClearLevels()
	{
		if (levelsParent != null)
		{
			foreach (Node child in levelsParent.GetChildren())
			{
				child.QueueFree();
			}
		}
	}
}
