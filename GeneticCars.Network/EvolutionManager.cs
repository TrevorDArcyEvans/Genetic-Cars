// stolen from:
//      ReInventing Neural Networks
//          https://www.codeproject.com/Articles/1220276/ReInventing-Neural-Networks
//      ReInventing Neural Networks - Part 2
//          https://www.codeproject.com/Articles/1220644/ReInventing-Neural-Networks-Part
//      ReInventing Neural Networks - Part 3
//          https://www.codeproject.com/Articles/1231020/ReInventing-Neural-Networks-Part-2

namespace GeneticCars.Network;

using System.Collections.ObjectModel;
using Models;

public sealed class EvolutionManager
{
  // Should we use node mutation?
  private const bool UseNodeMutation = true;

  public int MaxGenerations { get; }

  // The current generation number
  public int GenerationCount { get; private set; }

  // This list of cars currently alive
  private readonly List<CarNetwork> _carNets = new();
  private readonly List<CarNetwork> _carNetsPendingRemoval = new();
  private readonly ReadOnlyCollection<Car> _cars;

  // The best NeuralNetwork currently available
  private NeuralNetwork _bestNeuralNetwork;

  // The Fitness of the best NeuralNetwork ever created
  private int _bestFitness = -1;

  private readonly Track _track;

  public EvolutionManager(int maxGen, Track track, ReadOnlyCollection<Car> cars)
  {
    (MaxGenerations, _track, _cars) = (maxGen, track, cars);

    // Set the BestNeuralNetwork to a random new network
    _bestNeuralNetwork = new NeuralNetwork(CarNetwork.NextNetwork);

    StartGeneration();
  }

  public void Update()
  {
    _carNets.ForEach(car => car.Update());
    _carNetsPendingRemoval.ForEach(car => _carNets.Remove(car));
    _carNetsPendingRemoval.Clear();

    if (_carNets.Count == 0)
    {
      StartGeneration(); // Create a new generation
    }
  }

  public void Reset()
  {
    GenerationCount = 0;
    _cars.ToList().ForEach(car => car.Reset(_track.Start, _track.Direction));
    StartGeneration();
  }

  // Starts a whole new generation
  private void StartGeneration()
  {
    if (GenerationCount >= MaxGenerations)
    {
      return;
    }
    GenerationCount++; // Increment the generation count

    _carNets.Clear();
    for (var i = 0; i < _cars.Count; i++)
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
      _cars[i].Reset(_track.Start, _track.Direction);
      var car = new CarNetwork(this, _cars[i], _track);
      _carNets.Add(car);
    }
  }

  // Gets called by cars when they die
  public void CarDead(CarNetwork deadCar)
  {
    // This will be called indirectly from Update(), so we have to use a
    // pending removal list because cannot remove car from _carNets while
    // we are iterating over it
    _carNetsPendingRemoval.Add(deadCar); // add the car to pending removals

    if (deadCar.Fitness <= _bestFitness)
    {
      return;
    }

    _bestNeuralNetwork = deadCar.Network; // Make sure it becomes the best car
    _bestFitness = deadCar.Fitness; // And also set the best fitness
  }
}
