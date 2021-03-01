My solution to this assignment is in C#. 
The IDE I used is Jetbrains Rider 2020.3.3, but I also verified this ran against Visual Studio 2019.
The application is using .NET472 framework. 

## Sequence Diagram of Solution
![Alt text](kitchencli/workflow/seqdiag.png?raw=true "Sequence Diagram")

## Whiteboarding session
I wanted to show what I did once I read the prompt. I immediately took to my white board. 
![Alt text](kitchencli/workflow/whiteboarding.jpg?raw=true "Whiteboarding the solution")

## Cli Usage
    Usage:
    -f <filepath> [required]
    -c <M for Match or F First-in-First-Out> [required]

### example for Fifo Courier to Order match
     kitchencli.exe -f C:\temp\dispatch_orders.json -c F
### example for Exact Match Courier to Order
     kitchencli.exe -f C:\temp\dispatch_orders.json -c M

### Exit application
Ctrl+C or wait until it is finished. 

    14:29:18.4457180 [TELEMETRY - FOOD] Average Order Ready to Pickup Time 72 ms
    14:29:18.4457180 [TELEMETRY - COURIER] Average Arrival to Pickup Time 127 ms
    All ordered delivered!
    Total Time: 2.97379163833333 minutes
    Exit
    
## Api
### ICourier interface
Provide an interface for defining a courier. 
```
/// <summary>
/// A Courier will be created by the CourierFactory
/// It will have a property for order Id
/// It will have a property for created time    
/// </summary>
public interface ICourier: ICourierTelemetry
{
    Action<ICourier> NotifyArrivedForOrder { get; set; }

    Order CurrentOrder { get; set; }
    /// <summary>
    /// returns the type of courier, for example ubereats, doordash, uncle-pete's fast food deliver svc
    /// </summary>
    /// <returns></returns>
    string CourierType();

    /// <summary>
    /// returns int, should be an enum...if theres time, of courier
    /// </summary>
    /// <returns></returns>
    int CourierTypeInt();

    /// <summary>
    /// The IOrderReadyPass object will assign the order and set this Id. 
    /// The Order assigned is by Match or FIFO, or otherwise (future)
    /// The Id will be used to pick up an Order. 
    /// </summary>
    Guid OrderId();

    /// <summary>
    /// The Time for arrival, will be a random value between 3-15seconds, after which this Courier
    /// will "arrive" for pickup
    /// </summary>
    int DurationEstimateInSeconds();

    /// <summary>
    /// This method is intended to start the internal timer that will run for DurationEstimate
    /// and once that elapses, it will call the FoodOrdeMaker to indicate "I've arrived for the order"
    /// </summary>
    void LeaveForFood();

    /// <summary>
    /// an Id to uniquely identify this Courier
    /// </summary>
    Guid CourierUniqueId { get; }

    /// <summary>
    /// To be called when returning to a CourierPool so that the next time this courier is used, it has
    /// a different Duration Time
    /// </summary>
    void RecalcDuration();
}
```

### ICourierOrderMatcher interface
This interface recieves an order ready for pick up and a courier who has arrived for an order.
The implementation can use whatever matching scheme desired, FIFO or Match for example. 
```
/// <summary>
/// This interface receives Orders and handles logic for
/// Couriers picking up orders
/// 
/// Concrete implementations of this interface can implement Match or First-in-First-Out logic for 
/// order hand off to Couriers
/// </summary>
public interface ICourierOrderMatcher
{
    /// <summary>
    /// To be called once an order is finished, this sets the order ready for pickup.
    /// This should match an order with a courier, if possible
    /// </summary>
    /// <param name="order"></param>
    void AddToOrderReadyQueue(Order order);

    /// <summary>
    /// To be called by when a courier has arrived, this allows a courier to be matched with a ready order, if possible
    /// </summary>
    /// <param name="courier"></param>
    void CourierArrived(ICourier courier);

    /// <summary>
    /// Returns one of two types: Match or Fifo
    /// </summary>
    /// <returns>MatchType enum with either Match or Fifo, set from Cli</returns>
    MatchType GetMatchType();
}
```

### ICourierOrderTelemetry interface
This interface is to be used to calculate average wait times for the Food & Courier
```
/// <summary>
/// Telemetry for managing time of food and courier
/// </summary>
public interface ICourierOrderTelemetry
{
    /// <summary>
    /// Called to keep running average of Food wait/pick up times
    /// </summary>
    /// <param name="orderTelemetry"></param>
    void CalculateAverageFoodWaitTime(IOrderTelemetry orderTelemetry);

    /// <summary>
    /// Called to keep running average of Courier wait/pick up times
    /// </summary>
    /// <param name="courierTelemetry"></param>
    void CalculateAverageCourierWaitTime(ICourierTelemetry courierTelemetry);
}
```

