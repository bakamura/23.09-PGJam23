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

    [SerializeField] private float _damagedKnockBack;

    [Header("Inputs")]

    [SerializeField] private KeyCode _movementForwardKey;
    [SerializeField] private KeyCode _movementRightKey;
    [SerializeField] private KeyCode _movementLeftKey;
    [SerializeField] private KeyCode _movementBackKey;

    [Space(8)]

    [SerializeField] private KeyCode _jumpKey;

    [Space(8)]

    [SerializeField] private KeyCode _showMouseKey;

    private bool _canInput = true;

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
    private Animator _anim;
    private WaitForSeconds _invincibilityWait;

    private UI_Game _ui;
    private float _fC;
    private Vector3 _v3C;

    private void Awake() {
        _rb = GetComponent<Rigidbody>();
        _anim = GetComponentInChildren<Animator>();
        _invincibilityWait = new WaitForSeconds(_invincibilityDuration);

        _ui = FindObjectOfType<UI_Game>();

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
        MouseLockUpdate();
    }

    private void FixedUpdate() {
        if (_canInput) {
            Move();
            if (GroundCheck()) {
                if (_jumpInput > 0) Jump();
                else if (_anim.GetCurrentAnimatorStateInfo(0).IsName("Jump")) _anim.SetTrigger("Fall");
            }
        }
    }

    private void Move() {
        _v3C[0] = (Input.GetKey(_movementRightKey) ? 1 : 0) + (Input.GetKey(_movementLeftKey) ? -1 : 0);
        _v3C[2] = (Input.GetKey(_movementForwardKey) ? 1 : 0) + (Input.GetKey(_movementBackKey) ? -1 : 0);
        if (Mathf.Abs(_v3C[0]) + Mathf.Abs(_v3C[2]) > 0) {
            _fC = Mathf.Atan2(_v3C[0], _v3C[2]) * Mathf.Rad2Deg + _cameraRotator.eulerAngles.y; // Target Angle
            _v3C = (Quaternion.Euler(0, _fC, 0) * Vector3.forward).normalized * _movementSpeed;
            transform.eulerAngles = Vector3.up * _fC;
            _anim.SetBool("Running", true);
        }
        else _anim.SetBool("Running", false);

        _v3C[1] = _rb.velocity.y; // Maintain Y velocity

        _rb.velocity = _v3C;
    }

    private void Jump() {
        _jumpInput = 0;
        _anim.SetTrigger("Jumping");
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

    private void MouseLockUpdate() {
        if (Input.GetKeyDown(_showMouseKey)) {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else if (Input.GetKeyUp(_showMouseKey)) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void TakeDamage(Vector3 damageOrigin) {
        if (!_invincible) StartCoroutine(TakeDamageRoutine(damageOrigin));
    }

    private IEnumerator TakeDamageRoutine(Vector3 damageOrigin) {
        _canInput = false;

        _healthCurrent--;
        _ui.ChangeHealthDisplay((float)_healthCurrent / (float)_healthMax);

        if (_healthCurrent <= 0) {
            Die();

            yield break;
        }

        _v3C = transform.position - damageOrigin;
        _v3C[1] = 0;
        _rb.velocity = (Vector3.up * 2 + _v3C.normalized) * _damagedKnockBack;

        _invincible = true;

        yield return _invincibilityWait;

        _canInput = true;
        _invincible = false;
    }

    private void Die() {
        print("player dead");
        // Death Animation
        _canInput = true; // Debug
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
