using System.Collections.Generic;
using UnityEngine;

public abstract class BaseStateMachine<TState, TType> where TState : State
{
    public TState CurrentState { get; protected set; }
    protected Dictionary<TType, TState> states;

    public BaseStateMachine(Dictionary<TType, TState> allStates)
    {
        states = allStates;
    }

    public virtual void Initialize(TType startType)
    {
        if (!states.ContainsKey(startType)) return;

        CurrentState = states[startType];
        CurrentState.Enter();
    }

    public virtual void ChangeState(TType type)
    {
        if (!states.ContainsKey(type) || EqualityComparer<TState>.Default.Equals(CurrentState, states[type]))
            return;

        CurrentState.Exit();
        CurrentState = states[type];
        CurrentState.Enter();
    }

    public TState GetState(TType type)
    {
        if (states.ContainsKey(type))
        {
            return states[type];
        }
        Debug.LogError($"L'état {type} n'a pas été ajouté au dictionnaire de la StateMachine !");
        return null;
    }

    public void Update()
    {
        CurrentState?.Update();
    }

    public void FixedUpdate()
    {
        CurrentState?.FixedUpdate();
    }
}