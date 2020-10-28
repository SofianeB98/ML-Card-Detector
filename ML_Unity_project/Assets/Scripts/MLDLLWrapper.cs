﻿using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public static class MLDLLWrapper
{
    //Pas besoins de mettre l'extention
    [DllImport("MLDLL", EntryPoint = "my_add")]
    public static extern double MyAdd(double a, double b);
    
    [DllImport("MLDLL", EntryPoint = "my_add_ptr")]
    public static extern double MyAdd(System.IntPtr ptr);
    
    
    [DllImport("MLDLL", EntryPoint = "create_linear_model")]
    public static extern System.IntPtr CreateLinearModel(int input_count);
    
    
    [DllImport("MLDLL", EntryPoint = "predict_linear_model")]
    public static extern double PredictLinearModelClassification(System.IntPtr model, double[] inputs, int input_count, bool is_classification);
    
    
    [DllImport("MLDLL", EntryPoint = "predict_linear_model_multiclass")]
    public static extern System.IntPtr PredictLinearModelMulticlassClassification(System.IntPtr model, double[] inputs, int input_count, int class_count, bool is_classification);

    
    [DllImport("MLDLL", EntryPoint = "delete_linear_model")]
    public static extern void DeleteLinearModel(System.IntPtr model);

    
    [DllImport("MLDLL", EntryPoint = "create_model")]
    public static extern System.IntPtr CreateModel(int[] npl, int hidden_layer_counts);
    
    
    [DllImport("MLDLL", EntryPoint = "delete_model")]
    public static extern void DeleteModel(System.IntPtr model);

}