//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.Script.Developer.Panels
{
	/// <summary>
	/// Summary description for MethodEditionPanel.
	/// </summary>
	public class MethodEditionPanel : AbstractPanel
	{
		public MethodEditionPanel()
		{
			this.size.Width  = System.Math.Max (MethodProtoPanel.DefaultSize.Width, ParameterInfoPanel.DefaultSize.Width);
			this.size.Height = MethodProtoPanel.DefaultSize.Height + ParameterInfoPanel.DefaultSize.Height;
		}
		
		
		public Source.Method					Method
		{
			get
			{
				return this.method;
			}
			set
			{
				if (this.method != value)
				{
					this.method = value;
					this.UpdateFromMethod ();
				}
			}
		}
		
		
		protected override void CreateWidgets(Widget parent)
		{
			this.panel_proto = new MethodProtoPanel ();
			this.panel_param = new ParameterInfoPanel ();
			
			Widget widget_proto = this.panel_proto.Widget;
			Widget widget_param = this.panel_param.Widget;
			
			widget_proto.SetEmbedder (parent);
			widget_param.SetEmbedder (parent);
			
			widget_proto.Dock = DockStyle.Top;
			widget_param.Dock = DockStyle.Top;
			
			this.panel_proto.ComboName.IsReadOnly          = false;
			this.panel_proto.ComboName.ButtonShowCondition = ShowCondition.Never;
			
			this.panel_proto.IsModifiedChanged += new EventHandler (this.HandlePanelProtoIsModifiedChanged);
			this.panel_param.IsModifiedChanged += new EventHandler (this.HandlePanelProtoIsModifiedChanged);
			
			this.UpdateFromMethod ();
		}
		
		protected virtual void UpdateFromMethod()
		{
			if ((this.method == null) ||
				(this.panel_param == null) ||
				(this.panel_proto == null))
			{
				return;
			}
			
			this.panel_proto.MethodName = this.method.Name;
			this.panel_proto.MethodType = this.method.ReturnType;
			this.panel_param.Parameters = this.method.Parameters;
		}
		
		
		protected override void OnIsModifiedChanged()
		{
			base.OnIsModifiedChanged ();
			
			if (this.IsModified == false)
			{
				this.panel_proto.IsModified = false;
				this.panel_param.IsModified = false;
			}
		}

		
		private void HandlePanelProtoIsModifiedChanged(object sender)
		{
			if (this.panel_proto.IsModified)
			{
				this.IsModified = true;
			}
		}
		
		private void HandlePanelParamIsModifiedChanged(object sender)
		{
			if (this.panel_param.IsModified)
			{
				this.IsModified = true;
			}
		}
		
		
		protected MethodProtoPanel				panel_proto;
		protected ParameterInfoPanel			panel_param;
		
		protected Source.Method					method;
	}
}
