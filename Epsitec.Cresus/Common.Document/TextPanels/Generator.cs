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
		//	Possibilités (level.part1.part2) :
		//	0.[Prefix,Suffix].[Text,FontFace,FontSyle,FontSize,FontOffset]
		//	0.[Generic].[Disposition]
		//	n.[Prefix,Value,Suffix].[Text,FontFace,FontSyle,FontSize,FontOffset]
		//	n.[Generic].[SupressBefore,Tab,Indent]

		protected enum Part1
		{
			Generic,
			Prefix,
			Value,
			Suffix,
		}

		protected enum Part2
		{
			Text,
			FontFace,
			FontStyle,
			FontSize,
			FontOffset,
			SupressBefore,
			Tab,
			Indent,
			Disposition,
		}


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

				this.SetValue(p, 0, Part1.Prefix,  Part2.Text,          "");
				this.SetValue(p, 0, Part1.Suffix,  Part2.Text,          "");
				this.SetValue(p, 0, Part1.Generic, Part2.Disposition,   "Center");

				this.SetValue(p, 1, Part1.Generic, Part2.SupressBefore, "false");
				this.SetValue(p, 1, Part1.Prefix,  Part2.Text,          "\u25CF");  // puce ronde pleine
				this.SetValue(p, 1, Part1.Prefix,  Part2.FontFace,      "Arial");
				this.SetValue(p, 1, Part1.Value,   Part2.Text,          "");
				this.SetValue(p, 1, Part1.Suffix,  Part2.Text,          "");

				this.SetValue(p, 2, Part1.Generic, Part2.SupressBefore, "true");
				this.SetValue(p, 2, Part1.Prefix,  Part2.Text,          "\u25CB");  // puce ronde vide
				this.SetValue(p, 2, Part1.Prefix,  Part2.FontFace,      "Arial");
				this.SetValue(p, 2, Part1.Value,   Part2.Text,          "");
				this.SetValue(p, 2, Part1.Suffix,  Part2.Text,          "");

				this.SetValue(p, 3, Part1.Generic, Part2.SupressBefore, "true");
				this.SetValue(p, 3, Part1.Prefix,  Part2.Text,          "-");
				this.SetValue(p, 3, Part1.Prefix,  Part2.FontFace,      "Arial");
				this.SetValue(p, 3, Part1.Value,   Part2.Text,          "");
				this.SetValue(p, 3, Part1.Suffix,  Part2.Text,          "");

				p.Generator.UserData = user;
				this.ParagraphWrapper.Defined.ItemListParameters = p;
			}

			if ( type == "Bullet2" )
			{
				p = new Text.ParagraphManagers.ItemListManager.Parameters();
				p.Generator = this.document.TextContext.GeneratorList.NewGenerator();

				this.SetValue(p, 0, Part1.Prefix,  Part2.Text,          "");
				this.SetValue(p, 0, Part1.Suffix,  Part2.Text,          "");
				this.SetValue(p, 0, Part1.Generic, Part2.Disposition,   "Center");

				this.SetValue(p, 1, Part1.Generic, Part2.SupressBefore, "false");
				this.SetValue(p, 1, Part1.Prefix,  Part2.Text,          "\u25A0");  // puce carrée pleine
				this.SetValue(p, 1, Part1.Prefix,  Part2.FontFace,      "Arial");
				this.SetValue(p, 1, Part1.Value,   Part2.Text,          "");
				this.SetValue(p, 1, Part1.Suffix,  Part2.Text,          "");

				this.SetValue(p, 2, Part1.Generic, Part2.SupressBefore, "true");
				this.SetValue(p, 2, Part1.Prefix,  Part2.Text,          "\u25A1");  // puce carrée vide
				this.SetValue(p, 2, Part1.Prefix,  Part2.FontFace,      "Arial");
				this.SetValue(p, 2, Part1.Value,   Part2.Text,          "");
				this.SetValue(p, 2, Part1.Suffix,  Part2.Text,          "");

				this.SetValue(p, 3, Part1.Generic, Part2.SupressBefore, "true");
				this.SetValue(p, 3, Part1.Prefix,  Part2.Text,          "-");
				this.SetValue(p, 3, Part1.Prefix,  Part2.FontFace,      "Arial");
				this.SetValue(p, 3, Part1.Value,   Part2.Text,          "");
				this.SetValue(p, 3, Part1.Suffix,  Part2.Text,          "");

				p.Generator.UserData = user;
				this.ParagraphWrapper.Defined.ItemListParameters = p;
			}

			if ( type == "Bullet3" )
			{
				p = new Text.ParagraphManagers.ItemListManager.Parameters();
				p.Generator = this.document.TextContext.GeneratorList.NewGenerator();

				this.SetValue(p, 0, Part1.Prefix,  Part2.Text,          "");
				this.SetValue(p, 0, Part1.Suffix,  Part2.Text,          "");
				this.SetValue(p, 0, Part1.Generic, Part2.Disposition,   "Center");

				this.SetValue(p, 1, Part1.Generic, Part2.SupressBefore, "false");
				this.SetValue(p, 1, Part1.Prefix,  Part2.Text,          "\u25BA");  // triangle >
				this.SetValue(p, 1, Part1.Prefix,  Part2.FontFace,      "Arial");
				this.SetValue(p, 1, Part1.Value,   Part2.Text,          "");
				this.SetValue(p, 1, Part1.Suffix,  Part2.Text,          "");

				this.SetValue(p, 2, Part1.Generic, Part2.SupressBefore, "true");
				this.SetValue(p, 2, Part1.Prefix,  Part2.Text,          "-");
				this.SetValue(p, 2, Part1.Prefix,  Part2.FontFace,      "Arial");
				this.SetValue(p, 2, Part1.Value,   Part2.Text,          "");
				this.SetValue(p, 2, Part1.Suffix,  Part2.Text,          "");

				p.Generator.UserData = user;
				this.ParagraphWrapper.Defined.ItemListParameters = p;
			}

			if ( type == "Num1" )
			{
				p = new Text.ParagraphManagers.ItemListManager.Parameters();
				p.Generator = this.document.TextContext.GeneratorList.NewGenerator();

				this.SetValue(p, 0, Part1.Prefix,  Part2.Text,          "");
				this.SetValue(p, 0, Part1.Suffix,  Part2.Text,          "");
				this.SetValue(p, 0, Part1.Generic, Part2.Disposition,   "Right");

				this.SetValue(p, 1, Part1.Generic, Part2.SupressBefore, "false");
				this.SetValue(p, 1, Part1.Prefix,  Part2.Text,          "");
				this.SetValue(p, 1, Part1.Value,   Part2.Text,          Res.Strings.TextPanel.Generator.Numerator.Numeric);
				this.SetValue(p, 1, Part1.Suffix,  Part2.Text,          ".");

				p.Generator.UserData = user;
				this.ParagraphWrapper.Defined.ItemListParameters = p;
			}

			if ( type == "Num2" )
			{
				p = new Text.ParagraphManagers.ItemListManager.Parameters();
				p.Generator = this.document.TextContext.GeneratorList.NewGenerator();

				this.SetValue(p, 0, Part1.Prefix,  Part2.Text,          "");
				this.SetValue(p, 0, Part1.Suffix,  Part2.Text,          ")");
				this.SetValue(p, 0, Part1.Generic, Part2.Disposition,   "Right");

				this.SetValue(p, 1, Part1.Generic, Part2.SupressBefore, "false");
				this.SetValue(p, 1, Part1.Prefix,  Part2.Text,          "");
				this.SetValue(p, 1, Part1.Value,   Part2.Text,          Res.Strings.TextPanel.Generator.Numerator.AlphaUpper);
				this.SetValue(p, 1, Part1.Suffix,  Part2.Text,          "");
				this.SetValue(p, 1, Part1.Generic, Part2.Tab,           "15");
				this.SetValue(p, 1, Part1.Generic, Part2.Indent,        "20");

				this.SetValue(p, 2, Part1.Generic, Part2.SupressBefore, "false");
				this.SetValue(p, 2, Part1.Prefix,  Part2.Text,          "-");
				this.SetValue(p, 2, Part1.Value,   Part2.Text,          Res.Strings.TextPanel.Generator.Numerator.Numeric);
				this.SetValue(p, 2, Part1.Suffix,  Part2.Text,          "");
				this.SetValue(p, 2, Part1.Generic, Part2.Tab,           "15");
				this.SetValue(p, 2, Part1.Generic, Part2.Indent,        "20");

				this.SetValue(p, 3, Part1.Generic, Part2.SupressBefore, "true");
				this.SetValue(p, 3, Part1.Prefix,  Part2.Text,          "(");
				this.SetValue(p, 3, Part1.Value,   Part2.Text,          Res.Strings.TextPanel.Generator.Numerator.RomanLower);
				this.SetValue(p, 3, Part1.Suffix,  Part2.Text,          "");
				this.SetValue(p, 3, Part1.Generic, Part2.Tab,           "20");
				this.SetValue(p, 3, Part1.Generic, Part2.Indent,        "25");

				p.Generator.UserData = user;
				this.ParagraphWrapper.Defined.ItemListParameters = p;
			}

			if ( type == "Num3" )
			{
				p = new Text.ParagraphManagers.ItemListManager.Parameters();
				p.Generator = this.document.TextContext.GeneratorList.NewGenerator();

				this.SetValue(p, 0, Part1.Prefix,  Part2.Text,          "");
				this.SetValue(p, 0, Part1.Suffix,  Part2.Text,          "");
				this.SetValue(p, 0, Part1.Generic, Part2.Disposition,   "Left");

				this.SetValue(p, 1, Part1.Generic, Part2.SupressBefore, "false");
				this.SetValue(p, 1, Part1.Prefix,  Part2.Text,          "");
				this.SetValue(p, 1, Part1.Value,   Part2.Text,          Res.Strings.TextPanel.Generator.Numerator.Numeric);
				this.SetValue(p, 1, Part1.Suffix,  Part2.Text,          "");

				this.SetValue(p, 2, Part1.Generic, Part2.SupressBefore, "true");
				this.SetValue(p, 2, Part1.Prefix,  Part2.Text,          "");
				this.SetValue(p, 2, Part1.Value,   Part2.Text,          Res.Strings.TextPanel.Generator.Numerator.AlphaLower);
				this.SetValue(p, 2, Part1.Suffix,  Part2.Text,          ")");

				this.SetValue(p, 3, Part1.Generic, Part2.SupressBefore, "true");
				this.SetValue(p, 3, Part1.Prefix,  Part2.Text,          "");
				this.SetValue(p, 3, Part1.Value,   Part2.Text,          Res.Strings.TextPanel.Generator.Numerator.RomanLower);
				this.SetValue(p, 3, Part1.Suffix,  Part2.Text,          ")");

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


		protected string GetValue(int level, Part1 part1, Part2 part2)
		{
			System.Diagnostics.Debug.Assert(level >= 0 && level <= 10);
			if ( this.ParagraphWrapper.Defined.IsManagedParagraphDefined )
			{
				return null;
			}

			Text.ParagraphManagers.ItemListManager.Parameters p = this.ParagraphWrapper.Defined.ItemListParameters;
			Text.TabList tabs = this.document.TextContext.TabList;
			Common.Text.Property[] properties = null;

			if ( level == 0 )
			{
				if ( part2 == Part2.Text )
				{
					if ( part1 == Part1.Prefix )  return p.Generator.GlobalPrefix;
					if ( part1 == Part1.Suffix )  return p.Generator.GlobalSuffix;
				}

				if ( part1 == Part1.Prefix )  properties = p.Generator.GlobalPrefixProperties;
				if ( part1 == Part1.Suffix )  properties = p.Generator.GlobalSuffixProperties;

				if ( part2 == Part2.Disposition )
				{
					if ( p.TabItem == null )  return null;
					double dispo = tabs.GetTabDisposition(p.TabItem);
					if ( dispo == 0.5 )  return "Center";
					if ( dispo == 1.0 )  return "Right";
					return "Left";
				}
			}
			else if ( level-1 < p.Generator.Count )
			{
				Common.Text.Generator.Sequence sequence = p.Generator[level-1];

				if ( part2 == Part2.Text )
				{
					if ( part1 == Part1.Prefix )  return sequence.Prefix;
					if ( part1 == Part1.Suffix )  return sequence.Suffix;
					if ( part1 == Part1.Value  )  return Generator.ConvSequenceToText(sequence);
				}

				if ( part1 == Part1.Prefix )  properties = sequence.PrefixProperties;
				if ( part1 == Part1.Suffix )  properties = sequence.SuffixProperties;
				if ( part1 == Part1.Value  )  properties = sequence.ValueProperties;

				if ( part2 == Part2.SupressBefore )
				{
					return sequence.SuppressBefore ? "true" : "false";
				}

				if ( part2 == Part2.Tab || part2 == Part2.Indent )
				{
					string ta = this.document.TextContext.TabList.GetTabAttribute((part2 == Part2.Tab) ? p.TabItem : p.TabBody);
					if ( ta == null )  return null;
					string[] tas = TabList.UnpackFromAttribute(ta);
					double offset = TabList.GetLevelOffset(0, level-1, tas[0]);
					return this.document.Modifier.RealToString(offset);
				}
			}

			if ( properties != null )
			{
				if ( part2 == Part2.FontFace )
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

				if ( part2 == Part2.FontStyle )
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

				if ( part2 == Part2.FontSize )
				{
					foreach ( Common.Text.Property property in properties )
					{
						if ( property.WellKnownType == Common.Text.Properties.WellKnownType.FontSize )
						{
							Common.Text.Properties.FontSizeProperty size = property as Common.Text.Properties.FontSizeProperty;
							return this.document.Modifier.RealToString(size.Size);
						}
					}
				}

				if ( part2 == Part2.FontOffset )
				{
					foreach ( Common.Text.Property property in properties )
					{
						if ( property.WellKnownType == Common.Text.Properties.WellKnownType.FontOffset )
						{
							Common.Text.Properties.FontOffsetProperty offset = property as Common.Text.Properties.FontOffsetProperty;
							return this.document.Modifier.RealToString(offset.Offset);
						}
					}
				}
			}

			return null;
		}

		protected void SetValue(int level, Part1 part1, Part2 part2, string value)
		{
			if ( this.ParagraphWrapper.Defined.IsManagedParagraphDefined )  return;
			Text.ParagraphManagers.ItemListManager.Parameters p = this.ParagraphWrapper.Defined.ItemListParameters;
			this.SetValue(p, level, part1, part2, value);
		}

		protected void SetValue(Text.ParagraphManagers.ItemListManager.Parameters p, int level, Part1 part1, Part2 part2, string value)
		{
			System.Diagnostics.Debug.Assert(level >= 0 && level <= 10);

			Text.TabList tabs = this.document.TextContext.TabList;
			Common.Text.Property[] properties = null;

			if ( p.TabItem == null )
			{
				//	Si aucun tabulateur n'est défini, pose 10x2 taquets par défaut.
				double[] offsetsItem = new double[10];
				double[] offsetsBody = new double[10];
				double inc = System.Globalization.RegionInfo.CurrentRegion.IsMetric ? 100 : 127;  // 10mm ou 0.5in
				double posItem = inc/2;
				double posBody = inc;
				for ( int i=0 ; i<10 ; i++ )
				{
					offsetsItem[i] = posItem;
					offsetsBody[i] = posBody;
					posItem += inc;
					posBody += inc;
				}

				string attributeItem = TabList.CreateLevelTable(offsetsItem);
				string attributeBody = TabList.CreateLevelTable(offsetsBody);

				p.TabItem = tabs.NewTab(Common.Text.TabList.GenericSharedName, 0.0, Common.Text.Properties.SizeUnits.Points, 0.0, null, TabPositionMode.LeftRelative,       attributeItem);
				p.TabBody = tabs.NewTab(Common.Text.TabList.GenericSharedName, 0.0, Common.Text.Properties.SizeUnits.Points, 0.0, null, TabPositionMode.LeftRelativeIndent, attributeBody);
			}

			if ( level == 0 )
			{
				if ( part2 == Part2.Text )
				{
					if ( part1 == Part1.Prefix )  p.Generator.GlobalPrefix = value;
					if ( part1 == Part1.Suffix )  p.Generator.GlobalSuffix = value;
				}

				if ( part1 == Part1.Prefix )  properties = p.Generator.GlobalPrefixProperties;
				if ( part1 == Part1.Suffix )  properties = p.Generator.GlobalSuffixProperties;

				if ( part2 == Part2.Disposition )
				{
					if ( p.TabItem == null )  return;
					string ta = this.document.TextContext.TabList.GetTabAttribute(p.TabItem);

					string[] tas = TabList.UnpackFromAttribute(ta);
					double[] offsets = TabList.ParseLevelTable(tas[0]);
					string attribute = TabList.CreateLevelTable(offsets);

					double dispo = 0.0;
					if ( value == "Center" )  dispo = 0.5;
					if ( value == "Right"  )  dispo = 1.0;

					p.TabItem = tabs.NewTab(Common.Text.TabList.GenericSharedName, 0.0, Common.Text.Properties.SizeUnits.Points, dispo, null, TabPositionMode.LeftRelative,       attribute);
				}
			}
			else
			{
				while ( level-1 >= p.Generator.Count )
				{
					p.Generator.Add(Common.Text.Generator.CreateSequence(Common.Text.Generator.SequenceType.Empty));
				}

				Common.Text.Generator.Sequence sequence = p.Generator[level-1];

				if ( part2 == Part2.Text )
				{
					if ( part1 == Part1.Prefix )  sequence.Prefix = value;
					if ( part1 == Part1.Suffix )  sequence.Suffix = value;

					if ( part1 == Part1.Value  )
					{
						Common.Text.Generator.SequenceType type;
						Common.Text.Generator.Casing casing;
						Generator.ConvTextToSequence(value, out type, out casing);
						sequence.Casing = casing;
						p.Generator.Modify(level-1, type);
					}
				}

				if ( part1 == Part1.Prefix )  properties = sequence.PrefixProperties;
				if ( part1 == Part1.Suffix )  properties = sequence.SuffixProperties;
				if ( part1 == Part1.Value  )  properties = sequence.ValueProperties;

				if ( part2 == Part2.SupressBefore )
				{
					sequence.SuppressBefore = (value == "true");
				}

				if ( part2 == Part2.Tab || part2 == Part2.Indent )
				{
					string ta = this.document.TextContext.TabList.GetTabAttribute((part2 == Part2.Tab) ? p.TabItem : p.TabBody);

					string[] tas = TabList.UnpackFromAttribute(ta);
					double[] offsets = TabList.ParseLevelTable(tas[0]);

					offsets[level-1] = this.ConvTextToDistance(value);
					string attribute = TabList.CreateLevelTable(offsets);

					double dispo = tabs.GetTabDisposition(p.TabItem);

					if ( part2 == Part2.Tab    )  p.TabItem = tabs.NewTab(Common.Text.TabList.GenericSharedName, 0.0, Common.Text.Properties.SizeUnits.Points, dispo, null, TabPositionMode.LeftRelative,       attribute);
					if ( part2 == Part2.Indent )  p.TabBody = tabs.NewTab(Common.Text.TabList.GenericSharedName, 0.0, Common.Text.Properties.SizeUnits.Points, 0.0,   null, TabPositionMode.LeftRelativeIndent, attribute);
				}
			}

			if ( properties != null )
			{
				if ( part2 == Part2.FontFace )
				{
					Common.Text.Properties.FontProperty current = Generator.PropertyGet(properties, Common.Text.Properties.WellKnownType.Font) as Common.Text.Properties.FontProperty;
					Common.Text.Properties.FontProperty n = new Text.Properties.FontProperty(value, Misc.DefaultFontStyle(value));
					if ( current == null )
					{
						properties = Generator.PropertyAdd(properties, n);
					}
					else
					{
						Generator.PropertySet(properties, n);
					}
				}

				if ( part2 == Part2.FontStyle )
				{
					Common.Text.Properties.FontProperty current = Generator.PropertyGet(properties, Common.Text.Properties.WellKnownType.Font) as Common.Text.Properties.FontProperty;
					if ( current == null )
					{
						Common.Text.Properties.FontProperty n = new Text.Properties.FontProperty("Arial", value);
						properties = Generator.PropertyAdd(properties, n);
					}
					else
					{
						Common.Text.Properties.FontProperty n = new Text.Properties.FontProperty(current.FaceName, value);
						Generator.PropertySet(properties, n);
					}
				}

				if ( part2 == Part2.FontSize )
				{
					Common.Text.Properties.FontSizeProperty current = Generator.PropertyGet(properties, Common.Text.Properties.WellKnownType.Font) as Common.Text.Properties.FontSizeProperty;
					Common.Text.Properties.FontSizeProperty n = new Text.Properties.FontSizeProperty(this.ConvTextToDistance(value), Common.Text.Properties.SizeUnits.Points);
					if ( current == null )
					{
						properties = Generator.PropertyAdd(properties, n);
					}
					else
					{
						Generator.PropertySet(properties, n);
					}
				}

				if ( part2 == Part2.FontOffset )
				{
					Common.Text.Properties.FontOffsetProperty current = Generator.PropertyGet(properties, Common.Text.Properties.WellKnownType.Font) as Common.Text.Properties.FontOffsetProperty;
					Common.Text.Properties.FontOffsetProperty n = new Text.Properties.FontOffsetProperty(this.ConvTextToDistance(value), Common.Text.Properties.SizeUnits.Points);
					if ( current == null )
					{
						properties = Generator.PropertyAdd(properties, n);
					}
					else
					{
						Generator.PropertySet(properties, n);
					}
				}

				if ( level == 0 )
				{
					if ( part1 == Part1.Prefix )  p.Generator.GlobalPrefixProperties = properties;
					if ( part1 == Part1.Suffix )  p.Generator.GlobalSuffixProperties = properties;
				}
				else
				{
					Common.Text.Generator.Sequence sequence = p.Generator[level-1];

					if ( part1 == Part1.Prefix )  sequence.PrefixProperties = properties;
					if ( part1 == Part1.Suffix )  sequence.SuffixProperties = properties;
					if ( part1 == Part1.Value  )  sequence.ValueProperties  = properties;
				}
			}
		}

		#region Properties array manager
		protected static Common.Text.Property PropertyGet(Common.Text.Property[] properties, Common.Text.Properties.WellKnownType type)
		{
			foreach ( Common.Text.Property property in properties )
			{
				if ( property.WellKnownType == type )  return property;
			}
			return null;
		}

		protected static void PropertySet(Common.Text.Property[] properties, Common.Text.Property n)
		{
			for ( int i=0 ; i<properties.Length ; i++ )
			{
				Common.Text.Property property = properties[i] as Common.Text.Property;
				if ( property.WellKnownType == n.WellKnownType )
				{
					properties[i] = n;
				}
			}
		}

		protected static Common.Text.Property[] PropertyAdd(Common.Text.Property[] properties, Common.Text.Property n)
		{
			Common.Text.Property[] list = new Common.Text.Property[properties.Length+1];
			for ( int i=0 ; i<properties.Length ; i++ )
			{
				list[i] = properties[i];
			}
			list[properties.Length] = n;
			return list;
		}
		#endregion


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

		protected double ConvTextToDistance(string text)
		{
			try
			{
				return double.Parse(text) * this.document.Modifier.RealScale;
			}
			catch
			{
				return 0;
			}
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

		protected static void ConvTextToSequence(string text, out Common.Text.Generator.SequenceType type, out Common.Text.Generator.Casing casing)
		{
			type = Common.Text.Generator.SequenceType.None;
			casing = Common.Text.Generator.Casing.Default;

			if ( text == Res.Strings.TextPanel.Generator.Numerator.None || text == "" )
			{
				type = Common.Text.Generator.SequenceType.Empty;
			}

			if ( text == Res.Strings.TextPanel.Generator.Numerator.Numeric )
			{
				type = Common.Text.Generator.SequenceType.Numeric;
			}

			if ( text == Res.Strings.TextPanel.Generator.Numerator.AlphaLower )
			{
				type = Common.Text.Generator.SequenceType.Alphabetic;
				casing = Common.Text.Generator.Casing.Lower;
			}

			if ( text == Res.Strings.TextPanel.Generator.Numerator.AlphaUpper )
			{
				type = Common.Text.Generator.SequenceType.Alphabetic;
				casing = Common.Text.Generator.Casing.Upper;
			}

			if ( text == Res.Strings.TextPanel.Generator.Numerator.RomanLower )
			{
				type = Common.Text.Generator.SequenceType.Roman;
				casing = Common.Text.Generator.Casing.Lower;
			}

			if ( text == Res.Strings.TextPanel.Generator.Numerator.RomanUpper )
			{
				type = Common.Text.Generator.SequenceType.Roman;
				casing = Common.Text.Generator.Casing.Upper;
			}
		}

		protected static string ConvSequenceToText(Common.Text.Generator.Sequence sequence)
		{
			if ( sequence.WellKnownType == Common.Text.Generator.SequenceType.Empty )
			{
				return Res.Strings.TextPanel.Generator.Numerator.None;
			}

			if ( sequence.WellKnownType == Common.Text.Generator.SequenceType.Numeric )
			{
				return Res.Strings.TextPanel.Generator.Numerator.Numeric;
			}

			if ( sequence.WellKnownType == Common.Text.Generator.SequenceType.Alphabetic )
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

			if ( sequence.WellKnownType == Common.Text.Generator.SequenceType.Roman )
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
			if ( sequence.WellKnownType == Common.Text.Generator.SequenceType.Numeric )
			{
				return "1";
			}

			if ( sequence.WellKnownType == Common.Text.Generator.SequenceType.Alphabetic )
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

			if ( sequence.WellKnownType == Common.Text.Generator.SequenceType.Roman )
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
