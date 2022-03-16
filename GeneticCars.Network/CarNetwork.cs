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

  // checkpoints we have already encountered
  // key = [px.X,px.Y]
  private readonly HashSet<string> _foundCheckPts = new();

  private Thread _improvementThread;

  public CarNetwork(EvolutionManager evMgr, Car car, Track track)
  {
    (_evMgr, _car, _track) = (evMgr, car, track);

    // Sets the current network to the Next Network
    Network = NextNetwork;

    // Make sure the Next Network is reassigned to avoid having another car use the same network
    NextNetwork = new NeuralNetwork(NextNetwork.Topology, null);

    _improvementThread = new Thread(() =>
    {
      Thread.CurrentThread.IsBackground = true;
      IsNotImproving();
    });
  }

  public void Update()
  {
    GetNeuralInputAxis(out var linear, out var angular);

    // Moves the car
    //Move(linear, angular);
    Move(1, 0);

    TestCheckpointHit();
    TestOffTrack();

    if (!OperatingSystem.IsBrowser() &&
        _improvementThread is not null &&
        !_improvementThread.IsAlive)
    {
      // NOTE:  not supported on webassembly
      // Start checking if the score stayed the same for a lot of time
      _improvementThread.Start();
    }
  }

  // Casts all the rays, puts them through the NeuralNetwork and outputs the Move Axis
  // Vertical --> linear velocity
  // Horizontal --> angular velocity
  private void GetNeuralInputAxis(out double linear, out double angular)
  {
    // assumes 6 neurons in input layer
    var neuralInput = new double[NextNetwork.Topology[0]];

    var Forward = -Vector2.UnitY;
    var Back = Vector2.UnitY;
    var Left = -Vector2.UnitX;
    var Right = Vector2.UnitX;
    var FwdLeft = Vector2.Normalize(new (-1, -1));
    var FwdRight = Vector2.Normalize(new (1, -1));

    neuralInput[0] = DistanceToTrackEdge(_car, Forward, _track) / 4;  
    neuralInput[1] = DistanceToTrackEdge(_car, Back, _track) / 4;  
    neuralInput[2] = DistanceToTrackEdge(_car, Left, _track) / 4;  
    neuralInput[3] = DistanceToTrackEdge(_car, Right, _track) / 4;  
    neuralInput[4] = DistanceToTrackEdge(_car, FwdLeft, _track) / 4;  
    neuralInput[5] = DistanceToTrackEdge(_car, FwdRight, _track) / 4; 

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
  private static double DistanceToTrackEdge(Car car, Vector2 lidar, Track track)
  {
    var rot = Matrix3x2.CreateRotation((float)track.Direction.ToRadians());
    var rotLidar = Vector2.Transform(lidar, rot);
    var carPos = new Vector2(car.Position.X, car.Position.Y);
    for (int i = 1; i <= Car.LidarSenseDist; i++)
    {
      var endPt = carPos + i * rotLidar;
      var dist = Vector2.Distance(carPos, endPt);
      if (dist > Car.LidarSenseDist)
      {
        break;
      }

      if (!track.IsTrack((int)endPt.X, (int)endPt.Y))
      {
        return dist;
      }
    }

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
    var deltaY = -linear * Math.Cos(_car.Heading.ToRadians()) * 4;
    _car.Move((int)(deltaX), (int)(deltaY));
  }

  // This function is when the car hits any checkpoints
  private void TestCheckpointHit()
  {
    var carVec = new Vector2(_car.Position.X, _car.Position.Y);
    if (_track.Checkpoints
        .Where(cp => !_foundCheckPts.Contains(cp.ToString()) )
        .Any(cp =>
    {
      var cpVec = new Vector2(cp.Position.X, cp.Position.Y);
      var distSq = Vector2.DistanceSquared(carVec, cpVec);
      if (!(distSq < Checkpoint.RadiusSquared))
      {
        return false;
      }

      _foundCheckPts.Add(cp.ToString());
      return true;

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
    while (true)
    {
      // Save the initial fitness
      var oldFitness = Fitness;

      // wait for some time
      Thread.Sleep(TimeSpan.FromSeconds(2));
      if (oldFitness != Fitness)
      {
        continue;
      }

      _evMgr.CarDead(this); // Tell the Evolution Manager that the car is dead
      _improvementThread = null;
      break;
    }
  }
}
