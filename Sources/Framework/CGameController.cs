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
                do
                {
                    _game.Tick();
                } while (!_game.IsRequiringPlayerAction());

                do
                {
                    _game.PlayAction(_game.GetCurrentPlayer().ProvideAction(_game));
                } while (_game.IsRequiringPlayerAction());

            } while (!_game.IsGameCompleted());
        }

        #endregion
    }
}
