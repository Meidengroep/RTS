using UnityEngine;
using System.Collections;

/// <summary>
/// Simple test AI that doesn't do much.
/// </summary>
public class SimpleUnitAI : MonoBehaviour 
{
    public Unit ControlledUnit;

    void OnTriggerEnter(Collider other)
    {
        if (!ControlledUnit.IsIdle())
            return;

        if (ControlledUnit.CanAttackOther(other.gameObject))
        {
            ControlledUnit.AttackOrder(other.gameObject.GetComponent<SelectableWithHealth>());
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (!ControlledUnit.IsIdle())
            return;

        if (ControlledUnit.CanAttackOther(other.gameObject))
        {
            ControlledUnit.AttackOrder(other.gameObject.GetComponent<SelectableWithHealth>());
        }
    }
}
