using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dot : MonoBehaviour
{
    [Header("Board Variables")]
    public int column;
    public int row;
    public int targetX;
    public int targetY;
    public int previousColumn;
    public int previousRow;
    public bool isMatched = false;
    public float dotLerpSpeed = .5f;

    private Vector2
        firstTouchPos,
        finalTouchPos,
        tempPos;

    private Board board;

    private FindMatches findMatches;

    public GameObject otherDot;

    [Header("Swipe Variables")]
    public float swipeAngle = 0f;
    public float swipeResist = 1f;

    [Header("Powerup Variables")]
    public bool isColumnBomb;
    public bool isRowBomb;
    public GameObject rowArrow;
    public GameObject columnArrow;

    private void Start()
    {
        board = FindObjectOfType<Board>();
        findMatches = FindObjectOfType<FindMatches>();
        //targetX = (int)transform.position.x;
        //targetY = (int)transform.position.y;
        //row = targetY;
        //column = targetX;
        //previousColumn = column;
        //previousRow = row;
    }

    private void Update()
    {
        //FindMatches();
/*
        if (isMatched)
        {
            SpriteRenderer sprite = GetComponent<SpriteRenderer>();
            sprite.color = new Color(1f, 1f, 1f, .2f);
        }
*/
        targetX = column;
        targetY = row;

        ApplyMoving();
    }

    private void OnMouseDown()
    {
        if (board.currentState == GameState.move)
        {
            //ilk posu ayarlama
            firstTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        
    }
    private void OnMouseUp()
    {
        if (board.currentState == GameState.move)
        {
            //ikinci posu ayarlama
            finalTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CalculateAngle();
        }

    }

    private void OnMouseOver()
    {
        //burası test için
        if (Input.GetMouseButtonDown(1))
        {
            isRowBomb = true;
            GameObject arrow = Instantiate(rowArrow, transform.position, Quaternion.identity);
            arrow.transform.parent = this.transform;
        }
        

        
    }

    void CalculateAngle()
    {
        //ilk basma anında hemen hesaplanmasın diye bir değişkenden daha büyük olmasına bakıyoruz
        if (Mathf.Abs(finalTouchPos.y - firstTouchPos.y) > swipeResist || Mathf.Abs(finalTouchPos.x - firstTouchPos.x) > swipeResist)
        {
            //ilk ve son nokta arası açıyı hesaplama
            swipeAngle = Mathf.Atan2(finalTouchPos.y - firstTouchPos.y, finalTouchPos.x - firstTouchPos.x) * 180 / Mathf.PI;
            MovePieces();
            board.currentState = GameState.wait;
            board.currentDot = this;
        }
        else
        {
            board.currentState = GameState.move;
        }
        //Debug.Log(swipeAngle); 
    }

    void MovePieces()
    {
        if (swipeAngle >-45 && swipeAngle<=45 && column < board.widht -1)
        {
            //Sağa kaydırma
            otherDot = board.allDots[column + 1, row];
            previousColumn = column;
            previousRow = row;
            otherDot.GetComponent<Dot>().column -= 1;
            column += 1;
        }
        else if(swipeAngle > 45 && swipeAngle <= 135 && row < board.height -1)
        {
            //Yukarı kaydırma
            otherDot = board.allDots[column, row + 1];
            previousColumn = column;
            previousRow = row;
            otherDot.GetComponent<Dot>().row -= 1;
            row += 1;
        }
        else if ((swipeAngle > 135 || swipeAngle <= -135) && column > 0)
        {
            //Sola kaydırma
            otherDot = board.allDots[column -1, row];
            previousColumn = column;
            previousRow = row;
            otherDot.GetComponent<Dot>().column += 1;
            column -= 1;
        }
        else if (swipeAngle < -45 && swipeAngle >= -135 && row > 0)
        {
            //Aşağı kaydırma
            otherDot = board.allDots[column, row-1];
            previousColumn = column;
            previousRow = row;
            otherDot.GetComponent<Dot>().row += 1;
            row -= 1;
        }

        StartCoroutine(CheckMoveCo());
    }

    void ApplyMoving()
    {
        if (Mathf.Abs(targetX - transform.position.x) > .1f)
        {
            // X ekseninde yavaşça haraket etme
            tempPos = new Vector2(targetX, transform.position.y);
            
            transform.position = Vector2.Lerp(transform.position, tempPos, dotLerpSpeed); //yavaşlatılmış bir şekilde gitmesi için
            if (board.allDots[column,row]!= this.gameObject)
            {
                board.allDots[column, row] = this.gameObject;
            }
            // haraket ettikten sonra eşleşme var mı ?
            findMatches.FindAllMatches();
        }
        else
        {
            //X ekseninde direkt haraket
            tempPos = new Vector2(targetX, transform.position.y);
            transform.position = tempPos;
        }
        if (Mathf.Abs(targetY - transform.position.y) > .1f)
        {
            // Y ekseninde yavaşça haraket etme
            tempPos = new Vector2(transform.position.x,targetY);
            transform.position = Vector2.Lerp(transform.position, tempPos, dotLerpSpeed);
            if (board.allDots[column, row] != this.gameObject)
            {
                board.allDots[column, row] = this.gameObject;
            }
            findMatches.FindAllMatches();
        }
        else
        {
            //Y ekseninde direkt haraket
            tempPos = new Vector2(transform.position.x,targetY);
            transform.position = tempPos;
        }
    }

  

    public IEnumerator CheckMoveCo()
    {
        yield return new WaitForSeconds(.5f);
        if (otherDot != null)
        {
            if (!isMatched & !otherDot.GetComponent<Dot>().isMatched) // iki dotta da eşleşme yok ise
            {
                // diğer dotun column ve row değişkenlerini set etme
                otherDot.GetComponent<Dot>().row = row;
                otherDot.GetComponent<Dot>().column = column;
                //önceki column ve rowu hafızaya alma
                row = previousRow;
                column = previousColumn;
                yield return new WaitForSeconds(.5f);
                board.currentDot = null;
                board.currentState = GameState.move;
            }
            else
            {
                //eşleşme varsa yok et
                board.DestroyMatches();
            }
            //otherDot = null;
        }
        
    }

    public void MakeRowBomb()
    {
        isRowBomb = true;
        GameObject arrow = Instantiate(rowArrow, transform.position, Quaternion.identity);
        arrow.transform.parent = this.transform;
    }

    public void MakeColumnBomb()
    {
        isColumnBomb = true;
        GameObject arrow = Instantiate(columnArrow, transform.position, Quaternion.identity);
        arrow.transform.parent = this.transform;
    }

}
