namespace GeneticCars.Models;

using System.Drawing;

public sealed class Car
{
  public const int Width = 20;
  public const int Height = 20;
  public const int LidarSenseDist = 40;

  /// <summary>
  /// Location of nose of car
  /// </summary>
  public Point Position => _position;
  private Point _position;

  public Car(Point initialPos, double heading)
  {
    Reset(initialPos, heading);
  }

  public double Heading { get; set;} // degrees

  public void Move(int deltaX, int deltaY)
  {
    _position.X += deltaX;
    _position.Y += deltaY;
  }

  public void Reset(Point startPt, double heading)
  {
    _position.X = startPt.X;
    _position.Y = startPt.Y;
    Heading = heading;
  }
}
