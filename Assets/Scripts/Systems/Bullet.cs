using UnityEngine;

public class Bullet : Entity
{
    [SerializeField] private Monster target;
    [SerializeField] private float speed = 5f;

    protected override void Update()
    {
        base.Update();

        if (target != null)
        {
            Vector3 toTarget = (target.transform.position - transform.position).normalized;
            Move(toTarget * speed);
        }
    }

    #region SET
    public void SetTarget(Monster _mon) => target = _mon;
    #endregion

    #region GET
    #endregion
}
