using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MachineLearningAbstract : MonoBehaviour
{
    protected System.IntPtr model;

    [Header("ML Parameter")] 
    public int sampleCounts = 4;
    public int epochs = 1000;
    public double alpha = 0.01;
    public bool isClassification = true;
    public int input_size = 0;
    public int output_size = 0;
    
    [Header("Train Parameter")] 
    public int trainLoopCount = 1;
    [Range(0.1f, 1.0f)] public float useDatasetAsNPercent = 0.5f;
    
    public abstract void CreateModel();
    public abstract void TrainModel();
    public abstract void Predict();
    public abstract void DeleteModel();
}
