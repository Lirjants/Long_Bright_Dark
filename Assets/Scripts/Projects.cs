using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static Textbox;
public class Projects : MonoBehaviour
{
    public float incrementamount = 25f;
    public float maxamount = 100f;
    public float scienceProgress = 0f;
    public float electricalProgress = 0f;
    public float farmingProgress = 0f;
    public float current = 0f;
    
    public string aa = "";
    public Stats playerStats;
    public Textbox _textbox;
    
    // Project names
    public string scienceProjectName = "Research Paper";
    public string electricalProjectName = "Stabilize Power Grid";
    public string farmingProjectName = "Greenhouse Upgrade";

    [System.Serializable]
    private class ProjectState
    {
        public string name;
        public float progress;
        public bool at25;
        public bool at50;
        public bool at75;
        public bool complete;
    }

    private Dictionary<projectprogress, ProjectState> projectStates = new Dictionary<projectprogress, ProjectState>();


    public enum projectprogress
    {
        science,
        electrical,
        farming
    }



    public enum scienceprojecttextstring
    {
        conductexperiments,
        analyzedata,
        writereport
    }
    public static readonly Dictionary<scienceprojecttextstring, List<string>> ScienceProjectTexts = new Dictionary<scienceprojecttextstring, List<string>>
    {
        { scienceprojecttextstring.conductexperiments, new List<string>
            {
                "Conduct experiments: Initial tests show promise. (+Sanity)",
                "Conduct experiments: Data collection underway. (+Sanity)",
                "Conduct experiments: Results are encouraging. (+Sanity, +Electricity)",
                "Conduct experiments: Breakthrough achieved! Prototype developed. (+Max Electricity)"
            }
        },
        { scienceprojecttextstring.analyzedata, new List<string>
            {
                "Analyze data: Preliminary analysis completed. (+Sanity)",
                "Analyze data: Patterns emerging in results. (+Sanity)",
                "Analyze data: Insights leading to improvements. (+Sanity, +Electricity)",
                "Analyze data: Comprehensive understanding achieved. (+Max Electricity)"
            }
        },
        { scienceprojecttextstring.writereport, new List<string>
            {
                "Write report: Draft version completed. (+Sanity)",
                "Write report: Peer review underway. (+Sanity)",
                "Write report: Final revisions made. (+Sanity, +Electricity)",
                "Write report: Publication accepted! Research recognized. (+Max Electricity)"
            }
        }
    };

    public enum  farmingprojecttextstring
    {
        addfertilizer,  
        plantseeds,
        waterplants,
        harvestcrops,
    }

    public static readonly Dictionary<farmingprojecttextstring, List<string>> FarmingProjectTexts = new Dictionary<farmingprojecttextstring, List<string>>
    {
        { farmingprojecttextstring.addfertilizer, new List<string>
            {
                "Add fertilizer: Soil enriched for better growth. (+Max Food)",
                "Add fertilizer: Nutrient levels optimized. (+Max Food)",
                "Add fertilizer: Growth rates improved. (+Food, +Sanity)",
                "Add fertilizer: Soil health fully restored. (+Max Food)"
            }
        },
        { farmingprojecttextstring.plantseeds, new List<string>
            {
                "Plant seeds: Initial planting completed. (+Max Food)",
                "Plant seeds: Rows organized for efficiency. (+Max Food)",
                "Plant seeds: Crop rotation planned. (+Food, +Sanity)",
                "Plant seeds: Fields fully planted and optimized. (+Max Food)"
            }
        },
        { farmingprojecttextstring.waterplants, new List<string>
            {
                "Water plants: Irrigation system set up. (+Food)",
                "Water plants: Moisture levels stabilized. (+Food)",
                "Water plants: Watering schedule optimized. (+Food, +Sanity)",
                "Water plants: Hydration system fully automated. (+Max Food)"
            }
        },
        { farmingprojecttextstring.harvestcrops, new List<string>
            {
                "Harvest crops: First yield collected. (+Food)",
                "Harvest crops: Efficient harvesting techniques applied. (+Food)",
                "Harvest crops: Storage solutions improved. (+Food, +Sanity)",
                "Harvest crops: Harvesting process fully optimized. (+Max Food)"
            }
        }
    };

