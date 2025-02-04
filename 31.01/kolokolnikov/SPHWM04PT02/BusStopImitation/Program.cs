using static System.Console;
using System.Text;

namespace BusStopImitation {

public class Program {
    public const int Tick = 1000;
    public static bool DebugMode = false;
    public static bool ShouldTerminate = false;

    public static void Main(string[] args) {
        foreach (var arg in args) {
            switch (arg) {
            case "--debug":
                DebugMode = true;
                break;
            }
        }

        var network = Gen.GenerateBusStopNetwork(12);
        PrintDebugNetwork(network);

        Thread networkThread = new(network.Run);
        Thread printerThread = new(Printer.Run);
        networkThread.Start();
        printerThread.Start();
        List<Bus> buses = new() {
            new Bus(network, 1, 25, 20), new Bus(network, 2, 23, 16), new Bus(network, 3, 20, 24),
            new Bus(network, 4, 17, 22), new Bus(network, 5, 15, 26),
        };
        List<Thread> busThreads = new();
        foreach (var bus in buses) {
            var localBus = bus;
            busThreads.Add(new Thread(() => localBus.Run(1)));
        }
        foreach (var busthread in busThreads) {
            busthread.Start();
        }
        foreach (var busthread in busThreads) {
            busthread.Join();
        }
        ShouldTerminate = true;
        networkThread.Join();
        printerThread.Join();
    }

    public static void PrintDebugPath(List<BusStop> path) {
        if (!DebugMode)
            return;
        WriteLine("{0, -6} {1, -25} {2, -20} {3}", "Index", "Current Bus Stop", "Distance (meters)",
                  "Destination Bus Stop");
        WriteLine(new string('-', 80));
        bool isCyclable = false;
        for (int i = 0; i < path.Count; i++) {
            var currentStop = path[i];
            if (i < path.Count - 1) {
                var nextStop = path[i + 1];
                int distance = currentStop.GetDistanceTo(nextStop);
                WriteLine("{0, -6} {1, -25} {2, -20} {3}", i, currentStop.name, distance,
                          nextStop.name);
            } else {
                var firstStop = path[0];
                if (currentStop.GetConnection(firstStop) != null) {
                    isCyclable = true;
                    int distance = currentStop.GetDistanceTo(firstStop);
                    WriteLine("{0, -6} {1, -25} {2, -20} {3}", i, currentStop.name, distance,
                              $"{firstStop.name} (Return to Start)");
                } else {
                    WriteLine("{0, -6} {1, -25} {2, -20} {3}", i, currentStop.name, "",
                              "End of Path");
                }
            }
        }
        WriteLine(new string('-', 80));
        WriteLine(isCyclable ? "Path is cyclable." : "Path is not cyclable.");
    }

    public static void PrintDebugNetwork(Network network) {
        if (!DebugMode)
            return;
        Console.WriteLine("Bus Stops and Connections");
        Console.WriteLine(new string('-', 50));
        foreach (var busStop in network.GetAllBusStops()) {
            Console.WriteLine($"Bus Stop: {busStop.name}");
            foreach (var connection in busStop.connections) {
                Console.WriteLine(
                    $"    - Connection to {connection.destination.name} ({connection.distance} m)");
            }
            Console.WriteLine();
        }
    }
}

public class BusStop {
    private Random random = new();
    public string name { get; set; }
    public List<Connection> connections { get; } = new();
    public List<Passenger> passengers;
    private int capacity;
    private readonly object passengersLock = new();

    public BusStop(string name) {
        this.name = name;
        this.passengers = new();
        this.capacity = random.Next(5, 16);
    }
    public void CreatePassenger(Network network) {
        if (passengers.Count >= capacity)
            return;
        var person = new Passenger(network.GetRandomBusStopNameExcept(this.name));
        passengers.Add(person);
    }
    public int GetDistanceTo(BusStop destination) {
        var conn = GetConnection(destination);
        if (conn == null)
            throw new Exception("Bus Stops not connected.");
        return conn.distance;
    }
    public Connection? GetConnection(BusStop destination) {
        return connections.FirstOrDefault(c => c.destination == destination);
    }
    public object GetPassengersLock() {
        return passengersLock;
    }
}

public class Connection {
    public BusStop destination { get; }
    public int distance { get; } // meters
    public Connection(BusStop destination, int distance) {
        this.destination = destination;
        this.distance = distance;
    }
}

public class Network {
    private Dictionary<string, BusStop> network = new();
    private Random random = new();
    public static List<string> BusStopNames = new List<string> {
        "Main St & 1st Ave", "Downtown Station", "Central Park",    "City Hall",
        "Westside Mall",     "Eastside Plaza",   "Riverfront Park", "University Campus",
        "Hospital Station",  "Train Station",    "Northgate",       "Southgate",
        "Sports Complex",    "Library",          "Art Museum",
    };
    private List<string>? currentBusStopNames;

