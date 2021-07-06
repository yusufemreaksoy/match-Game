using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScaler : MonoBehaviour
{
    private Board board;
    
    public float cameraOffset;
    public float aspectRatio = 0.625f;
    public float padding = 2;

    private void Start()
    {
        board = FindObjectOfType<Board>();

        if (board != null)
        {
            RepositionCamera(board.widht - 1, board.height - 1);
        }
    }

    private void RepositionCamera(float x, float y)
    {
        Vector3 tempPos = new Vector3(x / 2, y / 2, cameraOffset);
        transform.position = tempPos;
        if(board.widht >= board.height)
        {
            Camera.main.orthographicSize = (board.widht / 2 + padding) / aspectRatio;
        }
        else
        {
            Camera.main.orthographicSize = board.height / 2 + padding;
        }
    }
}
