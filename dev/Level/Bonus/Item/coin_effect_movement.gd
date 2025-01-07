extends Node

@export_category("CoinEffectMovement")
@export var jump_speed :float = 250.0
@export var acc :float = 500.0
@export var direction :Vector2 = Vector2.DOWN

var parent :Node2D
var speed :float = 0.0

func _ready() -> void:
	parent = get_parent()
	speed = -jump_speed

func _process(delta: float) -> void:
	speed += acc * delta
	if speed > 0.0:
		speed = 0.0
	
	parent.position += direction * speed * delta
