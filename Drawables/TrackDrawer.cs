namespace GeneticCars.Models;

using Blazor.Extensions.Canvas.Canvas2D;

public sealed class TrackDrawer : IDrawable
{
  public Track Track { get; }

  public TrackDrawer(Track track)
  {
    Track = track;
  }

  public async Task Draw(Canvas2DContext ctx)
  {
    await ctx.SaveAsync();
    try
    {
      for (var x = 0; x < Track.Width; x++)
      {
        for (var y = 0; y < Track.Height; y++)
        {
          if (Track.IsTrack(x, y))
          {
            await ctx.SetFillStyleAsync("white");
          }
          else
          {
            await ctx.SetFillStyleAsync("black");
          }

          await ctx.FillRectAsync(x, y, 1, 1);
        }
      }
    }
    finally
    {
      await ctx.RestoreAsync();
    }
  }
}
