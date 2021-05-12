using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;


[Serializable]
public class QRCodeEntry
{
    int type;
    string content;
}



[Serializable]
public class QRCodeEntryCollection
{
    public QRCodeEntry[] entries;
}



public class QRCodeFetchData : MonoBehaviour
{
    Action<string> qrcodecallback;

    public JSONObject QRCodeInfoJson { get; private set; }

    public List<string> QRCodeText = new List<string>();

    public string cardText;
    public string imageURL;
    public string htmlDIV;

    public Material testMat;
    public Texture2D mainTex;

    void Start()
    {
        qrcodecallback = (jsonArrayString) =>
        {
            StartCoroutine(CreateQRCodeContent(jsonArrayString));
        };

        StartCoroutine(GetJSONRequest("https://arprototypeappservice.azurewebsites.net/api/card/a,b",
                                  qrcodecallback));
    }



    IEnumerator GetJSONRequest(string uri, System.Action<string> callback)
    {
        UnityWebRequest www = UnityWebRequest.Get(uri);
        yield return www.SendWebRequest();

        if(www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            // Show results as text
            Debug.Log(www.downloadHandler.text);

            string jsonArray = www.downloadHandler.text;
            

            callback(jsonArray);
        }
    }

    IEnumerator CreateQRCodeContent(string jsonArrayString)
    {
        // Parsing json array string as an array
        JSONArray jsonArray = JSON.Parse(jsonArrayString) as JSONArray;

        for(int i = 0; i < jsonArray.Count; i++)
        {
            QRCodeInfoJson = new JSONObject();
            QRCodeInfoJson = jsonArray[i].AsObject;

            QRCodeText.Add(QRCodeInfoJson["content"]);
        }

        cardText = QRCodeText[0];
        imageURL = QRCodeText[1];
        htmlDIV = QRCodeText[2];

        StartCoroutine(DownloadImage(imageURL));

        yield return null;
    }

    IEnumerator DownloadImage(string mediaUrl)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(mediaUrl);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log(request.error);
        }
        else
        {
            mainTex = ((DownloadHandlerTexture)request.downloadHandler).texture;
            testMat.mainTexture = mainTex;
        }


    }

    /*void PrintJSON(string print)
    {
        Debug.Log(print);
    }*/
}