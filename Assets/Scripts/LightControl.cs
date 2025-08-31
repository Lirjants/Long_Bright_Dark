using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class LightControl : MonoBehaviour
{
    public List<Light> lights;
    public List<GameObject> spriteContainers;
    private int lightStage;
    private int foodStage;

    public Timer timerScript;

    // Start is called before the first frame update
    void Start()
    {
        lightStage = 3;
        foodStage = 3;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            changeLight();
        }
    }

    public void changeLightSprite(GameObject spriteContainer)
    {
        for (int i = 0; i < spriteContainer.transform.childCount; i++)
        {
            spriteContainer.gameObject.transform.GetChild(i).gameObject.SetActive(false);
        }
        spriteContainer.gameObject.transform.GetChild(lightStage).gameObject.SetActive(true);
    }

    public void changeFoodSprite(GameObject spriteContainer)
    {
        for (int i = 0; i < spriteContainer.transform.childCount; i++)
        {
            spriteContainer.gameObject.transform.GetChild(i).gameObject.SetActive(false);
        }
        spriteContainer.gameObject.transform.GetChild(foodStage).gameObject.SetActive(true);
    }

    public void changeLight()
    {
        lightStage = (lightStage + 1) % 4;
        // turn all light off
        for (int i = 0; i < lights.Count; ++i)
        {
            lights[i].enabled = false;
        }

        // turn as many lights on as the lightStage dictates
        for (int i = 0; i < lightStage; ++i)
        {
            lights[i].enabled = true;
        }

        changeLightSprite(spriteContainers[0]);
        changeLightSprite(spriteContainers[1]);
        changeLightSprite(spriteContainers[2]);

        switch (lightStage)
        {
            case 0:
                timerScript.SetElectricityNone();
                break;
            case 1:
                timerScript.SetElectricityLow();
                break;
            case 2:
                timerScript.SetElectricityMedium();
                break;
            case 3:
                timerScript.SetElectricityHigh();
                break;
            default:
                break;
        }
    }

    public void changeFood()
    {
        foodStage = (foodStage + 1) % 4;

        changeFoodSprite(spriteContainers[3]);

        switch (foodStage)
        {
            case 0:
                timerScript.SetFoodNone();
                break;
            case 1:
                timerScript.SetFoodLow();
                break;
            case 2:
                timerScript.SetFoodNormal();
                break;
            case 3:
                timerScript.SetFoodHigh();
                break;
            default:
                Debug.Log("default food called");
                break;
        }
    }
}
