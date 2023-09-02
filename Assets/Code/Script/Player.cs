using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

// NOT using SOLID principles
public class Player : MonoBehaviour {

    [Header("Health")]

    [SerializeField] private int _healthMax;
    private int _healthCurrent;
    [SerializeField] private float _invincibilityDuration;
    private bool _invincible = false;

    [Header("Inputs")]

    [SerializeField] private KeyCode _movementForwardKey;
    [SerializeField] private KeyCode _movementRightKey;
    [SerializeField] private KeyCode _movementLeftKey;
    [SerializeField] private KeyCode _movementBackKey;

    [Space(8)]

    [SerializeField] private KeyCode _jumpKey;

    [Header("Movement")]

    [SerializeField] private float _movementSpeed;

    [Header("Jump")]

    [SerializeField] private float _jumpHeight;
    private float _jumpInput;

    [Space(8)]

    [SerializeField] private Vector3 _groundCheckPoint;
    [SerializeField] private Vector3 _groundCheckBox;
    [SerializeField] private LayerMask _groundCheckLayer;

    [Header("Camera")]

    [SerializeField] private float _cameraRotaionSensibility;
    [SerializeField] private Transform _cameraRotator;
    private const string MOUSE_X = "Mouse X";

    [Header("Cache")]

    private Rigidbody _rb;
    private WaitForSeconds _invincibilityWait;

    private float _fC;
    private Vector3 _v3C;

    private void Awake() {
        _rb = GetComponent<Rigidbody>();
        _invincibilityWait = new WaitForSeconds(_invincibilityDuration);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    //[DllImport("user32.dll")] //
    //static extern bool SetCursorPos(int X, int Y); //
    private void Start() {
        Spawn();
        //SetCursorPos(Screen.width / 2, Screen.height / 2);
        //_lastMouseX = Input.mousePosition.x / Screen.width;
    }

    private void Update() {
        _jumpInput -= Time.deltaTime;
        if (Input.GetKeyDown(_jumpKey)) _jumpInput = 0.2f;
        CameraUpdate();
    }

    private void FixedUpdate() {
        Move();
        if (_jumpInput > 0 && GroundCheck()) Jump();
    }

    private void Move() {
        _v3C[0] = (Input.GetKey(_movementRightKey) ? 1 : 0) + (Input.GetKey(_movementLeftKey) ? -1 : 0);
        _v3C[2] = (Input.GetKey(_movementForwardKey) ? 1 : 0) + (Input.GetKey(_movementBackKey) ? -1 : 0);
        if (Mathf.Abs(_v3C[0]) + Mathf.Abs(_v3C[2]) > 0) _v3C = 
                (Quaternion.Euler(0, Mathf.Atan2(_v3C[0], _v3C[2]) * Mathf.Rad2Deg + _cameraRotator.eulerAngles.y, 0) * Vector3.forward).normalized * _movementSpeed;
        _v3C[1] = _rb.velocity.y; // Maintain Y velocity

        _rb.velocity = _v3C;
    }

    private void Jump() {
        _jumpInput = 0;
        _v3C = _rb.velocity;
        _v3C[1] = Mathf.Sqrt(2f * _jumpHeight * Mathf.Abs(Physics.gravity.y));
        _rb.velocity = _v3C;
    }

    private bool GroundCheck() {
        return Physics.CheckBox(transform.position + _groundCheckPoint, _groundCheckBox / 2, Quaternion.identity, _groundCheckLayer);
    }

    private void CameraUpdate() {
        _cameraRotator.position = transform.position;
        _cameraRotator.eulerAngles += _cameraRotaionSensibility * Input.GetAxisRaw(MOUSE_X) * Vector3.up;

        //_fC = (Input.mousePosition.x / Screen.width) - _lastMouseX; // Mouse X Delta
        //print(_fC);
        //_cameraRotator.eulerAngles += _cameraRotaionSensibility * _fC * Vector3.up;
        //SetCursorPos(Screen.width / 2, Screen.height / 2);
    }

    public void TakeDamage() {
        if (!_invincible) StartCoroutine(TakeDamageRoutine());
    }

    private IEnumerator TakeDamageRoutine() {
        _healthCurrent--;
        if (_healthCurrent <= 0) {
            Die();

            yield break;
        }

        _invincible = true;

        yield return _invincibilityWait;

        _invincible = false;
    }

    private void Die() {
        print("player dead");
        // Death Animation

        // Reset Game
    }

    private void Spawn() {
        _healthCurrent = _healthMax;
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = GroundCheck() ? Color.green : Color.red;
        Gizmos.DrawWireCube(transform.position + _groundCheckPoint, _groundCheckBox);
    }
}
