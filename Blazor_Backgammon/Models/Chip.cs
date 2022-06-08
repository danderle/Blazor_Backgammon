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

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="player"></param>
        public Chip(Player player)
        {
            Player = player;
        } 
        
        #endregion

    }
}
