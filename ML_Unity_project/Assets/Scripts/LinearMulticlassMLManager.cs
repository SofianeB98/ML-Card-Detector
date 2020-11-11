using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearMulticlassMLManager : MonoBehaviour
{
    private static LinearMulticlassMLManager instance;

    public static LinearMulticlassMLManager Instance
    {
        get { return instance; }
    }

    [Header("ML Parameter")]
    public int sampleCounts = 4;
    public int epochs = 10000;
    public double alpha = 0.01;
    public bool isClassification = true;
    public int input_size = 0;
    public int output_size = 0;
    public int classCount = 3;

    private System.IntPtr[] models;

    [Header("Dataset")] public Transform[] dataset = new Transform[0];
    private double[] inputs_dataset = new double[0];
    private double[] outputs = new double[0];

    [Header("Inputs population")] public Transform[] inputs = new Transform[0];

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this);
    }

    private void Start()
    {
        if (!enabled)
            return;
        models = new IntPtr[classCount];
    }

    private void OnDestroy()
    {
        if (models == null || models.Length <= 0)
        {
            Debug.LogError("Models are empty");
            return;
        }
        
        for (int i = 0; i < models.Length; i++)
        {
            if (models[i].Equals(IntPtr.Zero))
                return;

            MLDLLWrapper.DeleteLinearModel(models[i]);
            Debug.Log("Modèle détruit\n");
        }
    }

    public void CreateModel()
    {
        if (!enabled)
            return;

        for (int i = 0; i < models.Length; i++)
        {
            if (!models[i].Equals(IntPtr.Zero))
            {
                Debug.LogError("You trying to created an other model, we delete the old model before");
                MLDLLWrapper.DeleteModel(models[i]);
                Debug.Log("Modèle détruit\n");
            }
        }
        
        models = new IntPtr[classCount];
        
        for (int i = 0; i < models.Length; i++)
        {
            if (isClassification)
                models[i] = MLDLLWrapper.CreateLinearModel(dataset.Length * input_size);
            else
                models[i] = MLDLLWrapper.CreateLinearModelRegression(dataset.Length * input_size);
            Debug.Log("Modèle créé \n");
        }
        
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
        if (!enabled)
            return;
        
        if (models == null || models.Length <= 0)
        {
            Debug.LogError("Models are empty");
            return;
        }
        
        for (int i = 0; i < classCount; i++)
        {
            if (models[i].Equals(IntPtr.Zero))
            {
                Debug.LogError("You trying to train your model, but it's not created");
                continue;
            }
            
            Transform[] tmp = new Transform[dataset.Length];
            
            int _idx = 0;
            for (int j = 0; j < dataset.Length; j++)
            {
                tmp[_idx] = dataset[j];
                _idx++;
            }
            
            int idx = 0;
            int idx_out = 0;
            double[] inputs_dataset_tmp = new double[tmp.Length * input_size];
            double[] outputs_tmp = new double[tmp.Length * output_size];
            
            foreach (var tr in tmp)
            {
                Vector3 p = tr.position;
                if (input_size == 1)
                {
                    inputs_dataset_tmp[idx] = p.x;
                    idx++;
                }
                else
                {
                    inputs_dataset_tmp[idx] = p.x;
                    idx++;
                    inputs_dataset_tmp[idx] = p.z;
                    idx++;
                }

                switch (i)
                {
                    case 0:
                        if(p.y.Equals(1))
                            outputs_tmp[idx_out] = 1.0;
                        else
                            outputs_tmp[idx_out] = -1.0;
                        idx_out++;
                        break;
                    
                    case 1:
                        if(p.y.Equals(2))
                            outputs_tmp[idx_out] = 1.0;
                        else
                            outputs_tmp[idx_out] = -1.0;
                        idx_out++;
                        break;
                    
                    case 2:
                        if(p.y.Equals(3))
                            outputs_tmp[idx_out] = 1.0;
                        else
                            outputs_tmp[idx_out] = -1.0;
                        idx_out++;
                        break;
                }
                
            
            }
            
            Debug.Log("On entraîne le modèle\n...");
            if (isClassification)
                MLDLLWrapper.TrainLinearModelRosenblatt(models[i], inputs_dataset_tmp, input_size, tmp.Length, outputs_tmp,
                    output_size, epochs, alpha);
            else
                MLDLLWrapper.TrainLinearModelRegression(models[i], inputs_dataset_tmp, input_size, tmp.Length, outputs_tmp,
                    output_size);
            Debug.Log("Modèle entrainé \n");
        }
        
    }

    public void Predict()
    {
        if (!enabled)
            return;
        
        if (models == null || models.Length <= 0)
        {
            Debug.LogError("Models are empty");
            return;
        }
        
        Debug.Log("Prediction du dataset !\n");
        int idx = 0;

        for (int i = 0; i < inputs.Length; i++)
        {
            string str = "";

            double[] data;

            if (input_size == 1)
                data = isClassification
                    ? new double[] {1.0, inputs[i].position.x}
                    : new double[] {inputs[i].position.x};
            else
                data = isClassification
                    ? new double[] {1.0, inputs[i].position.x, inputs[i].position.z}
                    : new double[] {inputs[i].position.x, inputs[i].position.z};

            str += "[ " + (input_size == 2
                ? data[0].ToString("0.00") + ", " + data[1].ToString("0.00")
                : data[0].ToString("0.00")) + " ] = ";
            var result = MLDLLWrapper.PredictLinearModelMulticlass(models, data, input_size, classCount, isClassification);
            
            double[] r = new double[classCount];
            System.Runtime.InteropServices.Marshal.Copy(result, r, 0, classCount);
            
            for(int z = 0; z < r.Length; z++)
                str += r[z].ToString("0.000") + " || ";
                
            Debug.LogWarning("Prediction : " + str);

            inputs[i].position = new Vector3(inputs[i].position.x,
                (r[0] > r[1] && r[0] > r[2] ? 1 :
                    r[1] > r[0] && r[1] > r[2] ? 2 :
                    r[2] > r[1] && r[0] < r[2] ? 3 : -1), inputs[i].position.z);

            MLDLLWrapper.DeleteDoubleArrayPtr(result);
        }
    }
}