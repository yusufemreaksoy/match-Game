using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="newBoardData",menuName ="Data/Board Data")]
public class BoardData : ScriptableObject
{
    [Header("Assignments")]
    public FindMatches findMatches;
    public ScoreManager scoreManager;
    public SoundManager soundManager;

    [Header("General Variables")]
    public int widht;
    public int height;
    public int offset;
    public int basePieceValue = 20;
    public float refillDelay = 0.5f;
    public int streakValue = 1;
    public int[] scoreGoals;

    [Header("Game Objects")]
    public GameObject[] dots;
    public GameObject[,] allDots;
    public GameObject destroyEffect;
    public GameObject breakableTilePrefab;
    public GameObject backgroundTilePrefab;
    public Dot currentDot;

    [Header("Transforms")]
    public Transform newPieces;
    public Transform tileBack;
    public Transform particles;
}