### ICourierPool interface
Because this is a real-time simulation, the logic was to instantiate any objects needed up front
to save time during the simulation from instantiating/"new"-ing up any new objects. 

Implementation can have a max object count and also have ability to create new objects if max is met, rather than have 
request wait/block for an available object
```
/// <summary>
/// This interface is intended to be a Courier Factory Method 
/// It exposes API for generating a Courier
/// we can have different types of couriers: doordash, grubhub, uber eats, etc
/// 
/// Implementations would create an Object of type ICourier
/// </summary>
public interface ICourierPool
{
    /// <summary>
    /// returns an instance of ICourier
    /// </summary>
    /// <returns></returns>
    ICourier GetCourier();

    /// <summary>
    /// an instance of ICourier is returned to the Courier Object Pool
    /// </summary>
    /// <param name="courier"></param>
    void ReturnCourier(ICourier courier);
}
```

### ICourierTelemetry interface
This interface allows for a courier to be extended to support telemetry
The implementation of the ICourierOrderTelemetry interface can receive this type of object and perform calculations
```
/// <summary>
/// interface for adding telemetry to a courier object
/// </summary>
public interface ICourierTelemetry
{
    /// <summary>
    /// The time the courier arrived to pick up an order
    /// </summary>
    DateTime ArrivalTime { get; set; }

    /// <summary>
    /// the time the courier picked up an order
    /// </summary>
    DateTime OrderPickupTime { get; set; }
}
```

### IKitchen interface
This interface is used to describe the minimum functionality of a kitchen. The kitchen receives
orders and couriers. The implementation of this interface should contain details for synchronizing those two.
In my implementation, the kitchen passes the ready orders & couriers that arrive to the ICourierOrderMatcher, which is 
passed in using Dependency Injection (at constructor). 
```
/// <summary>
/// The purpose of this api is to receive an Order, 
/// add the order to the order queue and send complete orders to "the pass" (ICourierOrderMatcher)
/// pickup by Couriers
/// 
/// IWO this interface is to manage the order's life cycle after it is received from the cli/json/order-originator
/// </summary>
public interface IKitchen
{
    /// <summary>
    /// Kitchen receives an order and uses preptime as indicator for how long it takes to prepare an order
    /// after which, order is deemed ready for pickup
    /// </summary>
    /// <param name="order"></param>
    void PrepareOrder(Order order);

    /// <summary>
    /// The Kitchen (establishment preparing the food) has a courier arrive and pick up an order
    /// </summary>
    /// <param name="courier"></param>
    void CourierHasArrived(ICourier courier);
}
```

### IKitchenCli interface
This interface is implemented by the bootstrapper. It manages the life-cycle of 
all the objects created, including disposing of any unmanaged resources. 
```
/// <summary>
/// interface implemented by the bootstrapper to control life-cycle of the application
/// </summary>
interface IKitchenCli
{
    /// <summary>
    /// starts all objects this bootstrapper is responsible for
    /// </summary>
    void Start();

    /// <summary>
    /// Initialize all objects and parse input from CLI
    /// </summary>
    /// <param name="jsonFile"></param>
    /// <param name="dipatcherType"></param>
    void Initialize(string jsonFile, DispatchCourierMatchEnum dipatcherType);

    /// <summary>
    /// Stop all objects this bootstrapper is responsible for
    /// </summary>
    void Stop();
}
```

### IOrderReceiver interface
This interface is what is used to receive orders from the input JSON file (order taker)
and it will also dispatch a courier for every order received
```
/// <summary>
/// The purpose of this interface is to receive an Order file
/// from the cli/json/order-originator
/// </summary>
interface IOrderReceiver
{
    /// <summary>
    /// Uses the orderId to ask factory for a Courier
    /// </summary>
    /// <param name="orderId"></param>
    /// <returns>a Courier</returns>
    void DispatchCourier(Order order);

    /// <summary>
    /// sends order to the order maker for making
    /// </summary>
    /// <param name="order"></param>
    void SendOrderToOrderMaker(Order order);
```

### IOrderTelemetry interface
interface for adding telemetry to an Order object
```
/// <summary>
/// interface for adding telemetry logic to an order
/// </summary>
public interface IOrderTelemetry
{
    /// <summary>
    /// To Be set when the order is deemed ready for pick up
    /// </summary>
    DateTime OrderReadyTime { get; set; }

    /// <summary>
    /// to be set when the order is actually picked up by a courier
    /// </summary>
    DateTime OrderPickUpTime { get; set; }
}
```

