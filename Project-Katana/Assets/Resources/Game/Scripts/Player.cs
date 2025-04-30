using UnityEngine;
using Cinemachine;
using Spine.Unity;
using System.Collections.Generic;
using System.Collections;
using Spine;
using System;
using Photon.Pun;
using FMODUnity;

public class Player : MonoBehaviour
{
    public enum CharacterState {
        None,
        Idle,
        Run,
        Attack,
        Jump,
        Fall,
        Land
    }

    [Header("Carac")]
    public float speed;
    public float MaxHealth;
    public float JumpForce;
    public int nbDoubleJump;
    CharacterState previousState, currentState;

    [Header("Params")]
    public CinemachineVirtualCamera VirtualCam;
    public GameObject groundCheckPosition;
    public LayerMask whatIsLayerGround;
    public SkeletonAnimation skeletonAnimation;
    public Transform AttackPoint;

    [Header("Spine")]

    [SpineAnimation] public string IdleAnimationName;
    [SpineAnimation] public string runAnimationName;
    [SpineAnimation] public string JumpAnimationName;
    [SpineAnimation] public string AttackAnimationName;
    [SpineAnimation] public string FallAnimationName;
    [SpineAnimation] public string ReceptionAnimationName;
    public string eventNameFootstep;
    public string eventNameImpact;
    public string eventNameAttack;
    public bool logDebugMessage = false;

    [Header("FMOD")]

    public FMODUnity.EventReference landSoundEvent;
    public FMODUnity.EventReference StepSoundEvent;

    private float Health;
    private bool isDead;
    private bool isFacingLeft;
    private Rigidbody2D rb;
    private float DirectionX;
    private bool isGrounded;
    private bool isLanding;
    private int doubleJumpCounter;
    private bool isAttacking;
    private bool previousGrounded;

    // Start is called before the first frame update
    void Start()
    {
        Health = MaxHealth;
        rb = GetComponent<Rigidbody2D>();
        SetFacing(false);

        // Event
        skeletonAnimation.AnimationState.Event += HandleEvent;

    }

    // Update is called once per frame
    void Update()
    {
        if (!isDead)
        {
            // Facing Charactere
            DirectionX = Input.GetAxisRaw("Horizontal");
            if (DirectionX == -1f && isFacingLeft == false)
            {
                transform.rotation = Quaternion.Euler(transform.rotation.x, 180f, transform.rotation.z);
                SetFacing(true);
            }
            else if (DirectionX == 1f && isFacingLeft == true)
            {
                transform.rotation = Quaternion.Euler(transform.rotation.x, 0f, transform.rotation.z);
                SetFacing(false);
            }

            isGrounded = Physics2D.OverlapCircle(groundCheckPosition.transform.position, 0.3f, whatIsLayerGround);

            if (Input.GetKeyDown("space"))
            {
                if (isGrounded)
                {
                    rb.velocity = Vector2.up * JumpForce;
                }
                else if (doubleJumpCounter > 0)
                {
                    rb.velocity = Vector2.up * JumpForce;
                    doubleJumpCounter--;
                }
            }

            if (isGrounded)
            {
                doubleJumpCounter = nbDoubleJump;
            }

            CheckState();

            // attack
            if (Input.GetMouseButtonDown(0) && !isAttacking && !isLanding)
            {
                StartCoroutine(AttackCoroutine());
            }

            //Land
            if (!previousGrounded && isGrounded && !isAttacking && !isLanding)
            {
               StartCoroutine(LandCoroutine());
            }
            previousGrounded = isGrounded;

            //Check si animation change
            bool stateChanged = previousState != currentState;
            previousState = currentState;

            //Animations
            if (stateChanged && !isAttacking && !isLanding) 
            {
                HandleStateChanged();
            }
        }
    }

    void FixedUpdate()
    {
        if (!isDead)
        {
            rb.position += new Vector2(DirectionX * speed * Time.deltaTime, rb.velocity.y);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheckPosition.transform.position, 0.3f);
    }

    public void SetFacing(bool FacingLeft)
    {
        isFacingLeft = FacingLeft;
    }

    IEnumerator AttackCoroutine()
    {
        if (isAttacking) yield break;

        isAttacking = true;
        
        currentState = CharacterState.Attack;

        // Joue l'attaque
        GetComponent<PhotonView>().RPC("PlayNewAnimation", RpcTarget.All, AttackAnimationName, 0);

        // Attend que l'animation soit finie
        TrackEntry track = skeletonAnimation.AnimationState.GetCurrent(0);
        yield return new WaitForSeconds(track.Animation.Duration);

        CheckState();
        HandleStateChanged();

        isAttacking = false;
    }

    IEnumerator LandCoroutine()
    {
        if (isLanding) yield break;

        isLanding = true;
        
        currentState = CharacterState.Land;

        // Joue le land
        GetComponent<PhotonView>().RPC("PlayNewAnimation", RpcTarget.All, ReceptionAnimationName, 0);

        // Attend que l'animation soit finie
        TrackEntry track = skeletonAnimation.AnimationState.GetCurrent(0);
        yield return new WaitForSeconds(track.Animation.Duration);

        CheckState();
        HandleStateChanged();

        isLanding = false;
    }

    void HandleStateChanged () {

        string animation;

        switch (currentState) {
        case CharacterState.Idle:
            animation = IdleAnimationName ;
            break;
        case CharacterState.Run:
            animation = runAnimationName ;
            break;
        case CharacterState.Jump:
            animation = JumpAnimationName ;
            break;
        case CharacterState.Fall:
            animation = FallAnimationName ;
            break;
        default:
            animation = null ;
            break;
        }

        GetComponent<PhotonView>().RPC("PlayNewAnimation", RpcTarget.All, animation, 0);
    }

    [PunRPC]
    public void PlayNewAnimation (string target, int layerIndex) {
        skeletonAnimation.AnimationState.SetAnimation(layerIndex, target, true);
    }

    public void CheckState()
    {
        if (isGrounded) {
            if (DirectionX == 0)
            {
                currentState = CharacterState.Idle;
            }
            else
            {
                currentState = CharacterState.Run;
            }
        } 
        else 
        {
            if (GetComponent<Rigidbody2D>().velocity.y > 0)
            {
                currentState = CharacterState.Jump;
            }
            else if (GetComponent<Rigidbody2D>().velocity.y < 0)
            {
                currentState = CharacterState.Fall;
            }
        }
    }

    private void HandleEvent (TrackEntry trackEntry, Spine.Event e) {
        if (logDebugMessage) Debug.Log("Event fired! " + e.Data.Name);
        {
            if (e.Data.Name == eventNameImpact || e.Data.Name == eventNameFootstep)
            {
                //Play(e.Data.Name);
            }
            else if (e.Data.Name == eventNameAttack)
            {
                Attack();
            }
        }
    }

    public void Play(String SoundName) {

        FMOD.Studio.EventInstance instance;

        if (SoundName == eventNameImpact)
        {
            instance = FMODUnity.RuntimeManager.CreateInstance(landSoundEvent);
            instance.start();
            instance.release(); // Libère l'instance une fois jouée
        }
        else if (SoundName == eventNameFootstep)
        {
            instance = FMODUnity.RuntimeManager.CreateInstance(StepSoundEvent);
            instance.start();
            instance.release(); // Libère l'instance une fois jouée
        }
    }

    public void Attack()
    {
        Debug.Log("AttackEvent");
    }
}
