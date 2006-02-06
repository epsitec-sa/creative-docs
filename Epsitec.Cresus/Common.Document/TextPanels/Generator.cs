using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Text;

namespace Epsitec.Common.Document.TextPanels
{
	/// <summary>
	/// La classe Generator permet de choisir les puces et les numérotations.
	/// </summary>
	[SuppressBundleSupport]
	public class Generator : Abstract
	{
		public Generator(Document document, bool isStyle, StyleCategory styleCategory) : base(document, isStyle, styleCategory)
		{
			this.label.Text = Res.Strings.TextPanel.Generator.Title;

			this.fixIcon.Text = Misc.Image("TextGenerator");
			ToolTip.Default.SetToolTip(this.fixIcon, Res.Strings.TextPanel.Generator.Title);

			this.fieldType = new TextFieldCombo(this);
			this.fieldType.Text = "Aucun";
			this.fieldType.IsReadOnly = true;
			this.fieldType.AutoFocus = false;
			this.fieldType.ClosedCombo += new EventHandler(this.HandleTypeChanged);
			this.fieldType.TabIndex = this.tabIndex++;
			this.fieldType.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.fieldType.Items.Add(Res.Strings.TextPanel.Generator.Type.None);
			this.fieldType.Items.Add(Res.Strings.TextPanel.Generator.Type.Bullet1);
			this.fieldType.Items.Add(Res.Strings.TextPanel.Generator.Type.Bullet2);
			this.fieldType.Items.Add(Res.Strings.TextPanel.Generator.Type.Num1);
			this.fieldType.Items.Add(Res.Strings.TextPanel.Generator.Type.Num2);
			this.fieldType.Items.Add(Res.Strings.TextPanel.Generator.Type.Num3);
			this.fieldType.Items.Add(Res.Strings.TextPanel.Generator.Type.Custom);

			this.radioLevel = new RadioButton[Generator.maxLevel];
			for ( int i=0 ; i<Generator.maxLevel ; i++ )
			{
				this.radioLevel[i] = new RadioButton(this);
				this.radioLevel[i].Name = i.ToString(System.Globalization.CultureInfo.InvariantCulture);
				this.radioLevel[i].Text = i.ToString();
				this.radioLevel[i].Group = "Level";
				this.radioLevel[i].Clicked += new MessageEventHandler(this.HandleRadioLevelClicked);
			}
			this.radioLevel[0].Text = Res.Strings.TextPanel.Generator.Radio.Global;
			this.radioLevel[0].ActiveState = ActiveState.Yes;

			this.fieldPrefix = new TextFieldCombo(this);
			this.fieldPrefix.AutoFocus = false;
			this.fieldPrefix.TextChanged += new EventHandler(this.HandlePrefixChanged);
			this.fieldPrefix.TabIndex = this.tabIndex++;
			this.fieldPrefix.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.fieldPrefix.Items.Add(" ");
			this.fieldPrefix.Items.Add("o");
			this.fieldPrefix.Items.Add("-");
			this.fieldPrefix.Items.Add("(");
			ToolTip.Default.SetToolTip(this.fieldPrefix, Res.Strings.TextPanel.Generator.Tooltip.Prefix);

			this.fieldGenerator = new TextFieldCombo(this);
			this.fieldGenerator.IsReadOnly = true;
			this.fieldGenerator.AutoFocus = false;
			this.fieldGenerator.TextChanged += new EventHandler(this.HandleGeneratorChanged);
			this.fieldGenerator.TabIndex = this.tabIndex++;
			this.fieldGenerator.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.fieldGenerator.Items.Add(" ");
			this.fieldGenerator.Items.Add("1, 2...");
			this.fieldGenerator.Items.Add("A, B...");
			this.fieldGenerator.Items.Add("a, b...");
			this.fieldGenerator.Items.Add("i, ii...");
			this.fieldGenerator.Items.Add("I, II...");
			ToolTip.Default.SetToolTip(this.fieldGenerator, Res.Strings.TextPanel.Generator.Tooltip.Generator);

			this.fieldSuffix = new TextFieldCombo(this);
			this.fieldSuffix.AutoFocus = false;
			this.fieldSuffix.TextChanged += new EventHandler(this.HandleSuffixChanged);
			this.fieldSuffix.TabIndex = this.tabIndex++;
			this.fieldSuffix.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.fieldSuffix.Items.Add(" ");
			this.fieldSuffix.Items.Add(".");
			this.fieldSuffix.Items.Add("-");
			this.fieldSuffix.Items.Add(")");
			ToolTip.Default.SetToolTip(this.fieldSuffix, Res.Strings.TextPanel.Generator.Tooltip.Suffix);

			this.buttonClear = this.CreateClearButton(new MessageEventHandler(this.HandleClearClicked));

			this.ParagraphWrapper.Active.Changed  += new EventHandler(this.HandleWrapperChanged);
			this.ParagraphWrapper.Defined.Changed += new EventHandler(this.HandleWrapperChanged);

			this.Type = "None";
			this.Level = 0;

			this.isNormalAndExtended = true;
			this.UpdateAfterChanging();
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.fieldType.ClosedCombo -= new EventHandler(this.HandleTypeChanged);

				for ( int i=0 ; i<Generator.maxLevel ; i++ )
				{
					this.radioLevel[i].Clicked -= new MessageEventHandler(this.HandleRadioLevelClicked);
				}

				this.fieldPrefix.TextChanged -= new EventHandler(this.HandlePrefixChanged);
				this.fieldGenerator.TextChanged -= new EventHandler(this.HandleGeneratorChanged);
				this.fieldSuffix.TextChanged -= new EventHandler(this.HandleSuffixChanged);

				this.ParagraphWrapper.Active.Changed  -= new EventHandler(this.HandleWrapperChanged);
				this.ParagraphWrapper.Defined.Changed -= new EventHandler(this.HandleWrapperChanged);
			}
			
