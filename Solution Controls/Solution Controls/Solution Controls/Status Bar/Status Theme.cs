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
        #region Init
        public StatusTheme()
        {
            CreateDefaultBrushes();
        }
        #endregion

        #region Properties
        public Dictionary<StatusType, Brush> DefaultStatusTypeBackgroundBrushes { get; private set; }
        public Dictionary<StatusType, Brush> DefaultStatusTypeForegroundBrushes { get; private set; }
        #endregion

        #region Client Interface
        public virtual Brush GetBackgroundBrush(object value)
        {
            if (value is StatusType)
                return GetStatusTypeBackgroundBrush((StatusType)value);

            else
                return null;
        }

        public virtual Brush GetForegroundBrush(object value)
        {
            if (value is StatusType)
                return GetStatusTypeForegroundBrush((StatusType)value);

            else
                return null;
        }
        #endregion

        #region Implementation
        private void CreateDefaultBrushes()
        {
            DefaultStatusTypeBackgroundBrushes = new Dictionary<StatusType, Brush>()
            {
                { StatusType.Normal, Brushes.White },
                { StatusType.Success, Brushes.LightGreen },
                { StatusType.Failure, Brushes.Red },
                { StatusType.Warning, Brushes.Orange },
                { StatusType.Busy, Brushes.Blue }
            };

            DefaultStatusTypeForegroundBrushes = new Dictionary<StatusType, Brush>()
            {
                { StatusType.Normal, Brushes.Black },
                { StatusType.Success, Brushes.Black },
                { StatusType.Failure, Brushes.Black },
                { StatusType.Warning, Brushes.Black },
                { StatusType.Busy, Brushes.White }
            };
        }

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
