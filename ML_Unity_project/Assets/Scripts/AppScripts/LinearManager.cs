using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class LinearManager : MonoBehaviour
{
    private static LinearManager instance;
    public static LinearManager Instance => LinearManager.instance;
    private int classCount = 1;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this);
    }

    #region Machine Learning Functions

    public void CreateModel()
    {
        if (!this.enabled || !this.gameObject.activeInHierarchy)
            return;

        //On vérifie que les input size dans npl[0]
        //soit cohérent avec la tailles des textures
        int isize = TexturesDataset.completeDatasetByClasses[0][0].width *
                    TexturesDataset.completeDatasetByClasses[0][0].height;

        classCount = TexturesDataset.completeDatasetByClasses.Keys.Count;

        MLParameters.Input_size = isize;
        MLParameters.Output_size = 1;
        MLParameters.models = new IntPtr[classCount];

        //On crée notre model
        for (int i = 0; i < classCount; i++)
        {
            MLParameters.models[i] = MLDLLWrapper.CreateLinearModel(MLParameters.Input_size);
        }

        Debug.Log("Modèle créé \n");
    }

    public void TrainModel()
    {
        if (!enabled || !this.gameObject.activeInHierarchy)
            return;

        if (MLParameters.models.Length <= 0)
        {
            Debug.LogError("You trying to train your model, but it's not created");
            return;
        }

        //On crée notre dataset selon le poucentage que l'on veut utiliser
        //On va prendre autant de texture de chaque classe
        //On cherche d'abord la classe qui va nous donner le moins de texture avec le poucentage voulu
        TextureClass[] datasets = new TextureClass[0];

        int texCounts = -1;
        for (int i = 0; i < TextureLoader.Instance.foldersName.Length; i++)
        {
            if (texCounts == -1)
                texCounts = Mathf.RoundToInt(TexturesDataset.completeDatasetByClasses[i].Length *
                                             MLParameters.UseDatasetAsNPercent);
            else
                texCounts = texCounts > Mathf.RoundToInt(TexturesDataset.completeDatasetByClasses[i].Length *
                                                         MLParameters.UseDatasetAsNPercent)
                    ? Mathf.RoundToInt(TexturesDataset.completeDatasetByClasses[i].Length *
                                       MLParameters.UseDatasetAsNPercent)
                    : texCounts;
        }

        double[] inputs_dataset = new double[0];
        double[] outputs = new double[0];

        for (int c = 0; c < classCount; c++)
        {
            for (int tr = 0; tr < MLParameters.TrainLoopCount; tr++)
            {
                //On crée le tableau de texture avec autant de counts par classe
                datasets = new TextureClass[texCounts * TextureLoader.Instance.foldersName.Length];
                int idx = 0;
                //on remplit le tableau
                for (int i = 0; i < TextureLoader.Instance.foldersName.Length; i++)
                {
                    List<int> randomIndex = new List<int>();

                    if (!TexturesDataset.unusedDatasetByClasses.ContainsKey(i))
                        TexturesDataset.unusedDatasetByClasses.Add(i,
                            new Texture2D[TexturesDataset.completeDatasetByClasses[i].Length - texCounts]);

                    for (int j = 0; j < texCounts; j++)
                    {
                        //on tire un index au hasard
                        int rdm = Random.Range(0, TexturesDataset.completeDatasetByClasses[i].Length);
                        int ite = 0;
                        while ((randomIndex.Contains(rdm) && randomIndex.Count >= 1) ||
                               ite >= TexturesDataset.completeDatasetByClasses[i].Length)
                        {
                            rdm = Random.Range(0, TexturesDataset.completeDatasetByClasses[i].Length);
                            ite++;
                        }

                        randomIndex.Add(rdm);

                        //on ajoute la texture
                        datasets[idx] = new TextureClass();
                        datasets[idx].tex = TexturesDataset.completeDatasetByClasses[i][rdm];
                        datasets[idx].classe = i;
                        idx++;
                    }
                }

                //On remplit de double[] array
                inputs_dataset = new double[datasets.Length * MLParameters.Input_size];
                outputs = new double[datasets.Length * MLParameters.Output_size];
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

                    outputs[idx_out] = datasets[n].classe == c ? 1 : -1;
                    idx_out++;
                }

                MLParameters.SampleCounts = datasets.Length;

                //Enfin, on entraine notre modèle N fois
                Debug.Log("On entraîne le modèle\n...");
                MLDLLWrapper.TrainLinearModelRosenblatt(MLParameters.models[c], inputs_dataset, MLParameters.Input_size,MLParameters.SampleCounts,
                    outputs, MLParameters.Output_size, MLParameters.Epochs, MLParameters.Alpha);
                Debug.Log("Modèle entrainé \n");
            }
        }
    }

    public void Predict(Texture2D card, out double accuracy, out string pred)
    {
        accuracy = -1.0;
        pred = "-";
        if (!enabled || !this.gameObject.activeInHierarchy)
            return;

        if (MLParameters.models.Length <= 0)
        {
            Debug.LogError("You trying to predict these inputs, but your model is not created");
            return;
        }

        Debug.Log("Prediction de la carte selectionnée !\n");
        double[] inputTmp = new double[MLParameters.Input_size + 1];

        inputTmp[0] = 1.0;
        for (int i = 0; i < card.width; i++)
        {
            for (int j = 0; j < card.height; j++)
            {
                inputTmp[i * card.width + j + 1] = card.GetPixel(i, j).grayscale;
            }
        }

        var res = MLDLLWrapper.PredictLinearModelMulticlass(MLParameters.models, inputTmp,  MLParameters.Input_size, classCount, MLParameters.IsClassification);
        double[] resFromPtr = new double[classCount];
        System.Runtime.InteropServices.Marshal.Copy(res, resFromPtr, 0, classCount);
        
        int foldId = MLParameters.GetIndexOfHigherValueInArray(resFromPtr);

        for (int i = 1; i < resFromPtr.Length; i++)
        {
            Debug.LogWarning($"RESULTAT ==> valeur {i} = {resFromPtr[i].ToString("0.0000")}");
        }

        pred = TextureLoader.Instance.foldersName[foldId];
        accuracy = (float) resFromPtr[foldId];

        Debug.LogWarning($"Prediction : {pred} \n " +
                         $"Pourcentage : {accuracy.ToString("0.000")}");


        MLDLLWrapper.DeleteDoubleArrayPtr(res);
    }

    public void DeleteModel()
    {
        for (int i = 0; i < MLParameters.models.Length; i++)
        {
            MLDLLWrapper.DeleteLinearModel(MLParameters.models[i]);
            MLParameters.models[i] = IntPtr.Zero;
        }

        MLParameters.models = new IntPtr[0];

        Debug.Log("Modèles détruit\n");
    }

    public void SaveModel()
    {
        MultiLinearModel linear = new MultiLinearModel();
        for (int i = 0; i < MLParameters.models.Length; i++)
        {
            ListOfDouble tmp = new ListOfDouble();
            tmp.Wj = new List<double>();
            
            double[] resFromPtr = new double[MLParameters.Input_size + 1];
            System.Runtime.InteropServices.Marshal.Copy(MLParameters.models[i], resFromPtr, 0, resFromPtr.Length);

            tmp.Wj = resFromPtr.ToList();
            linear.models.Add(tmp);
        }
        
        //save
        var str = JsonUtility.ToJson(linear, true);
        var path = Path.Combine(Application.dataPath, "SavedModels");
        path = Path.Combine(path, "pmcCard.json");

        File.WriteAllText(path,str);

        linear.models.Clear();
        
        Debug.Log("Modele sauvegarde !!");
    }

    public void LoadModel(string path)
    {
        var str = File.ReadAllText(path);
        var loadedModel = JsonUtility.FromJson<MultiLinearModel>(str);
        
        if (MLParameters.models.Length <= 0)
        {
            MLParameters.IsClassification = true;
            CreateModel();
        }

        if (MLParameters.models.Length > loadedModel.models.Count)
        {
            DeleteModel();
            MLParameters.models = new IntPtr[loadedModel.models.Count];
            for (int i = 0; i < loadedModel.models.Count; i++)
            {
                MLParameters.models[i] = MLDLLWrapper.CreateLinearModel(MLParameters.Input_size);
            }
        }

        for (int i = 0; i < MLParameters.models.Length; i++)
        {
            for (int j = 0; j < loadedModel.models[i].Wj.Count; j++)
            {
                //set
                MLDLLWrapper.SetWeightValueAt(MLParameters.models[i], j, loadedModel.models[i].Wj[j]);
            }
        }
    }
    #endregion
}