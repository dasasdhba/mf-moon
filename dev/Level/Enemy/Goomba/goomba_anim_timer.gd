extends Timer

@export_category("GoombaAnimTimer")
@export var disabled := false

@onready var anim :AnimatedSprite2D = get_parent()


func _on_timeout() -> void:
	if disabled:
		return

	anim.flip_h = !anim.flip_h
	anim.position.x = 0.0 if anim.flip_h else 0.5
