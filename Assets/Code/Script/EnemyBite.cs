using System.Collections;
using UnityEngine;

public class EnemyBite : MonoBehaviour {

    [Header("Behaviour")]

    [SerializeField] private float _movementSpeed;
    private bool _notWaitingToMove = true;

    [Space(8)]

    [SerializeField] private float _attackRange;
    [SerializeField] private float _attackDelay;

    [Header("Cache")]

    private Rigidbody _rb;
    private WaitForSeconds _attackWait;

    private Player _player;
    private Transform _target;

    private void Awake() {
        _rb = GetComponent<Rigidbody>();
        _attackWait = new WaitForSeconds(_attackDelay);

        _player = FindObjectOfType<Player>();
        _target = _player.transform;
    }

    private void FixedUpdate() {
        Move();
        if (_notWaitingToMove && _target == _player.transform) Attack();
    }

    private void Move() {
        if (_notWaitingToMove) _rb.velocity = (_target.position - transform.position).normalized * _movementSpeed;
        else _rb.velocity = Vector3.zero;
    }

    private void Attack() {
        if ((_target.position - transform.position).magnitude < _attackRange) StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine() {
        _player.TakeDamage();
        _notWaitingToMove = false;

        yield return _attackWait;

        _notWaitingToMove = true;
    }

    public void ChangeTarget(Transform newTarget) {
        _target = newTarget;
    }
}
