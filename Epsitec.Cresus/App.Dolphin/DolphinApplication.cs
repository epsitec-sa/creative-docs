//	Copyright © 2003-2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;

namespace Epsitec.App.Dolphin
{
	/// <summary>
	/// Fenêtre principale de l'application.
	/// </summary>
	public class DolphinApplication : Application
	{
		static DolphinApplication()
		{
			ImageProvider.Default.EnableLongLifeCache = true;
			ImageProvider.Default.PrefillManifestIconCache();
		}

		public DolphinApplication() : this(new ResourceManagerPool("App.Dolphin"), null)
		{
			this.resourceManagerPool.DefaultPrefix = "file";
			this.resourceManagerPool.SetupDefaultRootPaths();
		}

		public DolphinApplication(ResourceManagerPool pool, string[] args)
		{
			this.resourceManagerPool = pool;

			this.memory = new Components.Memory(this);
			this.processor = new Components.TinyProcessor(this.memory);
			this.assembler = new Assembler(this.processor, this.memory);
			this.breakAddress = Misc.undefined;
			this.memory.RomInitialise(this.processor);
			this.ips = 10000;
			this.panelMode = "Bus";
			this.firstOpenSaveDialog = true;

			this.CopySampleFiles();

			if (args != null)
			{
				foreach (string arg in args)
				{
					if (arg.EndsWith(".dolphin"))  // programme.dolphin sur la ligne de commande ?
					{
						this.filename = arg;  // il faudra l'ouvrir
					}
				}
			}
		}

		public void Show(Window parentWindow)
		{
			//	Crée et montre la fenêtre de l'éditeur.
			if ( this.Window == null )
			{
				Window window = new Window();
				this.Window = window;

				window.Root.WindowStyles = WindowStyles.DefaultDocumentWindow;
				window.Icon = Bitmap.FromManifestResource("Epsitec.App.Dolphin.Images.Application.icon", typeof(DolphinApplication).Assembly);

				Point parentCenter;
				Rectangle windowBounds;

				if (parentWindow == null)
				{
					Rectangle area = ScreenInfo.GlobalArea;
					parentCenter = area.Center;
				}
				else
				{
					parentCenter = parentWindow.WindowBounds.Center;
				}

				double w = DolphinApplication.MainWidth + DolphinApplication.MainMargin*2;
				double h = DolphinApplication.MainHeight + DolphinApplication.MainMargin*2;

				windowBounds = new Rectangle(parentCenter.X-w/2, parentCenter.Y-h/2, w, h);
				windowBounds = ScreenInfo.FitIntoWorkingArea(windowBounds);

				window.WindowBounds = windowBounds;
				window.ClientSize = windowBounds.Size;
				window.Text = "Dauphin";
				window.Name = "Application";  // utilisé pour générer "QuitApplication" !
				window.PreventAutoClose = true;
				window.MakeMinimizableFixedSizeWindow();  // tout le layout est basé sur une fenêtre fixe !
				
				this.CreateLayout();

				if (!string.IsNullOrEmpty(this.filename))  // programme sur la ligne de commande à ouvrir ?
				{
					this.Open();
				}
			}

			this.Window.Show();
		}

		internal void Hide()
		{
			this.Window.Hide();
		}

		public override string ShortWindowTitle
		{
			get
			{
				return "Dauphin";
			}
		}

		public new ResourceManagerPool ResourceManagerPool
		{
			get
			{
				return this.resourceManagerPool;
			}
		}
		
		public CommandState GetCommandState(string command)
		{
			CommandContext context = this.CommandContext;
			CommandState state = context.GetCommandState (command);

			return state;
		}


		[Command(ApplicationCommands.Id.Quit)]
		[Command("QuitApplication")]
		void CommandQuitApplication(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			e.Executed = true;

			if (this.Quit())
			{
				this.Window.Quit();
			}
		}

		protected override void ExecuteQuit(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			//	Evite que cette commande ne soit exécutée par Widgets.Application,
			//	car cela provoquerait la fin du programme, quelle que soit la
			//	réponse donnée par l'utilisateur au dialogue affiché par DocumentEditor.
		}


		protected void CreateLayout()
		{
			//	Crée tous les widgets de l'application.
			this.mainPanel = new MyWidgets.MainPanel(this.Window.Root);
			this.mainPanel.DolphinApplication = this;
			this.mainPanel.Brightness = 0.7;
			this.mainPanel.DrawFullFrame = true;
			this.mainPanel.DrawScrew = true;
			this.mainPanel.MinSize = new Size(DolphinApplication.MainWidth, DolphinApplication.MainHeight);
			this.mainPanel.MaxSize = new Size(DolphinApplication.MainWidth, DolphinApplication.MainHeight);
			this.mainPanel.PreferredSize = new Size(DolphinApplication.MainWidth, DolphinApplication.MainHeight);
			this.mainPanel.Margins = new Margins(DolphinApplication.MainMargin, DolphinApplication.MainMargin, DolphinApplication.MainMargin, DolphinApplication.MainMargin);
			this.mainPanel.Padding = new Margins(14, 14, 14, 0);
			this.mainPanel.Dock = DockStyle.Fill;

			this.panelTitle = new MyWidgets.Panel(this.mainPanel);
			this.panelTitle.Brightness = 0.9;
			this.panelTitle.DrawFullFrame = true;
			this.panelTitle.PreferredHeight = 40;
			this.panelTitle.Margins = new Margins(0, 0, 0, 10);
			this.panelTitle.Dock = DockStyle.Top;

			StaticText epsitec = new StaticText(this.mainPanel);
			epsitec.Text = "© EPSITEC SA";
			epsitec.ContentAlignment = ContentAlignment.MiddleRight;
			epsitec.PreferredHeight = 13;
			epsitec.Margins = new Margins(0, 10, 0, 1);
			epsitec.Dock = DockStyle.Bottom;

			this.buttonNew = new IconButton(this.panelTitle);
			this.buttonNew.AutoFocus = false;
			this.buttonNew.IconName = Misc.Icon("New");
			this.buttonNew.Margins = new Margins(10, 0, 8, 8);
			this.buttonNew.Dock = DockStyle.Left;
			this.buttonNew.Clicked += new MessageEventHandler(this.HandleButtonNewClicked);
			ToolTip.Default.SetToolTip(this.buttonNew, "Nouveau programme");

			this.buttonOpen = new IconButton(this.panelTitle);
			this.buttonOpen.AutoFocus = false;
			this.buttonOpen.IconName = Misc.Icon("Open");
			this.buttonOpen.Margins = new Margins(0, 0, 8, 8);
			this.buttonOpen.Dock = DockStyle.Left;
			this.buttonOpen.Clicked += new MessageEventHandler(this.HandleButtonOpenClicked);
			ToolTip.Default.SetToolTip(this.buttonOpen, "Ouvre un programme binaire .dolphin");

			this.buttonSave = new IconButton(this.panelTitle);
			this.buttonSave.AutoFocus = false;
			this.buttonSave.IconName = Misc.Icon("Save");
			this.buttonSave.Margins = new Margins(0, 0, 8, 8);
			this.buttonSave.Dock = DockStyle.Left;
			this.buttonSave.Clicked += new MessageEventHandler(this.HandleButtonSaveClicked);
			ToolTip.Default.SetToolTip(this.buttonSave, "Enregistre le programme binaire .dolphin");

			StaticText title = new StaticText(this.panelTitle);
			title.Text = string.Concat("<font size=\"200%\"><b>", DolphinApplication.ApplicationTitle, "</b></font>");
			title.ContentAlignment = ContentAlignment.MiddleCenter;
			title.Margins = new Margins(0, 0, 0, 0);
			title.Dock = DockStyle.Fill;

			this.buttonAbout = new IconButton(this.panelTitle);
			this.buttonAbout.AutoFocus = false;
			this.buttonAbout.IconName = Misc.Icon("About");
			this.buttonAbout.Margins = new Margins(0, 10, 8, 8);
			this.buttonAbout.Dock = DockStyle.Right;
			this.buttonAbout.Clicked += new MessageEventHandler(this.HandleButtonAboutClicked);
			ToolTip.Default.SetToolTip(this.buttonAbout, "A propos du Simulateur de Dauphin");

			this.buttonLook = new IconButton(this.panelTitle);
			this.buttonLook.AutoFocus = false;
			this.buttonLook.IconName = Misc.Icon("Look");
			this.buttonLook.Margins = new Margins(0, 0, 8, 8);
			this.buttonLook.Dock = DockStyle.Right;
			this.buttonLook.Clicked += new MessageEventHandler(this.HandleButtonLookClicked);
			ToolTip.Default.SetToolTip(this.buttonLook, "Change l'aspect de l'interface");

#if false
			StaticText version = new StaticText(this.panelTitle);
			version.Text = string.Concat("<font size=\"80%\">", Misc.GetVersion(), "</font>");
			version.ContentAlignment = ContentAlignment.MiddleRight;
			version.Margins = new Margins(0, 5, 0, 0);
			version.Dock = DockStyle.Right;
#endif

			MyWidgets.Panel all = new MyWidgets.Panel(this.mainPanel);
			all.Dock = DockStyle.Fill;

			//	Crée les deux grandes parties gauche/droite.
			this.leftPanel = new MyWidgets.Panel(all);
			this.leftPanel.Brightness = 0.9;
			this.leftPanel.DrawFullFrame = true;
			this.leftPanel.PreferredWidth = 510;
			this.leftPanel.Padding = new Margins(0, 0, 0, 10);
			this.leftPanel.Dock = DockStyle.Left;

			this.rightPanel = new MyWidgets.Panel(all);
			this.rightPanel.Margins = new Margins(10, 0, 0, 0);
			this.rightPanel.Dock = DockStyle.Fill;

			//	Crée les 3 parties de gauche.
			this.leftHeader = new MyWidgets.Panel(this.leftPanel);
			this.leftHeader.PreferredHeight = 1;  // sera agrandi
			this.leftHeader.Margins = new Margins(0, 0, 5, 0);
			this.leftHeader.Dock = DockStyle.Top;

			this.topLeftSep = new MyWidgets.Line(this.leftPanel);
			this.topLeftSep.PreferredHeight = 1;
			this.topLeftSep.Margins = new Margins(0, 0, 5, 3);
			this.topLeftSep.Dock = DockStyle.Top;

			this.leftClock = new MyWidgets.Panel(this.leftPanel);
			this.leftClock.PreferredWidth = 50;
			this.leftClock.Margins = new Margins(10, 0, 0, 0);
			this.leftClock.Dock = DockStyle.Left;

			this.leftPanelBus = new MyWidgets.Panel(this.leftPanel);
			this.leftPanelBus.Dock = DockStyle.Fill;

			this.leftPanelDetail = new MyWidgets.Panel(this.leftPanel);
			this.leftPanelDetail.Dock = DockStyle.Fill;
			this.leftPanelDetail.Visibility = false;

			this.leftPanelCode = new MyWidgets.Panel(this.leftPanel);
			this.leftPanelCode.Dock = DockStyle.Fill;
			this.leftPanelCode.Visibility = false;

			this.leftPanelCalm = new MyWidgets.Panel(this.leftPanel);
			this.leftPanelCalm.Dock = DockStyle.Fill;
			this.leftPanelCalm.Visibility = false;

			this.leftPanelQuick = new MyWidgets.Panel(this.leftPanel);
			this.leftPanelQuick.Dock = DockStyle.Fill;
			this.leftPanelQuick.Visibility = false;

			//	Crée les 2 parties de droite.
			this.helpPanel = new MyWidgets.Panel(this.rightPanel);
			this.helpPanel.Margins = new Margins(0, 0, 0, 10);
			this.helpPanel.Dock = DockStyle.Fill;

			MyWidgets.Panel kdPanel = new MyWidgets.Panel(this.rightPanel);
			kdPanel.Brightness = 0.9;
			kdPanel.DrawFullFrame = true;
			kdPanel.DrawScrew = true;
			kdPanel.PreferredHeight = 100;  // minimum qui sera étendu
			kdPanel.Margins = new Margins(0, 0, 0, 0);
			kdPanel.Padding = new Margins(12, 0, 10, 2);
			kdPanel.Dock = DockStyle.Bottom;

			//	Crée le contenu des différentes parties.
			this.CreateOptions(this.leftHeader);
			this.CreateClockControl(this.leftClock);
			this.CreateBusPanel(this.leftPanelBus);
			this.CreateDetailPanel(this.leftPanelDetail);
			this.CreateCodePanel(this.leftPanelCode);
			this.CreateCalmPanel(this.leftPanelCalm);
			this.CreateQuickPanel(this.leftPanelQuick);
			this.CreateHelp(this.helpPanel);
			this.CreateKeyboardDisplay(kdPanel);

			this.ProcessorFeedback();
			this.UpdateSave();
			this.UpdatePanelMode();
			this.UpdateMemoryBank();
		}

		public bool Quit()
		{
			//	Appelé avant de quitter l'application.
			//	Retourne true s'il est possible de quitter.
			return this.AutoSave();
		}

		public MyWidgets.MemoryAccessor MemoryAccessor
		{
			get
			{
				return this.memoryAccessor;
			}
		}

		public MyWidgets.CodeAccessor CodeAccessor
		{
			get
			{
				return this.codeAccessor;
			}
		}

		public List<MyWidgets.Digit> DisplayDigits
		{
			get
			{
				return this.displayDigits;
			}
		}

		public MyWidgets.Display DisplayBitmap
		{
			get
			{
				return this.displayBitmap;
			}
		}


		protected void CreateOptions(MyWidgets.Panel parent)
		{
			//	Crée la partie supérieure de panneau de gauche.
			this.buttonModeBus = new MyWidgets.PushButton(parent);
			this.buttonModeBus.Text = "BUS";
			this.buttonModeBus.Name = "Bus";
			this.buttonModeBus.PreferredSize = new Size(78, 24);
			this.buttonModeBus.Margins = new Margins(60+10+1, 0, 0, 0);
			this.buttonModeBus.Dock = DockStyle.Left;
			this.buttonModeBus.Clicked += new MessageEventHandler(this.HandleButtonModeClicked);
			ToolTip.Default.SetToolTip(this.buttonModeBus, "Montre les bus d'adresse et de données");

			this.buttonModeDetail = new MyWidgets.PushButton(parent);
			this.buttonModeDetail.Text = "CPU+MEM";
			this.buttonModeDetail.Name = "Detail";
			this.buttonModeDetail.PreferredSize = new Size(78, 24);
			this.buttonModeDetail.Margins = new Margins(2, 0, 0, 0);
			this.buttonModeDetail.Dock = DockStyle.Left;
			this.buttonModeDetail.Clicked += new MessageEventHandler(this.HandleButtonModeClicked);
			ToolTip.Default.SetToolTip(this.buttonModeDetail, "Montre les registres du processeur et la mémoire");

			this.buttonModeCode = new MyWidgets.PushButton(parent);
			this.buttonModeCode.Text = "CODE";
			this.buttonModeCode.Name = "Code";
			this.buttonModeCode.PreferredSize = new Size(78, 24);
			this.buttonModeCode.Margins = new Margins(2, 0, 0, 0);
			this.buttonModeCode.Dock = DockStyle.Left;
			this.buttonModeCode.Clicked += new MessageEventHandler(this.HandleButtonModeClicked);
			ToolTip.Default.SetToolTip(this.buttonModeCode, "Montre les instructions du processeur");

			this.buttonModeCalm = new MyWidgets.PushButton(parent);
			this.buttonModeCalm.Text = "SOURCE";
			this.buttonModeCalm.Name = "Calm";
			this.buttonModeCalm.PreferredSize = new Size(78, 24);
			this.buttonModeCalm.Margins = new Margins(2, 0, 0, 0);
			this.buttonModeCalm.Dock = DockStyle.Left;
			this.buttonModeCalm.Clicked += new MessageEventHandler(this.HandleButtonModeClicked);
			ToolTip.Default.SetToolTip(this.buttonModeCalm, "Assembleur");

			this.buttonModeQuick = new MyWidgets.PushButton(parent);
			this.buttonModeQuick.Text = "TURBO";
			this.buttonModeQuick.Name = "Quick";
			this.buttonModeQuick.PreferredSize = new Size(78, 24);
			this.buttonModeQuick.Margins = new Margins(2, 0, 0, 0);
			this.buttonModeQuick.Dock = DockStyle.Left;
			this.buttonModeQuick.Clicked += new MessageEventHandler(this.HandleButtonModeClicked);
			ToolTip.Default.SetToolTip(this.buttonModeQuick, "Panneau vide (vitesse maximale)");
		}

