﻿namespace UndoRedo;

using System;
using System.Collections.ObjectModel;
using System.Diagnostics;

/// <summary>
/// Contains information about operations that can be performed or reversed.
/// </summary>
public class UndoRedoManager
{
    #region Properties
    /// <summary>
    /// Gets the list of operation that can be reversed.
    /// </summary>
    /// <returns>
    /// The list of operation that can be reversed.
    /// </returns>
    public ObservableCollection<IReversibleOperation> UndoList { get; } = new();

    /// <summary>
    /// Gets the list of operation that have been reversed.
    /// </summary>
    /// <returns>
    /// The list of operation that have been reversed.
    /// </returns>
    public ObservableCollection<IReversibleOperation> RedoList { get; } = new();

    /// <summary>
    /// Gets the last operation that was either performed or reversed.
    /// </summary>
    /// <returns>
    /// The last operation that was either performed or reversed. Can be null.
    /// </returns>
    public IReversibleOperation LastOperation { get; private set; } = IdentityOperation.Default;

    /// <summary>
    /// Gets a value indicating whether there is an operation that can be reversed in <see cref="UndoList"/>.
    /// </summary>
    /// <returns>
    /// True if there is an operation that can be reversed in <see cref="UndoList"/>. False otherwise.
    /// </returns>
    public virtual bool CanUndo => UndoList.Count > 0;

    /// <summary>
    /// Gets a value indicating whether there is an operation that can be performed in <see cref="RedoList"/>.
    /// </summary>
    /// <returns>
    /// True if there is an operation that can be performed in <see cref="RedoList"/>. False otherwise.
    /// </returns>
    public virtual bool CanRedo => RedoList.Count > 0;
    #endregion

    #region Client Interface
    /// <summary>
    /// Returns the manager to a state with no operation that can be performed or reversed.
    /// </summary>
    public virtual void Reset()
    {
        UndoList.Clear();
        RedoList.Clear();
        LastOperation = IdentityOperation.Default;
    }

    /// <summary>
    /// Adds an operation to <see cref="RedoList"/> but without performing it.
    /// </summary>
    /// <param name="operation">The operation to add.</param>
    public virtual void AddOperation(IReversibleOperation operation)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(operation);
#else
        if (operation is null)
            throw new ArgumentNullException(nameof(operation));
#endif

        RedoList.Clear();
        UndoList.Insert(0, operation);
        LastOperation = IdentityOperation.Default;
    }

    /// <summary>
    /// Adds an operation to <see cref="RedoList"/> and performs it.
    /// </summary>
    /// <param name="operation">The operation to add and execute.</param>
    public virtual void AddAndExecuteOperation(IReversibleOperation operation)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(operation);
#else
        if (operation is null)
            throw new ArgumentNullException(nameof(operation));
#endif

        RedoList.Clear();
        UndoList.Insert(0, operation);

        operation.Redo();
        LastOperation = operation;
    }

    /// <summary>
    /// Removes the first operation in <see cref="UndoList"/>, reverse its effect and adds it to <see cref="RedoList"/>.
    /// </summary>
    public virtual void Undo()
    {
        Debug.Assert(UndoList.Count > 0);

        IReversibleOperation Operation = UndoList[0];
        UndoList.RemoveAt(0);
        RedoList.Insert(0, Operation);

        Operation.Undo();
        LastOperation = Operation;
    }

    /// <summary>
    /// Removes the first operation in <see cref="RedoList"/>, perform it again and adds it to <see cref="UndoList"/>.
    /// </summary>
    public virtual void Redo()
    {
        Debug.Assert(RedoList.Count > 0);

        IReversibleOperation Operation = RedoList[0];
        RedoList.RemoveAt(0);
        UndoList.Insert(0, Operation);

        Operation.Redo();
        LastOperation = Operation;
    }
    #endregion
}
