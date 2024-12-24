extends Node

@export_category("GoombaStompedDisappear")
@export var keep_time := 4.0
@export var disappear_time := 0.5

func _ready() ->void:
	var p :Node2D = get_parent()
	var t := create_tween()
	t.tween_interval(keep_time)
	t.tween_property(p, "modulate:a", 0, disappear_time)
	t.tween_callback(p.queue_free)