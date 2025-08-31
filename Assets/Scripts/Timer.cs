using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Stats;
using static Textbox;


public class Timer : MonoBehaviour
{
    
    public int currentDay = 0;
    public int survivalDaysToWin = 14;
    private bool isGameOver = false;

    private int delay = 4000;


    [Header("Intro Event")]
    public GameEvent introEvent;


    [Header("Daily Effects")]
    public float healthLossPerDay = -5f;
    public float sanityLossPerDay = -10f;
    public float electricityLossPerDay = -5f;


    public Stats playerStats;
    public Projects _projects;
    public MusicManager music;
    public Textbox _textbox;

    private DayPhase currentPhase = DayPhase.StartOfDay;



    [Header("UI References")]
    public Button exerciseButton;
    public Button farmButton;
    public Button projectButton;
    public TextMeshProUGUI eventText;
    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI endGameText;


    public string aa = "";





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
        {
            ApplyDailyConsumption();
            Debug.Log("Consumed");


        }
        if (playerStats.IsDead)
        {
            TriggerGameOver();
            return;
        }

        if (currentDay == 1 && introEvent != null)
        {
            Debug.Log("Intro Event triggered!");
            introEvent.ApplyEvent(playerStats, _projects);
            //eventText.text = introEvent.description;
            _textbox.DisplayText(introEvent.description);
            currentPhase = DayPhase.FreeTimePhase;
            UpdateUIButtons();
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
        music.StopMusic();
        
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
        chosenEvent.ApplyEvent(playerStats, _projects);
        //eventText.text = chosenEvent.description;
        _textbox.DisplayText(chosenEvent.description);
    }

    public void DoExercise()
    {
        if (currentPhase != DayPhase.FreeTimePhase) return;

        if (currentPhase != DayPhase.FreeTimePhase) return;
        playerStats.ModifySanity(+5f);
        Debug.Log("You exercised and feel a bit better.");
        _textbox.DisplayText("You exercised and feel a bit better.");
        EndDay();

    }

    public void DoFarm()
    {
        if (currentPhase != DayPhase.FreeTimePhase) return;

        if (currentPhase != DayPhase.FreeTimePhase) return;
        playerStats.ModifyFood(+5f);
        Debug.Log("You farmed and got some food.");
        _textbox.DisplayText("You farmed and got some food");
        EndDay();

    }


       public async void DoElectricalProject() // <-- Changed to async void
    {
    if (currentPhase != DayPhase.FreeTimePhase) return;

    // First, disable buttons so the player can't click again while the delay runs

    if (_projects.farmingProgress >= 100f)
    {
        _textbox.DisplayText("The electrical project is already complete.");
    }
    else if (_projects != null)
    {
        // I'm assuming your method returns the new progress percentage.
        // Displaying just a number isn't very descriptive. Let's make a better message.
        float newProgress = _projects.farmingProject(_projects.incrementamount);
        _textbox.DisplayText($"You worked on the electrical project. Progress is now {newProgress}%.");
        Debug.Log($"You worked on electrical project. Progress: {newProgress}%");
    }

    // Wait for 2 seconds (2000 milliseconds) so the player can read the text
    await Task.Delay(delay);

    // Now, end the day
    EndDay();
}
    public async void DoFarmingProject() // <-- Changed to async void
    {
        if (currentPhase != DayPhase.FreeTimePhase) return;

        // First, disable buttons so the player can't click again while the delay runs

        if (_projects.farmingProgress >= 100f)
        {
            _textbox.DisplayText("The farming project is already complete.");
        }
        else if (_projects != null)
        {
            // I'm assuming your method returns the new progress percentage.
            // Displaying just a number isn't very descriptive. Let's make a better message.
            float newProgress = _projects.farmingProject(_projects.incrementamount);
            _textbox.DisplayText($"You worked on the farming project. Progress is now {newProgress}%.");
            Debug.Log($"You worked on farming project. Progress: {newProgress}%");
        }

        // Wait for 2 seconds (2000 milliseconds) so the player can read the text
        await Task.Delay(delay);

        // Now, end the day
        EndDay();
    }
        public async void DoScienceProject() // <-- Changed to async void
    {
        if (currentPhase != DayPhase.FreeTimePhase) return;

        // First, disable buttons so the player can't click again while the delay runs

        if (_projects.farmingProgress >= 100f)
        {
            _textbox.DisplayText("The science project is already complete.");
        }
        else if (_projects != null)
        {
            // I'm assuming your method returns the new progress percentage.
            // Displaying just a number isn't very descriptive. Let's make a better message.
            float newProgress = _projects.farmingProject(_projects.incrementamount);
            _textbox.DisplayText($"You worked on the science project. Progress is now {newProgress}%.");
            Debug.Log($"You worked on science project. Progress: {newProgress}%");
        }

        // Wait for 2 seconds (2000 milliseconds) so the player can read the text
        await Task.Delay(delay);

        // Now, end the day
        EndDay();
    }

    private void UpdateUIButtons()
    {
        if (exerciseButton != null) exerciseButton.interactable = (currentPhase == DayPhase.FreeTimePhase);
        if (farmButton != null) farmButton.interactable = (currentPhase == DayPhase.FreeTimePhase);
        if (projectButton != null) projectButton.interactable = (currentPhase == DayPhase.FreeTimePhase);
    }


    public void SetElectricityHigh() { electricityUse = ElectricityUse.High; Debug.Log("Electricity use set to HIGH"); }
    public void SetElectricityMedium() { electricityUse = ElectricityUse.Medium; Debug.Log("Electricity use set to MEDIUM"); }
    public void SetElectricityLow() { electricityUse = ElectricityUse.Low; Debug.Log("Electricity use set to LOW"); }
    public void SetElectricityNone() { electricityUse = ElectricityUse.None; Debug.Log("Electricity use set to NONE"); }

    public void SetFoodHigh() { foodUse = FoodUse.High; Debug.Log("Food use set to HIGH"); }
    public void SetFoodNormal() { foodUse = FoodUse.Normal; Debug.Log("Food use set to NORMAL"); }
    public void SetFoodLow() { foodUse = FoodUse.Low; Debug.Log("Food use set to LOW"); }
    public void SetFoodNone() { foodUse = FoodUse.None; Debug.Log("Food use set to NONE"); }

    
}
