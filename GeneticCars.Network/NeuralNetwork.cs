// stolen from:
//      ReInventing Neural Networks
//          https://www.codeproject.com/Articles/1220276/ReInventing-Neural-Networks
//      ReInventing Neural Networks - Part 2
//          https://www.codeproject.com/Articles/1220644/ReInventing-Neural-Networks-Part
//      ReInventing Neural Networks - Part 3
//          https://www.codeproject.com/Articles/1231020/ReInventing-Neural-Networks-Part-2

namespace GeneticCars.Network;

using System.Collections.ObjectModel;

public sealed partial class NeuralNetwork
{
  // Returns the topology in the form of an array
  public uint[] Topology
  {
    get
    {
      var result = new uint[_topology.Count];

      _topology.CopyTo(result, 0);

      return result;
    }
  }

  // Contains the topology of the NeuralNetwork
  private readonly ReadOnlyCollection<uint> _topology;

  // Contains the all the sections of the NeuralNetwork
  public NeuralSection[] Sections { get; private set; }

  // It is the Random instance used to mutate the NeuralNetwork
  private readonly Random _randomizer = new Random();

  /// <summary>
  /// Initiates a NeuralNetwork from a Topology and a Seed.
  /// </summary>
  /// <param name="topology">The Topology of the Neural Network.</param>
  /// <param name="seed">The Seed of the Neural Network. Set to 'null' to use a Timed Seed.</param>
  public NeuralNetwork(uint[] topology)
  {
    // Validation Checks
    if (topology.Length < 2)
    {
      throw new ArgumentException("A Neural Network cannot contain less than 2 Layers.", nameof(topology));
    }

    if (topology.Any(numNeurons => numNeurons < 1))
    {
      throw new ArgumentException("A single layer of neurons must contain, at least, one neuron.", nameof(topology));
    }

    // Set Topology
    _topology = new List<uint>(topology).AsReadOnly();

    // Initialize Sections
    Sections = new NeuralSection[_topology.Count - 1];

    // Set the Sections
    for (var i = 0; i < Sections.Length; i++)
    {
      Sections[i] = new NeuralSection(_topology[i], _topology[i + 1]);
    }
  }

  /// <summary>
  /// Initiates an independent Deep-Copy of the Neural Network provided.
  /// </summary>
  /// <param name="main">The Neural Network that should be cloned.</param>
  public NeuralNetwork(NeuralNetwork main)
  {
    // Initialize Randomizer
    _randomizer = new Random(main._randomizer.Next());

    // Set Topology
    _topology = main._topology;

    // Initialize Sections
    Sections = new NeuralSection[_topology.Count - 1];

    // Set the Sections
    for (var i = 0; i < Sections.Length; i++)
    {
      Sections[i] = new NeuralSection(main.Sections[i]);
    }
  }

  /// <summary>
  /// Feed Input through the NeuralNetwork and Get the Output.
  /// </summary>
  /// <param name="input">The values to set the Input Neurons.</param>
  /// <returns>The values in the output neurons after propagation.</returns>
  public double[] FeedForward(double[] input)
  {
    // Validation Checks
    if (input == null)
    {
      throw new ArgumentException("The input array cannot be set to null.", nameof(input));
    }

    if (input.Length != _topology[0])
    {
      throw new ArgumentException("The input array's length does not match the number of neurons in the input layer.", nameof(input));
    }

    var output = input;

    // Feed values through all sections
    foreach (var ns in Sections)
    {
      output = ns.FeedForward(output);
    }

    return output;
  }

  /// <summary>
  /// Mutate the NeuralNetwork.
  /// </summary>
  /// <param name="mutationProbability">The probability that a weight is going to be mutated. (Ranges 0-1)</param>
  /// <param name="mutationAmount">The maximum amount a mutated weight would change.</param>
  public void Mutate(double mutationProbability = 0.3, double mutationAmount = 2.0)
  {
    // Mutate each section
    foreach (var ns in Sections)
    {
      ns.Mutate(mutationProbability, mutationAmount);
    }
  }

  /// <summary>
  /// Mutate the NeuralNetwork's Nodes.
  /// </summary>
  /// <param name="mutationProbability">The probability that a node is going to be mutated. (Ranges 0-1)</param>
  /// <param name="mutationAmount">The maximum amount a Mutated Weight would change.</param>
  public void MutateNodes(double mutationProbability = 0.3, double mutationAmount = 2.0)
  {
    // Mutate each section
    foreach (var ns in Sections)
    {
      ns.MutateNodes(mutationProbability, mutationAmount);
    }
  }
}
