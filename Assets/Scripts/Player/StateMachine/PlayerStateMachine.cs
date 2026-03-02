using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine
{
    public PlayerState CurrentState { get; private set; }

    private Dictionary<PlayerStateType, PlayerState> states;

    public PlayerStateMachine(Dictionary<PlayerStateType, PlayerState> allStates)
    {
        states = allStates;
    }

    public void Initialize(PlayerStateType startType)
    {
        CurrentState = states[startType];
        CurrentState.Enter();
    }

    public void ChangeState(PlayerStateType type)
    {
        if (CurrentState == states[type])
            return; // déjà dans cet état

        CurrentState.Exit();
        CurrentState = states[type];
        CurrentState.Enter();
    }

    public PlayerState GetState(PlayerStateType type)
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

public enum PlayerStateType
{
    Idle,
    Move,
    Attack,
    Roll,
    Hit,
    Jump,
    Fall,
    Aim,
    Equip,
    Unequip,
    Crouch,
    Death
}