		protected void CreateBusPanel(MyWidgets.Panel parent)
		{
			//	Crée le panneau de gauche complet avec les bus.
			MyWidgets.Panel top, bottom;
			this.CreateBitsPanel(parent, out top, out bottom, "Bus de données");

			this.dataDigits = new List<MyWidgets.Digit>();
			for (int i=0; i<Components.Memory.TotalData/4; i++)
			{
				this.CreateBitDigit(top, i, this.dataDigits);
			}

			this.dataLeds = new List<MyWidgets.Led>();
			this.dataSwitchs = new List<MyWidgets.Switch>();
			for (int i=0; i<Components.Memory.TotalData; i++)
			{
				this.CreateBitButton(bottom, i, Components.Memory.TotalData, this.dataLeds, this.dataSwitchs);
				this.dataSwitchs[i].Clicked += new MessageEventHandler(this.HandleDataSwitchClicked);
			}

			//	Panneau des adresses.
			this.CreateBitsPanel(parent, out top, out bottom, "Bus d'adresse");

			this.addressDigits = new List<MyWidgets.Digit>();
			for (int i=0; i<Components.Memory.TotalAddress/4; i++)
			{
				this.CreateBitDigit(top, i, this.addressDigits);
			}
			
			this.addressLeds = new List<MyWidgets.Led>();
			this.addressSwitchs = new List<MyWidgets.Switch>();
			for (int i=0; i<Components.Memory.TotalAddress; i++)
			{
				this.CreateBitButton(bottom, i, Components.Memory.TotalAddress, this.addressLeds, this.addressSwitchs);
				this.addressSwitchs[i].Clicked += new MessageEventHandler(this.HandleAddressSwitchClicked);
			}

			this.AddressBits = 0;
			this.DataBits = 0;
			this.UpdateButtons();
		}

		protected void CreateCodePanel(MyWidgets.Panel parent)
		{
			//	Crée le panneau de gauche pour le code.
			MyWidgets.Panel header;
			MyWidgets.Panel codePanel = this.CreatePanelWithTitle(parent, "Codage des instructions", out header);
			codePanel.PreferredHeight = 47+19*17+1;  // place pour 17 instructions
			codePanel.Dock = DockStyle.Bottom;

			this.codeButtonPC = new MyWidgets.PushButton(header);
			this.codeButtonPC.Text = "PC";
			this.codeButtonPC.PreferredSize = new Size(22, 22);
			this.codeButtonPC.Margins = new Margins(10+17, 8, 0, 3);
			this.codeButtonPC.Dock = DockStyle.Left;
			this.codeButtonPC.Clicked += new MessageEventHandler(this.HandleCodeButtonPCClicked);
			ToolTip.Default.SetToolTip(this.codeButtonPC, "Montre automatiquement l'adresse pointée par le registre PC");

			this.codeButtonM = new MyWidgets.PushButton(header);
			this.codeButtonM.Text = "RAM";
			this.codeButtonM.Name = "M";
			this.codeButtonM.PreferredSize = new Size(36, 22);
			this.codeButtonM.Margins = new Margins(0, 2, 0, 3);
			this.codeButtonM.Dock = DockStyle.Left;
			this.codeButtonM.Clicked += new MessageEventHandler(this.HandleCodeButtonClicked);
			ToolTip.Default.SetToolTip(this.codeButtonM, "Montre le début de la mémoire vive (RAM)");

			this.codeButtonR = new MyWidgets.PushButton(header);
			this.codeButtonR.Text = "ROM";
			this.codeButtonR.Name = "R";
			this.codeButtonR.PreferredSize = new Size(36, 22);
			this.codeButtonR.Margins = new Margins(0, 2, 0, 3);
			this.codeButtonR.Dock = DockStyle.Left;
			this.codeButtonR.Clicked += new MessageEventHandler(this.HandleCodeButtonClicked);
			ToolTip.Default.SetToolTip(this.codeButtonR, "Montre le début de la mémoire morte (ROM)");

			this.codeButtonSub = new MyWidgets.PushButton(header);
			this.codeButtonSub.Text = "−";
			this.codeButtonSub.Name = "SUB";
			this.codeButtonSub.Enable = false;
			this.codeButtonSub.AutoFocus = false;
			this.codeButtonSub.PreferredSize = new Size(22, 22);
			this.codeButtonSub.Margins = new Margins(0, 15, 0, 3);
			this.codeButtonSub.Dock = DockStyle.Right;
			this.codeButtonSub.Clicked += new MessageEventHandler(this.HandleCodeSubButtonClicked);
			ToolTip.Default.SetToolTip(this.codeButtonSub, "Supprime l'instruction sélectionnée");

			this.codeButtonAdd = new MyWidgets.PushButton(header);
			this.codeButtonAdd.Text = "+";
			this.codeButtonAdd.Name = "ADD";
			this.codeButtonAdd.Enable = false;
			this.codeButtonAdd.AutoFocus = false;
			this.codeButtonAdd.PreferredSize = new Size(22, 22);
			this.codeButtonAdd.Margins = new Margins(0, 2, 0, 3);
			this.codeButtonAdd.Dock = DockStyle.Right;
			this.codeButtonAdd.Clicked += new MessageEventHandler(this.HandleCodeAddButtonClicked);
			ToolTip.Default.SetToolTip(this.codeButtonAdd, "Ajoute un NOP avant l'instruction sélectionnée");

			this.codeAccessor = new MyWidgets.CodeAccessor(codePanel);
			this.codeAccessor.Processor = this.processor;
			this.codeAccessor.Memory = this.memory;
			this.codeAccessor.Bank = "M";
			this.codeAccessor.Margins = new Margins(10, 10, 0, 0);
			this.codeAccessor.Dock = DockStyle.Fill;
			this.codeAccessor.InstructionSelected += new EventHandler(this.HandleCodeAccessorInstructionSelected);
			this.codeAccessor.BankChanged += new EventHandler(this.HandleCodeAccessorBankChanged);

			//	Partie pour le processeur.
			MyWidgets.Panel processorPanel = this.CreatePanelWithTitle(parent, "Registres du microprocesseur", out header);
			processorPanel.PreferredHeight = 10;  // minuscule (sera étendu)
			processorPanel.Dock = DockStyle.Bottom;

			this.codeRegisters = new List<TextField>();
			bool first = true;
			foreach (string name in this.processor.RegisterNames)
			{
				StaticText label = new StaticText(processorPanel);
				label.Text = name;
				label.PreferredWidth = 20;
				label.ContentAlignment = ContentAlignment.MiddleRight;
				label.Margins = new Margins(first?10:0, 4, 0, 0);
				label.Dock = DockStyle.Left;

				int bitCount = this.processor.GetRegisterSize(name);

				TextField field = new TextField(processorPanel);
				field.Name = name;
				field.PreferredWidth = MyWidgets.TextFieldHexa.GetHexaWidth(bitCount);
				field.MaxChar = (bitCount+3)/4;
				field.Margins = new Margins(0, 2, 0, 0);
				field.Dock = DockStyle.Left;
				field.TextChanged += new EventHandler(this.HandleProcessorRegisterChanged);

				this.codeRegisters.Add(field);
				first = false;
			}
		}

		protected void CreateCalmPanel(MyWidgets.Panel parent)
		{
			//	Crée le panneau de gauche pour l'assembleur CALM.
			MyWidgets.Panel header;
			this.calmPanel = this.CreatePanelWithTitle(parent, "Source", out header);
			this.calmPanel.DrawScrew = false;
			this.calmPanel.Padding = new Margins(0, 0, 4, 5);
			this.calmPanel.PreferredHeight = DolphinApplication.PanelHeight;
			this.calmPanel.Dock = DockStyle.Bottom;

			this.calmButtonOpen = new IconButton(header);
			this.calmButtonOpen.AutoFocus = false;
			this.calmButtonOpen.IconName = Misc.Icon("OpenCalm");
			this.calmButtonOpen.Margins = new Margins(5, 0, 0, 1);
			this.calmButtonOpen.Dock = DockStyle.Left;
			this.calmButtonOpen.Clicked += new MessageEventHandler(this.HandleCalmOpenClicked);
			ToolTip.Default.SetToolTip(this.calmButtonOpen, "Importe un programme source .txt");

			this.calmButtonSave = new IconButton(header);
			this.calmButtonSave.AutoFocus = false;
			this.calmButtonSave.IconName = Misc.Icon("SaveCalm");
			this.calmButtonSave.Margins = new Margins(0, 0, 0, 1);
			this.calmButtonSave.Dock = DockStyle.Left;
			this.calmButtonSave.Clicked += new MessageEventHandler(this.HandleCalmSaveClicked);
			ToolTip.Default.SetToolTip(this.calmButtonSave, "Exporte le programme source .txt");

			this.calmButtonShow = new IconButton(header);
			this.calmButtonShow.ButtonStyle = ButtonStyle.ActivableIcon;
			this.calmButtonShow.AutoFocus = false;
			this.calmButtonShow.IconName = Misc.Icon("TextShowControlCharacters");
			this.calmButtonShow.Margins = new Margins(5, 0, 0, 1);
			this.calmButtonShow.Dock = DockStyle.Left;
			this.calmButtonShow.Clicked += new MessageEventHandler(this.HandleCalmShowClicked);
			ToolTip.Default.SetToolTip(this.calmButtonShow, "Montre les caractères spéciaux");

			this.calmButtonBig = new IconButton(header);
			this.calmButtonBig.ButtonStyle = ButtonStyle.ActivableIcon;
			this.calmButtonBig.AutoFocus = false;
			this.calmButtonBig.IconName = Misc.Icon("FontBig");
			this.calmButtonBig.Margins = new Margins(0, 0, 0, 1);
			this.calmButtonBig.Dock = DockStyle.Left;
			this.calmButtonBig.Clicked += new MessageEventHandler(this.HandleCalmBigClicked);
			ToolTip.Default.SetToolTip(this.calmButtonBig, "Grande police");

			this.calmButtonFull = new IconButton(header);
			this.calmButtonFull.ButtonStyle = ButtonStyle.ActivableIcon;
			this.calmButtonFull.AutoFocus = false;
			this.calmButtonFull.IconName = Misc.Icon("FullScreen");
			this.calmButtonFull.Margins = new Margins(0, 0, 0, 1);
			this.calmButtonFull.Dock = DockStyle.Left;
			this.calmButtonFull.Clicked += new MessageEventHandler(this.HandleCalmFullClicked);
			ToolTip.Default.SetToolTip(this.calmButtonFull, "Plein écran");

			this.calmButtonAss = new MyWidgets.PushButton(header);
			this.calmButtonAss.Text = "ASSEMBLER";
			this.calmButtonAss.AutoFocus = false;
			this.calmButtonAss.PreferredSize = new Size(80, 22);
			this.calmButtonAss.Margins = new Margins(0, 5, 0, 3);
			this.calmButtonAss.Dock = DockStyle.Right;
			this.calmButtonAss.Clicked += new MessageEventHandler(this.HandleCalmAssemblerClicked);
			ToolTip.Default.SetToolTip(this.calmButtonAss, "Assemble le programme");

			this.calmButtonErr = new MyWidgets.PushButton(header);
			this.calmButtonErr.Text = "ERR";
			this.calmButtonErr.AutoFocus = false;
			this.calmButtonErr.PreferredSize = new Size(36, 22);
			this.calmButtonErr.Margins = new Margins(0, 1, 0, 3);
			this.calmButtonErr.Dock = DockStyle.Right;
			this.calmButtonErr.Clicked += new MessageEventHandler(this.HandleCalmErrorClicked);
			ToolTip.Default.SetToolTip(this.calmButtonErr, "Cherche l'erreur suivante");

			this.calmEditor = new TextFieldMulti(this.calmPanel);
			this.calmEditor.MaxChar = 100000;
			//?this.calmEditor.TextLayout.DefaultFont = Font.GetFont("Courier New", "Regular");
			this.calmEditor.TextNavigator.AllowTabInsertion = true;
			this.calmEditor.Margins = new Margins(5, 5, 0, 0);
			this.calmEditor.Dock = DockStyle.Fill;
			this.calmEditor.TextChanged += new EventHandler(this.HandleCalmEditorTextChanged);

			this.UpdateCalmEditor();
		}

		protected void CreateQuickPanel(MyWidgets.Panel parent)
		{
			//	Crée le panneau de gauche vide, pour le mode rapide.
			MyWidgets.Panel panel = this.CreatePanelWithTitle(parent, "Turbo");
			panel.PreferredHeight = DolphinApplication.PanelHeight;
			panel.Dock = DockStyle.Bottom;

			StaticText label = new StaticText(panel);
			label.Text = "<i>Vide, pour permettre une exécution à vitesse maximale</i>";
			label.ContentAlignment = ContentAlignment.MiddleCenter;
			label.Dock = DockStyle.Fill;
		}

