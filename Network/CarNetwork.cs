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
  public NeuralNetwork TheNetwork { get; private set; }

  public void Update()
  {
    float Vertical; // linear velocity
    float Horizontal; // angular velocity

    GetNeuralInputAxis(out Vertical, out Horizontal);

    // Moves the car
    Move(Vertical, Horizontal);
  }

  // Casts all the rays, puts them through the NeuralNetwork and outputs the Move Axis
  // Vertical --> linear velocity
  // Horizontal --> angular velocity
  private void GetNeuralInputAxis(out float Vertical, out float Horizontal)
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
    var neuralOutput = TheNetwork.FeedForward(neuralInput);

    Vertical = neuralOutput[0] switch
    {
      // Get Vertical Value aka linear velocity
      // sigmoid activation function?
      <= 0.25f => -1,
      >= 0.75f => 1,
      _ => 0
    };

    Horizontal = neuralOutput[1] switch
    {
      // Get Horizontal Value aka angular velocity
      // sigmoid activation function?
      <= 0.25f => -1,
      >= 0.75f => 1,
      _ => 0
    };

    // If the output is just standing still, then move the car forward
    if (Vertical == 0 && Horizontal == 0)
    {
      Vertical = 1;
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
  // v --> linear velocity
  // h --> angular velocity
  //
  // NOTE:  both values will be either -1, 0, or +1
  public void Move(float v, float h)
  {
    // TODO     Move underlying car
  }

  // This function is called through all the checkpoints when the car hits any
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
