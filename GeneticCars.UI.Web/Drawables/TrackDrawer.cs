namespace GeneticCars.Drawables;

using Blazor.Extensions.Canvas.Canvas2D;
using Models;
using Microsoft.AspNetCore.Components;

public sealed class TrackDrawer : IDrawable
{
  public Track Track { get; }
  private readonly ElementReference _trackImgRef;

  public TrackDrawer(Track track, ElementReference trackImgRef)
  {
    Track = track;
    _trackImgRef = trackImgRef;
  }

  public async Task Draw(Canvas2DContext ctx)
  {
    await ctx.SaveAsync();
    try
    {
      await ctx.DrawImageAsync(_trackImgRef, 0, 0);
    }
    finally
    {
      await ctx.RestoreAsync();
    }
  }
}
