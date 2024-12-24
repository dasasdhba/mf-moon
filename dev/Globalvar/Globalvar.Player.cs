namespace Global;

public static partial class Globalvar
{
    // player global variables
    
    public enum PlayerState
    {
        Small, 
        Super,
        Fire,
        Beet,
        Lui
    }
    
    public static class Player
    {
        public const PlayerState DefaultState = PlayerState.Small;
        public const int DefaultLife = 4;
        
        // we don't need to save these
        
        public static PlayerState State { get; set; } = DefaultState;
        public static int Life { get ;set; } = DefaultLife;
        public static int Coin { get; set; } = 0;
        public static int Score { get; set; } = 0;

        public static void Reset()
        {
            State = DefaultState;
            Life = DefaultLife;
            Coin = 0;
            Score = 0;
        }
    }
}