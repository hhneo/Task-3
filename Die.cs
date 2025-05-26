using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NonTransitiveDice
{
    public class Die
    {
        public int[] Faces { get; }

        public Die(string faceString)
        {
            try
            {
                Faces = faceString.Split(',')
                    .Select(s => {
                        if (string.IsNullOrWhiteSpace(s))
                            throw new ArgumentException("Empty face value detected");
                        return int.Parse(s.Trim());
                    })
                    .ToArray();

                if (Faces.Length < Constants.MinDiceFaces)
                    throw new ArgumentException(
                        $"Die must have at least {Constants.MinDiceFaces} faces");
            }
            catch (FormatException)
            {
                throw new ArgumentException(
                    $"Invalid die format: '{faceString}'. All values must be integers.\n" +
                    "Example valid format: 1,2,3,4,5,6");
            }
        }

        public int GetFace(int index) => Faces[index];
        public override string ToString() => $"[{string.Join(",", Faces)}]";
        public string ToStringWithoutBrackets() => string.Join(",", Faces);
    }
}