			base.Dispose(disposing);
		}

		
		public override void UpdateAfterAttach()
		{
			//	Mise à jour après avoir attaché le wrappers.
			this.UpdateButtonClear();
		}


		public override double DefaultHeight
		{
			//	Retourne la hauteur standard.
			get
			{
				double h = this.LabelHeight;

				if ( this.isExtendedSize )  // panneau étendu ?
				{
					h += 105;
				}
				else	// panneau réduit ?
				{
					h += 30;
				}

				return h;
			}
		}


		protected void HandleWrapperChanged(object sender)
		{
			//	Le wrapper associé a changé.
			this.UpdateAfterChanging();
		}


		protected void CreateGenerator(string type)
		{
			Text.TabList tabs = this.document.TextContext.TabList;
			Text.ParagraphManagers.ItemListManager.Parameters p;

			string[] user = new string[1];
			user[0] = string.Concat("Type=", type);

			if ( type == "None" )
			{
				this.ParagraphWrapper.Defined.ItemListParameters = null;
			}

			if ( type == "Bullet1" || (type == "Custom" && this.ParagraphWrapper.Defined.ItemListParameters == null) )
			{
				p = new Text.ParagraphManagers.ItemListManager.Parameters();
				p.Generator = this.document.TextContext.GeneratorList.NewGenerator();

				p.Generator.Add(Common.Text.Generator.CreateSequence(Common.Text.Generator.SequenceType.Constant, "", "", Common.Text.Generator.Casing.Default, "\u25CF"));
				p.Generator.Add(Common.Text.Generator.CreateSequence(Common.Text.Generator.SequenceType.Constant, "", "", Common.Text.Generator.Casing.Default, "\u25CB", true));
				p.Generator.Add(Common.Text.Generator.CreateSequence(Common.Text.Generator.SequenceType.Constant, "", "", Common.Text.Generator.Casing.Default, "-", true));
				
				Text.ParagraphManagers.ItemListManager.Parameters items = new Text.ParagraphManagers.ItemListManager.Parameters();
				items.Generator = p.Generator;
				items.TabItem   = tabs.NewTab(Common.Text.TabList.GenericSharedName, 0.0, Common.Text.Properties.SizeUnits.Points, 0.5, null, TabPositionMode.LeftRelative,       TabList.PackToAttribute("Em:1"));
				items.TabBody   = tabs.NewTab(Common.Text.TabList.GenericSharedName, 0.0, Common.Text.Properties.SizeUnits.Points, 0.0, null, TabPositionMode.LeftRelativeIndent, TabList.PackToAttribute("Em:2"));
				items.Font      = new Text.Properties.FontProperty("Arial", "Regular");

				p.Generator.UserData = user;
				this.ParagraphWrapper.Defined.ItemListParameters = p;
			}

			if ( type == "Custom" )
			{
				p = this.ParagraphWrapper.Defined.ItemListParameters;
				p.Generator.UserData = user;
				this.ParagraphWrapper.Defined.ItemListParameters = p;
			}


				
				
#if false				
			Text.TabList tabs = this.document.TextContext.TabList;

			Text.TextStyle[] baseStyles = new Text.TextStyle[] { paraStyle };
		
			Text.Generator generator1 = this.document.TextContext.GeneratorList.NewGenerator("bullet-1");
		
			generator1.Add(Text.Generator.CreateSequence(Text.Generator.SequenceType.Constant, "", "", Text.Generator.Casing.Default, "\u25CF"));
			generator1.Add(Text.Generator.CreateSequence(Text.Generator.SequenceType.Constant, "", "", Text.Generator.Casing.Default, "\u25CB", true));
			generator1.Add(Text.Generator.CreateSequence(Text.Generator.SequenceType.Constant, "", "", Text.Generator.Casing.Default, "-", true));
		
			generator1[1].ValueProperties = new Text.Property[] { new Text.Properties.FontColorProperty(red) };
			generator1[2].ValueProperties = new Text.Property[] { new Text.Properties.FontColorProperty(green) };
		
			Text.ParagraphManagers.ItemListManager.Parameters items1 = new Text.ParagraphManagers.ItemListManager.Parameters();

			items1.Generator = generator1;
			items1.TabItem   = tabs.NewTab(Text.TabList.GenericSharedName, 0.0, Text.Properties.SizeUnits.Points, 0.5, null, TabPositionMode.LeftRelative,       TabList.PackToAttribute ("Em:1"));
			items1.TabBody   = tabs.NewTab(Text.TabList.GenericSharedName, 0.0, Text.Properties.SizeUnits.Points, 0.0, null, TabPositionMode.LeftRelativeIndent, TabList.PackToAttribute ("Em:2"));
			items1.Font      = new Text.Properties.FontProperty ("Arial", "Regular");
		
			Text.Properties.ManagedParagraphProperty itemList1 = new Text.Properties.ManagedParagraphProperty("ItemList", items1.Save());
		
			Text.TextStyle l1 = this.document.TextContext.StyleList.NewTextStyle(null, "BulletRound", Text.TextStyleClass.Paragraph, new Text.Property[] { itemList1 }, baseStyles);
		
			int rank = 1;
			this.document.TextContext.StyleList.StyleMap.SetRank(null, l1, rank++);
			this.document.TextContext.StyleList.StyleMap.SetCaption(null, l1, "Liste à puces");
#endif
		}


