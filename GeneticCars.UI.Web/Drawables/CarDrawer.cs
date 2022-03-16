namespace GeneticCars.Drawables;

using Blazor.Extensions.Canvas.Canvas2D;
using Models;

public sealed class CarDrawer : IDrawable
{
  public Car Car { get; }

  public CarDrawer(Car car)
  {
    Car = car;
  }

  public async Task Draw(Canvas2DContext ctx)
  {
    await ctx.SaveAsync();
    try
    {
      await ctx.TranslateAsync(Car.Position.X, Car.Position.Y);
      await ctx.RotateAsync((float)Car.Heading.ToRadians());
      await ctx.TranslateAsync(-Car.Position.X, -Car.Position.Y);

      // body
      await ctx.SetFillStyleAsync("blue");
      await ctx.FillRectAsync(Car.Position.X - Car.Width / 2, Car.Position.Y, Car.Width, Car.Height);

      // nose
      const int NoseWidth = 10;
      const int NoseHeight = 10;
      await ctx.SetFillStyleAsync("orange");
      await ctx.FillRectAsync(Car.Position.X - NoseWidth / 2, Car.Position.Y, NoseWidth, NoseHeight);

      // lidar
      const int LidarSenseWidth = 3;
      await ctx.SetStrokeStyleAsync("yellow");
      await ctx.SetLineWidthAsync(LidarSenseWidth);
      await ctx.BeginPathAsync();

      // forward
      await ctx.MoveToAsync(Car.Position.X, Car.Position.Y);
      await ctx.LineToAsync(Car.Position.X, Car.Position.Y - Car.LidarSenseDist);

      // right
      await ctx.MoveToAsync(Car.Position.X, Car.Position.Y);
      await ctx.LineToAsync(Car.Position.X + Car.LidarSenseDist, Car.Position.Y);

      // left
      await ctx.MoveToAsync(Car.Position.X, Car.Position.Y);
      await ctx.LineToAsync(Car.Position.X - Car.LidarSenseDist, Car.Position.Y);

      // forward-right
      await ctx.MoveToAsync(Car.Position.X, Car.Position.Y);
      await ctx.LineToAsync(Car.Position.X + Car.LidarSenseDist * 0.707, Car.Position.Y - Car.LidarSenseDist * 0.707);

      // forward-left
      await ctx.MoveToAsync(Car.Position.X, Car.Position.Y);
      await ctx.LineToAsync(Car.Position.X - Car.LidarSenseDist * 0.707, Car.Position.Y - Car.LidarSenseDist * 0.707);

      // back
      await ctx.MoveToAsync(Car.Position.X, Car.Position.Y + Car.Height);
      await ctx.LineToAsync(Car.Position.X, Car.Position.Y + Car.LidarSenseDist);

      await ctx.StrokeAsync();
    }
    finally
    {
      await ctx.RestoreAsync();
    }
  }
}
