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

func process_playback() -> void:
	while playback_has_available_frames() and voice_data_buffer.size() > 0:
		play_voice_packet(voice_data_buffer.pop_front())

func playback_has_available_frames() -> bool:
	return opus_chunk_size < audio_stream_playback_opus.available_space_frames()

func play_voice_packet(packet: PackedByteArray):
	audio_stream_playback_opus.push_opus_packet(packet, 0, 0)

func add_voice_packet(packet: PackedByteArray):
	voice_data_buffer.append(packet)

func play_voice_buffer(buffer):
	while playback_has_available_frames() and buffer.size() > 0:
		play_voice_packet(buffer.pop_front())
