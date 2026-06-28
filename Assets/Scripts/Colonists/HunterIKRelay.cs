using UnityEngine;

public class HunterIKRelay : MonoBehaviour
{
    private Hunter hunter;
    private Animator animator;

    private void Awake()
    {
        hunter = GetComponentInParent<Hunter>();
        animator = GetComponent<Animator>();
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (hunter == null || !hunter.BowActive)
            return;

        float weight = hunter.IKWeight;
        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, weight);
        animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, weight);
        animator.SetIKPosition(AvatarIKGoal.LeftHand, hunter.BowPosition);
        animator.SetIKRotation(AvatarIKGoal.LeftHand, hunter.BowRotation);
    }
}
