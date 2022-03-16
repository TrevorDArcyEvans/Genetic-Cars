namespace GeneticCars.Models;

using System.Drawing;

public sealed class Checkpoint
{
  public const int Width = 5;
  public const int Height = 5;

  public Point Position { get; }

  public Checkpoint(Point initialPos)
  {
    Position = new Point(initialPos.X, initialPos.Y);
  }
}
