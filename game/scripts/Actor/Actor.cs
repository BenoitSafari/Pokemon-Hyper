// **Actor main class**
// This class is the base class for all actors in the game.
// It does not contain any _Process methods and should not be used directly.

// TODO: This isn't quite right. Actor Class AND scene should act as a base.
// -> No _Ready() method in Actor Class
// -> Wrong class type?
// -> Check if GetNode methods can be called in a "non-godotObject" class

using Godot;

public class Actor : KinematicBody2D
{
  [Export]
  public float Speed = 6.0f;
  public Vector2 InitialPosition;
  public float MotionProgress;
  public bool IsMoving = false;
  private AnimationTree _animationTree;
  private AnimationNodeStateMachinePlayback _stateMachine;
  private RayCast2D _rayCast;

  public enum Movements
  {
    Forward,
    Stand,
    // Jump,
  }

  public override void _Ready()
  {
    _animationTree = GetNode<AnimationTree>("Sprite/AnimationTree");
    _stateMachine = (AnimationNodeStateMachinePlayback)_animationTree.Get("parameters/playback");
    _rayCast = GetNode<RayCast2D>("RayCast2D");
    InitialPosition = Position;
  }

  public void Move(Vector2 direction) { }

  public void Animate(Vector2 direction)
  {
    if (direction == Vector2.Zero)
      _stateMachine.Travel("idle");
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
}
