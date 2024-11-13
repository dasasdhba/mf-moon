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
        public static PlayerState State
        {
            get => (PlayerState)Moon.Save.GetItemValue("Player", "State", 0);
            set => Moon.Save.SetItemValue("Player", "State", (int)value);
        }
    }
}