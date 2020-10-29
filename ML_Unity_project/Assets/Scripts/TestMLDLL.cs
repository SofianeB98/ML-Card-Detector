using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class TestMLDLL : MonoBehaviour
{
    private void Start()
    {
        Debug.Log( MLDLLWrapper.MyAdd(2, 3) + " the result of my C code dll");

        var model = MLDLLWrapper.CreateModel(new int[] {2, 3, 1}, 3);
        
        MLDLLWrapper.Train(model, new double[] {
            0.0, 0.0,
            1.0, 0.0,
            0.0, 1.0,
            1.0, 1.0
        }, new double[] {
            -1.0, 1.0, 1.0, -1.0
        }, 4, 10000, 0.1, true);

        var result = MLDLLWrapper.Predict(model, new double[]
        {
            1.0, 1.0
        }, true);
        
        double[] r = new double[2];
        System.Runtime.InteropServices.Marshal.Copy(result, r, 0, 2);
        Debug.Log(r[1]);
        MLDLLWrapper.DeleteDoubleArrayPtr(result);
        
        result = MLDLLWrapper.Predict(model, new double[]
        {
            0.0, 1.0
        }, true);
        
        r = new double[2];
        System.Runtime.InteropServices.Marshal.Copy(result, r, 0, 2);
        Debug.Log(r[1]);
        MLDLLWrapper.DeleteDoubleArrayPtr(result);
        
        MLDLLWrapper.DeleteModel(model);
    }

}