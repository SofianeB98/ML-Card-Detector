using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearMultipleGenerator : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject sphereRouge;
    public GameObject sphereBleu;
    public GameObject sphereTest;

    [Header("Parent")] 
    public Transform datasetParent;

    public Transform inputsParent;
    
    [Header("Generation")] 
    public int spherePerClass = 50;
    
    void Start()
    {
        MLManager.Instance.dataset = new Transform[spherePerClass * 2];
        for (int i = 0; i < spherePerClass; ++i)
        {
            MLManager.Instance.dataset[i] = Instantiate(sphereBleu, new Vector3(Random.Range(1.0f, 2.01f), 1, Random.Range(1.0f, 2.01f)), Quaternion.identity, datasetParent).transform;
            MLManager.Instance.dataset[i].localScale = Vector3.one * 0.1f;
        }
        
        for (int i = spherePerClass; i < spherePerClass * 2; ++i)
        {
            MLManager.Instance.dataset[i] = Instantiate(sphereRouge, new Vector3(Random.Range(2.01f, 3.01f), -1, Random.Range(2.01f, 3.01f)), Quaternion.identity, datasetParent).transform;
            MLManager.Instance.dataset[i].localScale = Vector3.one * 0.1f; 
        }

        MLManager.Instance.sampleCounts = spherePerClass * 2;
        
        int sphereTestPerAxis = (int)(0.1f * 3 * (spherePerClass * 2));
        MLManager.Instance.inputs = new Transform[(int)sphereTestPerAxis * sphereTestPerAxis];
        for (int i = 0; i < sphereTestPerAxis; ++i)
        {
            float x = (i + 5) * 0.1f;
            for (int j = 0; j < sphereTestPerAxis; ++j)
            {
                float z = (j + 5) * 0.1f;
                MLManager.Instance.inputs[i * sphereTestPerAxis + j] = Instantiate(sphereTest, new Vector3(x, 0, z), Quaternion.identity, inputsParent).transform;
                MLManager.Instance.inputs[i * sphereTestPerAxis + j].localScale = Vector3.one * 0.1f;
            }
        }
    }

}
