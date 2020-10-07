namespace CustomControls
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Media;

    /// <summary>
    /// Represents a theme for a status control.
    /// </summary>
    public interface IStatusTheme
    {
        /// <summary>
        /// Gets the background brush.
        /// </summary>
        /// <param name="value">The status type.</param>
        /// <returns>The background brush.</returns>
        Brush GetBackgroundBrush(object value);

        /// <summary>
        /// Gets the foreground brush.
        /// </summary>
        /// <param name="value">The status type.</param>
        /// <returns>The background brush.</returns>
        Brush GetForegroundBrush(object value);
    }

    /// <summary>
    /// Represents a theme for a status control.
    /// </summary>
    public class StatusTheme : IStatusTheme
    {
        #region Properties
        /// <summary>
        /// Gets default background brushes.
        /// </summary>
        public Dictionary<StatusType, Brush> DefaultStatusTypeBackgroundBrushes { get; } = new Dictionary<StatusType, Brush>()
        {
            { StatusType.Normal, Brushes.White },
            { StatusType.Success, Brushes.LightGreen },
            { StatusType.Failure, Brushes.Red },
            { StatusType.Warning, Brushes.Orange },
            { StatusType.Busy, Brushes.Blue },
        };

        /// <summary>
        /// Gets default foreground brushes.
        /// </summary>
        public Dictionary<StatusType, Brush> DefaultStatusTypeForegroundBrushes { get; } = new Dictionary<StatusType, Brush>()
        {
            { StatusType.Normal, Brushes.Black },
            { StatusType.Success, Brushes.Black },
            { StatusType.Failure, Brushes.Black },
            { StatusType.Warning, Brushes.Black },
            { StatusType.Busy, Brushes.White },
        };
        #endregion

        #region Client Interface
        /// <summary>
        /// Gets the background brush.
        /// </summary>
        /// <param name="value">The status type.</param>
        /// <returns>The background brush.</returns>
        public virtual Brush GetBackgroundBrush(object value)
        {
            if (value is StatusType AsStatusType)
                return GetStatusTypeBackgroundBrush((StatusType)value);
            else
                throw new ArgumentOutOfRangeException(nameof(value));
        }

        /// <summary>
        /// Gets the foreground brush.
        /// </summary>
        /// <param name="value">The status type.</param>
        /// <returns>The background brush.</returns>
        public virtual Brush GetForegroundBrush(object value)
        {
            if (value is StatusType AsStatusType)
                return GetStatusTypeForegroundBrush((StatusType)value);
            else
                throw new ArgumentOutOfRangeException(nameof(value));
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Gets the background brush associated to a status type.
        /// </summary>
        /// <param name="statusType">The status type.</param>
        /// <returns>The background brush.</returns>
        protected virtual Brush GetStatusTypeBackgroundBrush(StatusType statusType)
        {
            return DefaultStatusTypeBackgroundBrushes[statusType];
        }

        /// <summary>
        /// Gets the foreground brush associated to a status type.
        /// </summary>
        /// <param name="statusType">The status type.</param>
        /// <returns>The foreground brush.</returns>
        protected virtual Brush GetStatusTypeForegroundBrush(StatusType statusType)
        {
            return DefaultStatusTypeForegroundBrushes[statusType];
        }
        #endregion
    }
}
