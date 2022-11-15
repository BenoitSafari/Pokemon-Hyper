using Godot;
using static Actor;

// FIXME: Player's sprite delayed on directionnal change
// FIXME: Player's sprite is not centered and uses two tiles. Z-Index is currently identical on both tiles, this need to be updated.

public class Player : KinematicBody2D
{
  private Actor _player;

  public enum Direction
  {
    Up,
    Down,
    Left,
    Right
  }

  public override void _Ready()
  {
    _player = new Actor();
  }

  public override void _PhysicsProcess(float delta)
  {
    if (!_player.IsMoving && Controls.IsPlayerEnabled)
    {
      Controls.UpdateDirectionVector();
      if (Controls.DirectionVector != Vector2.Zero)
        UpdateMovingState(true);
    }
    else if (Controls.DirectionVector != Vector2.Zero && Controls.DirectionStrength > 3)
    {
      if (_player.GetCollision(Controls.DirectionVector))
        MovePlayer(Movements.Stand, delta);
      else
        MovePlayer(Movements.Forward, delta);
    }
    else
    {
      MovePlayer(Movements.Stand, delta);
    }
    _player.Animate(Controls.DirectionVector);
  }

  private void MovePlayer(Movements enumerator, float delta)
  {
    switch (enumerator)
    {
      case Movements.Forward:
        _player.MotionProgress += _player.Speed * delta;
        if (_player.MotionProgress >= 1.0f)
        {
          Position = _player.InitialPosition + (Controls.DirectionVector * GameVar.TileSize);
          UpdateMovingState(false);
        }
        else
          Position =
            _player.InitialPosition
            + (Controls.DirectionVector * GameVar.TileSize * _player.MotionProgress);
        break;

      case Movements.Stand:
        UpdateMovingState(false);
        break;
    }
  }

  private void UpdateMovingState(bool isMoving)
  {
    _player.IsMoving = isMoving;

    if (!isMoving)
      _player.MotionProgress = 0.0f;
    else
      _player.InitialPosition = Position;
  }

  // private void EmitPlayerDirection()
  // {
  //   if (Controls.DirectionVector.x == 1)
  //     EmitSignal(nameof(PlayerDirection), Direction.Right);
  //   else if (Controls.DirectionVector.x == -1)
  //     EmitSignal(nameof(PlayerDirection), Direction.Left);
  //   else if (Controls.DirectionVector.y == 1)
  //     EmitSignal(nameof(PlayerDirection), Direction.Down);
  //   else if (Controls.DirectionVector.y == -1)
  //     EmitSignal(nameof(PlayerDirection), Direction.Up);
  // }
}
