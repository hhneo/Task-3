using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Digests;



namespace NonTransitiveDice
{
    public class Die
    {
        public int[] Faces { get; }
        public Die(string faceString) => Faces = faceString.Split(',').Select(int.Parse).ToArray();
        public int GetFace(int index) => Faces[index];
        public override string ToString() => $"[{string.Join(",", Faces)}]";
        public string ToStringWithoutBrackets() => string.Join(",", Faces);
    }

    public class FairRandomGenerator
    {
        public (byte[] key, int number) GenerateRandomNumber(int maxValue)
        {
            byte[] key = RandomNumberGenerator.GetBytes(32);
            int number = RandomNumberGenerator.GetInt32(0, maxValue + 1);
            return (key, number);
        }

        public string CalculateHmac(byte[] key, int number)
        {
            using var hmac = new HMACSHA256(key);
            byte[] hash = hmac.ComputeHash(BitConverter.GetBytes(number));
            return BitConverter.ToString(hash).Replace("-", "");
        }
    }

    public class ProbabilityCalculator
    {
        public Dictionary<(Die, Die), double> CalculateProbabilities(List<Die> dice)
        {
            var probabilities = new Dictionary<(Die, Die), double>();

            foreach (var dieA in dice)
            {
                foreach (var dieB in dice)
                {
                    int wins = dieA.Faces.Sum(faceA => dieB.Faces.Count(faceB => faceA > faceB));
                    int total = dieA.Faces.Length * dieB.Faces.Length;
                    probabilities[(dieA, dieB)] = (double)wins / total;
                }
            }
            return probabilities;
        }
    }

    public class HelpTableGenerator
    {
        public void ShowProbabilityTable(List<Die> dice)
        {
            var calculator = new ProbabilityCalculator();
            var probabilities = calculator.CalculateProbabilities(dice);

            Console.WriteLine("\nProbability of the win for the user:");

            // Calculate column widths
            int firstColWidth = Math.Max("User dice v".Length,
                dice.Max(d => d.ToStringWithoutBrackets().Length));
            int otherColWidth = dice.Max(d => d.ToStringWithoutBrackets().Length) + 2;

            // Print header
            Console.Write("| " + "User dice v".PadRight(firstColWidth) + " |");
            foreach (var die in dice)
                Console.Write(" " + die.ToStringWithoutBrackets().PadRight(otherColWidth - 1) + " |");
            Console.WriteLine();

            Console.Write("|" + new string('-', firstColWidth + 2) + "|");
            foreach (var die in dice)
                Console.Write(new string('-', otherColWidth) + "|");
            Console.WriteLine();

            // Print rows
            foreach (var userDie in dice)
            {
                Console.Write("| " + userDie.ToStringWithoutBrackets().PadRight(firstColWidth) + " |");
                foreach (var compDie in dice)
                {
                    if (userDie == compDie)
                        Console.Write(" " + "- (0.3333)".PadRight(otherColWidth - 1) + " |");
                    else
                        Console.Write(" " + probabilities[(userDie, compDie)].ToString("0.0000").PadRight(otherColWidth - 1) + " |");
                }
                Console.WriteLine();
            }
        }
    }

    public class GameEngine
    {
        private readonly List<Die> dice;
        private readonly FairRandomGenerator random = new FairRandomGenerator();
        private readonly HelpTableGenerator helpTable = new HelpTableGenerator();
        private readonly ProbabilityCalculator probabilityCalculator = new ProbabilityCalculator();

        public GameEngine(IEnumerable<Die> dice) => this.dice = dice.ToList();

