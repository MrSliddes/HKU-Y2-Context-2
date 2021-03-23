using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupHealth : MonoBehaviour
{
    public AudioClip clip;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            other.GetComponent<AudioSource>()?.PlayOneShot(clip);
            Player p = other.GetComponent<Player>();
            p.health++;
            if(p.health > 3) p.health = 3;
            Destroy(gameObject);
        }
    }
}
