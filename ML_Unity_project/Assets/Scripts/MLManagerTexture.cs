using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MLManagerTexture : MonoBehaviour
{
    [Serializable]
    public struct TextureClass
    {
        public Texture2D tex;
        public int classe; //index qui renvoi vers le nom du folder
    }
    
    private static MLManagerTexture instance;
    public static MLManagerTexture Instance
    {
        get { return instance; }
    }

    public bool createModelOnStart = true;
    private System.IntPtr model;


    [Header("ML Parameter")] 
    public int[] npl = new int[0];
    [Space(10)] public int sampleCounts = 4;
    public int epochs = 10000;
    public double alpha = 0.01;
    public bool isClassification = true;
    private int input_size = 0;
    private int output_size = 0;


    [Header("Train Parameter")] public int trainLoopCount = 1;
    [Range(0.1f, 1.0f)] public float useDatasetAsNPercent = 0.5f;


    [Header("Dataset")] 
    public TextureClass[] datasets = new TextureClass[0];
    private double[] inputs_dataset = new double[0];
    private double[] outputs = new double[0];


    [Header("Texture Loading")] public string mainFolder = "";
    public string[] foldersName = new string[0];

    #region Callbacks Unity
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this);
    }

    private void Start()
    {
        if (!createModelOnStart)
            return;
        
        CreateModel();
    }

    private void OnDestroy()
    {
        if (!enabled)
            return;
        if (model.Equals(IntPtr.Zero))
            return;

        MLDLLWrapper.DeleteModel(model);
        Debug.Log("Modèle détruit\n");
    }
    #endregion

    #region Machine Learning Functions
    public void CreateModel()
    {
        if (!enabled)
            return;

        if (!model.Equals(IntPtr.Zero))
        {
            Debug.LogError("You trying to created an other model, we delete the old model before");
            MLDLLWrapper.DeleteModel(model);
            Debug.Log("Modèle détruit\n");
        }
        
        LoadAllTextures();

        int isize = datasets[0].tex.width * datasets[0].tex.height;
        if (npl.Length > 0)
        {
            if (!npl[0].Equals(isize))
                npl[0] = isize;
            
            input_size = npl[0];
            output_size = npl[npl.Length - 1];
        }
        else
        {
            npl = new[] {isize, 1};
            input_size = npl[0];
            output_size = npl[npl.Length - 1];
        }
        
        model = MLDLLWrapper.CreateModel(npl, npl.Length);
        Debug.Log("Modèle créé \n");
        
    }

    public void TrainModel()
    {
        if (!enabled)
            return;

        if (model.Equals(IntPtr.Zero))
        {
            Debug.LogError("You trying to train your model, but it's not created");
            return;
        }

        Debug.Log("On entraîne le modèle\n...");
        for (int i = 0; i < trainLoopCount; i++)
            MLDLLWrapper.Train(model, inputs_dataset, outputs, sampleCounts, epochs, alpha, isClassification);
        Debug.Log("Modèle entrainé \n");
    }

    public void Predict()
    {
        if (!enabled)
            return;
        if (model.Equals(IntPtr.Zero))
        {
            Debug.LogError("You trying to predict these inputs, but your model is not created");
            return;
        }

        Debug.Log("Prediction du dataset !\n");
        double[] inputTmp = new double[input_size];
        for (int i = 0; i < datasets[0].tex.width; i++)
        {
            for (int j = 0; j < datasets[0].tex.height; j++)
                inputTmp[i] = datasets[0].tex.GetPixel(i, j).grayscale;
        }
        var result = MLDLLWrapper.Predict(model, inputTmp, isClassification);
        double[] r = new double[output_size + 1];
        System.Runtime.InteropServices.Marshal.Copy(result, r, 0, output_size + 1);
        Debug.LogWarning("Prediction : " + foldersName[Mathf.RoundToInt((float)(r[1] + 1.0 * 0.5))]);
        MLDLLWrapper.DeleteDoubleArrayPtr(result);
    }
    #endregion

    #region Loading des textures
    public int GetFilesNumber()
    {
        int nb = 0;

        for (int i = 0; i < foldersName.Length; i++)
        {
            string tmp = Path.Combine(mainFolder, foldersName[i]);
            if (mainFolder.EndsWith("/") || mainFolder.EndsWith("'\'"))
                tmp = mainFolder + foldersName[i];

            nb += Directory.GetFiles(tmp).Length;
        }

        Debug.LogWarning("Il y a " + nb + " images !");

        return nb;
    }

    public void LoadAllTextures()
    {
        datasets = new TextureClass[GetFilesNumber()];

        int idx = 0;
        for (int i = 0; i < foldersName.Length; i++)
        {
            string tmp = Path.Combine(mainFolder, foldersName[i]);
            if (mainFolder.EndsWith("/") || mainFolder.EndsWith("'\'"))
                tmp = mainFolder + foldersName[i];

            string[] allTexturesOnFolder = Directory.GetFiles(tmp);

            for (int j = 0; j < allTexturesOnFolder.Length; j++)
            {
                Texture2D t = new Texture2D(1, 1);
                t.LoadImage(File.ReadAllBytes(allTexturesOnFolder[i]));
                
                datasets[idx] = new TextureClass()
                {
                    tex = t,
                    classe = i
                };
                
                idx++;
            }
        }

        sampleCounts = datasets.Length;
        
        Debug.LogWarning("Toutes les textures ont été chargées !");
    }
    #endregion

    private void ShuffleArray()
    {
        
    }
}