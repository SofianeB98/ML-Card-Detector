using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public static class MLDLLWrapper
{
    // -----------------------------------------------------------------------------------------------------------------
    // Pas besoins de mettre l'extention à MLDLL
    [DllImport("MLDLL", EntryPoint = "my_add")]
    public static extern double MyAdd(double a, double b);
    // -----------------------------------------------------------------------------------------------------------------

    #region Linear Functions
    [DllImport("MLDLL", EntryPoint = "create_linear_model")]
    public static extern System.IntPtr CreateLinearModel(int input_counts);
    
    [DllImport("MLDLL", EntryPoint = "create_linear_model_regression")]
    public static extern System.IntPtr CreateLinearModelRegression(int input_counts);
    
    [DllImport("MLDLL", EntryPoint = "predict_linear_model_multiclass")]
    public static extern System.IntPtr PredictLinearModelMulticlass(System.IntPtr[] model, double[] inputs, int input_count, int class_count, bool is_classification);
    
    [DllImport("MLDLL", EntryPoint = "predict_linear_model")]
    public static extern double PredictLinearModel(System.IntPtr model, double[] inputs, int inputSize, bool is_classification);
    
    [DllImport("MLDLL", EntryPoint = "train_linear_model_regression")]
    public static extern void TrainLinearModelRegression(System.IntPtr model, double[] all_inputs, int inputSize, int sample_counts, double[] all_expected_outputs, int expected_output_size);
    
    [DllImport("MLDLL", EntryPoint = "train_linear_model_rosenblatt")]
    public static extern void TrainLinearModelRosenblatt(System.IntPtr model, double[] all_inputs, int inputSize, int sample_counts, double[] all_expected_outputs, int expected_output_size, int epochs, double learning_rate);
    
    [DllImport("MLDLL", EntryPoint = "setLinearWeightValueAt")]
    public static extern double SetWeightValueAt(System.IntPtr model, int j, double val);
    
    [DllImport("MLDLL", EntryPoint = "delete_linear_model")]
    public static extern void DeleteLinearModel(System.IntPtr model);
    #endregion

    #region MLP Functions
    [DllImport("MLDLL", EntryPoint = "create_model")]
    public static extern System.IntPtr CreateModel(int[] npl, int layer_counts);
    
    [DllImport("MLDLL", EntryPoint = "predict")]
    public static extern System.IntPtr Predict(System.IntPtr model, double[] inputs, bool isClass);

    [DllImport("MLDLL", EntryPoint = "train")]
    public static extern void Train(System.IntPtr model, double[] allInputs, double[] allExpectedOutputs,
    int sampleCount, int epochs, double alpha, bool isClassification);

    [DllImport("MLDLL", EntryPoint = "getWeightValueAt")]
    public static extern double GetWeightValueAt(System.IntPtr model, int l, int i, int j);
    
    [DllImport("MLDLL", EntryPoint = "setWeightValueAt")]
    public static extern double SetWeightValueAt(System.IntPtr model, int l, int i, int j, double val);
    
    [DllImport("MLDLL", EntryPoint = "delete_model")]
    public static extern void DeleteModel(System.IntPtr model);
    #endregion

    [DllImport("MLDLL", EntryPoint = "delete_double_array_ptr")]
    public static extern void DeleteDoubleArrayPtr(System.IntPtr ptr);

    #region RBF Functions

    [DllImport("MLDLL", EntryPoint = "create_rbf_model")]
    public static extern System.IntPtr CreateRBFModel(int k, double gamma);

    [DllImport("MLDLL", EntryPoint = "train_rbf_model")]
    public static extern void TrainRBFModel(System.IntPtr model, double[] all_inputs, int input_count, int sample_counts, double[] all_expected_outputs, int expected_output_count);

    [DllImport("MLDLL", EntryPoint = "predict_rbf")]
    public static extern double PredictRBF(System.IntPtr model, double[] inputs, int inputSize, int output_size, bool isClassification);

    [DllImport("MLDLL", EntryPoint = "predict_rbf_and_get_array")]
    public static extern System.IntPtr PredictRBFAndGetArray(System.IntPtr model, double[] inputs, int inputSize, int output_size, bool isClassification);

    [DllImport("MLDLL", EntryPoint = "delete_rbf_model")]
    public static extern double DeleteRBFModel(System.IntPtr model);
    
    #endregion
}
