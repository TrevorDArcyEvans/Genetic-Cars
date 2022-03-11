namespace GeneticCars.Models;

using System.Drawing;
using Excubo.Blazor.Canvas.Contexts;

public sealed class Car
{
  public const int Width = 20;
  public const int Height = 20;
  public const int LidarSenseDist = 100;

  public Point Position => _position;
  private Point _position;

  public Car(Point initialPos)
  {
    _position = new Point(initialPos.X, initialPos.Y);
  }

  public double Heading { get; private set;} // degrees

  public void Move(int deltaX, int deltaY)
  {
    _position.X += deltaX;
    _position.Y += deltaY;
  }

  public void Rotate(double degrees)
  {
    Heading += degrees;
  }
}
