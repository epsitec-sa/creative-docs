using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Text;

namespace Epsitec.Common.Document.TextPanels
{
	/// <summary>
	/// La classe Generator permet de choisir les puces et les num�rotations.
	/// </summary>
	[SuppressBundleSupport]
	public class Generator : Abstract
	{
		//	Possibilit�s (level.part1.part2) :
		//	0.[Prefix,Suffix].[Text,FontFace,FontSyle,FontSize,FontOffset]
		//	0.[Generic].[Disposition]
		//	n.[Prefix,Value,Suffix].[Text,FontFace,FontSyle,FontSize,FontOffset]
		//	n.[Generic].[SuppressBefore,Tab,Indent]

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
			FontColor,
			FontOffset,
			SuppressBefore,
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
			Generator.InitComboType(this.fieldType);

			this.table = new CellTable(this);
			this.table.StyleH |= CellArrayStyle.Header;
			this.table.StyleH |= CellArrayStyle.Separator;
			this.table.StyleV |= CellArrayStyle.ScrollNorm;
			this.table.StyleV |= CellArrayStyle.Separator;
			this.table.StyleV |= CellArrayStyle.SelectCell;
			this.table.FinalSelectionChanged += new EventHandler(this.HandleTableSelectionChanged);

			this.buttonAdd = this.CreateIconButton(Misc.Icon("ShaperHandleAdd"), "Ajouter une ligne � la fin", new MessageEventHandler(this.HandleAddClicked));
			this.buttonSub = this.CreateIconButton(Misc.Icon("ShaperHandleSub"), "Supprimer la derni�re ligne", new MessageEventHandler(this.HandleSubClicked));

			this.buttonLeft   = this.CreateIconButton(Misc.Icon("JustifHLeft"),   Res.Strings.Action.ParagraphAlignLeft,   new MessageEventHandler(this.HandleJustifClicked));
			this.buttonCenter = this.CreateIconButton(Misc.Icon("JustifHCenter"), Res.Strings.Action.ParagraphAlignCenter, new MessageEventHandler(this.HandleJustifClicked));
			this.buttonRight  = this.CreateIconButton(Misc.Icon("JustifHRight"),  Res.Strings.Action.ParagraphAlignRight,  new MessageEventHandler(this.HandleJustifClicked));

			this.buttonSuppressBefore = this.CreateIconButton(Misc.Icon("SuppressBefore"), "Cette ligne remplace les pr�c�dentes", new MessageEventHandler(this.HandleSuppressBeforeClicked));

			this.labelText = new StaticText(this);
			this.labelText.Alignment = ContentAlignment.MiddleRight;

			this.fieldText = new TextFieldCombo(this);
			this.fieldText.AutoFocus = false;
			this.fieldText.TextChanged += new EventHandler(this.HandleTextChanged);
			this.fieldText.TabIndex = this.tabIndex++;
			this.fieldText.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.fieldTab        = this.CreateTextFieldLabel("Position de la puce/num�ro", "Puces", "", 0.0, 0.1, 0.0, 1.0, Widgets.TextFieldLabel.Type.TextFieldReal, new EventHandler(this.HandleTabChanged));
			this.fieldIndent     = this.CreateTextFieldLabel("Position du texte", "Texte", "", 0.0, 0.1, 0.0, 1.0, Widgets.TextFieldLabel.Type.TextFieldReal, new EventHandler(this.HandleIndentChanged));
			this.fieldFontSize   = this.CreateTextFieldLabel("Taille de la police", "Taille", "", 0.0, 0.1, 0.0, 1.0, Widgets.TextFieldLabel.Type.TextFieldReal, new EventHandler(this.HandleFontSizeChanged));
			this.fieldFontOffset = this.CreateTextFieldLabel("Offset vertical", "Offset", "", -0.1, 0.1, 0.0, 1.0, Widgets.TextFieldLabel.Type.TextFieldReal, new EventHandler(this.HandleFontOffsetChanged));

			this.colorText = this.CreateColorSample("Couleur du texte", new MessageEventHandler(this.HandleSampleColorClicked), new EventHandler(this.HandleSampleColorChanged));

			this.buttonClear = this.CreateClearButton(new MessageEventHandler(this.HandleClearClicked));

			this.ParagraphWrapper.Active.Changed  += new EventHandler(this.HandleWrapperChanged);
			this.ParagraphWrapper.Defined.Changed += new EventHandler(this.HandleWrapperChanged);

			this.Type = "None";
			this.UpdateTable();
			this.UpdateFields();

			this.isNormalAndExtended = true;
			this.UpdateAfterChanging();
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.fieldType.ComboClosed -= new EventHandler(this.HandleTypeChanged);
				this.table.FinalSelectionChanged -= new EventHandler(this.HandleTableSelectionChanged);
				this.fieldText.TextChanged -= new EventHandler(this.HandleTextChanged);

				this.ParagraphWrapper.Active.Changed  -= new EventHandler(this.HandleWrapperChanged);
				this.ParagraphWrapper.Defined.Changed -= new EventHandler(this.HandleWrapperChanged);
			}
			
