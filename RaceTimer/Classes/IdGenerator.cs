namespace RaceTimer.Classes
{
    using System.Security.Cryptography;


    public class IdGenerator
    {
        public static string GenerateBase64String(int length)
        {
            // Skapa en byte-array av den angivna längden
            byte[] randomBytes = new byte[length];

            // Fyll byte-arrayen med slumpmässiga bytes
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }

            // Konvertera bytes till en Base64-sträng
            return Convert.ToBase64String(randomBytes);
        }
    }
}