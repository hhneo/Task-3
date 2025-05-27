using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NonTransitiveDice
{
    public class FairRandomGenerator
    {
        public (byte[] key, int number, string hmac) GenerateCommitment(int maxValue)
        {
            byte[] key = RandomNumberGenerator.GetBytes(Constants.KeySizeInBytes);
            int number = RandomNumberGenerator.GetInt32(0, maxValue + 1);
            string hmac = CalculateHmac(key, number);
            return (key, number, hmac);
        }

        public string CalculateHmac(byte[] key, int number)
        {
            using var hmac = new HMACSHA256(key);
            byte[] hash = hmac.ComputeHash(BitConverter.GetBytes(number));
            return BitConverter.ToString(hash).Replace("-", "");
        }

        public int CalculateFairResult(int userNumber, int computerNumber, int modulus)
        {
            return (userNumber + computerNumber) % modulus;
        }
    }
}


