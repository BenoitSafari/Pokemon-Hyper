using Godot;

public class Game : Node
{
  public override void _Ready()
  {
    Controls.IsPlayerEnabled = true;
    Controls.IsUIEnabled = true;
  }
}
