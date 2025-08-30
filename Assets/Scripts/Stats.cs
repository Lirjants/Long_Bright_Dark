using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour


{
    [Header("Player Stats")]
    public float maxHealth = 100f;
    public float maxSanity = 100f;
    public float maxElectricity = 100f;
    public float maxFood = 100f;



    private float currentHealth;
    private float currentSanity;
    private float currentElectricity;
    private float currentFood;


    public enum ElectricityUse
    {
        High,
        Medium,
        Low,
        None
    }

    public enum FoodUse
    {
        High,
        Normal,
        Low,
        None
    }



    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        currentSanity = maxSanity;
        currentElectricity = maxElectricity;
        currentFood = maxFood;
    }

    // Update is called once per frame
    void Update()
    {

    }



   

    public void ModifyHealth(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        Debug.Log("Health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void ModifySanity(float amount)
    {
        currentSanity = Mathf.Clamp(currentSanity + amount, 0, maxSanity);
        Debug.Log("Sanity: " + currentSanity);
    }

    public void ModifyElectricity(float amount)
    {
        currentElectricity = Mathf.Clamp(currentElectricity + amount, 0, maxElectricity);
        Debug.Log("Electricity: " + currentElectricity);
    }

    public void ModifyFood(float amount)
    {
        currentFood = Mathf.Clamp(currentFood + amount, 0, maxFood);
        Debug.Log("Food: " + currentFood);
    }

    // Getters
    public float GetHealth() => currentHealth;
    public float GetSanity() => currentSanity;
    public float GetElectricity() => currentElectricity;
    public float GetFood() => currentFood;




    public void Die()
    {
        Debug.Log("Player Died");
    }
}



