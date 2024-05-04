using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : InitBase
{
    [SerializeField]
    private float orthographicSize;
    private BaseObject target;
    public BaseObject Target
    {
        get { return target; }
        set { target = value; }
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        orthographicSize = 10.0f;
        Camera.main.orthographicSize = orthographicSize;

        return true;
    }

    void LateUpdate()
    {
        if (Target == null)
            return;

        Vector3 targetPosition = new Vector3(Target.CenterPosition.x, Target.CenterPosition.y, -10f);
        transform.position = targetPosition;
    }
}
