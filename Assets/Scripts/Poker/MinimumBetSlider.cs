using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class MinimumBetSlider : MonoBehaviour
{
    public Slider minimumBetSlider;
    public TextMeshProUGUI minimumBetText;
    int minimumBet = 2;
    public void SetMinimumBetBySlider()
    {
        minimumBet = (int)(95 * minimumBetSlider.value) + 5;
        Dealer.MinimumBet = minimumBet;
        minimumBetText.text = minimumBet + "$";
    }
}
