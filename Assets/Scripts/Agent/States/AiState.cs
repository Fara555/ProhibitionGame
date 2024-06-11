public enum AiStateId // Every states of enemy
{
    ChasePlayer,
    Patrol,
    Attack,
    Laugh,
    Stunned,
    AFK
}

public interface AiState // Interface for implementing in every state
{
    AiStateId GetId(); // Get id of a state
    void Enter (AiAgent agent); // Compile code when entity enter the state
    void Update(AiAgent agent); // Compile code every frame, as an usual MonoBehaviour update
    void Exit (AiAgent agent); // Compile code when entity exit the state
}
