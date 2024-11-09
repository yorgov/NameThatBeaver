using System.Text.RegularExpressions;

namespace NameThatBeaver
{
    public class BeaverName
    {
        public string Name { get; set; } = string.Empty;
        public string? Color { get; set; }
        public long Id { get; private set; }

        public BeaverName(string name, string? color)
        {
            Name = name;
            Color = color;
            Id = GetHashCode();
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + Name.GetHashCode();
                if (!string.IsNullOrWhiteSpace(Color))
                {
                    hash = hash * 23 + Color.GetHashCode();
                }
                return hash;
            }
        }

        public override string ToString()
        {
            return string.IsNullOrWhiteSpace(Color) ?
                Name :
                $"<color=#{Color}>{Name}";
        }

        public static BeaverName CreateFromString(string str)
        {
            var match = Regex.Match(str, @"^([A-Fa-f0-9]{6})\#(.+)$");
            string? color = null;
            string name = str;
            if (match.Success)
            {
                color = match.Groups[1].Value;
                name = match.Groups[2].Value;
            }

            return new BeaverName(name, color);
        }

        public static BeaverName CreateFromCharacterName(string characterName)
        {
            var match = Regex.Match(characterName, @"^(<color=\#([a-fA-F0-9]{6})>)(.+)$");
            string? color = null;
            string name = characterName;
            if (match.Success)
            {
                color = match.Groups[2].Value;
                name = match.Groups[3].Value;
            }
            return new BeaverName(name, color);
        }
    }
}