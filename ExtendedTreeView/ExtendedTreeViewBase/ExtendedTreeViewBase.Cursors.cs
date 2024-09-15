namespace CustomControls;

using System;
using System.IO;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Contracts;

/// <summary>
/// Represents a control with a tree of nodes that can be moved around with Drag and Drop.
/// </summary>
public abstract partial class ExtendedTreeViewBase : MultiSelector
{
    /// <summary>
    /// Initializes cursors by index.
    /// </summary>
    /// <param name="cursorIndex">Index of the cursor.</param>
    /// <returns>The initialized cursor.</returns>
    protected static Cursor? InitializeCursor(int cursorIndex)
    {
        string SystemPath = Environment.GetFolderPath(Environment.SpecialFolder.System);
        string Ole32Path = Path.Combine(SystemPath, "ole32.dll");

        return LoadCursorFromResourceFile(Ole32Path, cursorIndex);
    }

    /// <summary>
    /// Loads a cursor from a file.
    /// </summary>
    /// <param name="filePath">The path to the file.</param>
    /// <param name="resourceId">The cursor resource Id.</param>
    /// <returns>The loaded cursor.</returns>
    protected static Cursor? LoadCursorFromResourceFile(string filePath, int resourceId)
    {
        CursorResource CursorFromResource = new(filePath, (uint)resourceId);
        return CursorFromResource.Load();
    }

    /// <summary>
    /// Gets the default forbidden cursor.
    /// </summary>
    protected static Cursor? DefaultCursorForbidden { get; } = InitializeCursor(1);

    /// <summary>
    /// Gets the default move cursor.
    /// </summary>
    protected static Cursor? DefaultCursorMove { get; } = InitializeCursor(2);

    /// <summary>
    /// Gets the default copy cursor.
    /// </summary>
    protected static Cursor? DefaultCursorCopy { get; } = InitializeCursor(3);
}
