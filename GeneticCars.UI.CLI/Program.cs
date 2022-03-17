namespace GeneticCars.UI.CLI;

using Models;
using Network;
using CommandLine;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Reflection;
using Newtonsoft.Json;

internal static class Program
{
  public static async Task Main(string[] args)
  {
    var result = await Parser.Default.ParseArguments<Options>(args)
      .WithParsedAsync(Run);
    await result.WithNotParsedAsync(HandleParseError);
  }

  private static async Task Run(Options opt)
  {
    var trackName = opt.TrackName;
    var maxGenerations = opt.MaxGenerations;
    var carsPerGeneration = opt.CarsPerGeneration;

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

    var bestNN = JsonConvert.SerializeObject(evMgr.BestNeuralNetwork);
    Console.WriteLine("Best network:");
    Console.WriteLine(bestNN);

    // TODO   support deserialisation
    // var deserBestNN = JsonConvert.DeserializeObject<NeuralNetwork>(bestNN);
  }

  private static string GetTracksDir()
  {
    var currAssyPath = Assembly.GetExecutingAssembly().Location;
    var currAssyDir = Path.GetDirectoryName(currAssyPath);
    var tracksDir = Path.Combine(currAssyDir, "tracks");
    return tracksDir;
  }

  private static Task HandleParseError(IEnumerable<Error> errs)
  {
    if (errs.IsVersion())
    {
      Console.WriteLine("Version Request");
      return Task.CompletedTask;
    }

    if (errs.IsHelp())
    {
      Console.WriteLine("Help Request");
      return Task.CompletedTask;
      ;
    }

    Console.WriteLine("Parser Fail");
    return Task.CompletedTask;
  }
}

