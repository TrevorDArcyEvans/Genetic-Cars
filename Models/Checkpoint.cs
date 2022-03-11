namespace GeneticCars.Models;

using System.Drawing;

public sealed class Checkpoint
{
  public const int Width = 10;
  public const int Height = 10;

  public Point Position => _position;
  private Point _position;

  public Checkpoint(Point initialPos)
  {
    _position = new Point(initialPos.X, initialPos.Y);
  }
}
