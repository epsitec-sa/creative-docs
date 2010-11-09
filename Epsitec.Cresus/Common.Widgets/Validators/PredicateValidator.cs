//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Common.Widgets.Validators;
using Epsitec.Common.Types;

namespace Epsitec.Common.Widgets.Validators
{
	/// <summary>
	/// The <c>PredicateValidator</c> class uses a predicate to validate its widget.
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
		public PredicateValidator(Widget widget, System.Func<bool> predicate)
			: base (widget)
		{
			this.Predicate = predicate;
		}

		/// <summary>
		/// Gets or sets the delegate used to validate the widget.
		/// </summary>
		/// <value>The predicate.</value>
		public System.Func<bool> Predicate
		{
			get
			{
				return this.predicate;
			}
			set
			{
				if (this.predicate != value)
                {
					this.predicate = value;
					this.SetState (ValidationState.Dirty);
                }
			}
		}

		/// <summary>
		/// Validates the widget based on the result of the predicate. If this
		/// predicate is not set, the validation state will be set to <c>Unknown</c>.
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

		private System.Func<bool> predicate;
	}
}
