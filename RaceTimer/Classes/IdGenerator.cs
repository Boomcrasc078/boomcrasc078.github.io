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
                        generatedId.Append<char>('-');
                        break;
                    case '/':
                        generatedId.Append<char>('-');
                        break;
                    default:
                        generatedId.Append<char>(letter);
                        break;
                }
                
            }
            return generatedId;
        }
    }
}
