using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Dependencies for using the HeroController
/// - an optional jumpSound can be provided.
/// - the hero must have a rigidbody
/// - the hero must have an animator
///   required animator parameters:
///      Speed (float)
///      IsGrounded (bool)
/// - the hero must have a capsule collider used
///   for both collisions with walls/objects, but also
///   for ground detection.  Align bottom of capsule collider
///   with bottom of feet
/// - game objects defining the floor should pe placed on Layer 8
///   (ideally named "Floor", but not required).  They need a collider
///   component.
/// </summary>
public class HeroController : MonoBehaviour
{
    // run speed is scaled by speedMultiplier
    [SerializeField]
    private float speedMultiplier = 6;

    [SerializeField]
    private AudioClip jumpSound = null;

    [SerializeField]
    private float jumpForce = 18;


    private Rigidbody _rigidbody;
    private Animator _animator;


    // jump - has a jump been requested by player?
    // input is checked during Update() but jump is handled
    // in FixedUpdate()
    private bool jump;

    // if true, character's feet are on the ground
    // - used to modify animator parameters
    // - unlocks ability to jump
    private bool isGrounded;

    // negative for left, positive for right
    private float horizontalInput;

    void Start()
    {
        // cache hero components for use in Update/FixedUpdate
        // loops
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
    }


    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");

        // use left/right player input to determine direction
        // character should face.  Set the "forward" vector of the
        // hero accordingly
        transform.forward = new Vector3(horizontalInput, 0, (Mathf.Abs(horizontalInput)-1));

        // Update the hero's animation
        _animator.SetFloat("Speed", horizontalInput);
        _animator.SetBool("IsGrounded", isGrounded);

        // Detect a jump
        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
        }
    }

    private void FixedUpdate()
    {
        // update the velocity of the rigid body depending on player
        // input.  Retain the current y velocity (based on physics),
        // clamp the z velocity at 0 (since we're constraining the player
        // to a 2D plane perpendicular to the camera direction)
        _rigidbody.velocity = new Vector3(
            horizontalInput * speedMultiplier,
            _rigidbody.velocity.y,
            0);

        // compute isGrounded
        // The hero's origin is at their feet
        // create a cube .1f centered at the feet.  If this overlaps
        // with any colliders on layermask 8 (the floor) they are standing
        // on the floor
        int layerMask = 1 << 8; // Only intersect with layer 8 (Floor)
        isGrounded = Physics.OverlapBox(
            _rigidbody.position, Vector3.one * 0.1f, Quaternion.identity,
            layerMask).Length >= 1;

        if (jump && isGrounded)
        {
            // we don't allow jumping in the air, only allow
            // if grounded
            _rigidbody.velocity = new Vector3(
                _rigidbody.velocity.x,
                jumpForce,
                0
            );

            // if a jump sound is provided, play it
            if (jumpSound != null)
            {
                AudioSource.PlayClipAtPoint(jumpSound, transform.position, 0.5f);
            }
        }
        // don't "cache" a jump request if made while in the air
        jump = false;
    }
}
