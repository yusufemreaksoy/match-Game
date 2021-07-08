using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintManager : MonoBehaviour
{
    #region Variables
    private Board board;
    
    [Header("Variables")]
    public float hintDelay;

    private float hintDelaySeconds;
    [Header("Game Objects")]
    public GameObject hintParticle;
    public GameObject CurrentHint;
    #endregion

    #region Unity Callback Functions
    private void Start()
    {
        board = FindObjectOfType<Board>();
        hintDelaySeconds = hintDelay;

    }

    private void Update()
    {
        hintDelaySeconds -= Time.deltaTime;
        if (hintDelaySeconds <= 0 && CurrentHint == null)
        {
            MarkHint();
            hintDelaySeconds = hintDelay;
        }
    }
    #endregion

    #region Helper Methods
    //First, find all possible matches on the board
    private List<GameObject> FindAllMatches()
    {
        List<GameObject> possibleMoves = new List<GameObject>();

        for (int i = 0; i < board.widht; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                if (board.allDots[i, j] != null)
                {
                    if (i < board.widht - 1)
                    {
                        if (board.SwitchAndCheck(i, j, Vector2.right))
                        {
                            possibleMoves.Add(board.allDots[i, j]);
                        }
                    }
                    if (j < board.height - 1)
                    {
                        if (board.SwitchAndCheck(i, j, Vector2.up))
                        {
                            possibleMoves.Add(board.allDots[i, j]);
                        }
                    }
                }
            }
        }
        return possibleMoves;
    }

    //Pick one of those matches randomly
    private GameObject PickOneRandomly()
    {
        List<GameObject> possibleMoves = new List<GameObject>();
        possibleMoves = FindAllMatches();
        if (possibleMoves.Count > 0)
        {
            int pieceToUse = Random.Range(0, possibleMoves.Count);
            return possibleMoves[pieceToUse];
        }
        return null;
    }

    //Create the hint behind the chosen match
    private void MarkHint()
    {
        GameObject move = PickOneRandomly();
        if (move != null)
        {
            CurrentHint = Instantiate(hintParticle, move.transform.position, Quaternion.identity);
        }
    }

    //Destroy the hint
    public void DestroyHint()
    {
        if (CurrentHint != null)
        {
            Destroy(CurrentHint);
            CurrentHint = null;
            hintDelaySeconds = hintDelay;
        }
    }
    #endregion
}

