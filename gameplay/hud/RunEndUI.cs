using Godot;

public partial class RunEndUI : Control
{
	[Export] Control victoryUI;
	[Export] Control defeatUI;

	[Export] Button quitButton;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Hide();

		EventBus.Instance.RunEnded += OnRunEnded;
		quitButton.Pressed += () => { GetTree().Quit(); };
	}

	private void OnRunEnded(bool success)
	{
		Show();
		if (success)
		{
			ShowVictoryUI();
		}
		else
		{
			ShowDefeatUI();
		}
		Input.MouseMode = Input.MouseModeEnum.Visible;
	}

	private void ShowDefeatUI()
	{
		defeatUI.Show();
		victoryUI.Hide();
	}

	private void ShowVictoryUI()
	{
		victoryUI.Show();
		defeatUI.Hide();
	}
}
