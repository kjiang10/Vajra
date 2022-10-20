using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Spine.Unity;

public class CharacterController2D : MonoBehaviour
{
	[SerializeField] private float m_JumpForce = 400f;							// Amount of force added when the player jumps.
	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;	// How much to smooth out the movement
	[SerializeField] private bool m_AirControl = false;							// Whether or not a player can steer while jumping;
	[SerializeField] private LayerMask m_WhatIsGround;							// A mask determining what is ground to the character
	[SerializeField] private LayerMask m_WhatIsMovingPlatform;
	[SerializeField] private Transform m_GroundCheck;							// A position marking where to check if the player is grounded.
	[SerializeField] private Transform m_CeilingCheck;							// A position marking where to check for ceilings

	const float k_GroundedRadius = .35f; // Radius of the overlap circle to determine if grounded
	private bool m_Grounded;            // Whether or not the player is grounded.
	const float k_CeilingRadius = .35f; // Radius of the overlap circle to determine if the player can stand up
	private Rigidbody2D m_Rigidbody2D;
	private bool m_FacingRight = true;  // For determining which way the player is currently facing.
	private Vector3 m_Velocity = Vector3.zero;

	[Header("Events")]
	[Space]

	public UnityEvent OnLandEvent;

	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }



	// need to need the speed of the character to keep the player in the idle state
	SkeletonAnimation skeletonAnimation;
	
	float singleAttackInterval = 0.3f;
	bool preInIdleState = true;
	bool preinRunState = false;
	bool curInIdleState = true;
	bool curInRunState = false;

	bool hasInterrupted = false;
	float reservedStart;

	void Start(){
		skeletonAnimation = GetComponent<SkeletonAnimation>();
		if (skeletonAnimation == null){
            return;
        }
	}


	private void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();
		if (OnLandEvent == null){
			OnLandEvent = new UnityEvent();
		}
			
	}

	private void FixedUpdate()
	{
		bool wasGrounded = m_Grounded;
		m_Grounded = false;

		// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
		// This can be done using layers instead but Sample Assets will not overwrite your project settings.
		Collider2D[] groundColliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
		for (int i = 0; i < groundColliders.Length; i ++){
			if (groundColliders[i].gameObject != gameObject)
			{
				m_Grounded = true;
				if (!wasGrounded)
					OnLandEvent.Invoke();
			}
		}
		// probably need to be changed later
		bool isOnMovingPlatform = false;
		// platform is also ground, so the player can jump on it.
		Collider2D[] movingPlatformColliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsMovingPlatform);
		for (int i = 0; i < movingPlatformColliders.Length; i ++){
			// if the goundCheck has hit any movingPlatform
			if (movingPlatformColliders[i].gameObject != gameObject)
			{
				isOnMovingPlatform = true;
				transform.SetParent(movingPlatformColliders[i].gameObject.transform);
				m_Grounded = true;
				if (!wasGrounded)
					OnLandEvent.Invoke();
			}
		}
		// in case there is a race condition where isOnMovingPlatform is accidentally false at the start.
		if (!isOnMovingPlatform){
			transform.SetParent(null);
		}
	}


	public void Move(float move, bool jump, bool interruptFlag, float attackStart)
	{
		// if interrupted, the time passed in is the reserved start,
		// else, the time passed in should be ignored.
		
		if (Mathf.Abs(move) > 0.01) {
			curInIdleState = false;
			curInRunState = true;
		}
		else{
			curInIdleState = true;
			curInRunState = false;
		}

		//only control the player if grounded or airControl is turned on
		if (m_Grounded || m_AirControl)
		{

			// Move the character by finding the target velocity
			Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
			// And then smoothing it out and applying it to the character
			m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

			// the interrupt signal infers that a interruption has occured.
			if (interruptFlag){
				reservedStart = attackStart;
				hasInterrupted = true;
			}

			// if attack is finished, the interval is no longer reserved, perform idle or run animation
			// based on the current state

			// if it has been interrupted
			if (hasInterrupted){
				// if the interruption has been resolved
				if (Time.time - reservedStart > singleAttackInterval){
					if (curInRunState){
						skeletonAnimation.state.SetAnimation(0, "run", true);
					}	
					if (curInIdleState){
						skeletonAnimation.state.SetAnimation(0, "idle", true);
					}
					// only flip when resolved or nothing no interruption occured.
					// If the input is moving the player right and the player is facing left...
					//Debug.Log("speed:" + move + " pre_idle:" + preInIdleState + " pre_run:" + preinRunState);
					if (move > 0 && !m_FacingRight){
						// ... flip the player.
						Flip();
					}
					// Otherwise if the input is moving the player left and the player is facing right...
					else if (move < 0 && m_FacingRight){
						// ... flip the player.
						Flip();
						// no matter flip or not, if there is speed, the player should animate run
					}

					hasInterrupted = false;
				}
				// else do nothing, keep the original attack animation
			}

			// else if there is no interruption
			else{
				if (move > 0 && !m_FacingRight){
					// ... flip the player.
					Flip();
				}
				// Otherwise if the input is moving the player left and the player is facing right...
				else if (move < 0 && m_FacingRight){
					// ... flip the player.
					Flip();
					// no matter flip or not, if there is speed, the player should animate run
				}
				if (curInRunState && !preinRunState) {
					//Debug.Log("....................................................................................................................................................................................................................................................................");
					skeletonAnimation.state.SetAnimation(0, "run", true);
				}
				else if (curInIdleState && !preInIdleState) {
					//Debug.Log("....................................................................................................................................................................................................................................................................");
					skeletonAnimation.state.SetAnimation(0, "idle", true);
				}
			}
				
			// after attacking, the attack status is interrupted by the attack animation
			// for example, if I keep moving, my curRun and preRun will always be true,
			// which keeps in the run state, but after being interrupted by the attack
			// animation, it cannot set back to moving.
			// that means, if interrupted, its new animation will purely based on
			// current run and idle state.

			preInIdleState = curInIdleState;
			preinRunState = curInRunState;

		}
		// If the player should jump...
		if (m_Grounded && jump)
		{
			// Add a vertical force to the player.
			m_Grounded = false;
			m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
		}
	}

	private void Flip()
	{
		// Switch the way the player is labelled as facing.
		m_FacingRight = !m_FacingRight;

		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

}
