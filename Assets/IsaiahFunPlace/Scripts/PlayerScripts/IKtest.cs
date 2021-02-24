using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKtest : MonoBehaviour
{
    float leftFootPositionWeight;
    float leftFootRotationWeight;
    Transform leftFootObj;

    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnAnimatorIK(int layerIndex)
    {
        animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, leftFootPositionWeight);
        animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, leftFootRotationWeight);
        animator.SetIKPosition(AvatarIKGoal.LeftFoot, leftFootObj.position);
        animator.SetIKRotation(AvatarIKGoal.LeftFoot, leftFootObj.rotation);
    }
}
