//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.Script.Developer.Panels
{
	/// <summary>
	/// Summary description for MethodProtoPanel.
	/// </summary>
	public class MethodProtoPanel : Common.UI.AbstractPanel
	{
		public MethodProtoPanel()
		{
			this.size = MethodProtoPanel.DefaultSize;
			this.type_info = new Helpers.ParameterInfoStore ();
			this.type_info.IncludeVoidType ();
		}
		
		
		public static Drawing.Size				DefaultSize
		{
			get
			{
				return new Drawing.Size (200, 60);
			}
		}
		
		public string							MethodName
		{
			get
			{
				return this.method_name;
			}
			set
			{
				if (this.method_name != value)
				{
					this.method_name = value;
					
					if (this.combo_name != null)
					{
						this.combo_name.Text = this.method_name;
					}
				}
			}
		}
		
		public Types.INamedType					MethodType
		{
			get
			{
				return this.method_type;
			}
			set
			{
				if (this.method_type != value)
				{
					this.method_type = value;
					
					if (this.combo_type != null)
					{
						this.combo_type.Text = this.type_info.GetNameFromType (this.method_type);
					}
				}
			}
		}

		
		protected override void CreateWidgets(Widget parent)
		{
			this.combo_name = new TextFieldExList (parent);
			this.combo_type = new TextFieldCombo (parent);
			
			this.type_info.FillTypeNames (this.combo_type.Items);
			
			StaticText label_1 = new StaticText (parent, "Method name");
			StaticText label_2 = new StaticText (parent, "Return type");
			
			double width = System.Math.Max (label_1.GetBestFitSize ().Width, label_2.GetBestFitSize ().Width) + 4;
			
			this.combo_name.Text = this.MethodName;
			this.combo_type.Text = this.type_info.GetNameFromType (this.MethodType);
			
			this.combo_name.Anchor        = AnchorStyles.LeftAndRight | AnchorStyles.Top;
			this.combo_name.AnchorMargins = new Drawing.Margins (width, 0, 0, 0);
			
			this.combo_type.Anchor        = AnchorStyles.LeftAndRight | AnchorStyles.Top;
			this.combo_type.AnchorMargins = new Drawing.Margins (width, 0, this.combo_name.Height + 2, 0);
			this.combo_type.IsReadOnly    = true;
			
			label_1.Anchor = AnchorStyles.TopLeft;
			label_1.Width  = width;
			
			label_2.Anchor = AnchorStyles.TopLeft;
			label_2.Width  = width;
			
			Widget.BaseLineAlign (this.combo_name, label_1);
			Widget.BaseLineAlign (this.combo_type, label_2);
		}
		
		
		protected string						method_name;
		protected Types.INamedType				method_type;
		
		protected Helpers.ParameterInfoStore	type_info;
		
		protected TextFieldExList				combo_name;
		protected TextFieldCombo				combo_type;
	}
}