    public void AddBusStop(BusStop busStop) {
        if (!network.ContainsKey(busStop.name))
            network.Add(busStop.name, busStop);
    }
    public void ConnectBusStops(BusStop from, BusStop to, int distance) {
        var connectionFromTo = new Connection(to, distance);
        var connectionToFrom = new Connection(from, distance);
        from.connections.Add(connectionFromTo);
        to.connections.Add(connectionToFrom);
    }
    public IEnumerable<BusStop> GetAllBusStops() {
        return network.Values;
    }
    public BusStop FindBusStop(string name) {
        if (network.TryGetValue(name, out var busStop))
            return busStop;
        else
            throw new KeyNotFoundException($"Bus stop '{name}' not found.");
    }
    public BusStop RandomBusStop() {
        if (this.network.Count == 0)
            throw new Exception($"Network is empty: {this.network}");
        var keys = network.Keys.ToList();
        int idx = random.Next(0, keys.Count);
        return network[keys[idx]];
    }
    public List<string> GetBusStopNames() {
        if (currentBusStopNames == null) {
            currentBusStopNames = GetAllBusStops().Select(busStop => busStop.name).ToList();
        }
        return currentBusStopNames;
    }
    public string GetRandomBusStopNameExcept(string exception) {
        var names = GetBusStopNames();
        string selected;
        do {
            int idx = random.Next(names.Count);
            selected = names[idx];
        } while (selected == exception);
        return selected;
    }
    public void Run() {
        WriteLine("INFO: Network thread started.");
        while (!Program.ShouldTerminate) {
            foreach (var busStop in GetAllBusStops()) {
                busStop.CreatePassenger(this);
            }
            Thread.Sleep(5000);
        }
        WriteLine("INFO: Network thread finished.");
    }
}

public static class Gen {
    private static Random random = new();
    public static Network GenerateBusStopNetwork(int numberOfBusStops) {
        if (numberOfBusStops > Network.BusStopNames.Count) {
            throw new ArgumentException("Number of bus stops exceeds the available names.");
        }
        Network network = new Network();
        HashSet<int> selectedIndices = new HashSet<int>();
        Random random = new Random();
        while (selectedIndices.Count < numberOfBusStops) {
            int index = random.Next(Network.BusStopNames.Count);
            selectedIndices.Add(index);
        }
        List<BusStop> busStops = new List<BusStop>();
        foreach (int index in selectedIndices) {
            string busStopName = Network.BusStopNames[index];
            BusStop busStop = new BusStop(busStopName);
            network.AddBusStop(busStop);
            busStops.Add(busStop);
        }
        var existingConnections = new HashSet<(BusStop, BusStop)>();
        foreach (BusStop busStop in busStops) {
            int numberOfConnections = random.Next(1, busStops.Count - 1);
            var potentialConnections = busStops.Where(x => x != busStop).ToList();
            potentialConnections.Shuffle(random);
            for (int i = 0; i < numberOfConnections; i++) {
                BusStop randomStop = potentialConnections[i];
                if (!existingConnections.Contains((busStop, randomStop))) {
                    int distance = random.Next(100, 1000);
                    network.ConnectBusStops(busStop, randomStop, distance);
                    existingConnections.Add((busStop, randomStop));
                }
            }
        }
        return network;
    }
    public static List<BusStop> GenerateRandomCyclicPath(Network network) {
        List<BusStop> path;
        do {
            path = GenerateRandomCyclicPath(network, network.RandomBusStop());
        } while (!IsCyclablePath(path));
        return path;
    }
    public static List<BusStop> GenerateRandomCyclicPath(Network network, BusStop start) {
        var visited = new HashSet<BusStop>();
        var path = new List<BusStop>();
        var current = start;
        do {
            visited.Add(current);
            path.Add(current);
            var connections = new List<Connection>(current.connections);
            var random = new Random();
            connections = connections.OrderBy(x => random.Next()).ToList(); // Shuffle
            bool foundNext = false;
            foreach (var conn in connections) {
                if (!visited.Contains(conn.destination)) {
                    current = conn.destination;
                    foundNext = true;
                    break;
                }
            }
            if (!foundNext)
                break;
        } while (true);
        return path;
    }
    public static bool IsCyclablePath(List<BusStop> path) {
        var first = path[0];
        var last = path[^1];
        if (last.GetConnection(first) != null)
            return true;
        return false;
    }
}
public static class ListExtensions {
    public static void Shuffle<T>(this IList<T> list, Random rng) {
        int n = list.Count;
        while (n > 1) {
            int k = rng.Next(n--);
            (list[n], list[k]) = (list[k], list[n]); // Swap
        }
    }
}

public class Passenger {
    public string destination;
    public Passenger(string destination) {
        this.destination = destination;
    }
}

public static class Printer {
    public static Queue<string> messages = new();
    private static readonly object msgLock = new();
    public static object GetMsgLock() => msgLock;
    public static void AddMessage(string message) {
        messages.Enqueue(message);
    }
    private static char[] anim = { '\\', '|', '/', '–' };
    private static int curFrame = 0;
    private static char GetAnim() {
        if (curFrame == anim.Length - 1)
            curFrame = 0;
        else
            curFrame += 1;
        return anim[curFrame];
    }
    private static bool hasPrintedNotice = false;
    private static void PrintNotice() {
        Write($"Simulation is running... {GetAnim()}");
        hasPrintedNotice = true;
    }
    private static void RemoveNotice() {
        if (hasPrintedNotice)
            Write("\x1b[2K\x1b[0G");
        hasPrintedNotice = false;
    }
    public static void HideCursor() {
        Write("\x1b[?25l");
    }
    public static void ShowCursor() {
        Write("\x1b[?25h");
    }
    public static void Run() {
        string message;
        WriteLine("INFO: Printer thread started.");
        HideCursor();
        while (!Program.ShouldTerminate) {
            RemoveNotice();
            while (messages.Count > 0) {
                message = messages.Dequeue();
                WriteLine(message);
            }
            PrintNotice();
            Thread.Sleep(100);
        }
        ShowCursor();
        WriteLine("INFO: Printer thread finished.");
    }
}

public class Bus {
    private static Random rng = new();
    public List<BusStop> path = new(0);
    private int currentStopId = 0;
    public int timesCycled = 0;
    private int id;
    private int speedKmPerHour;
    private Network network;
    private int capacity;
    private List<Passenger> passengers = new();
    private static TimeSpan maxWaitAtStop = TimeSpan.FromSeconds(10);

