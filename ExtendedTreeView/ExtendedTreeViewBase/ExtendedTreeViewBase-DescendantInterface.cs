namespace CustomControls
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Windows.Controls.Primitives;

    /// <summary>
    /// Represents a control with a tree of nodes that can be moved around with Drag and Drop.
    /// </summary>
    public abstract partial class ExtendedTreeViewBase : MultiSelector
    {
        /// <summary>
        /// Checks if an item is the current content.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>True if equal to the current content; otherwise, false.</returns>
        protected abstract bool IsContent(object item);

        /// <summary>
        /// Checks if an item is of the same type as the current content.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>True if of the same type as the current content; otherwise, false.</returns>
        protected abstract bool IsSameTypeAsContent(object? item);

        /// <summary>
        /// Gets the parent of an item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The parent item.</returns>
        protected abstract object? GetItemParent(object item);

        /// <summary>
        /// Gets the number of children of an item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The number of children.</returns>
        protected abstract int GetItemChildrenCount(object item);

        /// <summary>
        /// Gets children of an item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The children.</returns>
        protected abstract IList GetItemChildren(object item);

        /// <summary>
        /// Gets the child of an item at the provided index.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="index">The child index.</param>
        /// <returns>The child at the provided index.</returns>
        protected abstract object GetItemChild(object item, int index);

        /// <summary>
        /// Installs event handlers on an item.
        /// </summary>
        /// <param name="item">The item.</param>
        protected abstract void InstallHandlers(object item);

        /// <summary>
        /// Uninstalls event handlers from an item.
        /// </summary>
        /// <param name="item">The item.</param>
        protected abstract void UninstallHandlers(object item);

        /// <summary>
        /// Moves items from source to destination.
        /// </summary>
        /// <param name="sourceItem">The source item.</param>
        /// <param name="destinationItem">The destination item.</param>
        /// <param name="itemList">Moved children.</param>
        protected abstract void DragDropMove(object sourceItem, object destinationItem, IList? itemList);

        /// <summary>
        /// Copy items from source to destination.
        /// </summary>
        /// <param name="sourceItem">The source item.</param>
        /// <param name="destinationItem">The destination item.</param>
        /// <param name="itemList">Children at the source.</param>
        /// <param name="cloneList">Cloned children at the destination.</param>
        protected abstract void DragDropCopy(object sourceItem, object destinationItem, IList itemList, IList cloneList);

        /// <summary>
        /// Inserts child items starting from the content root.
        /// </summary>
        protected abstract void InsertChildrenFromRootDontNotify();

        /// <summary>
        /// Creates a list of items.
        /// </summary>
        /// <returns>The created list of items.</returns>
        protected virtual IList CreateItemList()
        {
            return new List<object>();
        }

        /// <summary>
        /// Sets the dragged items.
        /// </summary>
        /// <param name="dragSource">The drag source.</param>
        /// <param name="itemList">The list of dragged items.</param>
        protected abstract void SetDragItemList(IDragSourceControl dragSource, IList itemList);

        /// <summary>
        /// Clears the dragged items.
        /// </summary>
        /// <param name="dragSource">The drag source.</param>
        protected abstract void ClearDragItemList(IDragSourceControl dragSource);

        /// <summary>
        /// Creates a context for inserting.
        /// </summary>
        /// <param name="item">The item where insertion takes place.</param>
        /// <param name="shownIndex">Index where insertion takes place.</param>
        /// <returns>The context.</returns>
        protected virtual IInsertItemContext CreateInsertItemContext(object item, int shownIndex)
        {
            return new InsertItemContext(shownIndex);
        }

        /// <summary>
        /// Creates a context for removing.
        /// </summary>
        /// <param name="item">The item where removal takes place.</param>
        /// <param name="shownIndex">Index where removal takes place.</param>
        /// <returns>The context.</returns>
        protected virtual IRemoveItemContext CreateRemoveItemContext(object item, int shownIndex)
        {
            return new RemoveItemContext(shownIndex);
        }

        /// <summary>
        /// Creates the control used for drag and drop.
        /// </summary>
        /// <returns>The control.</returns>
        protected virtual IDragSourceControl CreateSourceControl()
        {
            return new DragSourceControl(this);
        }
    }
}
