namespace GeneticCars.Models;

using System.Drawing;
using Excubo.Blazor.Canvas.Contexts;

public sealed class CarDrawer : IDrawable
{
  public Car Car { get; }

  public CarDrawer(Car car)
  {
    Car = car;
  }

  public async Task Draw(Context2D ctx)
  {
    await ctx.SaveAsync();
    try
    {
      await ctx.TranslateAsync(Car.Position.X, Car.Position.Y);
      await ctx.RotateAsync(Car.Heading.ToRadians());
      await ctx.TranslateAsync(-Car.Position.X, -Car.Position.Y);

      // body
      await ctx.FillStyleAsync("blue");
      await ctx.FillRectAsync(Car.Position.X, Car.Position.Y, Car.Width, Car.Height);

      // nose
      const int NoseWidth = 10;
      const int NoseHeight = 10;
      await ctx.FillStyleAsync("green");
      await ctx.FillRectAsync(Car.Position.X + Car.Width / 2 - NoseWidth / 2, Car.Position.Y - 3, NoseWidth, NoseHeight);

      // lidar
      const int LidarSenseWidth = 3;
      await ctx.StrokeStyleAsync("yellow");
      await ctx.LineWidthAsync(LidarSenseWidth);
      await ctx.BeginPathAsync();

      var nose = new Point(Car.Position.X + Car.Width / 2, Car.Position.Y);

      // forward
      await ctx.MoveToAsync(nose.X, nose.Y);
      await ctx.LineToAsync(nose.X, Car.Position.Y - Car.LidarSenseDist);

      // right
      await ctx.MoveToAsync(nose.X, nose.Y);
      await ctx.LineToAsync(nose.X + Car.LidarSenseDist, nose.Y);

      // left
      await ctx.MoveToAsync(nose.X, nose.Y);
      await ctx.LineToAsync(nose.X - Car.LidarSenseDist, nose.Y);

      // forward-right
      await ctx.MoveToAsync(nose.X, nose.Y);
      await ctx.LineToAsync(nose.X + Car.LidarSenseDist * 0.707, nose.Y - Car.LidarSenseDist * 0.707);

      // forward-left
      await ctx.MoveToAsync(nose.X, nose.Y);
      await ctx.LineToAsync(nose.X - Car.LidarSenseDist * 0.707, nose.Y - Car.LidarSenseDist * 0.707);

      // back
      await ctx.MoveToAsync(nose.X, nose.Y + Car.Height);
      await ctx.LineToAsync(nose.X, nose.Y + Car.Height + Car.LidarSenseDist);

      await ctx.StrokeAsync();
    }
    finally
    {
      await ctx.RestoreAsync();
    }
  }
}