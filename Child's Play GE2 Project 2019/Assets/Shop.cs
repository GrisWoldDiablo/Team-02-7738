﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum ButtonType
{
    Buy,
    Sell,
    Upgrade
}

public class Shop : MonoBehaviour
{
    #region Singleton
    private static Shop instance = null;

    public static Shop GetInstance()
    {
        if (instance == null)
        {
            instance = GameObject.FindObjectOfType<Shop>();
        }
        return instance;
    }
    #endregion

    [SerializeField] private Text toolTipText;
    [SerializeField] private GameObject[] panels;
    [SerializeField] private Text priceT;
    [SerializeField] private Text priceB;
    private int placeholder = 0;
    private int shopPanel = 1;
    private int upgradeSellPanel = 2;
    private int barrierPanel = 3;

    public GameObject[] Panels { get => panels; set => panels = value; }
    public int Placeholder { get => placeholder; set => placeholder = value; }
    public int ShopPanel { get => shopPanel; set => shopPanel = value; }
    public int UpgradeSellPanel { get => upgradeSellPanel; set => upgradeSellPanel = value; }
    public int BarrierPanel { get => barrierPanel; set => barrierPanel = value; }

    void Start()
    {
        toolTipText.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetPanelActive(int panelIndex)
    {
        Input.ResetInputAxes();
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(panelIndex == i);
        }
    }

    public void MoveToClick(int index)
    {
        var pos = Input.mousePosition;
        //posVec.z = obj.transform.position.z - Camera.main.transform.position.z;
        //posVec = Camera.main.ScreenToWorldPoint(posVec);
        Panels[index].transform.position = new Vector3(pos.x, pos.y, pos.z);
    }

    public void SetActiveToolTip(bool value)
    {
        if (value)
        {
            if (!toolTipText.IsActive())
            {
                toolTipText.gameObject.SetActive(true);
                return;
            }
            return;
        }
        else
        {
            if (toolTipText.IsActive())
            {
                toolTipText.gameObject.SetActive(false);
                return;
            }
            return;
        }
    }

    public void TowerSelect(int index)
    {
        GameManager.GetInstance().SetTowerSelectionIndex(index);
        GameManager.GetInstance().StoreButtonPressed();
    }

    public void SetToolTipText(ButtonType button)
    {
        if (button == ButtonType.Buy)
        {
            toolTipText.text = "BUY";
            return;
        }
        else if (button == ButtonType.Upgrade)
        {
            toolTipText.text = "UPGRADE";
            return;
        }
        toolTipText.text = "SELL";
        return;
    }

    public void MoveToolTip(Vector3 t)
    {
        toolTipText.transform.position = new Vector3(t.x, t.y + 80, t.z);
        //Debug.Log("AAAAAAAAAAAAAAAAAAAAAAAAAAAA");
    }

    public void ChangePrice(string p)
    {
        if (priceT.IsActive())
        {
            priceT.text = $"Price\n{p}";
        }
        else priceB.text = $"Price\n{p}";
    }
}