using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneSpawner : MonoBehaviour
{
    public bool shootRight = false;
    public float fireRate = 4;

    public GameObject prefabPlane;
    public SpriteRenderer spriteRenderer;
    public Sprite spriteLeft;
    public Sprite spriteRight;
    public Transform bulletPosLeft;
    public Transform bulletPosRight;

    private float fireRateTimer;

    // Start is called before the first frame update
    void Start()
    {
        // Set
        if(shootRight)
        {
            spriteRenderer.sprite = spriteRight;
        }
        else
        {
            spriteRenderer.sprite = spriteLeft;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(fireRateTimer > 0)
        {
            fireRateTimer -= Time.deltaTime;
        }
        else
        {
            // Shoot
            fireRateTimer = fireRate;
            Vector3 pos; if(shootRight) pos = bulletPosRight.position; else pos = bulletPosLeft.position;
            GameObject a = Instantiate(prefabPlane, pos, Quaternion.identity);
        }
    }
}
