using UnityEngine;
using UnityEngine.UI;

namespace FortuneWheel
{
    public class PieceObject : MonoBehaviour
    {
        public Image backgroundImage;
        public Image rewardIcon;
        public Text rewardAmount;
        public GameController1.RewardEnum rewardCategory1;
        public GameController.RewardEnum rewardCategory;

        public int index;
        private void Awake()
        {
            //if(!Menu.instance.isFortuneWheelOpened)
            //{
            //rewardIcon.gameObject.SetActive(false);
            //rewardAmount.gameObject.SetActive(false);
            //} startingAngle = 0;

            

        }

        public void SetValues(int pieceNo)
        {
            Debug.Log("____SetValues");
            if (GameController.ins)
            {
                index = pieceNo;

                if (GameController.ins.useCustomBackgrounds)
                {
                    backgroundImage.color = Color.white;
                    backgroundImage.sprite = GameController.ins.CustomBackgrounds[pieceNo];
                }
                else
                {
                    backgroundImage.color = GameController.ins.PiecesOfWheel[pieceNo].backgroundColor;
                    backgroundImage.sprite = GameController.ins.PiecesOfWheel[pieceNo].backgroundSprite;
                }

                rewardCategory = GameController.ins.PiecesOfWheel[pieceNo].rewardCategory;
                rewardAmount.text = GameController.ins.PiecesOfWheel[pieceNo].rewardAmount.ToString();

                for (int i = 0; i < GameController.ins.categoryIcons.Length; i++)
                {
                    if (rewardCategory == GameController.ins.categoryIcons[i].category)
                    {
                        rewardIcon.sprite = GameController.ins.categoryIcons[i].rewardIcon;
                        GameController.ins.PiecesOfWheel[pieceNo].rewardIcon = GameController.ins.categoryIcons[i].rewardIcon;
                    }
                }
            }
            
        }
        public void SetValues1(int pieceNo)
        {
            rewardIcon.gameObject.SetActive(false);
            rewardAmount.gameObject.SetActive(false);
            Debug.Log("____SetValues1");
            index = pieceNo;

            if (GameController1.ins.useCustomBackgrounds)
            {
                backgroundImage.color = Color.white;
                backgroundImage.sprite = GameController1.ins.CustomBackgrounds[pieceNo];
            }
            else
            {
                backgroundImage.color = GameController1.ins.PiecesOfWheel[pieceNo].backgroundColor;
                backgroundImage.sprite = GameController1.ins.PiecesOfWheel[pieceNo].backgroundSprite;
            }

            rewardCategory1 = GameController1.ins.PiecesOfWheel[pieceNo].rewardCategory;
            rewardAmount.text = GameController1.ins.PiecesOfWheel[pieceNo].rewardAmount.ToString();

            for (int i = 0; i < GameController1.ins.categoryIcons.Length; i++)
            {
                if (rewardCategory1 == GameController1.ins.categoryIcons[i].category)
                {
                    rewardIcon.sprite = GameController1.ins.categoryIcons[i].rewardIcon;
                    GameController1.ins.PiecesOfWheel[pieceNo].rewardIcon = GameController1.ins.categoryIcons[i].rewardIcon;
                }
            }
        }
    }
}