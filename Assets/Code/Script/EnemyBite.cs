using System.Collections;
using UnityEngine;

public class EnemyBite : MonoBehaviour {

    [Header("Behaviour")]

    [SerializeField] private float _movementSpeed;
    private bool _notWaitingToMove = true;
    [SerializeField] private Vector3 _trackOffsetDistant;

    [Space(8)]

    [SerializeField] private float _attackRange;
    [SerializeField] private float _attackDelay;

    [SerializeField] private float _sleepDistance;

    [Header("Cache")]

    private Rigidbody _rb;
    private WaitForSeconds _attackWait;

    private Player _player;
    private Transform _target;

    private void Awake() {
        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = false;
        _attackWait = new WaitForSeconds(_attackDelay);

        _player = FindObjectOfType<Player>();
        _target = transform;
        StartCoroutine(Sleep());
    }

    private void FixedUpdate() {
        Move();
        if (_notWaitingToMove && _target == _player.transform) Attack();
    }

    private void Move() {
        if (_notWaitingToMove && Vector3.Distance(transform.position, _target.position) > 0.1f) {
            _rb.velocity = (_target.position + (Vector3.Distance(_target.position, transform.position) > 3f ? _trackOffsetDistant : Vector3.zero) - transform.position).normalized * _movementSpeed;
            transform.eulerAngles = Vector3.up * Mathf.Atan2(_rb.velocity[0], _rb.velocity[2]) * Mathf.Rad2Deg;
        }
        else _rb.velocity = Vector3.zero;
    }

    private void Attack() {
        if ((_target.position - transform.position).magnitude < _attackRange) StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine() {
        _player.TakeDamage(transform.position);
        _notWaitingToMove = false;

        yield return _attackWait;

        _notWaitingToMove = true;
    }

    public void ChangeTarget(Transform newTarget) {
        _target = newTarget;
    }

    private IEnumerator Sleep() {
        while (Vector3.Distance(_player.transform.position, transform.position) > _sleepDistance) { yield return null; }

        _target = _player.transform;
    }

}
