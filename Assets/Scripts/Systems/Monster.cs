using UnityEngine;

public class Monster : Entity
{
    [Header("HP")]
    [SerializeField] private float hp;

    [Header("Speed")]
    [SerializeField] private float speed;

    [Header("Path")]
    [SerializeField] private Transform[] path;
    [SerializeField] private int pathIndex;

    protected override void Start()
    {
        base.Start();

        pathIndex = 0;
        transform.position = path[0].position;
    }

    protected override void Update()
    {
        base.Update();

        if (pathIndex >= path.Length)
        {
            Move(Vector3.down * speed);
            return;
        }

        Transform targetTrans = path[pathIndex];
        Vector3 target = targetTrans.position;
        Vector3 delta = target - transform.position;

        float arrive = Mathf.Max(speed * Time.deltaTime, 0.1f);
        if (delta.sqrMagnitude < arrive * arrive)
        {
            pathIndex++;
            if (pathIndex >= path.Length)
            {
                Move(Vector3.down * speed);
                return;
            }

            targetTrans = path[pathIndex];
            target = targetTrans.position;
            delta = target - transform.position;
        }

        Vector3 direction = delta.normalized;
        Move(direction * speed);
    }

    private void OnBecameInvisible()
    {
        EntityManager.Instance?.Despawn(this);
    }

    #region SET
    public void SetHP(float _hp) => hp = _hp;
    public void SetSpeed(float _speed) => speed = _speed;
    public void SetPath(Transform[] _path)
    {
        path = _path;
        pathIndex = 0;
    }
    #endregion
}
