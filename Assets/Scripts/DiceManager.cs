using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;



    public class DiceManager : MonoBehaviour
    {
        public static DiceManager Instance;
        
        private List<Vector3> initialDicePositions = new List<Vector3>();
        private List<Transform> Dices = new List<Transform>();
        private Quaternion rotation;
        private Vector3 force;
        private Vector3 torque;
        

        [SerializeField] private int simulationFrameLength = 50;
        [SerializeField] private int optimizationCount = 2;
        private Dictionary<int, List<TransformData>> diceAnimationData = new Dictionary<int, List<TransformData>>();
        [SerializeField] private List<TransformData> listShower;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (simulationFrameLength % optimizationCount != 0)
            {
                Debug.LogError("Frame Length must divide optimization count");
            }
        }
#endif

        private void Start()
        {
            foreach (var dice in GetComponentsInChildren<Transform>())
            {
                if (dice.GetComponent<Rigidbody>() != null)
                {
                    Dices.Add(dice);
                    DisablePhysics(dice);
                    initialDicePositions.Add(dice.position);
                }
            }
            for (int i = 0; i < Dices.Count; i++)
            {
                diceAnimationData.Add(i, new List<TransformData>());
            }

            listShower = diceAnimationData[0];
        }

        public void SimulateThrow(params int[] diceValues)
        {
            Physics.autoSimulation = false;
            SetInitialState();
            ClearAnimationData();
            RecordAnimation();
            RotateDices(diceValues);
            Physics.autoSimulation = true;
            StartCoroutine(PlayAnimation());
        }


        private void EnablePhysics(Transform dice)
        {
            dice.GetComponent<Rigidbody>().isKinematic = false;
            dice.GetComponent<Rigidbody>().useGravity = true;
        }
        private void DisablePhysics(Transform dice)
        {
            dice.GetComponent<Rigidbody>().isKinematic = true;
            dice.GetComponent<Rigidbody>().useGravity = false;
        }
        public void SetInitialState()
        {
            for (int i = 0; i < Dices.Count; i++)
            {
                Dices[i].position = initialDicePositions[i];
                EnablePhysics(Dices[i]);
                SetInitialRotation(Dices[i]);
                SetInitialForce(Dices[i]);
                SetInitialTorque(Dices[i]);
            }
        }
        private void SetInitialRotation(Transform dice)
        {
            float x = Random.Range(0f, 360f);
            float y = Random.Range(0f, 360f);
            float z = Random.Range(0f, 360f);

            rotation = Quaternion.Euler(x, y, z);
            dice.rotation = rotation;
        }
        private void SetInitialForce(Transform dice)
        {
            float x = Random.Range(0f, 1f);
            float y = Random.Range(0f, 1f);
            float z = 10f;

            force = new Vector3(x, y, z);
            dice.GetComponent<Rigidbody>().velocity = force;
        }
        private void SetInitialTorque(Transform dice)
        {
            float x = Random.Range(0f, 25f);
            float y = Random.Range(0f, 25f);
            float z = Random.Range(0f, 25f);

            torque = new Vector3(x, y, z);
            dice.GetComponent<Rigidbody>().AddTorque(torque, ForceMode.VelocityChange);
        }
        

        private void RecordAnimation()
        {
            for (int i = 0; i <= simulationFrameLength / optimizationCount; i++)
            {
                for (int j = 0; j < Dices.Count; j++)
                {
                    diceAnimationData[j].Add(new TransformData(Dices[j].position, Dices[j].rotation));
                }
                Physics.Simulate(Time.fixedDeltaTime * optimizationCount);
            }

            for (int j = 0; j < Dices.Count; j++)
            {
                var tempList = new List<TransformData>(simulationFrameLength);
                tempList.Add(diceAnimationData[j][0]);
                
                for (int i = 1; i < diceAnimationData[j].Count; i++)
                {
                    var nextTransformData = diceAnimationData[j][i];

                    for (int k = 1; k < optimizationCount; k++)
                    {
                        tempList.Add(new TransformData(
                            Vector3.Lerp(diceAnimationData[j][i - 1].position, nextTransformData.position, (float)k / optimizationCount),
                            Quaternion.Lerp(diceAnimationData[j][i - 1].rotation, nextTransformData.rotation, (float)k / optimizationCount)));
                    }

                    tempList.Add(nextTransformData);
                }

                diceAnimationData[j] = tempList;
                
                listShower = tempList;
            }

            
        }
        private IEnumerator PlayAnimation()
        {
            for (int i = 0; i <= simulationFrameLength; i++)
            {
                for (int j = 0; j < Dices.Count; j++)
                {
                    Dices[j].transform.position = diceAnimationData[j][i].position;
                    Dices[j].transform.rotation = diceAnimationData[j][i].rotation;
                }
                yield return new WaitForFixedUpdate();
            }
        }

        private void ClearAnimationData()
        {

            for (int i = 0; i < Dices.Count; i++)
            {
                if (diceAnimationData[i] != null)
                {
                    diceAnimationData[i].Clear();
                }
            }
        }

        private void RotateDices(params int[] diceValues)
        {
            for (var i = 0; i < diceValues.Length; i++)
            {
                var diceValue = diceValues[i];
                Dices[i].GetComponent<Dice>().RotateDice(diceValue);
            }
        }
    }
[System.Serializable]
    class TransformData
    {
        public Vector3 position;
        public Quaternion rotation;

        public TransformData(Vector3 position, Quaternion rotation)
        {
            this.position = position;
            this.rotation = rotation;
        }
    }
