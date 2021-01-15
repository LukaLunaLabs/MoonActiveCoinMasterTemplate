using UnityEngine;
using UnityEngine.UI;

public class SymbolObject
{
    public Image NormalSymbol { get; set; }
    public Image BlurredSymbol { get; set; }

    public Vector3 Position
    {
        get => NormalSymbol.rectTransform.localPosition;

        set
        {
            NormalSymbol.rectTransform.localPosition = value;
            BlurredSymbol.rectTransform.localPosition = value;
        }
    }

    public void Move(Vector3 translation)
    {
        NormalSymbol.rectTransform.Translate(translation);
        BlurredSymbol.rectTransform.Translate(translation);
    }
}
