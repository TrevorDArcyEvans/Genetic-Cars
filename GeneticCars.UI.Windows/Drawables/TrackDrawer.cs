namespace GeneticCars.UI.Windows.Drawables;

using Models;

public sealed class TrackDrawer : IDrawable
{
  public Track Track { get; }

  private readonly Bitmap _trackImage;

  public TrackDrawer(Track track, Bitmap trackImage)
  {
    (Track, _trackImage) = (track, trackImage);
  }

  public async Task Draw(Graphics ctx)
  {
    ctx.DrawImage(_trackImage, 0, 0);
  }
}
