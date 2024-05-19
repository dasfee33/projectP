using System.Collections;
using System.Collections.Generic;
using Spine;
using UnityEngine;
using Event = Spine.Event;

public abstract class SkillBase : InitBase
{
    public Creature Owner { get; protected set; }
    public float RemainCoolTime { get; set; }

    public Data.SkillData SkillData { get; private set; }

    public override bool Init()
    {
        if (base.Init() is false) return false;

        return true;
    }

    public virtual void SetInfo(Creature owner, int skillTemplateID)
    {
        Owner = owner;
        SkillData = Managers.Data.SkillDic[skillTemplateID];

        // Register AnimEvent
        if (Owner.SkeletonAnim != null && Owner.SkeletonAnim.AnimationState != null)
        {
            Owner.SkeletonAnim.AnimationState.Event -= OnOwnerAnimEventHandler;
            Owner.SkeletonAnim.AnimationState.Event += OnOwnerAnimEventHandler;
        }
    }

    private void OnDisable()
    {
        if (Managers.Game == null)
            return;
        if (Owner.IsValid() == false)
            return;
        if (Owner.SkeletonAnim == null)
            return;
        if (Owner.SkeletonAnim.AnimationState == null)
            return;

        Owner.SkeletonAnim.AnimationState.Event -= OnOwnerAnimEventHandler;
    }

    public virtual void DoSkill()
    {
        //�غ�� ��ų���� ����
        if (Owner.Skills != null)
            Owner.Skills.ActiveSkills.Remove(this);

        //���� ���
        float timeScale = 1.0f;

        if (Owner.Skills.DefaultSkill == this)
            Owner.PlayAnimation(0, SkillData.AnimName, false).TimeScale = timeScale;
        else
            Owner.PlayAnimation(0, SkillData.AnimName, false).TimeScale = 1;

        StartCoroutine(CoCountdownCooldown());
    }

    private IEnumerator CoCountdownCooldown()
    {
        RemainCoolTime = SkillData.CoolTime;
        yield return new WaitForSeconds(SkillData.CoolTime);
        RemainCoolTime = 0;

        // �غ�� ��ų�� �߰�
        if (Owner.Skills != null)
            Owner.Skills.ActiveSkills.Add(this);
    }

    public virtual void CancelSkill()
    {

    }

    protected virtual void GenerateProjectile(Creature owner, Vector3 spawnPos)
    {
        Projectile projectile = Managers.Object.Spawn<Projectile>(spawnPos, SkillData.ProjectileId);

        LayerMask excludeMask = 0;
        excludeMask.AddLayer(Define.Layers.Default);
        excludeMask.AddLayer(Define.Layers.Projectile);
        excludeMask.AddLayer(Define.Layers.Env);
        excludeMask.AddLayer(Define.Layers.Obstacle);

        switch (owner.CreatureType)
        {
            case Define.CreatureTypes.Player:
                excludeMask.AddLayer(Define.Layers.Player);
                break;
            case Define.CreatureTypes.Monster:
                excludeMask.AddLayer(Define.Layers.Monster);
                break;
        }

        projectile.SetSpawnInfo(Owner, this, excludeMask);
    }

    private void OnOwnerAnimEventHandler(TrackEntry trackEntry, Event e)
    {
        // �ٸ���ų�� �ִϸ��̼� �̺�Ʈ�� �ޱ� ������ �ڱⲨ�� �����
        if (trackEntry.Animation.Name == SkillData.AnimName)
            OnAttackEvent();
    }

    protected abstract void OnAttackEvent();
}
