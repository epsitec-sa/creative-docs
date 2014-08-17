//	Copyright © 2012-2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Binders
{
	internal sealed class PhoneNumberFieldBinder : IFieldBinder, IFieldBinderProvider
	{
		public PhoneNumberFieldBinder()
		{
		}

		#region IFieldBinder Members

		public FormattedText ConvertToUI(FormattedText value)
		{
			if (value.IsNullOrEmpty ())
			{
				return value;
			}

			var numbers = value.ToSimpleText ().Split (';').Select (x => x.Trim ());
			var formatted = string.Join (";", numbers.Select (x => Epsitec.TwixClip.TwixTel.FormatPhoneNumber (x)));

			return FormattedText.FromSimpleText (formatted);
		}

		public FormattedText ConvertFromUI(FormattedText value)
		{
			return FormattedText.FromSimpleText (Epsitec.TwixClip.TwixTel.ParsePhoneNumber (value.ToSimpleText ()));
		}

		public IValidationResult ValidateFromUI(FormattedText value)
		{
			if (Epsitec.TwixClip.TwixTel.IsValidPhoneNumber (value.ToSimpleText (), acceptEmptyNumbers: true))
			{
				return ValidationResult.Ok;
			}
			else
			{
				return ValidationResult.CreateError ("Numéro de téléphone incorrect. Pour la Suisse, le numéro a le format <i>021 345 67 89</i>, par exemple.");
			}
		}

		public void Attach(Marshaler marshaler)
		{
		}

		#endregion

		#region IFieldBinderProvider Members

		public IFieldBinder GetFieldBinder(INamedType namedType)
		{
			if (namedType.DefaultControllerParameters == "PhoneNumber")
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
