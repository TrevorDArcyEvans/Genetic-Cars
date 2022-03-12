namespace GeneticCars.Models;

using Blazor.Extensions.Canvas.Canvas2D;

public sealed class CheckpointDrawer : IDrawable
{
  public Checkpoint Checkpoint { get; }

  public CheckpointDrawer(Checkpoint checkpt)
  {
    Checkpoint = checkpt;
  }

  public async Task Draw(Canvas2DContext ctx)
  {
    await ctx.SaveAsync();
    try
    {
      // body
      await ctx.SetFillStyleAsync("red");
      await ctx.FillRectAsync(Checkpoint.Position.X, Checkpoint.Position.Y, Checkpoint.Width, Checkpoint.Height);
    }
    finally
    {
      await ctx.RestoreAsync();
    }
  }
}
