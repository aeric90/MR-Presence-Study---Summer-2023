using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class buttonController : MonoBehaviour
{
    public static buttonController instance;

    public Renderer buttonRenderer;
    public TMPro.TextMeshPro buttonText;
    public Material buttonOnMaterial;
    public Material buttonOffMaterial;
    public Material buttonEndMaterial;
    private bool buttonActive = false;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ButtonPushed()
    {
        if(buttonActive)
        {
            experimentController.instance.buttonPush();
        }
    }

    public void SetActive(bool status)
    {
        buttonActive = status;
        if(status)
        {
            buttonRenderer.material = buttonOnMaterial;
            buttonText.text = "Next Trial";
        }
        else
        {
            buttonRenderer.material = buttonOffMaterial;
            buttonText.text = "N/A";
        }
    }

    public void SetEnd()
    {
        buttonActive = false;
        buttonRenderer.material = buttonEndMaterial;
        buttonText.text = "END";
    }
}
