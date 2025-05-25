using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using EditorAttributes;


[System.Serializable]
public class DamageInfo
{
    public float damage;
    public float knockUpForce;
    public float knockBackForce;
}

[CreateAssetMenu(fileName = "CombatScriptableObj", menuName = "CombatScriptableObj")]
public class CombatScriptableObj : ScriptableObject
{
    public DamageInfo damageInfo; //
    public PlayerCombatState stateID;

    [Space(10)]
    [Header("Criteria")]
    public bool grounded;
    public bool interruptPerformingAction;
    public bool onlyLockedOn;

    [Space(5)]
    [Header("Input")]
    public bool requireDirectionalInput;

    [ShowIf(nameof(requireDirectionalInput))]
    [AllowNesting]
    public Vector2 directionalInput;

    public InputID[] requiredInputs;

    [Space(10)]
    public CombatScriptableObj[] nextStates;

}
