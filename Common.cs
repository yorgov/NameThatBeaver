using System;
using System.IO;
using Timberborn.PlatformUtilities;
using UnityEngine;

namespace NameThatBeaver
{
    internal static class Common
    {
        public static string GetPersistentDataPath()
        {
            return Path.Combine(
                Application.persistentDataPath.Replace('/', '\\'),
                Constants.MOD_NAME);
        }

        public static string GetModFolderPath()
        {
            return Path.Combine(
                UserDataFolder.Folder,
                "Mods",
                Constants.MOD_NAME);
        }

        public static readonly string[] RomanNumerals = new[]
        {
            "M", "CM", "D", "CD", "C", "XC", "L", "XL", "X", "IX", "V", "IV", "I"
        };

        public static string ToRoman(int number)
        {
            if ((number < 0) || (number > 3999)) throw new ArgumentOutOfRangeException("insert value betwheen 1 and 3999");
            if (number < 1) return string.Empty;
            if (number >= 1000) return RomanNumerals[0] + ToRoman(number - 1000);
            if (number >= 900) return RomanNumerals[1] + ToRoman(number - 900);
            if (number >= 500) return RomanNumerals[2] + ToRoman(number - 500);
            if (number >= 400) return RomanNumerals[3] + ToRoman(number - 400);
            if (number >= 100) return RomanNumerals[4] + ToRoman(number - 100);
            if (number >= 90) return RomanNumerals[5] + ToRoman(number - 90);
            if (number >= 50) return RomanNumerals[6] + ToRoman(number - 50);
            if (number >= 40) return RomanNumerals[7] + ToRoman(number - 40);
            if (number >= 10) return RomanNumerals[8] + ToRoman(number - 10);
            if (number >= 9) return RomanNumerals[9] + ToRoman(number - 9);
            if (number >= 5) return RomanNumerals[10] + ToRoman(number - 5);
            if (number >= 4) return RomanNumerals[11] + ToRoman(number - 4);
            if (number >= 1) return RomanNumerals[12] + ToRoman(number - 1);
            throw new ArgumentOutOfRangeException("something bad happened");
        }
    }
}