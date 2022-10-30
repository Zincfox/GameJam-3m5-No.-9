using System;

public class TurnController
{
    public event Action AllPlayerTurnsFinished;
    public event Action EnemyTurnFinshed;

    public bool player1TurnEnd;
    public bool player2TurnEnd;
    public bool player3TurnEnd;
    public bool player4TurnEnd;

    public void SetPlayerTurnToFinish(int playerId)
    {
        switch (playerId)
        {
            case 0: player1TurnEnd = true; break;
            case 1: player2TurnEnd = true; break;
            case 2: player3TurnEnd = true; break;
            case 3: player4TurnEnd = true; break;
        }

        CheckPlayerTurnState();
    }

    private void CheckPlayerTurnState()
    {
        if (player1TurnEnd
            && player2TurnEnd
            && player3TurnEnd
            && player4TurnEnd)
        {
            AllPlayerTurnsFinished?.Invoke();
        }
    }

    public void SetEnemyTurnToFinish()
    {
        player1TurnEnd = false;
        player2TurnEnd = false;
        player3TurnEnd = false;
        player4TurnEnd = false;

        EnemyTurnFinshed?.Invoke();
    }
}
