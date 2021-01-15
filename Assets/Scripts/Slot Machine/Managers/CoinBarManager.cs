using UnityEngine;
using UnityEngine.UI;

public class CoinBarManager : MonoBehaviour
{
    [SerializeField]
    private Text m_CoinAmountText = null;

    private long m_CoinAmount;
    
    // The amount aggregation speed
    private long m_AmountPerRound;
    
    public long Amount { get; set; }

    private long CoinAmountText
    {
        set => m_CoinAmountText.text = value.ToString("#,##0");
    }

    private bool m_IsAmountIncreasing;

    public void Init(long initialCoinAmount)
    {
        m_CoinAmount = initialCoinAmount;
        Amount = initialCoinAmount;
        CoinAmountText = m_CoinAmount;
    }

    public void AddCoins(long amount)
    {
        Amount = m_CoinAmount + amount;
    }

    private void Update()
    {
        if (Amount > m_CoinAmount)
        {
            if (!m_IsAmountIncreasing)
            {
                m_AmountPerRound = (Amount - m_CoinAmount) / 100;

                if (m_AmountPerRound < 1)
                {
                    m_AmountPerRound = 1;
                }
                
                m_IsAmountIncreasing = true;
            }
            
            m_CoinAmount += m_AmountPerRound;

            if (m_CoinAmount > Amount)
            {
                m_CoinAmount = Amount;
            }
            
            CoinAmountText = m_CoinAmount;
        }
        else
        {
            if (m_IsAmountIncreasing)
            {
                m_IsAmountIncreasing = false;
            }
        }
    }
}