		protected void CreateDetailPanel(MyWidgets.Panel parent)
		{
			//	Crée le panneau de gauche détaillé complet.
			MyWidgets.Panel header;
			MyWidgets.Panel memoryPanel = this.CreatePanelWithTitle(parent, "Mémoire", out header);
			memoryPanel.PreferredHeight = 47+21*10;  // place pour 10 adresses
			memoryPanel.Dock = DockStyle.Bottom;

			this.memoryButtonPC = new MyWidgets.PushButton(header);
			this.memoryButtonPC.Text = "PC";
			this.memoryButtonPC.PreferredSize = new Size(22, 22);
			this.memoryButtonPC.Margins = new Margins(10+17, 8, 0, 3);
			this.memoryButtonPC.Dock = DockStyle.Left;
			this.memoryButtonPC.Clicked += new MessageEventHandler(this.HandleMemoryButtonPCClicked);
			ToolTip.Default.SetToolTip(this.memoryButtonPC, "Montre automatiquement l'adresse pointée par le registre PC");

			this.memoryButtonM = new MyWidgets.PushButton(header);
			this.memoryButtonM.Text = "RAM";
			this.memoryButtonM.Name = "M";
			this.memoryButtonM.PreferredSize = new Size(36, 22);
			this.memoryButtonM.Margins = new Margins(0, 2, 0, 3);
			this.memoryButtonM.Dock = DockStyle.Left;
			this.memoryButtonM.Clicked += new MessageEventHandler(this.HandleMemoryButtonClicked);
			ToolTip.Default.SetToolTip(this.memoryButtonM, "Montre le début de la mémoire vive (RAM)");

			this.memoryButtonR = new MyWidgets.PushButton(header);
			this.memoryButtonR.Text = "ROM";
			this.memoryButtonR.Name = "R";
			this.memoryButtonR.PreferredSize = new Size(36, 22);
			this.memoryButtonR.Margins = new Margins(0, 2, 0, 3);
			this.memoryButtonR.Dock = DockStyle.Left;
			this.memoryButtonR.Clicked += new MessageEventHandler(this.HandleMemoryButtonClicked);
			ToolTip.Default.SetToolTip(this.memoryButtonR, "Montre le début de la mémoire morte (ROM)");

			this.memoryButtonP = new MyWidgets.PushButton(header);
			this.memoryButtonP.Text = "PER";
			this.memoryButtonP.Name = "P";
			this.memoryButtonP.PreferredSize = new Size(36, 22);
			this.memoryButtonP.Margins = new Margins(0, 2, 0, 3);
			this.memoryButtonP.Dock = DockStyle.Left;
			this.memoryButtonP.Clicked += new MessageEventHandler(this.HandleMemoryButtonClicked);
			ToolTip.Default.SetToolTip(this.memoryButtonP, "Montre le début de la zone des périphériques");

			this.memoryButtonD = new MyWidgets.PushButton(header);
			this.memoryButtonD.Text = "DIS";
			this.memoryButtonD.Name = "D";
			this.memoryButtonD.PreferredSize = new Size(36, 22);
			this.memoryButtonD.Margins = new Margins(0, 2, 0, 3);
			this.memoryButtonD.Dock = DockStyle.Left;
			this.memoryButtonD.Clicked += new MessageEventHandler(this.HandleMemoryButtonClicked);
			ToolTip.Default.SetToolTip(this.memoryButtonD, "Montre le début de la zone de l'écran bitmap");

			this.memoryAccessor = new MyWidgets.MemoryAccessor(memoryPanel);
			this.memoryAccessor.Memory = this.memory;
			this.memoryAccessor.Bank = "M";
			this.memoryAccessor.Margins = new Margins(10, 10, 0, 0);
			this.memoryAccessor.Dock = DockStyle.Fill;

			//	Partie pour le processeur.
			MyWidgets.Panel processorPanel = this.CreatePanelWithTitle(parent, "Registres du microprocesseur", out header);
			processorPanel.PreferredHeight = 10;  // minuscule (sera étendu)
			processorPanel.Dock = DockStyle.Bottom;

			this.buttonReset = new MyWidgets.PushButton(header);
			this.buttonReset.Text = "RESET";
			this.buttonReset.PreferredSize = new Size(40, 22);
			this.buttonReset.Margins = new Margins(10+17, 2, 0, 3);
			this.buttonReset.Dock = DockStyle.Left;
			this.buttonReset.Clicked += new MessageEventHandler(this.HandleButtonResetClicked);
			ToolTip.Default.SetToolTip(this.buttonReset, "Remise à zéro du processeur");

			this.registerFields = new List<MyWidgets.TextFieldHexa>();
			int index = 100;
			foreach (string name in this.processor.RegisterNames)
			{
				MyWidgets.TextFieldHexa field = this.CreateProcessorRegister(processorPanel, name);
				field.Name = name;
				field.BitNames = this.processor.GetRegisterBitNames(name);
				field.SetTabIndex(index++);
				field.Margins = new Margins(10+17, 0, 0, 1);  // laisse la largeur d'un Scroller
				field.Dock = DockStyle.Top;
				field.HexaValueChanged += new EventHandler(this.HandleProcessorHexaValueChanged);

				this.registerFields.Add(field);
			}
		}

		protected MyWidgets.TextFieldHexa CreateProcessorRegister(MyWidgets.Panel parent, string name)
		{
			//	Crée le widget complexe pour représenter un registre du processeur.
			MyWidgets.TextFieldHexa field = new MyWidgets.TextFieldHexa(parent);

			field.BitCount = this.processor.GetRegisterSize(name);
			field.Label = name;
			field.PreferredHeight = 20;
			field.Margins = new Margins(0, 0, 0, 1);

			return field;
		}

		protected void CreateClockControl(MyWidgets.Panel parent)
		{
			//	Crée les widgets pour le contrôle de l'horloge du processeur (bouton R/S, etc.).
			this.buttonRun = new MyWidgets.PushButton(parent);
			//?this.buttonRun.Text = "<font size=\"200%\"><b>R/S</b></font>";
			//?this.buttonRun.Text = "<font size=\"150%\"><b>RUN</b></font>";
			this.buttonRun.PreferredSize = new Size(50, 50);
			this.buttonRun.Margins = new Margins(0, 0, 10, 0);
			this.buttonRun.Dock = DockStyle.Top;
			this.buttonRun.Clicked += new MessageEventHandler(this.HandleButtonRunClicked);
			ToolTip.Default.SetToolTip(this.buttonRun, "Run/Stop");

			this.buttonClock6 = new MyWidgets.PushButton(parent);
			this.buttonClock6.Index = 1000000;
			this.buttonClock6.Text = "1M";
			this.buttonClock6.PreferredSize = new Size(50, 20);
			this.buttonClock6.Margins = new Margins(0, 0, 8, 0);
			this.buttonClock6.Dock = DockStyle.Top;
			this.buttonClock6.Clicked += new MessageEventHandler(this.HandleButtonClockClicked);
			ToolTip.Default.SetToolTip(this.buttonClock6, "1'000'000 instructions/seconde");

			this.buttonClock5 = new MyWidgets.PushButton(parent);
			this.buttonClock5.Index = 100000;
			this.buttonClock5.Text = "100'000";
			this.buttonClock5.PreferredSize = new Size(50, 20);
			this.buttonClock5.Margins = new Margins(0, 0, 1, 0);
			this.buttonClock5.Dock = DockStyle.Top;
			this.buttonClock5.Clicked += new MessageEventHandler(this.HandleButtonClockClicked);
			ToolTip.Default.SetToolTip(this.buttonClock5, "100'000 instructions/seconde");

			this.buttonClock4 = new MyWidgets.PushButton(parent);
			this.buttonClock4.Index = 10000;
			this.buttonClock4.Text = "10'000";
			this.buttonClock4.PreferredSize = new Size(50, 20);
			this.buttonClock4.Margins = new Margins(0, 0, 1, 0);
			this.buttonClock4.Dock = DockStyle.Top;
			this.buttonClock4.Clicked += new MessageEventHandler(this.HandleButtonClockClicked);
			ToolTip.Default.SetToolTip(this.buttonClock4, "10'000 instructions/seconde");

			this.buttonClock3 = new MyWidgets.PushButton(parent);
			this.buttonClock3.Index = 1000;
			this.buttonClock3.Text = "1'000";
			this.buttonClock3.PreferredSize = new Size(50, 20);
			this.buttonClock3.Margins = new Margins(0, 0, 1, 0);
			this.buttonClock3.Dock = DockStyle.Top;
			this.buttonClock3.Clicked += new MessageEventHandler(this.HandleButtonClockClicked);
			ToolTip.Default.SetToolTip(this.buttonClock3, "1'000 instructions/seconde");

			this.buttonClock2 = new MyWidgets.PushButton(parent);
			this.buttonClock2.Index = 100;
			this.buttonClock2.Text = "100";
			this.buttonClock2.PreferredSize = new Size(50, 20);
			this.buttonClock2.Margins = new Margins(0, 0, 1, 0);
			this.buttonClock2.Dock = DockStyle.Top;
			this.buttonClock2.Clicked += new MessageEventHandler(this.HandleButtonClockClicked);
			ToolTip.Default.SetToolTip(this.buttonClock2, "100 instructions/seconde");

			this.buttonClock1 = new MyWidgets.PushButton(parent);
			this.buttonClock1.Index = 10;
			this.buttonClock1.Text = "10";
			this.buttonClock1.PreferredSize = new Size(50, 20);
			this.buttonClock1.Margins = new Margins(0, 0, 1, 0);
			this.buttonClock1.Dock = DockStyle.Top;
			this.buttonClock1.Clicked += new MessageEventHandler(this.HandleButtonClockClicked);
			ToolTip.Default.SetToolTip(this.buttonClock1, "10 instructions/seconde");

			this.buttonClock0 = new MyWidgets.PushButton(parent);
			this.buttonClock0.Index = 1;
			this.buttonClock0.Text = "1";
			this.buttonClock0.PreferredSize = new Size(50, 20);
			this.buttonClock0.Margins = new Margins(0, 0, 1, 0);
			this.buttonClock0.Dock = DockStyle.Top;
			this.buttonClock0.Clicked += new MessageEventHandler(this.HandleButtonClockClicked);
			ToolTip.Default.SetToolTip(this.buttonClock0, "1 instruction/seconde");

			MyWidgets.Panel stepLabels = this.CreateSwitchHorizonalLabels(parent, "CONT", "STEP");
			stepLabels.Margins = new Margins(0, 0, 5, 0);
			stepLabels.Dock = DockStyle.Top;

			this.switchStep = new MyWidgets.Switch(parent);
			this.switchStep.PreferredSize = new Size(50, 20);
			this.switchStep.Margins = new Margins(0, 0, 0, 5);
			this.switchStep.Dock = DockStyle.Top;
			this.switchStep.Clicked += new MessageEventHandler(this.HandleSwitchStepClicked);
			ToolTip.Default.SetToolTip(this.switchStep, "Mode \"Continuous\" ou \"Step\"");

			this.buttonStep = new MyWidgets.PushButton(parent);
			this.buttonStep.Text = "<font size=\"200%\"><b>S</b></font>";
			this.buttonStep.PreferredSize = new Size(50, 50);
			this.buttonStep.Margins = new Margins(0, 0, 0, 0);
			this.buttonStep.Dock = DockStyle.Top;
			this.buttonStep.Clicked += new MessageEventHandler(this.HandleButtonStepClicked);
			ToolTip.Default.SetToolTip(this.buttonStep, "Avance d'un pas (step)");

			MyWidgets.Panel intoLabels = this.CreateSwitchHorizonalLabels(parent, "OVER", "INTO");
			intoLabels.Margins = new Margins(0, 0, 5, 0);
			intoLabels.Dock = DockStyle.Top;

			this.switchInto = new MyWidgets.Switch(parent);
			this.switchInto.PreferredSize = new Size(50, 20);
			this.switchInto.Margins = new Margins(0, 0, 0, 5);
			this.switchInto.Dock = DockStyle.Top;
			this.switchInto.Clicked += new MessageEventHandler(this.HandleSwitchIntoClicked);
			ToolTip.Default.SetToolTip(this.switchInto, "Mode \"Step Over\" ou \"Step Into\" lors d'un CALL");

			//	Partie inférieure gauche pour le contrôle des bus.
			this.clockBusPanel = new MyWidgets.Panel(parent);
			this.clockBusPanel.PreferredWidth = 40;
			this.clockBusPanel.Dock = DockStyle.Bottom;

			this.buttonMemoryRead = new MyWidgets.PushButton(this.clockBusPanel);
			this.buttonMemoryRead.Text = "<font size=\"200%\"><b>R</b></font>";
			this.buttonMemoryRead.PreferredSize = new Size(50, 50);
			this.buttonMemoryRead.Margins = new Margins(0, 0, 0, 2);
			this.buttonMemoryRead.Dock = DockStyle.Top;
			this.buttonMemoryRead.Pressed += new MessageEventHandler(this.HandleButtonMemoryPressed);
			this.buttonMemoryRead.Released += new MessageEventHandler(this.HandleButtonMemoryReleased);
			ToolTip.Default.SetToolTip(this.buttonMemoryRead, "Lecture en mémoire (read)");

			this.buttonMemoryWrite = new MyWidgets.PushButton(this.clockBusPanel);
			this.buttonMemoryWrite.Text = "<font size=\"200%\"><b>W</b></font>";
			this.buttonMemoryWrite.PreferredSize = new Size(50, 50);
			this.buttonMemoryWrite.Margins = new Margins(0, 0, 0, 0);
			this.buttonMemoryWrite.Dock = DockStyle.Top;
			this.buttonMemoryWrite.Pressed += new MessageEventHandler(this.HandleButtonMemoryPressed);
			this.buttonMemoryWrite.Released += new MessageEventHandler(this.HandleButtonMemoryReleased);
			ToolTip.Default.SetToolTip(this.buttonMemoryWrite, "Ecriture en mémoire (write)");

			this.UpdateClockButtons();
		}

		protected MyWidgets.Led CreateLabeledLed(MyWidgets.Panel parent, string text)
		{
			//	Crée une led dans un gros panneau avec un texte explicatif.
			MyWidgets.Panel panel = new MyWidgets.Panel(parent);

			panel.Brightness = 0.8;
			panel.DrawFullFrame = true;
			panel.PreferredWidth = parent.PreferredWidth;
			panel.PreferredHeight = 60;
			panel.Padding = new Margins(0, 0, 10, 5);
			panel.Dock = DockStyle.Top;

			MyWidgets.Led led = new MyWidgets.Led(panel);
			led.PreferredWidth = parent.PreferredWidth;
			led.Dock = DockStyle.Fill;

			StaticText label = new StaticText(panel);
			label.Text = text;
			label.ContentAlignment = ContentAlignment.MiddleCenter;
			label.PreferredWidth = parent.PreferredWidth;
			label.PreferredHeight = 16;
			label.Dock = DockStyle.Bottom;

			return led;
		}

		protected void CreateBitsPanel(MyWidgets.Panel parent, out MyWidgets.Panel top, out MyWidgets.Panel bottom, string title)
		{
			//	Crée un panneau recevant des boutons (led + switch) pour des bits.
			MyWidgets.Panel panel = this.CreatePanelWithTitle(parent, title);
			panel.Dock = DockStyle.Bottom;
			
			top = new MyWidgets.Panel(panel);
			top.PreferredHeight = 50;
			top.Padding = new Margins(0, 20, 0, 5);
			top.Dock = DockStyle.Top;
			
			bottom = new MyWidgets.Panel(panel);
			bottom.Dock = DockStyle.Fill;

			this.CreateSwitchVerticalLabels(bottom, "<b>0</b>", "<b>1</b>");
		}

		protected MyWidgets.Panel CreatePanelWithTitle(MyWidgets.Panel parent, string title)
		{
			MyWidgets.Panel header;
			return this.CreatePanelWithTitle(parent, title, out header);
		}

		protected MyWidgets.Panel CreatePanelWithTitle(MyWidgets.Panel parent, string title, out MyWidgets.Panel header)
		{
			//	Crée un panneau avec un titre en haut.
			MyWidgets.Panel panel = new MyWidgets.Panel(parent);
			panel.MinWidth = DolphinApplication.PanelWidth;
			panel.MaxWidth = DolphinApplication.PanelWidth;
			panel.Brightness = 0.8;
			panel.DrawFullFrame = true;
			panel.DrawScrew = true;
			panel.PreferredHeight = 195;
			panel.Padding = new Margins(0, 0, 4, 12);
			panel.Margins = new Margins(10, 10, 10, 0);

			header = new MyWidgets.Panel(panel);
			header.PreferredHeight = 25;
			header.Margins = new Margins(0, 0, 0, 0);
			header.Dock = DockStyle.Top;

			StaticText label = new StaticText(header);
			label.Text = string.Concat("<font size=\"150%\"><b>", title, "</b></font>");
			label.ContentAlignment = ContentAlignment.TopCenter;
			label.PreferredHeight = 25;
			label.Margins = new Margins(0, 0, 0, 0);
			label.Dock = DockStyle.Fill;

			MyWidgets.Line sep = new MyWidgets.Line(panel);
			sep.PreferredHeight = 1;
			sep.Margins = new Margins(0, 0, 0, 5);
			sep.Dock = DockStyle.Top;

			return panel;
		}

