#*
#* sine_flying_Movement.gd
#*
@tool
extends BTAction
## Applies velocity in the direction the agent is facing on each tick
## until the [member duration] is exceeded. [br]
## Returns [code]SUCCESS[/code] if the elapsed time exceeds [member duration]. [br]
## Returns [code]RUNNING[/code] if the elapsed time does not exceed [member duration]. [br]

@export var pattern_speed: float
@export var pattern_radius: float

@export var character_controller_path: NodePath

var character_controller: FlyingCharacterController
var angle = 0;

func _setup() -> void:
	character_controller = agent.get_node(character_controller_path)

# Display a customized name (requires @tool).
func _generate_name() -> String:
	return "SineFlyingMovement"

# Called each time this task is ticked (aka executed).
func _tick(_delta: float) -> Status:
	angle += pattern_speed
	var dir = Vector3(cos(angle), sin(angle) / 2, sin(angle))
	character_controller.SetAbsoluteMovementInput(dir.normalized())
	
	return SUCCESS