    public enum electricalprojecttextstring
    {
        readjustpowergrid,
        fixsolarpanels,
        changefuses,
        changeligthbulbs,
    }
    // Milestone texts for each electricalprojecttextstring step (every 25%)
    public static readonly Dictionary<electricalprojecttextstring, List<string>> ElectricalProjectTexts = new Dictionary<electricalprojecttextstring, List<string>>
    {
        { electricalprojecttextstring.readjustpowergrid, new List<string>
            {
                "Readjust the power grid: Initial assessment and minor tweaks. (+Electricity)",
                "Readjust the power grid: Major stabilization efforts underway. (+Electricity)",
                "Readjust the power grid: Efficiency upgrades implemented. (+Electricity, +Sanity)",
                "Readjust the power grid: Final checks and full stabilization. Grid reinforced. (+Max Electricity)"
            }
        },
        { electricalprojecttextstring.fixsolarpanels, new List<string>
            {
                "Fix solar panels: Inspect and clean panels. (+Max Electricity)",
                "Fix solar panels: Repair damaged cells and wiring. (+Max Electricity)",
                "Fix solar panels: Optimize panel alignment for better output. (+Electricity, +Sanity)",
                "Fix solar panels: System fully optimized and reinforced. (+Max Electricity)"
            }
        },
        { electricalprojecttextstring.changefuses, new List<string>
            {
                "Change fuses: Identify faulty fuses and replace. (+Electricity)",
                "Change fuses: Upgrade fuse capacity for stability. (+Electricity)",
                "Change fuses: Improve routing for efficiency. (+Electricity, +Sanity)",
                "Change fuses: Finalize upgrades and reinforce grid. (+Max Electricity)"
            }
        },
        { electricalprojecttextstring.changeligthbulbs, new List<string>
            {
                "Replace bulbs: Remove old bulbs and install new LEDs. (+Electricity)",
                "Replace bulbs: Optimize lighting circuits for efficiency. (+Electricity)",
                "Replace bulbs: Improve energy savings and morale. (+Electricity, +Sanity)",
                "Replace bulbs: Lighting fully upgraded, grid reinforced. (+Max Electricity)"
            }
        }
    };

    private List<string> currentProjectSteps; // Stores the chosen project's steps.
    private int currentStepIndex;
    // Returns a random electrical project type, but always returns its milestone texts in order (0-3).
    public List<string> GetRandomProjectTextsInOrder<T>(Dictionary<T, List<string>> projectTexts)
    {
        if (projectTexts.Count == 0) return new List<string>();
        var keys = new List<T>(projectTexts.Keys);
        var randomKey = keys[Random.Range(0, keys.Count)];
        return projectTexts[randomKey];
    }

    public string StartNewRandomProject<T>(Dictionary<T, List<string>> projectTexts)
    {
        currentProjectSteps = GetRandomProjectTextsInOrder(projectTexts);
        currentStepIndex = 0;
        _textbox.DisplayText("New project started!");
        return currentProjectSteps[0];
    }
    
   public string GetNextProjectStep()
    {
        if (currentProjectSteps != null && currentStepIndex < currentProjectSteps.Count)
        {
            string stepText = currentProjectSteps[currentStepIndex+1];

            currentStepIndex++;

            return stepText;
        }
        return "Project complete!";
    }


    void Awake()
    {
        InitProject();
    }


    void InitProject(){
        _textbox = FindObjectOfType<Textbox>();
        if (playerStats == null) playerStats = FindObjectOfType<Stats>();

        // Added a null check here for clearer error messages, just like in the Timer script.
        if (playerStats == null)
        {
            Debug.LogError("FATAL ERROR: The Projects script could not find the Player's Stats component in the scene!");
        }

        EnsureStates();
        foreach (var kv in projectStates)
        {
            kv.Value.progress = 0f;
            kv.Value.at25 = false;
            kv.Value.at50 = false;
            kv.Value.at75 = false;
            kv.Value.complete = false;
        }
        scienceProgress = 0f;
        electricalProgress = 0f;
        farmingProgress = 0f;

        MirrorPublicProgress();
    }
    

    

