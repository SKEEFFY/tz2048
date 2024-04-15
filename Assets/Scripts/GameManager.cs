using TMPro;
using UnityEngine;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TileBoard board;
    [SerializeField] private CanvasGroup gameOver;

    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI hiscoreText;

    private int _score;

    private void Start()
    {
        NewGame();
    }

    public void NewGame()
    {
        SetScore(0);

        hiscoreText.text = UpdateHighScore().ToString();
        gameOver.alpha = 0f;
        gameOver.interactable = false;

        board.ClearBoard();
        board.CreateTile();
        board.CreateTile();
        board.enabled = true;
    }

    public void GameOver()
    {
        board.enabled = false;
        gameOver.interactable = true;

        Fade(gameOver, 1f, 1f);
    }

    private void Fade(CanvasGroup canvasGroup, float to, float delay)
    {
        canvasGroup.DOFade(to, 0.5f).SetDelay(delay);   
    }

    public void IncreaseScore(int points)
    {
        SetScore(_score + points);
    }

    private void SetScore(int score)
    {
        this._score = score;

        scoreText.text = score.ToString();

        SaveHighScore();
    }

    private void SaveHighScore()
    {
        int highScore = UpdateHighScore();
        if (_score > highScore)
        {
            PlayerPrefs.SetInt("highscore", _score);
        }
    }

    private int UpdateHighScore()
    {
        return PlayerPrefs.GetInt("highscore", 0);
    }
}
