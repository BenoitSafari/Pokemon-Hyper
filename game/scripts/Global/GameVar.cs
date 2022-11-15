using Godot;

public class GameVar : Node
{
  public const int TileSize = 16;
  public const int TileSizeHalf = TileSize / 2;

  public static bool IsGamePaused = false;
}