    public float scienceProject(float incrementamount){
        return ProgressProject(projectprogress.science, incrementamount);
    }
    public float electricalProject(float incrementamount) {
        return ProgressProject(projectprogress.electrical, incrementamount);
    }
    public float farmingProject(float incrementamount) {
        return ProgressProject(projectprogress.farming, incrementamount);
    }

    void EnsureStates(){
        if (projectStates.Count == 0)
        {
            projectStates[projectprogress.science] = new ProjectState { name = scienceProjectName, progress = scienceProgress };
            projectStates[projectprogress.electrical] = new ProjectState { name = electricalProjectName, progress = electricalProgress };
            projectStates[projectprogress.farming] = new ProjectState { name = farmingProjectName, progress = farmingProgress };
        }
        else
        {
            projectStates[projectprogress.science].name = scienceProjectName;
            projectStates[projectprogress.electrical].name = electricalProjectName;
            projectStates[projectprogress.farming].name = farmingProjectName;
        }
    }

    void MirrorPublicProgress(){
        scienceProgress = projectStates[projectprogress.science].progress;
        electricalProgress = projectStates[projectprogress.electrical].progress;
        farmingProgress = projectStates[projectprogress.farming].progress;
    }

    float ProgressProject(projectprogress type, float incrementamount)
    {
        EnsureStates();
        ProjectState state = projectStates[type];
        if (state.complete)
        {
            _textbox.DisplayText("Project '" + state.name + "' already completed.");
            _textbox.DisplayText("You cannot work on this project until other projects are completed.");
            return state.progress;
        }

        state.progress = Mathf.Clamp(state.progress + incrementamount, 0f, maxamount);
        projectStates[type] = state;
        MirrorPublicProgress();

        _textbox.DisplayText(type.ToString() + " Project Percent: " + state.progress);
        GrantMilestones(type, state);
        MirrorPublicProgress();
        return state.progress;
    }

