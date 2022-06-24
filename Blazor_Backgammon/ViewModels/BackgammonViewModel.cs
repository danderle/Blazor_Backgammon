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
    /// Flag to let us know if player one has reached the base with all chips
    /// </summary>
    public bool PlayerOneReachedBase { get; private set; } = true;

    /// <summary>
    /// Flag to let us know if player two has reached the base with all chips
    /// </summary>
    public bool PlayerTwoReachedBase { get; private set; } = true;

    /// <summary>
    /// Flag to hide the roll dice button
    /// </summary>
    public bool HideRollDiceButton { get; set; } = true;

    /// <summary>
    /// Flag to hide the skip turn button
    /// </summary>
    public bool HideSkipTurnButton { get; set; } = true;

    /// <summary>
    /// Flag to hide the start game button
    /// </summary>
    public bool HideStartGameButton { get; set; }

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

    /// <summary>
    /// The chips which have reached the finish line
    /// </summary>
    public List<Chip> HomeList { get; private set; } = new ();

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

        for (int index = 0; index < 30; index++)
        {
            if (index < 15)
            {
                HomeList.Add(new Chip(-1, Player.Two));
            }
            else
            {
                HomeList.Add(new Chip(-1, Player.One));
            }
        }
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
        HideStartGameButton = true;
        HideRollDiceButton = false;
        HideSkipTurnButton = false;
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

        // We have selected or deselected a chip
        var chips = GetPlayerChipsAtIndex(chip.FieldIndex);
        if (AnyChipSelected(chip.FieldIndex))
        {
            DeselectAllChips();
        }

        RemoveMoveOptions();
        chips.Last().IsSelected ^= true;

        // if we selected a chip then show move options
        if (chips.Last().IsSelected)
        {
            ShowPossibleOptions(chip.FieldIndex);
        }
        else
        {
            DeselectAllChips();

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

    /// <summary>
    /// Switches the active player
    /// </summary>
    public void SwitchPlayer()
    {
        if (ActivePlayer == Player.One)
        {
            ActivePlayer = Player.Two;
        }
        else
        {
            ActivePlayer = Player.One;
        }
        
        // Show roll dice button
        HideRollDiceButton = false;

        //Clear dice incase we skipped a turn
        DiceNumbers.Clear();
    }

    /// <summary>
    /// Moves the chip into home base
    /// </summary>
    /// <param name="chip"></param>
    public void MoveToHome(Chip chip)
    {
        if (chip.IsMoveOption)
        {
            chip.IsMoveOption = false;

            if (!_doubles)
            {
                DiceNumbers.Remove(chip.MoveOption.DiceNumber);
            }
            else
            {
                DiceNumbers.RemoveRange(0, chip.MoveOption.DiceNumber/DiceNumbers.Last());
            }

            RemoveSelectedChipAndChipOptions();

            // If all dicenumbers were used switch player turn
            if (DiceNumbers.Count == 0)
            {
                SwitchPlayer();
            }
        }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Checks if there are any chips selected except for the given fieldIndex
    /// </summary>
    /// <param name="fieldIndex"></param>
    /// <returns></returns>
    private bool AnyChipSelected(int fieldIndex)
    {
        foreach (var list in GameField)
        {
            foreach (var chip in list)
            {
                if (chip.FieldIndex != fieldIndex && chip.IsSelected)
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Removes only chips which are move options
    /// </summary>
    private void RemoveMoveOptions()
    {
        var chipsToRemove = new List<Chip>();
        foreach (List<Chip> chipList in GameField)
        {
            foreach (Chip chip in chipList)
            {
                if (chip.Player != ActivePlayer)
                {
                    continue;
                }

                if (chip.IsMoveOption)
                {
                    chipsToRemove.Add(chip);
                }
            }

            lock (chipList)
            {
                foreach (Chip chip in chipsToRemove)
                {
                    chipList.Remove(chip);
                }
            }
        }

        foreach (var chip in HomeList)
        {
            if (chip.IsMoveOption)
            {
                HomeList.Remove(chip);
                break;
            }
        }
    }

    /// <summary>
    /// Deselects all chips
    /// </summary>
    private void DeselectAllChips()
    {
        foreach (List<Chip> list in GameField)
        {
            foreach (Chip chip in list)
            {
                chip.IsSelected = false;
            }
        }
    }

    /// <summary>
    /// Make the move option to a new position on the board
    /// </summary>
    /// <param name="chip"></param>
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

            // check if both dice were used for move option
            if (chip.MoveOption.DiceNumber == DiceNumbers.Sum())
            {
                chip.MoveOption.IsSet = true;
                chip.IsMoveOption = false;
                DiceNumbers.Clear();
            }
        }

        RemoveSelectedChipAndChipOptions();
        RemoveFromExiledList();
        KickOpponent(chip.FieldIndex);
        CheckIfAllChipsAreInBase();


        // If all dicenumbers were used switch player turn
        if (DiceNumbers.Count == 0)
        {
            SwitchPlayer();
        }
    }

    /// <summary>
    /// Checks if all the chips of a player have reached their base
    /// </summary>
    private void CheckIfAllChipsAreInBase()
    {
        if (ActivePlayer == Player.One)
        {
            if (ExiledChips.Count(chip => chip.Player == Player.One) > 0)
            {
                PlayerOneReachedBase = false;
                return;
            }

            for (int index = 0; index < TotalFieldSpaces-6; index++)
            {
                if(GameField[index].Count(chip => chip.Player == Player.One) > 0)
                {
                    PlayerOneReachedBase = false;
                    return;
                }
            }

            PlayerOneReachedBase = true;
        }
        else
        {
            if (ExiledChips.Count(chip => chip.Player == Player.Two) > 0)
            {
                PlayerTwoReachedBase = false;
                return;
            }

            for (int index = 6; index < TotalFieldSpaces; index++)
            {
                if(GameField[index].Count(chip => chip.Player == Player.Two) > 0)
                {
                    PlayerTwoReachedBase = false;
                    return;
                }

                PlayerTwoReachedBase = true;
            }
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

        foreach (var chip in HomeList)
        {
            if (chip.IsMoveOption)
            {
                HomeList.Remove(chip);
                break;
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
            int moveOptionCounter = 0;
            foreach (var number in DiceNumbers)
            {
                var moveOption = fieldIndex + (moveDirection * number);
                if (moveOption < TotalFieldSpaces && moveOption >= 0)
                {
                    moveOptionCounter += AddMoveOption(moveOption, number)? 1 : 0;
                }
                else
                {
                    CheckIfPlayerCanClearChip(number, moveOption, fieldIndex);
                }
            }

            // Add sum dicenumber move options
            if (DiceNumbers.Count > 1 && moveOptionCounter >= 1)
            {
                var sum = DiceNumbers.Sum();
                var sumOption = fieldIndex + (moveDirection * sum);
                if (sumOption < TotalFieldSpaces && sumOption >= 0)
                {
                    AddMoveOption(sumOption, sum);
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
                if (moveOption < TotalFieldSpaces && moveOption >= 0)
                {
                    if (!AddMoveOption(moveOption, sum))
                    {
                        return;
                    }
                }
                else
                {
                    if (!CheckIfPlayerCanClearChip(sum, moveOption, fieldIndex))
                    {
                        return;
                    }

                }
            }
        }
    }

    /// <summary>
    /// Check if player can clear a chip from the base
    /// </summary>
    /// <param name="diceNumber"></param>
    private bool CheckIfPlayerCanClearChip(int diceNumber, int moveOption, int fieldIndex)
    {
        if (ActivePlayer == Player.One && PlayerOneReachedBase)
        {
            if (moveOption == TotalFieldSpaces)
            {
                AddMoveOptionToHomeList(diceNumber);
            }
            else
            {
                bool chipsBehind = false;
                for (int index = TotalFieldSpaces-6; index < fieldIndex; index++)
                {
                    if(GameField[index].Count(chip => chip.Player == ActivePlayer) > 0)
                    {
                        chipsBehind = true;
                    }
                }

                if (!chipsBehind)
                {
                    if (_doubles)
                    {
                        if (fieldIndex + diceNumber >= TotalFieldSpaces)
                        {
                            AddMoveOptionToHomeList(diceNumber);
                            return false;
                        }
                    }
                    else
                    {
                        AddMoveOptionToHomeList(diceNumber);
                    }
                }
            }
        }

        else if (ActivePlayer == Player.Two && PlayerTwoReachedBase)
        {
            if (moveOption == -1)
            {
                AddMoveOptionToHomeList(diceNumber);
            }
            else
            {
                bool chipsBehind = false;
                for (int index = fieldIndex+1; index < 6; index++)
                {
                    if(GameField[index].Count(chip => chip.Player == ActivePlayer) > 0)
                    {
                        chipsBehind = true;
                    }
                }

                if (!chipsBehind)
                {
                    if (_doubles)
                    {
                        if (fieldIndex - diceNumber < 0)
                        {
                            AddMoveOptionToHomeList(diceNumber);
                            return false;
                        }
                    }
                    else
                    {
                        AddMoveOptionToHomeList(diceNumber);
                    }
                }
            }
        }

        return true;
    }

    /// <summary>
    /// Adds move options to the home list, but only 1 at a time
    /// </summary>
    /// <param name="diceNumber"></param>
    /// <returns></returns>
    private bool AddMoveOptionToHomeList(int diceNumber)
    {
        if (HomeList.Any(chip => chip.IsMoveOption))
        {
            return false;
        }

        HomeList.Add(new Chip(0, ActivePlayer, new MoveOption(diceNumber)));
        return true;
    }

    /// <summary>
    /// Adds move options to the gamefield with the possible fieldIndex and corresponding dicenumber
    /// </summary>
    /// <param name="moveOption">The possible field position</param>
    /// <param name="dicenumber">either die dicenumber or sum of 2 dice or multiples of doubles</param>
    /// <returns></returns>
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

        //GameField.Add(new List<Chip>());
        //GameField.Add(new List<Chip>());
        //GameField.Add(new List<Chip>());
        //GameField.Add(new List<Chip>());
        //GameField.Add(new List<Chip>());
        //GameField.Add(CreateChips(5, 5, Player.Two));
        //GameField.Add(new List<Chip>());
        //GameField.Add(new List<Chip>());
        //GameField.Add(new List<Chip>());
        //GameField.Add(new List<Chip>());
        //GameField.Add(new List<Chip>());
        //GameField.Add(new List<Chip>());
        //GameField.Add(new List<Chip>());
        //GameField.Add(new List<Chip>());
        //GameField.Add(new List<Chip>());
        //GameField.Add(new List<Chip>());
        //GameField.Add(new List<Chip>());
        //GameField.Add(new List<Chip>());
        //GameField.Add(CreateChips(18, 5, Player.One));
        //GameField.Add(new List<Chip>());
        //GameField.Add(new List<Chip>());
        //GameField.Add(new List<Chip>());
        //GameField.Add(new List<Chip>());
        //GameField.Add(new List<Chip>());
        HomeList.Clear();
        PlayerOneReachedBase = false;
        PlayerTwoReachedBase = false;
    }

    #endregion
}
