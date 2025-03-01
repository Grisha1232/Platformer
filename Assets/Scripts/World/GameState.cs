using System;
using System.Collections.Generic;

[Serializable]
public class GameState {
    /// Where player start over again
    public (float x, float y) checkpoint {get; set;}
    /// Player's inventory
    public List<Item> items {get; set;}
    /// Player currency
    public int currency {get; set;}
    public string NameScene {get; set;}

}