extends Node
class_name MarkNode

@export_category("MarkNode")
@export var debug_show :bool = false

func _ready() -> void:
	if !OS.is_debug_build():
		get_parent().queue_free()
		return

	get_parent().visible = debug_show

func _input(event: InputEvent) -> void:
	if !OS.is_debug_build():
		return

	if event is InputEventKey:
		if event.keycode == KEY_DELETE && event.is_released():
			var p = get_parent()
			p.visible = !p.visible
