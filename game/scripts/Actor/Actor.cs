// **Actor main class**
// This class is the base class for all actors in the game.
// It does not contain any _Process methods and cannot be used directly.

// FIXME: Player's sprite delayed on directionnal change
// FIXME: Player's sprite is not centered and uses two tiles. Z-Index is currently identical on both tiles, this need to be updated.

using Godot;

public class Actor
{
  public Actor(KinematicBody2D actorNode)
  {
    _actorNode = actorNode;
    _animationTree = _actorNode.GetNode<AnimationTree>("Sprite/AnimationTree");
    _stateMachine = (AnimationNodeStateMachinePlayback)_animationTree.Get("parameters/playback");
    _rayCast = _actorNode.GetNode<RayCast2D>("RayCast2D");
    InitialPosition = _actorNode.Position;
  }

  public float Speed = 6.0f;
  public Vector2 InitialPosition;
  public float MotionProgress;
  public bool IsMoving = false;
  private KinematicBody2D _actorNode;
  private AnimationTree _animationTree;
  private AnimationNodeStateMachinePlayback _stateMachine;
  private RayCast2D _rayCast;

  public enum MovementTypes
  {
    Forward,
    Stand,
    // Jump,
  }

  public void Move(MovementTypes movement, Vector2 direction, float delta)
  {
    switch (movement)
    {
      case MovementTypes.Forward:
        MotionProgress += Speed * delta;
        if (MotionProgress >= 1.0f)
        {
          _actorNode.Position = InitialPosition + (direction * GameVar.TileSize);
          UpdateMovingState(false);
        }
        else
          _actorNode.Position = InitialPosition + (direction * GameVar.TileSize * MotionProgress);
        break;

      case MovementTypes.Stand:
        UpdateMovingState(false);
        break;
    }
  }

  public void Animate(Vector2 direction)
  {
    if (direction == Vector2.Zero)
      _stateMachine.Travel("Idle");
    else
    {
      _animationTree.Set("parameters/Walk/blend_position", direction);
      _animationTree.Set("parameters/Idle/blend_position", direction);
      _stateMachine.Travel("Walk");
    }
  }

  public bool GetCollision(Vector2 direction)
  {
    _rayCast.CastTo = direction * GameVar.TileSizeHalf;
    _rayCast.ForceRaycastUpdate();
    return _rayCast.IsColliding();
  }

  public void UpdateMovingState(bool setter)
  {
    IsMoving = setter;

    if (!IsMoving)
      MotionProgress = 0.0f;
    else
      InitialPosition = _actorNode.Position;
  }
}
