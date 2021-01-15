using UnityEngine;
using UnityEngine.UI;

public class ProgressBarManager : MonoBehaviour
{
    [SerializeField]
    private Text m_ProgressText = null;
    
    [SerializeField]
    private Image m_ProgressBar = null;
    
    private int m_TotalAmount;
    private int m_CurrentAmount;
    private float m_CurrentProgress, m_NewProgress;
    private float m_ProgressAnimationSpeed;
    private bool m_IsIncreasing, m_IsRunningNumberAnimation;

    private RectTransform m_ProgressBarTransform;

    public void Init(int startingAmount, int totalAmount, float progressAnimationSpeed, bool isIncreasing, bool isRunningNumberAnimation)
    {
        m_TotalAmount = totalAmount;
        m_ProgressAnimationSpeed = progressAnimationSpeed;
        m_IsIncreasing = isIncreasing;
        m_IsRunningNumberAnimation = isRunningNumberAnimation;
        m_ProgressBarTransform = m_ProgressBar.rectTransform;
        
        // Set initial progress
        CurrentAmount = startingAmount;
    }

    public int CurrentAmount
    {
        set
        {
            if (m_CurrentAmount == value)
            {
                return;
            }
            
            m_CurrentAmount = value;

            if (!m_IsRunningNumberAnimation)
            {
                m_ProgressText.text = $"{m_CurrentAmount} / {m_TotalAmount}";
            }

            if (m_CurrentAmount == m_TotalAmount)
            {
                m_NewProgress = 1;
            } else if (m_CurrentAmount <= 0)
            {
                m_NewProgress = 0;
            }
            else
            {
                m_NewProgress = (float)m_CurrentAmount / m_TotalAmount;   
            }
        }
    }

    private void Update()
    {
        if ((m_IsIncreasing && m_CurrentProgress < m_NewProgress) || (!m_IsIncreasing && m_CurrentProgress > m_NewProgress))
        {
            UpdateProgress();
        } else if ((m_IsIncreasing && m_CurrentProgress >= m_NewProgress) || (!m_IsIncreasing && m_CurrentProgress <= m_NewProgress))
        {
            SetLastProgressValue();
        }
    }

    private void UpdateProgress()
    {
        var animationSpeed = Time.deltaTime * m_ProgressAnimationSpeed;
        m_CurrentProgress += m_IsIncreasing ? animationSpeed : -animationSpeed;
        
        if (m_IsRunningNumberAnimation)
        {
            var currentValue = Mathf.Floor(m_CurrentProgress * m_TotalAmount);

            if ((m_IsIncreasing && currentValue < m_CurrentAmount) || (!m_IsIncreasing && m_CurrentAmount < currentValue) )
            {
                SetProgressText();
            }
        }

        if ((m_IsIncreasing && m_CurrentProgress > m_NewProgress) || (!m_IsIncreasing && m_CurrentProgress < m_NewProgress))
        {
            m_CurrentProgress = m_NewProgress;
        }
        
        m_ProgressBarTransform.localScale = new Vector3(m_CurrentProgress, m_ProgressBarTransform.localScale.y);
    }

    private void SetLastProgressValue()
    {
        // This function is used to set the initial progress value, or to prevent from the progress value to exceed the destination value
        
        m_CurrentProgress = m_NewProgress;
                
        if (m_IsRunningNumberAnimation)
        {
            SetProgressText();
        }
        
        m_ProgressBarTransform.localScale = new Vector3(m_CurrentProgress, m_ProgressBarTransform.localScale.y);
    }

    private void SetProgressText()
    {
        m_ProgressText.text = $"{m_CurrentAmount} / {m_TotalAmount}";
    }
}
