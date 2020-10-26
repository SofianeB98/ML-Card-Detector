using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class TestMLDLL : MonoBehaviour
{
    private void Start()
    {
        Debug.Log( MLDLLWrapper.MyAdd(2, 3) + " the result of my C code dll");

        var model = MLDLLWrapper.CreateLinearModel(10);

        var inputs = new double[] {1.0, 2.0, 3.0};
        var result = MLDLLWrapper.PredictLinearModelMulticlassClassification(model, inputs, 3, 3);

        var result_managed = new double[3];
        //Permet de copier le contenue d'un Double* dans un double[] du c++ vers le c#
        Marshal.Copy(result, result_managed, 0, 3);
        
        //a changer avec u ntruc clean
        //car delete linear model est censé juste delete les model
        MLDLLWrapper.DeleteLinearModel(result);
        
        MLDLLWrapper.DeleteLinearModel(model);
    }
}