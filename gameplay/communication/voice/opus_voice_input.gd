extends Node
#@pedrosa99i: Não mexer. Ninguém sabe o que acontece.
var prepend = PackedByteArray()
var opus_encoder : TwovoipOpusEncoder
var voice_data_buffer = [ ]

var voice_data_packet : PackedByteArray = []

func _ready():
	opus_encoder = TwovoipOpusEncoder.new()

	AudioServer.set_input_device_active(true)
	opus_encoder.create_sampler(AudioServer.get_input_mix_rate(), 48000, 2, false)
	opus_encoder.create_opus_encoder(48000, 5, true)

var chunkcount = 0
var chunkmax = 0.0
var opus_chunk_size = 960
func process_record():
	voice_data_buffer = [ ]
	print("Processing record...")
	while true:
		var frames = AudioServer.get_input_frames(opus_encoder.calc_audio_chunk_size(opus_chunk_size))
		var lchunkmax = opus_encoder.process_pre_encoded_chunk(frames, opus_chunk_size, false, false)
		if lchunkmax == -1.0:
			break
		chunkcount += 1
		chunkmax = max(chunkmax, lchunkmax)
		voice_data_packet = opus_encoder.encode_chunk(prepend);
		voice_data_buffer.append(voice_data_packet)
		if (chunkcount % 50) == 0:
			# prints("audiomax: %.3f  data[%d]: %s" % [chunkmax, voice_data_packet.size(), voice_data_packet.slice(0,5)])
			chunkmax = 0.0

func has_voice_data() -> bool:
	return len(voice_data_buffer) != 0

func get_voice_data_buffer() -> Array:
	return voice_data_buffer

@rpc()
func receive_voice_data(data: PackedByteArray):
	voice_data_buffer.append(data)
