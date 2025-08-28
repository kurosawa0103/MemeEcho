using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ZSegmentRuleConfig", menuName = "Sorting Tools/ZSegmentRuleConfig")]
public class ZSegmentRuleConfig : ScriptableObject
{
    [System.Serializable]
    public class ZSegmentRule
    {
        public float minZ;
        public float maxZ;
        public string sortingLayer = "Default";
    }

    public List<ZSegmentRule> rules = new List<ZSegmentRule>();
}
