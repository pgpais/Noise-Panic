#*
#* face_target3D.gd
#* =============================================================================
#* Copyright (c) 2023-present Serhii Snitsaruk and the LimboAI contributors.
#*
#* Use of this source code is governed by an MIT-style
#* license that can be found in the LICENSE file or at
#* https://opensource.org/licenses/MIT.
#* =============================================================================
#*
@tool
extends BTAction
## Flips the agent to face the target, returning [code]SUCCESS[/code]. [br]
## Returns [code]FAILURE[/code] if [member target_var] is not a valid [Node2D] instance.

## Blackboard variable that stores our target (expecting Node2D).
@export var target_var: StringName = &"target"
@export var character_controller_path: NodePath

var character_controller: CharacterController

func _setup() -> void:
	character_controller = agent.get_node(character_controller_path)

# Display a customized name (requires @tool).
func _generate_name() -> String:
	return "FaceTarget " + LimboUtility.decorate_var(target_var)


# Called each time this task is ticked (aka executed).
func _tick(_delta: float) -> Status:
	var target: Node3D = blackboard.get_var(target_var)
	if not is_instance_valid(target):
		return FAILURE
	
	#agent.face_dir(dir)
	character_controller.LookAt(Vector3(target.global_position.x, agent.global_position.y, target.global_position.z))
	
	
	return SUCCESS
