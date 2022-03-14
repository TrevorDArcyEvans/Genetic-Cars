// stolen from:
//      ReInventing Neural Networks
//          https://www.codeproject.com/Articles/1220276/ReInventing-Neural-Networks
//      ReInventing Neural Networks - Part 2
//          https://www.codeproject.com/Articles/1220644/ReInventing-Neural-Networks-Part
//      ReInventing Neural Networks - Part 3
//          https://www.codeproject.com/Articles/1231020/ReInventing-Neural-Networks-Part-2

namespace GeneticCars.Network;

using System.Collections.ObjectModel;
using GeneticCars.Models;

public sealed class EvolutionManager
{
  // Should we use node mutation?
  private const bool UseNodeMutation = true;

  private const int MaxGenerations = 10;
  
  // The current generation number
  private int _generationCount;

  // This list of cars currently alive
  private readonly List<CarNetwork> _carNets = new();
  private readonly ReadOnlyCollection<Car> _cars;

  // The best NeuralNetwork currently available
  private NeuralNetwork _bestNeuralNetwork;

  // The Fitness of the best NeuralNetwork ever created
  private int _bestFitness = -1;

  private readonly Track _track;

  public EvolutionManager(Track track, ReadOnlyCollection<Car> cars)
  {
    (_track, _cars) = (track, cars);

    // Set the BestNeuralNetwork to a random new network
    _bestNeuralNetwork = new NeuralNetwork(CarNetwork.NextNetwork);

    StartGeneration();
  }

  public void Update()
  {
    _carNets.ForEach(car => car.Update());
  }
  
  // Starts a whole new generation
  private void StartGeneration()
  {
    _generationCount++; // Increment the generation count
    if (_generationCount > MaxGenerations)
    {
      return;
    }

    for (var i = 0; i < _cars.Count(); i++)
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
      var car = new CarNetwork(this, _cars[i], _track);
      _carNets.Add(car);
    }
  }

  // Gets called by cars when they die
  public void CarDead(CarNetwork deadCar)
  {
    _carNets.Remove(deadCar); // Remove the car from the list

    if (deadCar.Fitness > _bestFitness) // If it is better that the current best car
    {
      _bestNeuralNetwork = deadCar.Network; // Make sure it becomes the best car
      _bestFitness = deadCar.Fitness; // And also set the best fitness
    }

    if (_carNets.Count > 0)
    {
      return;
    }

    StartGeneration(); // Create a new generation
  }
}
