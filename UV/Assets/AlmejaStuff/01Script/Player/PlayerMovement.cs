using System;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMovement : MonoBehaviour
{
    #region Variables

    [SerializeField] private GameObject player;
    
    [Header("Movement"), SerializeField] private Rigidbody2D playerRB;
    [SerializeField] private float movementSpeed;
    [SerializeField] private InputActionReference move, attack;
    private Vector2 _moveDirection;

    [Header("Attack"), SerializeField] private GameObject attackEfect;

    [SerializeField] private BallAttack ballAttack;
    //[SerializeField] private GameObject pelota;
    [SerializeField] private SOBoolean unArmed;
    
    [Header("Animation"), SerializeField] private Animator playerAnimator/*, attackEAnimator*/;
    private Vector3 _playerRotation;
    
    #endregion
    
    #region UnityFunctions

    private void Start()
    {
        unArmed.Value = false;
    }

    private void Update()
    {
        _moveDirection = move.action.ReadValue<Vector2>();

    }
    private void FixedUpdate()
    {
        Movement();
    }
    
    private void OnEnable()
    {
        attack.action.started += Attack;
    }

    private void OnDisable()
    {
        attack.action.started -= Attack;
    }
    
    #endregion

    #region MovementFunctions
    
    /// <summary>
    /// transform the player position based in the moveDirection and the movenentSpeed
    /// Also Flip the Sprite based in the _moveDirection
    /// </summary>
    private void Movement()
    {
        playerRB.MovePosition(playerRB.position + _moveDirection * movementSpeed * Time.fixedDeltaTime);
        if (_moveDirection.x < 0) _playerRotation = new Vector3(0, 180, 0);
        else if (_moveDirection.x > 0) _playerRotation = new Vector3(0, 0, 0);
        if (_moveDirection.x == 0 & _moveDirection.y == 0) playerAnimator.ResetTrigger("Walking");
        else playerAnimator.SetTrigger("Walking");
        
        player.transform.eulerAngles = _playerRotation;
    }
    
    #endregion
    
    #region AttackFunctions
    
    private void Attack(InputAction.CallbackContext context)
    {
        playerAnimator.Play("Player Punch");
        //attackEAnimator.SetTrigger("IsAttacking");
        HitBall();
    }

    private void HitBall()
    {
        if (unArmed.Value == false)
        {
            ballAttack.TrowBall(_moveDirection, player.transform.position);
            unArmed.Value = true;
        }
        else
        {
            print("No hay pelota que patear");
        }

        
    }
    #endregion
}
