extends AudioStreamPlayer3D

var audio_stream_opus : AudioStreamOpus
var audio_stream_playback_opus : AudioStreamPlaybackOpus
var opus_chunk_size = 960
var voice_data_buffer = []

func _ready():
	audio_stream_opus = AudioStreamOpus.new()
	stream = audio_stream_opus
	play()

	audio_stream_playback_opus = get_stream_playback()
	audio_stream_playback_opus.mark_end_opus_stream(true)

func _process(delta: float):
	process_playback()

func process_playback():
	while opus_chunk_size < audio_stream_playback_opus.available_space_frames() and len(voice_data_buffer) != 0:
		var packet = voice_data_buffer.pop_front()
		audio_stream_playback_opus.push_opus_packet(packet, 0, 0)


func play_voice_packet(packet: PackedByteArray):
	audio_stream_playback_opus.push_opus_packet(packet, 0, 0)
