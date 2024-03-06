using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


using TMPro;


public class CollectableControl : MonoBehaviour
{
    public static int coinCount;
    public TMP_Text messageText;


    void Update()
    {
        messageText.SetText( "" + coinCount);
    }
}
