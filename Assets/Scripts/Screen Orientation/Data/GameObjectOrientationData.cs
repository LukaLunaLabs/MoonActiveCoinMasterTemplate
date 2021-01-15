using System;
using UnityEngine;

[Serializable]
public struct GameObjectOrientationData
{
    [Header("Game Object")]
    public GameObject m_GameObject;
    
    [Header("Active Orientations")]
    public bool m_Scale;
    public bool m_Position;

    [Header("Orientation Scale Parameters")]
    public float m_xScalePortrait;
    public float m_yScalePortrait;
    public float m_zScalePortrait;
    public float m_xScaleLandscape;
    public float m_yScaleLandscape;
    public float m_zScaleLandscape;
    
    [Header("Orientation Position Parameters")]
    public int m_xPositionPortrait;
    public int m_yPositionPortrait;
    public int m_zPositionPortrait;
    public int m_xPositionLandscape;
    public int m_yPositionLandscape;
    public int m_zPositionLandscape;
}