		protected static string ConvTypeToText(string type)
		{
			switch ( type )
			{
				case "None":     return Res.Strings.TextPanel.Generator.Type.None;
				case "Bullet1":  return Res.Strings.TextPanel.Generator.Type.Bullet1;
				case "Bullet2":  return Res.Strings.TextPanel.Generator.Type.Bullet2;
				case "Num1":     return Res.Strings.TextPanel.Generator.Type.Num1;
				case "Num2":     return Res.Strings.TextPanel.Generator.Type.Num2;
				case "Num3":     return Res.Strings.TextPanel.Generator.Type.Num3;
				case "Custom":   return Res.Strings.TextPanel.Generator.Type.Custom;
			}
			return "?";
		}

		protected static string ConvTextToType(string text)
		{
			if ( text == Res.Strings.TextPanel.Generator.Type.None    )  return "None";
			if ( text == Res.Strings.TextPanel.Generator.Type.Bullet1 )  return "Bullet1";
			if ( text == Res.Strings.TextPanel.Generator.Type.Bullet2 )  return "Bullet2";
			if ( text == Res.Strings.TextPanel.Generator.Type.Num1    )  return "Num1";
			if ( text == Res.Strings.TextPanel.Generator.Type.Num2    )  return "Num2";
			if ( text == Res.Strings.TextPanel.Generator.Type.Num3    )  return "Num3";
			if ( text == Res.Strings.TextPanel.Generator.Type.Custom  )  return "Custom";
			return null;
		}

		
		protected override void UpdateClientGeometry()
		{
			//	Met à jour la géométrie.
			base.UpdateClientGeometry();

			if ( this.buttonClear == null )  return;

			Rectangle rect = this.UsefulZone;
			Rectangle r;

			r = rect;
			r.Bottom = r.Top-20;
			r.Right = rect.Right-25;
			this.fieldType.Bounds = r;
			r.Left = rect.Right-20;
			r.Right = rect.Right;
			this.buttonClear.Bounds = r;

			if ( this.isExtendedSize )  // panneau étendu ?
			{
				r.Offset(0, -25);
				r.Left = rect.Left;
				r.Width = 31;
				r.Bottom = r.Top-20;
				for ( int i=0 ; i<Generator.maxLevel ; i++ )
				{
					this.radioLevel[i].Bounds = r;
					this.radioLevel[i].Visibility = true;
					r.Offset(r.Width, 0);
				}

				r.Offset(0, -25);
				r.Left = rect.Left;
				r.Width = 56;
				this.fieldPrefix.Bounds = r;
				this.fieldPrefix.Visibility = true;
				r.Offset(r.Width+5, 0);
				this.fieldGenerator.Bounds = r;
				this.fieldGenerator.Visibility = true;
				r.Offset(r.Width+5, 0);
				this.fieldSuffix.Bounds = r;
				this.fieldSuffix.Visibility = true;
			}
			else
			{
				for ( int i=0 ; i<Generator.maxLevel ; i++ )
				{
					this.radioLevel[i].Visibility = false;
				}

				this.fieldPrefix.Visibility = false;
				this.fieldGenerator.Visibility = false;
				this.fieldSuffix.Visibility = false;
			}

			this.UpdateButtonClear();
		}

