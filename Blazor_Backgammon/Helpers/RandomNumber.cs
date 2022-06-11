namespace Blazor_Backgammon.Helpers
{
    static class RandomNumber
    {
        public static Random random = new Random();

        public static int GenerateDiceRoll()
        {
            return random.Next(1, 7);
        }
    }
}
