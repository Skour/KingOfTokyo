using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoardGames.Framework
{
    class CGameController
    {
        #region Fields

        IGame _game = null;

        #endregion //Fields

        #region Constructors

        public CGameController(IGame aGame)
        {
            _game = aGame;
        }

        #endregion // Constructors

        #region Members

        public void PlayCompleteGame()
        {
            do
            {
                _game.Tick();
            } while (!_game.IsGameCompleted());
        }

        #endregion
    }
}
