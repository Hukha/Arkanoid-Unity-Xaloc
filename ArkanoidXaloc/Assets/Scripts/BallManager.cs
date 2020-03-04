using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallManager : MonoBehaviour
{

    #region Properties

    //Private
    [SerializeField] private float ballVelocity = 5;
    [SerializeField] private Vector2 startDirection = new Vector2(1, 4);
    [SerializeField] private Sprite massBallSprite;
    private SpriteRenderer spriteRenderer;
    private Sprite originalSprite;
    private Rigidbody2D rb;
    private Collider2D c2D;
    private float colliderBoundsSize;
    private Vector2 startLocalPosition;
    private bool inMovement;

    #endregion

    #region Unity Callbacks

    private void OnEnable()
    {
        GameManager.Instance.startBallMovement += StartBallMovement;
        GameManager.Instance.restartBallMovement += RestartBallMovement;
        GameManager.Instance.restartPowerUpMode += ResetPowerUpMode;
    }

    private void OnDisable()
    {
        GameManager.Instance.startBallMovement -= StartBallMovement;
        GameManager.Instance.restartBallMovement -= RestartBallMovement;
        GameManager.Instance.restartPowerUpMode -= ResetPowerUpMode;
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        c2D = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalSprite = spriteRenderer.sprite;
        startLocalPosition = transform.localPosition;
    }

    private void FixedUpdate()
    {
        colliderBoundsSize = (c2D.bounds.size.x / 2);
        if ((rb.velocity.x > 0f && transform.position.x + colliderBoundsSize > GameManager.Instance.rightBorder.position.x))
        {
            rb.velocity = rb.velocity.magnitude * Vector2.Reflect(rb.velocity, Vector2.left).normalized;
        }
        else if ((rb.velocity.x < 0f && transform.position.x - colliderBoundsSize < GameManager.Instance.leftBorder.position.x))
        {
            rb.velocity = rb.velocity.magnitude * Vector2.Reflect(rb.velocity, Vector2.right).normalized;
        }
        if (rb.velocity.y > 0f && transform.position.y + colliderBoundsSize > GameManager.Instance.upBorder.position.y)
        {
            rb.velocity = rb.velocity.magnitude * Vector2.Reflect(rb.velocity, Vector2.up).normalized;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        ContactPoint2D contact = collision.GetContact(0);
        Debug.DrawRay(contact.point, contact.normal, Color.white);
        if (collision.gameObject.CompareTag("Bar"))
        {
            if (contact.normal == Vector2.right && rb.velocity.x < 0f || contact.normal == Vector2.left && rb.velocity.x > 0f)
            {
                rb.velocity = new Vector2(-rb.velocity.x, rb.velocity.y);
            }
            else if (contact.normal != Vector2.down)
            {
                float angle = (-(transform.position.x - collision.transform.position.x) * 60f / 1f) + 90f;
                angle = ClampBallAngle(angle);
                rb.velocity = rb.velocity.magnitude * new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)).normalized;
            }
        }
        else if (collision.transform.CompareTag("Block") && GameManager.Instance.powerUpMode != PowerUpMode.MassBall || collision.transform.CompareTag("NDBlock"))
        {
            Vector2 delta = (transform.position - collision.transform.position);
            delta.Scale(new Vector2(0.5f, 1));
            if (Mathf.Abs(delta.x) >= Mathf.Abs(delta.y))
            {
                if (Mathf.Sign(-rb.velocity.normalized.x) == Mathf.Sign(delta.x))
                {
                    rb.velocity = new Vector2(-rb.velocity.x, rb.velocity.y);
                }
            }
            else
            {
                if (Mathf.Sign(-rb.velocity.normalized.y) == Mathf.Sign(delta.y))
                {
                    rb.velocity = new Vector2(rb.velocity.x, -rb.velocity.y);
                }
            }
        }
    }

    #endregion

    #region PowerUp Methods

    public void ChangePowerUpMode(PowerUpMode mode)
    {
        switch (mode)
        {
            case PowerUpMode.MassBall:
                spriteRenderer.sprite = massBallSprite;
                break;
        }
    }

    public void ResetPowerUpMode()
    {
        spriteRenderer.sprite = originalSprite;
    }

    #endregion

    #region Ball Movement Methods

    private void StartBallMovement()
    {
        if (!inMovement)
        {
            transform.parent = null;
            rb.velocity = startDirection.normalized * ballVelocity;
            inMovement = true;
        }
    }

    private void RestartBallMovement()
    {
        rb.velocity = Vector2.zero;
        transform.parent = GameManager.Instance.barController.transform;
        transform.localPosition = startLocalPosition;
        inMovement = false;
    }

    #endregion

    private float ClampBallAngle(float angle)
    {
        if (angle > 150f)
        {
            return 150f;
        }
        if (angle < 15f)
        {
            return 15f;
        }
        if (angle <= 90f && angle > 85f)
        {
            return 85f;
        }
        if (angle >= 90f && angle < 95f)
        {
            return 95f;
        }
        return angle;
    }

}
