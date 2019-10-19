using UnityEngine;

namespace ArowSampleGame.SampleScripts
{
/// <summary>
/// Unityちゃんの行動制御スクリプト
/// </summary>
public class ArowSampleCharacterMove : MonoBehaviour
{
    public float speed = 8f;
    public float rotateSpeed = 120f;

    public bool RestrictionOnControl = true;

    private Animator animator = null;
    private Rigidbody charactorRigidbody;

    Vector3 velocity = new Vector3(0, 0, 0);
    Vector3 befPos = new Vector3(0, 0, 0);

    const float WALK_VELOCITY = 0.2f;
    const float JAMP = 20f;
    const float JAMP_CHARGE_TIME = 4f;

    bool isJumping = false;
    float jumpChargePower = 0f;
    float jumpPower = 0f;

    // Use this for initialization
    void Start()
    {
        animator = GetComponent<Animator>();
        charactorRigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // Unityちゃんの制御に関係なく、
        //「動いた」「動かされた」どちらの場合でも、「移動」モーションを再生する
        if (RestrictionOnControl)
        {
            Vector3 sub = transform.localPosition - befPos;
            Vector3 normalSub = new Vector3(sub.x, 0f, sub.z).normalized;

            if (0 < normalSub.magnitude)
            {
                animator.SetFloat("Speed", 2.0f);
                transform.LookAt(transform.localPosition + normalSub);
            }
            else
            {
                animator.SetFloat("Speed", 0.0f);
            }

            befPos = transform.localPosition;
        }
        else
        {
            if (WALK_VELOCITY < velocity.magnitude)
            {
                animator.SetFloat("Speed", 2.0f);
            }
            else
            {
                animator.SetFloat("Speed", 0.0f);
            }
        }

        if (!Input.GetKey(KeyCode.Space))
        {
            if (Input.GetKeyUp(KeyCode.Space))
            {
                isJumping = true;
                jumpPower = jumpChargePower;
                jumpChargePower = 0;
            }
        }

        if (Input.GetKey(KeyCode.Space))
        {
            if (!isJumping)
            {
                jumpChargePower += Time.deltaTime;

                if (jumpChargePower > JAMP_CHARGE_TIME)
                {
                    jumpChargePower = JAMP_CHARGE_TIME;
                }

                isJumping = false;
                jumpPower = 0;
            }
        }
        else
        {
            jumpChargePower = 0f;
        }
    }

    /// <summary>
    /// GameObjectの実際の「移動」の処理はこっちで行う
    /// </summary>
    void FixedUpdate()
    {
        if (RestrictionOnControl)
        {
            return;
        }

        if (isJumping)
        {
            charactorRigidbody.AddForce(Vector3.up * jumpPower * JAMP, ForceMode.VelocityChange);
            isJumping = false;
            jumpChargePower = 0f;
            jumpPower = 0f;
        }

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        velocity = new Vector3(0, 0, v);
        // キャラクターのローカル空間での方向に変換
        velocity = transform.TransformDirection(velocity);
        // キャラクターの移動
        transform.localPosition += velocity * speed * Time.fixedDeltaTime;
        // キャラクターの回転
        transform.Rotate(0, h * rotateSpeed * Time.fixedDeltaTime, 0);
    }

}
}