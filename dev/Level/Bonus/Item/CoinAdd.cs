using Global;
using Godot;

namespace Level;

public partial class CoinAdd : Node
{
    [ExportCategory("CoinAdd")]
    [Export]
    public int Value { get; set; } = 1;

    public CoinAdd() : base()
    {
        TreeEntered += Add;
    }
    
    public void Add() => Globalvar.Player.Coin += Value;
}