        public void StartGame()
        {
            Console.WriteLine("Let's determine who makes the first move.");
            var (key, computerChoice) = random.GenerateRandomNumber(1);
            string hmac = random.CalculateHmac(key, computerChoice);

            Console.WriteLine($"I selected a random value in the range 0..1 (HMAC={hmac}).");
            Console.WriteLine("Try to guess my selection.");
            Console.WriteLine("0 - 0");
            Console.WriteLine("1 - 1");
            Console.WriteLine("X - exit");
            Console.WriteLine("? - help");
            Console.Write("Your selection: ");

            string input = Console.ReadLine();
            if (input == "X") return;
            if (input == "?")
            {
                ShowHelp();
                Console.Write("Your selection: ");
                input = Console.ReadLine();
            }

            int userGuess = int.Parse(input);
            Console.WriteLine($"My selection: {computerChoice} (KEY={BitConverter.ToString(key).Replace("-", "")}).");

            bool userFirst = userGuess == computerChoice;
            if (!userFirst)
            {
                int computerDieIndex = RandomNumberGenerator.GetInt32(0, dice.Count);
                Die computerDie = dice[computerDieIndex];
                Console.WriteLine($"I make the first move and choose the {computerDie} dice.");
                PlayRound(null, computerDie);
            }
            else
            {
                Console.WriteLine("You make the first move.");
                PlayRound(SelectDie(), null);
            }
        }

        private Die SelectDie()
        {
            Console.WriteLine("Choose your dice:");
            for (int i = 0; i < dice.Count; i++)
                Console.WriteLine($"{i} - {dice[i].ToStringWithoutBrackets()}");
            Console.WriteLine("X - exit");
            Console.WriteLine("? - help");
            Console.Write("Your selection: ");

            string input = Console.ReadLine();
            if (input == "X") Environment.Exit(0);
            if (input == "?") { ShowHelp(); return SelectDie(); }

            return dice[int.Parse(input)];
        }

        private void PlayRound(Die userDie, Die computerDie)
        {
            userDie ??= SelectDie();
            computerDie ??= dice.Where(d => d != userDie).ElementAt(
                RandomNumberGenerator.GetInt32(0, dice.Count - 1));

            Console.WriteLine($"You choose the {userDie} dice.");

            int computerRoll = PerformRoll("my", computerDie.Faces.Length);
            int userRoll = PerformRoll("your", userDie.Faces.Length);

            Console.WriteLine($"My roll result is {computerDie.GetFace(computerRoll)}.");
            Console.WriteLine($"Your roll result is {userDie.GetFace(userRoll)}.");

            if (userDie.GetFace(userRoll) > computerDie.GetFace(computerRoll))
                Console.WriteLine($"You win ({userDie.GetFace(userRoll)} > {computerDie.GetFace(computerRoll)})!");
            else
                Console.WriteLine($"I win ({computerDie.GetFace(computerRoll)} > {userDie.GetFace(userRoll)})!");
        }

        private int PerformRoll(string owner, int faceCount)
        {
            Console.WriteLine($"It's time for {owner} roll.");
            var (key, computerNumber) = random.GenerateRandomNumber(faceCount - 1);
            string hmac = random.CalculateHmac(key, computerNumber);

            Console.WriteLine($"I selected a random value in the range 0..{faceCount - 1} (HMAC={hmac}).");
            Console.WriteLine($"Add your number modulo {faceCount}.");
            for (int i = 0; i < faceCount; i++)
                Console.WriteLine($"{i} - {i}");
            Console.WriteLine("X - exit");
            Console.WriteLine("? - help");
            Console.Write("Your selection: ");

            string input = Console.ReadLine();
            if (input == "X") Environment.Exit(0);
            if (input == "?") { ShowHelp(); return PerformRoll(owner, faceCount); }

            int userNumber = int.Parse(input);
            Console.WriteLine($"My number is {computerNumber} (KEY={BitConverter.ToString(key).Replace("-", "")}).");

            int result = (userNumber + computerNumber) % faceCount;
            Console.WriteLine($"The fair number generation result is {userNumber} + {computerNumber} = {result} (mod {faceCount}).");
            return result;
        }

        private void ShowHelp()
        {
            helpTable.ShowProbabilityTable(dice);
            Console.WriteLine("\nGame Rules:");
            Console.WriteLine("- First move is determined by guessing 0 or 1");
            Console.WriteLine("- Select different dice than your opponent");
            Console.WriteLine("- Each roll uses fair random number generation");
            Console.WriteLine("- Higher roll wins the round");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (args.Length < 3)
                {
                    Console.WriteLine("Error: At least 3 dice required. Example:");
                    Console.WriteLine("game.exe 1,2,3 4,5,6 7,8,9");
                    return;
                }

                var dice = args.Select(arg => new Die(arg)).ToList();
                new GameEngine(dice).StartGame();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}