		protected void CreateBitDigit(MyWidgets.Panel parent, int rank, List<MyWidgets.Digit> digits)
		{
			//	Crée un digit pour un groupe de 4 bits.
			MyWidgets.Digit digit = new MyWidgets.Digit(parent);
			digit.PreferredWidth = 30;
			digit.Margins = new Margins(37+18, 37, 0, 0);
			digit.Dock = DockStyle.Right;

			digits.Add(digit);
		}

		protected void CreateBitButton(MyWidgets.Panel parent, int rank, int total, List<MyWidgets.Led> leds, List<MyWidgets.Switch> switchs)
		{
			//	Crée un bouton (led + switch) pour un bit.
			MyWidgets.Panel group = new MyWidgets.Panel(parent);
			group.PreferredSize = new Size(24, 24);
			group.Margins = new Margins(((rank+1)%4 == 0) ? 2+18/2-1:2, 0, 0, 0);
			group.Dock = DockStyle.Right;

			StaticText label = new StaticText(group);
			label.Text = rank.ToString();
			label.ContentAlignment = ContentAlignment.MiddleCenter;
			label.PreferredWidth = 24;
			label.PreferredHeight = 20;
			label.Dock = DockStyle.Top;

			MyWidgets.Led state = new MyWidgets.Led(group);
			state.Index = rank;
			state.PreferredWidth = 24;
			state.PreferredHeight = 24;
			state.Dock = DockStyle.Top;

			MyWidgets.Switch button = new MyWidgets.Switch(group);
			button.PreferredWidth = 24;
			button.Margins = new Margins(2, 2, 5, 0);
			button.Dock = DockStyle.Fill;

			if ((rank+1)%4 == 0 && rank < total-1)
			{
				MyWidgets.Line sep = new MyWidgets.Line(parent);
				sep.PreferredWidth = 1;
				sep.Margins = new Margins(18/2, 0, 0, 0);
				sep.Dock = DockStyle.Right;
			}

			leds.Add(state);
			switchs.Add(button);
		}

		protected void CreateSwitchVerticalLabels(MyWidgets.Panel parent, string no, string yes)
		{
			MyWidgets.Panel labels = new MyWidgets.Panel(parent);
			labels.PreferredWidth = 20;
			labels.Margins = new Margins(0, 10, 0, 0);
			labels.Dock = DockStyle.Right;

			StaticText label;

			label = new StaticText(labels);
			label.Text = no;
			label.ContentAlignment = ContentAlignment.MiddleCenter;
			label.PreferredWidth = 20;
			label.Margins = new Margins(0, 0, 5, 5);
			label.Dock = DockStyle.Bottom;

			label = new StaticText(labels);
			label.Text = yes;
			label.ContentAlignment = ContentAlignment.MiddleCenter;
			label.PreferredWidth = 20;
			label.Margins = new Margins(0, 0, 5, 5);
			label.Dock = DockStyle.Bottom;
		}

		protected MyWidgets.Panel CreateSwitchHorizonalLabels(MyWidgets.Panel parent, string no, string yes)
		{
			MyWidgets.Panel labels = new MyWidgets.Panel(parent);
			labels.PreferredHeight = 20;
			labels.PreferredWidth = parent.PreferredWidth;

			StaticText label;

			label = new StaticText(labels);
			label.Text = string.Concat("<font size=\"80%\">", no, "</font>");
			label.ContentAlignment = ContentAlignment.MiddleLeft;
			label.PreferredHeight = 20;
			label.PreferredWidth = parent.PreferredWidth/2;
			label.Margins = new Margins(0, 0, 0, 0);
			label.Dock = DockStyle.Left;

			label = new StaticText(labels);
			label.Text = string.Concat("<font size=\"80%\">", yes, "</font>");
			label.ContentAlignment = ContentAlignment.MiddleRight;
			label.PreferredHeight = 20;
			label.PreferredWidth = parent.PreferredWidth/2;
			label.Margins = new Margins(0, 0, 0, 0);
			label.Dock = DockStyle.Right;

			return labels;
		}


		protected void CreateHelp(MyWidgets.Panel parent)
		{
			//	Crée le panneau pour l'aide.
			this.book = new TabBook(parent);
			this.book.Arrows = TabBookArrows.Stretch;
			this.book.Dock = DockStyle.Fill;

			//	Crée l'onglet pour les commentaires sur le programme.
			this.pageProgram = new TabPage();
			this.pageProgram.TabTitle = "Comment";

			HToolBar toolbar = new HToolBar(this.pageProgram);
			toolbar.Margins = new Margins(0, 0, 0, -1);
			toolbar.Dock = DockStyle.Top;

			IconButton button;

			button = new IconButton();
			button.Name = "FontBold";
			button.IconName = Misc.Icon("FontBold");
			button.ButtonStyle = ButtonStyle.ActivableIcon;
			button.Clicked += new MessageEventHandler(this.HandleButtonStyleClicked);
			toolbar.Items.Add(button);

			button = new IconButton();
			button.Name = "FontItalic";
			button.IconName = Misc.Icon("FontItalic");
			button.ButtonStyle = ButtonStyle.ActivableIcon;
			button.Clicked += new MessageEventHandler(this.HandleButtonStyleClicked);
			toolbar.Items.Add(button);

			button = new IconButton();
			button.Name = "FontUnderline";
			button.IconName = Misc.Icon("FontUnderline");
			button.ButtonStyle = ButtonStyle.ActivableIcon;
			button.Clicked += new MessageEventHandler(this.HandleButtonStyleClicked);
			toolbar.Items.Add(button);

			this.fieldProgramRem = new TextFieldMulti(this.pageProgram);
			this.fieldProgramRem.Text = DolphinApplication.ProgramEmptyRem;
			this.fieldProgramRem.Dock = DockStyle.Fill;
			this.fieldProgramRem.TextChanged += new EventHandler(this.HandleFieldProgramRemTextChanged);

			this.book.Items.Add(this.pageProgram);

			//	Crée les onglets d'aide sur le processeur.
			List<string> chapters = this.processor.HelpChapters;
			foreach (string chapter in chapters)
			{
				TabPage page = new TabPage();
				page.TabTitle = chapter;

				string text = this.processor.HelpChapter(chapter);

				TextFieldMulti field = new TextFieldMulti(page);
				field.IsReadOnly = true;
				field.MaxChar = text.Length+10;
				field.Text = text;
				field.Dock = DockStyle.Fill;

				this.book.Items.Add(page);
			}

			this.book.ActivePage = this.pageProgram;
		}

		protected void CreateKeyboardDisplay(MyWidgets.Panel parent)
		{
			//	Crée le clavier et l'affichage simulé, dans la partie de droite.
			List<MyWidgets.Panel> lines = new List<MyWidgets.Panel>();
			for (int y=0; y<2; y++)
			{
				MyWidgets.Panel keyboard = new MyWidgets.Panel(parent);
				keyboard.PreferredHeight = 50;
				keyboard.Margins = new Margins(0, 0, 0, (y==0) ? 10:2);
				keyboard.Dock = DockStyle.Bottom;

				lines.Add(keyboard);
			}

			MyWidgets.Panel display = new MyWidgets.Panel(parent);
			display.Margins = new Margins(0, 0, 5, 10);
			display.Dock = DockStyle.Bottom;

			//	Crée les digits de l'affichage.
			this.displayDigits = new List<MyWidgets.Digit>();
			for (int i=0; i<4; i++)
			{
				MyWidgets.Digit digit = new MyWidgets.Digit(display);
				digit.Dock = DockStyle.Left;

				this.displayDigits.Add(digit);
			}

			MyWidgets.Panel key = new MyWidgets.Panel(display);
			key.PreferredWidth = 34;
			key.Margins = new Margins(0, 12, 0, 0);
			key.Dock = DockStyle.Right;

			this.displayButtonKey = new MyWidgets.PushButton(key);
			this.displayButtonKey.PreferredSize = new Size(34, 24);
			this.displayButtonKey.Dock = DockStyle.Bottom;
			this.displayButtonKey.Margins = new Margins(0, 0, 2, 0);
			this.displayButtonKey.Clicked += new MessageEventHandler(this.HandleDisplayButtonKeyClicked);
			ToolTip.Default.SetToolTip(this.displayButtonKey, "Type du clavier \"Numeric\" ou \"Arrows\"");

			MyWidgets.Panel mode = new MyWidgets.Panel(display);
			mode.PreferredWidth = 54;
			mode.Margins = new Margins(0, 6, 0, 0);
			mode.Dock = DockStyle.Right;

			this.displayButtonMode = new MyWidgets.PushButton(mode);
			this.displayButtonMode.Text = "DISPLAY";
			this.displayButtonMode.PreferredSize = new Size(54, 24);
			this.displayButtonMode.Dock = DockStyle.Bottom;
			this.displayButtonMode.Clicked += new MessageEventHandler(this.HandleDisplayButtonModeClicked);
			ToolTip.Default.SetToolTip(this.displayButtonMode, "Montre ou cache l'écran bitmap");

			MyWidgets.Panel techno = new MyWidgets.Panel(display);
			techno.PreferredWidth = 34;
			techno.Margins = new Margins(0, 1, 0, 0);
			techno.Dock = DockStyle.Right;

			this.displayButtonTechno = new MyWidgets.PushButton(techno);
			this.displayButtonTechno.PreferredSize = new Size(34, 24);
			this.displayButtonTechno.Dock = DockStyle.Bottom;
			this.displayButtonTechno.Clicked += new MessageEventHandler(this.HandleDisplayButtonTechnoClicked);
			ToolTip.Default.SetToolTip(this.displayButtonTechno, "Type de l'écran bitmap");

			MyWidgets.Panel cls = new MyWidgets.Panel(display);
			cls.PreferredWidth = 34;
			cls.Margins = new Margins(0, 1, 0, 0);
			cls.Dock = DockStyle.Right;

			this.displayButtonCls = new MyWidgets.PushButton(cls);
			this.displayButtonCls.Text = "CLS";
			this.displayButtonCls.PreferredSize = new Size(34, 24);
			this.displayButtonCls.Dock = DockStyle.Bottom;
			this.displayButtonCls.Clicked += new MessageEventHandler(this.HandleDisplayButtonClsClicked);
			ToolTip.Default.SetToolTip(this.displayButtonCls, "Efface l'écran bitmap");

			//	Crée l'affichage bitmap.
			MyWidgets.Panel bitmap = new MyWidgets.Panel(parent);
			bitmap.PreferredHeight = 0;
			bitmap.Margins = new Margins(0, 12, 0, 0);
			bitmap.Dock = DockStyle.Bottom;

			this.displayBitmap = new MyWidgets.Display(bitmap);
			this.displayBitmap.SetMemory(this.memory, Components.Memory.DisplayBase, Components.Memory.DisplayDx, Components.Memory.DisplayDy);
			this.displayBitmap.PreferredSize = new Size(258, 202);
			this.displayBitmap.Dock = DockStyle.Bottom;

			//	Crée les touches du clavier.
			this.keyboardButtons = new List<MyWidgets.PushButton>();
			MyWidgets.PushButton button;
			int t=0;
			for (int y=0; y<2; y++)
			{
				for (int x=0; x<5; x++)
				{
					int index = DolphinApplication.KeyboardIndex[t++];
					button = this.CreateKeyboardButton(index, lines[y]);
					this.keyboardButtons.Add(button);
				}
			}

			button = this.CreateKeyboardButton(100, lines[0]);
			button.Margins = new Margins((50+2)*1, 2, 0, 0);
			this.keyboardButtons.Add(button);

			button = this.CreateKeyboardButton(101, lines[0]);
			this.keyboardButtons.Add(button);

			button = this.CreateKeyboardButton(102, lines[0]);
			this.keyboardButtons.Add(button);

			button = this.CreateKeyboardButton(103, lines[1]);
			button.Margins = new Margins((50+2)*2, 2, 0, 0);
			this.keyboardButtons.Add(button);

			this.UpdateDisplayMode();
			this.UpdateKeyboard();
		}

		protected MyWidgets.PushButton CreateKeyboardButton(int index, MyWidgets.Panel parent)
		{
			MyWidgets.PushButton button = new MyWidgets.PushButton(parent);
			button.Index = index;
			button.PreferredWidth = 50;
			button.Margins = new Margins(0, 2, 0, 0);
			button.Dock = DockStyle.Left;
			button.Pressed += new MessageEventHandler(this.HandleKeyboardButtonPressed);
			button.Released += new MessageEventHandler(this.HandleKeyboardButtonReleased);

			if (index == 0x08)
			{
				button.Text = "<b>Shift</b>";
			}
			else if (index == 0x10)
			{
				button.Text = "<b>Ctrl</b>";
			}
			else if (index == 100)
			{
				button.Text = Misc.Image("ArrowLeft");
			}
			else if (index == 101)
			{
				button.Text = Misc.Image("ArrowDown");
			}
			else if (index == 102)
			{
				button.Text = Misc.Image("ArrowRight");
			}
			else if (index == 103)
			{
				button.Text = Misc.Image("ArrowUp");
			}

			return button;
		}

		protected static int[] KeyboardIndex =
		{
			0x08, 0x00, 0x01, 0x02, 0x03,  // Shift, 0..3
			0x10, 0x04, 0x05, 0x06, 0x07,  // Ctrl,  4..7
		};

		protected MyWidgets.PushButton SearchKey(int index)
		{
			//	Cherche une touche du clavier émulé.
			foreach (MyWidgets.PushButton button in this.keyboardButtons)
			{
				if (button.Index == index)
				{
					return button;
				}
			}

			return null;
		}

		public bool IsRunning
		{
			//	Retourne true si un programme est en cours d'exécution.
			get
			{
				return this.buttonRun.ActiveState == ActiveState.Yes;
			}
		}

		public void ProcessRunningMessage(Message message)
		{
			//	Gère les touches pendant qu'un programme est en cours d'exécution.
			if (message.MessageType == MessageType.KeyDown)
			{
				if (message.KeyCode == KeyCode.ArrowUp)
				{
					this.RunningKey(0x10, true);  // ctrl pressed
					this.RunningKey(103, true);  // up pressed
				}

				if (message.KeyCode == KeyCode.ArrowDown)
				{
					this.RunningKey(0x08, true);  // shift pressed
					this.RunningKey(101, true);  // down pressed
				}

				if (message.KeyCode == KeyCode.ArrowLeft)
				{
					this.RunningKey(100, true);  // left pressed
				}

				if (message.KeyCode == KeyCode.ArrowRight)
				{
					this.RunningKey(102, true);  // right pressed
				}
			}

			if (message.MessageType == MessageType.KeyUp)
			{
				if (message.KeyCode == KeyCode.ArrowUp)
				{
					this.RunningKey(0x10, false);  // ctrl released
					this.RunningKey(103, false);  // up released
				}

				if (message.KeyCode == KeyCode.ArrowDown)
				{
					this.RunningKey(0x08, false);  // shift released
					this.RunningKey(101, false);  // down released
				}

				if (message.KeyCode == KeyCode.ArrowLeft)
				{
					this.RunningKey(100, false);  // left released
				}

				if (message.KeyCode == KeyCode.ArrowRight)
				{
					this.RunningKey(102, false);  // right released
				}
			}
		}

