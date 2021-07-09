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

    #region Check Functions
    private void IsAdjacentBomb(Dot dot1, Dot dot2, Dot dot3)
    {
        if (dot1.isAdjentBomb)
        {
            currentMatches.Union(GetAdjacentPieces(dot1.column, dot1.row));
        }

        if (dot2.isAdjentBomb)
        {
            currentMatches.Union(GetAdjacentPieces(dot2.column, dot2.row));
        }

        if (dot3.isAdjentBomb)
        {
            currentMatches.Union(GetAdjacentPieces(dot3.column, dot3.row));
        }
    }
    private void IsRowBomb(Dot dot1, Dot dot2,Dot dot3)
    {
        if (dot1.isRowBomb)
        {
            currentMatches.Union(GetRowPieces(dot1.row));
        }

        if (dot2.isRowBomb)
        {
            currentMatches.Union(GetRowPieces(dot2.row));
        }

        if (dot3.isRowBomb)
        {
            currentMatches.Union(GetRowPieces(dot3.row));
        }

        
    }
    private void IsColumnBomb(Dot dot1, Dot dot2, Dot dot3)
    {
        if (dot1.isColumnBomb)
        {
            currentMatches.Union(GetColumnPieces(dot1.column));
        }

        if (dot2.isColumnBomb)
        {
            currentMatches.Union(GetColumnPieces(dot2.column));
            Debug.Log("column");
        }

        if (dot3.isColumnBomb)
        {
            currentMatches.Union(GetColumnPieces(dot3.column));
        }

       
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
                otherDot = null;
            }
        }
    }
    #endregion

    #region Other Functions
    private void AddToTheListAndMatch(GameObject dot)
    {
        if (!currentMatches.Contains(dot))
        {
            currentMatches.Add(dot);
        }
        dot.GetComponent<Dot>().isMatched = true;
    }
    private void GetNearbyPieces(GameObject dot1, GameObject dot2, GameObject dot3)
    {
        AddToTheListAndMatch(dot1);
        AddToTheListAndMatch(dot2);
        AddToTheListAndMatch(dot3);
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
                    Dot currentDotDot = currentDot.GetComponent<Dot>();
                    
                    if(i>0 && i <board.widht - 1)//for X axis
                    {
                        //Find left and right sides
                        GameObject leftDot = board.allDots[i - 1, j];
                        GameObject rightDot = board.allDots[i + 1, j];
                        if (leftDot != null && rightDot != null)
                        {
                            Dot lefDotDot = leftDot.GetComponent<Dot>();
                            Dot rightDotDot = rightDot.GetComponent<Dot>();

                            if (leftDot.tag == currentDot.tag && rightDot.tag == currentDot.tag)//if tags are equal each other
                            {

                                //If the dots are bomb then add the columns or rows to List

                                IsRowBomb(lefDotDot, currentDotDot, rightDotDot);
                                IsColumnBomb(lefDotDot, currentDotDot, rightDotDot);
                                IsAdjacentBomb(lefDotDot, currentDotDot, rightDotDot);

                                GetNearbyPieces(leftDot, currentDot, rightDot);
                            }
                            lefDotDot = null;
                            rightDotDot = null;
                        }
                        leftDot = null;
                        rightDot = null;
                    }

                    if (j > 0 && j < board.height - 1)//for Y axis
                    {
                        //find up and down sides
                        GameObject upDot = board.allDots[i , j + 1];
                        GameObject downDot = board.allDots[i , j - 1];

                        if (upDot != null && downDot != null)
                        {
                            Dot upDotDot = upDot.GetComponent<Dot>();
                            Dot downDotDot = downDot.GetComponent<Dot>();

                            if (upDot.tag == currentDot.tag && downDot.tag == currentDot.tag)//if tags are equal each other
                            {
                                IsColumnBomb( upDotDot, currentDotDot, downDotDot);
                                IsRowBomb(upDotDot, currentDotDot , downDotDot);
                                IsAdjacentBomb(upDotDot, currentDotDot, downDotDot);


                                GetNearbyPieces(upDot, currentDot, downDot);
                            }
                            upDotDot = null;
                            downDotDot = null;
                        }
                        upDot = null;
                        downDot = null;
                    }

                    currentDot = null;
                    currentDotDot = null;
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
                        Debug.Log(i+" , "+ j);
                    }
                }
            }
        }
    }

    
    
    #endregion

    #region List Functions
    List<GameObject> GetColumnPieces(int column)
    {
        List<GameObject> dots = new List<GameObject>();

        for (int i = 0; i < board.height; i++)
        {
            if (board.allDots[column, i] != null)
            {
                Dot dot = board.allDots[column, i].GetComponent<Dot>();
                if (dot.isRowBomb)
                {
                    dots.Union(GetRowPieces(i)).ToList();
                }

                dots.Add(board.allDots[column, i]);
                dot.isMatched = true;
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
                Dot dot = board.allDots[i, row].GetComponent<Dot>();
                if (dot.isColumnBomb)
                {
                    dots.Union(GetColumnPieces(i)).ToList();
                }

                dots.Add(board.allDots[i, row]);
                dot.isMatched = true;
            }
        }

        return dots;
    }
    List<GameObject> GetAdjacentPieces(int column,int row)
    {
        List<GameObject> dots = new List<GameObject>();
        for (int i = column -1 ; i <= column+1; i++)
        {
            for (int j = row -1; j <= row +1; j++)
            {
                //Check if the piece is inside the board
                if(i >=0 && i<board.widht && j>=0 && j < board.height)
                {
                    if (board.allDots[i, j] != null)
                    {
                        Dot dot = board.allDots[i, j].GetComponent<Dot>();
                        if (dot.isRowBomb)
                        {
                            dots.Union(GetRowPieces(j)).ToList();
                        }

                        if (dot.isColumnBomb)
                        {
                            dots.Union(GetColumnPieces(i)).ToList();
                        }

                        dots.Add(board.allDots[i, j]);
                        board.allDots[i, j].GetComponent<Dot>().isMatched = true;
                    }
                }
            }
        }
        return dots;
    }
    #endregion

}
