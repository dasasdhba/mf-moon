extends Node
class_name EnemyTurn

# make enemy turn when collide with each other
# the shape should be rectangle without rotation

@export_category("EnemyTurn")
@export var sync_collision_shape :CollisionShape2D

func _ready() -> void:
	if sync_collision_shape == null:
		return
	
	var shape := sync_collision_shape.shape
	if shape == null or shape is not RectangleShape2D:
		return

	var body = StaticBody2D.new()
	body.collision_layer = 536870912 # 30 on only
	body.collision_mask = 0

	var new_shape := RectangleShape2D.new()
	new_shape.size = Vector2(shape.size.y, shape.size.x)
	for i in range(2):
		var new_child := CollisionShape2D.new()
		new_child.position = sync_collision_shape.position
		new_child.one_way_collision = true
		new_child.shape = new_shape
		new_child.rotation = PI / 2 if i == 0 else -PI / 2
		body.add_child(new_child)

	add_sibling.call_deferred(body)
