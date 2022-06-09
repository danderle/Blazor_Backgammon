namespace Blazor_Backgammon.Helpers
{
    static class RandomNumber
    {
        public static Random random = new Random();

        public static int RollDice()
        {
            return random.Next(1, 7);
        }
    }
}
