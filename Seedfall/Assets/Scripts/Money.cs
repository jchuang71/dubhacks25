using UnityEngine;
using TMPro;

public class Money : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private float amount;

    public void AddAmount(float _amount)
    {
        amount += _amount;
        moneyText.text = "Money: " + amount;
    }
    
    public float GetAmount()
    {
        return amount;
    }
}