### IStartStoppableModule interface
Allows an object to implement this interface and be controlled as an abstract object rather than concrete object.
```
/// <summary>
/// interface for adding life-cycle control over modules
/// This is useful for starting/stopping any internal producer/consumers
/// and for disposing objects when stopping
/// </summary>
public interface IStartStoppableModule
{
    /// <summary>
    /// Stop a module
    /// </summary>
    void Start();

    /// <summary>
    /// Start a module
    /// </summary>
    void Stop();
}
```
# Objects in the System & Design
## Bootstrapper
I created a bootstrapper that puts all the APIs together with concerete implementations. It manages the life-cycle of the objects, ie. Start/Stop and IDisposable.
I make sure to inject the right interface implementation, FIFO vs Match and handle synchronizing the starts of each object. 
I also added a subscriber here to receive indication when the final order is received. I did that by counting the number of input orders
and using a ongoing count of how many times the publisher is called. 

## Orders
The way I designed Order objects was to make them self-expiring on their prep time.
I set their ManualResetEvent when the "chef" picked the order for Preparation. 
At that point, the "chef" is making the order and the self-expiring event would call the delegate I set on
the ICourierOrderMatcher object for matching "the pass" (I watch a lot of Hell's Kitchen :))

## Couriers
I had a little fun with this (and hopefully no Copyright infringements), and made an abstract
Courier object that implemented the ICourier interface. That abstract had some info like
Id and the logic for calcuation the duration time. 

I also implemented the couriers "arrival time" like the Orders, a ManualResetEvent. When the Courier
is "dispatched" I start the event and assign a callback to the ICourierOrderMatcher object.

## Courier Pool
At my last job, (ETAP RealTime), I once heard that "every instruction matters". That said, I felt that I should
try to limit creating objects and using strings (immutable). So I am hopeful I achieved that by creating a fixed number of courier
objects upfront and adding them to a pool. The ICourierPool is fascilitating couriers with every request. However, I 
found that 15-couriers was a good number for the file I was using (132 orders), but I did allow for additional Couriers
to be instantiated if the pool was depleted. Perhaps goes against the Object Pool design pattern, but I liked this idea. 

Update morning of 3/1: At first my implementation had concerte DoorDash, GrubHub, UberEats implementations of an abstract Courier class.
But then I realized that except for the CourierType property, "DoorDash", "GrubHub", "UberEats", there was no internal courier-specifc
logic. So I optimized them out, and updated the CourierPool to instead return a Courier object with a set type. 

Another thing I considered was, should I use something other than "Random" object for generating my couriers. I feel that it was my
idea to use concrete types of couriers, and there was no indication on the prompt, so I made it simple and went with random. I think 
I could have use a scheduling algorithm, round robin, fifo, etc, but for now I feel this satisfies the requirement. 
## OOP & Design Patterns 
### SOLID 
I really tried to stick to Single Responsibility, Open/Closed, Liskov, Interface seg & Dependency Inversion. 
S - it is why I split the workers the way I did and outlined them in the Sequence Diagram above & API
O - Courier is a good example, I made sure to extend it, rather than change it, ie. telemetry
L - I pass only base objects around, even in my UnitTests
I - an example in my work is that not all modules are requried to implement the IStartStoppableModule interface
D - I pass in the implementations by using the bootstrapper, yes, this is a monolithe and every impl in here references everything, but I think the theoretical principals still hold in my implementation. 

### Publisher/Subscriber
I used this to tell the bootstrapper when we were done delivering all the orders to that the application can exit on its own

### Producer/Consumer
I used a few. My consumers live as worker threads with locks around a queue that is filled by previous modules. 
I also used a consumer in the bootstrapper to get the JSON Orders into the system

### Concurrency
I am a fan of locks. I appreciate out of the box "concurrent" data structures, but I wanted to show that I am confortable with 
concurrency. In C#, async/await, is great and so I stayed away from instantiating any actual Threads and instead used Tasks 
so as to use the threadpool. 

### OOP
I tried to use all the design principals of OOP, Encapsulation, Abstraction, Polymorphism, Inheritance. 
I like Polymorphism, I'm a fan of using base objects. I think I succeeded with this, expecially with the ICourier object. 


## Unit Test
I tried to unit test as much of my API as possible. I hope that it is an seen as a **negative**, but I tend to use the encapsulation value "internal" on some
properties here and there for the purpose of controlling state within a unit test. I do this by setting the following in the AssemblyInfo.cs file for the project

```
[assembly: InternalsVisibleTo("unittests")]
```

I also use MOQ in my Unit Tests to help me control the dependencies and their state and expected values/setters/getters called.

I tried to show that in my UnitTests. 

# Possible Optimizations
I feel I should have exposed the the following through the Cli:
* max number of couriers value as an input value
* max number of chefs
I made some decisions of my own there with setting the max number of couriers, or perhaps I didn't because my object pool does allow you to create dynamic couriers if the pool is depleted, however the chefs (or producers of Orders for Matching) could probably have been a property set by the Cli. 

C# - I realize C# is not a real-time language. Possibly the GCC interferes here a bit with object management and the CPU cycles invovled with cleaning up managed memory. 
