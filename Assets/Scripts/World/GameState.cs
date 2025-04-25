using System;
using System.Collections.Generic;

[Serializable]
public class GameState {
    public GameState() {
        checkpoints = new();
        playerPositions = new();
        currency = new();
        countDeath = 0;
    }
    public Dictionary<string, (float x, float y)> checkpoints { get; set; }
    public Dictionary<string, (float x, float y)> playerPositions { get; set; }

    /// Player currency
    public int currency {get; set;}
    public int countDeath;

}