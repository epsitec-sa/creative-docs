//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Types.Converters.Binders
{
    internal sealed class NumericFieldBinder : IFieldBinder, IFieldBinderProvider
    {
        public NumericFieldBinder() { }

        public NumericFieldBinder(int decimals, bool percent)
        {
            this.decimals = decimals;
            this.percent = percent;
        }

        #region IFieldBinder Members

        public FormattedText ConvertToUI(FormattedText formattedValue)
        {
            if (formattedValue.IsNullOrEmpty())
            {
                return formattedValue;
            }

            var value = formattedValue.ToString();

            int posDot = value.IndexOf('.');
            int posComma = value.IndexOf(',');

            string before;
            string after;
            string separator;
            string zeroes;

            if (posDot >= 0)
            {
                separator = ".";
                before = value.Substring(0, posDot);
                after = value.Substring(posDot + 1);
            }
            else if (posComma >= 0)
            {
                separator = ",";
                before = value.Substring(0, posComma);
                after = value.Substring(posComma + 1);
            }
            else
            {
                separator = (1.1M).ToString().Substring(1, 1);
                before = value;
                after = "";
            }

            after = after.TrimEnd('0');

            if (after.Length == this.decimals)
            {
                if (this.decimals == 0)
                {
                    separator = "";
                }
                zeroes = "";
            }
            else if (after.Length < this.decimals)
            {
                zeroes = new string('0', this.decimals - after.Length);
            }
            else
            {
                zeroes = "";
            }

            if (this.percent)
            {
                zeroes = zeroes + "%";
            }

            return new FormattedText(string.Concat(before, separator, after, zeroes));
        }

        public FormattedText ConvertFromUI(FormattedText value)
        {
            if (this.percent)
            {
                return new FormattedText(NumericFieldBinder.RemovePercent(value.ToString()));
            }
            else
            {
                return value;
            }
        }

        public IValidationResult ValidateFromUI(FormattedText value)
        {
            return new ValidationResult(ValidationState.Ok);
        }

        public void Attach(Marshaler marshaler)
        {
            if (this.percent)
            {
                marshaler.CustomizeConverter();

                var converter = marshaler.GetConverter() as DecimalConverter;

                converter.Format = "{0}";
                converter.Multiplier = 100;
                converter.Filter = NumericFieldBinder.RemovePercent;
            }
        }

        #endregion

        #region IFieldBinderProvider Members

        public IFieldBinder GetFieldBinder(INamedType namedType)
        {
            if (namedType.DefaultControllerParameters.StartsWith("Decimals:"))
            {
                var args = namedType.DefaultControllerParameters.Split(':');
                int decimals = InvariantConverter.ToInt(args[1]);
                return new NumericFieldBinder(decimals, percent: false);
            }
            if (namedType.DefaultControllerParameters.StartsWith("Percentage:"))
            {
                var args = namedType.DefaultControllerParameters.Split(':');
                int decimals = InvariantConverter.ToInt(args[1]);
                return new NumericFieldBinder(decimals, percent: true);
            }
            return null;
        }

        #endregion

        private static string RemovePercent(string text)
        {
            return text.TrimEnd(' ', '%');
        }

        private readonly int decimals;
        private readonly bool percent;
    }
}
