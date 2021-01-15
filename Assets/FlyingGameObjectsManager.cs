using System;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;
using Vector3 = UnityEngine.Vector3;

[Serializable]
public enum FlyingGameObjectsDirection
{
    Up,
    Down
}

public class FlyingGameObjectsManager : MonoBehaviour
{
    [SerializeField]
    private Sprite[] m_ObjectSprites = null;

    [SerializeField]
    private int m_NumberOfObjects = 0;
    
    [SerializeField]
    private float m_Speed = 0;

    [SerializeField]
    private FlyingGameObjectsDirection m_FlyingGameObjectsDirection = FlyingGameObjectsDirection.Down;
    
    [SerializeField]
    private Image m_FlyingGameObjectPrefab = null;
    
    private Image[] m_FlyingGameObjects;
    
    // Start is called before the first frame update
    void Start()
    {
        m_FlyingGameObjects = new Image[m_NumberOfObjects];
        
        for (var i = 0; i < m_NumberOfObjects; i++)
        {
            m_FlyingGameObjects[i] = Instantiate(m_FlyingGameObjectPrefab);
            
            if (m_FlyingGameObjectsDirection == FlyingGameObjectsDirection.Up)
            {
                m_FlyingGameObjects[i].rectTransform.localPosition = new Vector3(UnityEngine.Random.Range(-Screen.width / 2, Screen.width), -Screen.height / 2, 0);
                m_FlyingGameObjects[i].rectTransform.SetParent(transform, false);
            }

            m_FlyingGameObjects[i].sprite = m_ObjectSprites[i % m_ObjectSprites.Length];
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (m_FlyingGameObjectsDirection == FlyingGameObjectsDirection.Up)
        {
            foreach (var flyingObject in m_FlyingGameObjects)
            {
                flyingObject.transform.Translate(Vector3.up * m_Speed * Time.deltaTime);
            }
        }
    }
}
