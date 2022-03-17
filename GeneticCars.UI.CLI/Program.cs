namespace GeneticCars.UI.CLI;

using Models;
using Network;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Reflection;

internal static class Program
{
  [STAThread]
  public static void Main(string[] args)
  {
    var trackName = args[0];
    var maxGenerations = int.Parse(args[1]);
    var carsPerGeneration = int.Parse(args[2]);

    var trackPath = Path.Combine(GetTracksDir(), trackName);
    var trackImg = Image.Load<Rgba32>(trackPath);
    var track = new Track(trackImg);

    var cars = Enumerable.Range(0, carsPerGeneration)
      .Select(_ => new Car(track.Start, track.Direction))
      .ToList()
      .AsReadOnly();

    var evMgr = new EvolutionManager((int)maxGenerations, track, cars);

    Console.WriteLine($"Running on: {trackName}");
    Console.WriteLine($"  Start             : [{track.Start.X}, {track.Start.Y}]");
    Console.WriteLine($"  ChkPts            : [{track.Checkpoints.Count()}]");
    Console.WriteLine($"  MaxGenerations    : {maxGenerations}");
    Console.WriteLine($"  CarsPerGeneration : {carsPerGeneration}");

    var currGenCount = evMgr.GenerationCount;
    while (evMgr.GenerationCount < evMgr.MaxGenerations)
    {
      evMgr.Update();

      if (currGenCount == evMgr.GenerationCount)
      {
        continue;
      }

      currGenCount = evMgr.GenerationCount;
      Console.WriteLine($"{evMgr.GenerationCount} / {evMgr.MaxGenerations} [{evMgr.BestFitness}]");
    }
  }

  private static string GetTracksDir()
  {
    var currAssyPath = Assembly.GetExecutingAssembly().Location;
    var currAssyDir = Path.GetDirectoryName(currAssyPath);
    var tracksDir = Path.Combine(currAssyDir, "tracks");
    return tracksDir;
  }
}

