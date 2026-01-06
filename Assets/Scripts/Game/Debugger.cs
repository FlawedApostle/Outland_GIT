using UnityEngine;

public class Debuger : MonoBehaviour
{
    [Header("TorsoBone Script Reference")]
    [SerializeField]// Tooltip("Reference to the RelativeMovement script for movement direction")]
    private RotateBodyMovement DEBUG_RotateBodyMovement;

    [Header("Relative Movement Script Reference")]
    [SerializeField, Tooltip("Reference to the RelativeMovement script for movement direction")]
    private RelativeMovement DEBUG_relativeMovement;

    [Header("Animator Script Reference")]
    //[SerializeField, Tooltip("Reference to the RelativeMovement script for movement direction")]
    private Animator DEBUG_animator;


    private void Update()
    {

        Debug.Log("MoveDir: " + DEBUG_RotateBodyMovement.Get_moveDir() + " | targetYaw: " + DEBUG_RotateBodyMovement.Get_targetYaw());
    }


    //Debug.Log("isWalking = " +  DEBUG_relativeMovement.Get_statusAnimator()); //+ " | magnitude = " + inputMovementMagnitude);       /// DEBUG

}
