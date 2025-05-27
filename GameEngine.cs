using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

//namespace NonTransitiveDice
//{
//    public class GameEngine
//    {
//        private readonly List<Die> dice;
//        private readonly FairRandomGenerator random = new FairRandomGenerator();
//        private readonly HelpTableGenerator helpTable = new HelpTableGenerator();

//        public GameEngine(IEnumerable<Die> dice) => this.dice = dice.ToList();

//        public void StartGame()
//        {
//            // First move determination
//            var (compKey, compFirstMove, compHmac) = random.GenerateCommitment(1);
//            Console.WriteLine($"First move commitment (HMAC): {compHmac}");
//            Console.WriteLine("Enter your guess (0 or 1):");
//            Console.WriteLine("? - help");

//            string input = GetUserInput(new[] {"0", "1"});
//            if (input == "?") 
//            {
//                ShowHelp();
//                StartGame(); // Restart after showing help
//                return;
//            }

//            int userGuess = int.Parse(input);
//            Console.WriteLine($"Computer's key: {BitConverter.ToString(compKey)}");
//            Console.WriteLine($"Computer's move: {compFirstMove}");

//            bool userFirst = userGuess == compFirstMove;
//            Console.WriteLine($"{(userFirst ? "You" : "Computer")} goes first");

//            // Dice selection
//            Die userDie = SelectDie();
//            var (compDieKey, compDieIndex, compDieHmac) = random.GenerateCommitment(dice.Count - 1);
//            Console.WriteLine($"Computer's die commitment (HMAC): {compDieHmac}");
//            Console.WriteLine("Enter your die modifier (0):");
//            Console.WriteLine("? - help");

//            input = GetUserInput(Enumerable.Range(0, dice.Count).Select(x => x.ToString()));
//            if (input == "?")
//            {
//                ShowHelp();
//                StartGame(); // Restart after showing help
//                return;
//            }
//            int userDieMod = int.Parse(input);

//            Console.WriteLine($"Computer's die key: {BitConverter.ToString(compDieKey)}");
//            Console.WriteLine($"Computer's die index: {compDieIndex}");
//            Die compDie = dice.Where(d => d != userDie).ElementAt(
//                random.CalculateFairResult(userDieMod, compDieIndex, dice.Count - 1));

//            // Perform fair rolls
//            PerformFairRolls(userDie, compDie);
//        }

//        private string GetUserInput(IEnumerable<string> validOptions)
//        {
//            while (true)
//            {
//                Console.Write("Your selection: ");
//                string input = Console.ReadLine();

//                if (input == "?") return "?";
//                if (input == "X") Environment.Exit(0);
//                if (validOptions.Contains(input)) return input;

//                Console.WriteLine("Invalid input. Please try again.");
//            }
//        }

//        private Die SelectDie()
//        {
//            Console.WriteLine("Choose your dice:");
//            for (int i = 0; i < dice.Count; i++)
//                Console.WriteLine($"{i} - {dice[i].ToStringWithoutBrackets()}");

//            Console.WriteLine("X - exit");
//            Console.WriteLine("? - help");

//            string input = GetUserInput(Enumerable.Range(0, dice.Count).Select(x => x.ToString()));
//            if (input == "?") 
//            {
//                ShowHelp();
//                return SelectDie();
//            }

//            return dice[int.Parse(input)];
//        }

//        //private void PlayRound(Die userDie, Die computerDie)
//        //{
//        //    userDie ??= SelectDie();
//        //    computerDie ??= dice.Where(d => d != userDie).ElementAt(
//        //        RandomNumberGenerator.GetInt32(0, dice.Count - 1));

//        //    Console.WriteLine($"You choose the {userDie} dice.");

//        //    int computerRoll = PerformFairRolls("my", computerDie);
//        //    int userRoll = PerformFairRolls("your", userDie);

//        //    int computerResult = computerDie.GetFace(computerRoll);
//        //    int userResult = userDie.GetFace(userRoll);

