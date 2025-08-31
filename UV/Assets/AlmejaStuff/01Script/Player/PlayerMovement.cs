using System;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMovement : MonoBehaviour
{
    #region Variables

    [SerializeField] private GameObject player;
    
    [Header("Movement"), SerializeField] private Rigidbody2D playerRB;
    [SerializeField] private float movementSpeed;
    [SerializeField] private InputActionReference move, interact, attack;
    private Vector2 _moveDirection;

    [Header("Attack"), SerializeField] private GameObject attackEfect;
    [SerializeField] private BallAttack ballAttack;
    [SerializeField] private SOBoolean unArmed;
    private bool _imWarrior;
    private Vector2 _lastDirection = Vector2.right;
    
    [Header("Animation"), SerializeField] private Animator playerAnimator/*, attackEAnimator*/;
    private Vector3 _playerRotation;
    
    #endregion
    
    #region UnityFunctions

    private void Start()
    {
        unArmed.Value = false;
        if (ballAttack == null)
        {
            _imWarrior = false;
        }
        else
        {
            _imWarrior = true;
        }
    }

    private void Update()
    {
        _moveDirection = move.action.ReadValue<Vector2>();
        if (_moveDirection != Vector2.zero)
        {
            _lastDirection = _moveDirection.normalized;
        }

    }
    private void FixedUpdate()
    {
        Movement();
    }
    
    private void OnEnable()
    {
        attack.action.started += Attack;
        interact.action.started += Interact;
    }

    private void OnDisable()
    {
        attack.action.started -= Attack;
        interact.action.started -= Interact;
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
    
    #region TriggersFunctions

    private void Interact(InputAction.CallbackContext context)
    {
        Debug.Log("Interact");
    }
    
    private void Attack(InputAction.CallbackContext context)
    {
        if (_imWarrior == true)
        {
            playerAnimator.SetTrigger("Attack");
            HitBall();    
        }
        
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
