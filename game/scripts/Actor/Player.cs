using Godot;
using static Actor;

public class Player : KinematicBody2D
{
  private Actor _player;

  public override void _Ready()
  {
    _player = new Actor(this);
  }

  public override void _PhysicsProcess(float delta)
  {
    if (!_player.IsMoving && Controls.IsPlayerEnabled)
    {
      Controls.UpdateDirectionVector();
      _player.Animate(Controls.DirectionVector);
      if (Controls.DirectionVector != Vector2.Zero)
        _player.UpdateMovingState(true);
    }
    else if (Controls.DirectionVector != Vector2.Zero && Controls.DirectionStrength > 3)
    {
      if (_player.GetCollision(Controls.DirectionVector))
        _player.Move(MovementTypes.Stand, Controls.DirectionVector, delta);
      else
        _player.Move(MovementTypes.Forward, Controls.DirectionVector, delta);
    }
    else
    {
      _player.Move(MovementTypes.Stand, Controls.DirectionVector, delta);
    }
    _player.Animate(Controls.DirectionVector);
  }
}
