using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FindMatches : MonoBehaviour
{
    private Board board;

    public List<GameObject> currentMatches = new List<GameObject>();

    private void Start()
    {
        board = FindObjectOfType<Board>();
    }

    public void FindAllMatches()
    {
        //This for call the Co
        StartCoroutine(FindAllMathcesCo()); 
    }

    private IEnumerator FindAllMathcesCo()
    {
        yield return new WaitForSeconds(.2f);
        //Create the matrix
        for (int i = 0; i < board.widht; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                //Find the current dot
                GameObject currentDot = board.allDots[i, j];
                if (currentDot != null)
                {
                    if(i>0 && i <board.widht - 1)//for X axis
                    {
                        //Find left and right sides
                        GameObject leftDot = board.allDots[i - 1, j];
                        GameObject rightDot = board.allDots[i + 1, j];

                        if(leftDot !=null&&rightDot != null)
                        {
                            if (leftDot.tag == currentDot.tag && rightDot.tag == currentDot.tag)//if tags are equal each other
                            {
                                #region Union statements

                                //İf the dots are bomb then add the columns or rows to List

                                if (currentDot.GetComponent<Dot>().isRowBomb || rightDot.GetComponent<Dot>().isRowBomb || leftDot.GetComponent<Dot>().isRowBomb)
                                {
                                    currentMatches.Union(GetRowPieces(j));
                                }

                                if (currentDot.GetComponent<Dot>().isColumnBomb)
                                {
                                    currentMatches.Union(GetColumnPieces(i));
                                }

                                if (rightDot.GetComponent<Dot>().isColumnBomb)
                                {
                                    currentMatches.Union(GetColumnPieces(i+1));
                                }

                                if (leftDot.GetComponent<Dot>().isColumnBomb)
                                {
                                    currentMatches.Union(GetColumnPieces(i-1));
                                }
                                #endregion

                                if (!currentMatches.Contains(leftDot))
                                {
                                    currentMatches.Add(leftDot);
                                }
                                leftDot.GetComponent<Dot>().isMatched = true;

                                if (!currentMatches.Contains(rightDot))
                                {
                                    currentMatches.Add(rightDot);
                                }
                                rightDot.GetComponent<Dot>().isMatched = true;

                                if (!currentMatches.Contains(currentDot))
                                {
                                    currentMatches.Add(currentDot);
                                }
                                currentDot.GetComponent<Dot>().isMatched = true;
                            }
                        }
                    }

                    if (j > 0 && j < board.height - 1)//for Y axis
                    {
                        //find up and down sides
                        GameObject upDot = board.allDots[i , j + 1];
                        GameObject downDot = board.allDots[i , j - 1];
                        if (upDot != null && downDot != null)
                        {
                            if (upDot.tag == currentDot.tag && downDot.tag == currentDot.tag)//if tags are equal each other
                            {
                                #region Union statements
                                if (currentDot.GetComponent<Dot>().isColumnBomb || upDot.GetComponent<Dot>().isColumnBomb || downDot.GetComponent<Dot>().isColumnBomb)
                                {
                                    currentMatches.Union(GetColumnPieces(i));
                                }

                                if (currentDot.GetComponent<Dot>().isRowBomb)
                                {
                                    currentMatches.Union(GetRowPieces(j));
                                }

                                if (upDot.GetComponent<Dot>().isRowBomb)
                                {
                                    currentMatches.Union(GetRowPieces(j + 1));
                                } 

                                if (downDot.GetComponent<Dot>().isRowBomb)
                                {
                                    currentMatches.Union(GetRowPieces(j - 1));
                                }
                                #endregion

                                if (!currentMatches.Contains(upDot))
                                {
                                    currentMatches.Add(upDot);
                                }
                                upDot.GetComponent<Dot>().isMatched = true;

                                if (!currentMatches.Contains(downDot))
                                {
                                    currentMatches.Add(downDot);
                                }
                                downDot.GetComponent<Dot>().isMatched = true;

                                if (!currentMatches.Contains(currentDot))
                                {
                                    currentMatches.Add(currentDot);
                                }
                                currentDot.GetComponent<Dot>().isMatched = true;
                            }
                        }
                    }
                }
            }
        }
    }

    public void MatchPiecesOfColor(string color)
    {
        for (int i = 0; i < board.widht; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                //Check if that piece exists
                if (board.allDots[i, j] != null)
                {
                    //Check if tag on that dot
                    if(board.allDots[i,j].tag == color)
                    {
                        //Set that dot to be matched
                        board.allDots[i, j].GetComponent<Dot>().isMatched = true;
                    }
                }
            }
        }
    }

    List<GameObject> GetColumnPieces(int column)
    {
        List<GameObject> dots = new List<GameObject>();

        for (int i = 0; i < board.height; i++)
        {
            if (board.allDots[column, i] != null)
            {
                dots.Add(board.allDots[column, i]);
                board.allDots[column, i].GetComponent<Dot>().isMatched = true;
            }
        }
        
        return dots;
    }
    List<GameObject> GetRowPieces(int row)
    {
 
        List<GameObject> dots = new List<GameObject>();

        for (int i = 0; i < board.widht; i++)
        {
            if (board.allDots[i, row] != null)
            {
                dots.Add(board.allDots[i, row]);
                board.allDots[i, row].GetComponent<Dot>().isMatched = true;
            }
        }

        return dots;
    }

    public void CheckBombs()
    { //Did the player move something ?
        if (board.currentDot != null)
        { //Is the piece they moved matched ?
            if (board.currentDot.isMatched)
            {
                //make it unmatched
                board.currentDot.isMatched = false;

                //Decide what kind of bomb to make
                if((board.currentDot.swipeAngle >-45 && board.currentDot.swipeAngle <= 45) || (board.currentDot.swipeAngle < -135 || board.currentDot.swipeAngle >=135))
                {
                    board.currentDot.MakeRowBomb();
                }
                else
                {
                    board.currentDot.MakeColumnBomb();
                }
            }
            //Is the other piece matched ?
            else if (board.currentDot.otherDot !=null)
            {
                Dot otherDot = board.currentDot.otherDot.GetComponent<Dot>();
                if (otherDot.isMatched)
                {
                    //Make it unmatched
                    otherDot.isMatched = false;

                    if ((board.currentDot.swipeAngle > -45 && board.currentDot.swipeAngle <= 45) || (board.currentDot.swipeAngle < -135 || board.currentDot.swipeAngle >= 135))
                    {
                        otherDot.MakeRowBomb();
                    }
                    else
                    {
                        otherDot.MakeColumnBomb();
                    }
                }
            }
        }
    }

}
