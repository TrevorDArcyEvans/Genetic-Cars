// stolen from:
//      ReInventing Neural Networks
//          https://www.codeproject.com/Articles/1220276/ReInventing-Neural-Networks
//      ReInventing Neural Networks - Part 2
//          https://www.codeproject.com/Articles/1220644/ReInventing-Neural-Networks-Part
//      ReInventing Neural Networks - Part 3
//          https://www.codeproject.com/Articles/1231020/ReInventing-Neural-Networks-Part-2

namespace GeneticCars.Network;

public sealed partial class NeuralNetwork
{
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
}
