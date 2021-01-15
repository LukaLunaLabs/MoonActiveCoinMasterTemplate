using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class OrientationManager : MonoBehaviour
{
    [Header("Canvas Orientation")]
    public CanvasOrientationData[] m_Canvases;
    
    [Header("Game Object Orientation")]
    public GameObjectOrientationData[] m_GameObjects;

    private Coroutine m_OrientationChangedCoroutine, m_CanvasOrientationChangedCoroutine;
    
    private void Start()
    {
        m_OrientationChangedCoroutine = StartCoroutine(CheckOrientationChanged());

        if (m_Canvases != null && m_Canvases.Length > 0)
        {
            m_CanvasOrientationChangedCoroutine = StartCoroutine(CheckCanvasOrientation());
        }
    }
    
    private void OnDestroy()
    {
        StopCoroutine(m_OrientationChangedCoroutine);

        if (m_Canvases != null && m_Canvases.Length > 0)
        {
            StopCoroutine(m_CanvasOrientationChangedCoroutine);
        }
    }
    
    private IEnumerator CheckOrientationChanged()
    {
        if (Screen.width > Screen.height)
        {
            ChangeOrientationLandscape();
            yield return new WaitUntil(() => Screen.width < Screen.height);
        }
        else
        {
            ChangeOrientationPortrait();
            yield return new WaitUntil(() => Screen.width > Screen.height);
        }
        
        StartCoroutine(CheckOrientationChanged());
    }

    private void ChangeOrientationLandscape()
    {
        foreach (var gameObjectData in m_GameObjects)
        {
            if (gameObjectData.m_Scale)
            {
                gameObjectData.m_GameObject.transform.localScale = new Vector3(gameObjectData.m_xScaleLandscape, gameObjectData.m_yScaleLandscape, gameObjectData.m_zScaleLandscape);
            }

            if (gameObjectData.m_Position)
            {
                gameObjectData.m_GameObject.transform.localPosition = new Vector3(gameObjectData.m_xPositionLandscape, gameObjectData.m_yPositionLandscape, gameObjectData.m_zPositionLandscape);
            }
        }
    }

    private void ChangeOrientationPortrait()
    {
        foreach (var gameObjectData in m_GameObjects)
        {
            if (gameObjectData.m_Scale)
            {
                gameObjectData.m_GameObject.transform.localScale = new Vector3(gameObjectData.m_xScalePortrait, gameObjectData.m_yScalePortrait, gameObjectData.m_zScalePortrait);
            }

            if (gameObjectData.m_Position)
            {
                gameObjectData.m_GameObject.transform.localPosition = new Vector3(gameObjectData.m_xPositionPortrait, gameObjectData.m_yPositionPortrait, gameObjectData.m_zPositionPortrait);
            }
        }
    }
    
    private IEnumerator CheckCanvasOrientation()
    {
        foreach (var canvasData in m_Canvases)
        {
            if (Screen.width > Screen.height)
            {
                canvasData.m_CanvasScalar.referenceResolution = new Vector2(canvasData.m_xCanvasSizeLandscape, canvasData.m_yCanvasSizeLandscape);
            }
            else
            {
                canvasData.m_CanvasScalar.referenceResolution = new Vector2(canvasData.m_xCanvasSizePortrait, canvasData.m_yCanvasSizePortrait);
            }
        }

        yield return new WaitForEndOfFrame();

        StartCoroutine(CheckCanvasOrientation());
    }
}
