using System.Text;

namespace RaceTimer.Classes
{
    public class RaceUrlGenerator
    {
        public static string GenerateUrl(string name)
        {
            var generatedId = new StringBuilder();

            foreach (char letter in name)
            {
                switch (letter)
                {
                    case ' ':
                    case '/':
                        generatedId.Append('-');
                        break;
                    default:
                        generatedId.Append(char.ToLower(letter)); // Gör bokstaven lowercase
                        break;
                }
            }

            return generatedId.ToString();
        }
    }
}
