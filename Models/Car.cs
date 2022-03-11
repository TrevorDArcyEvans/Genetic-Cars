namespace GeneticCars.Models;

using System.Drawing;
using Excubo.Blazor.Canvas.Contexts;

public sealed class Car : IDrawable
{
  public const int Width = 20;
  public const int Height = 20;
  public const int LidarSenseDist = 100;

  private Point _position = new(20, 20);
  private double _heading; // degrees

  public async Task Draw(Context2D ctx)
  {
    await ctx.SaveAsync();
    try
    {
      await ctx.TranslateAsync(_position.X, _position.Y);
      await ctx.RotateAsync(_heading.ToRadians());
      await ctx.TranslateAsync(-_position.X, -_position.Y);

      // body
      await ctx.FillStyleAsync("blue");
      await ctx.FillRectAsync(_position.X, _position.Y, Width, Height);

      // nose
      const int NoseWidth = 10;
      const int NoseHeight = 10;
      await ctx.FillStyleAsync("green");
      await ctx.FillRectAsync(_position.X + Width / 2 - NoseWidth / 2, _position.Y - 3, NoseWidth, NoseHeight);

      // lidar
      const int LidarSenseWidth = 3;
      await ctx.StrokeStyleAsync("yellow");
      await ctx.LineWidthAsync(LidarSenseWidth);
      await ctx.BeginPathAsync();

      var nose = new Point(_position.X + Width / 2, _position.Y);

      // forward
      await ctx.MoveToAsync(nose.X, nose.Y);
      await ctx.LineToAsync(nose.X, _position.Y - LidarSenseDist);

      // right
      await ctx.MoveToAsync(nose.X, nose.Y);
      await ctx.LineToAsync(nose.X + LidarSenseDist, nose.Y);

      // left
      await ctx.MoveToAsync(nose.X, nose.Y);
      await ctx.LineToAsync(nose.X - LidarSenseDist, nose.Y);

      // forward-right
      await ctx.MoveToAsync(nose.X, nose.Y);
      await ctx.LineToAsync(nose.X + LidarSenseDist * 0.707, nose.Y - LidarSenseDist * 0.707);

      // forward-left
      await ctx.MoveToAsync(nose.X, nose.Y);
      await ctx.LineToAsync(nose.X - LidarSenseDist * 0.707, nose.Y - LidarSenseDist * 0.707);

      // back
      await ctx.MoveToAsync(nose.X, nose.Y + Height);
      await ctx.LineToAsync(nose.X, nose.Y + Height + LidarSenseDist);

      await ctx.StrokeAsync();
    }
    finally
    {
      await ctx.RestoreAsync();
    }
  }

  public void Move(int deltaX, int deltaY)
  {
    _position.X += deltaX;
    _position.Y += deltaY;
  }

  public void Rotate(double degrees)
  {
    _heading += degrees;
  }
}
