//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core.Business;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Binders
{
	/// <summary>
	/// The <c>PostFinanceAccountFieldBinder</c> class implements field validation and conversion
	/// for PostFinance account numbers and ISR subscriber numbers, which follow the same convention.
	/// The type must be declared in the resources as having a <c>FieldBinder</c> controller with the
	/// <c>PostFinanceAccount</c> controller parameter.
	/// </summary>
	internal sealed class PostFinanceAccountFieldBinder : IFieldBinder, IFieldBinderProvider
	{
		#region IFieldBinder Members

		public string ConvertToUI(string value)
		{
			if (Isr.IsCompactSubscriberNumber (value))
			{
				value = Isr.FormatSubscriberNumber (value);
			}

			return value;
		}

		public string ConvertFromUI(string value)
		{
			string text;

			if (Isr.TryCompactSubscriberNumber (value, out text))
			{
				return text;
			}

			throw new System.FormatException ("Invalid format");
		}

		public IValidationResult ValidateFromUI(string value)
		{
			if (Isr.IsFormattedSubscriberNumber (value))
			{
				return new ValidationResult (ValidationState.Ok);
			}
			else
			{
				return new ValidationResult (ValidationState.Error, FormattedText.FromSimpleText ("Le n° ne respecte pas le format nn-nnn-n ou est incorrect"));
			}
		}

		public void Attach(Marshaler marshaler)
		{
		}

		#endregion

		#region IFieldBinderProvider Members

		public IFieldBinder GetFieldBinder(INamedType namedType)
		{
			if (namedType.DefaultControllerParameters == "PostFinanceAccount")
			{
				return this;
			}
			else
			{
				return null;
			}
		}

		#endregion
	}
}
