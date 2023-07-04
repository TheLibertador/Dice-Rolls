using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    private int rollOneInputValue = 1;
    private int rollTwoInputValue = 1;
    

    public void GetInputOne(string inputOne)
    {
        rollOneInputValue = Int32.Parse(inputOne);
    }
    public void GetInputTwo(string inputTwo)
    {
        rollTwoInputValue = Int32.Parse(inputTwo);
    }
    
    public void OnRollButtonClicked()
    {
        DiceManager.Instance.SimulateThrow(rollOneInputValue, rollTwoInputValue);
    }
}
