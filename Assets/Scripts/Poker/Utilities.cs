using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
    public enum CardValue { LowAce = 1, Two = 2, Three = 3, Four = 4, Five = 5, Six = 6, Seven = 7, Eight = 8, Nine = 9, Ten = 10, Jack = 11, Queen = 12, King = 13, Ace = 14 }
    public enum CardSuit { Spade = 0, Heart = 1, Diamond = 2, Clover = 3 }
    public enum Hand { Royal = 9, StraightFlush = 8, FourKind = 7, FullHouse = 6, Flush = 5, Straight = 4, ThreeKind = 3, TwoPair = 2, Pair = 1, High = 0 }
    public enum PlayStatus { Betting, Checked, Folded }
    public enum EventCodes {PlayerViewId,PlayerCards };

    public delegate void DealingCardsEvent();
    public delegate void CommunityCardsUpdate();
    public class CompareCardsByValue : IComparer<Card>
    {
        public int Compare(Card x, Card y)
        {
            return (int)y.value - (int)x.value;
        }
    }

}
