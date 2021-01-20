using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RBFMLManager : MachineLearningAbstract
{ 
    [Header("Dataset")]
    public TextureClass[] datasets = new TextureClass[0];
    private double[] inputs_dataset = new double[0];
    private double[] outputs = new double[0];
    public int k = 1;
    
    #region Callbacks Unity
    private void OnDestroy()
    {
        DeleteModel();
    }

    #endregion
    
    public override void CreateModel()
    {
        if (!enabled)
            return;
        
        if (!model.Equals(IntPtr.Zero))
        {
            Debug.LogError("You trying to created an other model, we delete the old model before");
            MLDLLWrapper.DeleteModel(model);
            Debug.Log("Modèle détruit\n");
        }

        k = TexturesDataset.completeDatasetByClasses.Keys.Count;
        
        if (isClassification)
            model = MLDLLWrapper.CreateLinearModel(k);
        Debug.Log("Modèle créé \n");
        
        //On initialise les inputs
        int isize = TexturesDataset.completeDatasetByClasses[0][0].width * TexturesDataset.completeDatasetByClasses[0][0].height;
        input_size = isize;

        //TODO : a quoi correspond l'output size ? 
        output_size = 1;
    }

    public override void TrainModel()
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
        for (int i = 0; i < TextureLoader.Instance.foldersName.Length; i++)
        {
            if (texCounts == -1)
                texCounts = Mathf.RoundToInt(TexturesDataset.completeDatasetByClasses[i].Length * useDatasetAsNPercent);
            else
                texCounts = texCounts > Mathf.RoundToInt(TexturesDataset.completeDatasetByClasses[i].Length * useDatasetAsNPercent)
                    ? Mathf.RoundToInt(TexturesDataset.completeDatasetByClasses[i].Length * useDatasetAsNPercent)
                    : texCounts;
        }
        
        //On crée le tableau de texture avec autant de counts par classe
        datasets = new TextureClass[texCounts * TextureLoader.Instance.foldersName.Length];
        int idx = 0;
        //on remplit le tableau
        for (int i = 0; i < TextureLoader.Instance.foldersName.Length; i++)
        {
            List<int> randomIndex = new List<int>();

            if (!TexturesDataset.unusedDatasetByClasses.ContainsKey(i))
                TexturesDataset.unusedDatasetByClasses.Add(i, new Texture2D[TexturesDataset.completeDatasetByClasses[i].Length - texCounts]);

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

            int tmp = 0;
            for (int j = 0; j < TexturesDataset.completeDatasetByClasses[i].Length; j++)
            {
                if (randomIndex.Contains(j))
                    continue;

                TexturesDataset.unusedDatasetByClasses[i][tmp] = TexturesDataset.completeDatasetByClasses[i][j];
                tmp++;
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

            //TODO : je sais pas quoi mettre en input :/
            outputs[idx_out] = datasets[n].classe;
            idx_out++;
        }
        
        Debug.Log("On entraîne le modèle\n...");
        MLDLLWrapper.TrainRBFModel(this.model, inputs_dataset, this.input_size, inputs_dataset.Length, outputs, output_size, k, alpha);
        Debug.Log("Modèle entrainé \n");
    }

    public override void Predict()
    {
        //TODO : On predit comment pour du Classif ? Et pour du Regression ? 
    }

    public override void DeleteModel()
    {
        if (model.Equals(IntPtr.Zero))
            return;
        
        MLDLLWrapper.DeleteLinearModel(model);
        model = IntPtr.Zero;
        Debug.Log("Modèle détruit\n");
    }
}
