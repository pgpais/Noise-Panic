@tool
extends BTAction
## WaitForHit
## @shadow_zerkling: Eu sou o Shadow mais conhecido como G-man e isto é meu código

@export var hurtbox_path: NodePath

var hurtbox: Hurtbox

# Display a customized name (requires @tool).
func _generate_name() -> String:
	return "WaitForHit - "


# Called once during initialization.
func _setup() -> void:
	hurtbox = agent.get_node(hurtbox_path)


# Called each time this task is entered.
func _enter() -> void:
	hurtbox.HitHitbox.connect(_on_hit)
	pass


# Called each time this task is exited.
func _exit() -> void:
	hurtbox.HitHitbox.disconnect(_on_hit)
	pass


# Called each time this task is ticked (aka executed).
func _tick(delta: float) -> Status:
	return RUNNING

func _on_hit(hitbox):
	return SUCCESS

# Strings returned from this method are displayed as warnings in the behavior tree editor (requires @tool).
func _get_configuration_warnings() -> PackedStringArray:
	var warnings := PackedStringArray()
	return warnings
