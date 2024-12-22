extends Node

@export_category("ScoreMovement")
@export var move_time := 0.6
@export var move_length := 60.0
@export var keep_time := 1.2
@export var disappear_time := 0.3

func _ready() -> void:
	var p :Node2D = get_parent()
	var tween := create_tween()
	tween.tween_property(p, "position", p.position + move_length * Vector2.UP, move_time)
	tween.tween_interval(keep_time)
	tween.tween_property(p, "modulate:a", 0, disappear_time)
	tween.tween_callback(p.queue_free)