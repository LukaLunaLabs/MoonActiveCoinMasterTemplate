using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class SlotBarManager : MonoBehaviour
{
    [SerializeField] 
    private Image m_SymbolPrefab = null;
    
    [SerializeField] 
    private GameObject m_NormalSymbolList = null, m_BlurredSymbolList = null;

    private AudioSource m_AudioSource;

    private AudioClip m_SpinEnd;

    // The bar that should display the next result
    // This value is used to maintain the result order (left bar stops first, then the middle one, and at the end the right one)
    private static int m_CurrentActiveBarIndex;

    public static Action OnSpinComplete { get; set; }

    private bool m_IsSpinning;
    
    private float m_SpinSpeed;
    private int m_BarIndex;
    private int m_LoopTimes;
    private int m_NextVisibleSymbol;
    private int m_SpeedDecreaseRate;
    
    // The result symbol of each spin
    private int[] m_ResultSymbols;
    
    // The indexes of the 3 symbols that are currently displayed on the slot machine
    private int[] m_CurrentVisibleSymbols;
    
    private int m_CurrentSpinNumber;
    private Vector3 m_PendingSymbolPosition;
    private float m_SymbolStartingPositionDelta;
    
    private int m_CurrentLoop;
    
    // This value is used to count the number of running symbols in each loop (indicates when the loop has ended)
    private int m_SymbolCounterPerRound;
    
    private bool m_IsEndingAnimation, m_IsEndingAnimationFirstPart;

    private SymbolObject[] m_SymbolObjects;

    private Vector3 m_SymbolAnimationStartPosition;
    private Vector3[] m_InitialPositions;

    // Bars should stop one after one, with a short delay between them
    private WaitForSeconds m_StopAnimationDelay;
    
    private void Awake()
    {
        m_AudioSource = GetComponent<AudioSource>();
        m_StopAnimationDelay = new WaitForSeconds(0.2f);
    }

    public void Init(int barIndex, Sprite symbolOnStartup, Sprite[] normalSymbols, Sprite[] blurredSymbols,
                     float symbolAnimationStartPositionY, Sprite[] resultSymbols)
    {
        m_BarIndex = barIndex;

        var pendingSymbolPositionX = transform.position.x;
        
        m_PendingSymbolPosition = new Vector3(pendingSymbolPositionX + 1000, symbolAnimationStartPositionY);
        
        // The symbol starts moving from that position (top) to the bottom
        m_SymbolAnimationStartPosition = new Vector3(pendingSymbolPositionX, symbolAnimationStartPositionY);
        
        // The 3 symbols that are currently displayed on the slot machine
        m_CurrentVisibleSymbols = new int[4];
        
        InstantiateSymbols(normalSymbols, blurredSymbols);
        
        // Order the symbols randomly and set the 'symbolOnStartup' to the main one
        m_SymbolObjects.OrderSymbols(symbolOnStartup);

        // Set the symbols that will be displayed at the end of each spin
        SetResultSymbols(resultSymbols);
        
        // Set the initial symbol positions, for accuracy purposes.
        // When the spin ends, the visible symbols will be set to the initial positions
        SetInitialSymbolPositions();

        m_SymbolStartingPositionDelta = m_SymbolAnimationStartPosition.y - m_InitialPositions[0].y;
    }

    public void StartSpin(int spinNumber, float speed, int loopTimes, AudioClip spinEnd)
    {
        if (!m_IsSpinning)
        {
            m_CurrentActiveBarIndex = 0;
            m_SymbolCounterPerRound = 0;
            m_SpeedDecreaseRate = -1;
            m_CurrentSpinNumber = spinNumber;
            m_SpinSpeed = speed;
            m_LoopTimes = loopTimes;
            m_SpinEnd = spinEnd;
            m_IsEndingAnimation = false;
            m_IsSpinning = true;
        }
    }

    private void InstantiateSymbols(Sprite[] normalSymbols, Sprite[] blurredSymbols)
    {
        var symbolYPosition = 0f;
        
        m_SymbolObjects = new SymbolObject[normalSymbols.Length];

        for (var i = 0; i < m_SymbolObjects.Length; i++)
        {
            InstantiateSymbol(i, i < 3 ? new Vector3(0, symbolYPosition) : m_PendingSymbolPosition, normalSymbols[i], blurredSymbols[i]);
            
            symbolYPosition -= Consts.SYMBOL_DISTANCE_Y;

            if (i < 4)
            {
                m_CurrentVisibleSymbols[i] = i;
            }
        }

        m_NextVisibleSymbol = 3;
    }
    
    private void Update()
    {
        if (m_IsEndingAnimation)
        {
            PlayEndingAnimation();
            return;
        }
        
        if (m_IsSpinning)
        {
            IncreaseLoopNumberIfReachedEndOfRound();
            
            if (m_CurrentActiveBarIndex == m_BarIndex && m_CurrentLoop >= m_LoopTimes)
            {
                // Stop the current bar
                OnLastLoopReached();
            }

            if (m_SpinSpeed >= Consts.MINIMUM_BLUR_VALUE)
            {
                SetBlurSymbols(true);
            }
            else
            {
                SetBlurSymbols(false);
            }

            if (!m_IsSpinning)
            {
                return;
            }

            // The top symbol starts a bit below the original m_SymbolAnimationStartPosition.y position
            // We calculate the delta using m_SymbolStartingPositionDelta, to know when should we start animating the fourth symbol and above
          //  Debug.LogError($"m_SymbolObjects[m_CurrentVisibleSymbols[0]].Position.y: {m_SymbolObjects[m_CurrentVisibleSymbols[0]].Position.y}, m_InitialPositions[1].y + m_SymbolStartingPositionDelta: {m_InitialPositions[1].y + m_SymbolStartingPositionDelta}");
            if (m_SymbolObjects[m_CurrentVisibleSymbols[0]].Position.y <= m_InitialPositions[1].y + m_SymbolStartingPositionDelta)
            {
                // When the first symbol reaches the position of the second one, activate the next symbol in the list
                SetNextSymbol();
                
                if (m_CurrentActiveBarIndex == 2 && m_CurrentLoop >= m_LoopTimes)
                {
                    DecreaseSpinSpeed();
                }
            }

            MoveSymbols();
        }
    }

    private void DecreaseSpinSpeed()
    {
        // Decrease the speed before the spin ends
        if (m_SpeedDecreaseRate == -1)
        {
            m_SpeedDecreaseRate = (int)m_SpinSpeed / (m_ResultSymbols[m_CurrentSpinNumber] + 1);
        }

        var newSpeed = (int) m_SpinSpeed - (int) (m_SpeedDecreaseRate * 0.2f);
            
        if (newSpeed > 150)
        {
            m_SpinSpeed = newSpeed;
        }
    }
    
    private void PlayEndingAnimation()
    {
        // This is the symbol "jump" animation when the spin ends
        var directionVector = m_IsEndingAnimationFirstPart ? Vector3.up : Vector3.down;
        var speed = 5f;
        var jumpingHeight = 20f;
        
        for (int i = 0; i < 3; i++)
        {
            m_SymbolObjects[m_CurrentVisibleSymbols[i]].Move(directionVector * (speed * Time.deltaTime));
        }

        if (m_IsEndingAnimationFirstPart)
        {
            if (m_SymbolObjects[m_CurrentVisibleSymbols[1]].Position.y > m_InitialPositions[1].y + jumpingHeight)
            {
                m_IsEndingAnimationFirstPart = false;
            }
        }
        else
        {
            if (m_SymbolObjects[m_CurrentVisibleSymbols[1]].Position.y <= m_InitialPositions[1].y)
            {
                SetSymbolsToInitialPositions();
                m_IsEndingAnimation = false;
            }
        }
    }

    private void MoveSymbols()
    {
        for (int i = 0; i < m_SymbolObjects.Length; i++)
        {
            if (m_SymbolObjects[i].Position != m_PendingSymbolPosition)
            {
                m_SymbolObjects[i].Move(Vector3.down * (m_SpinSpeed * Time.deltaTime));
            }
        }
    }
    
    private void IncreaseLoopNumberIfReachedEndOfRound()
    {
        if (m_SymbolCounterPerRound == m_SymbolObjects.Length - 1)
        {
            m_CurrentLoop++;
            m_SymbolCounterPerRound = 0;
        }
    }

    private void InstantiateSymbol(int index, Vector3 position, Sprite normalSprite, Sprite blurredSprite)
    {
        m_SymbolObjects[index] = new SymbolObject
        {
            NormalSymbol = Instantiate(m_SymbolPrefab),
            BlurredSymbol = Instantiate(m_SymbolPrefab)
        };

        m_SymbolObjects[index].NormalSymbol.rectTransform.localPosition = position;
        m_SymbolObjects[index].BlurredSymbol.rectTransform.localPosition = position;
        
        m_SymbolObjects[index].NormalSymbol.rectTransform.SetParent(m_NormalSymbolList.transform, false);
        m_SymbolObjects[index].BlurredSymbol.rectTransform.SetParent(m_BlurredSymbolList.transform, false);
        
        m_SymbolObjects[index].NormalSymbol.sprite = normalSprite;
        m_SymbolObjects[index].BlurredSymbol.sprite = blurredSprite;
    }

    private void SetSymbolsToInitialPositions()
    {
        for (int i = 0; i < 3; i++)
        {
            m_SymbolObjects[m_CurrentVisibleSymbols[i]].Position = m_InitialPositions[i];
        }
    }

    private void SetInitialSymbolPositions()
    {
        m_InitialPositions = new Vector3[3];

        for (int i = 0; i < m_InitialPositions.Length; i++)
        {
            m_InitialPositions[i] = m_SymbolObjects[i].Position;
        }
    }

    private void SetResultSymbols(Sprite[] resultSymbols)
    {
        m_ResultSymbols = new int[resultSymbols.Length];
        
        for (int i = 0; i < resultSymbols.Length; i++)
        {
            m_ResultSymbols[i] = m_SymbolObjects.IndexOf(resultSymbols[i]);
        }
    }

    private void SetNextSymbol()
    {
        m_SymbolObjects[m_CurrentVisibleSymbols[3]].Position = m_PendingSymbolPosition;

        var nextSymbolToActivate = (m_NextVisibleSymbol + 1) % m_SymbolObjects.Length;
        m_NextVisibleSymbol++;
        m_SymbolCounterPerRound++;

        m_CurrentVisibleSymbols[3] = m_CurrentVisibleSymbols[2];
        m_CurrentVisibleSymbols[2] = m_CurrentVisibleSymbols[1];
        m_CurrentVisibleSymbols[1] = m_CurrentVisibleSymbols[0];
        
        m_CurrentVisibleSymbols[0] = nextSymbolToActivate;

        m_SymbolObjects[m_CurrentVisibleSymbols[0]].Position = m_SymbolAnimationStartPosition;
    }

    private void SetBlurSymbols(bool isBlur)
    {
        if (m_NormalSymbolList.activeSelf == isBlur)
        {
            m_NormalSymbolList.SetActive(!isBlur);
        }

        if (m_BlurredSymbolList.activeSelf != isBlur)
        {
            m_BlurredSymbolList.SetActive(isBlur);
        }
    }
    
    private void OnLastLoopReached()
    {
        // Check if the symbol is equal to the result symbol
        // If yes - Stop the spinning animation
        // If no - Let the animation run until reaching the result symbol

        var currentMainVisibleSymbol = m_CurrentVisibleSymbols[1];
        
        if (currentMainVisibleSymbol == m_ResultSymbols[m_CurrentSpinNumber] && 
            m_SymbolObjects[currentMainVisibleSymbol].Position.y <= m_InitialPositions[1].y)
        {
            OnSpinEnd();
        }
    }

    private IEnumerator SetNextActiveBar()
    {
        yield return m_StopAnimationDelay;

        if (m_BarIndex == m_CurrentActiveBarIndex)
        {
            m_CurrentActiveBarIndex++;
        }
    }

    private void OnSpinEnd()
    {
        m_CurrentLoop = 0;
        m_IsSpinning = false;

        if (m_SpinSpeed >= 20)
        {
            m_IsEndingAnimationFirstPart = true;
            m_IsEndingAnimation = true;
        }
        
        m_AudioSource.PlayOneShot(m_SpinEnd);
        m_SpinSpeed = 0;
        
        // Set the symbols to their initial positions when the spin ends
        SetSymbolsToInitialPositions();
        
        if (m_BarIndex == 2)
        {
            OnSpinComplete?.Invoke();    
        }
        else
        {
            StartCoroutine(SetNextActiveBar());
        }
    }
}
