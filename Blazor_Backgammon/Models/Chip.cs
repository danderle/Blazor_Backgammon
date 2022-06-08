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
        /// The gameboard index position
        /// </summary>
        public int FieldIndex { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="player"></param>
        public Chip(int fieldIndex, Player player)
        {
            Player = player;
            FieldIndex = fieldIndex;
        } 
        
        #endregion

    }
}
