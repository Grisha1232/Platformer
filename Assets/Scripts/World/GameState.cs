using System;
using System.Collections.Generic;

[Serializable]
public class GameState {
    public GameState() {
        QuickItems = new();
        checkpoints = new();
        playerPositions = new();
        Items = new();
        currency = new();
        countDeath = 0;
    }
    public Dictionary<string, (float x, float y)> checkpoints { get; set; }
    public Dictionary<string, (float x, float y)> playerPositions { get; set; }

    /// Player's inventory
    public List<Item> Items {get; set;}
    public List<Item> QuickItems {get; set;}
    /// Player currency
    public int currency {get; set;}
    public int countDeath;

}