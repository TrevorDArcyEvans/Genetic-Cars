# Genetic Cars
Inspired by and based on:
* [ReInventing Neural Networks](https://www.codeproject.com/Articles/1220276/ReInventing-Neural-Networks)
* [ReInventing Neural Networks - Part 2](https://www.codeproject.com/Articles/1220644/ReInventing-Neural-Networks-Part)
* [ReInventing Neural Networks - Part 3](https://www.codeproject.com/Articles/1231020/ReInventing-Neural-Networks-Part-2)

## Neural Network
* input layer = 6 neurons
  *  LIDAR sensors:
    *  forward
    *  back
    *  left
    *  right
    *  forward-left
    *  forward-right

* two hidden layers = 4 & 3 neurons

    [How Many Hidden Layers and Hidden Nodes Does a Neural Network Need?](https://www.allaboutcircuits.com/technical-articles/how-many-hidden-layers-and-hidden-nodes-does-a-neural-network-need/)
```
      How Many Hidden Layers?
      [With] one hidden layer allows a neural network to approximate any function involving 
      “a continuous mapping from one finite space to another.”
      With two hidden layers, the network is able to “represent an arbitrary decision boundary 
      to arbitrary accuracy.”

      How Many Hidden Nodes?
      Dr. Heaton mentions three rules of thumb for choosing the dimensionality of a hidden 
      layer. I’ll build upon these by offering recommendations based on my vague 
      signal-processing intuition.

      1. If the network has only one output node and you believe that the required input–output 
      relationship 
          is fairly straightforward, start with a hidden-layer dimensionality that is equal to 
          two-thirds of the input dimensionality.
      2. If you have multiple output nodes or you believe that the required input–output 
          relationship is complex, make the hidden-layer dimensionality equal to the input 
          dimensionality plus the output dimensionality (but keep it less than twice the input 
          dimensionality).
      3. If you believe that the required input–output relationship is extremely complex, set 
         the hidden dimensionality to one less than twice the input dimensionality.
```

    [How to Configure the Number of Layers and Nodes in a Neural Network](https://machinelearningmastery.com/how-to-configure-the-number-of-layers-and-nodes-in-a-neural-network/)
```
      In fact, there is a theoretical finding by Lippmann in the 1987 paper “An introduction to 
      computing with neural nets” that shows that an MLP with two hidden layers is sufficient 
      for creating classification regions of any desired shape.
      This is instructive, although it should be noted that no indication of how many nodes to 
      use in each layer or how to
      learn the weights is given.
      ...
      ... In practice, we again have no idea how many nodes to use in the single hidden layer 
      for a given problem nor how
      to learn or set their weights effectively. ...
```

* output layer = 2 neurons
  *  linear velocity
  *  angular velocity

