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
			this.fieldType.ComboClosed += new EventHandler(this.HandleTypeChanged);
			this.fieldType.TabIndex = this.tabIndex++;
			this.fieldType.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.InitComboType(this.fieldType);

			this.buttonLevel = new Widgets.IconMarkButton[Generator.maxLevel];
			for ( int i=0 ; i<Generator.maxLevel ; i++ )
			{
				this.buttonLevel[i] = new Widgets.IconMarkButton(this);
				this.buttonLevel[i].ButtonStyle = ButtonStyle.ActivableIcon;
				this.buttonLevel[i].AutoFocus = false;
				this.buttonLevel[i].Name = i.ToString(System.Globalization.CultureInfo.InvariantCulture);
				this.buttonLevel[i].Text = i.ToString();
				this.buttonLevel[i].Clicked += new MessageEventHandler(this.HandleLevelClicked);
			}
			this.buttonLevel[0].Text = Res.Strings.TextPanel.Generator.Radio.Global;
			this.buttonLevel[0].ActiveState = ActiveState.Yes;

			this.fieldPrefix = new TextFieldCombo(this);
			this.fieldPrefix.IsReadOnly = true;
			this.fieldPrefix.AutoFocus = false;
			this.fieldPrefix.TextChanged += new EventHandler(this.HandlePrefixChanged);
			this.fieldPrefix.TabIndex = this.tabIndex++;
			this.fieldPrefix.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.InitComboFix(this.fieldPrefix);
			ToolTip.Default.SetToolTip(this.fieldPrefix, Res.Strings.TextPanel.Generator.Tooltip.Prefix);

			this.fieldNumerator = new TextFieldCombo(this);
			this.fieldNumerator.IsReadOnly = true;
			this.fieldNumerator.AutoFocus = false;
			this.fieldNumerator.TextChanged += new EventHandler(this.HandleNumeratorChanged);
			this.fieldNumerator.TabIndex = this.tabIndex++;
			this.fieldNumerator.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.InitComboNumerator(this.fieldNumerator);
			ToolTip.Default.SetToolTip(this.fieldNumerator, Res.Strings.TextPanel.Generator.Tooltip.Numerator);

			this.fieldSuffix = new TextFieldCombo(this);
			this.fieldSuffix.IsReadOnly = true;
			this.fieldSuffix.AutoFocus = false;
			this.fieldSuffix.TextChanged += new EventHandler(this.HandleSuffixChanged);
			this.fieldSuffix.TabIndex = this.tabIndex++;
			this.fieldSuffix.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.InitComboFix(this.fieldSuffix);
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
				this.fieldType.ComboClosed -= new EventHandler(this.HandleTypeChanged);

				for ( int i=0 ; i<Generator.maxLevel ; i++ )
				{
					this.buttonLevel[i].Clicked -= new MessageEventHandler(this.HandleLevelClicked);
				}

				this.fieldPrefix.TextChanged -= new EventHandler(this.HandlePrefixChanged);
				this.fieldNumerator.TextChanged -= new EventHandler(this.HandleNumeratorChanged);
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
					h += 110;
				}
				else	// panneau réduit ?
				{
					h += 35;
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
				this.ParagraphWrapper.Defined.ClearIndentationLevelAttribute();
			}
			else
			{
				this.ParagraphWrapper.Defined.IndentationLevelAttribute = string.Concat(TabList.LevelMultiplier, "0 pt");
			}

			if ( type == "Bullet1" || (type == "Custom" && !this.ParagraphWrapper.Defined.IsManagedParagraphDefined) )
			{
				p = new Text.ParagraphManagers.ItemListManager.Parameters();
				p.Generator = this.document.TextContext.GeneratorList.NewGenerator();

				p.Generator.GlobalPrefix = "";
				p.Generator.GlobalSuffix = "";
				p.Generator.Add(Common.Text.Generator.CreateSequence(Common.Text.Generator.SequenceType.Constant, "", "", Common.Text.Generator.Casing.Default, "\u25CF"));
				p.Generator.Add(Common.Text.Generator.CreateSequence(Common.Text.Generator.SequenceType.Constant, "", "", Common.Text.Generator.Casing.Default, "\u25CB", true));
				p.Generator.Add(Common.Text.Generator.CreateSequence(Common.Text.Generator.SequenceType.Constant, "", "", Common.Text.Generator.Casing.Default, "-", true));
				
				p.Generator[0].ValueProperties = new Text.Property[] { new Text.Properties.FontProperty("Arial", "Regular") };
				p.Generator[1].ValueProperties = new Text.Property[] { new Text.Properties.FontProperty("Arial", "Regular") };
				p.Generator[2].ValueProperties = new Text.Property[] { new Text.Properties.FontProperty("Arial", "Regular") };

				//?Common.Text.Properties.UnitsTools.SerializeSizeUnits();
				p.TabItem = tabs.NewTab(Common.Text.TabList.GenericSharedName, 0.0, Common.Text.Properties.SizeUnits.Points, 0.0, null, TabPositionMode.LeftRelative,       TabList.PackToAttribute("LevelTable:50 pt;150 pt;250 pt;350 pt"));
				p.TabBody = tabs.NewTab(Common.Text.TabList.GenericSharedName, 0.0, Common.Text.Properties.SizeUnits.Points, 0.0, null, TabPositionMode.LeftRelativeIndent, TabList.PackToAttribute("LevelTable:100 pt;200 pt;300 pt;400 pt"));

				p.Generator.UserData = user;
				this.ParagraphWrapper.Defined.ItemListParameters = p;
			}

			if ( type == "Bullet2" )
			{
				p = new Text.ParagraphManagers.ItemListManager.Parameters();
				p.Generator = this.document.TextContext.GeneratorList.NewGenerator();

				p.Generator.GlobalPrefix = "";
				p.Generator.GlobalSuffix = "";
				p.Generator.Add(Common.Text.Generator.CreateSequence(Common.Text.Generator.SequenceType.Constant, "", "", Common.Text.Generator.Casing.Default, "\u25A0"));
				p.Generator.Add(Common.Text.Generator.CreateSequence(Common.Text.Generator.SequenceType.Constant, "", "", Common.Text.Generator.Casing.Default, "\u25A1", true));
				p.Generator.Add(Common.Text.Generator.CreateSequence(Common.Text.Generator.SequenceType.Constant, "", "", Common.Text.Generator.Casing.Default, "-", true));
				
				p.TabItem = tabs.NewTab(Common.Text.TabList.GenericSharedName, 0.0, Common.Text.Properties.SizeUnits.Points, 0.5, null, TabPositionMode.LeftRelative,       TabList.PackToAttribute("Em:1"));
				p.TabBody = tabs.NewTab(Common.Text.TabList.GenericSharedName, 0.0, Common.Text.Properties.SizeUnits.Points, 0.0, null, TabPositionMode.LeftRelativeIndent, TabList.PackToAttribute("Em:2"));
				p.Font    = new Text.Properties.FontProperty("Arial", "Regular");

				p.Generator.UserData = user;
				this.ParagraphWrapper.Defined.ItemListParameters = p;
			}

			if ( type == "Bullet3" )
			{
				p = new Text.ParagraphManagers.ItemListManager.Parameters();
				p.Generator = this.document.TextContext.GeneratorList.NewGenerator();

				p.Generator.GlobalPrefix = "";
				p.Generator.GlobalSuffix = "";
				p.Generator.Add(Common.Text.Generator.CreateSequence(Common.Text.Generator.SequenceType.Constant, "", "", Common.Text.Generator.Casing.Default, "\u25BA"));
				p.Generator.Add(Common.Text.Generator.CreateSequence(Common.Text.Generator.SequenceType.Constant, "", "", Common.Text.Generator.Casing.Default, "-", true));
				
				p.TabItem = tabs.NewTab(Common.Text.TabList.GenericSharedName, 0.0, Common.Text.Properties.SizeUnits.Points, 0.5, null, TabPositionMode.LeftRelative,       TabList.PackToAttribute("Em:1"));
				p.TabBody = tabs.NewTab(Common.Text.TabList.GenericSharedName, 0.0, Common.Text.Properties.SizeUnits.Points, 0.0, null, TabPositionMode.LeftRelativeIndent, TabList.PackToAttribute("Em:2"));
				p.Font    = new Text.Properties.FontProperty("Arial", "Regular");

				p.Generator.UserData = user;
				this.ParagraphWrapper.Defined.ItemListParameters = p;
			}

			if ( type == "Num1" )
			{
				p = new Text.ParagraphManagers.ItemListManager.Parameters();
				p.Generator = this.document.TextContext.GeneratorList.NewGenerator();

				p.Generator.GlobalPrefix = "";
				p.Generator.GlobalSuffix = "";
				p.Generator.Add(Common.Text.Generator.CreateSequence(Common.Text.Generator.SequenceType.Numeric, "", "."));
				
				p.TabItem = tabs.NewTab(Common.Text.TabList.GenericSharedName, 0.0, Common.Text.Properties.SizeUnits.Points, 1.0, null, TabPositionMode.LeftRelative,       TabList.PackToAttribute("LevelMultiplier:150 %", "Em:1.5"));
				p.TabBody = tabs.NewTab(Common.Text.TabList.GenericSharedName, 0.0, Common.Text.Properties.SizeUnits.Points, 0.0, null, TabPositionMode.LeftRelativeIndent, TabList.PackToAttribute("LevelMultiplier:150 %", "Em:2"));

				p.Generator.UserData = user;
				this.ParagraphWrapper.Defined.ItemListParameters = p;
			}

			if ( type == "Num2" )
			{
				p = new Text.ParagraphManagers.ItemListManager.Parameters();
				p.Generator = this.document.TextContext.GeneratorList.NewGenerator();

				p.Generator.GlobalPrefix = "";
				p.Generator.GlobalSuffix = ")";
				p.Generator.Add(Common.Text.Generator.CreateSequence(Common.Text.Generator.SequenceType.Alphabetic, "", "", Common.Text.Generator.Casing.Upper));
				p.Generator.Add(Common.Text.Generator.CreateSequence(Common.Text.Generator.SequenceType.Numeric,    "-", ""));
				p.Generator.Add(Common.Text.Generator.CreateSequence(Common.Text.Generator.SequenceType.Roman,      "(", "", Common.Text.Generator.Casing.Lower, null, true));
				
				p.TabItem = tabs.NewTab(Common.Text.TabList.GenericSharedName, 0.0, Common.Text.Properties.SizeUnits.Points, 0.0, null, TabPositionMode.LeftRelative,       TabList.PackToAttribute("LevelMultiplier:100 %", "Em:0.5"));
				p.TabBody = tabs.NewTab(Common.Text.TabList.GenericSharedName, 0.0, Common.Text.Properties.SizeUnits.Points, 0.0, null, TabPositionMode.LeftRelativeIndent, TabList.PackToAttribute("LevelMultiplier:150 %", "Em:2"));

				p.Generator.UserData = user;
				this.ParagraphWrapper.Defined.ItemListParameters = p;
			}

			if ( type == "Num3" )
			{
				p = new Text.ParagraphManagers.ItemListManager.Parameters();
				p.Generator = this.document.TextContext.GeneratorList.NewGenerator();

				p.Generator.GlobalPrefix = "";
				p.Generator.GlobalSuffix = "";
				p.Generator.Add(Common.Text.Generator.CreateSequence(Common.Text.Generator.SequenceType.Numeric,    "", "", Common.Text.Generator.Casing.Default, null, true));
				p.Generator.Add(Common.Text.Generator.CreateSequence(Common.Text.Generator.SequenceType.Alphabetic, "", ")", Common.Text.Generator.Casing.Lower, null, true));
				p.Generator.Add(Common.Text.Generator.CreateSequence(Common.Text.Generator.SequenceType.Roman,      "", ")", Common.Text.Generator.Casing.Lower, null, true));
				
				p.TabItem = tabs.NewTab(Common.Text.TabList.GenericSharedName, 0.0, Common.Text.Properties.SizeUnits.Points, 0.0, null, TabPositionMode.LeftRelative,       TabList.PackToAttribute("LevelMultiplier:100 %", "Em:0.5"));
				p.TabBody = tabs.NewTab(Common.Text.TabList.GenericSharedName, 0.0, Common.Text.Properties.SizeUnits.Points, 0.0, null, TabPositionMode.LeftRelativeIndent, TabList.PackToAttribute("LevelMultiplier:150 %", "Em:2"));

				p.Generator.UserData = user;
				this.ParagraphWrapper.Defined.ItemListParameters = p;
			}

			if ( type == "Custom" )
			{
				p = this.ParagraphWrapper.Defined.ItemListParameters;
				p.Generator.UserData = user;
				this.ParagraphWrapper.Defined.ItemListParameters = p;
			}
		}


		protected string GetValue(int level, string part, string name)
		{
			//	Possibilités (level.part.name) :
			//	0.[Prefix,Suffix].[Text,FontFace,FontSyle,FontSize,FontOffset]
			//	n.[Prefix,Value,Suffix].[Text,FontFace,FontSyle,FontSize,FontOffset]
			//	n.[].[SupressBefore,Tab,Indent]
			if ( this.ParagraphWrapper.Defined.IsManagedParagraphDefined )
			{
				return null;
			}

			Text.ParagraphManagers.ItemListManager.Parameters p = this.ParagraphWrapper.Defined.ItemListParameters;
			Common.Text.Property[] properties = null;

			if ( level == 0 )
			{
				if ( name == "Text" )
				{
					if ( part == "Prefix" )  return p.Generator.GlobalPrefix;
					if ( part == "Suffix" )  return p.Generator.GlobalSuffix;
				}

				if ( part == "Prefix" )  properties = p.Generator.GlobalPrefixProperties;
				if ( part == "Suffix" )  properties = p.Generator.GlobalSuffixProperties;
			}
			else if ( level-1 < p.Generator.Count )
			{
				Common.Text.Generator.Sequence sequence = p.Generator[this.level-1];

				if ( name == "Text" )
				{
					if ( part == "Prefix" )  return sequence.Prefix;
					if ( part == "Value"  )  return Generator.ConvSequenceToText(sequence);
					if ( part == "Suffix" )  return sequence.Suffix;
				}

				if ( part == "Prefix" )  properties = sequence.PrefixProperties;
				if ( part == "Value"  )  properties = sequence.ValueProperties;
				if ( part == "Suffix" )  properties = sequence.SuffixProperties;

				if ( name == "SupressBefore" )
				{
					return sequence.SuppressBefore ? "true" : "false";
				}

				if ( name == "Tab" )
				{
					return p.TabItem.ToString();
				}

				if ( name == "Indent" )
				{
					return p.TabBody.ToString();
				}
			}

			if ( properties != null )
			{
				if ( name == "FontFace" )
				{
					foreach ( Common.Text.Property property in properties )
					{
						if ( property.WellKnownType == Common.Text.Properties.WellKnownType.Font )
						{
							Common.Text.Properties.FontProperty font = property as Common.Text.Properties.FontProperty;
							return font.FaceName;
						}
					}
				}

				if ( name == "FontStyle" )
				{
					foreach ( Common.Text.Property property in properties )
					{
						if ( property.WellKnownType == Common.Text.Properties.WellKnownType.Font )
						{
							Common.Text.Properties.FontProperty font = property as Common.Text.Properties.FontProperty;
							return font.StyleName;
						}
					}
				}

				if ( name == "FontSize" )
				{
					foreach ( Common.Text.Property property in properties )
					{
						if ( property.WellKnownType == Common.Text.Properties.WellKnownType.FontSize )
						{
							Common.Text.Properties.FontSizeProperty size = property as Common.Text.Properties.FontSizeProperty;
							//?return size.Size;
						}
					}
				}

				if ( name == "FontOffset" )
				{
					foreach ( Common.Text.Property property in properties )
					{
						if ( property.WellKnownType == Common.Text.Properties.WellKnownType.FontOffset )
						{
							Common.Text.Properties.FontOffsetProperty offset = property as Common.Text.Properties.FontOffsetProperty;
							//?return offset.Offset;
						}
					}
				}
			}

			return null;
		}

		protected void SetValue(int level, string part, string name, string value)
		{
			//	Possibilités (level.part.name) :
			//	0.[Prefix,Suffix].[Text,FontFace,FontSyle,FontSize,FontOffset]
			//	n.[Prefix,Value,Suffix].[Text,FontFace,FontSyle,FontSize,FontOffset]
			//	n.[].[SupressBefore,Tab,Indent]
			if ( this.ParagraphWrapper.Defined.IsManagedParagraphDefined )
			{
				return;
			}

			Text.ParagraphManagers.ItemListManager.Parameters p = this.ParagraphWrapper.Defined.ItemListParameters;
			Common.Text.Property[] properties = null;

			if ( level == 0 )
			{
				if ( name == "Text" )
				{
					if ( part == "Prefix" )  p.Generator.GlobalPrefix = value;
					if ( part == "Suffix" )  p.Generator.GlobalSuffix = value;
				}

				if ( part == "Prefix" )  properties = p.Generator.GlobalPrefixProperties;
				if ( part == "Suffix" )  properties = p.Generator.GlobalSuffixProperties;
			}
			else
			{
				if ( level-1 >= p.Generator.Count )
				{
					//p.Generator.Add(Common.Text.Generator.CreateSequence(Common.Text.Generator.SequenceType.Constant, "", "", Common.Text.Generator.Casing.Default, "-", true));
					// TODO:
				}

				Common.Text.Generator.Sequence sequence = p.Generator[this.level-1];

				if ( name == "Text" )
				{
					if ( part == "Prefix" )  sequence.Prefix = value;
//					if ( part == "Value"  )  // TODO:
					if ( part == "Suffix" )  sequence.Suffix = value;
				}

				if ( part == "Prefix" )  properties = sequence.PrefixProperties;
				if ( part == "Value"  )  properties = sequence.ValueProperties;
				if ( part == "Suffix" )  properties = sequence.SuffixProperties;

				if ( name == "SupressBefore" )
				{
					sequence.SuppressBefore = (value == "true");
				}

				if ( name == "Tab" )
				{
					// TODO:
				}

				if ( name == "Indent" )
				{
					// TODO:
				}
			}

			if ( properties != null )
			{
				if ( name == "FontFace" )
				{
					foreach ( Common.Text.Property property in properties )
					{
						if ( property.WellKnownType == Common.Text.Properties.WellKnownType.Font )
						{
							Common.Text.Properties.FontProperty font = property as Common.Text.Properties.FontProperty;
						}
					}
					// TODO:
				}

				if ( name == "FontStyle" )
				{
					foreach ( Common.Text.Property property in properties )
					{
						if ( property.WellKnownType == Common.Text.Properties.WellKnownType.Font )
						{
							Common.Text.Properties.FontProperty font = property as Common.Text.Properties.FontProperty;
						}
					}
					// TODO:
				}

				if ( name == "FontSize" )
				{
					foreach ( Common.Text.Property property in properties )
					{
						if ( property.WellKnownType == Common.Text.Properties.WellKnownType.FontSize )
						{
							Common.Text.Properties.FontSizeProperty size = property as Common.Text.Properties.FontSizeProperty;
						}
					}
					// TODO:
				}

				if ( name == "FontOffset" )
				{
					foreach ( Common.Text.Property property in properties )
					{
						if ( property.WellKnownType == Common.Text.Properties.WellKnownType.FontOffset )
						{
							Common.Text.Properties.FontOffsetProperty offset = property as Common.Text.Properties.FontOffsetProperty;
						}
					}
					// TODO:
				}
			}
		}


		protected void InitComboType(TextFieldCombo combo)
		{
			combo.Items.Add(Res.Strings.TextPanel.Generator.Type.None);
			
			combo.Items.Add(Res.Strings.TextPanel.Generator.Type.Bullet1);
			combo.Items.Add(Res.Strings.TextPanel.Generator.Type.Bullet2);
			combo.Items.Add(Res.Strings.TextPanel.Generator.Type.Bullet3);
			
			combo.Items.Add(Res.Strings.TextPanel.Generator.Type.Num1);
			combo.Items.Add(Res.Strings.TextPanel.Generator.Type.Num2);
			combo.Items.Add(Res.Strings.TextPanel.Generator.Type.Num3);
			
			combo.Items.Add(Res.Strings.TextPanel.Generator.Type.Custom);
		}

		protected void InitComboFix(TextFieldCombo combo)
		{
			combo.Items.Add(Res.Strings.TextPanel.Generator.Text.None);

			combo.Items.Add(Res.Strings.TextPanel.Generator.Text.Fix1);
			combo.Items.Add(Res.Strings.TextPanel.Generator.Text.Fix2);
			combo.Items.Add(Res.Strings.TextPanel.Generator.Text.Fix3);
			combo.Items.Add(Res.Strings.TextPanel.Generator.Text.Fix4);
			combo.Items.Add(Res.Strings.TextPanel.Generator.Text.Fix5);
			combo.Items.Add(Res.Strings.TextPanel.Generator.Text.Fix6);
			combo.Items.Add(Res.Strings.TextPanel.Generator.Text.Fix7);
			combo.Items.Add(Res.Strings.TextPanel.Generator.Text.Fix8);
			combo.Items.Add(Res.Strings.TextPanel.Generator.Text.Fix9);
			combo.Items.Add(Res.Strings.TextPanel.Generator.Text.Fix10);
			combo.Items.Add(Res.Strings.TextPanel.Generator.Text.Fix11);
			
			combo.Items.Add(Res.Strings.TextPanel.Generator.Text.Arial1);
			combo.Items.Add(Res.Strings.TextPanel.Generator.Text.Arial2);
			combo.Items.Add(Res.Strings.TextPanel.Generator.Text.Arial3);
			combo.Items.Add(Res.Strings.TextPanel.Generator.Text.Arial4);
			combo.Items.Add(Res.Strings.TextPanel.Generator.Text.Arial5);
			combo.Items.Add(Res.Strings.TextPanel.Generator.Text.Arial6);
			combo.Items.Add(Res.Strings.TextPanel.Generator.Text.Arial7);
			combo.Items.Add(Res.Strings.TextPanel.Generator.Text.Arial8);
			combo.Items.Add(Res.Strings.TextPanel.Generator.Text.Arial9);
		}

		protected void InitComboNumerator(TextFieldCombo combo)
		{
			combo.Items.Add(Res.Strings.TextPanel.Generator.Numerator.None);
			combo.Items.Add(Res.Strings.TextPanel.Generator.Numerator.Numeric);
			combo.Items.Add(Res.Strings.TextPanel.Generator.Numerator.AlphaLower);
			combo.Items.Add(Res.Strings.TextPanel.Generator.Numerator.AlphaUpper);
			combo.Items.Add(Res.Strings.TextPanel.Generator.Numerator.RomanLower);
			combo.Items.Add(Res.Strings.TextPanel.Generator.Numerator.RomanUpper);
		}

		protected static string ConvTypeToText(string type)
		{
			switch ( type )
			{
				case "None":     return Res.Strings.TextPanel.Generator.Type.None;
				case "Bullet1":  return Res.Strings.TextPanel.Generator.Type.Bullet1;
				case "Bullet2":  return Res.Strings.TextPanel.Generator.Type.Bullet2;
				case "Bullet3":  return Res.Strings.TextPanel.Generator.Type.Bullet3;
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
			if ( text == Res.Strings.TextPanel.Generator.Type.Bullet3 )  return "Bullet3";
			if ( text == Res.Strings.TextPanel.Generator.Type.Num1    )  return "Num1";
			if ( text == Res.Strings.TextPanel.Generator.Type.Num2    )  return "Num2";
			if ( text == Res.Strings.TextPanel.Generator.Type.Num3    )  return "Num3";
			if ( text == Res.Strings.TextPanel.Generator.Type.Custom  )  return "Custom";
			return null;
		}

		protected static string ConvSequenceToText(Common.Text.Generator.Sequence sequence)
		{
			if ( sequence is Common.Text.Internal.Sequences.Numeric )
			{
				return Res.Strings.TextPanel.Generator.Numerator.Numeric;
			}

			if ( sequence is Common.Text.Internal.Sequences.Alphabetic )
			{
				if ( sequence.Casing == Common.Text.Generator.Casing.Lower )
				{
					return Res.Strings.TextPanel.Generator.Numerator.AlphaLower;
				}
				else
				{
					return Res.Strings.TextPanel.Generator.Numerator.AlphaUpper;
				}
			}

			if ( sequence is Common.Text.Internal.Sequences.Roman )
			{
				if ( sequence.Casing == Common.Text.Generator.Casing.Lower )
				{
					return Res.Strings.TextPanel.Generator.Numerator.RomanLower;
				}
				else
				{
					return Res.Strings.TextPanel.Generator.Numerator.RomanUpper;
				}
			}

			return Res.Strings.TextPanel.Generator.Numerator.None;
		}

		public static string ConvSequenceToShort(Common.Text.Generator.Sequence sequence)
		{
			if ( sequence is Common.Text.Internal.Sequences.Numeric )
			{
				return "1";
			}

			if ( sequence is Common.Text.Internal.Sequences.Alphabetic )
			{
				if ( sequence.Casing == Common.Text.Generator.Casing.Lower )
				{
					return "a";
				}
				else
				{
					return "A";
				}
			}

			if ( sequence is Common.Text.Internal.Sequences.Roman )
			{
				if ( sequence.Casing == Common.Text.Generator.Casing.Lower )
				{
					return "i";
				}
				else
				{
					return "I";
				}
			}

			return "";
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
				r.Width = 37;
				r.Bottom = r.Top-(20+8);
				this.buttonLevel[0].Bounds = r;
				this.buttonLevel[0].Visibility = true;
				r.Offset(r.Width, 0);
				r.Width = 20;
				for ( int i=1 ; i<Generator.maxLevel ; i++ )
				{
					this.buttonLevel[i].Bounds = r;
					this.buttonLevel[i].Visibility = true;
					r.Offset(r.Width, 0);
				}

				r.Offset(0, -30);
				r.Bottom = r.Top-20;
				r.Left = rect.Left;
				r.Width = 56;
				this.fieldPrefix.Bounds = r;
				this.fieldPrefix.Visibility = true;
				r.Offset(r.Width+5, 0);
				this.fieldNumerator.Bounds = r;
				this.fieldNumerator.Visibility = true;
				r.Offset(r.Width+5, 0);
				this.fieldSuffix.Bounds = r;
				this.fieldSuffix.Visibility = true;
			}
			else
			{
				for ( int i=0 ; i<Generator.maxLevel ; i++ )
				{
					this.buttonLevel[i].Visibility = false;
				}

				this.fieldPrefix.Visibility = false;
				this.fieldNumerator.Visibility = false;
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

			bool isGenerator = this.ParagraphWrapper.Defined.IsManagedParagraphDefined;
			string type = "None";
			string prefix = "";
			string generator = "";
			string suffix = "";

			if ( isGenerator )
			{
				Text.ParagraphManagers.ItemListManager.Parameters p = this.ParagraphWrapper.Defined.ItemListParameters;
				string[] user = (p == null) ? null : p.Generator.UserData;
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

				if ( p != null )
				{
					if ( this.level == 0 )
					{
						prefix = p.Generator.GlobalPrefix;
						suffix = p.Generator.GlobalSuffix;
					}
					else if ( this.level-1 < p.Generator.Count )
					{
						Common.Text.Generator.Sequence sequence = p.Generator[this.level-1];
						prefix = sequence.Prefix;
						generator = Generator.ConvSequenceToText(sequence);
						suffix = sequence.Suffix;
					}
				}
			}

			this.Type = type;

			this.ignoreChanged = true;

			this.fieldType.Text = Generator.ConvTypeToText(type);
			this.fieldPrefix.Text = prefix;
			this.fieldNumerator.Text = generator;
			this.fieldSuffix.Text = suffix;

			this.ProposalTextFieldCombo(this.fieldType, !isGenerator);
			this.ProposalTextFieldCombo(this.fieldPrefix, !isGenerator);
			this.ProposalTextFieldCombo(this.fieldNumerator, !isGenerator);
			this.ProposalTextFieldCombo(this.fieldSuffix, !isGenerator);

			this.ignoreChanged = false;
		}


		protected void UpdateWidgets()
		{
			bool custom = (this.type == "Custom");

			for ( int i=0 ; i<Generator.maxLevel ; i++ )
			{
				this.buttonLevel[i].Enable = custom;
			}

			this.fieldPrefix.Enable = custom;
			this.fieldNumerator.Enable = (custom && this.level != 0);
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
					this.UpdateAfterChanging();
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

		private void HandleLevelClicked(object sender, MessageEventArgs e)
		{
			Button button = sender as Button;

			for ( int i=0 ; i<Generator.maxLevel ; i++ )
			{
				this.buttonLevel[i].ActiveState = (this.buttonLevel[i] == button) ? ActiveState.Yes : ActiveState.No;
			}
			
			int level = int.Parse(button.Name);
			this.Level = level;
		}

		private void HandlePrefixChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.ParagraphWrapper.IsAttached )  return;

		}

		private void HandleNumeratorChanged(object sender)
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
			this.ParagraphWrapper.Defined.ClearIndentationLevelAttribute();
			this.ParagraphWrapper.DefineOperationName("ParagraphGeneratorClear", Res.Strings.TextPanel.Clear);
			this.ParagraphWrapper.ResumeSynchronizations();
			this.document.IsDirtySerialize = true;
		}


		protected static readonly int		maxLevel = 8;

		protected Widgets.IconMarkButton[]	buttonLevel;
		protected TextFieldCombo			fieldType;
		protected TextFieldCombo			fieldPrefix;
		protected TextFieldCombo			fieldNumerator;
		protected TextFieldCombo			fieldSuffix;
		protected IconButton				buttonClear;

		protected string					type = null;
		protected int						level = -1;
	}
}
