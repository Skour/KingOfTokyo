using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoardGames.Framework;

namespace BoardGames
{
    namespace KingOfTokyo
    {
        class Program
        {
            static void Main(string[] args)
            {
                StreamWriter fileLogger = new StreamWriter("Game.log");

                for (int i = 0; i < 100; ++i)
                {
                    CGame kotGame = new CGame(4);

                    CGameController gameController = new CGameController(kotGame);
                    gameController.PlayCompleteGame();

                    fileLogger.WriteLine(kotGame.GetFinalGameStateDescription());
                }

                fileLogger.Close();
            }
        }
    }
}