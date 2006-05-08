//	Copyright © 2004-2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets.Validators
{
	/// <summary>
	/// La classe NumRangeValidator permet de vérifier que la valeur est dans les
	/// bornes requises.
	/// </summary>
	public class NumRangeValidator : AbstractValidator
	{
		public NumRangeValidator() : base (null)
		{
		}
		
		public NumRangeValidator(Widget widget) : base (widget)
		{
			if (widget is Support.Data.INumValue)
			{
				//	OK.
			}
			else
			{
				throw new System.ArgumentException (string.Format ("Widget {0} does not support interface INumValue.", widget), "widget");
			}
		}
		
		
		
		public override void Validate()
		{
			Support.Data.INumValue num = this.widget as Support.Data.INumValue;
			Types.DecimalRange range = new Types.DecimalRange (num.MinValue, num.MaxValue, num.Resolution);
			
			if (range.CheckInRange (num.Value))
			{
				this.state = ValidationState.Ok;
			}
			else
			{
				this.state = ValidationState.Error;
			}
		}
		
		
		protected override void AttachWidget(Widget widget)
		{
			base.AttachWidget (widget);
			
			Support.Data.INumValue num = this.widget as Support.Data.INumValue;
			
			num.ValueChanged += new Support.EventHandler (this.HandleValueChanged);
		}
		
		protected override void DetachWidget(Widget widget)
		{
			base.DetachWidget (widget);
			
			Support.Data.INumValue num = this.widget as Support.Data.INumValue;
			
			num.ValueChanged -= new Support.EventHandler (this.HandleValueChanged);
		}
		
		
		private void HandleValueChanged(object sender)
		{
			this.MakeDirty (false);
		}
	}
}
