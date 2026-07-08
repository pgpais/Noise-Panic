@tool
extends BTAction
## SetAnimationTreeVariable

@export var animation_tree_path: NodePath
@export var variable_name: String

@export var variable_value: Variant

var animation_tree: AnimationTree

# Display a customized name (requires @tool).
func _generate_name() -> String:
	return "SetAnimationTree " + LimboUtility.decorate_var(variable_name)


# Called once during initialization.
func _setup() -> void:
	animation_tree = agent.get_node(animation_tree_path)


# Called each time this task is entered.
func _enter() -> void:
	pass


# Called each time this task is exited.
func _exit() -> void:
	pass


# Called each time this task is ticked (aka executed).
func _tick(delta: float) -> Status:
	animation_tree.set(variable_name, variable_value)
	return SUCCESS


# Strings returned from this method are displayed as warnings in the behavior tree editor (requires @tool).
func _get_configuration_warnings() -> PackedStringArray:
	var warnings := PackedStringArray()
	return warnings
