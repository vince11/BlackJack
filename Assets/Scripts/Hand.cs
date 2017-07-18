using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Hand : MonoBehaviour
    {

        public GameObject cardPrefab;
        public Sprite[] faces;
        protected List<Card> cards;

        public void initialise()
        {
            cards = new List<Card>();
        }

        public virtual void add(Card c)
        {
            cards.Add(c);
        }

        public void clear()
        {
            foreach (Card c in cards)
            {
                c.destroyCard();
            }
            cards.Clear();
        }

        public void flipFaceUp()
        {
            foreach(Card c in cards)
            {
                if (!c.isFaceUp) c.flip();
            }
        }

    }
}

