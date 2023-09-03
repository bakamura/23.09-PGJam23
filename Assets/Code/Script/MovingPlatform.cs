using System.Collections;
using UnityEngine;

public class MovingPlatform : MonoBehaviour {

    [Header("Behaviour")]

    [SerializeField] private float _movementSpeed;
    [SerializeField] private Vector3[] _positions;

    private float _movementProgress;
    private bool _movementForward = true;

    [Header("Cache")]

    private float _fC;

    private void Start() {
        StartCoroutine(CycleThroughPositions(0, 1));
    }

    private IEnumerator CycleThroughPositions(int from, int target) {
        _fC = _movementSpeed / Vector3.Distance(_positions[from], _positions[target]);
        while (_movementProgress < 1) {
            _movementProgress += _fC * Time.deltaTime;
            transform.position = Vector3.Lerp(_positions[from], _positions[target], _movementProgress);

            yield return null;
        }
        _movementProgress -= 1;

        if (target == 0) _movementForward = true;
        else if(target >= _positions.Length - 1) _movementForward = false;
        StartCoroutine(CycleThroughPositions(target, target + (_movementForward ? 1 : -1)));
    }

    private void OnCollisionEnter(Collision collision) {
        if (!collision.gameObject.isStatic) collision.transform.parent = transform;
    }

    private void OnCollisionExit(Collision collision) {
        if (!collision.gameObject.isStatic) collision.transform.parent = null;
    }

}
