namespace BoardGames.Framework
{
    interface IPlayer
    {
        IAction ProvideAction(IGame aGame);
    }
}
