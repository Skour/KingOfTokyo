using BoardGames.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace BoardGames.KingOfTokyo
{
    class CPlayer : IPlayer
    {
        #region Const Fields

        private const int MinLifePoints = 0;
        private const int MinVictoryPoints = 0;
        private const int MaxVictoryPoints = 20;

        #endregion

        #region Fields

        private eLocations _location = eLocations.eL_Outside;
        private uint _lifePoints = 10;
        private uint _victoryPoints = 0;
        private uint _energyPoints = 0;
        private int _maxLifePoints = 10;

        #endregion

        #region Properties

        public eLocations Location
        {
            get
            {
                return _location;
            }
            set
            {
                _location = value;
            }
        }

        public uint LifePoints
        {
            get
            {
                return _lifePoints;
            }
            set
            {
                _lifePoints = value;
            }
        }

        public uint VictoryPoints
        {
            get
            {
                return _victoryPoints;
            }
            set
            {
                _victoryPoints = value;
            }
        }

        public uint EnergyPoints
        {
            get
            {
                return _energyPoints;
            }
            set
            {
                _energyPoints = value;
            }
        }

        #endregion

        #region Constructors

        public CPlayer()
        {
        }

        #endregion

        #region Members

        public void AdjustLifePoints(int aLifePointsDelta)
        {
            LifePoints = (uint)Math.Min(_maxLifePoints, Math.Max(MinLifePoints, LifePoints + aLifePointsDelta));
        }

        public void AdjustVictoryPoints(int aVictoryPointsDelta)
        {
            VictoryPoints = (uint)Math.Min(MaxVictoryPoints, Math.Max(MinVictoryPoints, VictoryPoints + aVictoryPointsDelta));
        }

        public void AdjustEnergyPoints(int aEnergyPointsDelta)
        {
            Debug.Assert(EnergyPoints + aEnergyPointsDelta >= 0);

            EnergyPoints = (uint)Math.Max(0, EnergyPoints + aEnergyPointsDelta);
        }

        public bool IsAlive()
        {
            return LifePoints > MinLifePoints;
        }

        public bool HasAchievedVictoryPointsWin()
        {
            return VictoryPoints == MaxVictoryPoints;
        }

        override public String ToString()
        {
            return String.Format("Location: {0}, Life Points: {1}, Victory Points: {2}, Energy Points: {3}",
                                        Location.GetDescription(),
                                        LifePoints,
                                        VictoryPoints,
                                        EnergyPoints);
        }

        #endregion

        #region IPlayer

        public IAction ProvideAction(IGame aGame)
        {
            CGame kotGame = (CGame)aGame;

            switch(kotGame.TurnState)
            {
                case eTurnState.eTS_ReactToDicePlayerHook:
                    {
                        // STODO: Implement logic for selection
                        List<int> diceIndex = new List<int>();
                        for (int i = 0; i < kotGame.Dice.Count; ++i)
                        {
                            if((_location == eLocations.eL_Outside && kotGame.Dice[i].Result != eDiceResult.eDR_Three) ||
                               (_location != eLocations.eL_Outside && kotGame.Dice[i].Result != eDiceResult.eDR_Punch))
                            {
                                diceIndex.Add(i);
                            }
                        }

                        return new CActionMarkDiceForReroll(diceIndex);
                    }                  
            }

            return null;
        }

        #endregion // IPlayer
    }
}
