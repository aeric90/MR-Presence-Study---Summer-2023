using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticipantUIController : MonoBehaviour
{
    public TMPro.TextMeshProUGUI idTextUI;
    private string idText = "";

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        idTextUI.text = idText;
    }

    public void UIButtonPush(string value)
    {
        idText += value;
    }

    public void UIClearPush()
    {
        if(idText.Length < 2) idText = "";
    }

    public void UIEnterPush()
    {
        experimentController.instance.UIStart(idText);
    }
}
