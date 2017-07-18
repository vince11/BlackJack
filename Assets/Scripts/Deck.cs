using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Deck : Hand
    {      
        public void populate()
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 13; j++)
                {
                    GameObject card = Instantiate(cardPrefab);
                    Card c = card.GetComponent<Card>() as Card;
                    c.initialise((Card.Suits)i, (Card.Ranks)j, new Vector2(0, 4), Quaternion.identity,faces[(i*13) + j]);
                    add(c);
                }
            }
        }

        public void displayCards()
        {
            float x = -3f;
            float y = 4.5f;
            for (int r = 0; r < 4; r++)
            {
                for (int c = 0; c < 13; c++)
                {
                    int index = (r * 13) + c;
                    cards[index].transform.position = new Vector2(x + (c*.5f), y + (r*-3));
                    if(c % 2 == 0) cards[index].flip();
                }
            }
        }

        public void shuffle()
        {
            for(int i = cards.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i);
                Card temp = cards[i];
                cards[i] = cards[j];
                cards[j] = temp;
                cards[i].GetComponent<SpriteRenderer>().sortingOrder = i;
            }
        }

        public void deal(Hand hand)
        {
            if(cards.Count != 0)
            {
                Card top = cards[0];
                cards.Remove(top);
                hand.add(top);
            }
           
        }
      
    }

}
