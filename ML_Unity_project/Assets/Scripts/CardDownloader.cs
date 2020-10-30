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
    public int cardCount = 4981;
    public string outPath = "";
    public string folderName = "Magic";

    private string url = "http://gatherer.wizards.com/Handlers/Image.ashx?multiverseid={0}&type=card";
    
    private string finalPath = "";
    private string cardName = "";
    
    [Header("Resize Parameter")]
    public Vector2Int resizeTo = new Vector2Int(256, 256);
    public Color backgroundColor = Color.white;
    
    
    private void Start()
    {
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
        
        if (canDownload)
            StartCoroutine(CardDownload(finalPath));
    }

    private IEnumerator CardDownload(string path)
    {
        Debug.LogWarning("On va telecharger les images dans : " + finalPath);

        for (int i = 0; i < cardCount; i++)
        {
            string cName = cardName + i.ToString("0000");
            string realUrl = string.Format(url, i);
            using (UnityWebRequest request = UnityWebRequest.Get(realUrl))
            {
                Debug.LogWarning("Lancement de la request");
                // Send the request and wait for a response
                yield return request.SendWebRequest();
                string pathReal = Path.Combine(finalPath, cName);
                
                
                
                if (!File.Exists(pathReal))
                {
                    var resizedImg = ResizePicture(request.downloadHandler.data);
                    File.WriteAllBytes(pathReal + ".jpg", resizedImg);
                    Debug.Log("Fichier telechargé au nom de : " + cName);
                }
            }
        }
        
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
