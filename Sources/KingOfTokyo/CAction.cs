using BoardGames.Framework;
using System;
using System.Collections.Generic;

namespace BoardGames.KingOfTokyo
{
    enum eActionType
    {
        eAT_MarkDiceForReroll,
        eAT_ReactToDamageOnTokyo,
        //eAT_PurchaseCard,
        //eAT_UseCard,
    }

    class CActionMarkDiceForReroll : IAction
    {
        #region Fields

        private List<int> _diceToRerollIndexList;

        #endregion // Fields

        #region Properties

        public List<int> DiceToRerollIndexList
        {
            get
            {
                return _diceToRerollIndexList;
            }
        }

        #endregion

        #region Constructors

        public CActionMarkDiceForReroll(List<int> aDiceToRerollIndexList)
        {
            _diceToRerollIndexList = aDiceToRerollIndexList;
        }

        #endregion // Constructors

        #region IAction

        public uint GetActionType()
        {
            return (uint)eActionType.eAT_MarkDiceForReroll;
        }

        #endregion // IAction
    }

    class CActionReactToDamageOnTokyo : IAction
    {
        #region Fields

        private bool _shouldExitTokyo;

        #endregion // Fields

        #region Properties

        public bool ShouldExitTokyo
        {
            get
            {
                return _shouldExitTokyo;
            }
        }

        #endregion

        #region Constructors

        public CActionReactToDamageOnTokyo(bool aShouldExitTokyo)
        {
            _shouldExitTokyo = aShouldExitTokyo;
        }

        #endregion // Constructors

        #region IAction

        public uint GetActionType()
        {
            return (uint)eActionType.eAT_ReactToDamageOnTokyo;
        }

        #endregion // IAction
    }
}
