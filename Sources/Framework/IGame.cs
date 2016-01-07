using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoardGames.Framework
{
    interface IGame
    {
        void Tick();
        bool IsGameCompleted();
    }
}
