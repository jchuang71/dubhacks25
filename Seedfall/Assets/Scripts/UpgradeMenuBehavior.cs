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

    //Upgrade buttons
    public Button fundraisingButton;
    public Button technologyButton;
    public Button legislationButton;

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
        money = GameManager.ManagerInstance.gameObject.GetComponent<Money>();
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

        upgradeText.text = "Increase passive income by " + (fundraisingLevel + 1) * 50;
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

        upgradeText.text = "Decrease pollution by " + (legislationLevel + 1) * 10 + "%";
    }

    public void ConfirmUpgrade()
    {
        if(fundraisingSelected)
        {
            if(fundraisingCost <= money.GetAmount())
            {
                //Apply upgrades
                //Adjust values
                fundraisingLevel++;
                money.PayAmount(fundraisingCost);
                fundraisingCost *= 2;
                //Check if the level is maxed out
                if (fundraisingLevel >= maxLevel)
                {
                    fundraisingButton.gameObject.SetActive(false);
                }
                upgradeText.text = "Upgrade purchased!";
                //Update the scrollbar size
                fundraisingScrollbar.size = (float)fundraisingLevel / maxLevel;

                // Set money passive income
                money.passiveIncomeInterval -= 2;
                money.passiveIncomeAmount = (fundraisingLevel * 50);
                money.EarnPassively();
            }
            else
            {
                upgradeText.text = "Not enough money!";
            }
        }
        else if (technologySelected)
        {
            if (technologyCost <= money.GetAmount())
            {
                //Apply upgrades
                //Adjust values
                technologyLevel++;
                money.PayAmount(technologyCost);
                technologyCost *= 2;
                //Check if the level is maxed out
                if (technologyLevel >= maxLevel)
                {
                    technologyButton.gameObject.SetActive(false);
                }
                upgradeText.text = "Upgrade purchased!";
                //Update the scrollbar size
                technologyScrollbar.size = (float)technologyLevel / maxLevel;

                GameManager.ManagerInstance.playerMoveSpeed *= 1.2f; 
            }
            else
            {
                upgradeText.text = "Not enough money!";
            }
        }
        else if (legislationSelected)
        {
            if (legislationCost <= money.GetAmount())
            {
                //Apply upgrades
                //Adjust values
                legislationLevel++;
                money.PayAmount(legislationCost);
                legislationCost *= 2;
                //Check if the level is maxed out
                if (legislationLevel >= maxLevel)
                {
                    legislationButton.gameObject.SetActive(false);
                }
                upgradeText.text = "Upgrade purchased!";
                //Update the scrollbar size
                legislationScrollbar.size = (float)legislationLevel / maxLevel;

                GameManager.ManagerInstance.pollutionInterval += (legislationLevel * 10);
            }
            else
            {
                upgradeText.text = "Not enough money!";
            }
        }

        fundraisingSelected = false;
        technologySelected = false;
        legislationSelected = false;
    }


}
