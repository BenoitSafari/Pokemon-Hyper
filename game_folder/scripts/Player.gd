extends KinematicBody2D

onready var anim_tree = $PlayerAnimTree
onready var anim_state = $PlayerAnimTree.get("parameters/playback")
onready var ray_cast = $RayCast2D

export var speed = 5.0
const TILE_SIZE = 16

var input_vector = Vector2(0,0)
var initial_position = Vector2(0,0)
var motion_progress: float = 0.0
var motion_strength: int = 0
var is_moving = false


func _ready():
	anim_tree.active = true
	initial_position = position
	animate_player()

func _physics_process(delta: float) -> void:
	get_motion_strength()
	if !is_moving:
		get_input()
		if input_vector != Vector2(0,0):
			initial_position = position
			is_moving = true
	elif input_vector != Vector2(0,0) && motion_strength > 3:
		move_player(delta)
	else: 
		is_moving = false
	animate_player()


func move_player(delta) -> void:
	if get_collision():
		motion_progress += speed * delta
		if motion_progress >= 1.0:
			position = initial_position + (input_vector * TILE_SIZE)
			motion_progress = 0.0
			is_moving = false
		else:
			position = initial_position + (input_vector * TILE_SIZE * motion_progress)
	else:
		is_moving = false
		motion_progress = 0.0

func animate_player() -> void:
	if input_vector == Vector2(0,0):
		anim_state.travel("Idle")
	else:
		anim_tree.set("parameters/Walk/blend_position", input_vector)
		anim_tree.set("parameters/Idle/blend_position", input_vector)
		anim_state.travel("Walk")

func get_collision() -> bool:
	ray_cast.cast_to = input_vector * TILE_SIZE / 2
	ray_cast.force_raycast_update()
	if !ray_cast.is_colliding():
		return true
	else:
		return false

func get_input() -> void:
	if input_vector.y == 0:
		input_vector.x = Input.get_action_strength("ui_right") - Input.get_action_strength("ui_left")
	if input_vector.x == 0:
		input_vector.y = Input.get_action_strength("ui_down") - Input.get_action_strength("ui_up")

func get_motion_strength() -> void:
	if input_vector != Vector2(0,0) && motion_strength < 4:
		motion_strength += 1
	elif input_vector == Vector2(0,0) && motion_strength > 0:
		motion_strength -= 1
