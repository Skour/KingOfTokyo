using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;

namespace BoardGames.KingOfTokyo
{
    #region Enums

    enum eTurnState
    {
        eTS_StartingTurn,
        eTS_InitialRollingDice,
        eTS_RollingDice,
        eTS_ReactToDiceAction,
        eTS_ApplyDamageOnTokyoAction,
        eTS_PurchaseCardsAction,
        eTS_EndingTurn,
    }

    enum ePlayerActionType
    {
        ePAT_ReactToDice,
        ePAT_ReactToDamageOnTokyo,
        ePAT_PurchaseCard,
        //ePAT_UseCard,
    }

    enum eLocations
    {
        [Description("Tokyo City")]
        eL_TokyoCity,
        //[Description("Tokyo Bay")]
        //eL_TokyoBay,
        [Description("Outside")]
        eL_Outside,
    }

    #endregion

    class CGame : Framework.IGame
    {
        #region Static Fields

        static public Random Rng = new Random();
        static public uint GlobalGameID = 0;

        #endregion

        #region Const Fields

        public const int DefaultDiceCount = 6;
        public const int VPRewardEnterTokyo = 1;
        public const int VPRewardBeginTurnInTokyo = 2;

        #endregion

        #region Fields

        private List<CPlayer> _playerList = new List<CPlayer>();
        private List<CDice> _diceList = new List<CDice>();
        private uint _gameID = GlobalGameID++;
        private uint _turnID = 0;
        private uint _totalTurnID = 0;
        private uint _startingPlayerIndex = 0;
        private uint _currentPlayerIndex = 0;
        private eTurnState _turnState = eTurnState.eTS_StartingTurn;
        private uint _nbDiceRollsLeft = 3;

        StringBuilder _log = new StringBuilder();

        #endregion

        #region Properties

        public List<CPlayer> Players
        {
            get
            {
                return _playerList;
            }
        }

        public CPlayer CurrentPlayer
        {
            get
            {
                return _playerList[(int)_currentPlayerIndex];
            }
        }

        public List<CDice> Dice
        {
            get
            {
                return _diceList;
            }
        }

        public String LogFilename
        {
            get
            {
                return String.Format("Log\\Output{0}.txt", _gameID);
            }
        }

        #endregion

        #region Constructors

        public CGame(uint nbPlayers)
        {
            Random rng = new Random();
            _startingPlayerIndex = _currentPlayerIndex = (uint)(rng.Next(0, (int)(nbPlayers)));

            for (uint i = 0; i < nbPlayers; ++i)
            {
                _playerList.Add(new CPlayer());
            }

            for (uint i = 0; i < DefaultDiceCount; ++i)
            {
                _diceList.Add(new CDice());
            }
        }

        ~CGame()
        {
            StreamWriter fileLogger = new StreamWriter(LogFilename);
            fileLogger.Write(_log.ToString());
            fileLogger.Close();
        }

        #endregion
            
        #region Members

        public String GetFinalGameStateDescription()
        {
            return String.Format("Game ID: {0}, Total Turn ID: {1}, Turn ID: {2}, Win by Player {3} ({4})",
                                    _gameID,
                                    _totalTurnID,
                                    _turnID,
                                    GetWinningPlayerId(),
                                    GetAlivePlayerCount() == 1 ? "Elimination" : "VP");
        }

        #endregion

        #region Private Members

        public uint GetAlivePlayerCount()
        {
            uint nbPlayersAlive = 0;
            foreach (CPlayer player in Players)
            {
                if (player.IsAlive())
                {
                    nbPlayersAlive++;
                }
            }

            return nbPlayersAlive;
        }

        public int GetWinningPlayerId()
        {
            uint nbPlayersAlive = 0;
            int lastAlivePlayerId = -1;
            int playerId = 0;
            foreach (CPlayer player in Players)
            {
                if (player.HasAchievedVictoryPointsWin())
                {
                    return playerId;
                }
                else if (player.IsAlive())
                {
                    lastAlivePlayerId = playerId;
                    nbPlayersAlive++;
                }
                playerId++;
            }

            return nbPlayersAlive == 1 ? lastAlivePlayerId : -1;
        }

