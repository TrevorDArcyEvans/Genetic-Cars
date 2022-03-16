using GeneticCars.Network;

namespace GeneticCars.UI.Windows.Drawables;

using Models;

public sealed class CarDrawer : IDrawable
{
  public Car Car { get; }

  public CarDrawer(Car car)
  {
    Car = car;
  }

  public async Task Draw(Graphics ctx)
  {
    try
    {
      // rotate transform
      ctx.TranslateTransform(Car.Position.X, Car.Position.Y);
      ctx.RotateTransform((float)Car.Heading);
      ctx.TranslateTransform(-Car.Position.X, -Car.Position.Y);

      // body
      ctx.FillRectangle(Brushes.Blue, Car.Position.X - Car.Width / 2, Car.Position.Y, Car.Width, Car.Height);

      // nose
      const int NoseWidth = 10;
      const int NoseHeight = 10;
      ctx.FillRectangle(Brushes.Orange,Car.Position.X - NoseWidth / 2, Car.Position.Y, NoseWidth, NoseHeight);

      // lidar
      const int LidarSenseWidth = 3;

      // forward
      ctx.DrawLine(Pens.Yellow, Car.Position.X, Car.Position.Y, Car.Position.X, Car.Position.Y - Car.LidarSenseDist);

      // right
      ctx.DrawLine(Pens.Yellow, Car.Position.X, Car.Position.Y, Car.Position.X + Car.LidarSenseDist, Car.Position.Y);

      // left
      ctx.DrawLine(Pens.Yellow, Car.Position.X, Car.Position.Y, Car.Position.X - Car.LidarSenseDist, Car.Position.Y);

      // forward-right
      ctx.DrawLine(Pens.Yellow, Car.Position.X, Car.Position.Y, (int)(Car.Position.X + Car.LidarSenseDist * 0.707), (int)(Car.Position.Y - Car.LidarSenseDist * 0.707));

      // forward-left
      ctx.DrawLine(Pens.Yellow, Car.Position.X, Car.Position.Y, (int)(Car.Position.X - Car.LidarSenseDist * 0.707), (int)(Car.Position.Y - Car.LidarSenseDist * 0.707));

      // back
      ctx.DrawLine(Pens.Yellow, Car.Position.X, Car.Position.Y + Car.Height, Car.Position.X, Car.Position.Y + Car.LidarSenseDist);
    }
    finally
    {
      // unrotate transform
      ctx.TranslateTransform(Car.Position.X, Car.Position.Y);
      ctx.RotateTransform(-(float)Car.Heading);
      ctx.TranslateTransform(-Car.Position.X, -Car.Position.Y);
    }
  }
}
