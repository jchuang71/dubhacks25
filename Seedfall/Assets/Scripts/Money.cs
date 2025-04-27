using UnityEngine;
using TMPro;
using System.Collections;

public class Money : MonoBehaviour
{
    public float passiveIncomeInterval;
    public float passiveIncomeAmount;

    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private float amount;

    private bool isEarningPassively;

    void Start()
    {
        isEarningPassively = false;
        passiveIncomeInterval = 20f;
        passiveIncomeAmount = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddAmount(float _amount)
    {
        amount += Mathf.Max(0, _amount);
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
            moneyText.text = "Money: " + (int) amount;
            return true;
        }
        else
        {
            return false;
        }
    }

    public void EarnPassively()
    {
        if (!isEarningPassively)
            StartCoroutine(PassiveMoney());
    }

    IEnumerator PassiveMoney()
    {
        isEarningPassively = true;
        while(isEarningPassively)
        {
            yield return new WaitForSeconds(passiveIncomeInterval);
            amount += passiveIncomeAmount;
            Debug.Log("Passively Earned: $" + passiveIncomeAmount);
        }
    }
}
