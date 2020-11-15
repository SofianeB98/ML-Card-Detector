using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

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
    //public Texture2D[] datasets = new Texture2D[0];
    public TextureClass[] datasets = new TextureClass[0];
    public Dictionary<int, Texture2D[]> datasetsByClasses = new Dictionary<int, Texture2D[]>();
    private double[] inputs_dataset = new double[0];
    private double[] outputs = new double[0];

    [Header("Prediction")] 
    [Range(0, 1)] public int classId = 0;

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
        
        //On charge toute les textures
        LoadAllTextures();
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
        
        //On vérifie que les input size dans npl[0]
        //soit cohérent avec la tailles des textures
        int isize = datasetsByClasses[0][0].width * datasetsByClasses[0][0].height;
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
        
        //On crée notre model
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

        //On crée notre dataset selon le poucentage que l'on veut utiliser
        //On va prendre autant de texture de chaque classe
        //On cherche d'abord la classe qui va nous donner le moins de texture avec le poucentage voulu
        int texCounts = -1;
        for (int i = 0; i < foldersName.Length; i++)
        {
            if (texCounts == -1)
                texCounts = Mathf.RoundToInt(datasetsByClasses[i].Length * useDatasetAsNPercent);
            else
                texCounts = texCounts > Mathf.RoundToInt(datasetsByClasses[i].Length * useDatasetAsNPercent)
                    ? Mathf.RoundToInt(datasetsByClasses[i].Length * useDatasetAsNPercent)
                    : texCounts;
        }

        for (int tr = 0; tr < trainLoopCount; tr++)
        {
            //On crée le tableau de texture avec autant de counts par classe
            datasets = new TextureClass[texCounts * foldersName.Length];
            int idx = 0;
            //on remplit le tableau
            for (int i = 0; i < foldersName.Length; i++)
            {
                List<int> randomIndex = new List<int>();
                for (int j = 0; j < texCounts; j++)
                {
                    //on tire un index au hasard
                    int rdm = Random.Range(0, datasetsByClasses[i].Length);
                    int ite = 0;
                    while ((randomIndex.Contains(rdm) && randomIndex.Count >= 1) || ite >= datasetsByClasses[i].Length)
                    {
                        rdm = Random.Range(0, datasetsByClasses[i].Length);
                        ite++;
                    }

                    randomIndex.Add(rdm);

                    //on ajoute la texture
                    datasets[idx] = new TextureClass();
                    datasets[idx].tex = datasetsByClasses[i][rdm];
                    datasets[idx].classe = i;
                    idx++;
                }
            }
        
            //On remplit de double[] array
            inputs_dataset = new double[datasets.Length * input_size];
            outputs = new double[datasets.Length * output_size];
            idx = 0;
            int idx_out = 0;
            for (int n = 0; n < datasets.Length; n++)
            {
                for (int i = 0; i < datasets[n].tex.width; i++)
                {
                    for (int j = 0; j < datasets[n].tex.height; j++)
                    {
                        inputs_dataset[idx] = datasets[n].tex.GetPixel(i, j).grayscale;
                        idx++;
                    }
                }
                outputs[idx_out] = datasets[n].classe == 0 ? -1 : 1;
                idx_out++;
            }

            sampleCounts = datasets.Length;
            
            //Enfin, on entraine notre modèle N fois
            Debug.Log("On entraîne le modèle\n...");
            MLDLLWrapper.Train(model, inputs_dataset, outputs, sampleCounts, epochs, alpha, isClassification);
            Debug.Log("Modèle entrainé \n");
        }
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

        int rdm = Random.Range(0, datasetsByClasses[classId].Length);
        
        for (int i = 0; i < datasetsByClasses[classId][rdm].width; i++)
        {
            for (int j = 0; j < datasetsByClasses[classId][rdm].height; j++)
            {
                inputTmp[i * datasetsByClasses[classId][rdm].width + j] = datasetsByClasses[classId][rdm].GetPixel(i, j).grayscale;
            }
        }
        
        var result = MLDLLWrapper.Predict(model, inputTmp, isClassification);
        double[] r = new double[output_size + 1];
        System.Runtime.InteropServices.Marshal.Copy(result, r, 0, output_size + 1);
        Debug.LogWarning("Prediction : " + r[1]);
        Debug.LogWarning("Prediction : " + foldersName[r[1] < 0 ? 0 : 1]);
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
        //On charge toute les texture et on les stock dans le tableau
        for (int i = 0; i < foldersName.Length; i++)
        { 
            //on prend le chemin des images
            string tmp = Path.Combine(mainFolder, foldersName[i]);
            if (mainFolder.EndsWith("/") || mainFolder.EndsWith("'\'"))
                tmp = mainFolder + foldersName[i];

            //on recupere tout les path
            string[] allTexturesOnFolder = Directory.GetFiles(tmp);

            //on ajoute la key I au dic
            if(!datasetsByClasses.ContainsKey(i))
                datasetsByClasses.Add(i, new Texture2D[allTexturesOnFolder.Length]);
          
            //on load
            for (int j = 0; j < allTexturesOnFolder.Length; j++)
            {
                Texture2D t = new Texture2D(1, 1);
                t.LoadImage(File.ReadAllBytes(allTexturesOnFolder[j]));

                datasetsByClasses[i][j] = t;
            }
        }
        
        Debug.LogWarning("Toutes les textures ont été chargées !");
    }
    #endregion

    private void ShuffleArray()
    {
        
    }
}