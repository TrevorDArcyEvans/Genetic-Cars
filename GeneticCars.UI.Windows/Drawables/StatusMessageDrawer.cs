namespace GeneticCars.UI.Windows.Drawables;

using Network;

public sealed class StatusMessageDrawer : IDrawable
{
  public StatusMessage StatusMessage { get; }

  public StatusMessageDrawer(StatusMessage statusMessage)
  {
    StatusMessage = statusMessage;
  }

  public async Task Draw(Graphics ctx)
  {
    ctx.DrawString(StatusMessage.Text, SystemFonts.DefaultFont, Brushes.White, 700, 750);
  }
}
