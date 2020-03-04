using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{

    #region Properties

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private PowerUpMode powerUp;

    #endregion

    #region Unity Callbacks

    private void Start()
    {
        rb.velocity = new Vector2(0, Random.Range(-2, -1));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bar"))
        {
            GameManager.Instance.ChangePowerUpMode(powerUp);
            gameObject.SetActive(false);
        }
    }

    #endregion

}
