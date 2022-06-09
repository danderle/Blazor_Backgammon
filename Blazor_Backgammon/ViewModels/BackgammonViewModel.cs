using Blazor_Backgammon.DataModels;
using Blazor_Backgammon.Models;

namespace Blazor_Backgammon.ViewModels;

/// <summary>
/// The view model for the <see cref="Backgammon.razor"/> page
/// </summary>
public class BackgammonViewModel
{
    #region Fields


    #endregion

    #region Public Properties

    /// <summary>
    /// The total number of spaces a player can move
    /// </summary>
    public int TotalFieldSpaces => 24;

    /// <summary>
    /// The number of spaces of either the top or bottom of the game board
    /// </summary>
    public int HalfFieldSpaces => TotalFieldSpaces / 2;

    /// <summary>
    /// The current active player
    /// </summary>
    public Player ActivePlayer { get; set; }

    /// <summary>
    /// The game field with all the chips
    /// </summary>
    public List<List<Chip>> GameField { get; set; } = new();

    /// <summary>
    /// 
    /// </summary>
    public List<Dice> Dice { get; set; } = new ();

    #endregion

    #region Constructor

    /// <summary>
    /// Constructor
    /// </summary>
    public BackgammonViewModel()
    {
        var dice1 = new Dice();
        var dice2 = new Dice();
        Dice.Add(dice1);
        Dice.Add(dice2);
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Gets a players chips at a field index
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public List<Chip> GetPlayerChipsAtIndex(int index)
    {
        if (GameField.Count > 0)
        {
            return GameField[index];
        }
        return new List<Chip>();
    }

    #endregion

    #region Command Methods

    /// <summary>
    /// The start / reset of the game
    /// </summary>
    public void StartGame()
    {
        SetupGame();
    }

    /// <summary>
    /// Select a chip to move positions
    /// </summary>
    /// <param name="chip"></param>
    public void ClickedChip(Chip chip)
    {
        if (chip.Player != ActivePlayer)
        {
            return;
        }
        
        var chips = GetPlayerChipsAtIndex(chip.FieldIndex);
        chips.Last().IsSelected ^= true;
    }

    /// <summary>
    /// Rolls each dice
    /// </summary>
    public void RollDice()
    {
        foreach (var dice in Dice)
        {
            dice.Roll();
        }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Creates a list of player chips
    /// </summary>
    /// <param name="fieldIndex">The field index position on the board</param>
    /// <param name="numberToCreate">The number of chips to create</param>
    /// <param name="player">The player</param>
    /// <returns>The list of chips</returns>
    private List<Chip> CreateChips(int fieldIndex, int numberToCreate, Player player)
    {
        var chips = new List<Chip>();
        for (int i = 0; i < numberToCreate; i++)
        {
            chips.Add(new Chip(fieldIndex, player));
        }
        return chips;
    }

    /// <summary>
    /// The Setup for the game start position
    /// </summary>
    private void SetupGame()
    {
        ActivePlayer = Player.One;
        GameField.Clear();
        GameField.Add(CreateChips(0, 2, Player.One));
        GameField.Add(new List<Chip>());
        GameField.Add(new List<Chip>());
        GameField.Add(new List<Chip>());
        GameField.Add(new List<Chip>());
        GameField.Add(CreateChips(5, 5, Player.Two));
        GameField.Add(new List<Chip>());
        GameField.Add(CreateChips(7, 3, Player.Two));
        GameField.Add(new List<Chip>());
        GameField.Add(new List<Chip>());
        GameField.Add(new List<Chip>());
        GameField.Add(CreateChips(11, 5, Player.One));
        GameField.Add(CreateChips(12, 5, Player.Two));
        GameField.Add(new List<Chip>());
        GameField.Add(new List<Chip>());
        GameField.Add(new List<Chip>());
        GameField.Add(CreateChips(16, 3, Player.One));
        GameField.Add(new List<Chip>());
        GameField.Add(CreateChips(18, 5, Player.One));
        GameField.Add(new List<Chip>());
        GameField.Add(new List<Chip>());
        GameField.Add(new List<Chip>());
        GameField.Add(new List<Chip>());
        GameField.Add(CreateChips(23, 2, Player.Two));
    }

    #endregion
}
