using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupWeaponStick : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if(FindObjectOfType<Player>().hasWeaponStick) Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            // Pickup stick
            other.GetComponent<Player>().hasWeaponStick = true;
            Destroy(gameObject);
        }
    }
}
