namespace CustomControls
{
    public interface IGestureTranslator
    {
        string PostTranslate(string rawGestureText);
    }

    public class GestureTranslator : IGestureTranslator
    {
        public virtual string PostTranslate(string rawGestureText)
        {
            if (rawGestureText == "OemPlus")
                return "+";

            else if (rawGestureText == "OemMinus")
                return "-";

            else
                return rawGestureText;
        }

    }
}