    void GrantMilestones(projectprogress type, ProjectState state)
    {
        // 25%
        if (!state.at25 && state.progress >= 25f)
        {
            state.at25 = true;
            switch (type)
            {
                case projectprogress.science:
                    if (playerStats != null) playerStats.ModifySanity(+5f);
                    aa = StartNewRandomProject(ScienceProjectTexts);
                    //aa = GetRandomProjectTextsInOrder(ScienceProjectTexts);
                    currentStepIndex = 0;
                    _textbox.DisplayText("Milestone reached (" + state.name + "): 25% - " + aa);
                    break;
                case projectprogress.electrical:
                    if (playerStats != null) playerStats.ModifyElectricity(+10f);
                    //_textbox.DisplayText("Milestone reached (" + state.name + "): 25% - Minor power gain (+Electricity).");
                    aa = StartNewRandomProject(ElectricalProjectTexts);
                    _textbox.DisplayText("Milestone reached (" + state.name + "): 25% - " + aa);
                    break;
                case projectprogress.farming:
                    if (playerStats != null) playerStats.ModifyFood(+10f);
                    aa = StartNewRandomProject(FarmingProjectTexts);
                    _textbox.DisplayText("Milestone reached (" + state.name + "): 25% - " + aa);
                    break;
            }
        }

        // 50%
        if (!state.at50 && state.progress >= 50f)
        {
            state.at50 = true;
            switch (type)
            {
                case projectprogress.science:
                    if (playerStats != null) playerStats.ModifySanity(+5f);
                    aa = GetNextProjectStep();//GetRandomProjectTextsInOrder(ScienceProjectTexts);
                    _textbox.DisplayText("Milestone reached (" + state.name + "): 50% - " + aa);
                    break;
                case projectprogress.electrical:
                    if (playerStats != null) playerStats.ModifyElectricity(+10f);
                    //_textbox.DisplayText("Milestone reached (" + state.name + "): 25% - Minor power gain (+Electricity).");
                    //aa = GetRandomProjectTextsInOrder(ElectricalProjectTexts);
                    aa = GetNextProjectStep();//GetRandomProjectTextsInOrder(ScienceProjectTexts);
                    _textbox.DisplayText("Milestone reached (" + state.name + "): 50% - " + aa);
                    break;
                case projectprogress.farming:
                    if (playerStats != null) playerStats.ModifyFood(+10f);
                    //aa = GetRandomProjectTextsInOrder(FarmingProjectTexts);
                    aa = GetNextProjectStep();//GetRandomProjectTextsInOrder(ScienceProjectTexts);
                    _textbox.DisplayText("Milestone reached (" + state.name + "): 50% - " + aa);
                    break;
            }
        }

        // 75%
        if (!state.at75 && state.progress >= 75f)
        {
            state.at75 = true;
            switch (type)
            {
                case projectprogress.science:
                    if (playerStats != null) playerStats.ModifySanity(+5f);
                    //aa = GetRandomProjectTextsInOrder(ScienceProjectTexts);
                    aa = GetNextProjectStep();//GetRandomProjectTextsInOrder(ScienceProjectTexts);
                    _textbox.DisplayText("Milestone reached (" + state.name + "): 75% - " + aa);
                    break;
                case projectprogress.electrical:
                    if (playerStats != null) playerStats.ModifyElectricity(+10f);
                    //_textbox.DisplayText("Milestone reached (" + state.name + "): 25% - Minor power gain (+Electricity).");
                   // aa = GetRandomProjectTextsInOrder(ElectricalProjectTexts);
                    aa = GetNextProjectStep();//GetRandomProjectTextsInOrder(ScienceProjectTexts);
                    _textbox.DisplayText("Milestone reached (" + state.name + "): 75% - " + aa);
                    break;
                case projectprogress.farming:
                    if (playerStats != null) playerStats.ModifyFood(+10f);
                    //aa = GetRandomProjectTextsInOrder(FarmingProjectTexts);
                    aa = GetNextProjectStep();//GetRandomProjectTextsInOrder(ScienceProjectTexts);
                    _textbox.DisplayText("Milestone reached (" + state.name + "): 75% - " + aa);
                    break;
            }
        }

        // 100%
        if (!state.complete && state.progress >= maxamount)
        {
            state.complete = true;
            state.progress = maxamount;
            switch (type)
            {
                case projectprogress.science:
                    if (playerStats != null) playerStats.ModifySanity(+5f);
                    //aa = GetRandomProjectTextsInOrder(ScienceProjectTexts);
                    aa = GetNextProjectStep();//GetRandomProjectTextsInOrder(ScienceProjectTexts);
                    _textbox.DisplayText("Milestone reached (" + state.name + "): 100% - " + aa);
                    break;
                case projectprogress.electrical:
                    if (playerStats != null) playerStats.ModifyElectricity(+10f);
                    //_textbox.DisplayText("Milestone reached (" + state.name + "): 25% - Minor power gain (+Electricity).");
                    //aa = GetRandomProjectTextsInOrder(ElectricalProjectTexts);
                    aa = GetNextProjectStep();//GetRandomProjectTextsInOrder(ScienceProjectTexts);
                    _textbox.DisplayText("Milestone reached (" + state.name + "): 100% - " + aa);
                    break;
                case projectprogress.farming:
                    if (playerStats != null) playerStats.ModifyFood(+10f);
                    //aa = GetRandomProjectTextsInOrder(FarmingProjectTexts);
                    aa = GetNextProjectStep();//GetRandomProjectTextsInOrder(ScienceProjectTexts);
                    _textbox.DisplayText("Milestone reached (" + state.name + "): 100% - " + aa);
                    break;
            }
        }
    }

    // Update is called once per frame
    void Update()

    {

    }
}
