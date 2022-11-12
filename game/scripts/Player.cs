using Godot;

// FIXME: Player's sprite delayed on directionnal change
// FIXME: Player's sprite is not centered and uses two tiles. Z-Index is currently identical on both tiles, this need to be updated.

public class Player : KinematicBody2D
{
  private enum CollisionType
  {
    Void,
    Bump,
    Door
  }

  [Signal]
  public delegate void PlayerCollidedWithDoor();

  [Export]
  public float Speed = 6.0f;

  private const int TileSize = 16;
  private Vector2 InputVector;
  private Vector2 InitialPosition;
  private float MotionProgress = 0.0f;
  private int MotionStrength = 0;
  private bool IsMoving = false;

  public override void _Ready()
  {
    GetNode<AnimationTree>("AnimationTree").Active = true;
    InitialPosition = Position;
    AnimatePlayer();
  }

  public override void _PhysicsProcess(float delta)
  {
    GetMotionStrength();
    if (!IsMoving)
    {
      GetInput();
      if (InputVector != Vector2.Zero)
      {
        InitialPosition = Position;
        IsMoving = true;
      }
    }
    else if (InputVector != Vector2.Zero && MotionStrength > 3)
      MovePlayer(delta);
    else
      IsMoving = false;

    AnimatePlayer();
  }

  private void MovePlayer(float delta)
  {
    switch (GetCollision())
    {
      case CollisionType.Bump:
        IsMoving = false;
        MotionProgress = 0.0f;
        break;

      case CollisionType.Void:
        MotionProgress += Speed * delta;
        if (MotionProgress >= 1.0f)
        {
          Position = InitialPosition + (InputVector * TileSize);
          MotionProgress = 0.0f;
          IsMoving = false;
        }
        else
          Position = InitialPosition + (InputVector * TileSize * MotionProgress);
        break;
    }
  }

  private void AnimatePlayer()
  {
    AnimationTree animTree = GetNode<AnimationTree>("AnimationTree");
    AnimationNodeStateMachinePlayback stateMachine = (AnimationNodeStateMachinePlayback)
      animTree.Get("parameters/playback");

    if (InputVector == Vector2.Zero)
    {
      stateMachine.Travel("Idle");
    }
    else
    {
      animTree.Set("parameters/Walk/blend_position", InputVector);
      animTree.Set("parameters/Idle/blend_position", InputVector);
      stateMachine.Travel("Walk");
    }
  }

  private CollisionType GetCollision()
  {
    RayCast2D RayCastBump = GetNode<RayCast2D>("RayCastBump");
    RayCastBump.CastTo = InputVector * TileSize / 2;
    RayCastBump.ForceRaycastUpdate();
    if (RayCastBump.IsColliding())
      return CollisionType.Bump;
    else
      return CollisionType.Void;
  }

  private void GetInput()
  {
    if (InputVector.y == 0)
      InputVector.x = Input.GetActionStrength("ui_right") - Input.GetActionStrength("ui_left");
    if (InputVector.x == 0)
      InputVector.y = Input.GetActionStrength("ui_down") - Input.GetActionStrength("ui_up");
  }

  private void GetMotionStrength()
  {
    if (InputVector != Vector2.Zero && MotionStrength < 4)
      MotionStrength++;
    else if (InputVector == Vector2.Zero && MotionStrength > 0)
      MotionStrength--;
  }
}
