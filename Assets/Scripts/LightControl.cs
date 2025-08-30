using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class LightControl : MonoBehaviour
{
    public List<Light> lights;
    public List<GameObject> spriteContainers;
    private int lightStage;

    // Start is called before the first frame update
    void Start()
    {
        lightStage = 3;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            changeLight();
        }
    }

    public void changeSprite(GameObject spriteContainer)
    {
        for (int i = 0; i < spriteContainer.transform.childCount; i++)
        {
            spriteContainer.gameObject.transform.GetChild(i).gameObject.SetActive(false);
        }
        spriteContainer.gameObject.transform.GetChild(lightStage).gameObject.SetActive(true);
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

        // turn the relevant sprite on for each room item
        for (int i = 0; i < spriteContainers.Count; i++)
        {
            changeSprite(spriteContainers[i]);
        }
    }
}
