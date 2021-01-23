using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialBasisFunctionMLManager : MachineLearningAbstract
{
    private static RadialBasisFunctionMLManager instance;

    public static RadialBasisFunctionMLManager Instance
    {
        get { return instance; }
    }

    public bool createModelOnStart = true;

    [Header("Dataset")] public Transform[] dataset = new Transform[0];
    private double[] inputs_dataset = new double[0];
    private double[] outputs = new double[0];
    public int k = 1;

    [Header("Inputs population")] public Transform[] inputs = new Transform[0];

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this);
    }

    private void OnDestroy()
    {
        DeleteModel();
    }

    private void Start()
    {
        if (!enabled)
            return;

        if (!createModelOnStart)
            return;

        CreateModel();
    }

    public override void CreateModel()
    {
        if (!enabled)
            return;

        if (!model.Equals(IntPtr.Zero))
        {
            Debug.LogError("You trying to created an other model, we delete the old model before");
            DeleteModel();
            Debug.Log("Modèle détruit\n");
        }

        model = MLDLLWrapper.CreateRBFModel(k, alpha);

        inputs_dataset = new double[dataset.Length * input_size];
        outputs = new double[dataset.Length * output_size];

        if (output_size == 1)
            isClassification = false;

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
                    outputs[idx_out] = p.y;
                    idx_out++;
                    outputs[idx_out] = p.y * -1.0f;
                    idx_out++;
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
        MLDLLWrapper.TrainRBFModel(model, inputs_dataset, input_size, sampleCounts, outputs, output_size);
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

        if (output_size == 1)
            isClassification = false;

        Debug.Log("Prediction du dataset !\n");
        for (int i = 0; i < inputs.Length; i++)
        {
            string str = "";

            double[] data;

            if (input_size == 1)
                data = new double[]
                {
                    inputs[i].position.x
                };
            else
                data = new double[] {inputs[i].position.x, inputs[i].position.z};

            str += "[ " + (input_size == 2
                ? data[0].ToString("0.00") + ", " + data[1].ToString("0.00")
                : data[0].ToString("0.00")) + " ] = ";

            var result = MLDLLWrapper.PredictRBF(model, data, input_size, isClassification);

            str += result.ToString("0.000");
            Debug.LogWarning("Prediction : " + str);

            inputs[i].position = new Vector3(inputs[i].position.x, (float)(output_size == 2 ? (result <= 0.01 ? -1.0f : result) : result), inputs[i].position.z);
        }
    }

    public override void DeleteModel()
    {
        if (model.Equals(IntPtr.Zero))
            return;

        MLDLLWrapper.DeleteRBFModel(model);
        model = IntPtr.Zero;
        Debug.Log("Modèle détruit\n");
    }
}