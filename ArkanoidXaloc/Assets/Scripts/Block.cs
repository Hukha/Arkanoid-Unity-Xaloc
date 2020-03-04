using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Block : MonoBehaviour
{

    #region Properties

    [SerializeField] private bool isBreakable;
    [SerializeField] private byte hits = 1;

    #endregion

    #region Unity Callbacks

    private void OnEnable()
    {
        if (isBreakable)
        {
            GameManager.Instance.breakableBlocks.Add(gameObject);
        }
    }

    private void OnDisable()
    {
        if (isBreakable)
        {
            GameManager.Instance.breakableBlocks.Remove(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isBreakable)
        {
            if (!collision.transform.CompareTag("Block") && GameManager.Instance.powerUpMode != PowerUpMode.MassBall)
            {
                if (hits <= 1)
                {
                    DestroyBlock();
                }
                else
                {
                    hits--;
                }
            }
            else
            {
                DestroyBlock();
            }
        }
    }

    private void DestroyBlock()
    {
        gameObject.SetActive(false);
        GameManager.Instance.CheckLevelStatus();
        if (Random.value <= 0.1f) //10%
        {
            Instantiate(GameManager.Instance.powerUpsPrefabs[Random.Range(0, GameManager.Instance.powerUpsPrefabs.Count)], transform.position, Quaternion.identity);
        }
    }

    #endregion

}
