using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LinearMultipleMulticlassGenerator : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject sphereRouge;
    public GameObject sphereBleu;
    public GameObject sphereVerte;
    public GameObject sphereTest;

    [Header("Parent")] 
    public Transform datasetParent;
    public Transform inputsParent;
    
    [Header("Generation")] 
    public int totalSphere = 600;
    public float sphereScale = 0.1f;
    
    //-p.x - p.z - 0.5 > 0 && p.z < 0 && p.x - p.z - 0.5 < 0 == [1, 0, 0] == bleu
    //-p.x - p.z - 0.5 < 0 && p.z > 0 && p.x - p.z - 0.5 < 0 == [0, 1, 0] == rouge
    //-p.x - p.z - 0.5 < 0 && p.z < 0 && p.x - p.z - 0.5 > 0 == [0, 0, 1] == verte
    
    
    void Start()
    {
        MLManager.Instance.dataset = new Transform[totalSphere];
        LinearMulticlassMLManager.Instance.dataset = new Transform[totalSphere];

        for (int i = 0; i < totalSphere; i++)
        {
            Vector3 p = new Vector3(Random.Range(-1.0f, 1.01f), 0, Random.Range(-1.0f, 1.01f));

            if (-p.x - p.z - 0.5 > 0 && p.z < 0 && p.x - p.z - 0.5 < 0)
            {
                p.y = 1;
                Transform tr = Instantiate( sphereBleu, p,
                    Quaternion.identity, datasetParent).transform;
                tr.localScale = Vector3.one * sphereScale;
                MLManager.Instance.dataset[i] = tr;
                LinearMulticlassMLManager.Instance.dataset[i] = tr;
            }
            else if (p.x - p.z - 0.5 < 0 && p.z > 0 && p.x - p.z - 0.5 < 0)
            {
                p.y = 2;
                Transform tr = Instantiate( sphereRouge, p,
                    Quaternion.identity, datasetParent).transform;
                tr.localScale = Vector3.one * sphereScale;
                MLManager.Instance.dataset[i] = tr;
                LinearMulticlassMLManager.Instance.dataset[i] = tr;

            }
            else if (-p.x - p.z - 0.5 < 0 && p.z < 0 && p.x - p.z - 0.5 > 0)
            {
                p.y = 3;
                Transform tr = Instantiate( sphereVerte, p,
                    Quaternion.identity, datasetParent).transform;
                tr.localScale = Vector3.one * sphereScale;
                MLManager.Instance.dataset[i] = tr;
                LinearMulticlassMLManager.Instance.dataset[i] = tr;

            }
            else
            {
                i--;
                continue;
            }
        }

        MLManager.Instance.dataset = MLManager.Instance.dataset.OrderBy(t => t.position.y).ToArray();
        LinearMulticlassMLManager.Instance.dataset = LinearMulticlassMLManager.Instance.dataset.OrderBy(t => t.position.y).ToArray();
        
        MLManager.Instance.sampleCounts = totalSphere;
        LinearMulticlassMLManager.Instance.sampleCounts = totalSphere;

        int sphereTestPerAxis = (int) Mathf.Sqrt(totalSphere);
        MLManager.Instance.inputs = new Transform[(int)sphereTestPerAxis * sphereTestPerAxis];
        LinearMulticlassMLManager.Instance.inputs = new Transform[(int)sphereTestPerAxis * sphereTestPerAxis];
        
        for (int i = 0; i < sphereTestPerAxis; ++i)
        {
            float x = Mathf.Lerp(-1.0f, 1.0f, (float) i / (float) sphereTestPerAxis);
            for (int j = 0; j < sphereTestPerAxis; ++j)
            {
                float z = Mathf.Lerp(-1.0f, 1.0f, (float) j / (float) sphereTestPerAxis);
                Transform tr =  Instantiate(sphereTest, new Vector3(x, 0, z), Quaternion.identity, inputsParent).transform;
                tr.localScale =Vector3.one * sphereScale;
                MLManager.Instance.inputs[i * sphereTestPerAxis + j] = tr;
                LinearMulticlassMLManager.Instance.inputs[i * sphereTestPerAxis + j] = tr;
            }
        }
    }

}
