using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMovement : MonoBehaviour
{
    #region Variables

    [Header("Movement"), SerializeField] private Rigidbody2D rb2D;
    [SerializeField] private float movementSpeed;
    [SerializeField] private InputActionReference move, attack;
    private Vector2 _moveDirection;
    
    [Header("Animation"), SerializeField] private SpriteRenderer playerSprite;
    [SerializeField] private Animator playerAnimator;
    
    #endregion
    
    #region UnityFunctions

    private void Update()
    {
        _moveDirection = move.action.ReadValue<Vector2>();
    }
    private void FixedUpdate()
    {
        Movement();
    }
    
    #endregion

    #region MovementFunctions

    /// <summary>
    /// transform the player position based in the moveDirection and the movenentSpeed
    /// Also Flip the Sprite based in the _moveDirection
    /// </summary>
    private void Movement()
    {
        rb2D.MovePosition(rb2D.position + _moveDirection * movementSpeed * Time.fixedDeltaTime);
        if (_moveDirection.x < 0) playerSprite.flipX = true;
        else if (_moveDirection.x > 0) playerSprite.flipX = false;
        if (_moveDirection.x == 0 & _moveDirection.y == 0) playerAnimator.ResetTrigger("Walking");
        else playerAnimator.SetTrigger("Walking");
    }
    #endregion
}
