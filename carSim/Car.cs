namespace carSim
{
    public class Car
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Speed { get; set; }
        public double Distance { get; set; }
        public TimeSpan? RaceTime { get; set; }
        public int Winner { get; set; }
        public int RaceFinished { get; set; }

        public Car(int id, string name, decimal distance, TimeSpan? raceTime, int speed, int winner, int raceFinnished)
        {
            Id = id;
            Name = name;
            Distance = Distance;
            Speed = speed;
            RaceTime = raceTime;
            Winner = winner;
            RaceFinished = raceFinnished;
        }
    }
}
