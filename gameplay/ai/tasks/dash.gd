@tool
extends BTAction
## Dash

@export var dash_time: float = 5

@export var character_controller_path: NodePath
@export var hurtbox_path: NodePath

var character_controller: FlyingCharacterController
var hurtbox: Hurtbox

var hit: bool = false

# Display a customized name (requires @tool).
func _generate_name() -> String:
	return "Charge Attack"


# Called once during initialization.
func _setup() -> void:
	character_controller = agent.get_node(character_controller_path)
	hurtbox = agent.get_node(hurtbox_path)


# Called each time this task is entered.
func _enter() -> void:
	character_controller.StartDash(20, dash_time)
	hit = false;
	hurtbox.Hit.connect(_on_hit)


# Called each time this task is exited.
func _exit() -> void:
	character_controller.StopDash()
	hurtbox.Hit.disconnect(_on_hit)


# Called each time this task is ticked (aka executed).
func _tick(delta: float) -> Status:
	if hit:
		return SUCCESS
	return RUNNING

func _on_hit():
	hit = true;
	return SUCCESS

# Strings returned from this method are displayed as warnings in the behavior tree editor (requires @tool).
func _get_configuration_warnings() -> PackedStringArray:
	var warnings := PackedStringArray()
	return warnings
