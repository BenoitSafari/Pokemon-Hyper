using Godot;

// FIXME: Player's sprite delayed on directionnal change
// FIXME: Player's sprite is not centered and uses two tiles. Z-Index is currently identical on both tiles, this need to be updated.

public class Player : KinematicBody2D
{
  [Signal]
  public delegate void PlayerIsMoving();

  [Signal]
  public delegate void PlayerIsStandingStill();

  [Signal]
  public delegate void PlayerCollidedWithDoor();

  [Export]
  public float Speed = 6.0f;

  public bool IsInputLocked = false;

  private AnimationTree _animationTree;
  private AnimationNodeStateMachinePlayback _stateMachine;
  private RayCast2D _rayCastDoor;
  private RayCast2D _rayCastBump;
  private Vector2 _inputVector;
  private Vector2 _initialPosition;
  private float _motionProgress;
  private int _motionStrength;
  private const int _tileSize = 16;
  private bool _isMoving = false;

  private enum CollisionType
  {
    Void,
    Bump,
    Door
  }

  private enum Move
  {
    Forward,
    Stand
  }

  public override void _Ready()
  {
    _animationTree = GetNode<AnimationTree>("AnimationTree");
    _animationTree.Active = true;
    _stateMachine = (AnimationNodeStateMachinePlayback)_animationTree.Get("parameters/playback");
    _rayCastDoor = GetNode<RayCast2D>("RayCastDoor");
    _rayCastBump = GetNode<RayCast2D>("RayCastBump");
    _initialPosition = Position;
    AnimatePlayer();
  }

  public override void _PhysicsProcess(float delta)
  {
    // DEBUG LOGS
    // GD.Print("Player pos:" + Position);
    // GD.Print("_motionProgress:" + _motionProgress);

    GetMotionStrength();
    if (!_isMoving && !IsInputLocked)
    {
      GetInput();
      if (_inputVector != Vector2.Zero)
      {
        _initialPosition = Position;
        _isMoving = true;
      }
    }
    else if (_inputVector != Vector2.Zero && _motionStrength > 3)
    {
      switch (GetCollision())
      {
        case CollisionType.Door:
          EmitSignal(nameof(PlayerCollidedWithDoor));
          MovePlayer(Move.Forward, delta);
          break;

        case CollisionType.Bump:
          MovePlayer(Move.Stand, delta);
          break;

        case CollisionType.Void:
          EmitSignal(nameof(PlayerIsMoving));
          MovePlayer(Move.Forward, delta);
          break;
      }
    }
    else
    {
      EmitSignal(nameof(PlayerIsStandingStill));
      MovePlayer(Move.Stand, delta);
    }

    AnimatePlayer();
  }

  private void MovePlayer(Move enumerator, float delta)
  {
    switch (enumerator)
    {
      case Move.Forward:
        _motionProgress += Speed * delta;
        if (_motionProgress >= 1.0f)
        {
          Position = _initialPosition + (_inputVector * _tileSize);
          _motionProgress = 0.0f;
          _isMoving = false;
        }
        else
          Position = _initialPosition + (_inputVector * _tileSize * _motionProgress);
        break;

      case Move.Stand:
        _motionProgress = 0.0f;
        _isMoving = false;
        break;
    }
  }

  private void AnimatePlayer()
  {
    if (_inputVector == Vector2.Zero)
      _stateMachine.Travel("Idle");
    else
    {
      _animationTree.Set("parameters/Walk/blend_position", _inputVector);
      _animationTree.Set("parameters/Idle/blend_position", _inputVector);
      _stateMachine.Travel("Walk");
    }
  }

  private CollisionType GetCollision()
  {
    _rayCastBump.CastTo = _inputVector * _tileSize / 2;
    _rayCastDoor.ForceRaycastUpdate();
    _rayCastBump.ForceRaycastUpdate();
    if (_rayCastDoor.IsColliding())
      return CollisionType.Door;
    else if (_rayCastBump.IsColliding())
      return CollisionType.Bump;
    else
      return CollisionType.Void;
  }

  private void GetInput()
  {
    if (_inputVector.y == 0)
      _inputVector.x = Input.GetActionStrength("ui_right") - Input.GetActionStrength("ui_left");
    if (_inputVector.x == 0)
      _inputVector.y = Input.GetActionStrength("ui_down") - Input.GetActionStrength("ui_up");
  }

  private void GetMotionStrength()
  {
    if (_inputVector != Vector2.Zero && _motionStrength < 4)
      _motionStrength++;
    else if (_inputVector == Vector2.Zero && _motionStrength > 0)
      _motionStrength--;
  }
}
