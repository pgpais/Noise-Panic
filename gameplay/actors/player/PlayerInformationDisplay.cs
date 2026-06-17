using Godot;

public partial class PlayerInformationDisplay : Control
{
	[Export] Label nameLabel;
	[Export] TextureRect iconRect;


	override public void _Ready()
	{
		int peerId = GetMultiplayerAuthority();

		if (IsMultiplayerAuthority())
		{
			Hide();
		}

		SetPlayerInfo(peerId.ToString());
	}

	public void SetPlayerInfo(string name, Texture2D icon = null)
	{
		nameLabel.Text = name;
		iconRect.Texture = icon ?? new PlaceholderTexture2D();
	}
}
