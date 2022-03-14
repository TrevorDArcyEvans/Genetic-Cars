// stolen from:
//      ReInventing Neural Networks
//          https://www.codeproject.com/Articles/1220276/ReInventing-Neural-Networks
//      ReInventing Neural Networks - Part 2
//          https://www.codeproject.com/Articles/1220644/ReInventing-Neural-Networks-Part
//      ReInventing Neural Networks - Part 3
//          https://www.codeproject.com/Articles/1231020/ReInventing-Neural-Networks-Part-2

namespace GeneticCars.Network;

public sealed class CarNetwork
{
    public static NeuralNetwork NextNetwork = new(new uint[] { 6, 4, 3, 2 }, null);

    // The fitness/score of the current car. Represents the number of checkpoints that his car hit.
    public int Fitness { get; private set; } 

    // The NeuralNetwork of the current car
    public NeuralNetwork TheNetwork { get; private set; } 
  
}
