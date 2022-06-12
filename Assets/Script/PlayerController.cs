using UnityEngine;

[RequireComponent(typeof(PlayerMotor))]
[RequireComponent(typeof(ConfigurableJoint))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float speed = 3f;

    [SerializeField]
    private float mouseSensitivityX; //= 3f;

    [SerializeField]
    private float mouseSensitivityY; //= 3f;

    [SerializeField]
    private float thrusterForce = 1000;

    [Header("Joint Options")]
    [SerializeField]
    private float jointSpring = 20f;

    [SerializeField]
    private float jointMaxForce = 50f;

    private PlayerMotor motor;
    private ConfigurableJoint joint;

    private void Start() 
    {
        motor = GetComponent<PlayerMotor>();
        joint = GetComponent<ConfigurableJoint>();
        SetJointSettings(jointSpring);
    }

    private void Update() 
    {
        float xMov = Input.GetAxisRaw("Horizontal");
        float zMov = Input.GetAxisRaw("Vertical");

        Vector3 moveHorizontal = transform.right * xMov;
        Vector3 moveVertical = transform.forward * zMov;

        Vector3 velocity = (moveHorizontal + moveVertical).normalized * speed;

        motor.Move(velocity);

        float yRot = Input.GetAxisRaw("Mouse X");

        Vector3 rotation = new Vector3(0, yRot, 0) * mouseSensitivityX;

        motor.Rotate(rotation);

        float xRot = Input.GetAxisRaw("Mouse Y");

        float cameraRotationX = xRot * mouseSensitivityY;

        motor.RotateCamera(cameraRotationX);

        Vector3 thrusterVelocity = Vector3.zero;

        if(Input.GetButton("Jump"))
        {
            thrusterVelocity = Vector3.up * thrusterForce;
            SetJointSettings(0f);
        }
        else
        {
            SetJointSettings(jointSpring);
        }

        motor.ApplyThruster(thrusterVelocity);
    }

    private void SetJointSettings(float _jointSpring)
    {
        joint.yDrive = new JointDrive {positionSpring = _jointSpring, maximumForce = jointMaxForce};
    }
}