        private void CalculateAndApplyDiceResult()
        {
            int nbOnes = 0, nbTwos = 0, nbThrees = 0, nbEnergy = 0, nbHearts = 0, nbPunches = 0;
            foreach (CDice die in _diceList)
            {
                switch(die.Result)
                {
                    case eDiceResult.eDR_One:
                        nbOnes++;
                        break;
                    case eDiceResult.eDR_Two:
                        nbTwos++;
                        break;
                    case eDiceResult.eDR_Three:
                        nbThrees++;
                        break;
                    case eDiceResult.eDR_Energy:
                        nbEnergy++;
                        break;
                    case eDiceResult.eDR_Punch:
                        nbPunches++;
                        break;
                    case eDiceResult.eDR_Heart:
                        nbHearts++;
                        break;
                }
            }

            CurrentPlayer.AdjustVictoryPoints((nbOnes > 3 ? nbOnes - 2 : 0) +
                                              (nbTwos > 3 ? nbTwos - 1 : 0) +
                                              (nbThrees > 3 ? nbThrees : 0));

            CurrentPlayer.AdjustEnergyPoints(nbEnergy);

            if(CurrentPlayer.Location == eLocations.eL_Outside)
            {
                CurrentPlayer.AdjustLifePoints(nbHearts);

                if (nbPunches > 0)
                { 
                    foreach (CPlayer player in Players)
                    {
                        if(player.Location != eLocations.eL_Outside)
                        {
                            player.AdjustLifePoints(-nbPunches);
                            
                            player.Location = eLocations.eL_Outside;
                            
                            // STODO: Implement eTurnState.eTS_ApplyDamageOnTokyoAction
                        }
                    }

                    CurrentPlayer.Location = eLocations.eL_TokyoCity;
                    CurrentPlayer.AdjustVictoryPoints(VPRewardEnterTokyo);
                }                    
            }
            else
            {
                foreach(CPlayer player in Players)
                {
                    if(player.Location == eLocations.eL_Outside)
                    {
                        player.AdjustLifePoints(-nbPunches);
                    }
                }
            }
        }

        private void LogGameState()
        {
            _log.AppendLine(String.Format("Total Turn ID: {0}, Turn ID: {1}, Current Player: {2}, Players: {3}/{4}",
                                                    _totalTurnID,
                                                    _turnID,
                                                    _currentPlayerIndex,
                                                    _playerList.Count(p => p.IsAlive()),
                                                    _playerList.Count));

            uint playerId = 0;
            foreach (CPlayer player in Players)
            {
                _log.AppendLine(String.Format("Player {0}; {1}",
                                                    playerId,
                                                    player.ToString()));

                if (playerId == _currentPlayerIndex)
                {
                    _diceList.Sort();

                    StringBuilder strDice = new StringBuilder();
                    foreach(CDice die in _diceList)
                    {
                        strDice.Append(die.Result.GetDescription());
                        strDice.Append(" ");
                    }

                    _log.AppendLine(String.Format("\t {0}",
                                                        strDice.ToString()));
                }

                playerId++;
            }

            _log.AppendLine("");
        }

        #endregion

        #region IGame

        public void Tick()
        {
            switch(_turnState)
            {
                case eTurnState.eTS_StartingTurn:
                    {
                        if (CurrentPlayer.Location != eLocations.eL_Outside)
                        {
                            CurrentPlayer.AdjustVictoryPoints(VPRewardBeginTurnInTokyo);
                        }
                    }
                    goto case eTurnState.eTS_InitialRollingDice;

                case eTurnState.eTS_InitialRollingDice:
                    {
                        foreach (CDice die in _diceList)
                        {
                            die.Roll();
                        }

                        _nbDiceRollsLeft--;
                        _turnState = eTurnState.eTS_RollingDice; // STODO: Implement eTurnState.eTS_ReactToDiceAction
                    }
                    break;

                case eTurnState.eTS_RollingDice:
                    {
                        foreach (CDice die in _diceList)
                        {
                            if (die.ShouldReroll)
                            {
                                die.Roll();
                            }
                        }

                        _nbDiceRollsLeft--;
                        _turnState = _nbDiceRollsLeft == 0 ? eTurnState.eTS_EndingTurn : eTurnState.eTS_RollingDice; // STODO: Implement eTurnState.eTS_ReactToDiceAction
                    }
                    break;

                case eTurnState.eTS_EndingTurn:
                    {
                        CalculateAndApplyDiceResult();

                        // STODO: Implement eTurnState.eTS_PurchaseCardsAction

                        LogGameState();

                        if (!IsGameCompleted())
                        {
                            _nbDiceRollsLeft = 3;
                            _totalTurnID++;

                            do
                            {
                                _currentPlayerIndex = (_currentPlayerIndex + 1) % (uint)Players.Count;

                                if (_currentPlayerIndex == _startingPlayerIndex)
                                {
                                    _turnID++;
                                }

                            } while (!CurrentPlayer.IsAlive());
                        }
                    }
                    goto case eTurnState.eTS_StartingTurn;

                case eTurnState.eTS_ReactToDiceAction:
                case eTurnState.eTS_ApplyDamageOnTokyoAction:
                case eTurnState.eTS_PurchaseCardsAction:
                    { 
                        Debug.Assert(false, "CGame - Tick: Should not be called while an action is expected.");
                    }
                    break;
            }            
        }

        public bool IsGameCompleted()
        {
            return GetWinningPlayerId() != -1 && GetAlivePlayerCount() > 0;
        }

        #endregion // IGame
    }
}
