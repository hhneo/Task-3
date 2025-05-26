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
        public (byte[] key, int number) GenerateRandomNumber(int maxValue)
        {
            byte[] key = RandomNumberGenerator.GetBytes(Constants.KeySizeInBytes);
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
}
