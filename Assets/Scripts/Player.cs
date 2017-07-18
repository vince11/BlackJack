using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Player : Hand
    {
        public float xPos, yPos;
        private bool isHouse;
        public void initialise(float x, float y, bool houseOrPlayer)
        {
            base.initialise();
            xPos = x;
            yPos = y;
            isHouse = houseOrPlayer;
        }

        public override void add(Card c)
        {
            base.add(c);
            int size = cards.Count;
            for(int i = 0; i < size; i++)
            {
                if (i < size - 1)
                {
                    //cards[i].transform.position = new Vector2(cards[i].transform.position.x - .2f, yPos);
                    iTween.MoveTo(cards[i].gameObject, new Vector2(cards[i].transform.position.x - .2f, yPos), 1f);
                }
                else
                {
                    //cards[i].transform.position = new Vector2(xPos + (i * (.2f)), yPos);
                    iTween.MoveTo(cards[i].gameObject, new Vector2(xPos + (i * (.2f)), yPos), 1f);
                }

                if(!cards[i].isFaceUp && (!isHouse || i != 1)) cards[i].flip();
            } 
        }

        public int getValue()
        {
            int val = 0;
            bool hasAce = false;
            foreach(Card c in cards)
            {
                if (c.isFaceUp)
                {
                    if (c.Rank >= Card.Ranks.Ten) val += 10;
                    else val += (int)c.Rank + 1;

                    if (c.Rank == Card.Ranks.Ace) hasAce = true;
                } 
            }

            if (hasAce && val <= 11) val += 10;

            if (val >= 0 && val < 21) return val;
            else if (val > 21) return -1; //bust
            else return -2;
        }

        public bool houseHitting()
        {
            flipFaceUp();
            int val = getValue();
            if (val >= 0 && val <= 15) return true;

            return false;
        }
    }
}

