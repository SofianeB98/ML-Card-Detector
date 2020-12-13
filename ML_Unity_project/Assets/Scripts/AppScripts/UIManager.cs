using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using SFB;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static ActiveMachineLearningEnum selectedModel = ActiveMachineLearningEnum.NONE;

    [Header("ML UI Elements")] 
    public Dropdown selectedModelDD;
    [Space(5)]
    public InputField epochsField;
    public InputField alphaField;
    public InputField trainCountField;
    public InputField usedDataSetField;
    public InputField nplField;

    [Header("Utils UI Element")] 
    public Image selectedCard;
    private Texture2D textureSelected;
    public Text accuracyText;
    public Text answerText;
    public Text messageText;
    
    [Header("Load Card Parameter")]
    public Vector2Int resizeTo = new Vector2Int(256, 256);
    public Color backgroundColor = Color.white;
    
    public void UpdateSelectedModel(int model)
    {
        //Je delete l'ancien model
        
        //Je set la selection
        selectedModel = (ActiveMachineLearningEnum) model;
        
        //J'active le npl uniquement si c'est un PMC
        nplField.transform.parent.gameObject.SetActive(selectedModel == ActiveMachineLearningEnum.PMC_MODEL);
        
        //J'effectuer les action necessaire
        switch (selectedModel)
        {
            case ActiveMachineLearningEnum.NONE:
                Debug.LogWarning("Aucun modèle c'est selectionné");
                break;
            
            case ActiveMachineLearningEnum.LINEAR_MODEL:
                Debug.LogWarning("Le modèle linéaire est selectionné");
                //Do something
                
                break;
            
            case ActiveMachineLearningEnum.PMC_MODEL:
                Debug.LogWarning("Le modèle pmc est selectionné");
                //do something
                
                break;
            
            case ActiveMachineLearningEnum.RBF_MODEL:
                Debug.LogWarning("Le modèle rbf est selectionné");
                //Do something
                
                break;
            
            default:
                break;
        }
    }

    #region Input Field Setter

    public void SetEpochs(string val)
    {
        int integerValue = int.Parse(val);
        MLParameters.Epochs = integerValue;

        Debug.Log($"Epochs a été set à {integerValue}");
    }

    public void SetAlpha(string val)
    {
        double doubleValue = double.Parse(val);
        MLParameters.Alpha = doubleValue;

        Debug.Log($"Alpha a été set à {doubleValue}");
    }
    
    public void SetTrainCount(string val)
    {
        int integerValue = int.Parse(val);
        MLParameters.TrainLoopCount = integerValue;

        Debug.Log($"Traincount a été set à {integerValue}");
    }
    
    public void SetUsedDataset(string val)
    {
        float floatValue = float.Parse(val);
        MLParameters.UseDatasetAsNPercent = floatValue;

        Debug.Log($"%Used Dataset a été set à {floatValue}");
    }
    
    public void SetNPL(string val)
    {
        var splittedVal = val.Split(',');
        int[] tmpNpl = new int[splittedVal.Length];
        for (int i = 0; i < tmpNpl.Length; i++)
        {
            tmpNpl[i] = int.Parse(splittedVal[i]);
        }
        
        MLParameters.NPL = new int[tmpNpl.Length];
        Array.Copy(tmpNpl, MLParameters.NPL, tmpNpl.Length);

        Debug.Log($"NPL a été set à {tmpNpl}");
    }
    #endregion
    
    #region ML Functions

    public void CreateModel()
    {
        switch (selectedModel)
        {
            case ActiveMachineLearningEnum.NONE:
                break;
            
            case ActiveMachineLearningEnum.LINEAR_MODEL:
                break;
            
            case ActiveMachineLearningEnum.PMC_MODEL:
                break;
            
            case ActiveMachineLearningEnum.RBF_MODEL:
                break;
     
            
            default:
                break;
        }
    }
    
    public void TrainModel()
    {
        switch (selectedModel)
        {
            case ActiveMachineLearningEnum.NONE:
                break;
            
            case ActiveMachineLearningEnum.LINEAR_MODEL:
                break;
            
            case ActiveMachineLearningEnum.PMC_MODEL:
                break;
            
            case ActiveMachineLearningEnum.RBF_MODEL:
                break;
     
            
            default:
                break;
        }
    }
    
    public void PredictSelection()
    {
        var imgResized = CardDownloader.ResizePicture(textureSelected.EncodeToJPG(), resizeTo, backgroundColor);
        Texture2D texResized = new Texture2D(0,0);
        texResized.LoadImage(imgResized);
        
        switch (selectedModel)
        {
            case ActiveMachineLearningEnum.NONE:
                break;
            
            case ActiveMachineLearningEnum.LINEAR_MODEL:
                break;
            
            case ActiveMachineLearningEnum.PMC_MODEL:
                break;
            
            case ActiveMachineLearningEnum.RBF_MODEL:
                break;
     
            
            default:
                break;
        }
    }

    public void DeleteModel()
    {
        switch (selectedModel)
        {
            case ActiveMachineLearningEnum.NONE:
                break;
            
            case ActiveMachineLearningEnum.LINEAR_MODEL:
                break;
            
            case ActiveMachineLearningEnum.PMC_MODEL:
                break;
            
            case ActiveMachineLearningEnum.RBF_MODEL:
                break;
     
            
            default:
                break;
        }
    }

    #endregion

    #region Load & Save Model Functions

    public void SaveModel()
    {
        switch (selectedModel)
        {
            case ActiveMachineLearningEnum.NONE:
                break;
            
            case ActiveMachineLearningEnum.LINEAR_MODEL:
                break;
            
            case ActiveMachineLearningEnum.PMC_MODEL:
                break;
            
            case ActiveMachineLearningEnum.RBF_MODEL:
                break;
     
            
            default:
                break;
        }
    }
    
    public void LoadModel()
    {
        switch (selectedModel)
        {
            case ActiveMachineLearningEnum.NONE:
                break;
            
            case ActiveMachineLearningEnum.LINEAR_MODEL:
                break;
            
            case ActiveMachineLearningEnum.PMC_MODEL:
                break;
            
            case ActiveMachineLearningEnum.RBF_MODEL:
                break;
     
            
            default:
                break;
        }
    }

    #endregion
    
    public void OpenBrowserToChooseCard()
    {
        var extensions = new [] {
            new ExtensionFilter("Image Files", "png", "jpg", "jpeg" )
        };
        var path = StandaloneFileBrowser.OpenFilePanel("Choose a card", "", extensions, false);

        if (path.Length <= 0)
            return;
        
        Debug.Log($"Selected path = {path[0]}"); 
        textureSelected = new Texture2D(0,0);
        textureSelected.LoadImage(File.ReadAllBytes(path[0]));

        selectedCard.sprite = Sprite.Create(textureSelected, new Rect(0, 0, textureSelected.width, textureSelected.height), Vector2.one * 0.5f);
    }
}
