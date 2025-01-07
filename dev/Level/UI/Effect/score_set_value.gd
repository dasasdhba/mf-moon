extends Node

@export_category("ScoreSetValue")
@export var value :int = 100

func set_value(node :Node2D) -> void:
	node.Value = value