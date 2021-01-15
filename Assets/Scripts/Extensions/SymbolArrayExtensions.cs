using UnityEngine;

public static class SymbolArrayExtensions
{
    public static void OrderSymbols(this SymbolObject[] array, Sprite symbolOnStartup)
    {
        // Symbol order should be random
        array.Shuffle();
        
        // Set the symbol that will be displayed on the slot machine bar on startup (index = 1)
        array.Replace(1, array.IndexOf(symbolOnStartup));
    }
    
    public static void Shuffle(this SymbolObject[] array)
    {
        for (var i = 0; i < array.Length; i++) {
            var random = Random.Range(0, array.Length);
            var tempRegularImage = array[random].NormalSymbol.sprite;
            var tempBlurredImage = array[random].BlurredSymbol.sprite;
            array[random].NormalSymbol.sprite = array[i].NormalSymbol.sprite;
            array[random].BlurredSymbol.sprite = array[i].BlurredSymbol.sprite;
            array[i].NormalSymbol.sprite = tempRegularImage;
            array[i].BlurredSymbol.sprite = tempBlurredImage;
        }
    }
    
    public static void Replace(this SymbolObject[] array, int index1, int index2)
    {
        if (index1 == index2 || index1 < 0 || index2 < 0)
        {
            return;
        }

        var tempRegularImage = array[index1].NormalSymbol.sprite;
        var tempBlurredImage = array[index1].BlurredSymbol.sprite;
        
        array[index1].NormalSymbol.sprite = array[index2].NormalSymbol.sprite;
        array[index1].BlurredSymbol.sprite = array[index2].BlurredSymbol.sprite;
        
        array[index2].NormalSymbol.sprite = tempRegularImage;
        array[index2].BlurredSymbol.sprite = tempBlurredImage;
    }
    
    public static int IndexOf(this SymbolObject[] array, Sprite sprite)
    {
        for (var i = 0; i < array.Length; i++)
        {
            if (array[i].NormalSymbol.sprite == sprite)
            {
                return i;
            }
        }

        return -1;
    }
}
