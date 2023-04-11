using System.Diagnostics;

namespace carSim
{

    //make function to update race cars , fix foreach loop not print out more than once , fix ui with colors 
    // IF TIME make functio nto att asmany cars as you want , delete printed event / update event instead
    internal class Program
    {
        static async Task Main(string[] args)
        {
            await CarSimulation();
        }

        public static async Task CarSimulation()
        {

            Console.WriteLine("Welcome to the race simulation, press [Any key] to start.");
            Console.WriteLine("During the race press [Enter] to get a status update on the cars.");
            Console.ReadKey();
            //Create car objects
            Car leoCar = new Car(1, "[Leo]", 0, null, 120, 0, 0);
            Car mimmiCar = new Car(1, "[Mimmi]", 0, null, 120, 0, 0);
            Car catMobile = new Car(1, "[Fibbe]", 0, null, 120, 0, 0);

            // Runs the race simulation for each car
            var leoRace = Race(leoCar);
            var mimmiRace = Race(mimmiCar);
            var catRace = Race(catMobile);

            //List ti check on the updates of each car 
            var carRaceStatus = carCheck(new List<Car> { leoCar, mimmiCar, catMobile });
            //List to check if car is winner or sore loser
            var carList = new List<Task<Car>> { leoRace, mimmiRace, catRace };
            var finishedCars = new List<Car>();

            Car winner = null;
            bool winnerFound = false;
            //A while loop to determin who crosses the finish line first 
            //This while loop should be able to run as many cars as you want 
            while (carList.Count > 0)
            {
                var finishedRace = await Task.WhenAny(carList);
                finishedCars.Add(finishedRace.Result);
                carList.Remove(finishedRace);

                foreach (var car in finishedCars)
                {
                    if (car != null && car.Distance >= 10 && !winnerFound)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        winner = car;
                        car.Winner++;
                        car.RaceFinished++;
                        winnerFound = true;
                        Console.WriteLine("----------------------------------------------------");
                        Console.WriteLine($"   ------------{winner.Name} is the winner------------");
                        Console.WriteLine("----------------------------------------------------");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else if (car.Winner == 0 && car.RaceFinished == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        car.RaceFinished++;
                        Console.WriteLine($"------------{car.Name} lost the race------------");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
            }
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("------------------------------------------------------------------------");
            Console.WriteLine($"   ---The simulation has ended and {winner.Name} is the winner Horaaay!---");
            Console.WriteLine("------------------------------------------------------------------------");

        }

        //Method to simulte the race, a stopwatch to keep track of time and a if check if cars har less than 30 sec to finish line 
        //the if check is in place to not run the random event if the car has less than 30 sec
        public static async Task<Car> Race(Car car)
        {
            Stopwatch timer = new();
            timer.Start();
            bool raceActive = true;
            int GoalDistans = 10;
            while (raceActive)
            {
                car.RaceTime = timer.Elapsed * 10;
                double distancePerSecond = car.Speed / 3600.0;
                double distanceTraveld = distancePerSecond * 30;
                double lastDistance = (GoalDistans - car.Distance) / distancePerSecond;
                if (car.Distance < GoalDistans)
                {

                    if (car.Distance + distanceTraveld >= GoalDistans)
                    {

                        await Wait(lastDistance);
                        timer.Stop();
                        car.RaceTime = timer.Elapsed * 10;
                        car.Distance += lastDistance * distancePerSecond;
                        carUpdate(car);
                        raceActive = false;
                    }
                    else
                    {


                        await RandomEvent(car);
                    }
                }
            }
            return car;
        }


        //Method to cause random event happening during the race
        //adds time to cars if they encounter anyting , else just runs it still adds
        // 30 sec to the timer becouse the event only happens each 30 sec
        public static async Task RandomEvent(Car car)
        {
            int rndNum = random.Next(50);
            await Wait(30);
            switch (rndNum)
            {
                case 0:
                    Console.WriteLine($"\r{car.Name}Ran out of gas! What a slow refill...");
                    await Wait(30);
                    break;
                case int n when (n > 0 && n <= 2):
                    Console.WriteLine($"\r{car.Name} got a flat tier , need to change it!\r");
                    await Wait(20);
                    break;
                case int n when (n > 2 && n <= 7):
                    Console.WriteLine($"\r{car.Name} hit a bird , it splatterd on the windshield!\r");
                    await Wait(10); ;
                    break;
                case int n when (n > 7 && n <= 17):
                    Console.WriteLine($"\r{car.Name} got some engine problems, car is going 1km/h slower\r");
                    car.Speed--;
                    double distanceTraveld1 = CarSpeedUpdate(car);
                    car.Distance += distanceTraveld1;

                    break;
                default:
                    double distanceTraveld = CarSpeedUpdate(car);
                    car.Distance += distanceTraveld;
                    break;
            }

        }
        //Method to update the car when they have crossed the finish line
        public static void carUpdate(Car car)
        {
            string time = string.Format(@"{0:mm\:ss\:ff}", car.RaceTime);
            Console.WriteLine($"\n{car.Name} crossed the finish line driving {car.Speed}km/h! {car.Name} had a time of {time}");
        }


        //Method to check traveld distance and speed during the race
        public static async Task carCheck(List<Car> cars)
        {
            while (true)
            {
                await Wait(10);
                if (Console.ReadKey(true).Key == ConsoleKey.Enter)
                {
                    foreach (var car in cars)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"\n--Status of {car.Name} "); Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine($"Distance traveled: {Math.Truncate(car.Distance * 100) / 100}km Speed-{car.Speed} km/h --");
                        Console.WriteLine("---------------------------------------------");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
                var totalDistance = cars.Select(car => car.RaceFinished).Sum();
                if (totalDistance >= 3)
                {
                    return;
                }
            }

        }
        private static Random random = new Random();
        public async static Task Wait(double delay = 30)
        {
            await Task.Delay(TimeSpan.FromSeconds(delay / 10));
        }

        //Method to update car speed during the race
        public static double CarSpeedUpdate(Car car)
        {
            double distancePerSecond = car.Speed / 3600.0;
            double distanceTraveld = distancePerSecond * 30;
            return distanceTraveld;

        }

    }
}