		protected void RunningKey(int index, bool pressed)
		{
			//	Gère une touche pendant qu'un programme est en cours d'exécution.
			MyWidgets.PushButton button = this.SearchKey(index);
			ActiveState state = pressed ? ActiveState.Yes : ActiveState.No;

			if (button.ActiveState != state)
			{
				button.ActiveState = state;
				this.UpdateKeyboard();
				this.KeyboardChanged(button, pressed);
			}
		}


		protected void UpdateButtons()
		{
			//	Met à jour le mode enable/disable de tous les boutons.
			this.buttonStep.Enable = (this.buttonRun.ActiveState == ActiveState.Yes) && (this.switchStep.ActiveState == ActiveState.Yes);

			bool run = (this.buttonRun.ActiveState == ActiveState.Yes);

			if (run)
			{
				this.buttonRun.Text = "<font size=\"140%\"><b>STOP</b></font>";
			}
			else
			{
				this.buttonRun.Text = "<font size=\"140%\"><b>RUN</b></font>";
			}

			this.buttonMemoryRead.Enable = !run;
			this.buttonMemoryWrite.Enable = !run;

			foreach (MyWidgets.Switch button in this.addressSwitchs)
			{
				button.Enable = !run;
			}

			foreach (MyWidgets.Switch button in this.dataSwitchs)
			{
				button.Enable = !run;
			}
		}

		protected void UpdateClockButtons()
		{
			//	Met à jour les boutons pour la fréquence de l'horloge.
			this.buttonClock6.ActiveState = (this.ips == 1000000) ? ActiveState.Yes : ActiveState.No;
			this.buttonClock5.ActiveState = (this.ips ==  100000) ? ActiveState.Yes : ActiveState.No;
			this.buttonClock4.ActiveState = (this.ips ==   10000) ? ActiveState.Yes : ActiveState.No;
			this.buttonClock3.ActiveState = (this.ips ==    1000) ? ActiveState.Yes : ActiveState.No;
			this.buttonClock2.ActiveState = (this.ips ==     100) ? ActiveState.Yes : ActiveState.No;
			this.buttonClock1.ActiveState = (this.ips ==      10) ? ActiveState.Yes : ActiveState.No;
			this.buttonClock0.ActiveState = (this.ips ==       1) ? ActiveState.Yes : ActiveState.No;
		}

		protected void UpdateSave()
		{
			//	Met à jour le bouton d'enregistrement.
			this.buttonSave.Enable = this.dirty;
		}

		protected void UpdateFilename()
		{
			//	Met à jour le nom du programme ouvert.
			if (string.IsNullOrEmpty(this.filename))
			{
				this.Window.Text = "Dauphin";
			}
			else
			{
				this.Window.Text = string.Concat("Dauphin - ", System.IO.Path.GetFileNameWithoutExtension(this.filename));
			}
		}

		protected void UpdateMemoryBank()
		{
			//	Met à jour la banque mémoire utilisée.
			this.memoryButtonPC.ActiveState = (this.memoryAccessor.FollowPC) ? ActiveState.Yes : ActiveState.No;
			this.memoryButtonM.ActiveState = (this.memoryAccessor.Bank == "M") ? ActiveState.Yes : ActiveState.No;
			this.memoryButtonR.ActiveState = (this.memoryAccessor.Bank == "R") ? ActiveState.Yes : ActiveState.No;
			this.memoryButtonP.ActiveState = (this.memoryAccessor.Bank == "P") ? ActiveState.Yes : ActiveState.No;
			this.memoryButtonD.ActiveState = (this.memoryAccessor.Bank == "D") ? ActiveState.Yes : ActiveState.No;

			this.codeButtonPC.ActiveState = (this.codeAccessor.FollowPC) ? ActiveState.Yes : ActiveState.No;
			this.codeButtonM.ActiveState = (this.codeAccessor.Bank == "M") ? ActiveState.Yes : ActiveState.No;
			this.codeButtonR.ActiveState = (this.codeAccessor.Bank == "R") ? ActiveState.Yes : ActiveState.No;
		}

		protected void UpdatePanelMode()
		{
			//	Met à jour le panneau choisi.
			this.buttonModeBus.ActiveState    = (this.panelMode == "Bus"   ) ? ActiveState.Yes : ActiveState.No;
			this.buttonModeDetail.ActiveState = (this.panelMode == "Detail") ? ActiveState.Yes : ActiveState.No;
			this.buttonModeCode.ActiveState   = (this.panelMode == "Code"  ) ? ActiveState.Yes : ActiveState.No;
			this.buttonModeCalm.ActiveState   = (this.panelMode == "Calm"  ) ? ActiveState.Yes : ActiveState.No;
			this.buttonModeQuick.ActiveState  = (this.panelMode == "Quick" ) ? ActiveState.Yes : ActiveState.No;

			this.leftPanelBus.Visibility    = (this.panelMode == "Bus");
			this.clockBusPanel.Visibility   = (this.panelMode == "Bus");
			this.leftPanelDetail.Visibility = (this.panelMode == "Detail");
			this.leftPanelCode.Visibility   = (this.panelMode == "Code");
			this.leftPanelCalm.Visibility   = (this.panelMode == "Calm");
			this.leftPanelQuick.Visibility  = (this.panelMode == "Quick");
		}

		protected void UpdateDisplayMode()
		{
			//	Met à jour en fonction du mode [DISPLAY].
			bool bitmap = (this.displayButtonMode.ActiveState == ActiveState.Yes);
			bool lcd = (this.displayBitmap.Technology == MyWidgets.Display.Type.LCD);

			foreach (MyWidgets.Digit digit in this.displayDigits)
			{
				digit.PreferredSize = bitmap ? new Size(20, 30) : new Size(40, 60);
			}

			this.displayBitmap.Visibility = bitmap;
			this.displayBitmap.Technology = lcd ? MyWidgets.Display.Type.LCD : MyWidgets.Display.Type.CRT;

			this.displayButtonCls.Visibility = bitmap;
			this.displayButtonTechno.Visibility = bitmap;
			this.displayButtonTechno.Text = lcd ? "CRT" : "LCD";
		}

		protected void UpdateCalmButtons()
		{
			//	Met à jour les boutons de l'éditeur CALM.
			this.calmButtonShow.ActiveState = this.calmEditor.TextLayout.ShowTab ? ActiveState.Yes : ActiveState.No;
		}

		protected void UpdateCalmEditor()
		{
			//	Met à jour l'éditeur CALM après un changement de la taille de la police.
			bool big = (this.calmButtonBig.ActiveState == ActiveState.Yes);

			this.calmEditor.TextLayout.DefaultFontSize = big ? 13.0 : 10.8;

			//	Supprime toutes les tabulations.
			while (this.calmEditor.TextLayout.TabCount > 0)
			{
				this.calmEditor.TextLayout.TabRemoveAt(0);
			}

			//	Insère de nouvelles tabulations, proportionnelles à la taille de la police.
			for (int i=0; i<8; i++)
			{
				TextStyle.Tab tab = new TextStyle.Tab();
				tab.Pos = (big ? 50.0*13.0/10.8 : 50.0)*i;
				this.calmEditor.TextLayout.TabInsert(tab);
			}

			//	TODO: pour mettre à jour l'ascenseur (devrait être inutile) !
			this.ignoreChange = true;
			this.calmEditor.Cursor = 0;
			this.calmEditor.TextLayout.InsertCharacter(this.calmEditor.TextNavigator.Context, '?');
			this.calmEditor.TextLayout.DeleteCharacter(this.calmEditor.TextNavigator.Context, -1);
			this.ignoreChange = false;

			this.calmEditor.Invalidate();  // TODO: devrait être inutile
		}

		protected void UpdateKeyboard()
		{
			//	Met à jour les inscriptions sur le clavier émulé.
			bool shift = this.SearchKey(0x08).ActiveState == ActiveState.Yes;
			bool ctrl  = this.SearchKey(0x10).ActiveState == ActiveState.Yes;

			this.displayButtonKey.Text = this.keyboardArrows ? "NUM" : "ARR";

			foreach (MyWidgets.PushButton button in this.keyboardButtons)
			{
				if (button.Index >= 0 && button.Index <= 7)
				{
					int i = button.Index;

					if (shift)
					{
						i += 0x08;
					}

					if (ctrl)
					{
						i += 0x10;
					}

					if (i <= 7)
					{
						button.Text = string.Concat("<font size=\"200%\"><b>", button.Index.ToString(), "</b></font>");
					}
					else
					{
						button.Text = string.Concat("(", i.ToString("X2"), ")<br/><font size=\"190%\"><b>", button.Index.ToString(), "</b></font>");
					}
				}

				if (button.Index < 100)
				{
					button.Visibility = !this.keyboardArrows;
				}
				else
				{
					button.Visibility = this.keyboardArrows;
				}
			}
		}

		protected void UpdateData()
		{
			//	Met à jour les données, après le changement d'une valeur en mémoire.
			if (this.panelMode == "Detail")
			{
				this.memoryAccessor.UpdateData();
			}

			if (this.panelMode == "Code")
			{
				this.codeAccessor.UpdateData();
			}
		}

		protected void MarkPC(int pc, bool force)
		{
			//	Montre les données pointées par le PC.
			if (this.panelMode == "Detail")
			{
				if (force)
				{
					this.memoryAccessor.DirtyMarkPC();
				}

				this.memoryAccessor.MarkPC = pc;
			}

			if (this.panelMode == "Code")
			{
				if (force)
				{
					this.codeAccessor.DirtyMarkPC();
				}

				this.codeAccessor.MarkPC = pc;
			}
		}


		public bool IsEmptyPanel
		{
			//	Est-on en mode sans panneau d'affichage ?
			get
			{
				return this.panelMode == "Quick";
			}
		}

		protected int AddressBits
		{
			get
			{
				return this.GetBits(this.addressSwitchs);
			}
			set
			{
				this.SetBits(this.addressDigits, this.addressLeds, value);
			}
		}

		protected int DataBits
		{
			get
			{
				return this.GetBits(this.dataSwitchs);
			}
			set
			{
				this.SetBits(this.dataDigits, this.dataLeds, value);
			}
		}

		protected void SetBits(List<MyWidgets.Digit> digits, List<MyWidgets.Led> leds, int value)
		{
			//	Initialise une rangée de leds.
			if (this.IsEmptyPanel)
			{
				return;
			}

			for (int i=0; i<digits.Count; i++)
			{
				digits[i].HexValue = (value >> i*4) & 0x0f;
			}

			for (int i=0; i<leds.Count; i++)
			{
				leds[i].ActiveState = (value & (1 << i)) == 0 ? ActiveState.No : ActiveState.Yes;
			}
		}

		protected int GetBits(List<MyWidgets.Switch> switchs)
		{
			//	Retourne la valeur d'une rangée de switchs.
			int value = 0;

			for (int i=0; i<switchs.Count; i++)
			{
				if (switchs[i].ActiveState == ActiveState.Yes)
				{
					value |= (1 << i);
				}
			}

			return value;
		}


		public void KeyboardChanged(MyWidgets.PushButton button, bool pressed)
		{
			//	Appelé lorsqu'une touche du clavier simulé a été pressée ou relâchée.
			int keys = this.memory.Read(Components.Memory.PeriphKeyboard);

			if (button.Index < 0x08)
			{
				if (pressed)
				{
					keys &= ~0x07;
					keys |= button.Index;
					keys |= 0x80;  // met le bit full
				}
			}
			else  // Shift, Ctrl ou flèche ?
			{
				int index = button.Index;

				if (index == 100)  // left ?
				{
					index = 0x20;
				}
				else if (index == 101)  // down ?
				{
					index = 0x08;
				}
				else if (index == 102)  // right ?
				{
					index = 0x40;
				}
				else if (index == 103)  // up ?
				{
					index = 0x10;
				}

				if (button.ActiveState == ActiveState.Yes)
				{
					keys |= index;
				}
				else
				{
					keys &= ~index;
				}
			}

			this.memory.Write(Components.Memory.PeriphKeyboard, keys);
		}


		#region Processor
		protected void ProcessorReset()
		{
			//	Reset du processeur pour démarrer à l'adresse 0.
			this.processor.Reset();
			this.ProcessorFeedback();

			this.memory.ClearPeriph();
			this.SearchKey(0x08).ActiveState = ActiveState.No;  // relâche Shift
			this.SearchKey(0x10).ActiveState = ActiveState.No;  // relâche Ctrl
			this.SearchKey(100).ActiveState = ActiveState.No;  // relâche Left
			this.SearchKey(101).ActiveState = ActiveState.No;  // relâche Down
			this.SearchKey(102).ActiveState = ActiveState.No;  // relâche Right
			this.SearchKey(103).ActiveState = ActiveState.No;  // relâche Up
			this.UpdateKeyboard();
		}

		protected void ProcessorStart()
		{
			//	Démarre le processeur.
			if (this.switchStep.ActiveState == ActiveState.No)  // Continuous ?
			{
				if (this.clock == null)
				{
					this.clock = new Timer();
					this.ProcessorClockAdjust();
					this.clock.TimeElapsed += new EventHandler(this.HandleClockTimeElapsed);
					this.clock.Start();
				}
			}
			else  // Step ?
			{
				this.ProcessorFeedback();
			}
		}

		protected void ProcessorClockAdjust()
		{
			//	Ajuste l'horloge du processeur.
			if (this.clock != null)
			{
				//	N'utilise pas le mode AutoRepeat, qui pose des problèmes d'accumulation d'événements
				//	lorsque le traitement est lent (par exemple avec le panneau [CODE]). Un Start est
				//	refait à chaque événement HandleClockTimeElapsed.
				this.clock.Delay = 1.0/System.Math.Min(this.ips, DolphinApplication.RealMaxIps);
			}
		}

		protected void ProcessorStop()
		{
			//	Stoppe le processeur.
			if (this.clock != null)
			{
				this.clock.Stop();
				this.clock.TimeElapsed -= new EventHandler(this.HandleClockTimeElapsed);
				this.clock.Dispose();
				this.clock = null;
			}

			this.breakAddress = Misc.undefined;
		}

		protected void ProcessorBreakIn(int retAddress)
		{
			//	Part jusqu'à un break.
			this.breakAddress = retAddress;

			this.switchStep.ActiveState = ActiveState.No;  // Continuous
			this.UpdateButtons();

			this.ProcessorStart();
		}

		protected void ProcessorBreakOut()
		{
			//	Stoppe une fois le break rencontré.
			this.switchStep.ActiveState = ActiveState.Yes;  // Step
			this.UpdateButtons();
			
			this.ProcessorStop();
			this.ProcessorFeedback();
		}

		protected bool ProcessorClock()
		{
			//	Exécute une instruction du processeur.
			//	Retourne true si le processeur est arrivé sur un break.
			this.processor.Clock();

			if (this.processor.IsHalted)
			{
				this.ProcessorStop();

				this.AddressBits = this.AddressBits;
				this.DataBits = 0;
				
				this.buttonRun.ActiveState = ActiveState.No;
				this.UpdateButtons();
			}

			if (this.breakAddress != Misc.undefined)
			{
				int address = this.processor.GetRegisterValue("PC");
				return address == this.breakAddress;
			}

			return false;
		}

