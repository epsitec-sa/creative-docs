//	Copyright © 2004-2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.Data;

namespace Epsitec.Common.Widgets.Validators
{
	/// <summary>
	/// The <c>SelectionValidator</c> class considers a value valid only if it
	/// belongs to an item returned through the attached widget's
	/// <see cref="IKeyedStringSelection"/> interface.
	/// </summary>
	public class SelectionValidator : AbstractValidator
	{
		public SelectionValidator()
			: base (null)
		{
		}
		
		
		public SelectionValidator(Widget widget)
			: base (widget)
		{
			if (widget is IKeyedStringSelection)
			{
				//	OK.
			}
			else
			{
				throw new System.ArgumentException (string.Format ("Widget {0} does not support interface IKeyedStringSelection", widget), "widget");
			}
		}
		
		
		public override void Validate()
		{
			var sel = this.Widget as IKeyedStringSelection;
			
			if ((sel.SelectedItemIndex >= 0) &&
				(this.IsSelectionValid (sel)))
			{
				this.SetState (ValidationState.Ok);
			}
			else
			{
				this.SetState (ValidationState.Error);
			}
		}
		
		
		protected virtual bool IsSelectionValid(IKeyedStringSelection selection)
		{
			return true;
		}
		
		
		protected override void AttachWidget(Widget widget)
		{
			base.AttachWidget (widget);
			
			var sel = this.Widget as IKeyedStringSelection;
			
			sel.SelectedItemChanged += this.HandleSelectionSelectedIndexChanged;
		}
		
		protected override void DetachWidget(Widget widget)
		{
			base.DetachWidget (widget);
			
			var sel = this.Widget as IKeyedStringSelection;
			
			sel.SelectedItemChanged -= this.HandleSelectionSelectedIndexChanged;
		}
		
		
		private void HandleSelectionSelectedIndexChanged(object sender)
		{
			this.MakeDirty (false);
		}
	}
}
