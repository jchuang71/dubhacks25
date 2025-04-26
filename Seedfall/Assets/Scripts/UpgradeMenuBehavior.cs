using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeMenuBehavior : MonoBehaviour
{
    //References to GameObjects
    public TMP_Text upgradeText;
    public GameObject menuBackground;
    public GameObject upgradeMenu;

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


    // Maximum level for upgrades
    public int maxLevel = 5;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleMenu()
    {
        //This will enable and disable the menu.
        menuBackground.SetActive(!menuBackground.activeSelf);
        upgradeMenu.SetActive(!upgradeMenu.activeSelf);


    }

    void SelectFundraising()
    {

    }

    void SelectTechnology()
    {

    }

    void SelectLegislation()
    {

    }

    void ConfirmUpgrade()
    {

    }


}