		protected void UpdateButtonClear()
		{
			this.buttonClear.Visibility = !this.ParagraphWrapper.IsAttachedToDefaultParagraphStyle;
		}

		protected override void UpdateAfterChanging()
		{
			//	Met à jour après un changement du wrapper.
			base.UpdateAfterChanging();
			
			if ( this.ParagraphWrapper.IsAttached == false )  return;

			string type = "None";
			bool isGenerator = this.ParagraphWrapper.Defined.IsManagedParagraphDefined;
			if ( isGenerator )
			{
				Text.ParagraphManagers.ItemListManager.Parameters p = this.ParagraphWrapper.Defined.ItemListParameters;
				string[] user = p.Generator.UserData;
				if ( user != null )
				{
					foreach ( string data in user )
					{
						if ( data.StartsWith("Type=") )
						{
							type = data.Substring(5);
						}
					}
				}
			}

			this.ignoreChanged = true;

			this.fieldType.Text = Generator.ConvTypeToText(type);

			this.ProposalTextFieldCombo(this.fieldType, !isGenerator);
			this.ProposalTextFieldCombo(this.fieldPrefix, !isGenerator);
			this.ProposalTextFieldCombo(this.fieldGenerator, !isGenerator);
			this.ProposalTextFieldCombo(this.fieldSuffix, !isGenerator);

			this.ignoreChanged = false;
		}


		protected void UpdateWidgets()
		{
			bool custom = (this.type == "Custom");

			for ( int i=0 ; i<Generator.maxLevel ; i++ )
			{
				this.radioLevel[i].Enable = custom;
			}

			this.fieldPrefix.Enable = custom;
			this.fieldGenerator.Enable = (custom && this.level != 0);
			this.fieldSuffix.Enable = custom;
		}

		protected string Type
		{
			get
			{
				return this.type;
			}

			set
			{
				if ( this.type != value )
				{
					this.type = value;
					this.UpdateWidgets();
				}
			}
		}

		protected int Level
		{
			get
			{
				return this.level;
			}

			set
			{
				if ( this.level != value )
				{
					this.level = value;
					this.UpdateWidgets();
				}
			}
		}


		private void HandleTypeChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.ParagraphWrapper.IsAttached )  return;

			TextFieldCombo combo = sender as TextFieldCombo;
			this.Type = Generator.ConvTextToType(combo.Text);
			this.CreateGenerator(this.type);
		}

		private void HandleRadioLevelClicked(object sender, MessageEventArgs e)
		{
			RadioButton button = sender as RadioButton;
			int level = int.Parse(button.Name);
			this.Level = level;
		}

		private void HandlePrefixChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.ParagraphWrapper.IsAttached )  return;

		}

		private void HandleGeneratorChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.ParagraphWrapper.IsAttached )  return;

		}

		private void HandleSuffixChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.ParagraphWrapper.IsAttached )  return;

		}

		private void HandleClearClicked(object sender, MessageEventArgs e)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.ParagraphWrapper.IsAttached )  return;

			this.ParagraphWrapper.SuspendSynchronizations();
			this.ParagraphWrapper.Defined.ItemListParameters = null;
			this.ParagraphWrapper.DefineOperationName("ParagraphGeneratorClear", Res.Strings.TextPanel.Clear);
			this.ParagraphWrapper.ResumeSynchronizations();
			this.document.IsDirtySerialize = true;
		}


		protected static readonly int		maxLevel = 6;

		protected RadioButton[]				radioLevel;
		protected TextFieldCombo			fieldType;
		protected TextFieldCombo			fieldPrefix;
		protected TextFieldCombo			fieldGenerator;
		protected TextFieldCombo			fieldSuffix;
		protected IconButton				buttonClear;

		protected string					type = null;
		protected int						level = -1;
	}
}