//        //    Console.WriteLine($"My roll result is {computerResult}.");
//        //    Console.WriteLine($"Your roll result is {userResult}.");

//        //    if (userResult > computerResult)
//        //        Console.WriteLine($"You win ({userResult} > {computerResult})!");
//        //    else if (computerResult > userResult)
//        //        Console.WriteLine($"I win ({computerResult} > {userResult})!");
//        //    else
//        //        Console.WriteLine($"It's a draw ({userResult} = {computerResult})!");
//        //}
//        private void PerformFairRolls(Die userDie, Die computerDie)
//        {
//            // Computer's roll
//            var (compRollKey, compRollNum, compRollHmac) = random.GenerateCommitment(computerDie.Faces.Length - 1);
//            Console.WriteLine($"\nComputer's roll commitment (HMAC): {compRollHmac}");
//            Console.WriteLine($"Enter your roll modifier (0-{computerDie.Faces.Length - 1}):");
//            Console.WriteLine("? - help");

//            string input = GetUserInput(Enumerable.Range(0, computerDie.Faces.Length).Select(x => x.ToString()));
//            if (input == "?")
//            {
//                ShowHelp();
//                PerformFairRolls(userDie, computerDie);
//                return;
//            }
//            int userRollMod = int.Parse(input);

//            Console.WriteLine($"Computer's roll key: {BitConverter.ToString(compRollKey)}");
//            Console.WriteLine($"Computer's roll number: {compRollNum}");
//            int compResult = random.CalculateFairResult(userRollMod, compRollNum, computerDie.Faces.Length);

//            // User's roll
//            var (userRollKey, userRollNum, userRollHmac) = random.GenerateCommitment(userDie.Faces.Length - 1);
//            Console.WriteLine($"\nYour roll commitment (HMAC): {userRollHmac}");
//            Console.WriteLine($"Enter computer's roll modifier (0-{userDie.Faces.Length - 1}):");
//            Console.WriteLine("? - help");

//            input = GetUserInput(Enumerable.Range(0, userDie.Faces.Length).Select(x => x.ToString()));
//            if (input == "?")
//            {
//                ShowHelp();
//                PerformFairRolls(userDie, computerDie);
//                return;
//            }
//            int compRollMod = int.Parse(input);

//            Console.WriteLine($"Your roll key: {BitConverter.ToString(userRollKey)}");
//            Console.WriteLine($"Your roll number: {userRollNum}");
//            int userResult = random.CalculateFairResult(compRollMod, userRollNum, userDie.Faces.Length);

//            // Results
//            int compFinal = computerDie.GetFace(compResult);
//            int userFinal = userDie.GetFace(userResult);

//            Console.WriteLine($"\nComputer rolled: {compFinal} (from {computerDie})");
//            Console.WriteLine($"You rolled: {userFinal} (from {userDie})");

//            if (userFinal > compFinal) Console.WriteLine("You win!");
//            else if (compFinal > userFinal) Console.WriteLine("Computer wins!");
//            else Console.WriteLine("It's a draw!");
//        }

//        private void ShowHelp()
//        {
//            helpTable.ShowProbabilityTable(dice);
//            Console.WriteLine("\nGame Rules:");
//            Console.WriteLine("- First move is determined by guessing 0 or 1");
//            Console.WriteLine("- Select different dice than your opponent");
//            Console.WriteLine("- Each roll uses fair random number generation");
//            Console.WriteLine("- Higher roll wins the round");
//            Console.WriteLine("- '?' shows this help");
//            Console.WriteLine("- 'X' exits the game");
//            Console.WriteLine("\nPress any key to continue...");
//            Console.ReadKey();
//            Console.Clear();
//        }
//    }
//}

namespace NonTransitiveDice
{
    public class GameEngine
    {
        private readonly List<Die> dice;
        private readonly FairRandomGenerator random = new FairRandomGenerator();
        private readonly HelpTableGenerator helpTable = new HelpTableGenerator();

