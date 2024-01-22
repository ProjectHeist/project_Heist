using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "PatrolRoute", fileName = "PatrolRoute")]
public class PatrolRoute : ScriptableObject
{
    [SerializeField]
    public List<Route> routes;

    [System.Serializable]
    public class Route
    {
        [SerializeField]
        public List<int> movedist;
        [SerializeField]
        public List<string> direction;
    }
}


