using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public GameObject ballPrefab;
    public TextMeshProUGUI scoreText;

    private int totalScore = 0;

    // --- DIFFICULTY VARIABLES ---
    public float minSpeed = 5f;
    public float newMaxSpeed = 10f;
    public float difficultyIncrement = 1f;

    void Start() {
        UpdateScoreText();
    }

    void Update()
    {
        int numberOfBalls = GameObject.FindGameObjectsWithTag("Player").Length;

        // If the number is 0... We have lost them all!
        if (numberOfBalls == 0)
        {
            RestartLevel();
        }
    }

    void SpawnBall()
    {
        Instantiate(ballPrefab, new Vector3(0, 3f, 0), Quaternion.identity); // Quaternion.identity is something about the rotation of the object.
    }

    public void AddScore()
    {
        totalScore++; // Add 1
        UpdateScoreText();
        IncreaseDifficultyLevel();

        // A new ball appears after 5 points scored.
        if (totalScore % 5 == 0)
        {
            SpawnBall();
        }
    }

    void UpdateScoreText()
    {
        scoreText.text = "Score: " + totalScore.ToString();
    }

    void IncreaseDifficultyLevel()
    {
        // Increase max speed.
        newMaxSpeed += difficultyIncrement;

        // Find ALL balls that already exist in the game
        GameObject[] existingBalls = GameObject.FindGameObjectsWithTag("Player");

        // Tell them one by one to update their values
        foreach (GameObject ball in existingBalls)
        {
            ball.GetComponent<BallLogic>().UpdateDifficulty(newMaxSpeed);
        }
    }

    void RestartLevel()
    {
        // Reloads the currently active scene (Resets everything to how it was at the beginning)
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}