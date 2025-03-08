public abstract class Item {
    /// <summary>
    /// Name of this item
    /// </summary>
    public string Name { get; protected set; }

    /// <summary>
    /// Determine wether you can drop / remove item from inventory
    /// </summary>
    public bool IsRemoveble { get; protected set; }

    public abstract void Use();
}