using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CrossGenerator : MonoBehaviour
{
    [Header("Prefab")] public GameObject sphereRouge;
    public GameObject sphereBleu;
    public GameObject sphereTest;
    public float sphereScale = 0.1f;
    public int totalSphere = 500;

    [Header("Parent")] public Transform datasetParent;
    public Transform inputsParent;

    private void Start()
    {
        MultiLayerPerceptronMLManager.Instance.dataset = new Transform[totalSphere];
        LinearMLManager.Instance.dataset = new Transform[totalSphere];
        RadialBasisFunctionMLManager.Instance.dataset = new Transform[totalSphere];
        
        for (int i = 0; i < totalSphere; ++i)
        {
            Vector3 pos = new Vector3(Random.Range(-1.0f, 1.01f), 0.0f, Random.Range(-1.0f, 1.01f));
            bool blue = (Mathf.Abs(pos.x) <= 0.3f || Mathf.Abs(pos.z) <= 0.3f);

            if (blue)
            {
                pos.y = 1.0f;
                Transform tr = Instantiate(sphereBleu, pos, Quaternion.identity, datasetParent).transform;
                tr.localScale = Vector3.one * sphereScale;

                MultiLayerPerceptronMLManager.Instance.dataset[i] = tr;
                LinearMLManager.Instance.dataset[i] = tr;
                RadialBasisFunctionMLManager.Instance.dataset[i] = tr;
            }
            else
            {
                pos.y = -1.0f;
                Transform tr = Instantiate(sphereRouge, pos, Quaternion.identity, datasetParent).transform;
                tr.localScale = Vector3.one * sphereScale;

                MultiLayerPerceptronMLManager.Instance.dataset[i] = tr;
                LinearMLManager.Instance.dataset[i] = tr;
                RadialBasisFunctionMLManager.Instance.dataset[i] = tr;
            }
        }

        MultiLayerPerceptronMLManager.Instance.sampleCounts = totalSphere;
        LinearMLManager.Instance.sampleCounts = totalSphere;
        RadialBasisFunctionMLManager.Instance.sampleCounts = totalSphere;
        
        int sphereTestPerAxis = (int)(Mathf.Sqrt(totalSphere));
        MultiLayerPerceptronMLManager.Instance.inputs = new Transform[(int) sphereTestPerAxis * sphereTestPerAxis];
        LinearMLManager.Instance.inputs = new Transform[(int) sphereTestPerAxis * sphereTestPerAxis];
        RadialBasisFunctionMLManager.Instance.inputs = new Transform[(int) sphereTestPerAxis * sphereTestPerAxis];
        
        for (int i = 0; i < sphereTestPerAxis; ++i)
        {
            float x = Mathf.Lerp(-1.0f, 1.0f, (float) i / (float) sphereTestPerAxis);
            for (int j = 0; j < sphereTestPerAxis; ++j)
            {
                float z = Mathf.Lerp(-1.0f, 1.0f, (float) j / (float) sphereTestPerAxis);
                Transform tr =  Instantiate(sphereTest, new Vector3(x, 0, z), Quaternion.identity, inputsParent).transform;
                tr.localScale =Vector3.one * sphereScale;
                MultiLayerPerceptronMLManager.Instance.inputs[i * sphereTestPerAxis + j] = tr;
                LinearMLManager.Instance.inputs[i * sphereTestPerAxis + j] = tr;
                RadialBasisFunctionMLManager.Instance.inputs[i * sphereTestPerAxis + j] = tr;
            }
        }
        
    }
}