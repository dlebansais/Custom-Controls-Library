using System.Collections.Generic;
using System.Collections.ObjectModel;
using Verification;

namespace UndoRedo
{
    /// <summary>
    ///     Contains information about operations that can be performed or reversed.
    /// </summary>
    public class UndoRedoManager
    {
        #region Init
        /// <summary>
        ///     Initializes a new instance of the UndoRedoManager class.
        /// </summary>
        public UndoRedoManager()
        {
            UndoList = new ObservableCollection<IReversibleOperation>();
            RedoList = new ObservableCollection<IReversibleOperation>();
            LastOperation = null;
        }
        #endregion

        #region Properties
        /// <summary>
        ///     Gets the list of operation that can be reversed.
        /// </summary>
        /// <returns>
        ///     The list of operation that can be reversed.
        /// </returns>
        public ObservableCollection<IReversibleOperation> UndoList { get; private set; }

        /// <summary>
        ///     Gets the list of operation that have been reversed.
        /// </summary>
        /// <returns>
        ///     The list of operation that have been reversed.
        /// </returns>
        public ObservableCollection<IReversibleOperation> RedoList { get; private set; }

        /// <summary>
        ///     Gets the last operation that was either performed or reversed.
        /// </summary>
        /// <returns>
        ///     The last operation that was either performed or reversed. Can be null.
        /// </returns>
        public IReversibleOperation LastOperation { get; private set; }

        /// <summary>
        ///     Gets a value that indicate if there is an operation that can be reversed in <see cref="UndoList"/>.
        /// </summary>
        /// <returns>
        ///     True if there is an operation that can be reversed in <see cref="UndoList"/>. False otherwise.
        /// </returns>
        public virtual bool CanUndo
        {
            get { return UndoList.Count > 0; }
        }

        /// <summary>
        ///     Gets a value that indicate if there is an operation that can be performed in <see cref="RedoList"/>.
        /// </summary>
        /// <returns>
        ///     True if there is an operation that can be performed in <see cref="RedoList"/>. False otherwise.
        /// </returns>
        public virtual bool CanRedo
        {
            get { return RedoList.Count > 0; }
        }
        #endregion

        #region Client Interface
        /// <summary>
        ///     Returns the manager to a state with no operation that can be performed or reversed.
        /// </summary>
        public virtual void Reset()
        {
            UndoList.Clear();
            RedoList.Clear();
            LastOperation = null;
        }

        /// <summary>
        ///     Adds an operation to <see cref="RedoList"/> but without performing it.
        /// </summary>
        public virtual void AddOperation(IReversibleOperation operation)
        {
            Assert.ValidateReference(operation);

            RedoList.Clear();
            UndoList.Insert(0, operation);
            LastOperation = null;
        }

        /// <summary>
        ///     Adds an operation to <see cref="RedoList"/> and performs it.
        /// </summary>
        public virtual void AddAndExecuteOperation(IReversibleOperation operation)
        {
            Assert.ValidateReference(operation);

            RedoList.Clear();
            UndoList.Insert(0, operation);

            operation.Redo();
            LastOperation = operation;
        }

        /// <summary>
        ///     Removes the first operation in <see cref="UndoList"/>, reverse its effect and adds it to <see cref="RedoList"/>.
        /// </summary>
        public virtual void Undo()
        {
            Assert.CheckCondition(UndoList.Count > 0);

            IReversibleOperation Operation = UndoList[0];
            UndoList.RemoveAt(0);
            RedoList.Insert(0, Operation);

            Operation.Undo();
            LastOperation = Operation;
        }

        /// <summary>
        ///     Removes the first operation in <see cref="RedoList"/>, perform it again and adds it to <see cref="UndoList"/>.
        /// </summary>
        public virtual void Redo()
        {
            Assert.CheckCondition(RedoList.Count > 0);

            IReversibleOperation Operation = RedoList[0];
            RedoList.RemoveAt(0);
            UndoList.Insert(0, Operation);

            Operation.Redo();
            LastOperation = Operation;
        }
        #endregion
    }
}
