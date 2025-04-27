using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu]
public class VegetationState : ScriptableObject
{
    public string stateName;
    public Sprite stateSprite;
    public float moneyCostToUpgrade;
    public float moneyAmountEffect;
    public float affectMoneyInterval;
}
