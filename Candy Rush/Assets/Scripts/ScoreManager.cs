using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public Text scoreText;
    public int score;
    public Image scoreBoard;

    private Board board;

    private void Start()
    {
        board = FindObjectOfType<Board>();
    }

    private void Update()
    {
        scoreText.text = score.ToString();
    }

    public void IncreaseScore(int amountToIncrease)
    {
        score += amountToIncrease;
        if(scoreBoard!=null && board != null)
        {
            int lenght = board.scoreGoals.Length;
            scoreBoard.fillAmount =(float)score / (float)board.scoreGoals[lenght - 1];
        }
    }
}