    private static class Log {
        private static string Time() {
            return $"[{DateTime.Now:HH:mm:ss}]";
        }
        public static void Departure(Bus bus) {
            lock (Printer.GetMsgLock()) {
                Printer.AddMessage($"➡️ {Time()} Bus #{bus.id} departed from " +
                                   $"'{bus.GetCurrentStop().name}' to '{bus.GetNextStop().name}' " +
                                   $"with {bus.passengers.Count} passengers.");
            }
        }
        public static void Arrival(Bus bus) {
            lock (Printer.GetMsgLock()) {
                Printer.AddMessage(
                    $"✅ {Time()} Bus #{bus.id} arrived at '{bus.GetCurrentStop().name}'.");
            }
        }
        public static void LoadPassengers(Bus bus, int numberOfPassengers) {
            lock (Printer.GetMsgLock()) {
                Printer.AddMessage(
                    $"📦 {Time()} Bus #{bus.id} is loading " +
                    $"{numberOfPassengers} passengers at '{bus.GetCurrentStop().name}'.");
            }
        }
        public static void UnloadPassengers(Bus bus, int numberOfPassengers) {
            lock (Printer.GetMsgLock()) {
                Printer.AddMessage(
                    $"🧳 {Time()} Bus #{bus.id} is unloading " +
                    $"{numberOfPassengers} passengers at '{bus.GetCurrentStop().name}'.");
            }
        }
        public static void PreparingForJob(Bus bus) {
            lock (Printer.GetMsgLock()) {
                Printer.AddMessage($"🔧 {Time()} Preparing Bus #{bus.id} for the job with " +
                                   $"{bus.speedKmPerHour} km/h avg, {bus.capacity} seats.");
            }
        }
        public static void BeginRoute(Bus bus) {
            lock (Printer.GetMsgLock()) {
                Printer.AddMessage($"🚍 {Time()} Bus #{bus.id} begins the route.");
            }
        }
        public static void Idling(Bus bus) {
            lock (Printer.GetMsgLock()) {
                Printer.AddMessage(
                    $"🕰 {Time()} Bus #{bus.id} is idling at '{bus.GetCurrentStop().name}'.");
            }
        }
    }

