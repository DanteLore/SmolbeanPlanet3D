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
        if (hunter == null)
            return;

        float weight = hunter.IKWeight;

        if (weight > 0f)
        {
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, weight);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, weight);
            animator.SetIKPosition(AvatarIKGoal.LeftHand, hunter.BowPosition);
            animator.SetIKRotation(AvatarIKGoal.LeftHand, hunter.BowRotation);

            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, weight);
            animator.SetIKPosition(AvatarIKGoal.RightHand, hunter.DrawHandPosition);
        }

        if (hunter.Prey != null)
        {
            animator.SetLookAtWeight(weight);
            animator.SetLookAtPosition(hunter.Prey.transform.position);
        }
    }
}
