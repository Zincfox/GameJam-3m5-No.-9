using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerBtnHandler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI actionTextField;
    
    public void OnRollActionsButtonClick()
    {
        if (actionTextField.text == "")
        {
            actionTextField.text = Random.Range(1, 5).ToString();
        }
        else
        {
            Debug.Log("NOPE");
        }
    }
}