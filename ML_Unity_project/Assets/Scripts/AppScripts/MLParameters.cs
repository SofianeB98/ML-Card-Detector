using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MLParameters : MonoBehaviour
{
    public static int SampleCounts = 4;
    public static int Epochs = 1000;
    public static double Alpha = 0.01;
    public static bool IsClassification = true;
    public static int Input_size = 0;
    public static int Output_size = 0;
    
    public static int[] NPL = new int[0];

    public static int TrainLoopCount = 1;
    public static float UseDatasetAsNPercent = 0.5f;
    
    // ------------------------------------------------------------------------------------
    
    [Header("ML Parameter")] 
    public int sampleCounts = 4;
    public int epochs = 1000;
    public double alpha = 0.01;
    public bool isClassification = true;
    public int input_size = 0;
    public int output_size = 0;
    
    [Header("PMC Parameter")] 
    public int[] npl = new int[0];
    
    [Header("Train Parameter")] 
    public int trainLoopCount = 1;
    [Range(0.1f, 1.0f)] public float useDatasetAsNPercent = 0.5f;
}
