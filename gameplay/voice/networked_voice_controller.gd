extends Node

var opusencoder : TwovoipOpusEncoder
var audiostreamopus : AudioStreamOpus
var audiostreamplaybackopus : AudioStreamPlaybackOpus
var prepend = PackedByteArray()
var opuspacketsbuffer = [ ]

var voice_data : PackedByteArray = []

func _ready():
	opusencoder = TwovoipOpusEncoder.new()
	audiostreamopus = $VoiceCapture.stream
	$VoiceCapture.play()
	audiostreamplaybackopus = $VoiceCapture.get_stream_playback()
	audiostreamplaybackopus.mark_end_opus_stream(true)

	AudioServer.set_input_device_active(true)
	opusencoder.create_sampler(AudioServer.get_input_mix_rate(), 48000, 2, false)
	opusencoder.create_opus_encoder(48000, 5, true)

	# Voice says: "Listen to me"
	#print("Message length (seconds): ", len(opusaudiodata)*960.0/audiostreamopus.opus_sample_rate)
	#for r in opusaudiodata:
		#opuspacketsbuffer.append(PackedByteArray(r))

func _process(_delta):
	if Input.is_action_pressed("voice"):
		_process_record()
	_process_playback()

var chunkcount = 0
var chunkmax = 0.0
var opus_chunk_size = 960
func _process_record():
	while true:
		var frames = AudioServer.get_input_frames(opusencoder.calc_audio_chunk_size(opus_chunk_size))
		var lchunkmax = opusencoder.process_pre_encoded_chunk(frames, opus_chunk_size, false, false)
		if lchunkmax == -1.0:
			break
		chunkcount += 1
		chunkmax = max(chunkmax, lchunkmax)
		voice_data = opusencoder.encode_chunk(prepend);
		rpc("_send_voice", voice_data)
		if (chunkcount % 50) == 0:
			# prints("audiomax: %.3f  data[%d]: %s" % [chunkmax, voice_data.size(), voice_data.slice(0,5)])
			chunkmax = 0.0

@rpc("any_peer")
func _send_voice(voice_data: PackedByteArray):
	opuspacketsbuffer.append(voice_data)

func _process_playback():
	while opus_chunk_size < audiostreamplaybackopus.available_space_frames() and len(opuspacketsbuffer) != 0:
		audiostreamplaybackopus.push_opus_packet(opuspacketsbuffer.pop_front(), 0, 0)
