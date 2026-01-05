using UnityEngine;

public class Debuger : MonoBehaviour
{
    //[Header("Movement Input Reference")]
    //[SerializeField, Tooltip("Reference to the RelativeMovement script for movement direction")]
    //private RelativeMovement relativeMovement;

    //[Header("Movement Input Reference")]
    [SerializeField]// Tooltip("Reference to the RelativeMovement script for movement direction")]
    private TorsoBoneMovement DEBUG_TorsoBoneMovement;


    private void Update()
    {
        
    //Debug.Log("MoveDir: " + moveDir + " | targetYaw: " + targetYaw);
    Debug.Log("MoveDir: " + DEBUG_TorsoBoneMovement.Get_moveDir() + " | targetYaw: " + DEBUG_TorsoBoneMovement.Get_targetYaw());
    }

}
