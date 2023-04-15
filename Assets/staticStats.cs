using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System.Linq;

public class staticStats : MonoBehaviour
{

    public Transform contextWindow;
    public GameObject recallTextObject; 

    public List<string> statsList; 

    static string stats; 


    public void EndofRun(){
        statsList.Insert(0 , stats);
        stats = "";
    }

    public static void addStats(string statsToAdd){
        stats+= statsToAdd; 
        Debug.Log(statsToAdd); 
    }

    public void printStats(){
        Debug.Log(stats); 



        string[] fileLines = stats.Split(new char[] { '\r', '\n' },System.StringSplitOptions.RemoveEmptyEntries ); 

        foreach(string line in fileLines)
        {
        Instantiate(recallTextObject, contextWindow);
        recallTextObject.GetComponent<Text>().text = line;
        }
    }


    public void addStatsToDisplay(){
        Instantiate(recallTextObject, contextWindow);
        recallTextObject.GetComponent<Text>().text = stats;
    }



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
