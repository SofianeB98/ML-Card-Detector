using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TextureLoader : MonoBehaviour
{
    private static TextureLoader instance;
    public static TextureLoader Instance => instance;
    
    [Header("Texture Loading")] 
    public string mainFolder = "";
    public string[] foldersName = new string[0];
 
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this);
    }
    
    private void Start()
    {
        LoadAllTextures();
    }

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
            if (!TexturesDataset.completeDatasetByClasses.ContainsKey(i))
                TexturesDataset.completeDatasetByClasses.Add(i, new Texture2D[allTexturesOnFolder.Length]);

            //on load
            for (int j = 0; j < allTexturesOnFolder.Length; j++)
            {
                Texture2D t = new Texture2D(1, 1);
                t.LoadImage(File.ReadAllBytes(allTexturesOnFolder[j]));

                TexturesDataset.completeDatasetByClasses[i][j] = t;
            }
        }

        Debug.LogWarning("Toutes les textures ont été chargées !");
    }

    #endregion
    
}
