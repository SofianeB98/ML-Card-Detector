using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearMLManager : MonoBehaviour
{
    private static LinearMLManager instance;

    public static LinearMLManager Instance
    {
        get { return instance; }
    }
    
    [Header("ML Parameter")] 
    public bool createModelOnStart = true;
    public int sampleCounts = 4;
    public int epochs = 10000;
    public double alpha = 0.01;
    public bool isClassification = true;
    public int input_size = 0;
    public int output_size = 0;
    private System.IntPtr model;
    
    [Header("Dataset")] 
    public Transform[] dataset = new Transform[0];
    private double[] inputs_dataset = new double[0];
    private double[] outputs = new double[0];

    [Header("Inputs population")] 
    public Transform[] inputs = new Transform[0];

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if(instance != this)
            Destroy(this);
    }

    private void Start()
    {
        if (!enabled)
            return;
        
        if (!createModelOnStart)
            return;
        
        if (!model.Equals(IntPtr.Zero))
        {
            Debug.LogError("You trying to created an other model, we delete the old model before");
            MLDLLWrapper.DeleteModel(model);
            Debug.Log("Modèle détruit\n");
        }

        if (isClassification)
            model = MLDLLWrapper.CreateLinearModel(input_size);
        else
            model = MLDLLWrapper.CreateLinearModelRegression(input_size);
        
        Debug.Log("Modèle créé \n");
        
        inputs_dataset = new double[dataset.Length * input_size];
        outputs = new double[dataset.Length * output_size];

        int idx = 0;
        int idx_out = 0;
        foreach (var tr in dataset)
        {
            Vector3 p = tr.position;
            if (input_size == 1)
            {
                inputs_dataset[idx] = p.x;
                idx++;
            }
            else
            {
                inputs_dataset[idx] = p.x;
                idx++;
                inputs_dataset[idx] = p.z;
                idx++;
            }
            
            outputs[idx_out] = p.y;
            idx_out++;

        }


        Debug.Log("Tableau d'input initialisé depuis les inputs bruts\n");
    }

    private void OnDestroy()
    {
        if (model.Equals(IntPtr.Zero))
            return;
        
        MLDLLWrapper.DeleteLinearModel(model);
        Debug.Log("Modèle détruit\n");
    }

    public void CreateModel()
    {if (!enabled)
            return;
        
        if (!model.Equals(IntPtr.Zero))
        {
            Debug.LogError("You trying to created an other model, we delete the old model before");
            MLDLLWrapper.DeleteModel(model);
            Debug.Log("Modèle détruit\n");
        }
        
        if (isClassification)
            model = MLDLLWrapper.CreateLinearModel(input_size);
        else
            model = MLDLLWrapper.CreateLinearModelRegression(input_size);
        Debug.Log("Modèle créé \n");


        inputs_dataset = new double[dataset.Length * input_size];
        outputs = new double[dataset.Length * output_size];

        int idx = 0;
        int idx_out = 0;
        foreach (var tr in dataset)
        {
            Vector3 p = tr.position;
            if (input_size == 1)
            {
                inputs_dataset[idx] = p.x;
                idx++;
            }
            else
            {
                inputs_dataset[idx] = p.x;
                idx++;
                inputs_dataset[idx] = p.z;
                idx++;
            }
            
            outputs[idx_out] = p.y;
            idx_out++;

        }

        Debug.Log("Tableau d'input initialisé depuis les inputs bruts\n");
    }
    
    public void TrainModel()
    {
        if (model.Equals(IntPtr.Zero))
        {
            Debug.LogError("You trying to train your model, but it's not created");
            return;
        }
        
        Debug.Log("On entraîne le modèle\n...");
        if(isClassification)
            MLDLLWrapper.TrainLinearModelRosenblatt(model, inputs_dataset, input_size, sampleCounts, outputs, output_size, epochs, alpha);
        else
            MLDLLWrapper.TrainLinearModelRegression(model, inputs_dataset, input_size, sampleCounts, outputs, output_size);
        Debug.Log("Modèle entrainé \n");
    }

    public void Predict()
    {
        if (model.Equals(IntPtr.Zero))
        {
            Debug.LogError("You trying to predict these inputs, but your model is not created");
            return;
        }
        
        Debug.Log("Prediction du dataset !\n");
        int idx = 0;

        for (int i = 0; i < inputs.Length; i++)
        {
            string str = "";

            double[] data; 
            
            if(input_size == 1)
                data = isClassification ? new double[] {1.0, inputs[i].position.x} : new double[] {inputs[i].position.x};
            else
                data = isClassification ? new double[] {1.0, inputs[i].position.x, inputs[i].position.z} : new double[] {inputs[i].position.x, inputs[i].position.z};
            
            str += "[ " + (input_size == 2 ? data[0].ToString("0.00") + ", " + data[1].ToString("0.00") : data[0].ToString("0.00")) + " ] = ";
            var result = MLDLLWrapper.PredictLinearModel(model, data, input_size, isClassification);
            
            str += result.ToString("0.000");
            Debug.LogWarning("Prediction : " + str);
            
            inputs[i].position = new Vector3(inputs[i].position.x, (isClassification ? Mathf.RoundToInt((float)result) : (float)result), inputs[i].position.z);

            //MLDLLWrapper.DeleteDoubleArrayPtr(result);
        }
    }
}
