//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Widgets.Validators;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.Widgets.Validators
{
	/// <summary>
	/// The <c>MarshalerValidator</c> uses a <see cref="Marshaler"/> to check for
	/// the validity of the input values.
	/// </summary>
	public class MarshalerValidator : PredicateValidator
	{
		public MarshalerValidator()
			: base ()
		{
		}

		public MarshalerValidator(Widget widget)
			: base (widget)
		{
		}

		public MarshalerValidator(Widget widget, Marshaler marshaler)
			: base (widget)
		{
			this.Marshaler = marshaler;
		}

		/// <summary>
		/// Gets or sets the marshaler used to validate the widget.
		/// </summary>
		/// <value>The marshaler.</value>
		public Marshaler						Marshaler
		{
			get
			{
				return this.marshaler;
			}
			set
			{
				this.marshaler = value;
				this.Predicate = this.CreateComposedPredicate ();
			}
		}

		public override FormattedText			ErrorMessage
		{
			get
			{
				return this.errorMessage;
			}
		}

		public System.Func<string, IValidationResult>	Validator
		{
			get
			{
				return this.validator;
			}
			set
			{
				this.validator = value;
				this.Predicate = this.CreateComposedPredicate ();
			}
		}


		public static MarshalerValidator CreateValidator(Widget widget, Marshaler marshaler)
		{
			return new MarshalerValidator (widget, marshaler);
		}

		private System.Func<bool> CreateComposedPredicate()
		{
			if (this.validator == null)
			{
				if (this.marshaler == null)
				{
					return () => true;
				}
				else
				{
					return () => this.marshaler.CanConvert (this.GetPredicateText ());
				}
			}
			else
			{
				if (this.marshaler == null)
				{
					return () => this.EvaluateValidator (this.GetPredicateText ());
				}
				else
				{
					return
						delegate
						{
							string text = this.GetPredicateText ();
							return this.marshaler.CanConvert (text) && this.EvaluateValidator (text);
						};
				}
			}
		}

		private string GetPredicateText()
		{
			return TextConverter.ConvertToSimpleText (this.Widget.Text);
		}

		private bool EvaluateValidator(string value)
		{
			var validationResult = this.validator (value);

			this.errorMessage = validationResult.ErrorMessage;
			
			return validationResult.IsValid;
		}

		private Marshaler marshaler;
		private System.Func<string, IValidationResult> validator;
		private FormattedText errorMessage;
	}
}
