//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.Script.Developer.Panels
{
	/// <summary>
	/// Summary description for MethodProtoPanel.
	/// </summary>
	public class MethodProtoPanel : AbstractPanel
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
					
					if (this.text_name != null)
					{
						this.text_name.Text = this.method_name;
						this.text_name.SelectAll ();
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

		public Widget							MethodNameWidget
		{
			get
			{
				return this.text_name;
			}
		}
		
		
		protected override void CreateWidgets(Widget parent)
		{
			this.text_name  = new TextFieldEx (parent);
			this.combo_type = new TextFieldCombo (parent);
			
			this.type_info.FillTypeNames (this.combo_type.Items);
			
			StaticText label_1 = new StaticText (parent, "Method name");
			StaticText label_2 = new StaticText (parent, "Return type");
			
			double width = System.Math.Max (label_1.GetBestFitSize ().Width, label_2.GetBestFitSize ().Width) + 4;
			
			this.text_name.Text  = this.MethodName;
			this.combo_type.Text = this.type_info.GetNameFromType (this.MethodType);
			
			this.text_name.Anchor         = AnchorStyles.LeftAndRight | AnchorStyles.Top;
			this.text_name.Margins  = new Drawing.Margins (width, 0, 0, 0);
			this.text_name.DefocusAction  = DefocusAction.AutoAcceptOrRejectEdition;
			this.text_name.ButtonShowCondition = ShowCondition.WhenModified;
			
			this.combo_type.Anchor        = AnchorStyles.LeftAndRight | AnchorStyles.Top;
			this.combo_type.Margins = new Drawing.Margins (width, 0, this.text_name.PreferredHeight + 2, 0);
			this.combo_type.IsReadOnly    = true;
			
			label_1.Anchor         = AnchorStyles.TopLeft;
			label_1.PreferredWidth = width;
			
			label_2.Anchor         = AnchorStyles.TopLeft;
			label_2.PreferredWidth = width;
			
			Widget.BaseLineAlign (this.text_name, label_1);
			Widget.BaseLineAlign (this.combo_type, label_2);
			
			this.text_name.TextEdited  += new EventHandler (this.HandleTextNameTextEdited);
			this.text_name.TextChanged += new EventHandler (this.HandleTextNameTextChanged);
			
			this.combo_type.SelectedIndexChanged += new EventHandler (this.HandleComboTypeSelectedIndexChanged);
			
			new Widgets.Validators.RegexValidator (this.text_name, Support.RegexFactory.AlphaNumName, false);
		}
		
		
		private void HandleTextNameTextEdited(object sender)
		{
			this.IsModified = true;
		}
		
		private void HandleTextNameTextChanged(object sender)
		{
			this.method_name = this.text_name.Text;
		}
		
		private void HandleComboTypeSelectedIndexChanged(object sender)
		{
			this.method_type = this.type_info.GetTypeFromName (this.combo_type.Text);
			this.IsModified  = true;
		}
		
		
		protected string						method_name;
		protected Types.INamedType				method_type;
		
		protected Helpers.ParameterInfoStore	type_info;
		
		protected TextFieldEx					text_name;
		protected TextFieldCombo				combo_type;
	}
}
