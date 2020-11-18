﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Random = UnityEngine.Random;

public class LinearModelManagerTexture : MonoBehaviour
{
    private System.IntPtr model;


    [Header("ML Parameter")] 
    public int inputSize = 256;
    public int outputSize = 1;
    [Space(10)] public int sampleCounts = 4;
    public int epochs = 10000;
    public double alpha = 0.01;
    public bool isClassification = true;


    [Header("Train Parameter")] public int trainLoopCount = 1;
    [Range(0.1f, 1.0f)] public float useDatasetAsNPercent = 0.5f;


    [Header("Dataset")]
    //public Texture2D[] datasets = new Texture2D[0];
    public TextureClass[] datasets = new TextureClass[0];
    private double[] inputs_dataset = new double[0];
    private double[] outputs = new double[0];

    [Header("Prediction")] [Range(0, 1)] public int classId = 0;
    public PredictMode mode = PredictMode.Accuracy;

    #region Callbacks Unity
    private void OnDestroy()
    {
        if (model.Equals(IntPtr.Zero))
            return;

        MLDLLWrapper.DeleteLinearModel(model);
        Debug.Log("Modèle détruit\n");
    }

    #endregion

    #region Machine Learning Functions

    public void CreateModel()
    {
        if (!enabled)
            return;

        if (!model.Equals(IntPtr.Zero))
        {
            Debug.LogError("You trying to created an other model, we delete the old model before");
            MLDLLWrapper.DeleteModel(model);
            Debug.Log("Modèle détruit\n");
        }

        //On vérifie que les input size dans npl[0]
        //soit cohérent avec la tailles des textures
        int isize = TexturesDataset.completeDatasetByClasses[0][0].width * TexturesDataset.completeDatasetByClasses[0][0].height;
        inputSize = isize;

        //On crée notre model
        model = MLDLLWrapper.CreateLinearModel(inputSize);
        Debug.Log("Modèle créé \n");
    }

    public void TrainModel()
    {
        if (!enabled)
            return;

        if (model.Equals(IntPtr.Zero))
        {
            Debug.LogError("You trying to train your model, but it's not created");
            return;
        }

        //On crée notre dataset selon le poucentage que l'on veut utiliser
        //On va prendre autant de texture de chaque classe
        //On cherche d'abord la classe qui va nous donner le moins de texture avec le poucentage voulu
        int texCounts = -1;
        for (int i = 0; i < TextureLoader.Instance.foldersName.Length; i++)
        {
            if (texCounts == -1)
                texCounts = Mathf.RoundToInt(TexturesDataset.completeDatasetByClasses[i].Length * useDatasetAsNPercent);
            else
                texCounts = texCounts > Mathf.RoundToInt(TexturesDataset.completeDatasetByClasses[i].Length * useDatasetAsNPercent)
                    ? Mathf.RoundToInt(TexturesDataset.completeDatasetByClasses[i].Length * useDatasetAsNPercent)
                    : texCounts;
        }

        for (int tr = 0; tr < trainLoopCount; tr++)
        {
            //On crée le tableau de texture avec autant de counts par classe
            datasets = new TextureClass[texCounts * TextureLoader.Instance.foldersName.Length];
            int idx = 0;
            //on remplit le tableau
            for (int i = 0; i < TextureLoader.Instance.foldersName.Length; i++)
            {
                List<int> randomIndex = new List<int>();

                if (!TexturesDataset.unusedDatasetByClasses.ContainsKey(i))
                    TexturesDataset.unusedDatasetByClasses.Add(i, new Texture2D[TexturesDataset.completeDatasetByClasses[i].Length - texCounts]);

                for (int j = 0; j < texCounts; j++)
                {
                    //on tire un index au hasard
                    int rdm = Random.Range(0, TexturesDataset.completeDatasetByClasses[i].Length);
                    int ite = 0;
                    while ((randomIndex.Contains(rdm) && randomIndex.Count >= 1) ||
                           ite >= TexturesDataset.completeDatasetByClasses[i].Length)
                    {
                        rdm = Random.Range(0, TexturesDataset.completeDatasetByClasses[i].Length);
                        ite++;
                    }

                    randomIndex.Add(rdm);

                    //on ajoute la texture
                    datasets[idx] = new TextureClass();
                    datasets[idx].tex = TexturesDataset.completeDatasetByClasses[i][rdm];
                    datasets[idx].classe = i;
                    idx++;
                }

                int tmp = 0;
                for (int j = 0; j < TexturesDataset.completeDatasetByClasses[i].Length; j++)
                {
                    if (randomIndex.Contains(j))
                        continue;

                    TexturesDataset.unusedDatasetByClasses[i][tmp] = TexturesDataset.completeDatasetByClasses[i][j];
                    tmp++;
                }
            }

            //On remplit de double[] array
            inputs_dataset = new double[datasets.Length * inputSize];
            outputs = new double[datasets.Length * outputSize];
            idx = 0;
            int idx_out = 0;
            for (int n = 0; n < datasets.Length; n++)
            {
                for (int i = 0; i < datasets[n].tex.width; i++)
                {
                    for (int j = 0; j < datasets[n].tex.height; j++)
                    {
                        inputs_dataset[idx] = datasets[n].tex.GetPixel(i, j).grayscale;
                        idx++;
                    }
                }

                outputs[idx_out] = datasets[n].classe == 0 ? -1 : 1;
                idx_out++;
            }

            sampleCounts = datasets.Length;

            //Enfin, on entraine notre modèle N fois
            Debug.Log("On entraîne le modèle\n...");
            if(isClassification)
                MLDLLWrapper.TrainLinearModelRosenblatt(model, inputs_dataset, inputSize, sampleCounts, outputs, outputSize, epochs, alpha);
            else
                MLDLLWrapper.TrainLinearModelRegression(model, inputs_dataset, inputSize, sampleCounts, outputs, outputSize);
            Debug.Log("Modèle entrainé \n");
        }
    }

