extends KinematicBody2D

onready var anim_tree = $PlayerAnimTree
onready var anim_state = $PlayerAnimTree.get("parameters/playback")

export var speed = 100.0

var input_vector = Vector2(0,0)


func _ready():
	anim_tree.active = true
	animate_player()

func _physics_process(_delta: float) -> void:
	input_vector = Vector2(
		Input.get_action_strength("ui_right") - Input.get_action_strength("ui_left"),
		Input.get_action_strength("ui_down") - Input.get_action_strength("ui_up")
	)
	move_player()
	animate_player()


func move_player() -> void:
	var _player_motion = move_and_slide(input_vector.normalized() * speed)

func animate_player() -> void:
	if input_vector == Vector2(0,0):
		anim_state.travel("Idle")
	else:
		anim_tree.set("parameters/Walk/blend_position", input_vector)
		anim_tree.set("parameters/Idle/blend_position", input_vector)
		anim_state.travel("Walk")