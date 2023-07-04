using System;
using System.Collections.Generic;
using UnityEngine;


public class Dice : MonoBehaviour
{
    [SerializeField] public DiceRotationSo diceRotationData;
    public Transform diceMesh;
    private int totalFaceValue = 7; //Every sum of the opposite face values are 7.
    
    
    private void ResetDiceRotation()
    {
        diceMesh.rotation = Quaternion.Euler(Vector3.zero);
    }

    public void RotateDice(int desiredValue)
    {
        ResetDiceRotation();
        diceMesh.rotation = Quaternion.Euler(diceRotationData.rotationsForIndexFaces[desiredValue]); 
    }


}

    


