using System.Collections;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeMenuBehavior : MonoBehaviour
{
    //References to GameObjects
    public TMP_Text upgradeText;
    public GameObject menuBackground;
    public GameObject upgradeMenu;
    public Button confirmButton;
    public Money money;

    //Levels of upgrades
    public int fundraisingLevel;
    public int technologyLevel;
    public int legislationLevel;

    //Costs of upgrades
    public int fundraisingCost;
    public int technologyCost;
    public int legislationCost;

    //Upgrade cost text
    public TMP_Text fundraisingCostText;
    public TMP_Text technologyCostText;
    public TMP_Text legislationCostText;

    //Upgrade scrollbars
    public Scrollbar fundraisingScrollbar;
    public Scrollbar technologyScrollbar;
    public Scrollbar legislationScrollbar;

    //Selected upgrade
    public bool fundraisingSelected;
    public bool technologySelected;
    public bool legislationSelected;


    // Maximum level for upgrades
    public int maxLevel = 5;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        fundraisingCostText.text = '$' + fundraisingCost.ToString();
        technologyCostText.text = '$' + technologyCost.ToString();
        legislationCostText.text = '$' + legislationCost.ToString();
        //Enable confirm button if any upgrade is selected
        confirmButton.gameObject.SetActive(fundraisingSelected || technologySelected || legislationSelected);
    }

    public void ToggleMenu()
    {
        fundraisingSelected = false;
        technologySelected = false;
        legislationSelected = false;
        upgradeText.text = "Select an upgrade path to see upgrade details.";
        //This will enable and disable the menu.
        menuBackground.SetActive(!menuBackground.activeSelf);
        upgradeMenu.SetActive(!upgradeMenu.activeSelf);
    }

    public void SelectFundraising()
    {
        fundraisingSelected = true;
        technologySelected = false;
        legislationSelected = false;

        //Update the upgrade text to show the selected upgrade

        upgradeText.text = "Increase money generation by " + (fundraisingLevel + 1) * 20 + "%";
    }

    public void SelectTechnology()
    {
        fundraisingSelected = false;
        technologySelected = true;
        legislationSelected = false;

        //Update the upgrade text to show the selected upgrade

        upgradeText.text = "Increase movement speed by " + (technologyLevel + 1) * 20 + "%";
    }

    public void SelectLegislation()
    {
        fundraisingSelected = false;
        technologySelected = false;
        legislationSelected = true;

        //Update the upgrade text to show the selected upgrade

        upgradeText.text = "Decrease pollution by " + (legislationLevel + 1) * 20 + "%";
    }

    public void ConfirmUpgrade()
    {
        if(fundraisingSelected)
        {
            if(fundraisingCost <= money.GetAmount())
            {
                fundraisingLevel++;
                money.AddAmount(-fundraisingCost);
                fundraisingCost += 10;
                if (fundraisingLevel >= maxLevel)
                {
                    fundraisingLevel = maxLevel;
                    fundraisingCost = 0;
                }
            }
        }
        fundraisingSelected = false;
        technologySelected = false;
        legislationSelected = false;
    }


}
