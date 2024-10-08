﻿namespace CustomControls;

/// <summary>
/// Represents the abstract interface for a modify item context.
/// </summary>
public interface IModifyContext
{
    /// <summary>
    /// Gets the index of the item.
    /// </summary>
    int ShownIndex { get; }

    /// <summary>
    /// Starts the operation.
    /// </summary>
    void Start();

    /// <summary>
    /// Moves to the next index.
    /// </summary>
    void NextIndex();

    /// <summary>
    /// Complete the operation.
    /// </summary>
    void Complete();

    /// <summary>
    /// Closes the context.
    /// </summary>
    void Close();
}

/// <summary>
/// Represents a modify item context.
/// </summary>
/// <param name="shownIndex">Index of the item.</param>
public abstract class ModifyContext(int shownIndex) : IModifyContext
{
    /// <summary>
    /// Gets or sets the index of the item.
    /// </summary>
    public int ShownIndex { get; protected set; } = shownIndex;

    /// <summary>
    /// Starts the operation.
    /// </summary>
    public virtual void Start()
    {
    }

    /// <summary>
    /// Moves to the next index.
    /// </summary>
    public abstract void NextIndex();

    /// <summary>
    /// Complete the operation.
    /// </summary>
    public virtual void Complete()
    {
    }

    /// <summary>
    /// Closes the context.
    /// </summary>
    public virtual void Close()
    {
    }
}
