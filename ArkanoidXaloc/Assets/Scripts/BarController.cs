using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum InputType { Left, Right, None }

public class BarController : MonoBehaviour
{

    #region Properties

    //Public
    public float barVelocity = 2;

    //Private
    [SerializeField] private Transform leftBorder;
    [SerializeField] private Transform rightBorder;
    private Rigidbody2D rb;
    private Collider2D c2D;
    private float colliderBoundsSize;
    private InputType inputType = InputType.None;
    private float startBarVelocity;

    #endregion

    #region Unity Callbacks

    private void OnEnable()
    {
        GameManager.Instance.restartPowerUpMode += ResetPowerUpMode;
    }

    private void OnDisable()
    {
        GameManager.Instance.restartPowerUpMode -= ResetPowerUpMode;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        c2D = GetComponent<Collider2D>();
        startBarVelocity = barVelocity;
    }

    void Update()
    {
        #if UNITY_EDITOR || UNITY_STANDALONE_WIN
        if (Input.GetKey(KeyCode.A))
        {
            inputType = InputType.Left;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            inputType = InputType.Right;
        }
        else
        {
            inputType = InputType.None;
        }
        if (Input.GetKey(KeyCode.Space))
        {
            GameManager.Instance.startBallMovement();
        }
        #endif
    }

    private void FixedUpdate()
    {
        if (inputType == InputType.Left)
        {
            rb.constraints = RigidbodyConstraints2D.None;
            rb.velocity = new Vector2(-barVelocity, 0);
        }
        else if (inputType == InputType.Right)
        {
            rb.constraints = RigidbodyConstraints2D.None;
            rb.velocity = new Vector2(barVelocity, 0);
        }
        else
        {
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            rb.velocity = Vector2.zero;
        }
        ClampBarPosition();
    }

    #endregion

    #region PowerUps Methods

    public void ChangePowerUpMode(PowerUpMode mode)
    {
        switch (mode)
        {
            case PowerUpMode.SlowBar:
                barVelocity = barVelocity / 1.5f;
                break;
        }
    }

    public void ResetPowerUpMode()
    {
        barVelocity = startBarVelocity;
    }

    #endregion

    #region Clamp method

    private void ClampBarPosition()
    {
        colliderBoundsSize = (c2D.bounds.size.x / 2);
        if (transform.position.x - colliderBoundsSize <= GameManager.Instance.leftBorder.position.x && rb.velocity.x < 0)
        {
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            rb.MovePosition(new Vector2(GameManager.Instance.leftBorder.position.x + colliderBoundsSize, transform.position.y));
        }
        if (transform.position.x + colliderBoundsSize >= GameManager.Instance.rightBorder.position.x && rb.velocity.x > 0)
        {
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            rb.MovePosition(new Vector2(GameManager.Instance.rightBorder.position.x - colliderBoundsSize, transform.position.y));
        }
    }

    #endregion

}
