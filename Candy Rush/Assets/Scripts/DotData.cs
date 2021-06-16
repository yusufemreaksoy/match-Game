using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName =("NewDotData"),menuName =("Data/ Dot Data"))]
public class DotData: ScriptableObject
{
    public float dotLerpSpeed = .5f;
    public float swipeResist = 1f;
}
