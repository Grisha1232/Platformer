
using System;

[Serializable]
public class Bone : Item
{
    public Bone() {

    }
    public override void Use()
    {
        GameManager.instance.ReturnToCheckpoint();
    }
}