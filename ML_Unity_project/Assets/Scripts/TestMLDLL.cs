using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class TestMLDLL : MonoBehaviour
{
    [Header("ML Parameter")]
    public int epochs = 100;
    public double alpha = 0.01;
    
    private void Start()
    {
        Debug.LogWarning("Creation du model");
        var model = MLDLLWrapper.CreateLinearModel(3);

        var inputs = new double[]
        {
            1, 1,
            2, 3,
            3, 3
        };
        
        Debug.LogWarning("Lancement du training !");
        MLDLLWrapper.TrainLinearModelRosenblatt(model, inputs, 2, 3, new double[]
        {
            1, -1, -1
        }, 1, epochs, alpha);
        Debug.LogWarning("Training compléter !");

        for (int i = 0; i < 6; i += 2)
        {
            Debug.LogWarning("Echantillon : " + inputs[i] + " :: " + inputs[i+1]);
            
            var fwrd = MLDLLWrapper.PredictLinearModel(model, new double[] {1.0, inputs[i], inputs[i+1]}, 2, true);
            Debug.LogWarning("\nPrediction = " + fwrd);
        }
        
        MLDLLWrapper.DeleteLinearModel(model);
        Debug.LogWarning("Modele detruit ! !");
    }
    
}