using System;
using UnityEditor;
using UnityEngine;

[Serializable]
public struct SpinCompleteAction
{
    [Header("Activate a Prefab")]
    public GameObject m_PrefabObject;
    
    [Header("Run Animation")]
    public string m_AnimationName;
    
    [Header("New Slot Machine and Button Skins")]
    public Sprite m_TopSkin;
    public Sprite m_MainSkin;
    public Sprite m_ButtonSkin;
    
    [Header("Delay Between Current Action to Next One")]
    public float m_DelayUntilNextActionInSeconds;
}