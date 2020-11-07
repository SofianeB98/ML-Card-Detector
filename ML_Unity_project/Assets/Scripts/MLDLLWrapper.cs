﻿using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public static class MLDLLWrapper
{
    // -----------------------------------------------------------------------------------------------------------------
    
    //Pas besoins de mettre l'extention
    [DllImport("MLDLL", EntryPoint = "my_add")]
    public static extern double MyAdd(double a, double b);

    // -----------------------------------------------------------------------------------------------------------------
    
    [DllImport("MLDLL", EntryPoint = "create_linear_model")]
    public static extern System.IntPtr CreateLinearModel(int input_counts);
    
    [DllImport("MLDLL", EntryPoint = "predict_linear_model_multiclass")]
    public static extern System.IntPtr PredictLinearModelMulticlass(System.IntPtr model, double[] inputs, int input_count, int class_count, bool is_classification);
    
    [DllImport("MLDLL", EntryPoint = "predict_linear_model")]
    public static extern double PredictLinearModel(System.IntPtr model, double[] inputs, int input_count, int sample_count, bool is_classification);
    
    [DllImport("MLDLL", EntryPoint = "train_linear_model_regression")]
    public static extern void TrainLinearModelRegression(System.IntPtr model, double[] all_inputs, int input_count, int sample_counts, double[] all_expected_outputs, int expected_output_count);
    
    [DllImport("MLDLL", EntryPoint = "train_linear_model_rosenblatt")]
    public static extern void  TrainLinearModelRosenblatt(System.IntPtr model, double[] all_inputs, int input_count, int sample_counts, double[] all_expected_outputs, int expected_output_count, int epochs, double learning_rate);
    
    [DllImport("MLDLL", EntryPoint = "delete_linear_model")]
    public static extern void DeleteLinearModel(System.IntPtr model);
    
    // -----------------------------------------------------------------------------------------------------------------
    
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
    
    // -----------------------------------------------------------------------------------------------------------------
}
