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
  public NeuralNetwork Network { get; private set; }

  public void Update()
  {
    GetNeuralInputAxis(out var linear, out var angular);

    // Moves the car
    Move(linear, angular);
  }

  // Casts all the rays, puts them through the NeuralNetwork and outputs the Move Axis
  // Vertical --> linear velocity
  // Horizontal --> angular velocity
  private void GetNeuralInputAxis(out double linear, out double angular)
  {
    // assumes 6 neurons in input layer
    var neuralInput = new double[NextNetwork.Topology[0]];

    // TODO   forward   neuralInput[0] = CastRay(transform.forward, -Vector2.UnitX) / 4;  
    // TODO   back   neuralInput[1] = CastRay(-transform.forward, -Vector3.forward) / 4;  
    // TODO   left   neuralInput[2] = CastRay(transform.right, Vector3.right) / 4;  
    // TODO   right    neuralInput[3] = CastRay(-transform.right, -Vector3.right) / 4;  

    const double SqrtHalf = 0.707;
    // TODO   forward-left   neuralInput[4] = CastRay(transform.right * SqrtHalf + transform.forward * SqrtHalf, Vector3.right * SqrtHalf + Vector3.forward * SqrtHalf) / 4;  
    // TODO   forward-right   neuralInput[5] = CastRay(transform.right * SqrtHalf + -transform.forward * SqrtHalf, Vector3.right * SqrtHalf + -Vector3.forward * SqrtHalf) / 4; 

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
  public void Move(double linear, double angular)
  {
    // TODO     Move underlying car
    // TODO     CheckpointHit()
    // TODO     OffTrack()
  }

  // This function is when the car hits any checkpoints
  public void CheckpointHit()
  {
    // Increase Fitness/Score
    Fitness++;
  }

  // Called when the car goes off track
  public void OffTrack()
  {
    EvolutionManager.Instance.CarDead(this, Fitness); // Tell the Evolution Manager that the car is dead
  }
}
