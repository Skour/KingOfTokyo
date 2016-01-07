using System;
using System.ComponentModel;

namespace BoardGames.KingOfTokyo
{
    enum eDiceResult
    {
        [Description("1")]
        eDR_One,
        [Description("2")]
        eDR_Two,
        [Description("3")]
        eDR_Three,
        [Description("☇")]
        eDR_Energy,
        [Description("☄")]
        eDR_Punch,
        [Description("♥")]
        eDR_Heart,

        eDR_INVALID,
        eDR_NbFaces = 6,
    }

    class CDice: IComparable
    {
        #region Fields

        private eDiceResult _result = eDiceResult.eDR_INVALID;
        private bool _shouldReroll = false;

        #endregion

        #region Properties

        public eDiceResult Result
        {
            get
            {
                return _result;
            }
        }

        public bool ShouldReroll
        {
            get
            {
                return _shouldReroll;
            }
            set
            {
                ShouldReroll = value;
            }
        }

        #endregion

        #region Members

        public void Roll()
        {
            _result = (eDiceResult)(CGame.Rng.Next(0, (int)(eDiceResult.eDR_NbFaces)));
        }            

        #endregion

        #region IComparable Members
            
        public int CompareTo(object aOther)
        {
            CDice aOtherDice = (CDice)aOther;

            if (Result > aOtherDice.Result)
            {
                return 1;
            }
            else if (Result < aOtherDice.Result)
            {
                return -1;
            }                  
            else
            {
                return 0;
            }
        }

        #endregion
    }
}