		protected void ProcessorFeedback()
		{
			//	Feedback visuel du processeur dans les widgets des différents panneaux.
			if (this.IsEmptyPanel)
			{
				return;
			}

			int pc = this.processor.GetRegisterValue("PC");
			this.AddressBits = pc;
			this.DataBits = this.memory.Read(pc);

			foreach (MyWidgets.TextFieldHexa field in this.registerFields)
			{
				this.ignoreChange = true;
				field.HexaValue = this.processor.GetRegisterValue(field.Label);
				this.ignoreChange = false;
			}

			foreach (TextField field in this.codeRegisters)
			{
				this.ignoreChange = true;
				int value = this.processor.GetRegisterValue(field.Name);
				int bitCount = this.processor.GetRegisterSize(field.Name);
				field.Text = MyWidgets.TextFieldHexa.GetHexaText(value, bitCount);
				this.ignoreChange = false;
			}

			this.MarkPC(pc, false);
			this.UpdateMemoryBank();
		}
		#endregion

		#region Binary Serialization
		protected bool QuestionYesNo(string message)
		{
			//	Pose une question de type oui/non. Retourne true en cas de réponse positive.
			string title = "Dauphin";
			string icon = "manifest:Epsitec.Common.Dialogs.Images.Question.icon";
			Common.Dialogs.IDialog dialog = Common.Dialogs.MessageDialog.CreateYesNo(title, icon, message, null, null, null);
			dialog.Owner = this.Window;
			dialog.OpenDialog();
			return dialog.Result == Common.Dialogs.DialogResult.Yes;
		}

		protected bool DlgOpenFilename()
		{
			//	Demande le nom du fichier à ouvrir.
			Common.Dialogs.FileOpen dlg = new Common.Dialogs.FileOpen();
			dlg.Title = "Ouverture d'un programme binaire";

			if (this.firstOpenSaveDialog)
			{
				dlg.InitialDirectory = this.UserSamplesPath;
				dlg.FileName = "";
			}
			else
			{
				dlg.FileName = "";
			}

			dlg.Filters.Add("dolphin", "Programmes", "*.dolphin");
			dlg.Owner = this.Window;
			dlg.OpenDialog();  // affiche le dialogue...

			if (dlg.Result != Common.Dialogs.DialogResult.Accept)
			{
				return false;
			}

			this.filename = dlg.FileName;
			return true;
		}

		protected bool DlgSaveFilename()
		{
			//	Demande le nom du fichier à enregistrer.
			Common.Dialogs.FileSave dlg = new Common.Dialogs.FileSave();
			dlg.PromptForOverwriting = true;
			dlg.Title = "Enregistrement d'un programme binaire";

			if (this.firstOpenSaveDialog && string.IsNullOrEmpty(this.filename))
			{
				dlg.InitialDirectory = this.UserSamplesPath;
			}
			else
			{
				dlg.FileName = this.filename;
			}

			dlg.Filters.Add("dolphin", "Programmes", "*.dolphin");
			dlg.Owner = this.Window;
			dlg.OpenDialog();  // affiche le dialogue...

			if (dlg.Result != Common.Dialogs.DialogResult.Accept)
			{
				return false;
			}

			this.filename = dlg.FileName;
			return true;
		}

		protected Common.Dialogs.DialogResult DlgAutoSave()
		{
			//	Demande s'il faut enregistrer le programme en cours.
			if (!this.dirty)
			{
				return Common.Dialogs.DialogResult.No;
			}

			string title = "Dauphin";
			string icon = "manifest:Epsitec.Common.Dialogs.Images.Question.icon";

			string message = "Voulez-vous enregistrer le programme ?";

			if (!string.IsNullOrEmpty(this.filename))
			{
				message = string.Format("Voulez-vous enregistrer <b>{0}</b> ?", System.IO.Path.GetFileNameWithoutExtension(this.filename));
			}

			Common.Dialogs.IDialog dialog = Common.Dialogs.MessageDialog.CreateYesNoCancel(title, icon, message, null, null, null);
			dialog.Owner = this.Window;
			dialog.OpenDialog();
			return dialog.Result;

		}

		protected void CopySampleFiles()
		{
			//	Copie tous les fichiers de "c:/Program Files/Epsitec/Dauphin/Samples" dans "c:/Mes Documents/Epsitec/Dauphin".
			string srcDir = this.OriginalSamplesPath;
			string dstDir = this.UserSamplesPath;

			System.IO.Directory.CreateDirectory(dstDir);

			string[] srcFiles = System.IO.Directory.GetFiles(srcDir, "*.dolphin", System.IO.SearchOption.TopDirectoryOnly);
			foreach (string srcFile in srcFiles)
			{
				string dstFile = System.IO.Path.Combine(dstDir, System.IO.Path.GetFileName(srcFile));
				if (!System.IO.File.Exists(dstFile))
				{
					System.IO.File.Copy(srcFile, dstFile);
				}
			}
		}

		protected void New()
		{
			//	Efface le programme en cours.
			if (this.AutoSave())
			{
				this.fieldProgramRem.Text = DolphinApplication.ProgramEmptyRem;
				this.fieldProgramRem.Cursor = 0;

				this.calmEditor.Text = "";
				this.calmEditor.Cursor = 0;

				this.Stop();
				this.memory.ClearRam();
				this.filename = "";
				this.Dirty = false;
				this.dirtyCalm = false;
				this.ProcessorReset();
				this.AddressBits = 0;
				this.DataBits = 0;
				this.UpdateData();
				this.UpdateButtons();
				this.UpdateFilename();
			}
		}

		protected bool AutoSave()
		{
			//	Demande s'il faut enregistrer le programme en cours avant de passer à un autre programme.
			//	Retourne true si on peut continuer (et donc effacer le programme en cours ou en ouvrir un autre).
			Common.Dialogs.DialogResult result = this.DlgAutoSave();

			if (result == Common.Dialogs.DialogResult.Cancel)
			{
				return false;
			}
			
			if (result == Common.Dialogs.DialogResult.Yes)
			{
				if (string.IsNullOrEmpty(this.filename))
				{
					if (!this.DlgSaveFilename())
					{
						return false;
					}
				}

				if (this.Save())
				{
					this.firstOpenSaveDialog = false;
				}
			}

			return true;
		}

		protected string OriginalSamplesPath
		{
			//	Retourne le nom du dossier contenant les exemples originaux.
			get
			{
				return string.Concat(Common.Support.Globals.Directories.Executable, "\\", "Samples");
			}
		}

		protected string UserSamplesPath
		{
			//	Retourne le nom du dossier contenant les exemples modifiables.
			get
			{
				FolderItem myDoc = FileManager.GetFolderItem(FolderId.MyDocuments, FolderQueryMode.NoIcons);
				return string.Concat(myDoc.FullPath, "\\Epsitec\\Dauphin");
			}
		}

		protected string Open()
		{
			//	Ouvre un nouveau programme.
			//	Retourne une éventuelle erreur.
			this.Stop();
			string err = null;

			string data = null;
			try
			{
				data = System.IO.File.ReadAllText(this.filename);
			}
			catch
			{
				data = null;
				this.filename = null;
				err = "Impossible d'ouvrir le fichier.";
			}

			if (data != null)
			{
				err = this.Deserialize(data);
				if (!string.IsNullOrEmpty(err))
				{
					this.filename = null;
				}
			}

			if (this.fieldProgramRem.Text != DolphinApplication.ProgramEmptyRem)
			{
				this.book.ActivePage = this.pageProgram;
			}

			this.Dirty = false;
			this.dirtyCalm = false;
			this.UpdateData();  // en premier, pour désassembler
			this.ProcessorReset();
			this.AddressBits = 0;
			this.DataBits = 0;
			this.UpdateButtons();
			this.UpdateClockButtons();
			this.UpdateFilename();

			return err;
		}

		protected bool Save()
		{
			//	Enregistre le programme en cours.
			//	Retourne false en cas d'erreur.
			string data = this.Serialize();
			try
			{
				System.IO.File.WriteAllText(this.filename, data);
			}
			catch
			{
				this.filename = null;
			}

			this.Dirty = false;
			this.UpdateFilename();

			return !string.IsNullOrEmpty(this.filename);
		}

		protected void Stop()
		{
			//	Stoppe le programme en cours.
			this.ProcessorStop();
			this.ProcessorReset();
			this.buttonRun.ActiveState = ActiveState.No;
			this.UpdateButtons();
		}

		public bool Dirty
		{
			//	Indique si le programme en cours doit être enregistré.
			get
			{
				return this.dirty;
			}
			set
			{
				//	Si on veut considérer le programme comme devant être enregistré et que
				//	la Ram est entièrement vide, ce n'est pas nécessaire, car la sérialisation
				//	n'aura aucun programme à sauvegarder !
				if (value == true && this.memory.IsEmptyRam && string.IsNullOrEmpty(this.calmEditor.Text))
				{
					return;
				}

				if (this.dirty != value)
				{
					this.dirty = value;
					this.UpdateSave();
				}
			}
		}

		protected string Serialize()
		{
			//	Sérialise la vue éditée et retourne le résultat dans un string.
			System.Text.StringBuilder buffer = new System.Text.StringBuilder();
			System.IO.StringWriter stringWriter = new System.IO.StringWriter(buffer);
			XmlTextWriter writer = new XmlTextWriter(stringWriter);
			writer.Formatting = Formatting.Indented;

			this.WriteXml(writer);

			writer.Flush();
			writer.Close();
			return buffer.ToString();
		}

		protected string Deserialize(string data)
		{
			//	Désérialise la vue à partir d'un string de données.
			//	Retourne une éventuelle erreur.
			System.IO.StringReader stringReader = new System.IO.StringReader(data);
			XmlTextReader reader = new XmlTextReader(stringReader);
			
			string err = this.ReadXml(reader);

			reader.Close();
			return err;
		}

		protected void WriteXml(XmlWriter writer)
		{
			//	Sérialise tout le programme.
			writer.WriteStartDocument();
			writer.WriteStartElement("Dolphin");
			
			writer.WriteElementString("Version", Misc.GetVersion());
			writer.WriteElementString("ProcessorName", this.processor.Name);
			writer.WriteElementString("ProcessorIPS", this.ips.ToString(System.Globalization.CultureInfo.InvariantCulture));
			writer.WriteElementString("ProcessorStep", (this.switchStep.ActiveState == ActiveState.Yes) ? "S" : "C");
			writer.WriteElementString("ProcessorInto", (this.switchInto.ActiveState == ActiveState.Yes) ? "I" : "O");
			writer.WriteElementString("PanelMode", this.panelMode);
			writer.WriteElementString("DisplayBitmap", (this.displayButtonMode.ActiveState == ActiveState.Yes) ? "Y" : "N");
			writer.WriteElementString("DisplayTechno", (this.displayBitmap.Technology == MyWidgets.Display.Type.LCD) ? "LCD" : "CRT");
			writer.WriteElementString("KeyboardArrows", this.keyboardArrows ? "Y" : "N");

			if (this.fieldProgramRem.Text != DolphinApplication.ProgramEmptyRem)
			{
				writer.WriteElementString("Rem", this.fieldProgramRem.Text);
			}

			if (!string.IsNullOrEmpty(this.calmEditor.Text))
			{
				writer.WriteElementString("Source", this.calmEditor.Text);
			}

			writer.WriteElementString("MemoryData", this.memory.GetContent());

			writer.WriteEndElement();
			writer.WriteEndDocument();
		}

		protected string ReadXml(XmlReader reader)
		{
			//	Désérialise tout le programme.
			//	Retourne une éventuelle erreur.
			this.memory.ClearRam();

			this.fieldProgramRem.Text = DolphinApplication.ProgramEmptyRem;
			this.fieldProgramRem.Cursor = 0;
			
			this.calmEditor.Text = "";
			this.calmEditor.Cursor = 0;

			reader.Read();
			while (true)
			{
				if (reader.NodeType == XmlNodeType.Element)
				{
					string name = reader.LocalName;

					if (name == "Version")
					{
						string element = reader.ReadElementString();
						if (Misc.CompareVersions(element, Misc.GetVersion()) > 0)
						{
							return "Ce fichier a été réalisé avec une version plus récente.";
						}
						reader.Read();
					}
					else if (name == "ProcessorName")
					{
						string element = reader.ReadElementString();
						reader.Read();
					}
					else if (name == "ProcessorIPS")
					{
						string element = reader.ReadElementString();
						this.ips = double.Parse(element, System.Globalization.CultureInfo.InvariantCulture);
						reader.Read();
					}
					else if (name == "ProcessorStep")
					{
						string element = reader.ReadElementString();
						this.switchStep.ActiveState = (element == "S") ? ActiveState.Yes : ActiveState.No;
						this.UpdateButtons();
						reader.Read();
					}
					else if (name == "ProcessorInto")
					{
						string element = reader.ReadElementString();
						this.switchInto.ActiveState = (element == "I") ? ActiveState.Yes : ActiveState.No;
						reader.Read();
					}
					else if (name == "PanelMode")
					{
						string element = reader.ReadElementString();
						this.panelMode = element;
						this.UpdatePanelMode();
						reader.Read();
					}
					else if (name == "DisplayBitmap")
					{
						string element = reader.ReadElementString();
						this.displayButtonMode.ActiveState = (element == "Y") ? ActiveState.Yes : ActiveState.No;
						this.UpdateDisplayMode();
						reader.Read();
					}
					else if (name == "DisplayTechno")
					{
						string element = reader.ReadElementString();
						this.displayBitmap.Technology = (element == "LCD") ? MyWidgets.Display.Type.LCD : MyWidgets.Display.Type.CRT;
						this.UpdateDisplayMode();
						reader.Read();
					}
					else if (name == "KeyboardArrows")
					{
						string element = reader.ReadElementString();
						this.keyboardArrows = (element == "Y");
						this.UpdateKeyboard();
						reader.Read();
					}
					else if (name == "MemoryData")
					{
						string element = reader.ReadElementString();
						this.memory.PutContent(element);
						reader.Read();
					}
					else if (name == "Rem")
					{
						string element = reader.ReadElementString();
						this.fieldProgramRem.Text = element;
						reader.Read();
					}
					else if (name == "Source")
					{
						string element = reader.ReadElementString();
						this.calmEditor.Text = element;
						reader.Read();
					}
					else
					{
						reader.Read();
					}
				}
				else if (reader.NodeType == XmlNodeType.EndElement)
				{
					if (reader.Name == "Dolphin")
					{
						return null;  // ok
					}
					else
					{
						return "Le format du fichier est incorrect.";
					}
				}
				else
				{
					reader.Read();
				}
			}
		}
		#endregion

		#region Source Serialization
		protected void CalmOpen()
		{
			//	Ouvre un programme source CALM.
			if (this.DlgOpenCalm())
			{
				string data = null;
				string err = null;
				try
				{
					data = System.IO.File.ReadAllText(this.filenameCalm);
				}
				catch
				{
					data = null;
					this.filenameCalm = null;
					err = "Impossible d'ouvrir le fichier.";
				}

				if (data == null)
				{
					string title = "Dauphin";
					string icon = "manifest:Epsitec.Common.Dialogs.Images.Warning.icon";
					Common.Dialogs.IDialog dialog = Common.Dialogs.MessageDialog.CreateOk(title, icon, err, null, null);
					dialog.Owner = this.Window;
					dialog.OpenDialog();
				}
				else
				{
					data = data.Replace("\r\n", "\n");
					this.calmEditor.Text = TextLayout.ConvertToTaggedText(data);
					this.calmEditor.Cursor = 0;
				}
			}
		}

