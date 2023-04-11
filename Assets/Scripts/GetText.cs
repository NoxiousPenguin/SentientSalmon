using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO; 
using System.Linq;

public class GetText : MonoBehaviour
{
    public Transform contextWindow;

    public GameObject recallTextObject; 


    // Start is called before the first frame update
    void Start()
    {

        string readFromFilePath = Application.streamingAssetsPath +"/stat_logs/" + "Evaluation - TestMapScene" + ".txt";

        string[] filePaths = Directory.GetFiles(Application.streamingAssetsPath +"/stat_logs/");
        filePaths.Reverse(); 
        foreach(string file in filePaths.Reverse()){
                 
            if(!file.Contains("meta")){//skip meta files

                Debug.Log(file); 
                List<string> fileLines = File.ReadAllLines(file).ToList(); 

                //fileLines.Sort(); 
                //fileLines.Reverse();
                foreach(string line in fileLines)
                {
                    Instantiate(recallTextObject, contextWindow);

                    recallTextObject.GetComponent<Text>().text = line; 
                
                //recallTextObject.GetComponent<Text>().text = line; 
                //recallTextObject.GetComponent<Text>().text = line; 

                }
        
            }
        
        
        }


       

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
