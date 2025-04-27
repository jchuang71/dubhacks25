using UnityEngine;
using TMPro;

public class Money : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private float amount;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddAmount(float _amount)
    {
        amount += _amount;
        moneyText.text = "Money: " + (int) amount;
    }

    public float GetAmount()
    {
        return amount;
    }

    public bool PayAmount(float _amount)
    {
        if (amount >= _amount)
        {
            amount -= _amount;
            moneyText.text = "Money: " + amount;
            return true;
        }
        else
        {
            return false;
        }
    }
}
