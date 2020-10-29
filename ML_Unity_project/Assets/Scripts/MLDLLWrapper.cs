using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public static class MLDLLWrapper
{
    //Pas besoins de mettre l'extention
    [DllImport("MLDLL", EntryPoint = "my_add")]
    public static extern double MyAdd(double a, double b);

    
    
    [DllImport("MLDLL", EntryPoint = "create_model")]
    public static extern System.IntPtr CreateModel(int[] npl, int layer_counts);
    
    [DllImport("MLDLL", EntryPoint = "predict")]
    public static extern System.IntPtr Predict(System.IntPtr model, double[] inputs, bool isClass);

    [DllImport("MLDLL", EntryPoint = "train")]
    public static extern void Train(System.IntPtr model, double[] allInputs, double[] allExpectedOutputs,
    int sampleCount, int epochs, double alpha, bool isClassification);

    [DllImport("MLDLL", EntryPoint = "delete_model")]
    public static extern void DeleteModel(System.IntPtr model);

    [DllImport("MLDLL", EntryPoint = "delete_double_array_ptr")]
    public static extern void DeleteDoubleArrayPtr(System.IntPtr ptr);
}
