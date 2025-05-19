using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using EditorAttributes;

[CreateAssetMenu(fileName = "CombatScriptableObj", menuName = "CombatScriptableObj")]
public class CombatScriptableObj : ScriptableObject
{
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
