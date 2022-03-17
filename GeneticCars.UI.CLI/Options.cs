namespace GeneticCars.UI.CLI;

using CommandLine;

internal sealed class Options
{
  [Value(index: 0, Required = true, HelpText = "Name of track image file in 'tracks' directory inc extension")]
  public string TrackName { get; set; }

  [Option('m', "MaxGen", Required = false, Default = 200, HelpText = "Maximum number of generations")]
  public int MaxGenerations { get; set; }

  [Option('c', "CarsPerGen", Required = false, Default = 10, HelpText = "Number of cars per generation")]
  public int CarsPerGeneration { get; set; }
}