    public void Predict()
    {
        if (!enabled)
            return;
        if (model.Equals(IntPtr.Zero))
        {
            Debug.LogError("You trying to predict these inputs, but your model is not created");
            return;
        }

        Debug.Log("Prediction du dataset !\n");
        double[] inputTmp = new double[inputSize + 1];

        switch (mode)
        {
            case PredictMode.Random:
                int rdm = Random.Range(0, TexturesDataset.unusedDatasetByClasses[classId].Length);
                inputTmp[0] = 1.0;
                for (int i = 0; i < TexturesDataset.unusedDatasetByClasses[classId][rdm].width; i++)
                {
                    for (int j = 0; j < TexturesDataset.unusedDatasetByClasses[classId][rdm].height; j++)
                    {
                        inputTmp[i * TexturesDataset.unusedDatasetByClasses[classId][rdm].width + j + 1] =
                            TexturesDataset.unusedDatasetByClasses[classId][rdm].GetPixel(i, j).grayscale;
                    }
                }

                var result = MLDLLWrapper.PredictLinearModel(model, inputTmp, inputSize, isClassification);
                Debug.LogWarning("Prediction : " + result.ToString("0.00") + " -- classe = " +
                                 TextureLoader.Instance.foldersName[result < 0 ? 0 : 1]);
                break;

            case PredictMode.Accuracy:
                float finalAccuracy = 0.0f;
                for (int n = 0; n < TextureLoader.Instance.foldersName.Length; n++)
                {
                    float accuracy = 0.0f;
                    
                    Debug.LogError("On commence à prédire la classe " + TextureLoader.Instance.foldersName[n]);
                    for (int t = 0; t < TexturesDataset.unusedDatasetByClasses[n].Length; t++)
                    {
                        inputTmp[0] = 1.0;
                        
                        for (int i = 0; i < TexturesDataset.unusedDatasetByClasses[n][t].width; i++)
                        {
                            for (int j = 0; j < TexturesDataset.unusedDatasetByClasses[n][t].height; j++)
                            {
                                inputTmp[i * TexturesDataset.unusedDatasetByClasses[n][t].width + j + 1] =
                                    TexturesDataset.unusedDatasetByClasses[n][t].GetPixel(i, j).grayscale;
                            }
                        }

                        var res = MLDLLWrapper.PredictLinearModel(model, inputTmp, inputSize, isClassification);
                        Debug.LogWarning("Prediction : " + res.ToString("0.00") + " -- classe = " +
                                         TextureLoader.Instance.foldersName[res < 0 ? 0 : 1]);

                        accuracy += (TextureLoader.Instance.foldersName[res < 0 ? 0 : 1].Equals(TextureLoader.Instance.foldersName[n]))
                            ? Mathf.Abs((float)res)
                            : 0.0f;
                    }

                    accuracy /= TexturesDataset.unusedDatasetByClasses[n].Length;
                    finalAccuracy += accuracy;
                    
                    Debug.LogWarning(string.Format("L'accuracy de la classe {0} est de {1}", TextureLoader.Instance.foldersName[n], accuracy));
                }

                finalAccuracy /= TextureLoader.Instance.foldersName.Length;
                Debug.LogWarning(string.Format("L'accuracy total est de {0}", finalAccuracy));
                break;
        }
    }

    #endregion

}