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

		// Met à jour le dialogue des informations.
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
		// Peuple le dialogue des réglages.
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
					Dialogs.CreateTitle(container, "Dimensions d'un pictogramme");
					this.CreatePoint(container, "PageSize");
				}
				else
				{
					Dialogs.CreateTitle(container, "Dimensions d'une page");
					this.CreatePaper(container);
					this.CreatePoint(container, "PageSize");
					Dialogs.CreateSeparator(container);
					this.CreateCombo(container, "DefaultUnit");
				}

				// Onglet Grid:
				parent = book.FindChild("Grid");
				container = new Widget(parent);
				container.Name = "Container";
				container.Dock = DockStyle.Fill;

				this.tabIndex = 0;
				Dialogs.CreateTitle(container, "Grille magnétique");
				this.CreateBool(container, "GridActive");
				this.CreateBool(container, "GridShow");
				Dialogs.CreateSeparator(container);
				this.CreatePoint(container, "GridStep");
				this.CreatePoint(container, "GridSubdiv");
				this.CreatePoint(container, "GridOffset");

				// Onglet Guides:
				parent = book.FindChild("Guides");
				container = new Widget(parent);
				container.Name = "Container";
				container.Dock = DockStyle.Fill;

				this.tabIndex = 0;
				Dialogs.CreateTitle(container, "Repères magnétiques");
				this.CreateBool(container, "GuidesActive");
				this.CreateBool(container, "GuidesShow");
				this.CreateBool(container, "GuidesMouse");
				Dialogs.CreateSeparator(container);

				this.containerGuides = new Containers.Guides(this.document);
				this.containerGuides.Dock = DockStyle.Fill;
				this.containerGuides.DockMargins = new Margins(10, 10, 4, 10);
				this.containerGuides.Parent = container;

				// Onglet Print:
				parent = book.FindChild("Print");
				container = new Widget(parent);
				container.Name = "Container";
				container.Dock = DockStyle.Fill;

				this.tabIndex = 0;
				Dialogs.CreateTitle(container, "Paramètres pour l'impression");
				this.CreateBool(container, "PrintAutoLandscape");
				this.CreateBool(container, "PrintDraft");
				this.CreateBool(container, "PrintPerfectJoin");
#if DEBUG
				this.CreateBool(container, "PrintDebugArea");
