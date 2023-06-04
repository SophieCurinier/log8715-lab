using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/*
   DO NOT CHANGE, THIS WILL BE REPLACED AT CORRECTION
   NE PAS CHANGER, CE FICHIER VA ÊTRE REMPLACÉ A LA CORRECTION
*/

[CreateAssetMenu(fileName = "Config", menuName = "General Config", order = 1)]
public class Config : ScriptableObject {

    [Serializable]
    public struct ShapeConfig
    {
        public int initialSize;
        public Vector2 initialPosition;
        public Vector2 initialVelocity;
    }
    
    [Header("Simulation")]
    [SerializeField]
    [Tooltip("Seed to initialize Randomization")]
    public int seed = int.MaxValue;
    [SerializeField]
    [Tooltip("At this size, circles explode")]
    public int explosionSize = 10;
    [SerializeField]
    [Tooltip("At this size, circles can be protected")]
    public int protectionSize = 2;
    [SerializeField]
    [Tooltip("At each loop, probability for a circle to become protected")]
    public float protectionProbability = 0.01f;
    [SerializeField]
    [Tooltip("Duration of the protection")]
    public float protectionDuration = 5;
    [SerializeField]
    [Tooltip("Cooldown before a circle can become protected again")]
    public float protectionCooldown = 15;
    
    [SerializeField]
    public List<ShapeConfig> circleInstancesToSpawn;
    
    [Header("Systems")]
    [SerializeField]
    [Tooltip("Key is the name of the system to enable")]
    private StringBoolDictionary systemsEnabled = StringBoolDictionary.New<StringBoolDictionary>();
    public Dictionary<string, bool> SystemsEnabled => systemsEnabled.dictionary;
}
