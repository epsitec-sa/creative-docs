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
			this.panel_size.Width  = System.Math.Max (MethodProtoPanel.DefaultSize.Width, ParameterInfoPanel.DefaultSize.Width);
			this.panel_size.Height = MethodProtoPanel.DefaultSize.Height + ParameterInfoPanel.DefaultSize.Height;
			
			this.size = new Drawing.Size (this.panel_size.Width + 200, this.panel_size.Height);
		}
		
		
		public Source.Method					Method
		{
			get
			{
				if (this.IsModified)
				{
					this.UpdateFromUI ();
				}
				
				return this.method;
			}
			set
			{
				this.method = value;
				this.UpdateFromMethod ();
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
		
		public TextFieldMulti					SourceWidget
		{
			get
			{
				return this.text_source;
			}
		}
		
		
		protected override void CreateWidgets(Widget parent)
		{
			Widget panel = new Widget (parent);
			
			panel.TabIndex      = 10;
			panel.TabNavigation = Widget.TabNavigationMode.ForwardTabPassive;
			panel.PreferredSize = this.panel_size;
			panel.Dock          = DockStyle.Left;
			
			this.panel_proto = new MethodProtoPanel ();
			this.panel_param = new ParameterInfoPanel ();
			
			Widget widget_proto = this.panel_proto.Widget;
			Widget widget_param = this.panel_param.Widget;
			
			widget_proto.SetEmbedder (panel);
			widget_param.SetEmbedder (panel);
			
			widget_proto.TabNavigation = Widget.TabNavigationMode.ForwardTabPassive;
			widget_proto.TabIndex      = 15;
			
			widget_param.TabNavigation = Widget.TabNavigationMode.ForwardTabPassive;
			widget_param.TabIndex      = 20;
			
			widget_proto.Dock = DockStyle.Top;
			widget_param.Dock = DockStyle.Fill;
			
			this.panel_proto.IsModifiedChanged += new EventHandler (this.HandlePanelProtoIsModifiedChanged);
			this.panel_param.IsModifiedChanged += new EventHandler (this.HandlePanelParamIsModifiedChanged);
			
			this.text_source = new TextFieldMulti (parent);

			Drawing.Font font_face = Drawing.Font.GetFont ("Courier New", "Regular");
			double       font_size = 13.0;
			
			this.text_source.Dock = DockStyle.Fill;
			this.text_source.Margins = new Drawing.Margins (4, 0, 0, 0);
			this.text_source.TextLayout.DefaultFont     = font_face;
			this.text_source.TextLayout.DefaultFontSize = font_size;
			this.text_source.TextEdited += new EventHandler (this.HandleTextSourceEdited);
			
			this.text_source.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.text_source.TabIndex      = 10;
			this.text_source.MaxChar       = 100000;
			
			this.text_source.TextLayout.Style.DefaultTabWidth = font_face.GetCharAdvance ('x') * font_size * this.text_tab_char_width;
			
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
		
		protected virtual void UpdateFromUI()
		{
			string           method_name = this.MethodProtoPanel.MethodName;
			Types.INamedType return_type = this.MethodProtoPanel.MethodType;
			Source.ParameterInfo[] infos = this.ParameterInfoPanel.Parameters;
			Source.CodeSection[]   codes = new Source.CodeSection[1];
			
			codes[0] = new Source.CodeSection (Source.CodeType.Local, this.text_source.Text);
			
			this.method = new Source.Method (method_name, return_type, infos, codes);
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
		protected int							text_tab_char_width = 4;
	}
}
