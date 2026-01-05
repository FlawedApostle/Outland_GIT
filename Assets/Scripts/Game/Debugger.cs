using UnityEngine;

public class Debuger : MonoBehaviour
{
    [Header("Relative Movement Script Reference")]
    [SerializeField, Tooltip("Reference to the RelativeMovement script for movement direction")]
    private RelativeMovement DEBUG_relativeMovement;

    [Header("Animator Script Reference")]
    //[SerializeField, Tooltip("Reference to the RelativeMovement script for movement direction")]
    private Animator DEBUG_animator;

    [Header("TorsoBone Script Reference")]
    [SerializeField]// Tooltip("Reference to the RelativeMovement script for movement direction")]
    private TorsoBoneMovement DEBUG_TorsoBoneMovement;


    private void Update()
    {
        
    //Debug.Log("MoveDir: " + moveDir + " | targetYaw: " + targetYaw);
    Debug.Log("MoveDir: " + DEBUG_TorsoBoneMovement.Get_moveDir() + " | targetYaw: " + DEBUG_TorsoBoneMovement.Get_targetYaw());
    }


    //Debug.Log("isWalking = " +  DEBUG_relativeMovement.Get_statusAnimator()); //+ " | magnitude = " + inputMovementMagnitude);       /// DEBUG

}
