using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MLManager : MonoBehaviour
{
    [Header("ML Parameter")]
    public int[] npl = new int[0];
    public int sampleCounts = 4;
    public int epochs = 10000;
    public double alpha = 0.01;
    public bool isClassification = true;

    [Header("Inputs")] 
    public Transform[] transformInputs = new Transform[0];
    private double[] inputs = new double[0];
    
    [Header("Outputs")] 
    public double[] outputs = new double[0];

    private System.IntPtr model;
    
    private void Start()
    {
        model = MLDLLWrapper.CreateModel(npl, npl.Length);
        Debug.Log("Modèle créé \n");
        
        inputs = new double[transformInputs.Length*2];
        int idx = 0;
        foreach (var tr in transformInputs)
        {
            Vector3 p = tr.position;
            inputs[idx] = p.x;
            idx++;
            inputs[idx] = p.y;
            idx++;
        }
        Debug.Log("Tableau d'input initialisé depuis les inputs bruts\n");
    }

    private void OnDestroy()
    {
        MLDLLWrapper.DeleteModel(model);
        Debug.Log("Modèle détruit\n");
    }

    public void TrainModel()
    {
        Debug.Log("On entraîne le modèle\n...");
        MLDLLWrapper.Train(model, inputs, outputs, sampleCounts, epochs, alpha, isClassification);
        Debug.Log("Modèle entrainé \n");
    }

    public void Predict()
    {
        Debug.Log("Prediction du dataset !\n");
        int idx = 0;
        for (int i = 0; i < transformInputs.Length; i++)
        {
            string str = "";

            double[] data = new double[] {inputs[idx++], inputs[idx++]};

            str += "[ " + data[0].ToString("0.000") + ", " + data[1].ToString("0.000") + " ] = ";
            var result = MLDLLWrapper.Predict(model, data, isClassification);
            double[] r = new double[2];
            System.Runtime.InteropServices.Marshal.Copy(result, r, 0, 2);
            str += r[1].ToString("0.0000000000000000");
            Debug.LogWarning("Prediction : " + str);
            MLDLLWrapper.DeleteDoubleArrayPtr(result);
        }
    }
}
