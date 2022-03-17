namespace GeneticCars.Models;

using System.Drawing;

public sealed class Checkpoint
{
  public const int Radius = 10;
  public const int RadiusSquared = Radius * Radius;

  public Point Position { get; }

  public Checkpoint(Point initialPos)
  {
    Position = new Point(initialPos.X, initialPos.Y);
  }

  public override string ToString()
  {
    return $"[{Position.X},{Position.Y}]";
  }
}
