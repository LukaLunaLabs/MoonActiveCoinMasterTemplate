using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct CanvasOrientationData
{
    [Header("Canvas Scalar")]
    public CanvasScaler m_CanvasScalar;
    
    [Header("Orientation Size Parameters")]
    public int m_xCanvasSizePortrait;
    public int m_yCanvasSizePortrait;
    public int m_xCanvasSizeLandscape;
    public int m_yCanvasSizeLandscape;
}
