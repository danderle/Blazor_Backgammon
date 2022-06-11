using Blazor_Backgammon.Helpers;

namespace Blazor_Backgammon.Models
{
    /// <summary>
    /// The Dice class
    /// </summary>
    public class Dice
    {
        #region Public Properties

        public bool IsSet { get; set; }


        public int Number { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public Dice()
        {
            Number = RandomNumber.GenerateDiceRoll();
        }

        #endregion

        #region Public Methods

        public void Roll()
        {
            Number = RandomNumber.GenerateDiceRoll();
        }

        #endregion
    }
}
