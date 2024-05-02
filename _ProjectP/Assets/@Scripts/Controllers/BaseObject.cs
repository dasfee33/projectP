using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using static Define;

public class BaseObject : InitBase
{
    public ObjectTypes ObjectType { get; protected set; } = ObjectTypes.None;
    public CircleCollider2D Collider { get; private set; }
    public SkeletonAnimation SkeletonAnim { get; private set; }
    public Rigidbody2D RigidBody { get; private set; }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        Collider = gameObject.GetOrAddComponent<CircleCollider2D>();
        SkeletonAnim = GetComponent<SkeletonAnimation>();
        RigidBody = GetComponent<Rigidbody2D>();

        return true;
    }
}
