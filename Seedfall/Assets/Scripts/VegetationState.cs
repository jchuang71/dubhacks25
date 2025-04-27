using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu]
public class VegetationState : ScriptableObject
{
    public string stateName;
    public List<Sprite> stateSprites;
    public Sprite stateSprite;
    public float moneyCostToUpgrade;
    public float moneyAmountEffect;
    public float affectMoneyInterval;
}
