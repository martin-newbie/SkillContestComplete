using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingMissile : Projectile
{
    public GameObject Explosion;
    Transform target;
    Vector3[] points = new Vector3[4];
    float curDelay, maxDelay;
    LockOn thisLock;

    public void HomingInit(Transform start, Transform end, float startPoint, float endPoint, LockOn thislock)
    {
        target = end;

        maxDelay = Random.Range(0.8f, 1f);
        curDelay = 0f;
        thisLock = thislock;

        points[0] = start.position;
        points[1] = start.position +
            (startPoint * Random.Range(-1f, 1f) * start.right) +
            (startPoint * Random.Range(-0.15f, 1f) * start.up) +
            (startPoint * Random.Range(-0.8f, 1f) * start.forward);
        points[2] = end.position +
            (endPoint * Random.Range(-1f, 1f) * end.right) +
            (endPoint * Random.Range(-1f, 1f) * end.up) +
            (endPoint * Random.Range(0.8f, 1f) * end.forward);
        points[3] = end.position;

        transform.position = start.position;
    }

    protected override void Update()
    {
        if (curDelay >= maxDelay) Push();
        if (!target.gameObject.activeSelf) Push();

        curDelay += Time.deltaTime * moveSpeed;

        transform.position = new Vector3(
            CubicBezier(points[0].x, points[1].x, points[2].x, points[3].x),
            CubicBezier(points[0].y, points[1].y, points[2].y, points[3].y),
            CubicBezier(points[0].z, points[1].z, points[2].z, points[3].z));
    }

    protected override void DestroyEffect()
    {
        base.DestroyEffect();

        try
        {
            Destroy(thisLock.gameObject);
        }
        catch (MissingReferenceException)
        {
        }

        GameObject temp = Instantiate(Explosion, transform.position, Quaternion.identity);
        Destroy(temp.gameObject, 5f);
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy")) Push();
    }

    float CubicBezier(float a, float b, float c, float d)
    {
        float t = curDelay / maxDelay;

        float ab = Mathf.Lerp(a, b, t);
        float bc = Mathf.Lerp(b, c, t);
        float cd = Mathf.Lerp(c, d, t);

        float abbc = Mathf.Lerp(ab, bc, t);
        float bccd = Mathf.Lerp(bc, cd, t);

        return Mathf.Lerp(abbc, bccd, t);
    }
}
