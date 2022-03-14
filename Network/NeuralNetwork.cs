// stolen from:
//      ReInventing Neural Networks
//          https://www.codeproject.com/Articles/1220276/ReInventing-Neural-Networks
//      ReInventing Neural Networks - Part 2
//          https://www.codeproject.com/Articles/1220644/ReInventing-Neural-Networks-Part
//      ReInventing Neural Networks - Part 3
//          https://www.codeproject.com/Articles/1231020/ReInventing-Neural-Networks-Part-2

namespace GeneticCars.Network;

using System.Collections.ObjectModel;

public sealed class NeuralNetwork
{
  // Returns the topology in the form of an array
  public uint[] Topology
  {
    get
    {
      var result = new uint[_theTopology.Count];

      _theTopology.CopyTo(result, 0);

      return result;
    }
  }

  // Contains the topology of the NeuralNetwork
  private readonly ReadOnlyCollection<uint> _theTopology;

  // Contains the all the sections of the NeuralNetwork
  private readonly NeuralSection[] _sections;

  // It is the Random instance used to mutate the NeuralNetwork
  private readonly Random _theRandomizer;

  private class NeuralSection
  {
    // Contains all the weights of the section where [i][j] represents the weight 
    // from neuron i in the input layer and neuron j in the output layer
    private readonly double[][] _weights;

    // Contains a reference to the Random instance of the NeuralNetwork
    private readonly Random _theRandomizer;

    /// <summary>
    /// Initiate a NeuralSection from a topology and a seed.
    /// </summary>
    /// <param name="inputCount">The number of input neurons in the section.</param>
    /// <param name="outputCount">The number of output neurons in the section.</param>
    /// <param name="randomizer">The Ransom instance of the NeuralNetwork.</param>
    public NeuralSection(uint inputCount, uint outputCount, Random randomizer)
    {
      // Validation Checks
      if (inputCount == 0)
      {
        throw new ArgumentException("You cannot create a Neural Layer with no input neurons.", nameof(inputCount));
      }

      if (outputCount == 0)
      {
        throw new ArgumentException("You cannot create a Neural Layer with no output neurons.", nameof(outputCount));
      }

      // Set Randomizer
      _theRandomizer = randomizer ?? throw new ArgumentException("The randomizer cannot be set to null.", nameof(randomizer));

      // Initialize the Weights array
      // +1 for the Bias Neuron
      _weights = new double[inputCount + 1][];

      for (var i = 0; i < _weights.Length; i++)
      {
        _weights[i] = new double[outputCount];
      }

      // Set random weights
      foreach (var weight in _weights)
      {
        for (var j = 0; j < weight.Length; j++)
        {
          weight[j] = _theRandomizer.NextDouble() - 0.5f;
        }
      }
    }

    /// <summary>
    /// Initiates an independent Deep-Copy of the NeuralSection provided.
    /// </summary>
    /// <param name="main">The NeuralSection that should be cloned.</param>
    public NeuralSection(NeuralSection main)
    {
      // Set Randomizer
      _theRandomizer = main._theRandomizer;

      // Initialize Weights
      _weights = new double[main._weights.Length][];

      for (var i = 0; i < _weights.Length; i++)
      {
        _weights[i] = new double[main._weights[0].Length];
      }

      // Set Weights
      for (var i = 0; i < _weights.Length; i++)
      {
        for (var j = 0; j < _weights[i].Length; j++)
        {
          _weights[i][j] = main._weights[i][j];
        }
      }
    }

    /// <summary>
    /// Feed input through the NeuralSection and get the output.
    /// </summary>
    /// <param name="input">The values to set the input neurons.</param>
    /// <returns>The values in the output neurons after propagation.</returns>
    public double[] FeedForward(double[] input)
    {
      // Validation Checks
      if (input == null)
      {
        throw new ArgumentException("The input array cannot be set to null.", nameof(input));
      }

      if (input.Length != _weights.Length - 1)
      {
        throw new ArgumentException("The input array's length does not match the number of neurons in the input layer.", nameof(input));
      }

      // Initialize Output Array
      var output = new double[_weights[0].Length];

      // Calculate Value
      for (var i = 0; i < _weights.Length; i++)
      {
        for (var j = 0; j < _weights[i].Length; j++)
        {
          if (i == _weights.Length - 1) // If is Bias Neuron
          {
            output[j] += _weights[i][j]; // Then, the value of the neuron is equal to one
          }
          else
          {
            output[j] += _weights[i][j] * input[i];
          }
        }
      }

      // Apply Activation Function
      for (var i = 0; i < output.Length; i++)
      {
        output[i] = ReLU(output[i]);
      }

      // Return Output
      return output;
    }

