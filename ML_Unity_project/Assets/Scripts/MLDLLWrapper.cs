using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public static class MLDLLWrapper
{
    //Pas besoins de mettre l'extention
    [DllImport("MLDLL", EntryPoint = "my_add")]
    public static extern double MyAdd(double a, double b);
    
    [DllImport("MLDLL", EntryPoint = "create_linear_model")]
    public static extern System.IntPtr CreateLinearModel(int input_count);
    
    [DllImport("MLDLL", EntryPoint = "predict_linear_model_classification")]
    public static extern double PredictLinearModelClassification(System.IntPtr model, double[] inputs, int input_count);
    
    [DllImport("MLDLL", EntryPoint = "predict_linear_model_multiclass_classification")]
    public static extern System.IntPtr PredictLinearModelMulticlassClassification(System.IntPtr model, double[] inputs, int input_count, int class_count);

    [DllImport("MLDLL", EntryPoint = "delete_linear_model")]
    public static extern void DeleteLinearModel(System.IntPtr model);
    
}
