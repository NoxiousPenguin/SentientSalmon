﻿/// Author: Samuel Arzt
/// Date: March 2017


#region Includes
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;
using System.IO;
using TMPro;
#endregion




/// <summary>
/// Singleton class for managing the evolutionary processes.
/// </summary>
public class EvolutionManager : MonoBehaviour
{
    #region Members
    private static System.Random randomizer = new System.Random();

    public static EvolutionManager Instance
    {
        get;
        private set;
    }

    // Whether or not the results of each generation shall be written to file, to be set in Unity Editor
    [SerializeField]
    private bool SaveStatistics = false;
    private string statisticsFileName;

    // a field to be set that dictates if this is a minigame or not; changes the behavior of how the initial fish are created
    [SerializeField]
    private bool isMiniGame = false; // false for now, can change this later

    // How many of the first to finish the course should be saved to file, to be set in Unity Editor
    [SerializeField]
    private uint SaveFirstNGenotype = 0;
    private uint genotypesSaved = 0;

    // Population size, to be set in Unity Editor
    private int PopulationSize;

    [HideInInspector]
    public int totalGenerationCount; //  not sure if we should expose this

    private string saveParameters; // parameters for the salmon from a previous training session

    // Whether to use elitist selection or remainder stochastic sampling, to be set in Unity Editor
    [SerializeField]
    private bool ElitistSelection = false;

    // Topology of the agent's FNN, to be set in Unity Editor
    [SerializeField]
    private uint[] FNNTopology;

    // menu game object that is displayed once the training is over
    [SerializeField]
    private GameObject endTrainingMenu;

    // The current population of agents.
    private List<Agent> agents = new List<Agent>();
    /// <summary>
    /// The amount of agents that are currently alive.
    /// </summary>
    public int AgentsAliveCount
    {
        get;
        private set;
    }

    /// <summary>
    /// Event for when all agents have died.
    /// </summary>
    public event System.Action AllAgentsDied;

    private GeneticAlgorithm geneticAlgorithm;

    /// <summary>
    /// The age of the current generation.
    /// </summary>
    public uint GenerationCount
    {
        get { return geneticAlgorithm.GenerationCount; }
    }

    public float averageEvaluation
    {
        get; set;
    }

    public float relativeFinish
    {
        get; set;
    }
    #endregion

