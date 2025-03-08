using System;

[Serializable]
public class Item {

    public Item(string name) {
        Name = name;
        Description = name + " - Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.";
    }
    /// <summary>
    /// Name of this item
    /// </summary>
    public string Name { get; protected set; }

    public string Description { get; protected set; }

    /// <summary>
    /// Determine wether you can drop / remove item from inventory
    /// </summary>
    public bool IsRemoveble { get; protected set; }

    public virtual void Use() {

    }
}