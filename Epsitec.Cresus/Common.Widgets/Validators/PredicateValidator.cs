//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Common.Widgets.Validators;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.Widgets.Validators
{

	/// <summary>
	/// This delegate represents a predicate, i.e. a function with no arguments
	/// returning a boolean value.
	/// </summary>
	public delegate bool Predicate();

	/// <summary>
	/// This Validator uses a delegate given by the user to validate its widget.
	/// </summary>
	public class PredicateValidator : AbstractValidator
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="PredicateValidator"/> class.
		/// </summary>
		public PredicateValidator()
			: this (null, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PredicateValidator"/> class.
		/// </summary>
		/// <param name="widget">The widget to validate.</param>
		public PredicateValidator(Widget widget)
			: this (widget, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PredicateValidator"/> class.
		/// </summary>
		/// <param name="widget">The widget to validate.</param>
		/// <param name="predicate">The delegate used to validate.</param>
		public PredicateValidator(Widget widget, Predicate predicate)
			: base (widget)
		{
			this.Predicate = predicate;
		}

		/// <summary>
		/// Gets or sets the delegate used to validate the widget.
		/// </summary>
		/// <value>The predicate.</value>
		public Predicate Predicate
		{
			get;
			set;
		}

		/// <summary>
		/// Validates the widget based on the result of this instance's predicate. If this
		/// instance's predicate is not set, the validation state is set to Unknown.
		/// </summary>
		public override void Validate()
		{
			if (this.Predicate == null)
			{
				this.SetState (ValidationState.Unknown);
			}
			else if (this.Predicate ())
			{
				this.SetState (ValidationState.Ok);
			}
			else
			{
				this.SetState (ValidationState.Error);
			}
		}

	}

}
