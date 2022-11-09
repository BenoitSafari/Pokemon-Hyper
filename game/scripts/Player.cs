using Godot;

public class Player : KinematicBody2D
{
  [Export]
  public float Speed = 5.0f;

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
    if (GetCollision())
    {
      MotionProgress += Speed * delta;
      if (MotionProgress >= 1.0f)
      {
        Position = InitialPosition + (InputVector * TileSize);
        MotionProgress = 0.0f;
        IsMoving = false;
      }
      else
        Position = InitialPosition + (InputVector * TileSize * MotionProgress);
    }
    else
    {
      IsMoving = false;
      MotionProgress = 0.0f;
    }
  }

  private void AnimatePlayer()
  {
    AnimationTree animTree = GetNode<AnimationTree>("AnimationTree");
    AnimationNodeStateMachinePlayback stateMachine = (AnimationNodeStateMachinePlayback)
      animTree.Get("parameters/playback");

    if (InputVector == Vector2.Zero)
    {
      stateMachine.Travel("idle");
    }
    else
    {
      animTree.Set("parameters/Walk/blend_position", InputVector);
      animTree.Set("parameters/Idle/blend_position", InputVector);
      stateMachine.Travel("walk");
    }
  }

  private bool GetCollision()
  {
    RayCast2D rayCast = GetNode<RayCast2D>("RayCast2D");
    rayCast.CastTo = InputVector * TileSize / 2;
    rayCast.ForceRaycastUpdate();
    if (!rayCast.IsColliding())
      return true;
    else
      return false;
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
