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

        public static string GenerateUniqueId(IEnumerable<string> existingIds)
        {
            int idLength = 5;
            string id = GenerateBase64String(idLength);

            while (existingIds.Contains(id) || id.Contains("/"))
            {
                id = GenerateBase64String(idLength);
            }

            return id;
        }
    }
}