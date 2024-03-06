using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


using TMPro;

public class LevelDistance : MonoBehaviour
{
    
    public TMP_Text messageText;
    public int disRun;
    public bool addingDis = false;
    public float disDelay = 0.5f;

    // Update is called once per frame
    void Update()
    {
        if(addingDis == false){
            addingDis = true;
            StartCoroutine(AddingDis());
        }
    }

    IEnumerator AddingDis(){
        disRun +=1;
        messageText.SetText( "" + disRun);
        yield return new WaitForSeconds(disDelay);
        addingDis = false;
    }
}
