using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SlotMachineManager : MonoBehaviour
{
    [Header("Slot Machine Symbols")]
    [SerializeField]
    private Sprite[] m_Symbols = null;

    [SerializeField]
    private Sprite[] m_BlurredSymbols = null;

    [Header("Initial Data")]
    [SerializeField]
    private int m_StartingSymbol1 = 0;

    [SerializeField]
    private int m_StartingSymbol2 = 0;
    
    [SerializeField]
    private int m_StartingSymbol3 = 0;
    
    [SerializeField] 
    private int m_InitialSpinAmount = 0;
    
    [SerializeField] 
    private int m_InitialCoinAmount = 0;
    
    [SerializeField] 
    private int m_EventCompletionValue = 0;
    
    [Header("Spin Properties")]
    [SerializeField]
    private SpinProperties[] m_SpinProperties = new SpinProperties[3];

    [Header("Audio")]
    [SerializeField] 
    private AudioConfiguration m_AudioConfiguration = new AudioConfiguration();

    [Header("Additional Elements")]
    [SerializeField] 
    private Button m_SpinButton = null;

    [SerializeField] 
    private Animator m_HandAnim = null;
    
    [SerializeField] 
    private RectTransform m_TopBorder = null;
    
    [SerializeField] 
    private ProgressBarManager m_SpinAmountBar = null, m_EventProgressBar = null;

    [SerializeField]
    private SlotBarManager m_SlotBar1 = null, m_SlotBar2 = null, m_SlotBar3 = null;
    
    [SerializeField] 
    private CoinBarManager m_CoinBarManager = null;

    [SerializeField]
    private Image m_SlotMachineMainSkin = null, m_SlotMachineTopSkin = null;

    private Animator m_SpinButtonAnimator;
    private Animator m_MainAnimator;
    
    private AudioSource m_SlotMachineAudioSource, m_SpinButtonAudioSource;

    private bool IsSpinButtonLocked;

    private int m_SpinNumber;

    private void Awake()
    {
        m_MainAnimator = GetComponent<Animator>();
        m_SlotMachineAudioSource = GetComponent<AudioSource>();
        m_SpinButtonAnimator = m_SpinButton.gameObject.GetComponent<Animator>();
        m_SpinButtonAudioSource = m_SpinButton.gameObject.GetComponent<AudioSource>();
        
        m_CoinBarManager.Init(m_InitialCoinAmount);
        m_SpinAmountBar.Init(m_InitialSpinAmount, m_InitialSpinAmount, Consts.SPIN_AMOUNT_BAR_ANIMATION_SPEED, false, false);
        m_EventProgressBar.Init(0, m_EventCompletionValue, Consts.EVENT_PROGRESS_BAR_ANIMATION_SPEED, true, true);

        AddListeners();
    }

    // Start is called before the first frame update
    void Start()
    {
        InitSlotMachineBars();
    }

    private void InitSlotMachineBars()
    {
        var resultSymbolIndexesBar1 = m_SpinProperties.Select(spinProperties => spinProperties.m_ResultSymbol1).ToArray();
        var resultSymbolSpritesBar1 = resultSymbolIndexesBar1.Select(index => m_Symbols[index]).ToArray();
        
        var resultSymbolIndexesBar2 = m_SpinProperties.Select(spinProperties => spinProperties.m_ResultSymbol2).ToArray();
        var resultSymbolSpritesBar2 = resultSymbolIndexesBar2.Select(index => m_Symbols[index]).ToArray();
        
        var resultSymbolIndexesBar3 = m_SpinProperties.Select(spinProperties => spinProperties.m_ResultSymbol3).ToArray();
        var resultSymbolSpritesBar3 = resultSymbolIndexesBar3.Select(index => m_Symbols[index]).ToArray();

        // The starting point of the symbol animation, on the slot machine - y axis (top to bottom)
        var symbolAnimationStartPositionY = m_TopBorder.localPosition.y;
        
        m_SlotBar1.Init(0, m_Symbols[m_StartingSymbol1], m_Symbols, m_BlurredSymbols, symbolAnimationStartPositionY, resultSymbolSpritesBar1);
        m_SlotBar2.Init(1, m_Symbols[m_StartingSymbol2], m_Symbols, m_BlurredSymbols, symbolAnimationStartPositionY, resultSymbolSpritesBar2);
        m_SlotBar3.Init(2, m_Symbols[m_StartingSymbol3], m_Symbols, m_BlurredSymbols, symbolAnimationStartPositionY, resultSymbolSpritesBar3);
    }

    private void AddListeners()
    {
        m_SpinButton.onClick.AddListener(OnSpinButtonClicked);
        SlotBarManager.OnSpinComplete += OnSpinComplete;
    }
    
    private void OnSpinButtonClicked()
    {
        if (!IsSpinButtonLocked && m_SpinNumber < m_SpinProperties.Length)
        {
            IsSpinButtonLocked = true;
                
            if (m_HandAnim.gameObject.activeSelf)
            {
                m_HandAnim.gameObject.SetActive(false);
            }

            m_SpinButtonAnimator.Play(Consts.SPIN_BUTTON_CLICKED);
            m_SpinButtonAudioSource.PlayOneShot(m_AudioConfiguration.m_SpinButtonClick);
            
            StartCoroutine(StartSpin());
        }
        else
        {
            m_SpinButtonAudioSource.PlayOneShot(m_AudioConfiguration.m_SpinButtonClick);
            m_SpinButtonAnimator.Play(Consts.SPIN_BUTTON_DISABLED);
        }
    }

    private void OnSpinComplete()
    {
        m_SlotMachineAudioSource.Stop();
        m_EventProgressBar.CurrentAmount = m_SpinProperties[m_SpinNumber].m_EventProgressPoints;
        m_CoinBarManager.Amount = m_SpinProperties[m_SpinNumber].m_CoinAmount;
        
        StartCoroutine(ExecutePostSpinActions());
    }

    private IEnumerator ExecutePostSpinActions()
    {
        var spinCompleteActions = m_SpinProperties[m_SpinNumber].m_SpinCompleteActions;

        if (spinCompleteActions != null && spinCompleteActions.Length > 0)
        {
            for (int i = 0; i < spinCompleteActions.Length; i++)
            {
                if (spinCompleteActions[i].m_MainSkin != null || spinCompleteActions[i].m_TopSkin != null || spinCompleteActions[i].m_ButtonSkin != null)
                {
                    ChangeSlotMachineSkin(spinCompleteActions[i].m_MainSkin, spinCompleteActions[i].m_TopSkin, spinCompleteActions[i].m_ButtonSkin);
                }
                
                if (spinCompleteActions[i].m_PrefabObject != null)
                { 
                    var go = Instantiate(spinCompleteActions[i].m_PrefabObject);
           //       Instantiate(spinCompleteActions[i].m_PrefabObject, transform.position, transform.rotation, gameObject.transform, Quaternion.identity);
           //         go.transform.SetParent(gameObject.transform);
                }
                
                if (!string.IsNullOrEmpty(spinCompleteActions[i].m_AnimationName))
                {
                    var animationName = spinCompleteActions[i].m_AnimationName;
                    
                    m_MainAnimator.Play(animationName);

                    if (i == spinCompleteActions.Length - 1)
                    {
                        yield return m_MainAnimator.WaitAnimationEnd(animationName);
                    }
                }

                yield return new WaitForSeconds(spinCompleteActions[i].m_DelayUntilNextActionInSeconds);
            }
        }
        
        // Increase the spin number only after the spin process ends
        m_SpinNumber++;

        if (m_SpinNumber < m_SpinProperties.Length)
        {
            IsSpinButtonLocked = false;
        }
    }

    private IEnumerator StartSpin()
    {
        m_SlotMachineAudioSource.Play(m_AudioConfiguration.m_SlotSpin);
        
        m_SlotBar1.StartSpin(m_SpinNumber, m_SpinProperties[m_SpinNumber].m_Speed, m_SpinProperties[m_SpinNumber].m_LoopTimes, m_AudioConfiguration.m_SlotStop);
        yield return new WaitForSeconds(Consts.SLOT_BAR_SPINNING_DELAY);
        m_SlotBar2.StartSpin(m_SpinNumber, m_SpinProperties[m_SpinNumber].m_Speed, m_SpinProperties[m_SpinNumber].m_LoopTimes, m_AudioConfiguration.m_SlotStop);
        yield return new WaitForSeconds(Consts.SLOT_BAR_SPINNING_DELAY);
        m_SlotBar3.StartSpin(m_SpinNumber, m_SpinProperties[m_SpinNumber].m_Speed, m_SpinProperties[m_SpinNumber].m_LoopTimes, m_AudioConfiguration.m_SlotStop);

        var remainingSpinAmount = m_InitialSpinAmount - (m_SpinNumber + 1);

        if (remainingSpinAmount > 0)
        {
            m_SpinAmountBar.CurrentAmount = remainingSpinAmount;
        }
    }

    private void ChangeSlotMachineSkin(Sprite mainSlotMachineSkin, Sprite topSlotMachineSkin, Sprite buttonSkin)
    {
        if (mainSlotMachineSkin != null)
        {
            m_SlotMachineMainSkin.sprite = mainSlotMachineSkin;
            
            if (topSlotMachineSkin != null)
            {
                m_SlotMachineTopSkin.enabled = true;
                m_SlotMachineTopSkin.sprite = topSlotMachineSkin;
            }
            else
            {
                m_SlotMachineTopSkin.enabled = false;
            }
        }
        
        if (buttonSkin != null)
        {
            m_SpinButton.GetComponent<Image>().sprite = buttonSkin;
        }
    }
}
