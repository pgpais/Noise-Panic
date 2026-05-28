extends Node


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	var initialize_response: Dictionary = Steam.steamInitEx( 480, true )
	print("Did Steam initialize?: %s " % initialize_response)
	
	Steam.join_requested.connect(on_join_requested)
	Steam.lobby_created.connect(on_lobby_created)
	
	Steam.lobby_match_list.connect(on_lobby_match_list)
	print("Requesting a lobby list")
	Steam.requestLobbyList()

func on_join_requested(lobby_id:int, steam_id:int):
	join_lobby(lobby_id)

func on_lobby_match_list(lobbies: Array) -> void:
	for lobby in lobbies:
		print("lobby: ", lobby)

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	Steam.run_callbacks();

func host_server(port: int = 0) -> Error:
	var peer : MultiplayerPeer = SteamMultiplayerPeer.new();
	var result = peer.create_host(0);
	if result == Error.OK:
		multiplayer.multiplayer_peer = peer;
		Steam.createLobby(Steam.LOBBY_TYPE_PUBLIC)
		NetworkManager.emit_signal("ServerHosted")
	return result

func on_lobby_created(connect: int, lobby_id: int):
	print("lobby created: ", connect, " " ,lobby_id)
	
func join_lobby(lobby_id: int, port: int = 0) -> void:
	Steam.lobby_joined.connect(on_lobby_joined)
	Steam.joinLobby(lobby_id);
	#var result = peer.connect_to_lobby(int(lobby_id))
	#print(result);
	#return result;

func on_lobby_joined(lobby_id: int, permissions: int, locked: bool, response: int):
	print("lobby joined: ", lobby_id, " ", permissions, " ", response)
	
	var peer : MultiplayerPeer = SteamMultiplayerPeer.new();
	var steam_id = Steam.getLobbyOwner(int(lobby_id));
	peer.create_client(steam_id);
	multiplayer.multiplayer_peer = peer;
