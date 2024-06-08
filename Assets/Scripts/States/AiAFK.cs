public class AiAFK : AiState
{
    public void Enter(AiAgent agent)
    {
        agent.isListening = true;
    }

    public void Exit(AiAgent agent)
    {
        agent.isListening = false;
    }

    public AiStateId GetId()
    {
        return AiStateId.AFK;
    }

    public void Update(AiAgent agent)
    {

    }
}
