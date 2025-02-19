namespace AngleSharp.Css.Parser
{
    using AngleSharp.Css.Values;
    using AngleSharp.Text;
    using System;
    using System.Globalization;
    using System.Linq;

    /// <summary>
    /// Represents extensions to for number values.
    /// </summary>
    public static class NumberParser
    {
        /// <summary>
        /// Parses the number (double) value.
        /// </summary>
        public static Double? ParseNumber(this StringSource source)
        {
            var unit = source.ParseUnit();

            if (unit != null &&
                unit.Dimension == String.Empty &&
                Double.TryParse(unit.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var result))
            {
                return result;
            }

            return null;
        }

        /// <summary>
        /// Parses the ratio (double) value.
        /// </summary>
        public static Ratio? ParseRatio(this StringSource source)
        {
            var pos = source.Index;
            var top = source.ParseNumber();
            var c = source.SkipGetSkip();
            var bottom = source.ParseNumber();

            if (top.HasValue && bottom.HasValue && c == Symbols.Solidus)
            {
                return new Ratio(top.Value, bottom.Value);
            }

            source.BackTo(pos);
            return null;
        }

        /// <summary>
        /// Parses the integer (double) value.
        /// </summary>
        public static Double? ParseNaturalNumber(this StringSource source)
        {
            var pos = source.Index;
            var element = source.ParseNumber();

            if (element.HasValue && element.Value >= 0f)
            {
                return element;
            }

            source.BackTo(pos);
            return null;
        }

        /// <summary>
        /// Parses the natural number (double) value.
        /// </summary>
        public static Double? ParseGreaterOrEqualOneNumber(this StringSource source)
        {
            var pos = source.Index;
            var element = source.ParseNumber();

            if (element.HasValue && element.Value >= 1f)
            {
                return element;
            }

            source.BackTo(pos);
            return null;
        }

        /// <summary>
        /// Parses the natural integer (int) value.
        /// </summary>
        public static Int32? ParseNaturalInteger(this StringSource source)
        {
            var pos = source.Index;
            var element = source.ParseInteger();

            if (element.HasValue && element.Value >= 0)
            {
                return element;
            }

            source.BackTo(pos);
            return null;
        }

        /// <summary>
        /// Parses the positive integer (int) value.
        /// </summary>
        public static Int32? ParsePositiveInteger(this StringSource source)
        {
            var pos = source.Index;
            var element = source.ParseInteger();

            if (element.HasValue && element.Value > 0)
            {
                return element;
            }

            source.BackTo(pos);
            return null;
        }

        /// <summary>
        /// Parses the weight (int) value.
        /// </summary>
        public static Int32? ParseWeightInteger(this StringSource source)
        {
            var pos = source.Index;
            var element = source.ParsePositiveInteger();

            if (element.HasValue && IsWeight(element.Value))
            {
                return element;
            }

            source.BackTo(pos);
            return null;
        }

        /// <summary>
        /// Parses the binary (int) value.
        /// </summary>
        public static Int32? ParseBinary(this StringSource source)
        {
            var pos = source.Index;
            var element = source.ParseInteger();

            if (element.HasValue && (element.Value == 0 || element.Value == 1))
            {
                return element;
            }

            source.BackTo(pos);
            return null;
        }

        /// <summary>
        /// Parses the integer (int) value.
        /// </summary>
        public static Int32? ParseInteger(this StringSource source)
        {
            var unit = source.ParseUnit();

            if (unit != null && unit.Dimension == String.Empty)
            {
                var value = unit.Value;

                if (Int32.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result))
                {
                    return result;
                }

                var negative = value.StartsWith("-");
                var allNumbers = (negative ? value.Substring(1) : value).All(m => m.IsDigit());

                if (allNumbers)
                {
                    return negative ? Int32.MinValue : Int32.MaxValue;
                }
            }

            return null;
        }

        private static Boolean IsWeight(Int32 value)
        {
            if (value % 100 == 0)
            {
                var s = value / 100;
                return s > 0 && s < 10;
            }

            return false;
        }
    }
}