    /// <summary>
    /// Mutate the NeuralSection.
    /// </summary>
    /// <param name="mutationProbability">The probability that a weight is going to be mutated. (Ranges 0-1)</param>
    /// <param name="mutationAmount">The maximum amount a Mutated Weight would change.</param>
    public void Mutate(double mutationProbability, double mutationAmount)
    {
      foreach (var weight in _weights)
      {
        for (var j = 0; j < weight.Length; j++)
        {
          if (_theRandomizer.NextDouble() < mutationProbability)
          {
            weight[j] = _theRandomizer.NextDouble() * (mutationAmount * 2) - mutationAmount;
          }
        }
      }
    }

    /// <summary>
    /// Mutate the NeuralSection's Nodes.
    /// </summary>
    /// <param name="mutationProbability">The probability that a node is going to be mutated. (Ranges 0-1)</param>
    /// <param name="mutationAmount">The maximum amount a Mutated Weight would change.</param>
    public void MutateNodes(double mutationProbability, double mutationAmount)
    {
      for (var j = 0; j < _weights[0].Length; j++) // For each output node
      {
        if (_theRandomizer.NextDouble() < mutationProbability) // Check if we are going to mutate this node
        {
          foreach (var weight in _weights)
          {
            weight[j] = _theRandomizer.NextDouble() * (mutationAmount * 2) - mutationAmount; // Mutate the weight connecting both nodes
          }
        }
      }
    }

    /// <summary>
    /// Puts a double through the activation function ReLU.
    /// </summary>
    /// <param name="x">The value to put through the function.</param>
    /// <returns>x after it is put through ReLU.</returns>
    private double ReLU(double x)
    {
      if (x >= 0)
      {
        return x;
      }

      return x / 20;
    }
  }

  /// <summary>
  /// Initiates a NeuralNetwork from a Topology and a Seed.
  /// </summary>
  /// <param name="topology">The Topology of the Neural Network.</param>
  /// <param name="seed">The Seed of the Neural Network. Set to 'null' to use a Timed Seed.</param>
  public NeuralNetwork(uint[] topology, int? seed = 0)
  {
    // Validation Checks
    if (topology.Length < 2)
    {
      throw new ArgumentException("A Neural Network cannot contain less than 2 Layers.", nameof(topology));
    }

    foreach (var numNeurons in topology)
    {
      if (numNeurons < 1)
      {
        throw new ArgumentException("A single layer of neurons must contain, at least, one neuron.", nameof(topology));
      }
    }

    // Initialize Randomizer
    _theRandomizer = seed.HasValue ? new Random(seed.Value) : new Random();

    // Set Topology
    _theTopology = new List<uint>(topology).AsReadOnly();

    // Initialize Sections
    _sections = new NeuralSection[_theTopology.Count - 1];

    // Set the Sections
    for (var i = 0; i < _sections.Length; i++)
    {
      _sections[i] = new NeuralSection(_theTopology[i], _theTopology[i + 1], _theRandomizer);
    }
  }

  /// <summary>
  /// Initiates an independent Deep-Copy of the Neural Network provided.
  /// </summary>
  /// <param name="main">The Neural Network that should be cloned.</param>
  public NeuralNetwork(NeuralNetwork main)
  {
    // Initialize Randomizer
    _theRandomizer = new Random(main._theRandomizer.Next());

    // Set Topology
    _theTopology = main._theTopology;

    // Initialize Sections
    _sections = new NeuralSection[_theTopology.Count - 1];

    // Set the Sections
    for (var i = 0; i < _sections.Length; i++)
    {
      _sections[i] = new NeuralSection(main._sections[i]);
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

    if (input.Length != _theTopology[0])
    {
      throw new ArgumentException("The input array's length does not match the number of neurons in the input layer.", nameof(input));
    }

    var output = input;

    // Feed values through all sections
    foreach (var ns in _sections)
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
    foreach (var ns in _sections)
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
    foreach (var ns in _sections)
    {
      ns.MutateNodes(mutationProbability, mutationAmount);
    }
  }
}
