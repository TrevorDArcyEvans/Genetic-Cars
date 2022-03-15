// stolen from:
//      ReInventing Neural Networks
//          https://www.codeproject.com/Articles/1220276/ReInventing-Neural-Networks
//      ReInventing Neural Networks - Part 2
//          https://www.codeproject.com/Articles/1220644/ReInventing-Neural-Networks-Part
//      ReInventing Neural Networks - Part 3
//          https://www.codeproject.com/Articles/1231020/ReInventing-Neural-Networks-Part-2

namespace GeneticCars.Network;

using Models;
using System.Drawing;
using System.Numerics;

public sealed class CarNetwork
{
  public static NeuralNetwork NextNetwork = new(new uint[] { 6, 4, 3, 2 }, null);

  // The fitness/score of the current car. Represents the number of checkpoints that his car hit.
  public int Fitness { get; private set; }

  // The NeuralNetwork of the current car
  public NeuralNetwork Network { get; }

  private readonly EvolutionManager _evMgr;
  private readonly Car _car;
  private readonly Track _track;

  public CarNetwork(EvolutionManager evMgr, Car car, Track track)
  {
    (_evMgr, _car, _track)  = (evMgr, car, track);

    // Sets the current network to the Next Network
    Network = NextNetwork;

    // Make sure the Next Network is reassigned to avoid having another car use the same network
    NextNetwork = new NeuralNetwork(NextNetwork.Topology, null); 

    #if false
    // Start checking if the score stayed the same for a lot of time
    new Thread(() => 
    {
      Thread.CurrentThread.IsBackground = true; 
      IsNotImproving();
    }).Start();
    #endif
  }

  public void Update()
  {
    GetNeuralInputAxis(out var linear, out var angular);

    // Moves the car
    //Move(linear, angular);
    Move(1, 1);
    
    TestCheckpointHit();
    //TestOffTrack();
  }

  // Casts all the rays, puts them through the NeuralNetwork and outputs the Move Axis
  // Vertical --> linear velocity
  // Horizontal --> angular velocity
  private void GetNeuralInputAxis(out double linear, out double angular)
  {
    // assumes 6 neurons in input layer
    var neuralInput = new double[NextNetwork.Topology[0]];

    // TODO   forward   neuralInput[0] = DistanceToTrackEdge(transform.forward, -Vector2.UnitX) / 4;  
    // TODO   back   neuralInput[1] = DistanceToTrackEdge(-transform.forward, -Vector3.forward) / 4;  
    // TODO   left   neuralInput[2] = DistanceToTrackEdge(transform.right, Vector3.right) / 4;  
    // TODO   right    neuralInput[3] = DistanceToTrackEdge(-transform.right, -Vector3.right) / 4;  

    const double SqrtHalf = 0.707;
    // TODO   forward-left   neuralInput[4] = DistanceToTrackEdge(transform.right * SqrtHalf + transform.forward * SqrtHalf, Vector3.right * SqrtHalf + Vector3.forward * SqrtHalf) / 4;  
    // TODO   forward-right   neuralInput[5] = DistanceToTrackEdge(transform.right * SqrtHalf + -transform.forward * SqrtHalf, Vector3.right * SqrtHalf + -Vector3.forward * SqrtHalf) / 4; 

    // Feed through the network
    // assumes 2 neurons in output layer
    var neuralOutput = Network.FeedForward(neuralInput);

    linear = neuralOutput[0] switch
    {
      // Get Vertical Value aka linear velocity
      // sigmoid activation function?
      <= 0.25f => -1,
      >= 0.75f => 1,
      _ => 0
    };

    angular = neuralOutput[1] switch
    {
      // Get Horizontal Value aka angular velocity
      // sigmoid activation function?
      <= 0.25f => -1,
      >= 0.75f => 1,
      _ => 0
    };

    // If the output is just standing still, then move the car forward
    if (linear == 0 && angular == 0)
    {
      linear = 1;
    }
  }

  // Measures distance from car to edge of track
  private double DistanceToTrackEdge(Point car, Vector2 rayDirection)
  {
    // TODO   DistanceToTrackEdge
    // Return the maximum distance
    return Car.LidarSenseDist;
  }

  // The main function that moves the car.
  // linear velocity
  // angular velocity in degrees
  //
  // NOTE:  both values will be either -1, 0, or +1
  private void Move(double linear, double angular)
  {
    // Move underlying car
    _car.Heading += angular;
    var deltaX = linear * Math.Sin(_car.Heading.ToRadians()) * 4;
    var deltaY = linear * Math.Cos(_car.Heading.ToRadians()) * 4;
    _car.Move((int)(deltaX), (int)(deltaY));
  }

  // This function is when the car hits any checkpoints
  private void TestCheckpointHit()
  {
    var carVec = new Vector2(_car.Position.X, _car.Position.Y);
    if (_track.Checkpoints.Any(cp => 
    {
      var cpVec = new Vector2(cp.X, cp.Y);
      var distSq = Vector2.DistanceSquared(carVec, cpVec);
      return distSq < 9;
    }))
    {
      // Increase Fitness/Score
      Fitness++;
    }
  }

  // check if car is off track
  private void TestOffTrack()
  {
    if (!_track.IsTrack(_car.Position.X, _car.Position.Y))
    {
      _evMgr.CarDead(this); // Tell the Evolution Manager that the car is dead
    }
  }

    // Checks each few seconds if the car didn't make any improvement
    private void IsNotImproving()
    {
      var oldFitness = Fitness; // Save the initial fitness
      while (true)
      {
        // wait for some time
        Thread.Sleep(TimeSpan.FromMilliseconds(100));
        if (oldFitness == Fitness) // Check if the fitness didn't change yet
        {
          _evMgr.CarDead(this); // Tell the Evolution Manager that the car is dead
        }
      }
    }
}
