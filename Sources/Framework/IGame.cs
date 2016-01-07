namespace BoardGames.Framework
{
    interface IGame
    {
        void Tick();
        void PlayAction(IAction aAction);

        bool IsGameCompleted();
        bool IsRequiringPlayerAction();

        IPlayer GetCurrentPlayer();
    }
}