#endif
				this.CreateDouble(container, "PrintDpi");
				this.CreateBool(container, "PrintAA");

				Dialogs.CreateTitle(container, "Pré-presse");
				this.CreateBool(container, "PrintAutoZoom");
				this.CreateCombo(container, "PrintCentring");
				this.CreateDouble(container, "PrintMargins");
				this.CreateDouble(container, "PrintDebord");
				this.CreateBool(container, "PrintTarget");

				// Onglet Misc:
				parent = book.FindChild("Misc");
				container = new Widget(parent);
				container.Name = "Container";
				container.Dock = DockStyle.Fill;

				this.tabIndex = 0;
				Dialogs.CreateTitle(container, "Déplacement lorsqu'un objet est dupliqué");
				this.CreatePoint(container, "DuplicateMove");
				this.CreateBool(container, "RepeatDuplicateMove");

				Dialogs.CreateTitle(container, "Déplacements avec les touches flèches");
				this.CreatePoint(container, "ArrowMove");
				this.CreateDouble(container, "ArrowMoveMul");
				this.CreateDouble(container, "ArrowMoveDiv");
			}

			this.UpdateSettings();
		}

		// Appelé lorsque les réglages ont changé.
		public void UpdateSettings()
		{
			if ( this.windowSettings == null || !this.windowSettings.IsVisible )  return;

			int total = this.document.Settings.Count;
			for ( int i=0 ; i<total ; i++ )
			{
				Settings.Abstract setting = this.document.Settings.Get(i);

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
			}

			for ( int i=0 ; i<total ; i++ )
			{
				Settings.Abstract setting = this.document.Settings.Get(i);
				if ( setting.ConditionName == "" )  continue;
				Settings.Bool sBool = this.document.Settings.Get(setting.ConditionName) as Settings.Bool;
				if ( sBool == null )  continue;
				this.EnableWidget(setting.Name, sBool.Value^setting.ConditionState);
			}
		}

		// Appelé lorsque les repères ont changé.
		public void UpdateGuides()
		{
			if ( this.containerGuides == null )  return;
			this.containerGuides.SetDirtyContent();
		}

		// Sélectionne un repère.
		public void SelectGuide(int rank)
		{
			if ( this.containerGuides == null )  return;
			this.containerGuides.SelectGuide = rank;
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

				Dialogs.CreateTitle(container, "Paramètres pour l'image à exporter");
				this.CreateDouble(container, "ImageDpi");
				this.CreateCombo(container, "ImageDepth");
				this.CreateCombo(container, "ImageCompression");
				this.CreateDouble(container, "ImageQuality");
				this.CreateDouble(container, "ImageAA");
				Dialogs.CreateSeparator(container);
			}

			this.UpdateExport();
		}

		// Appelé lorsque les réglages ont changé.
		public void UpdateExport()
		{
			if ( this.windowExport == null || !this.windowExport.IsVisible )  return;

			this.UpdateCombo("ImageDepth");
			this.UpdateCombo("ImageCompression");
			this.UpdateDouble("ImageQuality");
		}
		#endregion


		#region WidgetTitle
		// Crée un widget de titre pour un onglet.
		public static void CreateTitle(Widget parent, string labelText)
		{
			StaticText text = new StaticText(parent);
			text.Text = string.Format("<b>{0}</b>", labelText);
			text.Dock = DockStyle.Top;
			text.DockMargins = new Margins(10, 10, 8, 2);

			Separator sep = new Separator(parent);
			sep.Width = parent.Width;
			sep.Height = 1;
			sep.Dock = DockStyle.Top;
			sep.DockMargins = new Margins(0, 0, 3, 6);
		}

		// Crée un séparateur pour un onglet.
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
		// Crée des widgets pour afficher un texte fixe.
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
		// Crée un widget pour éditer un réglage de type Bool.
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
		// Crée des widgets pour éditer un réglage de type Double.
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
		// Crée des widgets pour éditer un réglage de type Point.
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
			check.Text = "Liés";
			check.ActiveState = sPoint.Link ? WidgetState.ActiveYes : WidgetState.ActiveNo;
			check.TabIndex = this.tabIndex++;
			check.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			check.Dock = DockStyle.Left;
			check.DockMargins = new Margins(-3, 0, 0, 0);
			check.ActiveStateChanged += new EventHandler(this.HandleCheckPointActiveStateChanged);
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
				button.Width = 16;
				button.Name = sPoint.Name;
				button.Text = "/2";
				button.TabIndex = this.tabIndex++;
				button.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
				button.Dock = DockStyle.Left;
				button.DockMargins = new Margins(0, 0, 2, 2);
				button.Clicked += new MessageEventHandler(HandleDoublerPointDivXClicked);
				this.WidgetsTableAdd(button, ".DoublerDivX");

				button = new Button(containerDX);
				button.ButtonStyle = ButtonStyle.Icon;
				button.Width = 16;
				button.Name = sPoint.Name;
				button.Text = "x2";
				button.TabIndex = this.tabIndex++;
				button.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
				button.Dock = DockStyle.Left;
				button.DockMargins = new Margins(1, 0, 2, 2);
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
				button.Width = 16;
				button.Name = sPoint.Name;
				button.Text = "/2";
				button.TabIndex = this.tabIndex++;
				button.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
				button.Dock = DockStyle.Left;
				button.DockMargins = new Margins(0, 0, 2, 2);
				button.Clicked += new MessageEventHandler(HandleDoublerPointDivYClicked);
				this.WidgetsTableAdd(button, ".DoublerDivY");

				button = new Button(containerDY);
				button.ButtonStyle = ButtonStyle.Icon;
				button.Width = 16;
				button.Name = sPoint.Name;
				button.Text = "x2";
				button.TabIndex = this.tabIndex++;
				button.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
				button.Dock = DockStyle.Left;
				button.DockMargins = new Margins(1, 0, 2, 2);
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
		// Crée un widget combo pour éditer un réglage de type Integer.
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

		#region WidgetPaper
		// Crée un widget combo pour éditer le format d'une page.
		protected void CreatePaper(Widget parent)
		{
			Panel container = new Panel(parent);
			container.Height = 22;
			container.TabIndex = this.tabIndex++;
			container.Dock = DockStyle.Top;
			container.DockMargins = new Margins(10, 10, 0, 0);

			StaticText text = new StaticText(container);
			text.Text = "Orientation";
			text.Width = 120;
			text.Dock = DockStyle.Left;
			text.DockMargins = new Margins(0, 0, 0, 0);

			RadioButton radio = new RadioButton(container);
			radio.Width = 70;
			radio.Name = "PaperFormat.Portrait";
			radio.Text = "Portrait";
			radio.Clicked += new MessageEventHandler(this.HandlePaperActiveStateChanged);
			radio.TabIndex = this.tabIndex++;
			radio.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			radio.Dock = DockStyle.Left;
			radio.DockMargins = new Margins(0, 0, 0, 0);
			this.WidgetsTableAdd(radio, "");
			
			radio = new RadioButton(container);
			radio.Width = 70;
			radio.Name = "PaperFormat.Landscape";
			radio.Text = "Paysage";
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
			text.Text = "Papier";
			text.Width = 120;
			text.Dock = DockStyle.Left;
			text.DockMargins = new Margins(0, 0, 0, 0);

			TextFieldCombo field = new TextFieldCombo(container);
			field.Width = 140;
			field.IsReadOnly = true;
			field.Name = "PaperFormat";

			field.Items.Add("Personnalisée");
			field.Items.Add("Diapositive");
			field.Items.Add("Lettre US");
			field.Items.Add("Légal");
			field.Items.Add("Tabloïd");
			field.Items.Add("Formulaire/Demi");
			field.Items.Add("Exécutive US");
			field.Items.Add("Listing");
			field.Items.Add("Affiche");
			field.Items.Add("A1");
			field.Items.Add("A2");
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
			if ( sel == 0 )  return;  // personnalisé ?

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
				case  3:  size = new Size(215.900, 355.600);  break;  // Légal
				case  4:  size = new Size(279.400, 431.800);  break;  // Tabloïd
				case  5:  size = new Size(139.700, 215.900);  break;  // Formulaire/Demi
				case  6:  size = new Size(184.150, 266.700);  break;  // Exécutive US
				case  7:  size = new Size(279.400, 377.952);  break;  // Listing
				case  8:  size = new Size(457.200, 609.600);  break;  // Affiche
				case  9:  size = new Size(594.000, 841.000);  break;  // A1
				case 10:  size = new Size(420.000, 594.000);  break;  // A2
				case 11:  size = new Size(297.000, 420.000);  break;  // A3
				case 12:  size = new Size(210.000, 297.000);  break;  // A4
				case 13:  size = new Size(148.000, 210.000);  break;  // A5
				case 14:  size = new Size(105.000, 148.000);  break;  // A6
				case 15:  size = new Size(707.000,1000.000);  break;  // B1 (ISO)
				case 16:  size = new Size(250.000, 353.000);  break;  // B4 (ISO)
				case 17:  size = new Size(176.000, 250.000);  break;  // B5 (ISO)
				case 18:  size = new Size(257.000, 364.000);  break;  // B4 (JIS)
				case 19:  size = new Size(182.000, 257.000);  break;  // B5 (JIS)
				case 20:  size = new Size(324.000, 458.000);  break;  // C3
				case 21:  size = new Size(229.000, 324.000);  break;  // C4
				case 22:  size = new Size(162.000, 229.000);  break;  // C5
				case 23:  size = new Size(114.000, 162.000);  break;  // C6
				case 24:  size = new Size(430.000, 610.000);  break;  // RA2
				case 25:  size = new Size(305.000, 430.000);  break;  // RA3
				case 26:  size = new Size(215.000, 305.000);  break;  // RA4
				case 27:  size = new Size(220.000, 110.000);  break;  // DL
			}
			return size*10.0;
		}

		protected static int PaperSizeToRank(Size size)
		{
			for ( int rank=1 ; rank<=27 ; rank++ )
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
				this.DeleteContainer("Format");
				this.DeleteContainer("Grid");
				this.DeleteContainer("Guides");
				this.DeleteContainer("Print");
				this.DeleteContainer("Misc");
				this.windowSettings = null;
			}

			if ( this.windowExport != null )
			{
				Panel panel = this.windowExport.Root.FindChild("Panel") as Panel;
				if ( panel != null )
				{
					Panel container = panel.FindChild("Container") as Panel;
					if ( container != null )
					{
						container.Dispose();
					}
				}
				this.windowExport = null;
			}

			this.widgetsTable.Clear();
		}

		protected void DeleteContainer(string name)
		{
			Widget parent = this.windowSettings.Root.FindChild("BookDocument");
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

		// Modifie l'état d'un widget.
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
		protected Window						windowExport;
		protected Containers.Guides				containerGuides;
		protected System.Collections.Hashtable	widgetsTable;
		protected bool							ignoreChanged = false;
		protected int							tabIndex;
	}
}
