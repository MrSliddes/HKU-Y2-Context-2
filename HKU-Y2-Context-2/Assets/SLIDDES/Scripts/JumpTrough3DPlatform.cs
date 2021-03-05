using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpTrough3DPlatform : MonoBehaviour
{
    public BoxCollider platformTriggerCollider;
    public BoxCollider platformCollider;

    // Start is called before the first frame update
    void Start()
    {
        DisablePlatform();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Collision with player, enable box collision
        if(other.CompareTag("Player"))
        {
            // TODO Check if player is coming from below and not sides, check with triggerstay > longer than x sec EnablePlatform
            //if(other.transform.position.)
            EnablePlatform();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Player leaves, disable collision
        if(other.CompareTag("Player")) DisablePlatform();
    }

    private void EnablePlatform()
    {
        platformCollider.enabled = true;
    }

    private void DisablePlatform()
    {
        platformCollider.enabled = false;
    }
}
