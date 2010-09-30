//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

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

		public static MarshalerValidator CreateValidator(Widget widget, Marshaler marshaler)
		{
			return new MarshalerValidator (widget, marshaler);
		}

		/// <summary>
		/// Gets or sets the marshaler used to validate the widget.
		/// </summary>
		/// <value>The marshaler.</value>
		public Marshaler Marshaler
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

		public System.Predicate<string> AdditionalPredicate
		{
			get
			{
				return this.predicate;
			}
			set
			{
				this.predicate = value;
				this.Predicate = this.CreateComposedPredicate ();
			}
		}


		private System.Func<bool> CreateComposedPredicate()
		{
			if (this.predicate == null)
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
					return () => this.predicate (this.GetPredicateText ());
				}
				else
				{
					return
						delegate
						{
							string text = this.GetPredicateText ();
							return this.marshaler.CanConvert (text) && this.predicate (text);
						};
				}
			}
		}

		private string GetPredicateText()
		{
			return TextConverter.ConvertToSimpleText (this.Widget.Text);
		}

		private Marshaler marshaler;
		private System.Predicate<string> predicate;
	}
}
