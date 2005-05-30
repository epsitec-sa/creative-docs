using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.Document
{
	/// <summary>
	/// Summary description for Dialogs.
	/// </summary>
	public class Dialogs
	{
		public Dialogs(Document document)
		{
			this.document = document;
			this.widgetsTable = new System.Collections.Hashtable();
		}


		// Met � jour tous les r�glages dans les diff�rents dialogues.
		public void UpdateAllSettings()
		{
			this.UpdateSettings(false);
			this.UpdatePrint(false);
			this.UpdateExport(false);
		}


		#region Infos
		// Peuple le dialogue des informations.
		public void BuildInfos(Window window)
		{
			if ( this.windowInfos == null )
			{
				this.windowInfos = window;
			}

			this.UpdateInfos();
		}

		// Met � jour le dialogue des informations.
		public void UpdateInfos()
		{
			if ( this.windowInfos == null || !this.windowInfos.IsVisible )  return;

			TextFieldMulti multi = this.windowInfos.Root.FindChild("Infos") as TextFieldMulti;
			if ( multi != null )
			{
				multi.Text = this.document.Modifier.Statistic(true, true);
			}
		}
		#endregion

		#region Settings
		// Peuple le dialogue des r�glages.
		public void BuildSettings(Window window)
		{
			if ( this.windowSettings == null )
			{
				this.windowSettings = window;

				Widget parent, container;
				Widget book = this.windowSettings.Root.FindChild("BookDocument");

				// Onglet Format:
				parent = book.FindChild("Format");
				container = new Widget(parent);
				container.Name = "Container";
				container.Dock = DockStyle.Fill;

				this.tabIndex = 0;
				if ( this.document.Type == DocumentType.Pictogram )
				{
					Dialogs.CreateTitle(container, Res.Strings.Dialog.Settings.SizePic);
					this.CreatePoint(container, "PageSize");
				}
				else
				{
					Dialogs.CreateTitle(container, Res.Strings.Dialog.Settings.SizeDoc);
					this.CreatePaper(container);
					this.CreatePoint(container, "PageSize");
					this.CreateDouble(container, "OutsideArea");
					Dialogs.CreateSeparator(container);
					this.CreateCombo(container, "DefaultUnit");
				}
				Dialogs.CreateSeparator(container);

				// Onglet Grid:
				parent = book.FindChild("Grid");
				container = new Widget(parent);
				container.Name = "Container";
				container.Dock = DockStyle.Fill;

				this.tabIndex = 0;
				Dialogs.CreateTitle(container, Res.Strings.Dialog.Settings.Grid);
				this.CreateBool(container, "GridActive");
				this.CreateBool(container, "GridShow");
				Dialogs.CreateSeparator(container);
				this.CreatePoint(container, "GridStep");
				this.CreatePoint(container, "GridSubdiv");
				this.CreatePoint(container, "GridOffset");
				Dialogs.CreateSeparator(container);

				// Onglet Guides:
				parent = book.FindChild("Guides");
				container = new Widget(parent);
				container.Name = "Container";
				container.Dock = DockStyle.Fill;

				this.tabIndex = 0;
				Dialogs.CreateTitle(container, Res.Strings.Dialog.Settings.Magnet);
				this.CreateBool(container, "GuidesActive");
				this.CreateBool(container, "GuidesShow");
				this.CreateBool(container, "GuidesMouse");
				Dialogs.CreateSeparator(container);

				this.containerGuides = new Containers.Guides(this.document);
				this.containerGuides.Dock = DockStyle.Fill;
				this.containerGuides.DockMargins = new Margins(10, 10, 4, 10);
				this.containerGuides.Parent = container;

				// Onglet Move:
				parent = book.FindChild("Move");
				container = new Widget(parent);
				container.Name = "Container";
				container.Dock = DockStyle.Fill;

				this.tabIndex = 0;
				Dialogs.CreateTitle(container, Res.Strings.Dialog.Settings.DuplicateMove);
				this.CreatePoint(container, "DuplicateMove");
				this.CreateBool(container, "RepeatDuplicateMove");

				Dialogs.CreateTitle(container, Res.Strings.Dialog.Settings.ArrowMove);
				this.CreatePoint(container, "ArrowMove");
				this.CreateDouble(container, "ArrowMoveMul");
				this.CreateDouble(container, "ArrowMoveDiv");
				Dialogs.CreateSeparator(container);

				// Onglet Misc:
				parent = book.FindChild("Misc");
				container = new Widget(parent);
				container.Name = "Container";
				container.Dock = DockStyle.Fill;

				this.tabIndex = 0;
				Dialogs.CreateTitle(container, Res.Strings.Dialog.Settings.Dimension);
				this.CreateDouble(container, "DimensionScale");
				this.CreateDouble(container, "DimensionDecimal");

				Dialogs.CreateTitle(container, Res.Strings.Dialog.Settings.ToLinePrecision);
				this.CreateDouble(container, "ToLinePrecision");
				Dialogs.CreateSeparator(container);
			}

			this.UpdateSettings(true);
		}

		// Appel� lorsque les r�glages ont chang�.
		protected void UpdateSettings(bool force)
		{
			if ( !force )
			{
				if ( this.windowSettings == null || !this.windowSettings.IsVisible )  return;
			}

			this.UpdateDialogSettings("Settings");
		}

		// Appel� lorsque les rep�res ont chang�.
		public void UpdateGuides()
		{
			if ( this.containerGuides == null )  return;
			this.containerGuides.SetDirtyContent();
		}

		// S�lectionne un rep�re.
		public void SelectGuide(int rank)
		{
			if ( this.containerGuides == null )  return;
			this.containerGuides.SelectGuide = rank;
		}
		#endregion

		#region Print
		// Peuple le dialogue d'impression.
		public void BuildPrint(Window window)
		{
			if ( this.windowPrint == null )
			{
				this.windowPrint = window;

				Widget parent, container;
				Widget book = this.windowPrint.Root.FindChild("Book");

				// Onglet Imprimante:
				parent = book.FindChild("Printer");
				container = new Widget(parent);
				container.Name = "Container";
				container.Dock = DockStyle.Fill;
				
				this.tabIndex = 0;
				Dialogs.CreateTitle(container, Res.Strings.Dialog.Print.Printer);
				this.CreatePrinter(container, "PrintName");

				Dialogs.CreateTitle(container, Res.Strings.Dialog.Print.Area);
				this.CreateRange(container, "PrintRange");
				this.CreateCombo(container, "PrintArea");

				Dialogs.CreateTitle(container, Res.Strings.Dialog.Print.Copies);
				this.CreateDouble(container, "PrintCopies");
				this.CreateBool(container, "PrintCollate");
				this.CreateBool(container, "PrintReverse");
				Dialogs.CreateSeparator(container);

				// Onglet Param�tres:
				parent = book.FindChild("Param");
				container = new Widget(parent);
				container.Name = "Container";
				container.Dock = DockStyle.Fill;
				
				this.tabIndex = 0;
				Dialogs.CreateTitle(container, Res.Strings.Dialog.Print.Param);
				this.CreateBool(container, "PrintAutoLandscape");
				this.CreateBool(container, "PrintDraft");
				this.CreateBool(container, "PrintPerfectJoin");
#if DEBUG
				this.CreateBool(container, "PrintDebugArea");
#endif
				this.CreateDouble(container, "PrintDpi");
				this.CreateBool(container, "PrintAA");

				Dialogs.CreateTitle(container, Res.Strings.Dialog.Print.File);
				this.CreateBool(container, "PrintToFile");
				this.CreateFilename(container, "PrintFilename");
				Dialogs.CreateSeparator(container);

				// Onglet Pr�-presse:
				parent = book.FindChild("Publisher");
				container = new Widget(parent);
				container.Name = "Container";
				container.Dock = DockStyle.Fill;
				
				this.tabIndex = 0;
				Dialogs.CreateTitle(container, Res.Strings.Dialog.Print.Publisher);
				this.CreateBool(container, "PrintAutoZoom");
				this.CreateCombo(container, "PrintCentring");
				this.CreateDouble(container, "PrintMargins");
				this.CreateDouble(container, "PrintDebord");
				this.CreateBool(container, "PrintTarget");
				Dialogs.CreateSeparator(container);
			}

			this.UpdatePrint(true);
		}

		// Appel� lorsque les r�glages ont chang�.
		protected void UpdatePrint(bool force)
		{
			if ( !force )
			{
				if ( this.windowPrint == null || !this.windowPrint.IsVisible )  return;
			}

			this.UpdatePrinter("PrintName");
			this.UpdateRangeField("PrintRange");
			this.UpdateDialogSettings("Print");
		}

		// Appel� lorsque les pages ont chang�.
		public void UpdatePrintPages()
		{
			if ( this.windowPrint == null || !this.windowPrint.IsVisible )  return;

			this.UpdateRangeField("PrintRange");
		}
		#endregion

		#region Export
		// Peuple le dialogue des exportations.
		public void BuildExport(Window window)
		{
			if ( this.windowExport == null )
			{
				this.windowExport = window;

				Widget parent = this.windowExport.Root.FindChild("Panel");
				Panel container = new Panel(parent);
				container.Name = "Container";
				container.Dock = DockStyle.Fill;

				this.tabIndex = 0;

				Dialogs.CreateTitle(container, Res.Strings.Dialog.Export.Param);
				this.CreateDouble(container, "ImageDpi");
				this.CreateCombo(container, "ImageDepth");
				this.CreateCombo(container, "ImageCompression");
				this.CreateDouble(container, "ImageQuality");
				this.CreateDouble(container, "ImageAA");
				Dialogs.CreateSeparator(container);
			}

			this.UpdateExport(true);
		}

		// Appel� lorsque les r�glages ont chang�.
		protected void UpdateExport(bool force)
		{
			if ( !force )
			{
				if ( this.windowExport == null || !this.windowExport.IsVisible )  return;
			}

			this.UpdateDialogSettings("Export");
			this.UpdateCombo("ImageDepth");
			this.UpdateCombo("ImageCompression");
			this.UpdateDouble("ImageQuality");
		}
		#endregion

		#region Glyphs
		// Peuple le dialogue des informations.
		public void BuildGlyphs(Window window)
		{
			if ( this.windowGlyphs == null )
			{
				this.windowGlyphs = window;
			}

			this.UpdateGlyphs();
		}

		// Met � jour le dialogue des informations.
		public void UpdateGlyphs()
		{
			if ( this.windowGlyphs == null || !this.windowGlyphs.IsVisible )  return;
		}
		#endregion


		#region WidgetTitle
		// Cr�e un widget de titre pour un onglet.
		public static void CreateTitle(Widget parent, string labelText)
		{
#if false
			StaticText text = new StaticText(parent);
			text.Text = string.Format("<b>{0}</b>", labelText);
			text.Dock = DockStyle.Top;
			text.DockMargins = new Margins(10, 10, 8, 2);

			Separator sep = new Separator(parent);
			sep.Width = parent.Width;
			sep.Height = 1;
			sep.Dock = DockStyle.Top;
			sep.DockMargins = new Margins(0, 0, 3, 6);
#else
			Separator sep = new Separator(parent);
			sep.Width = parent.Width;
			sep.Height = 1;
			sep.Dock = DockStyle.Top;
			sep.DockMargins = new Margins(0, 0, 6, 3);

			StaticText text = new StaticText(parent);
			text.Text = string.Format("<b>{0}</b>", labelText);
			text.Dock = DockStyle.Top;
			text.DockMargins = new Margins(10, 10, 2, 8);
#endif
		}

		// Cr�e un s�parateur pour un onglet.
		public static void CreateSeparator(Widget parent)
		{
			Separator sep = new Separator(parent);
			sep.Width = parent.Width;
			sep.Height = 1;
			sep.Dock = DockStyle.Top;
			sep.DockMargins = new Margins(0, 0, 4, 6);
		}
		#endregion

		#region WidgetLabel
		// Cr�e des widgets pour afficher un texte fixe.
		public static void CreateLabel(Widget parent, string label, string info)
		{
			Panel container = new Panel(parent);
			container.Height = 18;
			container.Dock = DockStyle.Top;
			container.DockMargins = new Margins(10, 10, 0, 0);

			StaticText text = new StaticText(container);
			text.Text = label;
			text.Width = 120;
			text.Dock = DockStyle.Left;
			text.DockMargins = new Margins(0, 0, 0, 0);

			text = new StaticText(container);
			text.Text = info;
			text.Width = 150;
			text.Dock = DockStyle.Left;
			text.DockMargins = new Margins(0, 0, 0, 0);
		}
		#endregion

		#region WidgetBool
		// Cr�e un widget pour �diter un r�glage de type Bool.
		protected void CreateBool(Widget parent, string name)
		{
			Settings.Abstract settings = this.document.Settings.Get(name);
			if ( settings == null )  return;
			Settings.Bool sBool = settings as Settings.Bool;
			if ( sBool == null )  return;

			CheckButton check = new CheckButton(parent);
			check.Text = sBool.Text;
			check.Width = 100;
			check.Name = sBool.Name;
			check.ActiveState = sBool.Value ? WidgetState.ActiveYes : WidgetState.ActiveNo;
			check.TabIndex = this.tabIndex++;
			check.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			check.Dock = DockStyle.Top;
			check.DockMargins = new Margins(10, 10, 0, 5);
			check.ActiveStateChanged += new EventHandler(this.HandleCheckActiveStateChanged);
			this.WidgetsTableAdd(check, "");
		}

		private void HandleCheckActiveStateChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			CheckButton check = sender as CheckButton;
			if ( check == null )  return;

			Settings.Abstract settings = this.document.Settings.Get(check.Name);
			if ( settings == null )  return;
			Settings.Bool sBool = settings as Settings.Bool;
			if ( sBool == null )  return;

			sBool.Value = ( check.ActiveState == WidgetState.ActiveYes );

			int total = this.document.Settings.Count;
			for ( int i=0 ; i<total ; i++ )
			{
				Settings.Abstract setting = this.document.Settings.Get(i);
				if ( setting.ConditionName == "" )  continue;

				if ( setting.ConditionName == sBool.Name )
				{
					this.EnableWidget(setting.Name, sBool.Value^setting.ConditionState);
				}
			}
		}
		#endregion

		#region WidgetDouble
		// Cr�e des widgets pour �diter un r�glage de type Double.
		protected void CreateDouble(Widget parent, string name)
		{
			Settings.Abstract settings = this.document.Settings.Get(name);
			if ( settings == null )  return;
			Settings.Double sDouble = settings as Settings.Double;
			if ( sDouble == null )  return;

			Panel container = new Panel(parent);
			container.Height = 22;
			container.TabIndex = this.tabIndex++;
			container.Dock = DockStyle.Top;
			container.DockMargins = new Margins(10, 10, 0, sDouble.Info?0:5);

			StaticText text = new StaticText(container);
			text.Text = sDouble.Text;
			text.Width = 120;
			text.Dock = DockStyle.Left;
			text.DockMargins = new Margins(0, 0, 0, 0);

			TextFieldReal field = new TextFieldReal(container);
			field.Width = 60;
			field.Name = sDouble.Name;
			field.TextSuffix = sDouble.Suffix;
			if ( sDouble.Integer )
			{
				this.document.Modifier.AdaptTextFieldRealScalar(field);
				field.MinValue = (decimal) sDouble.FactorMinValue;
				field.MaxValue = (decimal) sDouble.FactorMaxValue;
				field.Resolution = (decimal) sDouble.FactorResolution;
				field.Step = (decimal) sDouble.FactorStep;
			}
			else
			{
				field.FactorMinRange = (decimal) sDouble.FactorMinValue;
				field.FactorMaxRange = (decimal) sDouble.FactorMaxValue;
				field.FactorStep = (decimal) sDouble.FactorStep;
				this.document.Modifier.AdaptTextFieldRealDimension(field);
			}
			field.InternalValue = (decimal) sDouble.Value;
			field.ValueChanged += new EventHandler(this.HandleFieldDoubleChanged);
			field.TabIndex = this.tabIndex++;
			field.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			field.Dock = DockStyle.Left;
			field.DockMargins = new Margins(0, 0, 0, 0);
			this.WidgetsTableAdd(field, "");

			if ( sDouble.Info )
			{
				container = new Panel(parent);
				container.Height = 18;
				container.TabIndex = this.tabIndex++;
				container.Dock = DockStyle.Top;
				container.DockMargins = new Margins(10, 10, 0, 5);

				text = new StaticText(container);
				text.Name = sDouble.Name;
				text.Text = sDouble.GetInfo();
				text.Width = 150;
				text.SetClientZoom(0.8);
				text.Dock = DockStyle.Left;
				text.DockMargins = new Margins(120, 0, 0, 0);
				this.WidgetsTableAdd(text, ".Info");
			}
		}

		private void HandleFieldDoubleChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			TextFieldReal field = sender as TextFieldReal;
			if ( field == null )  return;

			Settings.Abstract settings = this.document.Settings.Get(field.Name);
			if ( settings == null )  return;
			Settings.Double sDouble = settings as Settings.Double;
			if ( sDouble == null )  return;

			sDouble.Value = (double) field.InternalValue;

			if ( sDouble.Info )
			{
				StaticText info = this.WidgetsTableSearch(sDouble.Name, ".Info") as StaticText;
				info.Text = sDouble.GetInfo();
			}
		}

		protected void UpdateDouble(string name)
		{
			Settings.Abstract settings = this.document.Settings.Get(name);
			if ( settings == null )  return;
			Settings.Double sDouble = settings as Settings.Double;
			if ( sDouble == null )  return;

			TextFieldSlider field = this.WidgetsTableSearch(name, "") as TextFieldSlider;
			if ( field != null )
			{
				field.SetEnabled(sDouble.IsEnabled);
			}

			if ( sDouble.Info )
			{
				StaticText info = this.WidgetsTableSearch(name, ".Info") as StaticText;
				if ( info != null )
				{
					info.SetVisible(sDouble.IsEnabled);
				}
			}
		}
		#endregion

		#region WidgetPoint
		// Cr�e des widgets pour �diter un r�glage de type Point.
		protected void CreatePoint(Widget parent, string name)
		{
			Settings.Abstract settings = this.document.Settings.Get(name);
			if ( settings == null )  return;
			Settings.Point sPoint = settings as Settings.Point;
			if ( sPoint == null )  return;

			StaticText text;
			TextFieldReal field;

			Panel container = new Panel(parent);
			container.Height = 22+2+22;
			container.TabIndex = this.tabIndex++;
			container.Dock = DockStyle.Top;
			container.DockMargins = new Margins(10, 10, 0, 5);

			Panel containerXY = new Panel(container);
			containerXY.Width = 120+60;
			containerXY.Height = container.Height;
			containerXY.TabIndex = this.tabIndex++;
			containerXY.Dock = DockStyle.Left;
			containerXY.DockMargins = new Margins(0, 0, 0, 0);

			Panel containerX = new Panel(containerXY);
			containerX.Width = containerXY.Width;
			containerX.Height = 22;
			containerX.TabIndex = this.tabIndex++;
			containerX.Dock = DockStyle.Top;
			containerX.DockMargins = new Margins(0, 0, 0, 0);

			text = new StaticText(containerX);
			text.Text = sPoint.TextX;
			text.Width = 120;
			text.Dock = DockStyle.Left;
			text.DockMargins = new Margins(0, 0, 0, 0);

			field = new TextFieldReal(containerX);
			field.Width = 60;
			field.Name = sPoint.Name;
			if ( sPoint.Integer )
			{
				this.document.Modifier.AdaptTextFieldRealScalar(field);
				field.MinValue = (decimal) sPoint.FactorMinValue;
				field.MaxValue = (decimal) sPoint.FactorMaxValue;
				field.Step = (decimal) sPoint.FactorStep;
			}
			else
			{
				field.FactorMinRange = (decimal) sPoint.FactorMinValue;
				field.FactorMaxRange = (decimal) sPoint.FactorMaxValue;
				field.FactorStep = (decimal) sPoint.FactorStep;
				this.document.Modifier.AdaptTextFieldRealDimension(field);
			}
			field.InternalValue = (decimal) sPoint.Value.X;
			field.ValueChanged += new EventHandler(this.HandleFieldPointXChanged);
			field.TabIndex = this.tabIndex++;
			field.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			field.Dock = DockStyle.Left;
			field.DockMargins = new Margins(0, 0, 0, 0);
			this.WidgetsTableAdd(field, ".X");

			Panel containerY = new Panel(containerXY);
			containerY.Width = containerXY.Width;
			containerY.Height = 22;
			containerY.TabIndex = this.tabIndex++;
			containerY.Dock = DockStyle.Bottom;
			containerY.DockMargins = new Margins(0, 0, 0, 0);

			text = new StaticText(containerY);
			text.Text = sPoint.TextY;
			text.Width = 120;
			text.Dock = DockStyle.Left;
			text.DockMargins = new Margins(0, 0, 0, 0);

			field = new TextFieldReal(containerY);
			field.Width = 60;
			field.Name = sPoint.Name;
			if ( sPoint.Integer )
			{
				this.document.Modifier.AdaptTextFieldRealScalar(field);
				field.MinValue = (decimal) sPoint.FactorMinValue;
				field.MaxValue = (decimal) sPoint.FactorMaxValue;
				field.Step = (decimal) sPoint.FactorStep;
			}
			else
			{
				field.FactorMinRange = (decimal) sPoint.FactorMinValue;
				field.FactorMaxRange = (decimal) sPoint.FactorMaxValue;
				field.FactorStep = (decimal) sPoint.FactorStep;
				this.document.Modifier.AdaptTextFieldRealDimension(field);
			}
			field.InternalValue = (decimal) sPoint.Value.Y;
			field.ValueChanged += new EventHandler(this.HandleFieldPointYChanged);
			field.TabIndex = this.tabIndex++;
			field.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			field.Dock = DockStyle.Left;
			field.DockMargins = new Margins(0, 0, 0, 0);
			this.WidgetsTableAdd(field, ".Y");

			Separator sep = new Separator(container);
			sep.Width = 1;
			sep.Height = container.Height;
			sep.Dock = DockStyle.Left;
			sep.DockMargins = new Margins(8, 0, 0, 0);

			CheckButton check = new CheckButton(container);
			check.Width = 45;
			check.Height = container.Height;
			check.Name = sPoint.Name;
			check.Text = @"<img src=""manifest:Epsitec.App.DocumentEditor.Images.Linked.icon""/>";
			check.ActiveState = sPoint.Link ? WidgetState.ActiveYes : WidgetState.ActiveNo;
			check.TabIndex = this.tabIndex++;
			check.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			check.Dock = DockStyle.Left;
			check.DockMargins = new Margins(-3, 0, 0, 0);
			check.ActiveStateChanged += new EventHandler(this.HandleCheckPointActiveStateChanged);
			ToolTip.Default.SetToolTip(check, Res.Strings.Dialog.Point.Link);
			this.WidgetsTableAdd(check, ".Link");

			if ( sPoint.Doubler )
			{
				Button button;

				Panel containerD = new Panel(container);
				containerD.Width = 33;
				containerD.TabIndex = this.tabIndex++;
				containerD.Dock = DockStyle.Left;
				containerD.DockMargins = new Margins(0, 0, 0, 0);

				Panel containerDX = new Panel(containerD);
				containerDX.Width = containerD.Width;
				containerDX.Height = 22;
				containerDX.TabIndex = this.tabIndex++;
				containerDX.Dock = DockStyle.Top;
				containerDX.DockMargins = new Margins(0, 0, 0, 0);

				button = new Button(containerDX);
				button.ButtonStyle = ButtonStyle.Icon;
				button.Width = 17;
				button.Name = sPoint.Name;
				button.Text = "\u00F72";  // /2
				button.SetClientZoom(0.8);
				button.TabIndex = this.tabIndex++;
				button.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
				button.Dock = DockStyle.Left;
				button.DockMargins = new Margins(0, 0, 2, 2);
				button.Clicked += new MessageEventHandler(HandleDoublerPointDivXClicked);
				this.WidgetsTableAdd(button, ".DoublerDivX");

				button = new Button(containerDX);
				button.ButtonStyle = ButtonStyle.Icon;
				button.Width = 17;
				button.Name = sPoint.Name;
				button.Text = "\u00D72";  // x2
				button.SetClientZoom(0.8);
				button.TabIndex = this.tabIndex++;
				button.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
				button.Dock = DockStyle.Left;
				button.DockMargins = new Margins(-1, 0, 2, 2);
				button.Clicked += new MessageEventHandler(HandleDoublerPointMulXClicked);
				this.WidgetsTableAdd(button, ".DoublerMulX");

				Panel containerDY = new Panel(containerD);
				containerDY.Width = containerD.Width;
				containerDY.Height = 22;
				containerDY.TabIndex = this.tabIndex++;
				containerDY.Dock = DockStyle.Top;
				containerDY.DockMargins = new Margins(0, 0, 0, 0);

				button = new Button(containerDY);
				button.ButtonStyle = ButtonStyle.Icon;
				button.Width = 17;
				button.Name = sPoint.Name;
				button.Text = "\u00F72";  // /2
				button.SetClientZoom(0.8);
				button.TabIndex = this.tabIndex++;
				button.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
				button.Dock = DockStyle.Left;
				button.DockMargins = new Margins(0, 0, 2, 2);
				button.Clicked += new MessageEventHandler(HandleDoublerPointDivYClicked);
				this.WidgetsTableAdd(button, ".DoublerDivY");

				button = new Button(containerDY);
				button.ButtonStyle = ButtonStyle.Icon;
				button.Width = 17;
				button.Name = sPoint.Name;
				button.Text = "\u00D72";  // x2
				button.SetClientZoom(0.8);
				button.TabIndex = this.tabIndex++;
				button.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
				button.Dock = DockStyle.Left;
				button.DockMargins = new Margins(-1, 0, 2, 2);
				button.Clicked += new MessageEventHandler(HandleDoublerPointMulYClicked);
				this.WidgetsTableAdd(button, ".DoublerMulY");
			}
		}

		private void HandleFieldPointXChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			TextFieldReal field = sender as TextFieldReal;
			if ( field == null )  return;

			Settings.Abstract settings = this.document.Settings.Get(field.Name);
			if ( settings == null )  return;
			Settings.Point sPoint = settings as Settings.Point;
			if ( sPoint == null )  return;

			Drawing.Point point = sPoint.Value;
			point.X = (double) field.InternalValue;
			if ( sPoint.Link )
			{
				point.Y = point.X;
			}
			sPoint.Value = point;
		}

		private void HandleFieldPointYChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			TextFieldReal field = sender as TextFieldReal;
			if ( field == null )  return;

			Settings.Abstract settings = this.document.Settings.Get(field.Name);
			if ( settings == null )  return;
			Settings.Point sPoint = settings as Settings.Point;
			if ( sPoint == null )  return;

			Drawing.Point point = sPoint.Value;
			point.Y = (double) field.InternalValue;
			if ( sPoint.Link )
			{
				point.X = point.Y;
			}
			sPoint.Value = point;
		}

		private void HandleCheckPointActiveStateChanged(object sender)
		{
			CheckButton check = sender as CheckButton;
			if ( check == null )  return;

			Settings.Abstract settings = this.document.Settings.Get(check.Name);
			if ( settings == null )  return;
			Settings.Point sPoint = settings as Settings.Point;
			if ( sPoint == null )  return;

			sPoint.Link = ( check.ActiveState == WidgetState.ActiveYes );
		}

		private void HandleDoublerPointDivXClicked(object sender, MessageEventArgs e)
		{
			Button button = sender as Button;
			if ( button == null )  return;

			TextFieldSlider field = this.WidgetsTableSearch(button.Name, ".X")as TextFieldSlider;
			if ( field == null )  return;
			field.Value = field.Value/2.0M;

		}

		private void HandleDoublerPointMulXClicked(object sender, MessageEventArgs e)
		{
			Button button = sender as Button;
			if ( button == null )  return;

			TextFieldSlider field = this.WidgetsTableSearch(button.Name, ".X")as TextFieldSlider;
			if ( field == null )  return;
			field.Value = field.Value*2.0M;

		}

		private void HandleDoublerPointDivYClicked(object sender, MessageEventArgs e)
		{
			Button button = sender as Button;
			if ( button == null )  return;

			TextFieldSlider field = this.WidgetsTableSearch(button.Name, ".Y")as TextFieldSlider;
			if ( field == null )  return;
			field.Value = field.Value/2.0M;

		}

		private void HandleDoublerPointMulYClicked(object sender, MessageEventArgs e)
		{
			Button button = sender as Button;
			if ( button == null )  return;

			TextFieldSlider field = this.WidgetsTableSearch(button.Name, ".Y")as TextFieldSlider;
			if ( field == null )  return;
			field.Value = field.Value*2.0M;

		}
		#endregion

		#region WidgetCombo
		// Cr�e un widget combo pour �diter un r�glage de type Integer.
		protected void CreateCombo(Widget parent, string name)
		{
			Settings.Abstract settings = this.document.Settings.Get(name);
			if ( settings == null )  return;
			Settings.Integer sInteger = settings as Settings.Integer;
			if ( sInteger == null )  return;

			Panel container = new Panel(parent);
			container.Height = 22;
			container.TabIndex = this.tabIndex++;
			container.Dock = DockStyle.Top;
			container.DockMargins = new Margins(10, 10, 0, 5);

			StaticText text = new StaticText(container);
			text.Text = sInteger.Text;
			text.Width = 120;
			text.Dock = DockStyle.Left;
			text.DockMargins = new Margins(0, 0, 0, 0);

			TextFieldCombo field = new TextFieldCombo(container);
			field.Width = 140;
			field.IsReadOnly = true;
			field.Name = sInteger.Name;
			sInteger.InitCombo(field);
			field.SelectedIndexChanged += new EventHandler(this.HandleFieldComboChanged);
			field.TabIndex = this.tabIndex++;
			field.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			field.Dock = DockStyle.Left;
			field.DockMargins = new Margins(0, 0, 0, 0);
			this.WidgetsTableAdd(field, "");
		}

		private void HandleFieldComboChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			TextFieldCombo field = sender as TextFieldCombo;
			if ( field == null )  return;

			Settings.Abstract settings = this.document.Settings.Get(field.Name);
			if ( settings == null )  return;
			Settings.Integer sInteger = settings as Settings.Integer;
			if ( sInteger == null )  return;

			sInteger.Value = sInteger.RankToType(field.SelectedIndex);
		}

		protected void UpdateCombo(string name)
		{
			Settings.Abstract settings = this.document.Settings.Get(name);
			if ( settings == null )  return;
			Settings.Integer sInteger = settings as Settings.Integer;
			if ( sInteger == null )  return;

			TextFieldCombo combo = this.WidgetsTableSearch(name, "") as TextFieldCombo;
			if ( combo == null )  return;

			sInteger.InitCombo(combo);
		}
		#endregion

		#region WidgetFilename
		// Cr�e des widgets pour choisir un nom de fichier.
		protected void CreateFilename(Widget parent, string name)
		{
			Settings.Abstract settings = this.document.Settings.Get(name);
			if ( settings == null )  return;
			Settings.String sString = settings as Settings.String;
			if ( sString == null )  return;

			Panel container = new Panel(parent);
			container.Height = 22;
			container.TabIndex = this.tabIndex++;
			container.Dock = DockStyle.Top;
			container.DockMargins = new Margins(10, 10, 0, 5);

			TextField field = new TextField(container);
			field.Name = sString.Name;
			field.Text = sString.Value;
			field.Width = 177;
			field.Dock = DockStyle.Left;
			field.DockMargins = new Margins(0, 0, 0, 0);
			field.TextChanged += new EventHandler(this.HandleFilenameTextChanged);
			this.WidgetsTableAdd(field, "");

			Button button = new Button(container);
			button.Name = sString.Name;
			button.Text = Res.Strings.Dialog.Button.Browse;
			button.Width = 80;
			button.Dock = DockStyle.Left;
			button.DockMargins = new Margins(3, 0, 0, 0);
			button.Clicked += new MessageEventHandler(HandleFilenameButtonClicked);
			this.WidgetsTableAdd(button, ".Button");
		}

		private void HandleFilenameTextChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			TextField field = sender as TextField;
			if ( field == null )  return;

			Settings.Abstract settings = this.document.Settings.Get(field.Name);
			if ( settings == null )  return;
			Settings.String sString = settings as Settings.String;
			if ( sString == null )  return;

			sString.Value = field.Text;
		}

		private void HandleFilenameButtonClicked(object sender, MessageEventArgs e)
		{
			Common.Dialogs.FileSave dialog = new Common.Dialogs.FileSave();
			dialog.FileName = this.document.Settings.PrintInfo.PrintFilename;
			dialog.Title = Res.Strings.Dialog.Print.ToFile.Title;
			dialog.Filters.Add("prn", Res.Strings.Dialog.Print.ToFile.Type, "*.prn");
			dialog.PromptForOverwriting = true;
			dialog.Owner = this.windowPrint;
			dialog.OpenDialog();
			if ( dialog.Result != Common.Dialogs.DialogResult.Accept )  return;

			this.document.Settings.PrintInfo.PrintFilename = dialog.FileName;
			this.document.Settings.PrintInfo.PrintToFile = true;
			this.UpdatePrint(false);
		}
		#endregion

		#region WidgetPaper
		// Cr�e un widget combo pour �diter le format d'une page.
		protected void CreatePaper(Widget parent)
		{
			Panel container = new Panel(parent);
			container.Height = 22;
			container.TabIndex = this.tabIndex++;
			container.Dock = DockStyle.Top;
			container.DockMargins = new Margins(10, 10, 0, 0);

			StaticText text = new StaticText(container);
			text.Text = Res.Strings.Dialog.Print.Paper.Direction;
			text.Width = 120;
			text.Dock = DockStyle.Left;
			text.DockMargins = new Margins(0, 0, 0, 0);

			RadioButton radio = new RadioButton(container);
			radio.Width = 65;
			radio.Name = "PaperFormat.Portrait";
			radio.Text = Res.Strings.Dialog.Print.Paper.Portrait;
			radio.Clicked += new MessageEventHandler(this.HandlePaperActiveStateChanged);
			radio.TabIndex = this.tabIndex++;
			radio.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			radio.Dock = DockStyle.Left;
			radio.DockMargins = new Margins(0, 0, 0, 0);
			this.WidgetsTableAdd(radio, "");
			
			radio = new RadioButton(container);
			radio.Width = 75;
			radio.Name = "PaperFormat.Landscape";
			radio.Text = Res.Strings.Dialog.Print.Paper.Landscape;
			radio.Clicked += new MessageEventHandler(this.HandlePaperActiveStateChanged);
			radio.TabIndex = this.tabIndex++;
			radio.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			radio.Dock = DockStyle.Left;
			this.WidgetsTableAdd(radio, "");

			container = new Panel(parent);
			container.Height = 22;
			container.TabIndex = this.tabIndex++;
			container.Dock = DockStyle.Top;
			container.DockMargins = new Margins(10, 10, 0, 5);

			text = new StaticText(container);
			text.Text = Res.Strings.Dialog.Print.Paper.PaperList;
			text.Width = 120;
			text.Dock = DockStyle.Left;
			text.DockMargins = new Margins(0, 0, 0, 0);

			TextFieldCombo field = new TextFieldCombo(container);
			field.Width = 140;
			field.IsReadOnly = true;
			field.Name = "PaperFormat";

			field.Items.Add(Res.Strings.Dialog.Print.Format.User);
			field.Items.Add("Diapositive");
			field.Items.Add("Lettre US");
			field.Items.Add("Legal");
			field.Items.Add("Tablo�d");
			field.Items.Add("Formulaire/Demi");
			field.Items.Add("Executive US");
			field.Items.Add("Listing");
			field.Items.Add("Affiche");
			field.Items.Add("A1");
			field.Items.Add("A2");
			field.Items.Add("A3+");
			field.Items.Add("A3");
			field.Items.Add("A4");
			field.Items.Add("A5");
			field.Items.Add("A6");
			field.Items.Add("B1 (ISO)");
			field.Items.Add("B4 (ISO)");
			field.Items.Add("B5 (ISO)");
			field.Items.Add("B4 (JIS)");
			field.Items.Add("B5 (JIS)");
			field.Items.Add("C3");
			field.Items.Add("C4");
			field.Items.Add("C5");
			field.Items.Add("C6");
			field.Items.Add("RA2");
			field.Items.Add("RA3");
			field.Items.Add("RA4");
			field.Items.Add("DL");

			field.SelectedIndexChanged += new EventHandler(this.HandleFieldPaperChanged);
			field.TabIndex = this.tabIndex++;
			field.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			field.Dock = DockStyle.Left;
			field.DockMargins = new Margins(0, 0, 0, 0);
			this.WidgetsTableAdd(field, "");

			this.UpdatePaper();
		}

		private void HandleFieldPaperChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			TextFieldCombo field = sender as TextFieldCombo;
			if ( field == null )  return;

			int sel = field.SelectedIndex;
			if ( sel == 0 )  return;  // personnalis� ?

			Settings.Abstract settings = this.document.Settings.Get("PageSize");
			if ( settings == null )  return;
			Settings.Point sPoint = settings as Settings.Point;
			if ( sPoint == null )  return;
			sPoint.Link = false;

			Size size = Dialogs.PaperRankToSize(sel);
			RadioButton radio = this.WidgetsTableSearch("PaperFormat.Landscape", "") as RadioButton;
			if ( radio != null && radio.ActiveState == WidgetState.ActiveYes )
			{
				Dialogs.SwapSize(ref size);
			}
			this.document.Size = size;
		}

		private void HandlePaperActiveStateChanged(object sender, MessageEventArgs e)
		{
			if ( this.ignoreChanged )  return;
			RadioButton radio = sender as RadioButton;
			if ( radio == null )  return;

			Size size = this.document.Size;
			if ( radio.Name == "PaperFormat.Portrait" )
			{
				if ( size.Width > size.Height )
				{
					Dialogs.SwapSize(ref size);
					this.document.Size = size;
				}
			}
			else
			{
				if ( size.Width < size.Height )
				{
					Dialogs.SwapSize(ref size);
					this.document.Size = size;
				}
			}
		}

		protected void UpdatePaper()
		{
			this.ignoreChanged = true;

			TextFieldCombo combo = this.WidgetsTableSearch("PaperFormat", "") as TextFieldCombo;
			if ( combo != null )
			{
				combo.SelectedIndex = Dialogs.PaperSizeToRank(this.document.Size);
			}

			RadioButton radio;
			bool portrait = (this.document.Size.Width <= this.document.Size.Height);

			radio = this.WidgetsTableSearch("PaperFormat.Portrait", "") as RadioButton;
			if ( radio != null )
			{
				radio.ActiveState = portrait ? WidgetState.ActiveYes : WidgetState.ActiveNo;
			}

			radio = this.WidgetsTableSearch("PaperFormat.Landscape", "") as RadioButton;
			if ( radio != null )
			{
				radio.ActiveState = !portrait ? WidgetState.ActiveYes : WidgetState.ActiveNo;
			}

			this.ignoreChanged = false;
		}

		protected static Size PaperRankToSize(int rank)
		{
			Size size = new Size(500.0, 500.0);
			switch ( rank )
			{
				case  1:  size = new Size(279.400, 186.182);  break;  // Diapositive
				case  2:  size = new Size(215.900, 279.400);  break;  // Lettre US
				case  3:  size = new Size(215.900, 355.600);  break;  // L�gal
				case  4:  size = new Size(279.400, 431.800);  break;  // Tablo�d
				case  5:  size = new Size(139.700, 215.900);  break;  // Formulaire/Demi
				case  6:  size = new Size(184.150, 266.700);  break;  // Ex�cutive US
				case  7:  size = new Size(279.400, 377.952);  break;  // Listing
				case  8:  size = new Size(457.200, 609.600);  break;  // Affiche
				case  9:  size = new Size(594.000, 841.000);  break;  // A1
				case 10:  size = new Size(420.000, 594.000);  break;  // A2
				case 11:  size = new Size(329.000, 483.000);  break;  // A3+
				case 12:  size = new Size(297.000, 420.000);  break;  // A3
				case 13:  size = new Size(210.000, 297.000);  break;  // A4
				case 14:  size = new Size(148.000, 210.000);  break;  // A5
				case 15:  size = new Size(105.000, 148.000);  break;  // A6
				case 16:  size = new Size(707.000,1000.000);  break;  // B1 (ISO)
				case 17:  size = new Size(250.000, 353.000);  break;  // B4 (ISO)
				case 18:  size = new Size(176.000, 250.000);  break;  // B5 (ISO)
				case 19:  size = new Size(257.000, 364.000);  break;  // B4 (JIS)
				case 20:  size = new Size(182.000, 257.000);  break;  // B5 (JIS)
				case 21:  size = new Size(324.000, 458.000);  break;  // C3
				case 22:  size = new Size(229.000, 324.000);  break;  // C4
				case 23:  size = new Size(162.000, 229.000);  break;  // C5
				case 24:  size = new Size(114.000, 162.000);  break;  // C6
				case 25:  size = new Size(430.000, 610.000);  break;  // RA2
				case 26:  size = new Size(305.000, 430.000);  break;  // RA3
				case 27:  size = new Size(215.000, 305.000);  break;  // RA4
				case 28:  size = new Size(220.000, 110.000);  break;  // DL
			}
			return size*10.0;
		}

		protected static int PaperSizeToRank(Size size)
		{
			for ( int rank=1 ; rank<=28 ; rank++ )
			{
				Size paper = Dialogs.PaperRankToSize(rank);
				if ( size.Width == paper.Width  && size.Height == paper.Height )  return rank;
				if ( size.Width == paper.Height && size.Height == paper.Width  )  return rank;
			}
			return 0;
		}

		protected static void SwapSize(ref Size size)
		{
			double temp = size.Width;
			size.Width = size.Height;
			size.Height = temp;
		}
		#endregion

		#region WidgetPrinter
		// Cr�e des widgets pour choisir un nom d'imprimante.
		protected void CreatePrinter(Widget parent, string name)
		{
			Settings.Abstract settings = this.document.Settings.Get(name);
			if ( settings == null )  return;
			Settings.String sString = settings as Settings.String;
			if ( sString == null )  return;

			Panel container = new Panel(parent);
			container.Height = 22;
			container.TabIndex = this.tabIndex++;
			container.Dock = DockStyle.Top;
			container.DockMargins = new Margins(10, 10, 0, 5);

			TextFieldCombo field = new TextFieldCombo(container);
			field.Name = sString.Name;
			field.IsReadOnly = true;
			field.Text = this.document.Settings.PrintInfo.PrintName;
			field.Width = 177;
			field.Dock = DockStyle.Left;
			field.DockMargins = new Margins(0, 0, 0, 0);
			field.OpeningCombo += new CancelEventHandler(this.HandlePrinterOpeningCombo);
			field.ClosedCombo += new EventHandler(this.HandlePrinterClosedCombo);
			this.WidgetsTableAdd(field, "");

			Button button = new Button(container);
			button.Name = sString.Name;
			button.Text = Res.Strings.Dialog.Button.Properties;
			button.Width = 80;
			button.Dock = DockStyle.Left;
			button.DockMargins = new Margins(3, 0, 0, 0);
			button.Clicked += new MessageEventHandler(HandlePrinterButtonClicked);
			this.WidgetsTableAdd(button, ".Button");
		}

		private void HandlePrinterOpeningCombo(object sender, CancelEventArgs e)
		{
			TextFieldCombo field = sender as TextFieldCombo;
			field.Items.Clear();

			string[] installed = Common.Printing.PrinterSettings.InstalledPrinters;
			System.Collections.ArrayList list = new System.Collections.ArrayList();
			foreach ( string name in installed )
			{
				list.Add(name);
			}
			list.Sort();

			foreach ( string name in list )
			{
				field.Items.Add(name);
			}
		}

		private void HandlePrinterClosedCombo(object sender)
		{
			TextFieldCombo field = sender as TextFieldCombo;
			this.document.Settings.PrintInfo.PrintName = field.Text;
		}

		private void HandlePrinterButtonClicked(object sender, MessageEventArgs e)
		{
			Button button = sender as Button;
			Common.Dialogs.Print dialog = this.document.PrintDialog;
			Settings.PrintInfo pi = this.document.Settings.PrintInfo;
			
			if ( dialog.Document.PrinterSettings.PrinterName != pi.PrintName )
			{
				dialog.Document.SelectPrinter(pi.PrintName);
			}
			
			dialog.Owner = button.Window;
			dialog.OpenDialog();

			if ( dialog.Result == Common.Dialogs.DialogResult.Accept )
			{
				pi.PrintName = dialog.Document.PrinterSettings.PrinterName;
				this.UpdatePrint(false);
			}
		}

		protected void UpdatePrinter(string name)
		{
			Settings.Abstract settings = this.document.Settings.Get(name);
			if ( settings == null )  return;
			Settings.String sString = settings as Settings.String;
			if ( sString == null )  return;

			Common.Dialogs.Print dialog = this.document.PrintDialog;
			TextFieldCombo field = this.WidgetsTableSearch(name, "") as TextFieldCombo;
			field.Text = this.document.Settings.PrintInfo.PrintName;
		}
		#endregion

		#region WidgetRange
		// Cr�e des widgets pour choisir les pages � imprimer.
		protected void CreateRange(Widget parent, string name)
		{
			Settings.Abstract settings = this.document.Settings.Get(name);
			if ( settings == null )  return;
			Settings.Range sRange = settings as Settings.Range;
			if ( sRange == null )  return;

			RadioButton radio;
			TextFieldReal field;

			radio = new RadioButton(parent);
			radio.Text = Res.Strings.Dialog.Print.Range.All;
			radio.Height = 20;
			radio.Width = 100;
			radio.Name = sRange.Name;
			radio.ActiveState = (sRange.PrintRange == Settings.PrintRange.All) ? WidgetState.ActiveYes : WidgetState.ActiveNo;
			radio.TabIndex = this.tabIndex++;
			radio.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			radio.Dock = DockStyle.Top;
			radio.DockMargins = new Margins(10, 10, 0, 0);
			radio.Clicked += new MessageEventHandler(this.HandleRangeRadioClicked);
			this.WidgetsTableAdd(radio, ".All");
			
			// d�but from-to
			Panel container = new Panel(parent);
			container.Height = 20;
			container.TabIndex = this.tabIndex++;
			container.Dock = DockStyle.Top;
			container.DockMargins = new Margins(10, 10, 0, 0);

			radio = new RadioButton(container);
			radio.Text = Res.Strings.Dialog.Print.Range.From;
			radio.Height = 20;
			radio.Width = 85;
			radio.Name = sRange.Name;
			radio.ActiveState = (sRange.PrintRange == Settings.PrintRange.FromTo) ? WidgetState.ActiveYes : WidgetState.ActiveNo;
			radio.TabIndex = this.tabIndex++;
			radio.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			radio.Dock = DockStyle.Left;
			radio.DockMargins = new Margins(0, 0, 0, 0);
			radio.Clicked += new MessageEventHandler(this.HandleRangeRadioClicked);
			this.WidgetsTableAdd(radio, ".FromTo");
			
			field = new TextFieldReal(container);
			field.Height = 20;
			field.Width = 50;
			field.Name = sRange.Name;
			this.document.Modifier.AdaptTextFieldRealScalar(field);
			field.MinValue = (decimal) sRange.Min;
			field.MaxValue = (decimal) sRange.Max;
			field.Step = 1M;
			field.InternalValue = (decimal) sRange.From;
			field.ValueChanged += new EventHandler(this.HandleRangeFieldChanged);
			field.TabIndex = this.tabIndex++;
			field.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			field.Dock = DockStyle.Left;
			field.DockMargins = new Margins(0, 0, 0, 0);
			this.WidgetsTableAdd(field, ".From");

			StaticText text = new StaticText(container);
			text.Text = Res.Strings.Dialog.Print.Range.To;
			text.Alignment = ContentAlignment.MiddleCenter;
			text.Height = 20;
			text.Width = 30;
			text.Dock = DockStyle.Left;
			text.DockMargins = new Margins(0, 0, 0, 0);

			field = new TextFieldReal(container);
			field.Height = 20;
			field.Width = 50;
			field.Name = sRange.Name;
			this.document.Modifier.AdaptTextFieldRealScalar(field);
			field.MinValue = (decimal) sRange.Min;
			field.MaxValue = (decimal) sRange.Max;
			field.Step = 1M;
			field.InternalValue = (decimal) sRange.To;
			field.ValueChanged += new EventHandler(this.HandleRangeFieldChanged);
			field.TabIndex = this.tabIndex++;
			field.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			field.Dock = DockStyle.Left;
			field.DockMargins = new Margins(0, 0, 0, 0);
			this.WidgetsTableAdd(field, ".To");
			// fin from-to

			radio = new RadioButton(parent);
			radio.Text = Res.Strings.Dialog.Print.Range.Current;
			radio.Height = 20;
			radio.Width = 100;
			radio.Name = sRange.Name;
			radio.ActiveState = (sRange.PrintRange == Settings.PrintRange.Current) ? WidgetState.ActiveYes : WidgetState.ActiveNo;
			radio.TabIndex = this.tabIndex++;
			radio.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			radio.Dock = DockStyle.Top;
			radio.DockMargins = new Margins(10, 10, 0, 5);
			radio.Clicked += new MessageEventHandler(this.HandleRangeRadioClicked);
			this.WidgetsTableAdd(radio, ".Current");

			this.UpdateRangeRadio(name);
		}

		private void HandleRangeRadioClicked(object sender, MessageEventArgs e)
		{
			RadioButton radio = sender as RadioButton;
			if ( radio == null )  return;

			Settings.Abstract settings = this.document.Settings.Get(radio.Name);
			if ( settings == null )  return;
			Settings.Range sRange = settings as Settings.Range;
			if ( sRange == null )  return;

			if ( radio == this.WidgetsTableSearch(radio.Name, ".All") )
			{
				sRange.PrintRange = Settings.PrintRange.All;
			}
			if ( radio == this.WidgetsTableSearch(radio.Name, ".FromTo") )
			{
				sRange.PrintRange = Settings.PrintRange.FromTo;
			}
			if ( radio == this.WidgetsTableSearch(radio.Name, ".Current") )
			{
				sRange.PrintRange = Settings.PrintRange.Current;
			}

			this.UpdateRangeRadio(radio.Name);
		}

		private void HandleRangeFieldChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			TextFieldReal field = sender as TextFieldReal;
			if ( field == null )  return;

			Settings.Abstract settings = this.document.Settings.Get(field.Name);
			if ( settings == null )  return;
			Settings.Range sRange = settings as Settings.Range;
			if ( sRange == null )  return;

			if ( field == this.WidgetsTableSearch(field.Name, ".From") )
			{
				sRange.From = (int) field.InternalValue;
			}
			if ( field == this.WidgetsTableSearch(field.Name, ".To") )
			{
				sRange.To = (int) field.InternalValue;
			}

			sRange.PrintRange = Settings.PrintRange.FromTo;
			this.UpdateRangeRadio(field.Name);
		}

		protected void UpdateRangeRadio(string name)
		{
			Settings.Abstract settings = this.document.Settings.Get(name);
			if ( settings == null )  return;
			Settings.Range sRange = settings as Settings.Range;
			if ( sRange == null )  return;

			RadioButton radio;

			radio = this.WidgetsTableSearch(name, ".All") as RadioButton;
			if ( radio != null )
			{
				radio.ActiveState = (sRange.PrintRange == Settings.PrintRange.All) ? WidgetState.ActiveYes : WidgetState.ActiveNo;
			}

			radio = this.WidgetsTableSearch(name, ".FromTo") as RadioButton;
			if ( radio != null )
			{
				radio.ActiveState = (sRange.PrintRange == Settings.PrintRange.FromTo) ? WidgetState.ActiveYes : WidgetState.ActiveNo;
			}

			radio = this.WidgetsTableSearch(name, ".Current") as RadioButton;
			if ( radio != null )
			{
				radio.ActiveState = (sRange.PrintRange == Settings.PrintRange.Current) ? WidgetState.ActiveYes : WidgetState.ActiveNo;
			}
		}

		protected void UpdateRangeField(string name)
		{
			Settings.Abstract settings = this.document.Settings.Get(name);
			if ( settings == null )  return;
			Settings.Range sRange = settings as Settings.Range;
			if ( sRange == null )  return;

			TextFieldReal field;

			field = this.WidgetsTableSearch(name, ".From") as TextFieldReal;
			if ( field != null )
			{
				this.UpdateRangeField(field, sRange, sRange.From);
			}

			field = this.WidgetsTableSearch(name, ".To") as TextFieldReal;
			if ( field != null )
			{
				this.UpdateRangeField(field, sRange, sRange.To);
			}
		}

		protected void UpdateRangeField(TextFieldReal field, Settings.Range sRange, int value)
		{
			field.MinValue = (decimal) sRange.Min;
			field.MaxValue = (decimal) sRange.Max;

			value = System.Math.Max(value, sRange.Min);
			value = System.Math.Min(value, sRange.Max);
			this.ignoreChanged = true;
			field.InternalValue = (decimal) value;
			this.ignoreChanged = false;
		}
		#endregion

		// Supprime tous les widgets de tous les dialogues.
		public void FlushAll()
		{
			if ( this.windowInfos != null )
			{
				TextFieldMulti multi = this.windowInfos.Root.FindChild("Infos") as TextFieldMulti;
				if ( multi != null )
				{
					multi.Text = "";
				}
				this.windowInfos = null;
			}

			if ( this.windowSettings != null )
			{
				Widget parent = this.windowSettings.Root.FindChild("BookDocument");
				this.DeletePage(parent, "Format");
				this.DeletePage(parent, "Grid");
				this.DeletePage(parent, "Guides");
				this.DeletePage(parent, "Move");
				this.DeletePage(parent, "Misc");
				this.windowSettings = null;
			}

			if ( this.windowPrint != null )
			{
				Widget parent = this.windowPrint.Root.FindChild("Book");
				this.DeletePage(parent, "Printer");
				this.DeletePage(parent, "Param");
				this.DeletePage(parent, "Publisher");
				this.windowPrint = null;
			}

			if ( this.windowExport != null )
			{
				Widget parent = this.windowExport.Root.FindChild("Panel");
				this.DeleteContainer(parent, "Container");
				this.windowExport = null;
			}

			this.widgetsTable.Clear();
		}

		protected void DeletePage(Widget parent, string name)
		{
			Widget container = parent.FindChild(name);
			if ( container != null )
			{
				Widget page = container.FindChild("Container");
				if ( page != null )
				{
					page.Dispose();
				}
			}
		}

		protected void DeleteContainer(Widget parent, string name)
		{
			Widget container = parent.FindChild(name);
			if ( container != null )
			{
				container.Dispose();
			}
		}

		// Met � jour tous les widgets d'un dialogue.
		protected void UpdateDialogSettings(string dialog)
		{
			int total = this.document.Settings.Count;
			for ( int i=0 ; i<total ; i++ )
			{
				Settings.Abstract setting = this.document.Settings.Get(i);
				if ( dialog != this.document.Settings.GetOwnerDialog(setting.Name) )  continue;

				if ( setting is Settings.Bool )
				{
					Settings.Bool sBool = setting as Settings.Bool;

					CheckButton check = this.WidgetsTableSearch(setting.Name, "") as CheckButton;
					if ( check != null )
					{
						check.ActiveState = sBool.Value ? WidgetState.ActiveYes : WidgetState.ActiveNo;
					}
				}

				if ( setting is Settings.Integer )
				{
					Settings.Integer sInteger = setting as Settings.Integer;

					TextFieldReal field = this.WidgetsTableSearch(setting.Name, "") as TextFieldReal;
					if ( field != null )
					{
						field.InternalValue = (decimal) sInteger.Value;
					}
				}

				if ( setting is Settings.Double )
				{
					Settings.Double sDouble = setting as Settings.Double;

					TextFieldReal field = this.WidgetsTableSearch(setting.Name, "") as TextFieldReal;
					if ( field != null )
					{
						field.InternalValue = (decimal) sDouble.Value;
					}
				}

				if ( setting is Settings.Point )
				{
					Settings.Point sPoint = setting as Settings.Point;
					TextFieldReal field;

					field = this.WidgetsTableSearch(setting.Name, ".X") as TextFieldReal;
					if ( field != null )
					{
						field.InternalValue = (decimal) sPoint.Value.X;
					}

					field = this.WidgetsTableSearch(setting.Name, ".Y") as TextFieldReal;
					if ( field != null )
					{
						field.InternalValue = (decimal) sPoint.Value.Y;
					}

					CheckButton check = this.WidgetsTableSearch(setting.Name, ".Link") as CheckButton;
					if ( check != null )
					{
						check.ActiveState = sPoint.Link ? WidgetState.ActiveYes : WidgetState.ActiveNo;
					}

					if ( setting.Name == "PageSize" )
					{
						this.UpdatePaper();
					}
				}

				if ( setting is Settings.String )
				{
					Settings.String sString = setting as Settings.String;

					TextField field = this.WidgetsTableSearch(setting.Name, "") as TextField;
					if ( field != null )
					{
						field.Text = sString.Value;
					}
				}

				if ( setting is Settings.Range )
				{
					Settings.Range sRange = setting as Settings.Range;

					this.UpdateRangeRadio(sRange.Name);
					this.UpdateRangeField(sRange.Name);
				}
			}

			for ( int i=0 ; i<total ; i++ )
			{
				Settings.Abstract setting = this.document.Settings.Get(i);
				if ( setting.ConditionName == "" )  continue;
				if ( dialog != this.document.Settings.GetOwnerDialog(setting.Name) )  continue;

				Settings.Bool sBool = this.document.Settings.Get(setting.ConditionName) as Settings.Bool;
				if ( sBool == null )  continue;
				this.EnableWidget(setting.Name, sBool.Value^setting.ConditionState);
			}
		}

		// Modifie l'�tat d'un widget.
		protected void EnableWidget(string name, bool enabled)
		{
			Widget widget = this.WidgetsTableSearch(name, "");
			if ( widget == null )  return;
			widget.SetEnabled(enabled);
		}

		// Ajoute un widget dans la table.
		protected void WidgetsTableAdd(Widget widget, string option)
		{
			this.widgetsTable.Add(widget.Name+option, widget);
		}

		// Cherche un widget dans la table.
		protected Widget WidgetsTableSearch(string name, string option)
		{
			return this.widgetsTable[name+option] as Widget;
		}


		protected Document						document;
		protected Window						windowInfos;
		protected Window						windowSettings;
		protected Window						windowPrint;
		protected Window						windowExport;
		protected Window						windowGlyphs;
		protected Containers.Guides				containerGuides;
		protected System.Collections.Hashtable	widgetsTable;
		protected bool							ignoreChanged = false;
		protected int							tabIndex;
	}
}
