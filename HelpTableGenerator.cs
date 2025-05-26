using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NonTransitiveDice
{
    public class HelpTableGenerator
    {
        public void ShowProbabilityTable(List<Die> dice)
        {
            var calculator = new ProbabilityCalculator();
            var probabilities = calculator.CalculateProbabilities(dice);

            Console.WriteLine("\nProbability of the win for the user:");

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
                    Console.Write(" " + (userDie == compDie ?
                        $"- ({Constants.SelfComparisonProbability:F4})".PadRight(otherColWidth - 1) :
                        probabilities[(userDie, compDie)].ToString("0.0000").PadRight(otherColWidth - 1)) + " |");
                }
                Console.WriteLine();
            }
        }
    }
}
