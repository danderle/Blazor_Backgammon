using Blazor_Backgammon.DataModels;

namespace Blazor_Backgammon.Models
{
    /// <summary>
    /// The Chip class
    /// </summary>
    public class Chip
    {
        #region Public Properties

        /// <summary>
        /// Which player
        /// </summary>
        public Player Player { get; private set; }

        /// <summary>
        /// Flag for selecting a chip
        /// </summary>
        public bool IsSelected { get; set; }

        /// <summary>
        /// Flag to let us know if this is a move option
        /// </summary>
        public bool IsMoveOption { get; set; }

        /// <summary>
        /// The gameboard index position
        /// </summary>
        public int FieldIndex { get; set; }

        /// <summary>
        /// The move option object
        /// </summary>
        public MoveOption MoveOption { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="player"></param>
        public Chip(int fieldIndex, Player player, MoveOption moveOption = null)
        {
            Player = player;
            FieldIndex = fieldIndex;
            IsMoveOption = (moveOption != null);
            MoveOption = moveOption;
        } 
        
        #endregion

    }
}
