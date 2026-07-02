#*
#* get_first_in_group.gd
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
## Stores the first node in the [member group] on the blackboard, returning [code]SUCCESS[/code]. [br]
## Returns [code]FAILURE[/code] if the group contains 0 nodes.

## Name of the SceneTree group.
@export var group: StringName

## Blackboard variable in which the task will store the acquired node.
@export var output_var: StringName = &"target"


func _generate_name() -> String:
	return "GetClosestNodeInGroup \"%s\"  ➜%s" % [
		group,
		LimboUtility.decorate_var(output_var)
		]

func _tick(_delta: float) -> Status:
	var nodes: Array[Node] = agent.get_tree().get_nodes_in_group(group)
	if nodes.size() == 0:
		print("No nodes in group")
		return FAILURE
	
	
	var closest_node = null
	var closest_dist: int = INT64_MAX
	for node in nodes:
		if !closest_node:
			closest_node = node
			closest_dist = agent.global_position.distance_to(node.global_position)
			continue
		
		var node_dist = agent.global_position.distance_to(node.global_position)
		if node_dist < closest_dist:
			closest_dist = node_dist
			closest_node = node

	blackboard.set_var(output_var, closest_node)
	return SUCCESS
