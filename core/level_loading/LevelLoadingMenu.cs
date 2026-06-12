using Godot;

public partial class LevelLoadingMenu : Control
{
	[Export] ProgressBar progressBar;

	public void SetProgress(float progress)
	{
		progressBar.Value = progress;
	}

	public void Enable()
	{
		Show();
		progressBar.Value = 0;
	}

	public void Disable()
	{
		Hide();
	}
}
