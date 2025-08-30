using UnityEngine;
using TMPro;


[CreateAssetMenu(fileName = "NewEvent", menuName = "LongBrightDark/Event")]
public class GameEvent : ScriptableObject
{
    [TextArea] public string description;

    [Header("Effects")]
    public float healthChange;
    public float sanityChange;
    public float electricityChange;
    

  



    [Header("Requirements")]
    public float minHealth = 0f;
    public float maxHealth = 999f;
    public float minSanity = 0f;
    public float maxSanity = 999f;
    public float minElectricity = 0f;
    public float maxElectricity = 999f;



    public bool RequirementsMet(Stats player)
    {
        return
            player.GetHealth() >= minHealth && player.GetHealth() <= maxHealth &&
            player.GetSanity() >= minSanity && player.GetSanity() <= maxSanity &&
            player.GetElectricity() >= minElectricity && player.GetElectricity() <= maxElectricity;
    }


    public void ApplyEvent(Stats player)
    {
        if (player != null)
        {
            player.ModifyHealth(healthChange);
            player.ModifySanity(sanityChange);
            player.ModifyElectricity(electricityChange);

        }
    }
}
