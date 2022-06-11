using Blazor_Backgammon.DataModels;

namespace Blazor_Backgammon.Models
{
    /// <summary>
    /// The Chip class
    /// </summary>
    public class MoveOption
    {
        #region Public Properties

        public int DiceNumber { get; set; }

        public bool IsSet { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="player"></param>
        public MoveOption(int diceNumber)
        {
            DiceNumber = diceNumber;
        } 
        
        #endregion

    }
}
