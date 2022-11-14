using Godot;

public class Door : StaticBody2D
{
  private AnimatedSprite _animatedSprite;
  private Player _player;
  private Player.Direction _playerDirection;

  private enum Animation
  {
    Open,
    Close
  }

  public override void _Ready()
  {
    _animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
    _player = GetNode<Player>("/root/Game/Player");
    _player.Connect("PlayerDirection", this, nameof(OnPlayerDirectionChange));
    _player.Connect("PlayerCollidedWithDoor", this, nameof(OnPlayerCollision));
  }

  public override void _Process(float delta) { }

  private void AnimateDoor(Animation animation)
  {
    switch (animation)
    {
      case Animation.Open:
        _animatedSprite.Play("Open");
        break;
      case Animation.Close:
        _animatedSprite.Play("Close");
        break;
    }
  }

  private void OnPlayerDirectionChange(Player.Direction direction)
  {
    _playerDirection = direction;
  }

  private void OnPlayerCollision()
  {
    GD.Print("Player collided with door");
  }
}
