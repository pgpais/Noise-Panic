extends AudioStreamPlayer3D

var prepend = PackedByteArray()
var opus_encoder : TwovoipOpusEncoder
var opus_packets_buffer = [ ]

var voice_data : PackedByteArray = []

func _ready():
	opus_encoder = TwovoipOpusEncoder.new()

	AudioServer.set_input_device_active(true)
	opus_encoder.create_sampler(AudioServer.get_input_mix_rate(), 48000, 2, false)
	opus_encoder.create_opus_encoder(48000, 5, true)

var chunkcount = 0
var chunkmax = 0.0
var opus_chunk_size = 960
func process_record():
	print("Processing record...")
	while true:
		var frames = AudioServer.get_input_frames(opus_encoder.calc_audio_chunk_size(opus_chunk_size))
		var lchunkmax = opus_encoder.process_pre_encoded_chunk(frames, opus_chunk_size, false, false)
		if lchunkmax == -1.0:
			break
		chunkcount += 1
		chunkmax = max(chunkmax, lchunkmax)
		voice_data = opus_encoder.encode_chunk(prepend);
		if (chunkcount % 50) == 0:
			# prints("audiomax: %.3f  data[%d]: %s" % [chunkmax, voice_data.size(), voice_data.slice(0,5)])
			chunkmax = 0.0

func get_voice_data() -> PackedByteArray:
	if(opus_packets_buffer.size() == 0):
		return PackedByteArray()
	return opus_packets_buffer.pop_front()

func add_voice_data(data: PackedByteArray):
	opus_packets_buffer.append(data)
