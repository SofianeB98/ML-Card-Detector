﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiLayerPerceptronMLManager : MachineLearningAbstract
{
    private static MultiLayerPerceptronMLManager instance;

    public static MultiLayerPerceptronMLManager Instance
    {
        get { return instance; }
    }
    
    [Header("PMC Parameter")] 
    public int[] npl = new int[0];

    [Space(10)]
    public bool createModelOnStart = true;
    
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
        
        CreateModel();
        Debug.Log("Tableau d'input initialisé depuis les inputs bruts\n");
    }

    private void OnDestroy()
    {
        DeleteModel();
    }

    public override void CreateModel()
    {
        if (!enabled)
            return;
        
        if (!model.Equals(IntPtr.Zero))
        {
            Debug.LogError("You trying to created an other model, we delete the old model before");
            MLDLLWrapper.DeleteModel(model);
            Debug.Log("Modèle détruit\n");
        }
        
        model = MLDLLWrapper.CreateModel(npl, npl.Length);
        Debug.Log("Modèle créé \n");

        input_size = npl[0];
        output_size = npl[npl.Length - 1];

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
            
            switch (output_size)
            {
                case 0:
                case 1:
                    outputs[idx_out] = p.y;
                    idx_out++;
                    break;
                
                case 2:
                    break;
                
                case 3:
                    if (Mathf.Abs(p.y - 1.0f) <= 0.01f)
                    {
                        outputs[idx_out] = 1.0;
                        idx_out++;
                        outputs[idx_out] = 0.0;
                        idx_out++;
                        outputs[idx_out] = 0.0;
                        idx_out++;
                    }
                    else if (Mathf.Abs(p.y - 2.0f) <= 0.01f)
                    {
                        outputs[idx_out] = 0.0;
                        idx_out++;
                        outputs[idx_out] = 1.0;
                        idx_out++;
                        outputs[idx_out] = 0.0;
                        idx_out++;
                    }
                    else if (Mathf.Abs(p.y - 3.0f) <= 0.01f)
                    {
                        outputs[idx_out] = 0.0;
                        idx_out++;
                        outputs[idx_out] = 0.0;
                        idx_out++;
                        outputs[idx_out] = 1.0;
                        idx_out++;
                    }
                    break;
            }

        }


        Debug.Log("Tableau d'input initialisé depuis les inputs bruts\n");
    }
    
    public override void TrainModel()
    {
        if (!enabled)
            return;
        if (model.Equals(IntPtr.Zero))
        {
            Debug.LogError("You trying to train your model, but it's not created");
            return;
        }
        
        Debug.Log("On entraîne le modèle\n...");
        MLDLLWrapper.Train(model, inputs_dataset, outputs, sampleCounts, epochs, alpha, isClassification);
        Debug.Log("Modèle entrainé \n");
    }

    public override void Predict()
    {
        if (!enabled)
            return;
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

            double[] data;// = new double[] {inputs[i].position.x, inputs[i].position.z};
            
            if(input_size == 1)
                data = new double[]{inputs[i].position.x};
            else
                data = new double[]{inputs[i].position.x, inputs[i].position.z};

            if (output_size == 1)
            {
                str += "[ " + (input_size == 2 ? data[0].ToString("0.00") + ", " + data[1].ToString("0.00") : data[0].ToString("0.00")) + " ] = ";
                var result = MLDLLWrapper.Predict(model, data, isClassification);
                double[] r = new double[output_size + 1];
                System.Runtime.InteropServices.Marshal.Copy(result, r, 0, output_size + 1);
                str += r[1].ToString("0.000");
                Debug.LogWarning("Prediction : " + str);

                inputs[i].position = new Vector3(inputs[i].position.x,
                    (isClassification ? (r[1] < 0 ? -1 : 1) : (float) r[1]), inputs[i].position.z); //Mathf.RoundToInt((float) r[1])

                MLDLLWrapper.DeleteDoubleArrayPtr(result);
            }
            else if (output_size == 3)
            {
                str += "[ " + (input_size == 2 ? data[0].ToString("0.00") + ", " + data[1].ToString("0.00") : data[0].ToString("0.00")) + " ] = ";
                var result = MLDLLWrapper.Predict(model, data, isClassification);
                double[] r = new double[output_size + 1];
                System.Runtime.InteropServices.Marshal.Copy(result, r, 0, output_size + 1);
                for(int z = 0; z < r.Length; z++)
                    str += r[z].ToString("0.000") + " || ";
                
                Debug.LogWarning("Prediction : " + str);

                inputs[i].position = new Vector3(inputs[i].position.x,
                    (r[1] > r[2] && r[1] > r[3] ? 1 :
                        r[2] > r[1] && r[2] > r[3] ? 2 :
                        r[3] > r[2] && r[1] < r[3] ? 3 : -1), inputs[i].position.z);

                MLDLLWrapper.DeleteDoubleArrayPtr(result);
            }
        }
    }

    public override void DeleteModel()
    {
        if (model.Equals(IntPtr.Zero))
            return;
        
        MLDLLWrapper.DeleteModel(model);
        Debug.Log("Modèle détruit\n");
    }
}