			base.Dispose(disposing);
		}

		
		public override void UpdateAfterAttach()
		{
			//	Mise � jour apr�s avoir attach� le wrappers.
			this.UpdateButtonClear();
		}


		public override double DefaultHeight
		{
			//	Retourne la hauteur standard.
			get
			{
				double h = this.LabelHeight;

				if ( this.isExtendedSize )  // panneau �tendu ?
				{
					h += 86 + (23+17*4);
				}
				else	// panneau r�duit ?
				{
					h += 35;
				}

				return h;
			}
		}


		protected void HandleWrapperChanged(object sender)
		{
			//	Le wrapper associ� a chang�.
			this.UpdateAfterChanging();
		}


		protected void CreateGenerator(string type)
		{
			//	Cr�e un nouveau g�n�rateur pr�d�fini.
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

				this.SetValue(p, 0, Part1.Prefix,  Part2.Text,           "");
				this.SetValue(p, 0, Part1.Suffix,  Part2.Text,           "");
				this.SetValue(p, 0, Part1.Generic, Part2.Disposition,    "Center");

				this.SetValue(p, 1, Part1.Generic, Part2.SuppressBefore, "false");
				this.SetValue(p, 1, Part1.Prefix,  Part2.Text,           "\u25CF");  // puce ronde pleine
				this.SetValue(p, 1, Part1.Prefix,  Part2.FontFace,       "Arial");
				this.SetValue(p, 1, Part1.Value,   Part2.Text,           "");
				this.SetValue(p, 1, Part1.Suffix,  Part2.Text,           "");

				this.SetValue(p, 2, Part1.Generic, Part2.SuppressBefore, "true");
				this.SetValue(p, 2, Part1.Prefix,  Part2.Text,           "\u25CB");  // puce ronde vide
				this.SetValue(p, 2, Part1.Prefix,  Part2.FontFace,       "Arial");
				this.SetValue(p, 2, Part1.Value,   Part2.Text,           "");
				this.SetValue(p, 2, Part1.Suffix,  Part2.Text,           "");

				this.SetValue(p, 3, Part1.Generic, Part2.SuppressBefore, "true");
				this.SetValue(p, 3, Part1.Prefix,  Part2.Text,           "-");
				this.SetValue(p, 3, Part1.Prefix,  Part2.FontFace,       "Arial");
				this.SetValue(p, 3, Part1.Value,   Part2.Text,           "");
				this.SetValue(p, 3, Part1.Suffix,  Part2.Text,           "");

				p.Generator.UserData = user;
				this.ParagraphWrapper.Defined.ItemListParameters = p;
			}

			if ( type == "Bullet2" )
			{
				p = new Text.ParagraphManagers.ItemListManager.Parameters();
				p.Generator = this.document.TextContext.GeneratorList.NewGenerator();

				this.SetValue(p, 0, Part1.Prefix,  Part2.Text,           "");
				this.SetValue(p, 0, Part1.Suffix,  Part2.Text,           "");
				this.SetValue(p, 0, Part1.Generic, Part2.Disposition,    "Center");

				this.SetValue(p, 1, Part1.Generic, Part2.SuppressBefore, "false");
				this.SetValue(p, 1, Part1.Prefix,  Part2.Text,           "\u25A0");  // puce carr�e pleine
				this.SetValue(p, 1, Part1.Prefix,  Part2.FontFace,       "Arial");
//				this.SetValue(p, 1, Part1.Prefix,  Part2.FontColor,      RichColor.ToString(RichColor.FromRgb(1,0,0)));

				this.SetValue(p, 2, Part1.Generic, Part2.SuppressBefore, "true");
				this.SetValue(p, 2, Part1.Prefix,  Part2.Text,           "\u25A1");  // puce carr�e vide
				this.SetValue(p, 2, Part1.Prefix,  Part2.FontFace,       "Arial");

				this.SetValue(p, 3, Part1.Generic, Part2.SuppressBefore, "true");
				this.SetValue(p, 3, Part1.Prefix,  Part2.Text,           "-");
				this.SetValue(p, 3, Part1.Prefix,  Part2.FontFace,       "Arial");

				p.Generator.UserData = user;
				this.ParagraphWrapper.Defined.ItemListParameters = p;
			}

			if ( type == "Bullet3" )
			{
				p = new Text.ParagraphManagers.ItemListManager.Parameters();
				p.Generator = this.document.TextContext.GeneratorList.NewGenerator();

				this.SetValue(p, 0, Part1.Generic, Part2.Disposition,    "Center");

				this.SetValue(p, 1, Part1.Generic, Part2.SuppressBefore, "true");
				this.SetValue(p, 1, Part1.Prefix,  Part2.Text,           "\u25BA");  // triangle >
				this.SetValue(p, 1, Part1.Prefix,  Part2.FontFace,       "Arial");

				p.Generator.UserData = user;
				this.ParagraphWrapper.Defined.ItemListParameters = p;
			}

			if ( type == "Num1" )
			{
				p = new Text.ParagraphManagers.ItemListManager.Parameters();
				p.Generator = this.document.TextContext.GeneratorList.NewGenerator();

				this.SetValue(p, 0, Part1.Generic, Part2.Disposition,    "Right");

				this.SetValue(p, 1, Part1.Generic, Part2.SuppressBefore, "false");
				this.SetValue(p, 1, Part1.Value,   Part2.Text,           Res.Strings.TextPanel.Generator.Numerator.Numeric);
				this.SetValue(p, 1, Part1.Suffix,  Part2.Text,           ".");

				p.Generator.UserData = user;
				this.ParagraphWrapper.Defined.ItemListParameters = p;
			}

			if ( type == "Num2" )
			{
				p = new Text.ParagraphManagers.ItemListManager.Parameters();
				p.Generator = this.document.TextContext.GeneratorList.NewGenerator();

				this.SetValue(p, 0, Part1.Suffix,  Part2.Text,           ")");
				this.SetValue(p, 0, Part1.Generic, Part2.Disposition,    "Right");

				this.SetValue(p, 1, Part1.Generic, Part2.SuppressBefore, "false");
				this.SetValue(p, 1, Part1.Value,   Part2.Text,           Res.Strings.TextPanel.Generator.Numerator.AlphaUpper);
				this.SetValue(p, 1, Part1.Generic, Part2.Tab,            "15");
				this.SetValue(p, 1, Part1.Generic, Part2.Indent,         "20");

				this.SetValue(p, 2, Part1.Generic, Part2.SuppressBefore, "false");
				this.SetValue(p, 2, Part1.Prefix,  Part2.Text,           "-");
				this.SetValue(p, 2, Part1.Value,   Part2.Text,           Res.Strings.TextPanel.Generator.Numerator.Numeric);
//				this.SetValue(p, 2, Part1.Value,   Part2.FontColor,      RichColor.ToString(RichColor.FromRgb(0,0,1)));
				this.SetValue(p, 2, Part1.Generic, Part2.Tab,            "15");
				this.SetValue(p, 2, Part1.Generic, Part2.Indent,         "20");

				this.SetValue(p, 3, Part1.Generic, Part2.SuppressBefore, "true");
				this.SetValue(p, 3, Part1.Prefix,  Part2.Text,           "(");
				this.SetValue(p, 3, Part1.Value,   Part2.Text,           Res.Strings.TextPanel.Generator.Numerator.RomanLower);
				this.SetValue(p, 3, Part1.Suffix,  Part2.Text,           "");
				this.SetValue(p, 3, Part1.Generic, Part2.Tab,            "20");
				this.SetValue(p, 3, Part1.Generic, Part2.Indent,         "25");

				p.Generator.UserData = user;
				this.ParagraphWrapper.Defined.ItemListParameters = p;
			}

			if ( type == "Num3" )
			{
				p = new Text.ParagraphManagers.ItemListManager.Parameters();
				p.Generator = this.document.TextContext.GeneratorList.NewGenerator();

				this.SetValue(p, 0, Part1.Generic, Part2.Disposition,    "Left");

				this.SetValue(p, 1, Part1.Generic, Part2.SuppressBefore, "false");
				this.SetValue(p, 1, Part1.Value,   Part2.Text,           Res.Strings.TextPanel.Generator.Numerator.Numeric);

				this.SetValue(p, 2, Part1.Generic, Part2.SuppressBefore, "true");
				this.SetValue(p, 2, Part1.Value,   Part2.Text,           Res.Strings.TextPanel.Generator.Numerator.AlphaLower);
				this.SetValue(p, 2, Part1.Suffix,  Part2.Text,           ")");

				this.SetValue(p, 3, Part1.Generic, Part2.SuppressBefore, "true");
				this.SetValue(p, 3, Part1.Value,   Part2.Text,           Res.Strings.TextPanel.Generator.Numerator.RomanLower);
				this.SetValue(p, 3, Part1.Suffix,  Part2.Text,           ")");

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


		protected int GetCount()
		{
			//	Nombre de lignes du g�n�rateur.
			if ( !this.ParagraphWrapper.Defined.IsManagedParagraphDefined )  return 0;
			Text.ParagraphManagers.ItemListManager.Parameters p = this.ParagraphWrapper.Defined.ItemListParameters;
			if ( p == null )  return 0;
			return p.Generator.Count;
		}

		protected void IncCount()
		{
			//	Ajoute une nouvelle ligne au g�n�rateur.
			if ( !this.ParagraphWrapper.Defined.IsManagedParagraphDefined )  return;
			Text.ParagraphManagers.ItemListManager.Parameters p = this.ParagraphWrapper.Defined.ItemListParameters;

			int count = p.Generator.Count;
			this.SetValue(count+1, Part1.Generic, Part2.SuppressBefore, "false");
		}

		protected void DecCount()
		{
			//	Supprime la derni�re ligne du g�n�rateur.
			if ( !this.ParagraphWrapper.Defined.IsManagedParagraphDefined )  return;
			Text.ParagraphManagers.ItemListManager.Parameters p = this.ParagraphWrapper.Defined.ItemListParameters;

			int count = p.Generator.Count;
			p.Generator.Truncate(count-1);
		}

		protected string GetResume(int level)
		{
			//	Donne le texte r�sum� pour un niveau complet. Le r�sum� contient aussi tous
			//	les niveaux pr�c�dents.
			if ( level == 0 )
			{
				return "Base";
			}
			else
			{
				System.Text.StringBuilder builder = new System.Text.StringBuilder();

				builder.Append(this.GetResume(0, Part1.Prefix, true));
				int lp = builder.Length;

				for ( int i=1 ; i<=level ; i++ )
				{
					if ( this.GetValue(i, Part1.Generic, Part2.SuppressBefore) == "true" )
					{
						builder.Remove(lp, builder.Length-lp);
					}

					builder.Append(this.GetResume(i, Part1.Prefix, true));
					builder.Append(this.GetResume(i, Part1.Value,  true));
					builder.Append(this.GetResume(i, Part1.Suffix, true));
				}

				builder.Append(this.GetResume(0, Part1.Suffix, true));

				return builder.ToString();
			}
		}

		protected string GetResume(int level, Part1 part1, bool isShort)
		{
			//	Donne le texte r�sum� d'une partie d'un niveau (Prefix, Value ou Suffix).
			//	Le r�sum� contient les commandes pour la police et la couleur.
			string s = this.GetValue(level, part1, Part2.Text);
			if ( s == null )  return "";

			if ( part1 == Part1.Value && isShort )
			{
				s = Generator.ConvTextToShort(s);
			}

			string f = this.GetValue(level, part1, Part2.FontFace);
			string c = this.GetValue(level, part1, Part2.FontColor);

			if ( f == null && c == null )
			{
				return s;
			}
			else
			{
				System.Text.StringBuilder builder = new System.Text.StringBuilder();

				builder.Append("<font");

				if ( f != null )
				{
					builder.Append(" face=\"");
					builder.Append(f);
					builder.Append("\"");
				}

				if ( c != null )
				{
					builder.Append(" color=\"#");
					builder.Append(RichColor.ToHexa(RichColor.Parse(c)));
					builder.Append("\"");
				}

				builder.Append(">");
				builder.Append(s);
				builder.Append("</font>");

				return builder.ToString();
			}
		}

		protected string GetValue(int level, Part1 part1, Part2 part2)
		{
			//	Donne la valeur d'une partie d'un niveau. Il s'agit du passage oblig�
			//	pour lire les d�finitions de puces/num�rotations.
			if ( level < 0 || level > 10 )  return null;
			if ( !this.ParagraphWrapper.Defined.IsManagedParagraphDefined )  return null;

			Text.ParagraphManagers.ItemListManager.Parameters p = this.ParagraphWrapper.Defined.ItemListParameters;
			if ( p == null )  return null;
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

				if ( part2 == Part2.SuppressBefore )
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

				if ( part2 == Part2.FontColor )
				{
					foreach ( Common.Text.Property property in properties )
					{
						if ( property.WellKnownType == Common.Text.Properties.WellKnownType.FontColor )
						{
							Common.Text.Properties.FontColorProperty color = property as Common.Text.Properties.FontColorProperty;
							return color.TextColor;
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
			if ( !this.ParagraphWrapper.Defined.IsManagedParagraphDefined )  return;
			Text.ParagraphManagers.ItemListManager.Parameters p = this.ParagraphWrapper.Defined.ItemListParameters;
			this.SetValue(p, level, part1, part2, value);
		}

		protected void SetValue(Text.ParagraphManagers.ItemListManager.Parameters p, int level, Part1 part1, Part2 part2, string value)
		{
			//	Modifie une valeur d'une partie d'un niveau. Il s'agit du passage oblig�
			//	pour modifier les d�finitions de puces/num�rotations.
			if ( level < 0 || level > 10 )  return;

			Text.TabList tabs = this.document.TextContext.TabList;
			Common.Text.Property[] properties = null;

			if ( p.TabItem == null )
			{
				//	Si aucun tabulateur n'est d�fini, pose 10x2 taquets par d�faut.
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

				if ( part2 == Part2.SuppressBefore )
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

			if ( part2 == Part2.FontFace )
			{
				Common.Text.Properties.FontProperty current = Generator.PropertyGet(properties, Common.Text.Properties.WellKnownType.Font) as Common.Text.Properties.FontProperty;
				if ( value == null )
				{
					properties = Generator.PropertyRemove(properties, Common.Text.Properties.WellKnownType.Font);
				}
				else
				{
					Common.Text.Properties.FontProperty n = new Text.Properties.FontProperty(value, Misc.DefaultFontStyle(value));
					if ( current == null )
					{
						properties = Generator.PropertyAdd(properties, n);
					}
					else
					{
						Generator.PropertyModify(properties, n);
					}
				}
			}

			if ( part2 == Part2.FontStyle )
			{
				Common.Text.Properties.FontProperty current = Generator.PropertyGet(properties, Common.Text.Properties.WellKnownType.Font) as Common.Text.Properties.FontProperty;
				if ( value == null )
				{
					properties = Generator.PropertyRemove(properties, Common.Text.Properties.WellKnownType.Font);
				}
				else
				{
					if ( current == null )
					{
						Common.Text.Properties.FontProperty n = new Text.Properties.FontProperty("Arial", value);
						properties = Generator.PropertyAdd(properties, n);
					}
					else
					{
						Common.Text.Properties.FontProperty n = new Text.Properties.FontProperty(current.FaceName, value);
						Generator.PropertyModify(properties, n);
					}
				}
			}

			if ( part2 == Part2.FontSize )
			{
				Common.Text.Properties.FontSizeProperty current = Generator.PropertyGet(properties, Common.Text.Properties.WellKnownType.FontSize) as Common.Text.Properties.FontSizeProperty;
				if ( value == null )
				{
					properties = Generator.PropertyRemove(properties, Common.Text.Properties.WellKnownType.FontSize);
				}
				else
				{
					Common.Text.Properties.FontSizeProperty n = new Text.Properties.FontSizeProperty(this.ConvTextToDistance(value), Common.Text.Properties.SizeUnits.Points);
					if ( current == null )
					{
						properties = Generator.PropertyAdd(properties, n);
					}
					else
					{
						Generator.PropertyModify(properties, n);
					}
				}
			}

			if ( part2 == Part2.FontColor )
			{
				Common.Text.Properties.FontColorProperty current = Generator.PropertyGet(properties, Common.Text.Properties.WellKnownType.FontColor) as Common.Text.Properties.FontColorProperty;
				if ( value == null )
				{
					properties = Generator.PropertyRemove(properties, Common.Text.Properties.WellKnownType.FontColor);
				}
				else
				{
					Common.Text.Properties.FontColorProperty n = new Text.Properties.FontColorProperty(value);
					if ( current == null )
					{
						properties = Generator.PropertyAdd(properties, n);
					}
					else
					{
						Generator.PropertyModify(properties, n);
					}
				}
			}

			if ( part2 == Part2.FontOffset )
			{
				Common.Text.Properties.FontOffsetProperty current = Generator.PropertyGet(properties, Common.Text.Properties.WellKnownType.FontOffset) as Common.Text.Properties.FontOffsetProperty;
				if ( value == null )
				{
					properties = Generator.PropertyRemove(properties, Common.Text.Properties.WellKnownType.FontOffset);
				}
				else
				{
					Common.Text.Properties.FontOffsetProperty n = new Text.Properties.FontOffsetProperty(this.ConvTextToDistance(value), Common.Text.Properties.SizeUnits.Points);
					if ( current == null )
					{
						properties = Generator.PropertyAdd(properties, n);
					}
					else
					{
						Generator.PropertyModify(properties, n);
					}
				}
			}

			if ( properties != null )
			{
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

			this.ParagraphWrapper.Defined.ItemListParameters = p;
		}

		#region Properties array manager
		protected static Common.Text.Property PropertyGet(Common.Text.Property[] properties, Common.Text.Properties.WellKnownType type)
		{
			//	Cherche une propri�t� dans un tableau.
			if ( properties == null )  return null;

			foreach ( Common.Text.Property property in properties )
			{
				if ( property.WellKnownType == type )  return property;
			}
			return null;
		}

		protected static void PropertyModify(Common.Text.Property[] properties, Common.Text.Property n)
		{
			//	Modifie une propri�t� d'un tableau.
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
			//	Ajoute une propri�t� � la fin d'un tableau.
			int length = (properties == null) ? 0 : properties.Length;
			Common.Text.Property[] list = new Common.Text.Property[length+1];
			for ( int i=0 ; i<length ; i++ )
			{
				list[i] = properties[i];
			}
			list[length] = n;
			return list;
		}

		protected static Common.Text.Property[] PropertyRemove(Common.Text.Property[] properties, Common.Text.Properties.WellKnownType type)
		{
			//	Supprime une propri�t� d'un tableau.
			if ( properties == null )  return null;

			int length = 0;
			foreach ( Common.Text.Property property in properties )
			{
				if ( property.WellKnownType != type )
				{
					length ++;
				}
			}
			if ( length == 0 )  return null;

			Common.Text.Property[] list = new Common.Text.Property[length];
			int i = 0;
			foreach ( Common.Text.Property property in properties )
			{
				if ( property.WellKnownType != type )
				{
					list[i++] = property;
				}
			}
			return list;
		}
		#endregion


		protected static void InitComboType(TextFieldCombo combo)
		{
			combo.Items.Clear();

			combo.Items.Add(Res.Strings.TextPanel.Generator.Type.None);
			
			combo.Items.Add(Res.Strings.TextPanel.Generator.Type.Bullet1);
			combo.Items.Add(Res.Strings.TextPanel.Generator.Type.Bullet2);
			combo.Items.Add(Res.Strings.TextPanel.Generator.Type.Bullet3);
			
			combo.Items.Add(Res.Strings.TextPanel.Generator.Type.Num1);
			combo.Items.Add(Res.Strings.TextPanel.Generator.Type.Num2);
			combo.Items.Add(Res.Strings.TextPanel.Generator.Type.Num3);
			
			combo.Items.Add(Res.Strings.TextPanel.Generator.Type.Custom);
		}

		protected static void InitComboFix(TextFieldCombo combo)
		{
			combo.Items.Clear();

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

		protected static void InitComboNumerator(TextFieldCombo combo)
		{
			combo.Items.Clear();

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

		protected static string ConvTextToShort(string text)
		{
			if ( text == Res.Strings.TextPanel.Generator.Numerator.None || text == "" )
			{
				return "";
			}

			if ( text == Res.Strings.TextPanel.Generator.Numerator.Numeric )
			{
				return "1";
			}

			if ( text == Res.Strings.TextPanel.Generator.Numerator.AlphaLower )
			{
				return "a";
			}

			if ( text == Res.Strings.TextPanel.Generator.Numerator.AlphaUpper )
			{
				return "A";
			}

			if ( text == Res.Strings.TextPanel.Generator.Numerator.RomanLower )
			{
				return "i";
			}

			if ( text == Res.Strings.TextPanel.Generator.Numerator.RomanUpper )
			{
				return "I";
			}

			return "";
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

		protected static Part1 ConvColumnToPart1(int column)
		{
			Part1 part1 = Part1.Generic;
			if ( column == 1 )  part1 = Part1.Prefix;
			if ( column == 2 )  part1 = Part1.Value;
			if ( column == 3 )  part1 = Part1.Suffix;
			return part1;
		}

		
		protected void UpdateTable()
		{
			//	Met � jour le contenu de la liste.
			int columns = 4;
			int rows = 0;

			if ( this.ParagraphWrapper.Defined.IsManagedParagraphDefined )
			{
				Text.ParagraphManagers.ItemListManager.Parameters p = this.ParagraphWrapper.Defined.ItemListParameters;
				if ( p != null )
				{
					rows = 1 + p.Generator.Count;
				}
			}

			int initialColumns = this.table.Columns;
			this.table.SetArraySize(columns, rows);

			if ( initialColumns != this.table.Columns )  // changement du nombre de colonnes ?
			{
				this.table.SetWidthColumn(0, 49);
				this.table.SetWidthColumn(1, 36);
				this.table.SetWidthColumn(2, 36);
				this.table.SetWidthColumn(3, 36);
			}

			this.table.SetHeaderTextH(0, "Texte");
			this.table.SetHeaderTextH(1, "Pr�fix");
			this.table.SetHeaderTextH(2, "Num.");
			this.table.SetHeaderTextH(3, "Suffix");

			for ( int i=0 ; i<rows ; i++ )
			{
				this.TableFillRow(i);
				this.TableUpdateRow(i);
			}
		}

		protected void TableFillRow(int row)
		{
			//	Peuple une ligne de la table, si n�cessaire.
			if ( this.table[0, row].IsEmpty )
			{
				StaticText st = new StaticText();
				st.Alignment = ContentAlignment.MiddleLeft;
				st.Dock = DockStyle.Fill;
				st.DockMargins = new Drawing.Margins(2, 2, 0, 0);
				this.table[0, row].Insert(st);
			}

			if ( this.table[1, row].IsEmpty )
			{
				StaticText st = new StaticText();
				st.Alignment = ContentAlignment.MiddleCenter;
				st.Dock = DockStyle.Fill;
				st.DockMargins = new Drawing.Margins(2, 0, 0, 0);
				this.table[1, row].Insert(st);
			}

			if ( this.table[2, row].IsEmpty )
			{
				StaticText st = new StaticText();
				st.Alignment = ContentAlignment.MiddleCenter;
				st.Dock = DockStyle.Fill;
				st.DockMargins = new Drawing.Margins(2, 0, 0, 0);
				this.table[2, row].Insert(st);
			}

			if ( this.table[3, row].IsEmpty )
			{
				StaticText st = new StaticText();
				st.Alignment = ContentAlignment.MiddleCenter;
				st.Dock = DockStyle.Fill;
				st.DockMargins = new Drawing.Margins(2, 0, 0, 0);
				this.table[3, row].Insert(st);
			}
		}

		protected void TableUpdateRow(int row)
		{
			//	Met � jour le contenu d'une ligne de la table.
			StaticText st;

			st = this.table[0, row].Children[0] as StaticText;
			st.Text = this.GetResume(row);
			string justif = this.GetValue(0, Part1.Generic, Part2.Disposition);
			if ( justif == "Left"   )  st.Alignment = ContentAlignment.MiddleLeft;
			if ( justif == "Center" )  st.Alignment = ContentAlignment.MiddleCenter;
			if ( justif == "Right"  )  st.Alignment = ContentAlignment.MiddleRight;

			st = this.table[1, row].Children[0] as StaticText;
			st.Text = this.GetResume(row, Part1.Prefix, false);

			st = this.table[2, row].Children[0] as StaticText;
			st.Text = this.GetResume(row, Part1.Value, false);

			st = this.table[3, row].Children[0] as StaticText;
			st.Text = this.GetResume(row, Part1.Suffix, false);
		}

		
		protected override void UpdateClientGeometry()
		{
			//	Met � jour la g�om�trie.
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

			if ( this.isExtendedSize )  // panneau �tendu ?
			{
				r.Offset(0, -25);
				r.Left = rect.Left;
				r.Right = rect.Right;
				r.Bottom = r.Top-(23+17*4);
				this.table.Bounds = r;
				this.table.Visibility = true;

				r.Top = r.Bottom-5;
				r.Bottom = r.Top-20;
				r.Left = rect.Left;
				r.Width = 20;
				this.buttonAdd.Bounds = r;
				r.Offset(20, 0);
				this.buttonSub.Bounds = r;
				r.Offset(20+10, 0);
				this.buttonLeft.Bounds = r;
				r.Offset(20, 0);
				this.buttonCenter.Bounds = r;
				r.Offset(20, 0);
				this.buttonRight.Bounds = r;

				r.Left = rect.Left;
				r.Width = 20;
				this.buttonSuppressBefore.Bounds = r;
				r.Offset(20+10, 0);
				r.Width = 30;
				this.labelText.Bounds = r;
				r.Offset(30+3, 0);
				r.Width = 60;
				this.fieldText.Bounds = r;
				r.Left = rect.Right-48;
				r.Right = rect.Right;
				this.colorText.Bounds = r;

				r.Offset(0, -25);
				r.Left = rect.Left;
				r.Width = 90;
				this.fieldTab.Bounds = r;
				r.Offset(90, 0);
				this.fieldIndent.Bounds = r;

				r.Left = rect.Left;
				r.Width = 90;
				this.fieldFontSize.Bounds = r;
				r.Offset(90, 0);
				this.fieldFontOffset.Bounds = r;
			}
			else
			{
				this.table.Visibility = false;
			}

			this.UpdateButtonClear();
		}

		protected void UpdateButtonClear()
		{
			this.buttonClear.Visibility = !this.ParagraphWrapper.IsAttachedToDefaultParagraphStyle;
		}

		protected override void UpdateAfterChanging()
		{
			//	Met � jour apr�s un changement du wrapper.
			base.UpdateAfterChanging();
			
			if ( this.ParagraphWrapper.IsAttached == false )  return;

			bool isGenerator = this.ParagraphWrapper.Defined.IsManagedParagraphDefined;
			string type = "None";

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
			}

			this.Type = type;

			this.ignoreChanged = true;
			this.fieldType.Text = Generator.ConvTypeToText(type);
			this.ProposalTextFieldCombo(this.fieldType, !isGenerator);
			this.ignoreChanged = false;

			this.UpdateTable();
			this.UpdateWidgets();
			this.UpdateFields();
		}

		protected void UpdateFields()
		{
			this.ignoreChanged = true;

			int row    = this.tableSelectedRow;
			int column = this.tableSelectedColumn;
			Part1 part1 = Generator.ConvColumnToPart1(column);
			string text;

			text = this.GetValue(0, Part1.Generic, Part2.Disposition);
			this.buttonLeft.ActiveState   = (text == "Left"  ) ? ActiveState.Yes : ActiveState.No;
			this.buttonCenter.ActiveState = (text == "Center") ? ActiveState.Yes : ActiveState.No;
			this.buttonRight.ActiveState  = (text == "Right" ) ? ActiveState.Yes : ActiveState.No;

			text = this.GetValue(row, Part1.Generic, Part2.SuppressBefore);
			this.buttonSuppressBefore.ActiveState = (text == "true"  ) ? ActiveState.Yes : ActiveState.No;

			text = this.GetResume(row, part1, false);
			this.fieldText.Text = text;

			if ( part1 == Part1.Prefix || part1 == Part1.Suffix )
			{
				this.labelText.Text = "Texte";
				this.fieldText.IsReadOnly = false;
				Generator.InitComboFix(this.fieldText);
				ToolTip.Default.SetToolTip(this.fieldText, "Texte fixe");
			}
			
			if ( part1 == Part1.Value )
			{
				this.labelText.Text = "Num.";
				this.fieldText.IsReadOnly = true;
				Generator.InitComboNumerator(this.fieldText);
				ToolTip.Default.SetToolTip(this.fieldText, "Type de num�rotation");
			}
			
			text = this.GetValue(row, part1, Part2.FontSize);
			if ( text == null )
			{
				this.fieldFontSize.TextFieldReal.ClearText();
			}
			else
			{
				this.fieldFontSize.TextFieldReal.Text = text;
			}

			text = this.GetValue(row, part1, Part2.FontOffset);
			if ( text == null )
			{
				this.fieldFontOffset.TextFieldReal.ClearText();
			}
			else
			{
				this.fieldFontOffset.TextFieldReal.Text = text;
			}

			text = this.GetValue(row, Part1.Generic, Part2.Tab);
			if ( text == null )
			{
				this.fieldTab.TextFieldReal.ClearText();
			}
			else
			{
				this.fieldTab.TextFieldReal.Text = text;
			}

			text = this.GetValue(row, Part1.Generic, Part2.Indent);
			if ( text == null )
			{
				this.fieldIndent.TextFieldReal.ClearText();
			}
			else
			{
				this.fieldIndent.TextFieldReal.Text = text;
			}

			this.ignoreChanged = false;
		}

		protected void UpdateWidgets()
		{
			bool custom = (this.type == "Custom");

			int row    = this.tableSelectedRow;
			int column = this.tableSelectedColumn;
			bool enable;

			enable = (this.isExtendedSize && column == 0);
			this.buttonAdd.Visibility    = enable;
			this.buttonSub.Visibility    = enable;
			this.buttonLeft.Visibility   = enable;
			this.buttonCenter.Visibility = enable;
			this.buttonRight.Visibility  = enable;

			enable = (this.isExtendedSize && column != 0 && column != -1);
			if ( row == 0 && column == 2 )  enable = false;
			this.buttonSuppressBefore.Visibility = (enable && row != 0);
			this.labelText.Visibility       = enable;
			this.fieldText.Visibility       = enable;
			this.fieldFontSize.Visibility   = enable;
			this.fieldFontOffset.Visibility = enable;
			this.colorText.Visibility       = enable;

			enable = (this.isExtendedSize && column == 0 && row != 0);
			this.fieldTab.Visibility        = enable;
			this.fieldIndent.Visibility     = enable;

			int count = this.GetCount();
			this.table.Enable = custom;
			this.buttonAdd.Enable = (custom && count < 9);
			this.buttonSub.Enable = (custom && count > 1);
			this.buttonLeft.Enable = custom;
			this.buttonCenter.Enable = custom;
			this.buttonRight.Enable = custom;
			this.buttonSuppressBefore.Enable = custom;
			this.labelText.Enable = custom;
			this.fieldText.Enable = custom;
			this.fieldFontSize.Enable = custom;
			this.fieldFontOffset.Enable = custom;
			this.colorText.Enable = custom;
			this.fieldTab.Enable = custom;
			this.fieldIndent.Enable = custom;
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

					this.table.DeselectAll();
					this.tableSelectedRow    = -1;
					this.tableSelectedColumn = -1;

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

		private void HandleTableSelectionChanged(object sender)
		{
			this.tableSelectedRow    = this.table.SelectedRow;
			this.tableSelectedColumn = this.table.SelectedColumn;

			this.UpdateWidgets();
			this.UpdateFields();
		}

		private void HandleAddClicked(object sender, MessageEventArgs e)
		{
			this.IncCount();
		}

		private void HandleSubClicked(object sender, MessageEventArgs e)
		{
			this.DecCount();
		}

		private void HandleJustifClicked(object sender, MessageEventArgs e)
		{
			string text = null;
			if ( sender == this.buttonLeft   )  text = "Left";
			if ( sender == this.buttonCenter )  text = "Center";
			if ( sender == this.buttonRight  )  text = "Right";
			this.SetValue(0, Part1.Generic, Part2.Disposition, text);
		}

		private void HandleSuppressBeforeClicked(object sender, MessageEventArgs e)
		{
			IconButton button = sender as IconButton;

			int row = this.tableSelectedRow;
			if ( row == -1 )  return;

			string text = "false";
			if ( button.ActiveState == ActiveState.No)  text = "true";
			this.SetValue(row, Part1.Generic, Part2.SuppressBefore, text);
		}

		private void HandleTextChanged(object sender)
		{
			if ( this.ignoreChanged )  return;

			int row    = this.tableSelectedRow;
			int column = this.tableSelectedColumn;
			if ( row == -1 || column == -1 )  return;

			string text = this.fieldText.Text;
			string font = null;
			if ( text.StartsWith("<font face=\"") )
			{
				font = text.Substring(12);
				int i = font.IndexOf("\"");
				if ( i != -1 )
				{
					font = font.Substring(0, i);
				}
				text = TextLayout.ConvertToSimpleText(text);
			}

			this.SetValue(row, Generator.ConvColumnToPart1(column), Part2.Text, text);
			this.SetValue(row, Generator.ConvColumnToPart1(column), Part2.FontFace, font);
		}

		private void HandleFontSizeChanged(object sender)
		{
			if ( this.ignoreChanged )  return;

			int row    = this.tableSelectedRow;
			int column = this.tableSelectedColumn;
			if ( row == -1 || column == -1 )  return;

			string text = this.fieldFontSize.TextFieldReal.Text;
			this.SetValue(row, Generator.ConvColumnToPart1(column), Part2.FontSize, text);
		}

		private void HandleFontOffsetChanged(object sender)
		{
			if ( this.ignoreChanged )  return;

			int row    = this.tableSelectedRow;
			int column = this.tableSelectedColumn;
			if ( row == -1 || column == -1 )  return;

			string text = this.fieldFontOffset.TextFieldReal.Text;
			this.SetValue(row, Generator.ConvColumnToPart1(column), Part2.FontOffset, text);
		}

		private void HandleTabChanged(object sender)
		{
			if ( this.ignoreChanged )  return;

			int row = this.tableSelectedRow;
			if ( row == -1 )  return;

			string text = this.fieldTab.TextFieldReal.Text;
			this.SetValue(row, Part1.Generic, Part2.Tab, text);
		}

		private void HandleIndentChanged(object sender)
		{
			if ( this.ignoreChanged )  return;

			int row = this.tableSelectedRow;
			if ( row == -1 )  return;

			string text = this.fieldIndent.TextFieldReal.Text;
			this.SetValue(row, Part1.Generic, Part2.Indent, text);
		}

		private void HandleSampleColorClicked(object sender, MessageEventArgs e)
		{
		}

		private void HandleSampleColorChanged(object sender)
		{
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

		protected TextFieldCombo			fieldType;
		protected CellTable					table;
		protected IconButton				buttonAdd;
		protected IconButton				buttonSub;
		protected IconButton				buttonLeft;
		protected IconButton				buttonCenter;
		protected IconButton				buttonRight;
		protected IconButton				buttonSuppressBefore;
		protected StaticText				labelText;
		protected TextFieldCombo			fieldText;
		protected Widgets.TextFieldLabel	fieldTab;
		protected Widgets.TextFieldLabel	fieldIndent;
		protected Widgets.TextFieldLabel	fieldFontSize;
		protected Widgets.TextFieldLabel	fieldFontOffset;
		protected ColorSample				colorText;
		protected IconButton				buttonClear;

		protected string					type = null;
		protected int						tableSelectedRow = -1;
		protected int						tableSelectedColumn = -1;
	}
}
