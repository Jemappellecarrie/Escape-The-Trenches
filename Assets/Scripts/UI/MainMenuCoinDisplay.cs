using UnityEngine;
using UnityEngine.UI;
using EscapeTheTrenches.Data;

public class MainMenuCoinDisplay : MonoBehaviour
{
    public Text totalCoinText;

    private void OnEnable()
    {
        UpdateCoinDisplay();
    }

    public void UpdateCoinDisplay()
    {
        GameData data = SaveSystem.LoadData();
        if (totalCoinText != null)
        {
            totalCoinText.text = "Total Coins: " + data.currency;
        }
    }
}
