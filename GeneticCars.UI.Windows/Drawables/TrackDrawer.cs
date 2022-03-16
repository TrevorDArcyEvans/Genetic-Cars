namespace GeneticCars.UI.Windows.Drawables;

using Models;

public sealed class TrackDrawer : IDrawable
{
  public Track Track { get; }

  public TrackDrawer(Track track)
  {
    Track = track;
  }

  public async Task Draw(Graphics ctx)
  {
    try
    {
    }
    finally
    {
    }
  }
}
