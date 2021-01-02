using System;
using UnityEngine;

public class MLParameters : MonoBehaviour
{
    public static IntPtr model = IntPtr.Zero;
    
    public static IntPtr[] models = new IntPtr[0];

    public static int SampleCounts = 4;
    public static int Epochs = 1000;
    public static double Alpha = 0.05;
    public static bool IsClassification = true;
    public static int Input_size = 0;
    public static int Output_size = 0;
    
    public static int[] NPL = new int[]{1, 32, 3};

    public static int TrainLoopCount = 1;
    public static float UseDatasetAsNPercent = 0.8f;
    
    public static int GetIndexOfHigherValueInArray(double[] ar)
    {
        int idx = 0;
        double val = 0;

        for (int i = 1; i < ar.Length; i++)
        {
            if(ar[i] < val)
                continue;

            idx = i;
            val = ar[i];
        }

        return idx - 1;
    }
}
