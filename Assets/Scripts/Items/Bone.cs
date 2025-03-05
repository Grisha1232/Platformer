
using System;

[Serializable]
public class Bone : Item
{
    public Bone(string name = "Bone"): base(name) {

    }
    public override void Use()
    {
        GameManager.instance.ReturnToCheckpoint();
    }
}