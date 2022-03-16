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

      // body

      // nose
      const int NoseWidth = 10;
      const int NoseHeight = 10;

      // lidar
      const int LidarSenseWidth = 3;

      // forward

      // right

      // left

      // forward-right

      // forward-left

      // back

    }
    finally
    {
    }
  }
}
