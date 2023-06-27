using RPG.Core;
using RPG.Combat;
using UnityEngine;

public class ProjectTile : MonoBehaviour
{
    [SerializeField] private BoxCollider _collider;
    [SerializeField] private float _speed;
    
    private Health _target;
    private float _damage;

    private void Update()
    {
        if (_target == null) return;

        transform.position = Vector3.MoveTowards(transform.position, _target.transform.position + Vector3.up,
            _speed * Time.deltaTime);
        transform.LookAt(_target.transform);
    }

    private Vector3 GetAimMeshLocation()
    {
        Vector3 center = _target.GetComponent<MeshFilter>().mesh.bounds.size / 2;
        return _target.transform.position + center;
    }

    private Vector3 GetAimLocation()
    {
        CapsuleCollider center = _target.GetComponent<CapsuleCollider>();
        return _target.transform.position + Vector3.up * center.height / 2;
    }

    public void InitArrow(Health target, float damage)
    {
        _target = target;
        _damage = damage;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Health targetHealth) && targetHealth == _target)
        {
            targetHealth.TakeDamage(_damage);
            Destroy(gameObject);
        }
    }
}