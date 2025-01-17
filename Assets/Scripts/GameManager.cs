using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager instance {get; private set;}
    public BoardManager boardManager;
    public PlayerController player;
    public TurnManager turnManager {get; private set;}
    public int foodAmountInput = 100;
    private int foodAmount;
    private int currentLevel = 0;
    public UIDocument uiDocument;
    private Label foodLabel;
    private VisualElement gameOverPanel;
    private Label gameOverText;
    private void Awake()
    {
        turnManager = new TurnManager();
        ManageSingleton();
    }
    private void ManageSingleton()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public void Start()
    {
        turnManager.OnTick += OnTurnHappen;
        gameOverPanel = uiDocument.rootVisualElement.Q<VisualElement>("GameOverPanel");
        gameOverText = uiDocument.rootVisualElement.Q<Label>("GameOverText");
        foodLabel = uiDocument.rootVisualElement.Q<Label>("FoodLabel");
        gameOverPanel.style.visibility = Visibility.Hidden;
        StartNewGame();
    }
    public void StartNewGame()
    {
        gameOverPanel.style.visibility = Visibility.Hidden;
        foodAmount = foodAmountInput;
        currentLevel = 0;
        foodLabel.text = "Food: " + foodAmount;
        boardManager.Init();

        player.Init();
        player.Spawn(boardManager, new Vector2Int(1, 1));
    }
    void OnTurnHappen()
    {
        AudioPlayer.instance.PlayStepClip(player.transform.position);
        ChangeFood(-1);
    }
    public int GetFoodCount()
    {
        return foodAmount;
    }
    public void ChangeFood(int foodValue)
    {
        foodAmount = Mathf.Max(0, foodAmount + foodValue);
        foodLabel.text = "Food: " + foodAmount;
        if (foodAmount <= 0)
        {
            player.GameOver();
            gameOverPanel.style.visibility = Visibility.Visible;
            gameOverText.text = "Game Over\n You traveled " + currentLevel + " levels" + "\n Press Space to restart";
        }
    }
    public void NewLevel()
    {
        currentLevel++;
        boardManager.Clean();
        boardManager.Init();
        player.Spawn(boardManager, new Vector2Int(1, 1));
    }
    public int GetCurrentLevel()
    {
        return currentLevel;
    }
}
