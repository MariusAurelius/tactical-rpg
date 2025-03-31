using UnityEngine;
using AgentScript;

public class AnimationStateController : MonoBehaviour
{
    private Animator _animator;
    private Unit _unit;

    private void Start()
    {
        // Récupère les composants nécessaires
        _animator = GetComponent<Animator>();
        _unit = GetComponent<Unit>();

        if (_animator == null)
        {
            Debug.LogError("Animator component is missing on this GameObject.");
        }

        if (_unit == null)
        {
            Debug.LogError("Unit component is missing on this GameObject.");
        }
    }

    private void Update()
    {
        if (_unit == null || _animator == null) return;

        // Gère les animations en fonction du comportement de l'unité
        switch (_unit.currentBehaviour)
        {
            case Unit.BEHAVIOURS.WANDERING:
                SetAnimatorParameters(isMoving: true, isAttacking: false);
                break;
            case Unit.BEHAVIOURS.GOING:
                SetAnimatorParameters(isMoving: true, isAttacking: false);
                break;

            case Unit.BEHAVIOURS.ATTACKING:
                SetAnimatorParameters(isMoving: false, isAttacking: true);
                break;

            default:
                SetAnimatorParameters(isMoving: false, isAttacking: false);
                Debug.LogWarning("Unknown behaviour detected.");
                break;
        }
    }

    private void SetAnimatorParameters(bool isMoving, bool isAttacking)
    {
        _animator.SetBool("isMoving", isMoving);
        _animator.SetBool("isAttacking", isAttacking);
    }
}