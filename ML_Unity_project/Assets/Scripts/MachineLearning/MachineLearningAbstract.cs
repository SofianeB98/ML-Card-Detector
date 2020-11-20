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
    protected int input_size = 0;
    protected int output_size = 0;
    
    public abstract void CreateModel();
    public abstract void TrainModel();
    public abstract void Predict();
    public abstract void DeleteModel();
}
