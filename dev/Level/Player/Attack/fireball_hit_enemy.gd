extends Node

func _on_attacker_attacked(enemy :Node, respond :int) -> void:
	if respond == 0:
		get_parent().PlayOneshot()
