using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Stats;

public class Timer : MonoBehaviour
{
    public int currentDay = 1;

    [Header("Daily Effects")]
    public float healthLossPerDay = -5f;
    public float sanityLossPerDay = -10f;
    public float electricityLossPerDay = -5f;


    public Stats playerStats;
    private DayPhase currentPhase = DayPhase.StartOfDay;



    public enum FreeTimeActivity
    {
        Exercise,
        Farm,
        Project
    }

    public enum DayPhase
    {
        StartOfDay,
        EventPhase,
        FreeTimePhase,
        DayComplete
    }

    [Header("Events")]
    public List<GameEvent> possibleEvents = new List<GameEvent>();
    void Start()
    {
        playerStats = FindObjectOfType<Stats>();

    }

    [Header("Player Choices")]
    public ElectricityUse electricityUse = ElectricityUse.Medium;
    public FoodUse foodUse = FoodUse.Normal;

    public void EndDay()
    {
        if (currentPhase != DayPhase.StartOfDay) return; // only valid at start
        currentDay++;
        Debug.Log("Day " + currentDay + " begins!");

        // Step 1: Apply consumption
        ApplyDailyConsumption();

        // Step 2: Trigger daily event
        TriggerEvent();

        // Move into free-time phase
        currentPhase = DayPhase.FreeTimePhase;
        Debug.Log("Choose your free time activity...");
    }


    private void ApplyDailyConsumption()
    {
        // ELECTRICITY
        switch (electricityUse)
        {
            case ElectricityUse.High:
                if (playerStats.GetElectricity() >= 20) // must have enough
                {
                    playerStats.ModifyElectricity(-20f);
                    playerStats.ModifySanity(+2f); // calmer with more light
                }
                else
                {
                    Debug.Log("Not enough electricity for High use!");
                }
                break;

            case ElectricityUse.Medium:
                if (playerStats.GetElectricity() >= 10)
                {
                    playerStats.ModifyElectricity(-10f);
                    // normal sanity effect (no change)
                }
                else
                {
                    Debug.Log("Not enough electricity for Medium use!");
                }
                break;

            case ElectricityUse.Low:
                if (playerStats.GetElectricity() >= 5)
                {
                    playerStats.ModifyElectricity(-5f);
                    playerStats.ModifySanity(-3f); // more afraid in the dark
                }
                else
                {
                    Debug.Log("Not enough electricity for Low use!");
                }
                break;

            case ElectricityUse.None:
                // no electricity consumed
                playerStats.ModifySanity(-7f); // very stressed without light
                break;
        }

        // FOOD
        switch (foodUse)
        {
            case FoodUse.High:
                if (playerStats.GetFood() >= 20)
                {
                    playerStats.ModifyFood(-20f);
                    playerStats.ModifyHealth(0f); // no health loss
                    playerStats.ModifySanity(+3f); // feeling better when well-fed
                }
                else
                {
                    Debug.Log("Not enough food for High use!");
                }
                break;

            case FoodUse.Normal:
                if (playerStats.GetFood() >= 10)
                {
                    playerStats.ModifyFood(-10f);
                    // no extra effects
                }
                else
                {
                    Debug.Log("Not enough food for Normal use!");
                }
                break;

            case FoodUse.Low:
                if (playerStats.GetFood() >= 5)
                {
                    playerStats.ModifyFood(-5f);
                    playerStats.ModifyHealth(-5f); // minor health loss
                }
                else
                {
                    Debug.Log("Not enough food for Low use!");
                }
                break;

            case FoodUse.None:
                // no food consumed
                playerStats.ModifyHealth(-10f); // moderate health loss
                break;
        }
    }


    private void TriggerEvent()
    {
        // Find all valid events
        List<GameEvent> validEvents = new List<GameEvent>();
        foreach (var e in possibleEvents)
        {
            if (e.RequirementsMet(playerStats))
            {
                validEvents.Add(e);
            }
        }

        if (validEvents.Count == 0)
        {
            Debug.Log("No valid events this day.");
            return;
        }

        // Pick random valid event
        int index = Random.Range(0, validEvents.Count);
        GameEvent chosenEvent = validEvents[index];

        Debug.Log("Event: " + chosenEvent.description);
        chosenEvent.ApplyEvent(playerStats);
    }

    public void DoExercise()
    {
        if (currentPhase != DayPhase.FreeTimePhase) return;
        playerStats.ModifySanity(+5f);
        Debug.Log("You exercised and feel a bit better.");
        FinishDay();
    }

    public void DoFarm()
    {
        if (currentPhase != DayPhase.FreeTimePhase) return;
        playerStats.ModifyFood(+5f);
        Debug.Log("You farmed and got some food.");
        FinishDay();
    }

    public void DoProject()
    {
        if (currentPhase != DayPhase.FreeTimePhase) return;
        Debug.Log("You worked on a project (effect TBD).");
        FinishDay();
    }

    private void FinishDay()
    {
        currentPhase = DayPhase.DayComplete;
        Debug.Log("Day " + currentDay + " is complete.");
        // Reset for next cycle
        currentPhase = DayPhase.StartOfDay;
    }


}