		protected void CalmSave()
		{
			//	Enregistre un programme source CALM.
			if (this.DlgSaveCalm())
			{
				string data = TextLayout.ConvertToSimpleText(this.calmEditor.Text);
				data = data.Replace("\n", "\r\n");
				string err = null;
				try
				{
					System.IO.File.WriteAllText(this.filenameCalm, data);
				}
				catch
				{
					this.filenameCalm = null;
					err = "Impossible d'écrire le fichier.";
				}

				if (err != null)
				{
					string title = "Dauphin";
					string icon = "manifest:Epsitec.Common.Dialogs.Images.Warning.icon";
					Common.Dialogs.IDialog dialog = Common.Dialogs.MessageDialog.CreateOk(title, icon, err, null, null);
					dialog.Owner = this.Window;
					dialog.OpenDialog();
				}
			}
		}

		protected bool DlgOpenCalm()
		{
			//	Demande le nom du fichier à ouvrir.
			Common.Dialogs.FileOpen dlg = new Common.Dialogs.FileOpen();
			dlg.Title = "Importation d'un programme source";

			if (this.firstOpenSaveDialog)
			{
				dlg.InitialDirectory = this.UserSamplesPath;
				dlg.FileName = "";
			}
			else
			{
				dlg.FileName = "";
			}

			dlg.Filters.Add("txt", "Programmes", "*.txt");
			dlg.Owner = this.Window;
			dlg.OpenDialog();  // affiche le dialogue...

			if (dlg.Result != Common.Dialogs.DialogResult.Accept)
			{
				return false;
			}

			this.filenameCalm = dlg.FileName;
			return true;
		}

		protected bool DlgSaveCalm()
		{
			//	Demande le nom du fichier à enregistrer.
			Common.Dialogs.FileSave dlg = new Common.Dialogs.FileSave();
			dlg.PromptForOverwriting = true;
			dlg.Title = "Exportation d'un programme source";

			if (this.firstOpenSaveDialog && string.IsNullOrEmpty(this.filenameCalm))
			{
				dlg.InitialDirectory = this.UserSamplesPath;
			}
			else
			{
				dlg.FileName = this.filenameCalm;
			}

			dlg.Filters.Add("txt", "Programmes", "*.txt");
			dlg.Owner = this.Window;
			dlg.OpenDialog();  // affiche le dialogue...

			if (dlg.Result != Common.Dialogs.DialogResult.Accept)
			{
				return false;
			}

			this.filenameCalm = dlg.FileName;
			return true;
		}
		#endregion

		#region Event handler
		private void HandleButtonNewClicked(object sender, MessageEventArgs e)
		{
			//	Bouton ouvrir cliqué.
			this.New();
		}

		private void HandleButtonOpenClicked(object sender, MessageEventArgs e)
		{
			//	Bouton ouvrir cliqué.
			if (this.AutoSave() && this.DlgOpenFilename())
			{
				string err = this.Open();
				if (string.IsNullOrEmpty(err))
				{
					this.firstOpenSaveDialog = false;
				}
				else
				{
					string title = "Dauphin";
					string icon = "manifest:Epsitec.Common.Dialogs.Images.Warning.icon";
					Common.Dialogs.IDialog dialog = Common.Dialogs.MessageDialog.CreateOk(title, icon, err, null, null);
					dialog.Owner = this.Window;
					dialog.OpenDialog();
				}
			}
		}

		private void HandleButtonSaveClicked(object sender, MessageEventArgs e)
		{
			//	Bouton enregistrer cliqué.
			if (this.DlgSaveFilename())
			{
				if (this.Save())
				{
					this.firstOpenSaveDialog = false;
				}
			}
		}

		private void HandleButtonLookClicked(object sender, MessageEventArgs e)
		{
			//	Bouton "autre look" cliqué.
			switch (DolphinApplication.look)
			{
				case Look.GrayAndRed:
					DolphinApplication.look = Look.Vista;
					Epsitec.Common.Widgets.Adorners.Factory.SetActive("LookRoyale");
					break;

				case Look.Vista:
					DolphinApplication.look = Look.Metal;
					Epsitec.Common.Widgets.Adorners.Factory.SetActive("LookMetal");
					break;

				case Look.Metal:
					DolphinApplication.look = Look.GrayAndRed;
					Epsitec.Common.Widgets.Adorners.Factory.SetActive("LookSimply");
					break;
			}

			this.mainPanel.Invalidate();
		}

		private void HandleButtonAboutClicked(object sender, MessageEventArgs e)
		{
			//	Bouton "à propos de" cliqué.
			Dialogs.About dlg = new Dialogs.About(this);
			dlg.Show();
		}

		private void HandleButtonModeClicked(object sender, MessageEventArgs e)
		{
			//	Bouton pour choisir le mode des panneaux cliqué.
			MyWidgets.PushButton button = sender as MyWidgets.PushButton;
			this.panelMode = button.Name;

			this.UpdatePanelMode();
			this.UpdateData();
			this.ProcessorFeedback();
			this.MarkPC(this.processor.GetRegisterValue("PC"), false);
			this.Dirty = true;
		}

		private void HandleClockTimeElapsed(object sender)
		{
			//	Le timer demande d'exécuter l'instruction suivante.
			this.clock.Start();  // redémarre le timer

			if (this.ips > DolphinApplication.RealMaxIps)
			{
				//	Lorsqu'on exécute plusieurs ProcessorClock par HandleClockTimeElapsed, il est important
				//	de différer MemoryAccessor.UpdateData et CodeAccessor.UpdateData. Sans cela, un énorme
				//	ralentissement a lieu lorsqu'une instruction écrit en mémoire.
				this.memoryAccessor.IsDeferUpdateData = true;
				this.codeAccessor.IsDeferUpdateData = true;

				int count = (int) (this.ips/DolphinApplication.RealMaxIps);
				for (int i=0; i<count; i++)
				{
					if (this.ProcessorClock())
					{
						this.memoryAccessor.IsDeferUpdateData = false;
						this.codeAccessor.IsDeferUpdateData = false;

						this.ProcessorBreakOut();
						return;
					}
				}

				this.memoryAccessor.IsDeferUpdateData = false;
				this.codeAccessor.IsDeferUpdateData = false;
			}
			else
			{
				if (this.ProcessorClock())
				{
					this.ProcessorBreakOut();
					return;
				}
			}

			this.ProcessorFeedback();
		}

		private void HandleButtonRunClicked(object sender, MessageEventArgs e)
		{
			//	Bouton [R/S] cliqué.
			if (this.buttonRun.ActiveState == ActiveState.No)
			{
				if (this.dirtyCalm)  // source CALM modifié ?
				{
					if (this.QuestionYesNo("Voulez-vous assembler le programme source ?"))
					{
						if (this.assembler.Action(this.calmEditor, this.Window, false))
						{
							this.dirtyCalm = false;
						}
						else
						{
							return;
						}
					}
				}

				this.ProcessorReset();
				this.ProcessorStart();
				
				this.buttonRun.ActiveState = ActiveState.Yes;
			}
			else
			{
				this.ProcessorStop();

				this.AddressBits = this.AddressBits;
				this.DataBits = 0;
				
				this.buttonRun.ActiveState = ActiveState.No;
			}

			this.UpdateButtons();
		}

		private void HandleButtonResetClicked(object sender, MessageEventArgs e)
		{
			//	Bouton [RESET] cliqué.
			this.ProcessorReset();
		}

		private void HandleButtonStepClicked(object sender, MessageEventArgs e)
		{
			//	Bouton [S] cliqué.
			if (this.switchInto.ActiveState == ActiveState.Yes)  // into ?
			{
				this.ProcessorClock();
			}
			else  // over ?
			{
				int retAddress;
				if (this.processor.IsCall(out retAddress))
				{
					this.ProcessorBreakIn(retAddress);
				}
				else
				{
					this.ProcessorClock();
				}
			}

			this.ProcessorFeedback();
		}

		private void HandleButtonClockClicked(object sender, MessageEventArgs e)
		{
			//	Bouton pour choisir la fréquence d'horloge cliqué.
			MyWidgets.PushButton button = sender as MyWidgets.PushButton;

			this.ips = button.Index;
			this.UpdateClockButtons();
			this.ProcessorClockAdjust();

			this.Dirty = true;
		}

		private void HandleSwitchStepClicked(object sender, MessageEventArgs e)
		{
			//	Switch CONT/STEP basculé.
			this.switchStep.ActiveState = (this.switchStep.ActiveState == ActiveState.No) ? ActiveState.Yes : ActiveState.No;
			this.UpdateButtons();

			if (this.buttonRun.ActiveState == ActiveState.Yes)
			{
				if (this.switchStep.ActiveState == ActiveState.No)  // Continuous ?
				{
					this.ProcessorStart();
				}
				else  // Step ?
				{
					this.ProcessorStop();
					this.ProcessorFeedback();
				}
			}

			this.Dirty = true;
		}

		private void HandleSwitchIntoClicked(object sender, MessageEventArgs e)
		{
			//	Switch OVER/INTO basculé.
			this.switchInto.ActiveState = (this.switchInto.ActiveState == ActiveState.No) ? ActiveState.Yes : ActiveState.No;
			this.Dirty = true;
		}

		private void HandleButtonMemoryPressed(object sender, MessageEventArgs e)
		{
			//	Bouton [R]/[W] d'accès à la mémoire pressé.
			if (this.buttonRun.ActiveState == ActiveState.Yes)
			{
				return;
			}

			if (sender == this.buttonMemoryRead)  // read ?
			{
				this.DataBits = this.memory.Read(this.AddressBits);
			}

			if (sender == this.buttonMemoryWrite)  // write ?
			{
				this.memory.WriteWithDirty(this.AddressBits, this.DataBits);
				this.DataBits = this.memory.Read(this.AddressBits);
			}
		}

		private void HandleButtonMemoryReleased(object sender, MessageEventArgs e)
		{
			//	Bouton [M] relâché.
			if (this.buttonRun.ActiveState == ActiveState.Yes)
			{
				return;
			}

			this.DataBits = 0;

			//	Nécessaire même en lecture, car la lecture du clavier clear le bit full !
			this.UpdateData();
		}

		private void HandleAddressSwitchClicked(object sender, MessageEventArgs e)
		{
			//	Switch d'adresse basculé.
			if (this.buttonRun.ActiveState == ActiveState.Yes)
			{
				return;
			}

			MyWidgets.Switch button = sender as MyWidgets.Switch;

			if (button.ActiveState == ActiveState.No)
			{
				button.ActiveState = ActiveState.Yes;
			}
			else
			{
				button.ActiveState = ActiveState.No;
			}

			this.AddressBits = this.AddressBits;  // allume les leds selon les switchs
		}

		private void HandleDataSwitchClicked(object sender, MessageEventArgs e)
		{
			//	Switch de data basculé.
			if (this.buttonRun.ActiveState == ActiveState.Yes)
			{
				return;
			}

			MyWidgets.Switch button = sender as MyWidgets.Switch;

			if (button.ActiveState == ActiveState.No)
			{
				button.ActiveState = ActiveState.Yes;
			}
			else
			{
				button.ActiveState = ActiveState.No;
			}
		}

		private void HandleProcessorRegisterChanged(object sender)
		{
			//	La valeur d'un registre du processeur a été changée.
			if (this.ignoreChange)
			{
				return;
			}

			TextField field = sender as TextField;
			int value = Misc.ParseHexa(field.Text);
			this.processor.SetRegisterValue(field.Name, value);

			if (field.Name == "PC")
			{
				int pc = this.processor.GetRegisterValue("PC");
				this.MarkPC(pc, false);
				this.UpdateMemoryBank();
			}
		}

		private void HandleProcessorHexaValueChanged(object sender)
		{
			//	La valeur d'un registre du processeur a été changée.
			if (this.ignoreChange)
			{
				return;
			}

			MyWidgets.TextFieldHexa field = sender as MyWidgets.TextFieldHexa;
			this.processor.SetRegisterValue(field.Name, field.HexaValue);

			if (field.Name == "PC")
			{
				int pc = this.processor.GetRegisterValue("PC");
				this.MarkPC(pc, false);
				this.UpdateMemoryBank();
			}
		}

		private void HandleMemoryButtonPCClicked(object sender, MessageEventArgs e)
		{
			this.memoryAccessor.FollowPC = !this.memoryAccessor.FollowPC;

			if (this.memoryAccessor.FollowPC)
			{
				this.MarkPC(this.processor.GetRegisterValue("PC"), true);
			}

			this.UpdateMemoryBank();
		}

		private void HandleMemoryButtonClicked(object sender, MessageEventArgs e)
		{
			//	Bouton [RAM], [ROM] ou [PER] cliqué.
			MyWidgets.PushButton button = sender as MyWidgets.PushButton;
			this.memoryAccessor.Bank = button.Name;
			this.UpdateMemoryBank();
		}

		private void HandleCodeAccessorInstructionSelected(object sender)
		{
			//	L'instruction sélectionnée dans le panneau Code a changé.
			int address = this.codeAccessor.AddressSelected;
			bool isReadOnly = this.memory.IsReadOnly(this.codeAccessor.MemoryStart);

			this.codeButtonAdd.Enable = (address != Misc.undefined && !isReadOnly);
			this.codeButtonSub.Enable = (address != Misc.undefined && !isReadOnly);
		}

		private void HandleCodeAccessorBankChanged(object sender)
		{
			this.UpdateMemoryBank();
		}

		private void HandleCodeButtonPCClicked(object sender, MessageEventArgs e)
		{
			this.codeAccessor.FollowPC = !this.codeAccessor.FollowPC;

			if (this.codeAccessor.FollowPC)
			{
				this.MarkPC(this.processor.GetRegisterValue("PC"), true);
			}

			this.UpdateMemoryBank();
		}

		private void HandleCodeButtonClicked(object sender, MessageEventArgs e)
		{
			//	Bouton [RAM], [ROM] ou [PER] cliqué.
			MyWidgets.PushButton button = sender as MyWidgets.PushButton;
			this.codeAccessor.Bank = button.Name;
			this.UpdateMemoryBank();
		}

		private void HandleCodeAddButtonClicked(object sender, MessageEventArgs e)
		{
			//	Bouton [+] cliqué.
			int address = this.codeAccessor.AddressSelected;
			this.memory.ShiftRam(address, 1);
			this.UpdateData();
		}

		private void HandleCodeSubButtonClicked(object sender, MessageEventArgs e)
		{
			//	Bouton [-] cliqué.
			int address = this.codeAccessor.AddressSelected;
			int length = this.codeAccessor.LengthSelected;
			this.memory.ShiftRam(address, -length);
			this.UpdateData();
		}

		private void HandleCalmOpenClicked(object sender, MessageEventArgs e)
		{
			//	Bouton "open CALM" cliqué.
			this.CalmOpen();
		}

		private void HandleCalmSaveClicked(object sender, MessageEventArgs e)
		{
			//	Bouton "save CALM" cliqué.
			this.CalmSave();
		}

		private void HandleCalmShowClicked(object sender, MessageEventArgs e)
		{
			//	Bouton "show parrus" cliqué.
			this.calmEditor.TextLayout.ShowLineBreak = !this.calmEditor.TextLayout.ShowLineBreak;
			this.calmEditor.TextLayout.ShowTab = !this.calmEditor.TextLayout.ShowTab;
			this.calmEditor.Invalidate();  // TODO: devrait être inutile, non ?
			this.UpdateCalmButtons();
		}

