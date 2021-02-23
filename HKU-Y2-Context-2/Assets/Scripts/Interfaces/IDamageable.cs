using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Use this interface the class can receive damage
/// </summary>
public interface IDamageable
{
    void ReceiveDamage(float amount);
}
