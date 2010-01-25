//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Common.Widgets.Validators;
using Epsitec.Common.Widgets;

namespace Epsitec.App.BanquePiguet
{

	public delegate bool Predicate();

	class PredicateValidator : AbstractValidator
	{

		public PredicateValidator() : this (null, null)
		{
		}

		public PredicateValidator(Widget widget) : this (widget, null)
		{
		}

		public PredicateValidator(Widget widget, Predicate predicate) : base (widget)
		{
			this.Predicate = predicate;
		}

		public Predicate Predicate
		{
			get;
			set;
		}

		public override void Validate()
		{
			if (this.Predicate())
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
