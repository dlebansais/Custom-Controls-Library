using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace CustomControls
{
    public interface IStatusTheme
    {
        Brush GetBackgroundBrush(object value);
        Brush GetForegroundBrush(object value);
    }

    public class StatusTheme : IStatusTheme
    {
        #region Properties
        public Dictionary<StatusType, Brush> DefaultStatusTypeBackgroundBrushes { get; } = new Dictionary<StatusType, Brush>()
        {
            { StatusType.Normal, Brushes.White },
            { StatusType.Success, Brushes.LightGreen },
            { StatusType.Failure, Brushes.Red },
            { StatusType.Warning, Brushes.Orange },
            { StatusType.Busy, Brushes.Blue }
        };

        public Dictionary<StatusType, Brush> DefaultStatusTypeForegroundBrushes { get; } = new Dictionary<StatusType, Brush>()
        {
            { StatusType.Normal, Brushes.Black },
            { StatusType.Success, Brushes.Black },
            { StatusType.Failure, Brushes.Black },
            { StatusType.Warning, Brushes.Black },
            { StatusType.Busy, Brushes.White }
        };
        #endregion

        #region Client Interface
        public virtual Brush GetBackgroundBrush(object value)
        {
            if (value is StatusType AsStatusType)
                return GetStatusTypeBackgroundBrush((StatusType)value);
            else
                throw new ArgumentOutOfRangeException(nameof(value));
        }

        public virtual Brush GetForegroundBrush(object value)
        {
            if (value is StatusType AsStatusType)
                return GetStatusTypeForegroundBrush((StatusType)value);
            else
                throw new ArgumentOutOfRangeException(nameof(value));
        }
        #endregion

        #region Implementation
        protected virtual Brush GetStatusTypeBackgroundBrush(StatusType statusType)
        {
            return DefaultStatusTypeBackgroundBrushes[statusType];
        }

        protected virtual Brush GetStatusTypeForegroundBrush(StatusType statusType)
        {
            return DefaultStatusTypeForegroundBrushes[statusType];
        }
        #endregion
    }
}
