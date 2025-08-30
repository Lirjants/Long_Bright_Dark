using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Stats;

public class Timer : MonoBehaviour
{
    public int currentDay = 0;
    public int survivalDaysToWin = 14;
    private bool isGameOver = false;


    [Header("Daily Effects")]
    public float healthLossPerDay = -5f;
    public float sanityLossPerDay = -10f;
    public float electricityLossPerDay = -5f;


    public Stats playerStats;
    public Projects _projects;

    private DayPhase currentPhase = DayPhase.StartOfDay;



    [Header("UI References")]
    public Button exerciseButton;
    public Button farmButton;
    public Button projectButton;
    public TextMeshProUGUI eventText;
    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI endGameText;







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
        _projects = FindObjectOfType<Projects>();

        BeginNextDay();

       


    }

    [Header("Player Choices")]
    public ElectricityUse electricityUse = ElectricityUse.Medium;
    public FoodUse foodUse = FoodUse.Normal;



    private void BeginNextDay()
    {

        if (isGameOver) return;
        if (currentDay > survivalDaysToWin)
        {
            TriggerVictory();
            return;
        }
        currentPhase = DayPhase.StartOfDay;

        Debug.Log($"Day {currentDay} begins!");

        // Only apply daily consumption if currentDay > 1
        if (currentDay > 1)
            ApplyDailyConsumption();
        if (playerStats.IsDead)
        {
            TriggerGameOver();
            return;
        }

        TriggerEvent();

        currentPhase = DayPhase.FreeTimePhase;
        Debug.Log("Choose your free time activity...");

        UpdateUIButtons();
    }

    private void TriggerVictory()
    {
        isGameOver = true;
        Debug.Log("YOU SURVIVED! Victory!");
        if (endGameText != null)
            endGameText.text = "YOU SURVIVED!\nYou lasted until rescue arrived.";
        UpdateUIButtons(); // Disable buttons
    }

    private void TriggerGameOver()
    {
        isGameOver = true;
        Debug.Log("Game Over! You died.");
        if (gameOverText != null)
            gameOverText.text = "GAME OVER\nYou did not survive.";
        UpdateUIButtons(); // Disable buttons
    }

    public void EndDay() // called after free-time activity
    {
        currentPhase = DayPhase.DayComplete;
        Debug.Log($"Day {currentDay} is complete.");
        currentDay++;
        BeginNextDay(); // immediately start next day
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

        if (playerStats.IsDead)
        {
            TriggerGameOver();
            return;
        }
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
        eventText.text = chosenEvent.description;   
    }

    public void DoExercise()
    {
        if (currentPhase != DayPhase.FreeTimePhase) return;

        if (currentPhase != DayPhase.FreeTimePhase) return;
        playerStats.ModifySanity(+5f);
        Debug.Log("You exercised and feel a bit better.");
        EndDay();

    }

    public void DoFarm()
    {
        if (currentPhase != DayPhase.FreeTimePhase) return;

        if (currentPhase != DayPhase.FreeTimePhase) return;
        playerStats.ModifyFood(+5f);
        Debug.Log("You farmed and got some food.");
        EndDay();

    }

    public void DoElectricalProject()
    {
        if (currentPhase != DayPhase.FreeTimePhase) return;
        if (_projects.electricalProgress >= 100f)
        {
            Debug.Log("Electrical project already completed.");
            return;
        }

        else if (_projects != null)
        {
            _projects.electricalProject(30f);
        }
        Debug.Log("You finished your electrical project and got some electricity.");
        EndDay();

    }
    public void DoFarmingProject()
    {
        if (currentPhase != DayPhase.FreeTimePhase) return;
        if (_projects.farmingProgress >= 100f)
        {
            Debug.Log("Farming project already completed.");
            return;
        }

        else if (_projects != null) 
        {
            _projects.farmingProject(30f);
        }
        Debug.Log("You worked on farming project.");
        EndDay();
    }

    public void DoScienceProject()
    {
        if (currentPhase != DayPhase.FreeTimePhase) return;
        if (_projects.scienceProgress >= 100f)
        {
            Debug.Log("Science project already completed.");
            return;
        }
        else  { _projects?.scienceProject(30f); }
        Debug.Log("You worked on the science project.");
        EndDay();
    }
  

    private void UpdateUIButtons()
    {
        if (exerciseButton != null) exerciseButton.interactable = (currentPhase == DayPhase.FreeTimePhase);
        if (farmButton != null) farmButton.interactable = (currentPhase == DayPhase.FreeTimePhase);
        if (projectButton != null) projectButton.interactable = (currentPhase == DayPhase.FreeTimePhase);
    }


}