    public Bus(Network network, int id, int speedKmPerHour, int capacity) {
        this.id = id;
        this.network = network;
        this.speedKmPerHour = speedKmPerHour;
        this.capacity = capacity;
    }

    public void Run(int cycles) {
        Log.PreparingForJob(this);
        this.path = Gen.GenerateRandomCyclicPath(network);
        Log.BeginRoute(this);
        while (timesCycled < cycles && !Program.ShouldTerminate) {
            IdleAtBusStop();
            DriveToNextStop();
        }
    }

    public void DriveToNextStop() {
        var currentStop = GetCurrentStop();
        var nextStop = GetNextStop();
        Log.Departure(this);
        double totalDistance = currentStop.GetDistanceTo(nextStop);
        double distanceCrossed = 0;
        int ticks = 0;
        while (distanceCrossed < totalDistance) {
            double speedMPerMilli = speedKmPerHour * (1000.0 / 60.0 / 60.0 / 1000.0);
            ReportPosition((int)distanceCrossed, (int)totalDistance);
            Thread.Sleep(Program.Tick);
            ticks++;
            distanceCrossed += speedMPerMilli * Program.Tick;
        }
        var tookTime = TimeSpan.FromMilliseconds(ticks * Program.Tick);
        IncrementStopId();
        Log.Arrival(this);
    }

    public void IdleAtBusStop() {
        var thisBusStop = GetCurrentStop();
        if (this.passengers.Count > 0)
            UnloadPassengers();
        if (thisBusStop.passengers.Count == 0) {
            Log.Idling(this);
            Thread.Sleep(5000);
        }
        if (this.passengers.Count < this.capacity && thisBusStop.passengers.Count > 0)
            LoadPassengers();
    }

    public void LoadPassengers() {
        var thisBusStop = GetCurrentStop();
        lock (thisBusStop.GetPassengersLock()) {
            int freeSpaces = this.capacity - this.passengers.Count;
            if (freeSpaces != 0) {
                var busStopNames = GetBusStopNames();
                List<Passenger> toLoad = new();
                foreach (var person in thisBusStop.passengers) {
                    if (busStopNames.Contains(person.destination) && freeSpaces > 0) {
                        toLoad.Add(person);
                        freeSpaces -= 1;
                    }
                }
                Log.LoadPassengers(this, toLoad.Count);
                foreach (var person in toLoad) {
                    thisBusStop.passengers.Remove(person);
                    this.passengers.Add(person);
                    Thread.Sleep(1000);
                }
            }
        }
    }

    public void UnloadPassengers() {
        var thisBusStop = path[currentStopId];
        List<Passenger> toUnload = new();
        foreach (var person in passengers) {
            if (person.destination == thisBusStop.name) {
                toUnload.Add(person);
            }
        }
        Log.UnloadPassengers(this, toUnload.Count);
        foreach (var person in toUnload) {
            passengers.Remove(person);
            Thread.Sleep(1000);
        }
    }

    public BusStop GetNextStop() {
        if (currentStopId == path.Count - 1) {
            return path[0];
        } else {
            return path[currentStopId + 1];
        }
    }

    public BusStop GetCurrentStop() {
        return path[currentStopId]!;
    }

    private void ReportPosition(int distanceCrossed, int totalDistance) {
        if (!Program.DebugMode)
            return;
        WriteLine("{0, -8}{1, -8}{2, -8}", $"Bus #{id}", $"{distanceCrossed}m",
                  $"{totalDistance}m");
    }

    private void IncrementStopId() {
        if (currentStopId == path.Count - 1) {
            currentStopId = 0;
            timesCycled += 1;
        } else
            currentStopId += 1;
    }

    public List<string> GetBusStopNames() {
        return path.Select(busStop => busStop.name).ToList();
    }
}

}
