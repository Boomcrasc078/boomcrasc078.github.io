namespace RaceTimer.Classes
{
    public class IdGenerator
    {
        public static string GenerateId(String name)
        {
            String generatedId = "";
            foreach(char letter in name)
            {
                switch (letter)
                {
                    case ' ':
                        generatedId = generatedId + "-"; 
                        break;
                    case '/':
                        generatedId = generatedId + "-";
                        break;
                    default:
                        generatedId = generatedId + letter;
                        break;
                }
            }
            return generatedId;
        }
    }
}
