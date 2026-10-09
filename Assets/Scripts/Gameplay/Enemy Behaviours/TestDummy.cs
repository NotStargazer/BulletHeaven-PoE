using System;
using Gameplay;
using Gameplay.Data;
using Gameplay.Modifiers;
using UnityEngine;

public class TestDummy : MonoBehaviour, IEntity
{
    [SerializeField] private StatTracker _statTracker;
    [SerializeField] private HitPool _hitPool;
    [SerializeField] private SkillManager _skillManager;
    public Transform Transform => transform;

    StatTracker IEntity.StatTracker
    {
        get => _statTracker;
        set => _statTracker = value;
    }

    HitPool IEntity.HitPool
    {
        get => _hitPool;
        set => _hitPool = value;
    }

    SkillManager IEntity.SkillManager
    {
        get => _skillManager;
        set => _skillManager = value;
    }

    private void Awake()
    {
        _hitPool.Restore();
    }
    
    private void OnValidate()
    {
        Validate();
    }

    public virtual void Validate()
    {
        (this as IEntity).ImplementComponents();
    }
}
