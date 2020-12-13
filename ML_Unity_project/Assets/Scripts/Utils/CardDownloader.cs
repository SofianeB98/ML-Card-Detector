using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

//Cartes hearthstone
//https://hearthstonejson.com/docs/cards.html
//https://api.hearthstonejson.com/v1/25770/frFR/cards.json
//https://art.hearthstonejson.com/v1/render/latest/enUS/256x/AT_048.png
//
//
//
public class CardDownloader : MonoBehaviour
{
    [Header("Download Parameter")] public bool canDownload = true;
    public int cardCount = 295;
    public string outPath = "";
    public string folderName = "Magic";
    public bool useDic = false;
    public bool isJson = false;

    public Dictionary<string, int> pokemonDownloadDic = new Dictionary<string, int>();

    //private string url = "https://images.pokemontcg.io/{1}/{0}.png"; //permet de dl les carte pokemon";
    public string url = "http://gatherer.wizards.com/Handlers/Image.ashx?multiverseid={0}&type=card";

    private string finalPath = "";
    private string cardName = "";
    private int startId = 0;

    [Header("Resize Parameter")] public Vector2Int resizeTo = new Vector2Int(256, 256);
    public Color backgroundColor = Color.white;

    private void OnValidate()
    {
        if (isJson)
            useDic = false;
        else if (useDic)
            isJson = false;
    }

    private void Start()
    {
        pokemonDownloadDic.Add("base1", 100);
        //pokemonDownloadDic.Add("base4", 100);
        pokemonDownloadDic.Add("ex4", 97);
        pokemonDownloadDic.Add("dp6", 100);
        //pokemonDownloadDic.Add("xy1", 100);

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
            StartCoroutine(CardDownload(useDic, isJson));
    }

    private IEnumerator CardDownload(bool useDic = false, bool fromJson = false)
    {
        Debug.LogWarning("On va telecharger les images dans : " + finalPath);
        if (!fromJson)
        {
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
                            var resizedImg = ResizePicture(request.downloadHandler.data, resizeTo, backgroundColor);
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
                                var resizedImg = ResizePicture(request.downloadHandler.data, resizeTo, backgroundColor);
                                File.WriteAllBytes(pathReal + ".jpg", resizedImg);
                                //Debug.Log("Fichier telechargé au nom de : " + cName);
                            }
                        }
                    }

                    start = 1;
                }
            }
        }
        else
        {
            var path = Path.Combine(Application.dataPath, "ygocards.json");
            CarteData cartes = new CarteData();
            var txt = File.ReadAllText(path);
            cartes = JsonUtility.FromJson<CarteData>(txt);

            url = "https://storage.googleapis.com/ygoprodeck.com/pics/{0}.jpg";
            
            for (int i = 0; i < cardCount; i++)
            {
                string cName = cardName + i.ToString("0000");
                string realUrl = string.Format(url, cartes.data[i].id);
                using (UnityWebRequest request = UnityWebRequest.Get(realUrl))
                {
                    //Debug.LogWarning("Lancement de la request");
                    // Send the request and wait for a response
                    yield return request.SendWebRequest();
                    string pathReal = Path.Combine(finalPath, cName);

                    if (!File.Exists(pathReal))
                    {
                        var resizedImg = ResizePicture(request.downloadHandler.data, resizeTo, backgroundColor);
                        File.WriteAllBytes(pathReal + ".jpg", resizedImg);
                        //Debug.Log("Fichier telechargé au nom de : " + cName);
                    }
                }
                yield return new WaitForSeconds(0.06f);
            }
        }


        Debug.LogWarning("DOWNLOAD FINI !");

        yield break;
    }

    
    public static byte[] ResizePicture(byte[] b, Vector2Int resizeTo, Color backgroundColor)
    {
        Texture2D baseImg = new Texture2D(0, 0);
        baseImg.LoadImage(b);
        float ratio = (float) baseImg.width / (float) baseImg.height;
        baseImg.Apply();

        TextureScale.Bilinear(baseImg, (int) (resizeTo.x * ratio), resizeTo.y);

        Color[] colors = baseImg.GetPixels(0, 0, baseImg.width, baseImg.height);

        Texture2D finalImg = new Texture2D(resizeTo.x, resizeTo.y);
        var c = finalImg.GetPixels();
        for (int i = 0; i < c.Length; i++)
        {
            c[i] = backgroundColor;
        }

        finalImg.SetPixels(c);
        finalImg.Apply();

        float x = (float) (resizeTo.x - baseImg.width) * 0.5f;
        finalImg.SetPixels((int) x, 0, baseImg.width, resizeTo.y, colors);

        return finalImg.EncodeToJPG();
    }
}

public class CardSet
{
    public string set_name, set_code, set_rarity, set_rarity_code, set_price;
}

public class CardImage
{
    public int id;
    public string image_url, image_url_small;
}

public class CardPrices
{
    public string cardmarket_price, tcgplayer_price, ebay_price, amazon_price, coolstuffinc_price;
}

[System.Serializable]
public class Carte
{
    public int id;
    public string name, type, desc;
    public int atk, def, level;
    public string race, attribute;
    public CardSet[] card_sets;
    public CardImage[] card_images;
    public CardPrices[] card_prices;
}

[System.Serializable]
public class CarteData
{
    public Carte[] data;
}