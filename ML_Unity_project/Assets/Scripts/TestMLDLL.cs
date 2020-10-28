using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class TestMLDLL : MonoBehaviour
{
    private void Start()
    {
        Debug.Log( MLDLLWrapper.MyAdd(2, 3) + " the result of my C code dll");

        var model = MLDLLWrapper.CreateModel(new int[] {2, 2, 1}, 3);
        
        Debug.Log( MLDLLWrapper.MyAdd(model) + " the result");

        MLDLLWrapper.DeleteModel(model);
    }

    private void Update()
    {
        var model = MLDLLWrapper.CreateModel(new int[] {2, 2, 1}, 3);
        
        Debug.Log( MLDLLWrapper.MyAdd(model) + " the result");

        MLDLLWrapper.DeleteModel(model);
    }
}