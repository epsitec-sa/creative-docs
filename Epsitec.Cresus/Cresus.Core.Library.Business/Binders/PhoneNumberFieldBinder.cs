//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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

		public string ConvertToUI(string value)
		{
			if (string.IsNullOrEmpty (value))
			{
				return value;
			}

			return Epsitec.TwixClip.TwixTel.FormatPhoneNumber (value);
		}

		public string ConvertFromUI(string value)
		{
			return Epsitec.TwixClip.TwixTel.ParsePhoneNumber (value);
		}

		public IValidationResult ValidateFromUI(string value)
		{
			if (Epsitec.TwixClip.TwixTel.IsValidPhoneNumber (value, acceptEmptyNumbers: true))
			{
				return new ValidationResult (ValidationState.Ok);
			}
			else
			{
				return new ValidationResult (ValidationState.Error, "Numéro de téléphone incorrect. Pour la Suisse, le numéro a le<br/>format 021 345 67 89, par exemple.");
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