        public GameEngine(IEnumerable<Die> dice) => this.dice = dice.ToList();

        public void StartGame()
        {
            Console.WriteLine("Let's determine who makes the first move.");

            var (key, computerChoice, hmac) = random.GenerateCommitment(1);
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
                Console.WriteLine();
                PlayRound(null, computerDie);
            }
            else
            {
                Console.WriteLine("You make the first move.");
                Console.WriteLine();
                PlayRound(SelectDie(), null);
            }
        }

        private Die SelectDie()
        {
            while (true)
            {
                Console.WriteLine("Choose your dice:");
                for (int i = 0; i < dice.Count; i++)
                    Console.WriteLine($"{i} - {dice[i].ToStringWithoutBrackets()}");
                Console.WriteLine("X - exit");
                Console.WriteLine("? - help");
                Console.Write("Your selection: ");

                string input = Console.ReadLine();
                if (input == "X") Environment.Exit(0);
                if (input == "?")
                {
                    ShowHelp();
                    continue;
                }

                if (int.TryParse(input, out int index) && index >= 0 && index < dice.Count)
                    return dice[index];

                Console.WriteLine("Invalid selection.");
            }
        }

        private void PlayRound(Die userDie, Die computerDie)
        {
            userDie ??= SelectDie();
            computerDie ??= dice.Where(d => d != userDie).ElementAt(
                RandomNumberGenerator.GetInt32(0, dice.Count - 1));

            Console.WriteLine($"You choose the {userDie} dice.");
            Console.WriteLine();

            int computerRoll = PerformRoll("my", computerDie);
            int userRoll = PerformRoll("your", userDie);

            int computerResult = computerDie.GetFace(computerRoll);
            int userResult = userDie.GetFace(userRoll);

            Console.WriteLine($"My roll result is {computerResult}.");
            Console.WriteLine($"Your roll result is {userResult}.");
            Console.WriteLine();

            if (userResult > computerResult)
                Console.WriteLine($"You win ({userResult} > {computerResult})!");
            else if (computerResult > userResult)
                Console.WriteLine($"I win ({computerResult} > {userResult})!");
            else
                Console.WriteLine($"It's a draw ({userResult} = {computerResult})!");
        }

        private int PerformRoll(string owner, Die die)
        {
            int faceCount = die.Faces.Length;
            Console.WriteLine($"It's time for {owner} roll.");
            var (key, computerNumber, hmac) = random.GenerateCommitment(faceCount - 1);
            Console.WriteLine($"I selected a random value in the range 0..{faceCount - 1} (HMAC={hmac}).");
            Console.WriteLine($"Add your number modulo {faceCount}.");
            for (int i = 0; i < faceCount; i++)
                Console.WriteLine($"{i} - {i}");
            Console.WriteLine("X - exit");
            Console.WriteLine("? - help");
            Console.Write("Your selection: ");

            string input = Console.ReadLine();
            if (input == "X") Environment.Exit(0);
            if (input == "?")
            {
                ShowHelp();
                return PerformRoll(owner, die);
            }

            int userNumber = int.Parse(input);
            Console.WriteLine($"My number is {computerNumber} (KEY={BitConverter.ToString(key).Replace("-", "")}).");
            int result = (userNumber + computerNumber) % faceCount;
            Console.WriteLine($"The fair number generation result is {userNumber} + {computerNumber} = {result} (mod {faceCount}).");
            Console.WriteLine();
            return result;
        }

        private void ShowHelp()
        {
            helpTable.ShowProbabilityTable(dice);
            Console.WriteLine("\nGame Rules:");
            Console.WriteLine("- First move is determined by guessing 0 or 1");
            Console.WriteLine("- Select different dice than your opponent");
            Console.WriteLine("- Each roll uses fair random number generation");
            Console.WriteLine("- Higher roll wins the round\n");
        }
    }
}