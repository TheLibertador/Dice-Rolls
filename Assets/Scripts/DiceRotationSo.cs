using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Dice Rotation Data", menuName = "DiceRotationData")]
    public class DiceRotationSo : ScriptableObject
    {
        public List<Vector3> rotationsForIndexFaces = new List<Vector3>(7); //Index numbers here represents dice face values please do not use the index 0.
    }

