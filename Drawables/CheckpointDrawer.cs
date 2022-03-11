namespace GeneticCars.Models;

using Excubo.Blazor.Canvas.Contexts;

public sealed class CheckpointDrawer : IDrawable
{
  public Checkpoint Checkpoint { get; }

  public CheckpointDrawer(Checkpoint checkpt)
  {
    Checkpoint = checkpt;
  }

  public async Task Draw(Context2D ctx)
  {
    await ctx.SaveAsync();
    try
    {
      // body
      await ctx.FillStyleAsync("red");
      await ctx.FillRectAsync(Checkpoint.Position.X, Checkpoint.Position.Y, Checkpoint.Width, Checkpoint.Height);
    }
    finally
    {
      await ctx.RestoreAsync();
    }
  }
}
