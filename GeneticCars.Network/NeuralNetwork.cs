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

  public sealed class NeuralSection
  {
    // Contains all the weights of the section where [i][j] represents the weight 
    // from neuron i in the input layer and neuron j in the output layer
    public double[][] Weights { get; private set; }

    // Contains a reference to the Random instance of the NeuralNetwork
    private readonly Random _randomizer = new ();

    /// <summary>
    /// Initiate a NeuralSection from a topology and a seed.
    /// </summary>
    /// <param name="inputCount">The number of input neurons in the section.</param>
    /// <param name="outputCount">The number of output neurons in the section.</param>
    public NeuralSection(uint inputCount, uint outputCount)
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

      // Initialize the Weights array
      // +1 for the Bias Neuron
      Weights = new double[inputCount + 1][];

      for (var i = 0; i < Weights.Length; i++)
      {
        Weights[i] = new double[outputCount];
      }

      // Set random weights
      foreach (var weight in Weights)
      {
        for (var j = 0; j < weight.Length; j++)
        {
          weight[j] = _randomizer.NextDouble() - 0.5f;
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
      _randomizer = main._randomizer;

      // Initialize Weights
      Weights = new double[main.Weights.Length][];

      for (var i = 0; i < Weights.Length; i++)
      {
        Weights[i] = new double[main.Weights[0].Length];
      }

      // Set Weights
      for (var i = 0; i < Weights.Length; i++)
      {
        for (var j = 0; j < Weights[i].Length; j++)
        {
          Weights[i][j] = main.Weights[i][j];
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

      if (input.Length != Weights.Length - 1)
      {
        throw new ArgumentException("The input array's length does not match the number of neurons in the input layer.", nameof(input));
      }

      // Initialize Output Array
      var output = new double[Weights[0].Length];

      // Calculate Value
      for (var i = 0; i < Weights.Length; i++)
      {
        for (var j = 0; j < Weights[i].Length; j++)
        {
          if (i == Weights.Length - 1) // If is Bias Neuron
          {
            output[j] += Weights[i][j]; // Then, the value of the neuron is equal to one
          }
          else
          {
            output[j] += Weights[i][j] * input[i];
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
      foreach (var weight in Weights)
      {
        for (var j = 0; j < weight.Length; j++)
        {
          if (_randomizer.NextDouble() < mutationProbability)
          {
            weight[j] = _randomizer.NextDouble() * (mutationAmount * 2) - mutationAmount;
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
      for (var j = 0; j < Weights[0].Length; j++) // For each output node
      {
        if (!(_randomizer.NextDouble() < mutationProbability))
        {
          continue;
        }

        foreach (var weight in Weights)
        {
          // Mutate the weight connecting both nodes
          weight[j] = _randomizer.NextDouble() * (mutationAmount * 2) - mutationAmount;
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
