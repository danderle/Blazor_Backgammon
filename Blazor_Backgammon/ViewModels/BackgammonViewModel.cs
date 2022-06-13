using Blazor_Backgammon.DataModels;
using Blazor_Backgammon.Models;

namespace Blazor_Backgammon.ViewModels;

/// <summary>
/// The view model for the <see cref="Backgammon.razor"/> page
/// </summary>
public class BackgammonViewModel
{
    #region Fields

    /// <summary>
    /// Flag to let us know if we rolled doubles
    /// </summary>
    private bool _doubles;

    #endregion

    #region Public Properties

    /// <summary>
    /// Flag to hide the roll dice button
    /// </summary>
    public bool HideRollDiceButton { get; set; }

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
    /// The list with the exiled chips
    /// </summary>
    public List<Chip> ExiledChips { get; set; } = new();

    /// <summary>
    /// The game field with all the chips
    /// </summary>
    public List<List<Chip>> GameField { get; set; } = new();

    /// <summary>
    /// The dice
    /// </summary>
    public List<Dice> Dice { get; set; } = new ();

    /// <summary>
    /// The dice numbers
    /// </summary>
    public List<int> DiceNumbers { get; set; } = new ();

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
    public void MoveChip(Chip chip)
    {
        if (chip.Player != ActivePlayer)
        {
            return;
        }

        // If clicked chip is a move option
        if (chip.IsMoveOption)
        {
            HandleMoveOptionClick(chip);

            return;
        }
        
        // Check if there are any exiled chips. They need to return before other moves
        if (ExiledChips.Count(item => item.Player == ActivePlayer) > 0)
        {
            return;
        }

        // We habe selected or deselected a chip
        var chips = GetPlayerChipsAtIndex(chip.FieldIndex);
        chips.Last().IsSelected ^= true;

        // if we selected a chip then show move options
        if (chips.Last().IsSelected)
        {
            ShowPossibleOptions(chip.FieldIndex);
        }
        else
        {
            RemoveSelectedChipAndChipOptions();
        }
    }

    /// <summary>
    /// Return an exiled chip to the game board
    /// </summary>
    /// <param name="chip"></param>
    public void ReturnExiledChip(Chip chip)
    {
        if (ActivePlayer != chip.Player)
        {
            return;
        }

        Chip targetChip;
        int startIndex;
        if (ActivePlayer == Player.One)
        {
            startIndex = -1;
            targetChip = ExiledChips.First(item => item.Player == Player.One);
            targetChip.IsSelected ^= true;
        }
        else
        {
            startIndex = 24;
            targetChip = ExiledChips.First(item => item.Player == Player.Two);
            targetChip.IsSelected ^= true;
        }

        if (targetChip.IsSelected)
        {
            ShowPossibleOptions(startIndex);
        }
        else
        {
            RemoveSelectedChipAndChipOptions();
        }
    }

    /// <summary>
    /// Rolls each dice
    /// </summary>
    public void RollDice()
    {
        _doubles = false;
        foreach (var dice in Dice)
        {
            dice.Roll();
            DiceNumbers.Add(dice.Number);
        }

        if (Dice[0].Number == Dice[1].Number)
        {
            DiceNumbers.Add(Dice.Last().Number);
            DiceNumbers.Add(Dice.Last().Number);
            _doubles = true;
        }

        HideRollDiceButton = true;
    }

    #endregion

    #region Private Methods

    private void HandleMoveOptionClick(Chip chip)
    {
        //if we rolled doubles
            if (_doubles)
            {
                int sum = 0;
                foreach (int number in DiceNumbers)
                {
                    //sum the dice numbers to verify how many dicenumbers where used
                    sum += number;
                    if (chip.MoveOption.DiceNumber == sum)
                    {
                        chip.MoveOption.IsSet = true;
                        chip.IsMoveOption = false;
                        DiceNumbers.RemoveRange(0, sum / DiceNumbers.First());
                        break;
                    }
                }
            }
            else
            {
                // just check which dice number was used
                foreach (int number in DiceNumbers)
                {
                    if (chip.MoveOption.DiceNumber == number)
                    {
                        chip.MoveOption.IsSet = true;
                        chip.IsMoveOption = false;
                        DiceNumbers.Remove(number);
                        break;
                    }
                }
            }

            // check if both dice were used for move option
            if (chip.MoveOption.DiceNumber == DiceNumbers.Sum())
            {
                chip.MoveOption.IsSet = true;
                chip.IsMoveOption = false;
                DiceNumbers.Clear();
            }

            RemoveSelectedChipAndChipOptions();
            RemoveFromExiledList();
            KickOpponent(chip.FieldIndex);

            // If all dicenumbers were used switch player turn
            if (DiceNumbers.Count == 0)
            {
                HideRollDiceButton = false;
                SwitchPlayer();
            }
    }

