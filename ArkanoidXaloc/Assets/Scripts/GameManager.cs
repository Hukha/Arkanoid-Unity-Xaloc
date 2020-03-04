using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum PowerUpMode {None, SlowBar, MassBall}

[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{

    #region Properties

    public static GameManager Instance;

    //Events
    public delegate void GameBehaviour();
    public GameBehaviour startBallMovement;
    public GameBehaviour restartBallMovement;
    public GameBehaviour restartPowerUpMode;

    //Public
    public Transform leftBorder;
    public Transform rightBorder;
    public Transform upBorder;
    public List<GameObject> breakableBlocks = new List<GameObject>();
    public List<GameObject> powerUpsPrefabs = new List<GameObject>();
    public BarController barController;
    public BallManager ballManager;
    public PowerUpMode powerUpMode = PowerUpMode.None;

    //Private
    [SerializeField] private GameObject life01;
    [SerializeField] private GameObject life02;
    [SerializeField] private GameObject life03;
    private int lifes = 3;

    #endregion

    #region Unity Callbacks

    void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            if (PlayerPrefs.HasKey("Lifes"))
            {
                lifes = PlayerPrefs.GetInt("Lifes");
            }
            CheckLifes();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ball"))
        {
            lifes--;
            CheckLifes();
        }
        else
        {
            collision.gameObject.SetActive(false);
        }
    }

    #endregion

    #region Game PowerUp Method

    public void ChangePowerUpMode(PowerUpMode mode)
    {
        restartPowerUpMode();
        switch (mode)
        {
            case PowerUpMode.None:
                break;
            case PowerUpMode.SlowBar:
                barController.ChangePowerUpMode(mode);
                powerUpMode = PowerUpMode.SlowBar;
                break;
            case PowerUpMode.MassBall:
                ballManager.ChangePowerUpMode(mode);
                powerUpMode = PowerUpMode.MassBall;
                break;
            default:
                break;
        }
    }

    #endregion

    #region Game Check Methods

    private void CheckLifes()
    {
        switch (lifes)
        {
            case 2:
                life03.SetActive(false);
                restartBallMovement();
                break;
            case 1:
                life03.SetActive(false);
                life02.SetActive(false);
                restartBallMovement();
                break;
            case 0:
                life01.SetActive(false);
                SceneManager.LoadScene(0);
                break;
        }
    }

    public void CheckLevelStatus()
    {
        if (breakableBlocks.Count <= 0)
        {
            PlayerPrefs.SetInt("Lifes", lifes);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    #endregion

}
