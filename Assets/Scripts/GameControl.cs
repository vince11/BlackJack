using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class GameControl : MonoBehaviour
    {
        const int dealPhase = 0, playerPhase = 1, housePhase = 2, endPhase = 3, canvasScale = 50, padding = 30;

        public GameObject deckPrefab;
        public GameObject playerPrefab;
        public GameObject buttonPrefab;
        public GameObject textPrefab;

        public GameObject darkScreen;
        private Button[] gameButtons;
        private Text playerTxt, houseTxt,statusTxt;

        private Deck deck;
        private Player player, house;
        
        private int currentPhase;


        void Awake()
        {
            currentPhase = dealPhase;
            darkScreen = GameObject.Find("Image");  

            initPlayers();
            initButtons();

            deck = deckPrefab.GetComponent<Deck>();
            deck.initialise();
            deck.populate();
            deck.shuffle();

        }
 
        void Update()
        {
            if(currentPhase == playerPhase)
            {
                int status = player.getValue();
                if (status == -1)
                {
                    playerTxt.text = "Bust";
                    StartCoroutine(gameOver(status));
                }
                else if (status == -2)
                {
                    playerTxt.text = "Black Jack";
                    StartCoroutine(gameOver(status));
                }
                else playerTxt.text = "Value: " + player.getValue().ToString();

                houseTxt.text = "Value: " + house.getValue().ToString();
            }
            else if(currentPhase == housePhase || currentPhase == endPhase)
            {
                int status = house.getValue();
                if (status == -1) houseTxt.text = "Bust";
                else if (status == -2) houseTxt.text = "Black Jack";
                else houseTxt.text = "Value: " + status.ToString();
            }
        }

        private void initButtons()
        {
            gameButtons = new Button[4];
            for (int i = 0; i < gameButtons.Length; i++)
            {
                //0 -> hit, 1 -> stand, 2-> restart, 3-> start
                GameObject button = Instantiate(buttonPrefab);
                gameButtons[i] = button.GetComponent<Button>();
                string buttonName = "";
                float x, y;
                bool isActive;
                switch (i)
                {
                    case 0:
                        buttonName = "Hit";
                        x = (player.xPos * canvasScale) - 50; y = -canvasScale;
                        isActive = false;
                        break;
                    case 1:
                        buttonName = "Stand";
                        x = (player.xPos * canvasScale) + 50; y = -canvasScale;
                        isActive = false;
                        break;
                    case 2:
                        buttonName = "Restart";
                        x = 0; y = -canvasScale;
                        isActive = false;
                        break;
                    default:
                        buttonName = "Start";
                        x = 0; y = 0;
                        isActive = true;
                        break;
                }

                button.transform.SetParent(GameObject.Find("Canvas").transform);
                gameButtons[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
                gameButtons[i].GetComponentInChildren<Text>().text = buttonName;
                gameButtons[i].gameObject.SetActive(isActive);
            }

            gameButtons[0].onClick.AddListener(() => hitPressed());
            gameButtons[1].onClick.AddListener(() => standPressed());
            gameButtons[2].onClick.AddListener(() => restartPressed());
            gameButtons[3].onClick.AddListener(() => startPressed());
            gameButtons[3].GetComponent<RectTransform>().sizeDelta = new Vector2(400, 100);
        }

        private void initPlayers()
        {
            GameObject txtClone1 = Instantiate(textPrefab);
            GameObject txtClone2 = Instantiate(textPrefab);
            txtClone1.transform.SetParent(GameObject.Find("Canvas").transform);
            txtClone2.transform.SetParent(GameObject.Find("Canvas").transform);
            RectTransform playerTxtRT = txtClone1.GetComponent<RectTransform>();
            RectTransform houseTxtRT = txtClone2.GetComponent<RectTransform>();
            playerTxt = txtClone1.GetComponent<Text>(); 
            houseTxt = txtClone2.GetComponent<Text>();

            player = playerPrefab.GetComponent<Player>();
            player.initialise(0, -3.5f, false);
            playerTxtRT.anchoredPosition = new Vector2((player.xPos) * canvasScale, (player.yPos + 1) * canvasScale + padding);

            GameObject playerClone = Instantiate(playerPrefab);
            house = playerClone.GetComponent<Player>();
            house.initialise(0, 1.5f, true);
            houseTxtRT.anchoredPosition = new Vector2((house.xPos) * canvasScale, (house.yPos - 1) * canvasScale - padding);

            GameObject statusTxtClone = Instantiate(textPrefab);
            statusTxtClone.transform.SetParent(GameObject.Find("Canvas").transform);
            statusTxtClone.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 100);
            statusTxtClone.GetComponent<RectTransform>().sizeDelta = new Vector2(300, 50);
            statusTxt = statusTxtClone.GetComponent<Text>();
            statusTxt.fontSize = 30;

            statusTxt.gameObject.SetActive(false);
            playerTxt.gameObject.SetActive(false);
            houseTxt.gameObject.SetActive(false);
        }

        private void dealCards()
        {
            StartCoroutine(initialDeal());
        }

        private IEnumerator initialDeal()
        {
            yield return new WaitForSeconds(1);
            int n = 2;
            while (n > 0)
            {
                deck.deal(player);
                yield return new WaitForSeconds(1);
                deck.deal(house);
                yield return new WaitForSeconds(1);
                n--;
            }

            gameButtons[0].gameObject.SetActive(true);
            gameButtons[1].gameObject.SetActive(true);
            playerTxt.gameObject.SetActive(true);
            houseTxt.gameObject.SetActive(true);

            currentPhase = playerPhase;

        }

        private IEnumerator gameOver(int status)
        {
            gameButtons[0].gameObject.SetActive(false);
            gameButtons[1].gameObject.SetActive(false);
            yield return new WaitForSeconds(.5f);
            currentPhase = endPhase;
            string playerStatus = "";

            if (status == -1) playerStatus = "You Lose!";
            else if (status == -2) playerStatus = "You Win!";
            else if (status == -3) playerStatus = "Draw!";

            darkScreen.SetActive(true);
            statusTxt.gameObject.SetActive(true);
            statusTxt.text = playerStatus;

            gameButtons[2].gameObject.SetActive(true);
        }

        public void hitPressed()
        {
            deck.deal(player);
        }


        public void standPressed()
        {
            currentPhase = housePhase;
            StartCoroutine(houseMove());
        }

        private IEnumerator houseMove()
        {
            while (house.houseHitting())
            {
                deck.deal(house);
                yield return new WaitForSeconds(1f);
            }
            int houseVal = house.getValue();
            int playerVal = player.getValue();
            if (houseVal == -2 || (houseVal > playerVal)) StartCoroutine(gameOver(-1));//house black jack OR house greater, player loses
            else if (houseVal == -1 || (houseVal < playerVal)) StartCoroutine(gameOver(-2)); //house busted OR house lesser, player wins
            else if (houseVal == playerVal) StartCoroutine(gameOver(-3)); ; //same value on each player's hand
        }

        public void restartPressed()
        {
            deck.clear();
            player.clear();
            house.clear();
            gameButtons[2].gameObject.SetActive(false);
            darkScreen.SetActive(false);

            statusTxt.gameObject.SetActive(false);
            playerTxt.gameObject.SetActive(false);
            houseTxt.gameObject.SetActive(false);

            deck.populate();
            deck.shuffle();
            currentPhase = dealPhase;
            dealCards();
        }

        public void startPressed()
        {
            darkScreen.SetActive(false);
            dealCards();
            gameButtons[3].gameObject.SetActive(false);
            
        }

    }

}
