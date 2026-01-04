using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrlShortenerWpf.Services
{
    public static class ShortKeyGenerator
    {
        private const string Characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private static readonly Random _random = new();
        public static string Generate(int length = 6)
        {
            var sb = new StringBuilder(length);
            for(int i = 0; i<length; i++)
            {
                int index = _random.Next(Characters.Length);
                sb.Append(Characters[index]);
            }
            return sb.ToString();
        }
    }
}