    #region Constructors
    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("More than one EvolutionManager in the Scene.");
            return;
        }
        Instance = this;

        // hide end training menu
        if (endTrainingMenu != null)
        {
            endTrainingMenu.SetActive(false);
            Button[] buttons = endTrainingMenu.GetComponentsInChildren<Button>();
            foreach (Button button in buttons)
            {
                if (button.name == "MainMenuButton")
                {
                    button.onClick.AddListener(delegate { BackToMainMenuButton(); });
                }

                else if (button.name == "RestartButton")
                {
                    // add callback that would restart the training
                    button.onClick.AddListener(delegate { TrainAgainButton(); });
                }
            }
        }
    }

    void OnEnable()
    {
        PopulationSize = PlayerPrefs.GetInt("popCount", 30);
        totalGenerationCount = PlayerPrefs.GetInt("totalGenerationCount", 25);
        saveParameters = PlayerPrefs.GetString("saveParameters", "");
        Debug.Log("Population Size: " + PopulationSize);

        if (isMiniGame)
            totalGenerationCount = 0; // does not end in the minigame, only the time affects the spawning

        if (saveParameters == "")
        { 
            if (isMiniGame)
            {
                Debug.Log("no saved fish parameters found.");
            }
            else {
                Debug.Log("this isnt a minigame, we dont need to load fish parameters");
            }
        }
        else
        {
            if (isMiniGame)
            {
                Debug.Log("Loading these parameters for the MiniGame:\n" + saveParameters);
            }
            else {
                Debug.Log("This isnt a minigame, we dont need to load fish parameters");
            }
        }

        Debug.Log("totalGenerationCount: " + totalGenerationCount);
    }

    void OnDisable()
    {
        // only save parameters if we are at the end of a simulation
        if (!isMiniGame)
        {
            if (geneticAlgorithm.saveParameters == null)
            {
                Debug.Log("no fish parameters to save.");
            }
            else {
                Debug.Log("Saving these parameters:\n" + geneticAlgorithm.saveParameters);
            }
            
            PlayerPrefs.SetString("saveParameters", geneticAlgorithm.saveParameters);
        }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Starts the evolutionary process.
    /// </summary>
    public void StartEvolution()
    {
        //Create neural network to determine parameter count
        NeuralNetwork nn = new NeuralNetwork(FNNTopology);

        //Setup genetic algorithm; if minigame then reconstruct the fish from saved parameters
        if (isMiniGame && saveParameters != "")
        {
            geneticAlgorithm = new GeneticAlgorithm((uint) nn.WeightCount, (uint) PopulationSize, saveParameters);
            geneticAlgorithm.InitialisePopulation = GeneticAlgorithm.DoNotPopulationInitialisation; // should ignore the randomization portion of parameters
        }

        else
        {
            geneticAlgorithm = new GeneticAlgorithm((uint) nn.WeightCount, (uint) PopulationSize);
        }
        genotypesSaved = 0;

        geneticAlgorithm.Evaluation = StartEvaluation;

        if (ElitistSelection)
        {
            //Second configuration
            // geneticAlgorithm.Selection = RemainderStochasticSampling;
            geneticAlgorithm.Selection = GeneticAlgorithm.DefaultSelectionOperator;
            // geneticAlgorithm.Recombination = RandomRecombination;
            geneticAlgorithm.Recombination = GeneticAlgorithm.DefaultRecombinationOperator;
            geneticAlgorithm.Mutation = MutateAllButBestTwo;
        }
        else
        {   
            //First configuration
            geneticAlgorithm.Selection = RemainderStochasticSampling;
            geneticAlgorithm.Recombination = RandomRecombination;
            geneticAlgorithm.Mutation = MutateAllButBestTwo;
        }
        
        AllAgentsDied += geneticAlgorithm.EvaluationFinished;

        //Statistics
        if (SaveStatistics)
        {
            Debug.Log("SAVING STATS"); 
            Directory.CreateDirectory(Application.streamingAssetsPath+ "/stat_logs/");
            //statisticsFileName = Application.streamingAssetsPath+ "/stat_logs/" +"Evaluation - " + GameStateManager.Instance.TrackName ;
            statisticsFileName = Application.streamingAssetsPath+ "/stat_logs/" +"Evaluation - " + GameStateManager.Instance.TrackName + " " + DateTime.Now.ToString("yyyy_MM_dd_HH-mm-ss");
            WriteStatisticsFileStart();
            geneticAlgorithm.FitnessCalculationFinished += WriteStatisticsToFile;
        }
        geneticAlgorithm.FitnessCalculationFinished += CheckForTrackFinished;

        //Restart logic
        if (totalGenerationCount > 0)
        {
            geneticAlgorithm.TerminationCriterion += CheckGenerationTermination;
            geneticAlgorithm.AlgorithmTerminated += OnGATermination;
        }

        geneticAlgorithm.Start();
    }

    // Writes the starting line to the statistics file, stating all genetic algorithm parameters.
    private void WriteStatisticsFileStart()
    {
        
         string stats =  Environment.NewLine + DateTime.Now.ToString("yyyy_MM_dd_HH-mm-ss") + " - Evaluation of a Population with size " + PopulationSize + 
                ", on Track \"" + GameStateManager.Instance.TrackName + "\", using the following GA operators: " + Environment.NewLine +
                "Selection: " + geneticAlgorithm.Selection.Method.Name + Environment.NewLine +
                "Recombination: " + geneticAlgorithm.Recombination.Method.Name + Environment.NewLine +
                "Mutation: " + geneticAlgorithm.Mutation.Method.Name + Environment.NewLine + 
                "FitnessCalculation: " + geneticAlgorithm.FitnessCalculationMethod.Method.Name + Environment.NewLine + 
                
                "Population Count: " + PlayerPrefs.GetInt("popCount", 99) + Environment.NewLine + 
                "Salmon Max Speed: " + PlayerPrefs.GetFloat("salmonMaxSpeed", 99) + Environment.NewLine +
                "Current Resistance: " + PlayerPrefs.GetFloat("currentResistance", 99) + Environment.NewLine +
                "Bear Speed: " + PlayerPrefs.GetFloat("bearSpeed", 99) + Environment.NewLine +
                "Spotting Range: " + PlayerPrefs.GetFloat("spottingRange", 99) + Environment.NewLine ;


        File.WriteAllText( statisticsFileName + ".txt",stats);


        staticStats.addStats(stats); 
    }

    // Appends the current generation count and the evaluation of the best genotype to the statistics file.
    private void WriteStatisticsToFile(IEnumerable<Genotype> currentPopulation)
    {
        foreach (Genotype genotype in currentPopulation)
        {
            File.AppendAllText(statisticsFileName + ".txt", geneticAlgorithm.GenerationCount + "\t" + genotype.Evaluation + Environment.NewLine);
            staticStats.addStats(geneticAlgorithm.GenerationCount + "\t" + genotype.Evaluation + Environment.NewLine); 

            break; //Only write first
        }
    }

    // Checks the current population and saves genotypes to a file if their evaluation is greater than or equal to 1
    private void CheckForTrackFinished(IEnumerable<Genotype> currentPopulation)
    {
        if (genotypesSaved >= SaveFirstNGenotype) return;

        string saveFolder = statisticsFileName + "/";

        foreach (Genotype genotype in currentPopulation)
        {
            if (genotype.Evaluation >= 1)
            {
                if (!Directory.Exists(saveFolder))
                    Directory.CreateDirectory(saveFolder);

                genotype.SaveToFile(saveFolder + "Genotype - Finished as " + (++genotypesSaved) + ".txt");

                if (genotypesSaved >= SaveFirstNGenotype) return;
            }
            else
                return; //List should be sorted, so we can exit here
        }
    }

    // Checks whether the termination criterion of generation count was met.
    private bool CheckGenerationTermination(IEnumerable<Genotype> currentPopulation)
    {
        return geneticAlgorithm.GenerationCount >= totalGenerationCount;
    }

    // To be called when the genetic algorithm was terminated
    private void OnGATermination(GeneticAlgorithm ga)
    {
        AllAgentsDied -= ga.EvaluationFinished;

        // rather than restarting, we just end
        Debug.Log("Training Over.");
        staticStats.addStats("--------------------------------------------");


        // prompt the end of sim menu
        if (endTrainingMenu != null)
        {
            endTrainingMenu.SetActive(true);
            //TextMeshProUGUI evalText = GameObject.Find("Evaluation").GetComponent<TextMeshProUGUI>();
            //TextMeshProUGUI endTrainingScoreText = GameObject.Find("EndGameScoreText").GetComponent<TextMeshProUGUI>();
            //endTrainingScoreText.text = "Score: " + evalText.text;
        }
    }

    // Restarts the algorithm after a specific wait time second wait
    private void RestartAlgorithm(float wait)
    {
        Invoke("StartEvolution", wait);
    }

    // Starts the evaluation by first creating new agents from the current population and then restarting the track manager.
    private void StartEvaluation(IEnumerable<Genotype> currentPopulation)
    {
        //Create new agents from currentPopulation
        agents.Clear();
        AgentsAliveCount = 0;

        foreach (Genotype genotype in currentPopulation)
            agents.Add(new Agent(genotype, MathHelper.SoftSignFunction, FNNTopology));

        TrackManager.Instance.SetCarAmount(agents.Count);
        IEnumerator<CarController> carsEnum = TrackManager.Instance.GetCarEnumerator();
        for (int i = 0; i < agents.Count; i++)
        {
            if (!carsEnum.MoveNext())
            {
                Debug.LogError("Cars enum ended before agents.");
                break;
            }

            carsEnum.Current.Agent = agents[i];
            AgentsAliveCount++;
            agents[i].AgentDied += OnAgentDied;
        }

        TrackManager.Instance.Restart();
    }

    // Callback for when an agent died.
    private void OnAgentDied(Agent agent)
    {
        AgentsAliveCount--;

        if (AgentsAliveCount == 0 && AllAgentsDied != null)
            AllAgentsDied();
    }

    #region GA Operators
    // Selection operator for the genetic algorithm, using a method called remainder stochastic sampling.
    private List<Genotype> RemainderStochasticSampling(List<Genotype> currentPopulation)
    {
        List<Genotype> intermediatePopulation = new List<Genotype>();
        //Put integer portion of genotypes into intermediatePopulation
        //Assumes that currentPopulation is already sorted
        foreach (Genotype genotype in currentPopulation)
        {
            if (genotype.Fitness < 1)
                break;
            else
            {
                for (int i = 0; i < (int) genotype.Fitness; i++)
                    intermediatePopulation.Add(new Genotype(genotype.GetParameterCopy()));
            }
        }

        //Put remainder portion of genotypes into intermediatePopulation
        foreach (Genotype genotype in currentPopulation)
        {
            float remainder = genotype.Fitness - (int)genotype.Fitness;
            if (randomizer.NextDouble() < remainder)
                intermediatePopulation.Add(new Genotype(genotype.GetParameterCopy()));
        }

        return intermediatePopulation;
    }

    // Recombination operator for the genetic algorithm, recombining random genotypes of the intermediate population
    private List<Genotype> RandomRecombination(List<Genotype> intermediatePopulation, uint newPopulationSize)
    {
        //Check arguments
        if (intermediatePopulation.Count < 2)
            throw new System.ArgumentException("The intermediate population has to be at least of size 2 for this operator.");

        List<Genotype> newPopulation = new List<Genotype>();
        //Always add best two (unmodified)
        newPopulation.Add(intermediatePopulation[0]);
        newPopulation.Add(intermediatePopulation[1]);


        while (newPopulation.Count < newPopulationSize)
        {
            //Get two random indices that are not the same
            int randomIndex1 = randomizer.Next(0, intermediatePopulation.Count), randomIndex2;
            do
            {
                randomIndex2 = randomizer.Next(0, intermediatePopulation.Count);
            } while (randomIndex2 == randomIndex1);

            Genotype offspring1, offspring2;
            GeneticAlgorithm.CompleteCrossover(intermediatePopulation[randomIndex1], intermediatePopulation[randomIndex2], 
                GeneticAlgorithm.DefCrossSwapProb, out offspring1, out offspring2);

            newPopulation.Add(offspring1);
            if (newPopulation.Count < newPopulationSize)
                newPopulation.Add(offspring2);
        }

        return newPopulation;
    }

    // Mutates all members of the new population with the default probability, while leaving the first 2 genotypes in the list untouched.
    private void MutateAllButBestTwo(List<Genotype> newPopulation)
    {
        for (int i = 2; i < newPopulation.Count; i++)
        {
            if (randomizer.NextDouble() < GeneticAlgorithm.DefMutationPerc)
                GeneticAlgorithm.MutateGenotype(newPopulation[i], GeneticAlgorithm.DefMutationProb, GeneticAlgorithm.DefMutationAmount);
        }
    }

    // Mutates all members of the new population with the default parameters
    private void MutateAll(List<Genotype> newPopulation)
    {
        foreach (Genotype genotype in newPopulation)
        {
            if (randomizer.NextDouble() < GeneticAlgorithm.DefMutationPerc)
                GeneticAlgorithm.MutateGenotype(genotype, GeneticAlgorithm.DefMutationProb, GeneticAlgorithm.DefMutationAmount);
        }
    }

    // callback function for when the main menu button is pressed
    private void BackToMainMenuButton()
    {
        Debug.Log("Main menu button was pressed.");
        SceneManager.LoadScene("TitleScreen");
    }

    private void TrainAgainButton()
    {
        Debug.Log("Retrain button was pressed.");
        if (endTrainingMenu != null)
            endTrainingMenu.SetActive(false);
        
        averageEvaluation = 0;
        relativeFinish = 0;
        RestartAlgorithm(0.1f); // arg is time delay
    }
    #endregion
    #endregion

    }
