using UnityEngine;

public class EnemyDistraction : MonoBehaviour {

    [Header("Behaviour")]

    private bool _occupied = false;

    private void OnTriggerEnter(Collider other) {
        if (!_occupied) {
            EnemyBite bite = other.GetComponent<EnemyBite>();
            if (bite != null) {
                bite.ChangeTarget(transform.GetChild(0));
                _occupied = true;
            }
        }
    }

}
