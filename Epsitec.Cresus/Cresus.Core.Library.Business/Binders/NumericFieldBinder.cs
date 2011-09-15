//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Types.Converters;

namespace Epsitec.Cresus.Core.Binders
{
	internal sealed class NumericFieldBinder : IFieldBinder, IFieldBinderProvider
	{
		public NumericFieldBinder()
		{
		}

		public NumericFieldBinder(int decimals, bool percent)
		{
			this.decimals = decimals;
			this.percent = percent;
		}

		#region IFieldBinder Members

		public string ConvertToUI(string value)
		{
			if (string.IsNullOrEmpty (value))
			{
				return value;
			}
			
			int posDot   = value.IndexOf ('.');
			int posComma = value.IndexOf (',');

			string before;
			string after;
			string separator;
			string zeroes;

			if (posDot >= 0)
			{
				separator = ".";
				before = value.Substring (0, posDot);
				after  = value.Substring (posDot+1);
			}
			else if (posComma >= 0)
			{
				separator = ",";
				before = value.Substring (0, posComma);
				after = value.Substring (posComma+1);
			}
			else
			{
				separator = (1.1M).ToString ().Substring (1, 1);
				before = value;
				after = "";
			}

			after = after.TrimEnd ('0');

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
				zeroes = new string ('0', this.decimals - after.Length);
			}
			else
			{
				zeroes = "";
			}

			if (this.percent)
			{
				zeroes = zeroes + "%";
			}

			return string.Concat (before, separator, after, zeroes);
		}

		public string ConvertFromUI(string value)
		{
			if (this.percent)
			{
				return NumericFieldBinder.RemovePercent (value);
			}
			else
			{
				return value;
			}
		}

		public IValidationResult ValidateFromUI(string value)
		{
			return new ValidationResult (ValidationState.Ok);
		}

		public void Attach(Marshaler marshaler)
		{
			if (this.percent)
			{
				marshaler.CustomizeConverter ();

				var converter = marshaler.GetConverter () as DecimalConverter;

				converter.Format     = "{0}";
				converter.Multiplier = 100;
				converter.Filter     = NumericFieldBinder.RemovePercent;
			}
		}

		#endregion

		#region IFieldBinderProvider Members

		public IFieldBinder GetFieldBinder(INamedType namedType)
		{
			if (namedType.DefaultControllerParameters.StartsWith ("Decimals:"))
			{
				var args = namedType.DefaultControllerParameters.Split (':');
				int decimals = InvariantConverter.ToInt (args[1]);
				return new NumericFieldBinder (decimals, percent: false);
			}
			if (namedType.DefaultControllerParameters.StartsWith ("Percentage:"))
			{
				var args = namedType.DefaultControllerParameters.Split (':');
				int decimals = InvariantConverter.ToInt (args[1]);
				return new NumericFieldBinder (decimals, percent: true);
			}
			return null;
		}

		#endregion

		private static string RemovePercent(string text)
		{
			return text.TrimEnd (' ', '%');
		}

		private readonly int decimals;
		private readonly bool percent;
	}
}