    /// <summary>
    /// If landing on the same field as opponenet we exile opponents chip
    /// </summary>
    /// <param name="fieldIndex"></param>
    private void KickOpponent(int fieldIndex)
    {
        var chipList = GameField[fieldIndex];
        var removed = chipList.RemoveAll(chip => chip.Player != ActivePlayer);
        if (removed != 0)
        {
            ExiledChips.Add(new Chip(-1, ActivePlayer == Player.One ? Player.Two : Player.One));
        }
    }

    /// <summary>
    /// Remove a selected exiled chip from list
    /// </summary>
    private void RemoveFromExiledList()
    {
        var selected = ExiledChips.Find(item => item.IsSelected);
        if (selected != null)
        {
            ExiledChips.Remove(selected);
        }
    }

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
    /// Switches the active player
    /// </summary>
    private void SwitchPlayer()
    {
        if (ActivePlayer == Player.One)
        {
            ActivePlayer = Player.Two;
        }
        else
        {
            ActivePlayer = Player.One;
        }
    }

    /// <summary>
    /// After selecting a move option the selection will be removed and
    /// the other move options will also be removed
    /// </summary>
    private void RemoveSelectedChipAndChipOptions()
    {
        var chipsToRemove = new List<Chip>();
        foreach (var chipList in GameField)
        {
            foreach (var chip in chipList)
            {
                if (chip.Player != ActivePlayer)
                {
                    continue;
                }

                if (chip.IsSelected || chip.IsMoveOption)
                {
                    chipsToRemove.Add(chip);
                }
            }

            lock (chipList)
            {
                foreach (var chip in chipsToRemove)
                {
                    chipList.Remove(chip);
                }
            }
        }
    }

    /// <summary>
    /// Adds chips to the game field which are marked as move options for the selected chip and
    /// current rolled dice numbers
    /// </summary>
    /// <param name="fieldIndex"></param>
    private void ShowPossibleOptions(int fieldIndex)
    {
        int moveDirection = ActivePlayer == Player.One ? 1 : -1;

        // If no doubles and we have more than 1 dice number left to use or exactly 1 dice left
        if ((DiceNumbers.Count > 1 && !_doubles) || DiceNumbers.Count == 1)
        {
            foreach (var number in DiceNumbers)
            {
                var moveOption = fieldIndex + (moveDirection * number);
                if (moveOption < TotalFieldSpaces && moveOption >= 0)
                {
                    AddMoveOption(moveOption, number);
                }
            }

            // Add sum dicenumber move options
            if (DiceNumbers.Count > 1)
            {
                var sum = DiceNumbers.Sum();
                var sumOption = fieldIndex + (moveDirection * sum);
                if (sumOption < TotalFieldSpaces)
                {
                    var chipsOtherPlayer = GameField[sumOption].Count(item => item.Player != ActivePlayer);
                    if (chipsOtherPlayer < 2)
                    {
                        GameField[sumOption].Add(new Chip(sumOption, ActivePlayer, new MoveOption(sum)));
                    }
                }
                else if (sumOption >= 0)
                {
                    var chipsOtherPlayer = GameField[sumOption].Count(item => item.Player != ActivePlayer);
                    if (chipsOtherPlayer < 2)
                    {
                        GameField[sumOption].Add(new Chip(sumOption, ActivePlayer, new MoveOption(sum)));
                    }
                }
            }
        }
        else
        {
            int sum = 0;
            foreach (var number in DiceNumbers)
            {
                sum += number;
                var moveOption = fieldIndex + (moveDirection * sum);
                if (moveOption < TotalFieldSpaces)
                {
                    var chipsOtherPlayer = GameField[moveOption].Count(item => item.Player != ActivePlayer);
                    if (chipsOtherPlayer < 2)
                    {
                        GameField[moveOption].Add(new Chip(moveOption, ActivePlayer, new MoveOption(sum)));
                    }
                    else
                    {
                        return;
                    }
                }
                else if (moveOption >= 0)
                {
                    var chipsOtherPlayer = GameField[moveOption].Count(item => item.Player != ActivePlayer);
                    if (chipsOtherPlayer < 2)
                    {
                        GameField[moveOption].Add(new Chip(moveOption, ActivePlayer, new MoveOption(sum)));
                    }
                    else
                    {
                        return;
                    }
                }
            }
        }
    }

    private bool AddMoveOption(int moveOption, int dicenumber)
    {
        bool added = false;
        var chipsOtherPlayer = GameField[moveOption].Count(item => item.Player != ActivePlayer);
        if (chipsOtherPlayer < 2)
        {
            GameField[moveOption].Add(new Chip(moveOption, ActivePlayer, new MoveOption(dicenumber)));
            added = true;
        }

        return added;
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
