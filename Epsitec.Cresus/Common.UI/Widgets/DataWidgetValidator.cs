//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.UI.Widgets
{
	using SuppressBundleSupportAttribute = Support.SuppressBundleSupportAttribute;
	
	/// <summary>
	/// La classe DataWidgetValidator implémente IValidator pour un DataWidget.
	/// </summary>
	
	[SuppressBundleSupport]
	
	public class DataWidgetValidator : Common.Widgets.Validators.AbstractValidator
	{
		public DataWidgetValidator(Common.Widgets.Widget widget) : base (widget)
		{
		}
		
		
		public override void Validate()
		{
			DataWidget  data_widget = this.widget as DataWidget;
			
			if ((data_widget == null) ||
				(data_widget.DataSource == null))
			{
				this.state = Support.ValidationState.Error;
				return;
			}
			
			Types.IDataValue      data_source = data_widget.DataSource;
			Types.IDataConstraint constraint  = data_source.DataConstraint;
			
			bool ok = data_source.IsValueValid;
			
			if ((ok) &&
				(constraint != null))
			{
				ok = constraint.CheckConstraint (data_source.ReadValue ());
			}
			
			this.state = ok ? Support.ValidationState.Ok : Support.ValidationState.Error;
		}
		
		
		protected override void AttachWidget(Common.Widgets.Widget widget)
		{
			base.AttachWidget (widget);
			
			DataWidget data_widget = widget as DataWidget;
			
			data_widget.DataChanged += new Support.EventHandler (this.HandleWidgetDataChanged);
		}
		
		protected override void DetachWidget(Common.Widgets.Widget widget)
		{
			base.DetachWidget (widget);
			
			DataWidget data_widget = widget as DataWidget;
			
			data_widget.TextChanged -= new Support.EventHandler (this.HandleWidgetDataChanged);
		}
		
		
		private void HandleWidgetDataChanged(object sender)
		{
			this.MakeDirty (false);
		}
	}
}

