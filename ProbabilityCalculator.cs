using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NonTransitiveDice
{
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
}
