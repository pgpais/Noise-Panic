extends Control

@export var nameLabel: Label
@export var iconRect: TextureRect

var steam_id: int

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	Steam.avatar_loaded.connect(on_avatar_loaded)
	print("DISPLAY: _ready called")

	if(!is_multiplayer_authority()):
		rpc_id(get_multiplayer_authority(), "request_initialization", multiplayer.get_unique_id())
		return

	hide();
	update_steam_id(steam_id)

@rpc("any_peer")
func request_initialization(peer_id: int) -> void:
	if(!is_multiplayer_authority()):
		return
	rpc_id(peer_id, "update_steam_id", steam_id)

@rpc("call_local")
func update_steam_id(new_steam_id: int) -> void:
	steam_id = new_steam_id
	Steam.getPlayerAvatar()
	update_name()

func update_name() -> void:
	nameLabel.text = Steam.getPersonaName()

func update_avatar(avatar: ImageTexture) -> void:
	iconRect.texture = avatar

func on_avatar_loaded(user_id: int, avatar_size: int, avatar_buffer: PackedByteArray) -> void:
	print("Avatar for user: %s" % user_id)
	print("Size: %s" % avatar_size)

	# Create the image and texture for loading
	var avatar_image: Image = Image.create_from_data(avatar_size, avatar_size, false, Image.FORMAT_RGBA8, avatar_buffer)

	# Optionally resize the image if it is too large
	if avatar_size > 128:
		avatar_image.resize(128, 128, Image.INTERPOLATE_LANCZOS)

	# Apply the image to a texture
	var avatar_texture: ImageTexture = ImageTexture.create_from_image(avatar_image)

	# Set the texture to a Sprite, TextureRect, etc.
	update_avatar(avatar_texture)
