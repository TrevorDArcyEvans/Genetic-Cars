namespace GeneticCars.UI.Windows.Drawables;

using Network;
using Blazor.Extensions.Canvas.Canvas2D;

public sealed class StatusMessageDrawer : IDrawable
{
  public StatusMessage StatusMessage { get; }

  public StatusMessageDrawer(StatusMessage statusMessage)
  {
    StatusMessage = statusMessage;
  }

  public async Task Draw(Canvas2DContext ctx)
  {
    await ctx.SetFontAsync("24px solid");
    await ctx.SetFillStyleAsync("white");
    await ctx.FillTextAsync($"{StatusMessage.Text}", 700, 750);
  }
}
