//	Copyright � 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
			this.panel_size.Width  = System.Math.Max (MethodProtoPanel.DefaultSize.Width, ParameterInfoPanel.DefaultSize.Width);
			this.panel_size.Height = MethodProtoPanel.DefaultSize.Height + ParameterInfoPanel.DefaultSize.Height;
			
			this.size = new Drawing.Size (this.panel_size.Width + 200, this.panel_size.Height);
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
		
		public MethodProtoPanel					MethodProtoPanel
		{
			get
			{
				return this.panel_proto;
			}
		}
		
		public ParameterInfoPanel				ParameterInfoPanel
		{
			get
			{
				return this.panel_param;
			}
		}
		
		
		protected override void CreateWidgets(Widget parent)
		{
			Widget panel = new Widget (parent);
			
			panel.Size = this.panel_size;
			panel.Dock = DockStyle.Left;
			
			this.panel_proto = new MethodProtoPanel ();
			this.panel_param = new ParameterInfoPanel ();
			
			Widget widget_proto = this.panel_proto.Widget;
			Widget widget_param = this.panel_param.Widget;
			
			widget_proto.SetEmbedder (panel);
			widget_param.SetEmbedder (panel);
			
			widget_proto.Dock = DockStyle.Top;
			widget_param.Dock = DockStyle.Fill;
			
			this.panel_proto.IsModifiedChanged += new EventHandler (this.HandlePanelProtoIsModifiedChanged);
			this.panel_param.IsModifiedChanged += new EventHandler (this.HandlePanelParamIsModifiedChanged);
			
			this.text_source = new TextFieldMulti (parent);
			
			this.text_source.Dock = DockStyle.Fill;
			this.text_source.DockMargins = new Drawing.Margins (4, 0, 0, 0);
			this.text_source.TextLayout.DefaultFont = Drawing.Font.GetFont ("Courier New", "Regular");
			this.text_source.TextLayout.DefaultFontSize = 13.0;
			this.text_source.TextEdited += new EventHandler (this.HandleTextSourceEdited);
			
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
			
			this.text_source.Text       = this.method.CodeSections[0].Code;
			this.text_source.SelectAll ();
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
		
		private void HandleTextSourceEdited(object sender)
		{
			this.IsModified = true;
		}
		
		
		protected Drawing.Size					panel_size;
		protected MethodProtoPanel				panel_proto;
		protected ParameterInfoPanel			panel_param;
		
		protected Source.Method					method;
		protected TextFieldMulti				text_source;
	}
}
