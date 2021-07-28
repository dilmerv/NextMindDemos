using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI gameStateText;

    [SerializeField]
    private TextMeshProUGUI scoreText;

    [SerializeField]
    private TextMeshProUGUI timerText;

    [SerializeField]
    private GameObject restartButton;

    [SerializeField]
    private GameObject[] mindControls;

    [SerializeField]
    private MindContinuousMoveProvider mindContinuousMoveProvider;

    private float gameTime;

    [Header("Level Options")]
    [SerializeField]
    // Note this is a very very rough way of managing a level
    // It should probably be extended in a more structure way...
    private float minLevelTime = 30.0f;

    [SerializeField]
    private float minLevelBeatDistance = 62.0f;

    public enum GameState
    { 
        NotStarted,
        Playing,
        Lost,
        Won
    }

    private GameState gameState = GameState.NotStarted;

    private void Awake() => SetInitialOptions();
    
    private void SetInitialOptions()
    {
        mindContinuousMoveProvider.gameObject.transform.position = Vector3.zero;
        gameState = GameState.NotStarted;

        ToggleMindProvider(available: false);
        timerText.text = $"{LoadScore().ToString("00")}";
        gameStateText.text = $"{gameState}";
        restartButton.SetActive(false);
    }

    public void Play()
    {
        gameTime = 0;
        gameState = GameState.Playing;
        gameStateText.text = $"{gameState}";
        ToggleMindProvider(available: true);
    }

    public void SetAs(GameState gameState)
    {
        this.gameState = gameState;
        ToggleMindProvider(available: false);
        SaveScore();
        if(this.gameState == GameState.Lost)
            gameStateText.text = $"<color=red>You've {gameState} !</color>";
        else if (this.gameState == GameState.Won)
            gameStateText.text = $"<color=green>You've {gameState} !</color>";

        restartButton.SetActive(true);
    }

    private void ToggleMindProvider(bool available)
    {
        foreach (var go in mindControls) go.SetActive(available);
        mindContinuousMoveProvider.enabled = available;
    }

    private void Update()
    {
        if (gameState == GameState.Playing)
        {
            timerText.text = $"{gameTime.ToString("00")}";
            gameTime += Time.deltaTime * 1.0f;
        }

        // lost game state
        if (gameTime > minLevelTime && gameState == GameState.Playing)
        {
            SetAs(GameState.Lost);        
        }

        // win game state
        if (gameTime <= minLevelTime && gameState == GameState.Playing && 
            mindContinuousMoveProvider.transform.position.z >= minLevelBeatDistance)
        {
            SetAs(GameState.Won);        
        }
    }

    public void Restart() 
    {
        SetInitialOptions();
    }

    private void SaveScore()
    {
        // only save it if we beat it
        float bestScore = LoadScore();
        if (bestScore > gameTime)
        {
            PlayerPrefs.SetFloat("BestScore", gameTime);
            PlayerPrefs.Save();
        }
    }

    private float LoadScore()
    {
        return PlayerPrefs.GetFloat("BestScore", 0);
    }
}
