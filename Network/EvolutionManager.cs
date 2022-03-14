// stolen from:
//      ReInventing Neural Networks
//          https://www.codeproject.com/Articles/1220276/ReInventing-Neural-Networks
//      ReInventing Neural Networks - Part 2
//          https://www.codeproject.com/Articles/1220644/ReInventing-Neural-Networks-Part
//      ReInventing Neural Networks - Part 3
//          https://www.codeproject.com/Articles/1231020/ReInventing-Neural-Networks-Part-2

namespace GeneticCars.Network;

public sealed class EvolutionManager
{
  // The current EvolutionManager Instance
  public static EvolutionManager Instance;

  // Should we use node mutation?
  private const bool UseNodeMutation = true;

  // The number of cars per generation
  private const int CarCount = 100;

  // The current generation number
  private int _generationCount = 0;

  // This list of cars currently alive
  private readonly List<CarNetwork> _cars = new();

  // The best NeuralNetwork currently available
  private NeuralNetwork _bestNeuralNetwork;

  // The Fitness of the best NeuralNetwork ever created
  private int _bestFitness = -1;

  // Unity-Start
  private void Start()
  {
    if (Instance == null) // If no other instances were created
    {
      Instance = this; // Make the only instance this one
    }

    // Set the BestNeuralNetwork to a random new network
    _bestNeuralNetwork = new NeuralNetwork(CarNetwork.NextNetwork);

    StartGeneration();
  }

  // Starts a whole new generation
  private void StartGeneration()
  {
    _generationCount++; // Increment the generation count

    for (var i = 0; i < CarCount; i++)
    {
      if (i == 0)
      {
        CarNetwork.NextNetwork = _bestNeuralNetwork; // Make sure one car uses the best network
      }
      else
      {
        // Clone the best neural network and set it to be for the next car
        CarNetwork.NextNetwork = new NeuralNetwork(_bestNeuralNetwork);

        if (UseNodeMutation) // Should we use Node Mutation
        {
          CarNetwork.NextNetwork.MutateNodes(); // Mutate its nodes
        }
        else
        {
          CarNetwork.NextNetwork.Mutate(); // Mutate its weights
        }
      }

      // Instantiate a new car and add it to the list of cars
      // TODO   _cars.Add(Instantiate(CarPrefab, transform.position, Quaternion.identity, transform).GetComponent<Car>());
    }
  }

  // Gets called by cars when they die
  public void CarDead(CarNetwork deadCar, int fitness)
  {
    _cars.Remove(deadCar); // Remove the car from the list

    if (fitness > _bestFitness) // If it is better that the current best car
    {
      _bestNeuralNetwork = deadCar.TheNetwork; // Make sure it becomes the best car
      _bestFitness = fitness; // And also set the best fitness
    }

    if (_cars.Count <= 0) // If there are no cars left
    {
      StartGeneration(); // Create a new generation
    }
  }
}
