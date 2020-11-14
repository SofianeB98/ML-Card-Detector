using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class CardDownloader : MonoBehaviour
{
    [Header("Download Parameter")] 
    public bool canDownload = true;
    public int cardCount = 295;
    public string outPath = "";
    public string folderName = "Magic";
    public bool useDic = false;
    
    public Dictionary<string, int> pokemonDownloadDic = new Dictionary<string, int>();

    //private string url = "https://images.pokemontcg.io/{1}/{0}.png"; //permet de dl les carte pokemon";
    public string url = "http://gatherer.wizards.com/Handlers/Image.ashx?multiverseid={0}&type=card";
    
    private string finalPath = "";
    private string cardName = "";
    private int startId = 0;
    
    [Header("Resize Parameter")]
    public Vector2Int resizeTo = new Vector2Int(256, 256);
    public Color backgroundColor = Color.white;
    
    
    private void Start()
    {
        pokemonDownloadDic.Add("base1", 100);
        pokemonDownloadDic.Add("base4", 100);
        pokemonDownloadDic.Add("ex4", 97);
        pokemonDownloadDic.Add("dp6", 100);
        pokemonDownloadDic.Add("xy1", 100);
        
        if (string.IsNullOrEmpty(outPath))
            outPath = Application.persistentDataPath;

        if (string.IsNullOrEmpty(folderName))
            folderName = "Cards";

        cardName = folderName + "_";
        finalPath = Path.Combine(outPath, folderName);

        if (!Directory.Exists(finalPath))
        {
            Directory.CreateDirectory(finalPath);
        }
        else
        {
            if (Directory.GetFiles(finalPath).Length >= cardCount)
                return;
            else
                startId = cardCount - (cardCount - Directory.GetFiles(finalPath).Length);
        }
        
        if (canDownload)
            StartCoroutine(CardDownload(useDic));
    }

    private IEnumerator CardDownload(bool useDic = false)
    {
        Debug.LogWarning("On va telecharger les images dans : " + finalPath);
        if (!useDic)
        {
            for (int i = startId; i < cardCount; i++)
            {
                string cName = cardName + i.ToString("0000");
                string realUrl = string.Format(url, i);
                using (UnityWebRequest request = UnityWebRequest.Get(realUrl))
                {
                    //Debug.LogWarning("Lancement de la request");
                    // Send the request and wait for a response
                    yield return request.SendWebRequest();
                    string pathReal = Path.Combine(finalPath, cName);
                
                    if (!File.Exists(pathReal))
                    {
                        var resizedImg = ResizePicture(request.downloadHandler.data);
                        File.WriteAllBytes(pathReal + ".jpg", resizedImg);
                        //Debug.Log("Fichier telechargé au nom de : " + cName);
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning("use dic version");
            int carNb = 0;
            int start = 0;
            foreach (var key in pokemonDownloadDic.Keys)
            {
                var v = pokemonDownloadDic[key];
                
                for (int i = start; i < v; i++)
                {
                    string cName = cardName + carNb.ToString("0000");
                    carNb++;
                    
                    string realUrl = string.Format(url, i, key);
                    using (UnityWebRequest request = UnityWebRequest.Get(realUrl))
                    {
                        //Debug.LogWarning("Lancement de la request");
                        // Send the request and wait for a response
                        yield return request.SendWebRequest();
                        string pathReal = Path.Combine(finalPath, cName);
                
                        if (!File.Exists(pathReal))
                        {
                            var resizedImg = ResizePicture(request.downloadHandler.data);
                            File.WriteAllBytes(pathReal + ".jpg", resizedImg);
                            //Debug.Log("Fichier telechargé au nom de : " + cName);
                        }
                    }
                }

                start = 1;
            }
            
        }
        
        Debug.LogWarning("DOWNLOAD FINI !");
        
        yield break;
    }

    private byte[] ResizePicture(byte[] b)
    {
        Texture2D baseImg = new Texture2D(0, 0);
        baseImg.LoadImage(b);
        float ratio = (float)baseImg.width / (float)baseImg.height;
        baseImg.Apply();
        
        TextureScale.Bilinear(baseImg, (int)(resizeTo.x * ratio), resizeTo.y);
        
        Color[] colors = baseImg.GetPixels(0, 0, baseImg.width, baseImg.height);
        
        Texture2D finalImg = new Texture2D(resizeTo.x, resizeTo.y);
        var c = finalImg.GetPixels();
        for (int i = 0; i < c.Length; i++)
        {
            c[i] = backgroundColor;
        }
        finalImg.SetPixels(c);
        finalImg.Apply();
        
        float x = (float)(resizeTo.x - baseImg.width) * 0.5f;
        finalImg.SetPixels((int)x, 0, baseImg.width, resizeTo.y, colors);

        return finalImg.EncodeToJPG();
    }
}
