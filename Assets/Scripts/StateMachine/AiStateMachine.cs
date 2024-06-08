using UnityEngine;

public class AiStateMachine
{
    public AiState[] states;
    public AiAgent agent;
    public AiStateId currentState;

    public AiStateMachine(AiAgent agent)
    {
        this.agent = agent; // Set agent
        int numsStates = System.Enum.GetNames(typeof(AiStateId)).Length; // Get length of states array 
        states = new AiState[numsStates]; // Fill lenght of states array
    }

    public void RegisterState(AiState state)
    {
        int index = (int)state.GetId();
        states[index] = state;  
    }

    public AiState GetState(AiStateId stateId) // Get state by index from states array;
    {
        int index = (int)stateId;
        return states[index];
    }

    public void Update() // Updating state machine to check states every frame
    {
        GetState(currentState)?.Update(agent);
    }

    public void ChangeState(AiStateId newState) // Exit state and get to the new one, set it as a current state
    {
        GetState(currentState)?.Exit(agent);
        currentState = newState;
        GetState(currentState)?.Enter(agent);
    }
}
