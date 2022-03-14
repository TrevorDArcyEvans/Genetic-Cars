namespace GeneticCars.Models;

using System.Drawing;

public sealed class Car
{
  public const int Width = 20;
  public const int Height = 20;
  public const int LidarSenseDist = 100;

  public Point Position => _position;
  private Point _position;

  public Car(Point initialPos, double heading)
  {
    Reset(initialPos, heading);
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

  public void Reset(Point startPt, double heading)
  {
    _position.X = startPt.X;
    _position.Y = startPt.Y;
    Heading = heading;
  }
}
