namespace GeneticCars.Models;

using System.Drawing;

public sealed class Checkpoint
{
  public const int Width = 10;
  public const int Height = 10;

  public Point Position => _position;
  private Point _position = new(200, 200);
}
