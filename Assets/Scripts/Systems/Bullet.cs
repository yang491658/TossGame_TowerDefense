using UnityEngine;

public class Bullet : Entity
{
    [Header("Battle")]
    [SerializeField] private Monster target;
    private Vector3 targetPos;
    [SerializeField] private int attackDamage;
    [SerializeField] private float moveSpeed = 10f;

    protected override void Update()
    {
        base.Update();

        Shoot();
    }

    #region 전투
    public virtual void Shoot()
    {
        if (target != null)
            targetPos = target.transform.position;

        Vector3 toTarget = targetPos - transform.position;

        if (toTarget.sqrMagnitude < 0.01f)
        {
            if (target != null)
                target.TakeDamage(attackDamage);

            Destroy(gameObject);
            return;
        }

        Move(toTarget.normalized * moveSpeed);
    }
    #endregion

    #region SET
    public void SetBullet(Tower _tower)
    {
        Vector3 baseScale = transform.localScale;
        Vector3 towerScale = _tower.transform.lossyScale;
        transform.localScale = new Vector3(
            baseScale.x / towerScale.x,
            baseScale.y / towerScale.y,
            baseScale.z / towerScale.z
        );

        sr.color = _tower.GetColor();
        target = _tower.GetTarget();
        attackDamage = _tower.GetDamage();
    }
    #endregion

    #region GET
    #endregion
}
