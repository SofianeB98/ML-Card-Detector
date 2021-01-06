using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MLP
{
    [SerializeField] public List<ListOfListDouble> W;
}

[System.Serializable]
public class ListOfListDouble
{
    [SerializeField] public List<ListOfDouble> Wi;
}

[System.Serializable]
public class ListOfDouble
{
    [SerializeField] public List<double> Wj;
}