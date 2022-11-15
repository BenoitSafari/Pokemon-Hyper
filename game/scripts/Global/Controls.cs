using Godot;

public class Controls : Node
{
  public static Vector2 DirectionVector;
  public static int DirectionStrength;
  public static bool IsUIEnabled = false;
  public static bool IsPlayerEnabled = false;

  public override void _Process(float delta)
  {
    if (IsPlayerEnabled)
      UpdateDirectionStrength();
  }

  public static void UpdateDirectionVector()
  {
    if (!IsPlayerEnabled || !IsUIEnabled)
      throw new System.Exception("Controls needs to be enabled to call this method.");
    if (DirectionVector.y == 0)
      DirectionVector.x = Input.GetActionStrength("ui_right") - Input.GetActionStrength("ui_left");
    if (DirectionVector.x == 0)
      DirectionVector.y = Input.GetActionStrength("ui_down") - Input.GetActionStrength("ui_up");
  }

  private void UpdateDirectionStrength()
  {
    if (DirectionVector != Vector2.Zero && DirectionStrength < 4)
      DirectionStrength++;
    else if (DirectionVector == Vector2.Zero && DirectionStrength > 0)
      DirectionStrength--;
  }
}
