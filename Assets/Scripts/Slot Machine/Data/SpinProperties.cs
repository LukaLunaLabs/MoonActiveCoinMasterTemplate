using System;
using UnityEngine;

[Serializable]
public struct SpinProperties
{
    [Header("Loops")]
    [Range(1, 1000)]
    public int m_LoopTimes;
    
    [Header("Speed")]
    [Range(1, 6000)]
    public int m_Speed;
    
    [Header("Spin Result Symbols")]
    public int m_ResultSymbol1;
    public int m_ResultSymbol2;
    public int m_ResultSymbol3;

    [Header("Updated Inventory")]
    public int m_CoinAmount;
    public int m_EventProgressPoints;
    
    [Header("Spin Result Symbols")]
    public SpinCompleteAction[] m_SpinCompleteActions;
}