		private void HandleCalmBigClicked(object sender, MessageEventArgs e)
		{
			//	Bouton "grande police" cliqué.
			if (this.calmButtonBig.ActiveState == ActiveState.No)
			{
				this.calmButtonBig.ActiveState = ActiveState.Yes;
			}
			else
			{
				this.calmButtonBig.ActiveState = ActiveState.No;
			}

			this.UpdateCalmEditor();
		}

		private void HandleCalmFullClicked(object sender, MessageEventArgs e)
		{
			//	Bouton "plein écran" cliqué.
			if (this.calmButtonFull.ActiveState == ActiveState.No)
			{
				this.calmButtonFull.ActiveState = ActiveState.Yes;
			}
			else
			{
				this.calmButtonFull.ActiveState = ActiveState.No;
			}

			bool full = (this.calmButtonFull.ActiveState == ActiveState.Yes);

			this.panelTitle.Visibility = !full;
			this.rightPanel.Visibility = !full;
			this.leftHeader.Visibility = !full;
			this.topLeftSep.Visibility = !full;
			this.leftClock.Visibility = !full;

			this.calmPanel.PreferredHeight = full ? 552 : DolphinApplication.PanelHeight;
			this.calmPanel.MinWidth = full ? 752 : DolphinApplication.PanelWidth;
			this.calmPanel.MaxWidth = full ? 752 : DolphinApplication.PanelWidth;
		}

		private void HandleCalmErrorClicked(object sender, MessageEventArgs e)
		{
			//	Bouton [ERR] cliqué, pour chercher l'erreur suivante.
			int cursor = this.calmEditor.Cursor;
			int index = -1;

			string text = TextLayout.ConvertToSimpleText(this.calmEditor.Text);
			if (cursor+1 < text.Length)
			{
				index = text.IndexOf("^ ", cursor+1);  // cherche depuis la position du curseur
			}

			if (index == -1 && cursor > 0)
			{
				index = text.IndexOf("^ ");  // cherche depuis le début
			}

			if (index == -1)  // pas trouvé ?
			{
				string title = "Dauphin";
				string icon = "manifest:Epsitec.Common.Dialogs.Images.Warning.icon";
				string err = "Le source ne contient aucune erreur.";
				Common.Dialogs.IDialog dialog = Common.Dialogs.MessageDialog.CreateOk(title, icon, err, null, null);
				dialog.Owner = this.Window;
				dialog.OpenDialog();
			}
			else  // trouvé ?
			{
				this.calmEditor.Cursor = index;
			}
		}

		private void HandleCalmAssemblerClicked(object sender, MessageEventArgs e)
		{
			//	Bouton [ASSEMBLER] cliqué.
			this.Stop();
			if (this.assembler.Action(this.calmEditor, this.Window, true))
			{
				this.dirtyCalm = false;
			}
		}

		private void HandleCalmEditorTextChanged(object sender)
		{
			//	Le texte source CALM a été changé.
			if (this.ignoreChange)
			{
				return;
			}

			this.Dirty = true;
			this.dirtyCalm = true;
		}

		private void HandleDisplayButtonModeClicked(object sender, MessageEventArgs e)
		{
			//	Bouton [DISPLAY] cliqué.
			if (this.displayButtonMode.ActiveState == ActiveState.No)
			{
				this.displayButtonMode.ActiveState = ActiveState.Yes;
			}
			else
			{
				this.displayButtonMode.ActiveState = ActiveState.No;
			}

			this.UpdateDisplayMode();
			this.Dirty = true;
		}

		private void HandleDisplayButtonTechnoClicked(object sender, MessageEventArgs e)
		{
			//	Bouton [CRT/LCD] cliqué.
			if (this.displayBitmap.Technology == MyWidgets.Display.Type.CRT)
			{
				this.displayBitmap.Technology = MyWidgets.Display.Type.LCD;
			}
			else
			{
				this.displayBitmap.Technology = MyWidgets.Display.Type.CRT;
			}

			this.UpdateDisplayMode();
			this.Dirty = true;
		}

		private void HandleDisplayButtonClsClicked(object sender, MessageEventArgs e)
		{
			//	Bouton [CLS] cliqué.
			this.memory.ClearDisplay();
		}

		private void HandleDisplayButtonKeyClicked(object sender, MessageEventArgs e)
		{
			//	Bouton [ARROWS/NUMERIC] cliqué.
			this.keyboardArrows = !this.keyboardArrows;
			this.UpdateKeyboard();
			this.Dirty = true;
		}

		private void HandleKeyboardButtonPressed(object sender, MessageEventArgs e)
		{
			//	Touche du clavier simulé pressée.
			MyWidgets.PushButton button = sender as MyWidgets.PushButton;

			if (button.Index == 0x08 || button.Index == 0x10 || button.Index >= 100)  // shift, ctrl ou flèche ?
			{
				button.ActiveState = (button.ActiveState == ActiveState.Yes) ? ActiveState.No : ActiveState.Yes;
				this.UpdateKeyboard();
			}

			this.KeyboardChanged(button, true);
		}

		private void HandleKeyboardButtonReleased(object sender, MessageEventArgs e)
		{
			//	Touche du clavier simulé relâchée.
			MyWidgets.PushButton button = sender as MyWidgets.PushButton;
			this.KeyboardChanged(button, false);
		}

		private void HandleButtonStyleClicked(object sender, MessageEventArgs e)
		{
			//	Bouton pour modifier le style du commentaire cliqué.
			IconButton button = sender as IconButton;

			if (button.Name == "FontBold")
			{
				this.fieldProgramRem.TextNavigator.SelectionBold = !this.fieldProgramRem.TextNavigator.SelectionBold;
			}

			if (button.Name == "FontItalic")
			{
				this.fieldProgramRem.TextNavigator.SelectionItalic = !this.fieldProgramRem.TextNavigator.SelectionItalic;
			}

			if (button.Name == "FontUnderline")
			{
				this.fieldProgramRem.TextNavigator.SelectionUnderline = !this.fieldProgramRem.TextNavigator.SelectionUnderline;
			}

			this.Dirty = true;
		}

		private void HandleFieldProgramRemTextChanged(object sender)
		{
			//	Le commentaire lié au programme est changé.
			this.Dirty = true;
		}
		#endregion


		#region Look
		protected enum Look
		{
			GrayAndRed,		// look standard gris et rouge
			Vista,			// look "Vista" bleu et orange
			Metal,			// look métalique gris-vert et jaune
		}

		public static Color ColorHilite
		{
			//	Retourne la couleur de mise en évidence, lorsqu'un bouton est allumé.
			get
			{
				switch (DolphinApplication.look)
				{
					case Look.GrayAndRed:
						return Color.FromRgb(1.00, 0.00, 0.00);  // rouge

					case Look.Vista:
						return Color.FromRgb(254.0/255.0, 156.0/255.0, 84.0/255.0);  // orange

					case Look.Metal:
						return Color.FromRgb(254.0/255.0, 224.0/255.0, 84.0/255.0);  // jaune

					default:
						return Color.FromRgb(1.00, 0.00, 0.00);
				}
			}
		}

		public static Color FromBrightness(double brightness)
		{
			//	Retourne la couleur à partir de l'intensité.
			if (DolphinApplication.look == Look.GrayAndRed)
			{
				return Color.FromBrightness(brightness);
			}

			if (DolphinApplication.look == Look.Vista)
			{
				brightness = System.Math.Pow(brightness, 1.2);
				brightness = Misc.Norm(brightness);

				for (int i=0; i<DolphinApplication.rgb.Length; i+=3)
				{
					if (brightness == DolphinApplication.rgb[i+1])
					{
						return Color.FromRgb(DolphinApplication.rgb[i+0], brightness, DolphinApplication.rgb[i+2]);
					}
					else if (brightness > DolphinApplication.rgb[i+1])
					{
						double d = (brightness-DolphinApplication.rgb[i+1])/(DolphinApplication.rgb[i-3+1]-DolphinApplication.rgb[i+1]);
						double r = DolphinApplication.rgb[i+0] + d*(DolphinApplication.rgb[i-3+0]-DolphinApplication.rgb[i+0]);
						double b = DolphinApplication.rgb[i+2] + d*(DolphinApplication.rgb[i-3+2]-DolphinApplication.rgb[i+2]);
						return Color.FromRgb(r, brightness, b);
					}
				}
			}

			if (DolphinApplication.look == Look.Metal)
			{
				brightness = System.Math.Pow(brightness, 1.5);
				brightness = Misc.Norm(brightness);

				for (int i=0; i<DolphinApplication.rgb.Length; i+=3)
				{
					if (brightness == DolphinApplication.rgb[i+1])
					{
						return Color.FromRgb(DolphinApplication.rgb[i+0], brightness, brightness);
					}
					else if (brightness > DolphinApplication.rgb[i+1])
					{
						double d = (brightness-DolphinApplication.rgb[i+1])/(DolphinApplication.rgb[i-3+1]-DolphinApplication.rgb[i+1]);
						double r = DolphinApplication.rgb[i+0] + d*(DolphinApplication.rgb[i-3+0]-DolphinApplication.rgb[i+0]);
						return Color.FromRgb(r, brightness, brightness);
					}
				}
			}

			return Color.FromBrightness(0);
		}

		//	Cette table reproduit les bleus de Office Word 2007:
		protected static double[] rgb =
		{
			255.0/255.0, 255.0/255.0, 255.0/255.0,
			219.0/255.0, 244.0/255.0, 254.0/255.0,
			204.0/255.0, 223.0/255.0, 248.0/255.0,
			191.0/255.0, 219.0/255.0, 255.0/255.0,
			177.0/255.0, 203.0/255.0, 235.0/255.0,
			153.0/255.0, 184.0/255.0, 223.0/255.0,
			140.0/255.0, 174.0/255.0, 217.0/255.0,
			121.0/255.0, 157.0/255.0, 203.0/255.0,
			 95.0/255.0, 138.0/255.0, 194.0/255.0,
			 79.0/255.0, 102.0/255.0, 132.0/255.0,
			 66.0/255.0,  75.0/255.0,  99.0/255.0,
			  0.0/255.0,   0.0/255.0,   0.0/255.0,
		};

		//	Look actuellement sélectionné.
		protected static Look look = Look.GrayAndRed;
		#endregion


		public static readonly double MainWidth  = 800;
		public static readonly double MainHeight = 600;
		public static readonly double MainMargin = 6;

		public static readonly double PanelWidth  = 400;
		public static readonly double PanelHeight = 464;

		protected static readonly double RealMaxIps = 20;
		public static readonly string ApplicationTitle = "Simulateur de Dauphin";
		protected static readonly string ProgramEmptyRem = "<br/><i>Tapez ici les commentaires sur le programme...</i>";


		protected Common.Support.ResourceManagerPool	resourceManagerPool;
		protected MyWidgets.MainPanel					mainPanel;
		protected MyWidgets.Panel						panelTitle;
		protected MyWidgets.Panel						leftPanel;
		protected MyWidgets.Panel						rightPanel;
		protected MyWidgets.Panel						leftHeader;
		protected MyWidgets.Panel						leftClock;
		protected MyWidgets.Panel						leftPanelBus;
		protected MyWidgets.Panel						leftPanelDetail;
		protected MyWidgets.Panel						leftPanelCode;
		protected MyWidgets.Panel						leftPanelCalm;
		protected MyWidgets.Panel						leftPanelQuick;
		protected MyWidgets.Panel						clockBusPanel;
		protected MyWidgets.Panel						helpPanel;
		protected MyWidgets.Panel						calmPanel;
		protected MyWidgets.Line						topLeftSep;
		protected IconButton							buttonNew;
		protected IconButton							buttonOpen;
		protected IconButton							buttonSave;
		protected IconButton							buttonLook;
		protected IconButton							buttonAbout;
		protected MyWidgets.PushButton					buttonModeBus;
		protected MyWidgets.PushButton					buttonModeDetail;
		protected MyWidgets.PushButton					buttonModeCode;
		protected MyWidgets.PushButton					buttonModeCalm;
		protected MyWidgets.PushButton					buttonModeQuick;
		protected MyWidgets.PushButton					buttonRun;
		protected MyWidgets.PushButton					buttonStep;
		protected MyWidgets.PushButton					buttonMemoryRead;
		protected MyWidgets.PushButton					buttonMemoryWrite;
		protected MyWidgets.PushButton					buttonClock6;
		protected MyWidgets.PushButton					buttonClock5;
		protected MyWidgets.PushButton					buttonClock4;
		protected MyWidgets.PushButton					buttonClock3;
		protected MyWidgets.PushButton					buttonClock2;
		protected MyWidgets.PushButton					buttonClock1;
		protected MyWidgets.PushButton					buttonClock0;
		protected MyWidgets.Switch						switchStep;
		protected MyWidgets.Switch						switchInto;
		protected List<MyWidgets.Digit>					addressDigits;
		protected List<MyWidgets.Led>					addressLeds;
		protected List<MyWidgets.Switch>				addressSwitchs;
		protected List<MyWidgets.Digit>					dataDigits;
		protected List<MyWidgets.Led>					dataLeds;
		protected List<MyWidgets.Switch>				dataSwitchs;
		protected MyWidgets.PushButton					displayButtonMode;
		protected MyWidgets.PushButton					displayButtonTechno;
		protected MyWidgets.PushButton					displayButtonCls;
		protected MyWidgets.PushButton					displayButtonKey;
		protected List<MyWidgets.Digit>					displayDigits;
		protected MyWidgets.Display						displayBitmap;
		protected List<MyWidgets.PushButton>			keyboardButtons;
		protected MyWidgets.PushButton					buttonReset;
		protected List<MyWidgets.TextFieldHexa>			registerFields;
		protected MyWidgets.MemoryAccessor				memoryAccessor;
		protected MyWidgets.PushButton					memoryButtonPC;
		protected MyWidgets.PushButton					memoryButtonM;
		protected MyWidgets.PushButton					memoryButtonR;
		protected MyWidgets.PushButton					memoryButtonP;
		protected MyWidgets.PushButton					memoryButtonD;
		protected MyWidgets.CodeAccessor				codeAccessor;
		protected MyWidgets.PushButton					codeButtonPC;
		protected MyWidgets.PushButton					codeButtonM;
		protected MyWidgets.PushButton					codeButtonR;
		protected MyWidgets.PushButton					codeButtonAdd;
		protected MyWidgets.PushButton					codeButtonSub;
		protected List<TextField>						codeRegisters;
		protected TextFieldMulti						calmEditor;
		protected IconButton							calmButtonOpen;
		protected IconButton							calmButtonSave;
		protected IconButton							calmButtonShow;
		protected IconButton							calmButtonBig;
		protected IconButton							calmButtonFull;
		protected MyWidgets.PushButton					calmButtonErr;
		protected MyWidgets.PushButton					calmButtonAss;
		protected TabBook								book;
		protected TabPage								pageProgram;
		protected TextFieldMulti						fieldProgramRem;

		protected Components.Memory						memory;
		protected Components.AbstractProcessor			processor;
		protected Assembler								assembler;
		protected int									breakAddress;
		protected Timer									clock;
		protected double								ips;
		protected string								panelMode;
		protected string								filename;
		protected string								filenameCalm;
		protected bool									firstOpenSaveDialog;
		protected bool									keyboardArrows;
		protected bool									dirty;
		protected bool									dirtyCalm;
		protected bool									ignoreChange;
	}
}
