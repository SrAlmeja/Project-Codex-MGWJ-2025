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
    
    #region Animation Variables
    [Header("Animation"), SerializeField] private Animator playerAnimator;
    private Vector3 _playerRotation;
    #endregion
    
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
        UpdateAniamtor();
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Interactable"))
        {
            canInteract.Value = true;
            npcInteraction = collision.GetComponent<NpcInteraction>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Interactable"))
        {
            canInteract.Value = false;
        }
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
        
        player.transform.eulerAngles = _playerRotation;
    }
    
    #endregion
    
    #region TriggersFunctions

    private void Interact(InputAction.CallbackContext context)
    {
        if (canInteract.Value == true)
        {
            npcInteraction.EnableInteractable();
            Debug.Log("Interact");
        }
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
            ballAttack.TrowBall(_lastDirection, player.transform.position);
            unArmed.Value = true;
        }
        else
        {
            print("No hay pelota que patear");
        }

        
    }
    #endregion
    
    #region AnimationFunctions

    private void UpdateAniamtor()
    {
        playerAnimator.SetBool("Walking", false);
        playerAnimator.SetBool("SideWalk", false);
        playerAnimator.SetBool("UpWalk", false);
        playerAnimator.SetBool("UpIddle", false);
        
        if (_moveDirection == Vector2.zero)
        {
            // El jugador está quieto, usamos la última dirección
            if (_lastDirection.y > 0)
                playerAnimator.SetBool("UpIddle", true);
            else if (Mathf.Abs(_lastDirection.x) > 0)
                playerAnimator.SetBool("SideWalk", true); // Puedes usar una idle lateral si tienes
            else
                playerAnimator.SetBool("Walking", true); // Idle hacia abajo
        }
        else
        {
            // El jugador se está moviendo
            if (_moveDirection.y > 0)
                playerAnimator.SetBool("UpWalk", true);
            else if (Mathf.Abs(_moveDirection.x) > 0)
                playerAnimator.SetBool("SideWalk", true);
            else
                playerAnimator.SetBool("Walking", true); // Movimiento hacia abajo
        }
    }

    
    #endregion
}
