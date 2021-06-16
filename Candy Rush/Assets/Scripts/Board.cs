using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    wait,
    move
}

public class Board : MonoBehaviour
{
    [HideInInspector]
    public GameState currentState = GameState.move;

    private FindMatches findMatches;

    public int widht,height;
    public int offset;

    public GameObject[] dots;
    public GameObject[,] allDots;
    public GameObject destroyEffect;

    public Dot currentDot;

    private void Start()
    {
        findMatches = FindObjectOfType<FindMatches>();
        allDots = new GameObject[widht, height];
        SetUp();
    }


    void SetUp()
    {
        //create the matrix
        for (int i = 0; i < widht; i++)
        {
            for (int j = 0; j <height; j++)
            {
                Vector2 tempPos = new Vector2(i, j + offset);//set temp pos
                int dotToUse = Random.Range(0, dots.Length);//create random index
                int maxIterations = 0;
                while (MatchesAt(i,j,dots[dotToUse]) && maxIterations < 100)//if there is a match at the beginning change the index
                {
                    dotToUse = Random.Range(0, dots.Length);
                    maxIterations++;
                }

                maxIterations = 0;

                GameObject dot = Instantiate(dots[dotToUse], tempPos, Quaternion.identity);
                dot.GetComponent<Dot>().row= j;
                dot.GetComponent<Dot>().column = i;
                dot.transform.parent = this.transform;
                dot.name = "( " + i + ", " + j + " )";
                allDots[i, j] = dot;
            }
        }
    }

    bool MatchesAt(int column, int row, GameObject piece)
    {
        if(column > 1 && row > 1)
        {
            if(allDots[column -1 , row].tag == piece.tag && allDots[column - 2, row].tag == piece.tag)
            {
                return true;
            }
            if (allDots[column , row - 1].tag == piece.tag && allDots[column, row - 2].tag == piece.tag)
            {
                return true;
            }
        }
        else if (column <= 1 || row >= 1)
        {
            if (row > 1)
            {
                if(allDots[column,row-1].tag==piece.tag && allDots[column, row - 2].tag == piece.tag)
                {
                    return true;
                }
            }
            if (column > 1)
            {
                if (allDots[column - 1, row].tag == piece.tag && allDots[column - 2, row].tag == piece.tag)
                {
                    return true;
                }
            }
        }
        return false;
    }

    void DestroyMatchesAt(int column, int row)
    {
        if (allDots[column, row].GetComponent<Dot>().isMatched)
        {
            //How many elements are in the matched pieces list from findmatches ?
            Debug.Log(findMatches.currentMatches.Count);
            if(findMatches.currentMatches.Count == 4 || findMatches.currentMatches.Count == 7)
            {
                findMatches.CheckBombs();
            }

            findMatches.currentMatches.Remove(allDots[column, row]);
            GameObject particle = Instantiate(destroyEffect, allDots[column, row].transform.position, Quaternion.identity);
            Destroy(particle, .5f);
            Destroy(allDots[column, row]);
            allDots[column, row] = null;
        }
    }

    public void DestroyMatches()
    {
        for (int i = 0; i < widht; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    DestroyMatchesAt(i, j);
                }
            }
        }
        StartCoroutine(DecreaseRowCo());
    }
    
    private void RefillBoard()
    {
        for (int i = 0; i < widht; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] == null)
                {
                    Vector2 tempPos = new Vector2(i, j+offset);
                    int dotToIse = Random.Range(0, dots.Length);
                    GameObject piece = Instantiate(dots[dotToIse], tempPos, Quaternion.identity);
                    allDots[i, j] = piece;
                    piece.GetComponent<Dot>().row = j;
                    piece.GetComponent<Dot>().column = i;
                }
            }
        }
    }

    private bool MatchesOnBoard()
    {
        for (int i = 0; i < widht; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    if(allDots[i, j].GetComponent<Dot>().isMatched)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private IEnumerator DecreaseRowCo()
    {
        int nullCont = 0;
        for (int i = 0; i < widht; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] == null)
                {
                    nullCont++;
                }
                else if(nullCont > 0)
                {
                    allDots[i, j].GetComponent<Dot>().row -= nullCont;
                    allDots[i, j] = null;
                }
            }

            nullCont = 0;
        }
        yield return new WaitForSeconds(.4f);
        StartCoroutine(FillBoardCo());
    }

    private IEnumerator FillBoardCo()
    {
        RefillBoard();
        yield return new WaitForSeconds(.5f);

        while (MatchesOnBoard())
        {
            yield return new WaitForSeconds(.5f);
            DestroyMatches();
        }
        findMatches.currentMatches.Clear();
        currentDot = null;
        yield return new WaitForSeconds(.5f);

        currentState = GameState.move;
    }

}
