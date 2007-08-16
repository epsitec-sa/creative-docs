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
			this.breakAddress = -1;
			this.memory.RomInitialise(this.processor);
			this.ips = 10000;
			this.panelMode = "Bus";
			this.firstOpenSaveDialog = true;

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
				//?window.PreventAutoQuit = false;
				
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
			this.mainPanel.BackColor = Color.FromBrightness(0.7);
			this.mainPanel.DrawFullFrame = true;
			this.mainPanel.DrawScrew = true;
			this.mainPanel.MinSize = new Size(DolphinApplication.MainWidth, DolphinApplication.MainHeight);
			this.mainPanel.MaxSize = new Size(DolphinApplication.MainWidth, DolphinApplication.MainHeight);
			this.mainPanel.PreferredSize = new Size(DolphinApplication.MainWidth, DolphinApplication.MainHeight);
			this.mainPanel.Margins = new Margins(DolphinApplication.MainMargin, DolphinApplication.MainMargin, DolphinApplication.MainMargin, DolphinApplication.MainMargin);
			this.mainPanel.Padding = new Margins(14, 14, 14, 0);
			this.mainPanel.Dock = DockStyle.Fill;

			MyWidgets.Panel panelTitle = new MyWidgets.Panel(this.mainPanel);
			panelTitle.BackColor = Color.FromBrightness(0.9);
			panelTitle.DrawFullFrame = true;
			panelTitle.PreferredHeight = 40;
			panelTitle.Margins = new Margins(0, 0, 0, 10);
			panelTitle.Dock = DockStyle.Top;

			StaticText epsitec = new StaticText(this.mainPanel);
			epsitec.Text = "© EPSITEC";
			epsitec.ContentAlignment = ContentAlignment.MiddleRight;
			epsitec.PreferredHeight = 13;
			epsitec.Margins = new Margins(0, 10, 0, 1);
			epsitec.Dock = DockStyle.Bottom;

			this.buttonNew = new IconButton(panelTitle);
			this.buttonNew.AutoFocus = false;
			this.buttonNew.IconName = Misc.Icon("New");
			this.buttonNew.Margins = new Margins(10, 0, 8, 8);
			this.buttonNew.Dock = DockStyle.Left;
			this.buttonNew.Clicked += new MessageEventHandler(this.HandleButtonNewClicked);
			ToolTip.Default.SetToolTip(this.buttonNew, "Nouveau programme");

			this.buttonOpen = new IconButton(panelTitle);
			this.buttonOpen.AutoFocus = false;
			this.buttonOpen.IconName = Misc.Icon("Open");
			this.buttonOpen.Margins = new Margins(0, 0, 8, 8);
			this.buttonOpen.Dock = DockStyle.Left;
			this.buttonOpen.Clicked += new MessageEventHandler(this.HandleButtonOpenClicked);
			ToolTip.Default.SetToolTip(this.buttonOpen, "Ouvrir un programme");

			this.buttonSave = new IconButton(panelTitle);
			this.buttonSave.AutoFocus = false;
			this.buttonSave.IconName = Misc.Icon("Save");
			this.buttonSave.Margins = new Margins(0, 0, 8, 8);
			this.buttonSave.Dock = DockStyle.Left;
			this.buttonSave.Clicked += new MessageEventHandler(this.HandleButtonSaveClicked);
			ToolTip.Default.SetToolTip(this.buttonSave, "Enregistrer le programme");

			this.programmFilename = new StaticText(panelTitle);
			this.programmFilename.PreferredWidth = 100;
			this.programmFilename.Margins = new Margins(10, 0, 0, 0);
			this.programmFilename.Dock = DockStyle.Left;

			StaticText title = new StaticText(panelTitle);
			title.Text = string.Concat("<font size=\"200%\"><b>", DolphinApplication.ApplicationTitle, "</b></font>");
			title.ContentAlignment = ContentAlignment.MiddleCenter;
			title.Margins = new Margins(0, 100, 0, 0);
			title.Dock = DockStyle.Fill;

			this.buttonAbout = new IconButton(panelTitle);
			this.buttonAbout.AutoFocus = false;
			this.buttonAbout.IconName = Misc.Icon("About");
			this.buttonAbout.Margins = new Margins(0, 10, 8, 8);
			this.buttonAbout.Dock = DockStyle.Right;
			this.buttonAbout.Clicked += new MessageEventHandler(this.HandleButtonAboutClicked);
			ToolTip.Default.SetToolTip(this.buttonAbout, "A propos du Simulateur de Dauphin");

#if false
			StaticText version = new StaticText(panelTitle);
			version.Text = string.Concat("<font size=\"80%\">", Misc.GetVersion(), "</font>");
			version.ContentAlignment = ContentAlignment.MiddleRight;
			version.Margins = new Margins(0, 5, 0, 0);
			version.Dock = DockStyle.Right;
#endif

			MyWidgets.Panel all = new MyWidgets.Panel(this.mainPanel);
			all.Dock = DockStyle.Fill;

			//	Crée les deux grandes parties gauche/droite.
			MyWidgets.Panel leftPanel = new MyWidgets.Panel(all);
			leftPanel.BackColor = Color.FromBrightness(0.9);
			leftPanel.DrawFullFrame = true;
			leftPanel.PreferredWidth = 510;
			leftPanel.Padding = new Margins(0, 0, 0, 10);
			leftPanel.Dock = DockStyle.Left;

			MyWidgets.Panel rightPanel = new MyWidgets.Panel(all);
			rightPanel.Margins = new Margins(10, 0, 0, 0);
			rightPanel.Dock = DockStyle.Fill;

			//	Crée les 3 parties de gauche.
			MyWidgets.Panel leftHeader = new MyWidgets.Panel(leftPanel);
			leftHeader.PreferredHeight = 1;  // sera agrandi
			leftHeader.Margins = new Margins(0, 0, 5, 0);
			leftHeader.Dock = DockStyle.Top;

			MyWidgets.Line sep = new MyWidgets.Line(leftPanel);
			sep.PreferredHeight = 1;
			sep.Margins = new Margins(0, 0, 5, 3);
			sep.Dock = DockStyle.Top;

			MyWidgets.Panel leftClock = new MyWidgets.Panel(leftPanel);
			leftClock.PreferredWidth = 50;
			leftClock.Margins = new Margins(10, 0, 0, 0);
			leftClock.Dock = DockStyle.Left;

			this.leftPanelBus = new MyWidgets.Panel(leftPanel);
			this.leftPanelBus.Dock = DockStyle.Fill;

			this.leftPanelDetail = new MyWidgets.Panel(leftPanel);
			this.leftPanelDetail.Dock = DockStyle.Fill;
			this.leftPanelDetail.Visibility = false;

			this.leftPanelCode = new MyWidgets.Panel(leftPanel);
			this.leftPanelCode.Dock = DockStyle.Fill;
			this.leftPanelCode.Visibility = false;

			this.leftPanelQuick = new MyWidgets.Panel(leftPanel);
			this.leftPanelQuick.Dock = DockStyle.Fill;
			this.leftPanelQuick.Visibility = false;

			//	Crée les 2 parties de droite.
			this.helpPanel = new MyWidgets.Panel(rightPanel);
			this.helpPanel.Margins = new Margins(0, 0, 0, 10);
			this.helpPanel.Dock = DockStyle.Fill;

			MyWidgets.Panel kdPanel = new MyWidgets.Panel(rightPanel);
			kdPanel.BackColor = Color.FromBrightness(0.9);
			kdPanel.DrawFullFrame = true;
			kdPanel.DrawScrew = true;
			kdPanel.PreferredHeight = 100;  // minimum qui sera étendu
			kdPanel.Margins = new Margins(0, 0, 0, 0);
			kdPanel.Padding = new Margins(12, 0, 10, 2);
			kdPanel.Dock = DockStyle.Bottom;

			//	Crée le contenu des différentes parties.
			this.CreateOptions(leftHeader);
			this.CreateClockControl(leftClock);
			this.CreateBusPanel(this.leftPanelBus);
			this.CreateDetailPanel(this.leftPanelDetail);
			this.CreateCodePanel(this.leftPanelCode);
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
			this.buttonModeBus.PreferredSize = new Size(83, 24);
			this.buttonModeBus.Margins = new Margins(60+10, 0, 0, 0);
			this.buttonModeBus.Dock = DockStyle.Left;
			this.buttonModeBus.Clicked += new MessageEventHandler(this.HandleButtonModeClicked);
			ToolTip.Default.SetToolTip(this.buttonModeBus, "Montre les bus d'adresse et de données");

			this.buttonModeDetail = new MyWidgets.PushButton(parent);
			this.buttonModeDetail.Text = "PROC+MEM";
			this.buttonModeDetail.Name = "Detail";
			this.buttonModeDetail.PreferredSize = new Size(83, 24);
			this.buttonModeDetail.Margins = new Margins(2, 0, 0, 0);
			this.buttonModeDetail.Dock = DockStyle.Left;
			this.buttonModeDetail.Clicked += new MessageEventHandler(this.HandleButtonModeClicked);
			ToolTip.Default.SetToolTip(this.buttonModeDetail, "Montre les registres du processeur et la mémoire");

			this.buttonModeCode = new MyWidgets.PushButton(parent);
			this.buttonModeCode.Text = "CODE";
			this.buttonModeCode.Name = "Code";
			this.buttonModeCode.PreferredSize = new Size(83, 24);
			this.buttonModeCode.Margins = new Margins(2, 0, 0, 0);
			this.buttonModeCode.Dock = DockStyle.Left;
			this.buttonModeCode.Clicked += new MessageEventHandler(this.HandleButtonModeClicked);
			ToolTip.Default.SetToolTip(this.buttonModeCode, "Montre les instructions du processeur");

			this.buttonModeQuick = new MyWidgets.PushButton(parent);
			this.buttonModeQuick.Text = "RAPIDE";
			this.buttonModeQuick.Name = "Quick";
			this.buttonModeQuick.PreferredSize = new Size(83, 24);
			this.buttonModeQuick.Margins = new Margins(2, 0, 0, 0);
			this.buttonModeQuick.Dock = DockStyle.Left;
			this.buttonModeQuick.Clicked += new MessageEventHandler(this.HandleButtonModeClicked);
			ToolTip.Default.SetToolTip(this.buttonModeQuick, "Ne montre rien (mode rapide)");
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
			codePanel.PreferredHeight = 47+21*16;  // place pour 16 instructions
			codePanel.Dock = DockStyle.Bottom;

			this.codeButtonM = new MyWidgets.PushButton(header);
			this.codeButtonM.Text = "RAM";
			this.codeButtonM.Name = "M";
			this.codeButtonM.PreferredSize = new Size(36, 22);
			this.codeButtonM.Margins = new Margins(10+17, 2, 0, 3);
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

				TextField field = new TextField(processorPanel);
				field.Name = name;
				field.IsReadOnly = true;
				field.PreferredWidth = MyWidgets.TextFieldHexa.GetHexaWidth(this.processor.GetRegisterSize(name));
				field.Margins = new Margins(0, 2, 0, 0);
				field.Dock = DockStyle.Left;

				this.codeRegisters.Add(field);
				first = false;
			}
		}

		protected void CreateQuickPanel(MyWidgets.Panel parent)
		{
			//	Crée le panneau de gauche vide, pour le mode rapide.
			MyWidgets.Panel panel = this.CreatePanelWithTitle(parent, "Rapide");
			panel.PreferredHeight = 400;
			panel.Dock = DockStyle.Bottom;

			StaticText label = new StaticText(panel);
			label.Text = "<i>Vide</i>";
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

			this.memoryButtonM = new MyWidgets.PushButton(header);
			this.memoryButtonM.Text = "RAM";
			this.memoryButtonM.Name = "M";
			this.memoryButtonM.PreferredSize = new Size(36, 22);
			this.memoryButtonM.Margins = new Margins(10+17, 2, 0, 3);
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
			this.buttonRun.Text = "<font size=\"200%\"><b>R/S</b></font>";
			this.buttonRun.PreferredSize = new Size(50, 50);
			this.buttonRun.Margins = new Margins(0, 0, 10, 0);
			this.buttonRun.Dock = DockStyle.Top;
			this.buttonRun.Clicked += new MessageEventHandler(this.HandleButtonRunClicked);
			ToolTip.Default.SetToolTip(this.buttonRun, "Run/Stop");

			this.buttonClock5 = new MyWidgets.PushButton(parent);
			this.buttonClock5.Index = 100000;
			this.buttonClock5.Text = "100'000";
			this.buttonClock5.PreferredSize = new Size(50, 24);
			this.buttonClock5.Margins = new Margins(0, 0, 10, 0);
			this.buttonClock5.Dock = DockStyle.Top;
			this.buttonClock5.Clicked += new MessageEventHandler(this.HandleButtonClockClicked);
			ToolTip.Default.SetToolTip(this.buttonClock5, "100'000 instructions/seconde");

			this.buttonClock4 = new MyWidgets.PushButton(parent);
			this.buttonClock4.Index = 10000;
			this.buttonClock4.Text = "10'000";
			this.buttonClock4.PreferredSize = new Size(50, 24);
			this.buttonClock4.Margins = new Margins(0, 0, 2, 0);
			this.buttonClock4.Dock = DockStyle.Top;
			this.buttonClock4.Clicked += new MessageEventHandler(this.HandleButtonClockClicked);
			ToolTip.Default.SetToolTip(this.buttonClock4, "10'000 instructions/seconde");

			this.buttonClock3 = new MyWidgets.PushButton(parent);
			this.buttonClock3.Index = 1000;
			this.buttonClock3.Text = "1'000";
			this.buttonClock3.PreferredSize = new Size(50, 24);
			this.buttonClock3.Margins = new Margins(0, 0, 2, 0);
			this.buttonClock3.Dock = DockStyle.Top;
			this.buttonClock3.Clicked += new MessageEventHandler(this.HandleButtonClockClicked);
			ToolTip.Default.SetToolTip(this.buttonClock3, "1'000 instructions/seconde");

			this.buttonClock2 = new MyWidgets.PushButton(parent);
			this.buttonClock2.Index = 100;
			this.buttonClock2.Text = "100";
			this.buttonClock2.PreferredSize = new Size(50, 24);
			this.buttonClock2.Margins = new Margins(0, 0, 2, 0);
			this.buttonClock2.Dock = DockStyle.Top;
			this.buttonClock2.Clicked += new MessageEventHandler(this.HandleButtonClockClicked);
			ToolTip.Default.SetToolTip(this.buttonClock2, "100 instructions/seconde");

			this.buttonClock1 = new MyWidgets.PushButton(parent);
			this.buttonClock1.Index = 10;
			this.buttonClock1.Text = "10";
			this.buttonClock1.PreferredSize = new Size(50, 24);
			this.buttonClock1.Margins = new Margins(0, 0, 2, 0);
			this.buttonClock1.Dock = DockStyle.Top;
			this.buttonClock1.Clicked += new MessageEventHandler(this.HandleButtonClockClicked);
			ToolTip.Default.SetToolTip(this.buttonClock1, "10 instructions/seconde");

			MyWidgets.Panel stepLabels = this.CreateSwitchHorizonalLabels(parent, "CONT", "STEP");
			stepLabels.Margins = new Margins(0, 0, 6, 0);
			stepLabels.Dock = DockStyle.Top;

			this.switchStep = new MyWidgets.Switch(parent);
			this.switchStep.PreferredSize = new Size(50, 20);
			this.switchStep.Margins = new Margins(0, 0, 0, 5);
			this.switchStep.Dock = DockStyle.Top;
			this.switchStep.Clicked += new MessageEventHandler(this.HandleSwitchStepClicked);
			ToolTip.Default.SetToolTip(this.switchStep, "Mode Continuous ou Step");

			this.buttonStep = new MyWidgets.PushButton(parent);
			this.buttonStep.Text = "<font size=\"200%\"><b>S</b></font>";
			this.buttonStep.PreferredSize = new Size(50, 50);
			this.buttonStep.Margins = new Margins(0, 0, 0, 0);
			this.buttonStep.Dock = DockStyle.Top;
			this.buttonStep.Clicked += new MessageEventHandler(this.HandleButtonStepClicked);
			ToolTip.Default.SetToolTip(this.buttonStep, "Step");

			MyWidgets.Panel intoLabels = this.CreateSwitchHorizonalLabels(parent, "OVER", "INTO");
			intoLabels.Margins = new Margins(0, 0, 6, 0);
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
			ToolTip.Default.SetToolTip(this.buttonMemoryRead, "Memory read");

			this.buttonMemoryWrite = new MyWidgets.PushButton(this.clockBusPanel);
			this.buttonMemoryWrite.Text = "<font size=\"200%\"><b>W</b></font>";
			this.buttonMemoryWrite.PreferredSize = new Size(50, 50);
			this.buttonMemoryWrite.Margins = new Margins(0, 0, 0, 0);
			this.buttonMemoryWrite.Dock = DockStyle.Top;
			this.buttonMemoryWrite.Pressed += new MessageEventHandler(this.HandleButtonMemoryPressed);
			this.buttonMemoryWrite.Released += new MessageEventHandler(this.HandleButtonMemoryReleased);
			ToolTip.Default.SetToolTip(this.buttonMemoryWrite, "Memory write");

			this.UpdateClockButtons();
		}

		protected MyWidgets.Led CreateLabeledLed(MyWidgets.Panel parent, string text)
		{
			//	Crée une led dans un gros panneau avec un texte explicatif.
			MyWidgets.Panel panel = new MyWidgets.Panel(parent);

			panel.BackColor = Color.FromBrightness(0.8);
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
			panel.MinWidth = 400;
			panel.MaxWidth = 400;
			panel.BackColor = Color.FromBrightness(0.8);
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
			this.pageProgramm = new TabPage();
			this.pageProgramm.TabTitle = "Programme";

			HToolBar toolbar = new HToolBar(this.pageProgramm);
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

			this.fieldProgrammRem = new TextFieldMulti(this.pageProgramm);
			this.fieldProgrammRem.Text = DolphinApplication.ProgrammEmptyRem;
			this.fieldProgrammRem.Dock = DockStyle.Fill;
			this.fieldProgrammRem.TextChanged += new EventHandler(this.HandleFieldProgrammRemTextChanged);

			this.book.Items.Add(this.pageProgramm);

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

			this.book.ActivePage = this.pageProgramm;
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

			MyWidgets.Panel mode = new MyWidgets.Panel(display);
			mode.PreferredWidth = 60;
			mode.Margins = new Margins(0, 12, 0, 0);
			mode.Dock = DockStyle.Right;

			this.displayButtonMode = new MyWidgets.PushButton(mode);
			this.displayButtonMode.Text = "DISPLAY";
			this.displayButtonMode.PreferredSize = new Size(60, 24);
			this.displayButtonMode.Dock = DockStyle.Bottom;
			this.displayButtonMode.Clicked += new MessageEventHandler(this.HandleDisplayButtonModeClicked);
			ToolTip.Default.SetToolTip(this.displayButtonMode, "Montre ou cache l'écran bitmap");

			MyWidgets.Panel techno = new MyWidgets.Panel(display);
			techno.PreferredWidth = 40;
			techno.Margins = new Margins(0, 2, 0, 0);
			techno.Dock = DockStyle.Right;

			this.displayButtonTechno = new MyWidgets.PushButton(techno);
			this.displayButtonTechno.PreferredSize = new Size(40, 24);
			this.displayButtonTechno.Dock = DockStyle.Bottom;
			this.displayButtonTechno.Clicked += new MessageEventHandler(this.HandleDisplayButtonTechnoClicked);
			ToolTip.Default.SetToolTip(this.displayButtonTechno, "Type de l'écran bitmap");

			MyWidgets.Panel cls = new MyWidgets.Panel(display);
			cls.PreferredWidth = 40;
			cls.Margins = new Margins(0, 2, 0, 0);
			cls.Dock = DockStyle.Right;

			this.displayButtonCls = new MyWidgets.PushButton(cls);
			this.displayButtonCls.Text = "CLS";
			this.displayButtonCls.PreferredSize = new Size(40, 24);
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
			int t=0;
			for (int y=0; y<2; y++)
			{
				for (int x=0; x<5; x++)
				{
					int index = DolphinApplication.KeyboardIndex[t++];

					MyWidgets.PushButton button = new MyWidgets.PushButton(lines[y]);
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

					this.keyboardButtons.Add(button);
				}
			}

			this.UpdateDisplayMode();
			this.UpdateKeyboard();
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


		protected void UpdateButtons()
		{
			//	Met à jour le mode enable/disable de tous les boutons.
			this.buttonStep.Enable = (this.buttonRun.ActiveState == ActiveState.Yes) && (this.switchStep.ActiveState == ActiveState.Yes);
			//?this.switchInto.Enable = (this.buttonRun.ActiveState == ActiveState.Yes) && (this.switchStep.ActiveState == ActiveState.Yes);

			bool run = (this.buttonRun.ActiveState == ActiveState.Yes);

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
			
			//?bool enable = (this.switchStep.ActiveState == ActiveState.No);
			//?this.buttonClock5.Enable = enable;
			//?this.buttonClock4.Enable = enable;
			//?this.buttonClock3.Enable = enable;
			//?this.buttonClock2.Enable = enable;
			//?this.buttonClock1.Enable = enable;
		}

		protected void UpdateClockButtons()
		{
			//	Met à jour les boutons pour la fréquence de l'horloge.
			this.buttonClock5.ActiveState = (this.ips == 100000) ? ActiveState.Yes : ActiveState.No;
			this.buttonClock4.ActiveState = (this.ips ==  10000) ? ActiveState.Yes : ActiveState.No;
			this.buttonClock3.ActiveState = (this.ips ==   1000) ? ActiveState.Yes : ActiveState.No;
			this.buttonClock2.ActiveState = (this.ips ==    100) ? ActiveState.Yes : ActiveState.No;
			this.buttonClock1.ActiveState = (this.ips ==     10) ? ActiveState.Yes : ActiveState.No;
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
				this.programmFilename.Text = null;
			}
			else
			{
				this.programmFilename.Text = System.IO.Path.GetFileNameWithoutExtension(this.filename);
			}
		}

		protected void UpdateMemoryBank()
		{
			//	Met à jour la banque mémoire utilisée.
			this.memoryButtonM.ActiveState = (this.memoryAccessor.Bank == "M") ? ActiveState.Yes : ActiveState.No;
			this.memoryButtonR.ActiveState = (this.memoryAccessor.Bank == "R") ? ActiveState.Yes : ActiveState.No;
			this.memoryButtonP.ActiveState = (this.memoryAccessor.Bank == "P") ? ActiveState.Yes : ActiveState.No;
			this.memoryButtonD.ActiveState = (this.memoryAccessor.Bank == "D") ? ActiveState.Yes : ActiveState.No;

			this.codeButtonM.ActiveState = (this.codeAccessor.Bank == "M") ? ActiveState.Yes : ActiveState.No;
			this.codeButtonR.ActiveState = (this.codeAccessor.Bank == "R") ? ActiveState.Yes : ActiveState.No;
		}

		protected void UpdatePanelMode()
		{
			//	Met à jour le panneau choisi.
			this.buttonModeBus.ActiveState    = (this.panelMode == "Bus"   ) ? ActiveState.Yes : ActiveState.No;
			this.buttonModeDetail.ActiveState = (this.panelMode == "Detail") ? ActiveState.Yes : ActiveState.No;
			this.buttonModeCode.ActiveState   = (this.panelMode == "Code"  ) ? ActiveState.Yes : ActiveState.No;
			this.buttonModeQuick.ActiveState  = (this.panelMode == "Quick" ) ? ActiveState.Yes : ActiveState.No;

			this.leftPanelBus.Visibility    = (this.panelMode == "Bus");
			this.clockBusPanel.Visibility   = (this.panelMode == "Bus");
			this.leftPanelDetail.Visibility = (this.panelMode == "Detail");
			this.leftPanelCode.Visibility   = (this.panelMode == "Code");
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

		protected void UpdateKeyboard()
		{
			//	Met à jour les inscriptions sur le clavier émulé.
			bool shift = this.SearchKey(0x08).ActiveState == ActiveState.Yes;
			bool ctrl  = this.SearchKey(0x10).ActiveState == ActiveState.Yes;

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

		protected void MarkPC(int pc)
		{
			//	Montre les données pointées par le PC.
			if (this.panelMode == "Detail")
			{
				this.memoryAccessor.MarkPC = pc;
			}

			if (this.panelMode == "Code")
			{
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
			else
			{
				if (button.ActiveState == ActiveState.Yes)
				{
					keys |= button.Index;
				}
				else
				{
					keys &= ~button.Index;
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
				this.clock.AutoRepeat = 1.0/System.Math.Min(this.ips, DolphinApplication.RealMaxIps);
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

			this.breakAddress = -1;
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

			if (this.breakAddress != -1)
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
				int value = this.processor.GetRegisterValue(field.Name);
				int bitCount = this.processor.GetRegisterSize(field.Name);
				field.Text = MyWidgets.TextFieldHexa.GetHexaText(value, bitCount);
			}

			this.MarkPC(pc);
			this.UpdateMemoryBank();
		}
		#endregion

		#region Serialization
		protected bool DlgOpenFilename()
		{
			//	Demande le nom du fichier à ouvrir.
			Common.Dialogs.FileOpen dlg = new Common.Dialogs.FileOpen();
			dlg.Title = "Ouverture d'un programme";

			if (this.firstOpenSaveDialog)
			{
				dlg.InitialDirectory = this.OriginalSamplesPath;
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
			dlg.Title = "Enregistrement d'un programme";

			if (this.firstOpenSaveDialog && string.IsNullOrEmpty(this.filename))
			{
				dlg.InitialDirectory = this.OriginalSamplesPath;
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

		protected void New()
		{
			//	Efface le programme en cours.
			if (this.AutoSave())
			{
				this.fieldProgrammRem.Text = DolphinApplication.ProgrammEmptyRem;
				this.Stop();
				this.memory.ClearRam();
				this.filename = "";
				this.Dirty = false;
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
				err = "Impossible de lire le fichier.";
			}

			if (data != null)
			{
				err = this.Deserialize(data);
				if (!string.IsNullOrEmpty(err))
				{
					this.filename = null;
				}
			}

			if (this.fieldProgrammRem.Text != DolphinApplication.ProgrammEmptyRem)
			{
				this.book.ActivePage = this.pageProgramm;
			}

			this.Dirty = false;
			this.ProcessorReset();
			this.AddressBits = 0;
			this.DataBits = 0;
			this.UpdateData();
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
				if (value == true && this.memory.IsEmptyRam)
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

			if (this.fieldProgrammRem.Text != DolphinApplication.ProgrammEmptyRem)
			{
				writer.WriteElementString("Rem", this.fieldProgrammRem.Text);
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
			this.fieldProgrammRem.Text = DolphinApplication.ProgrammEmptyRem;

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
					else if (name == "MemoryData")
					{
						string element = reader.ReadElementString();
						this.memory.PutContent(element);
						reader.Read();
					}
					else if (name == "Rem")
					{
						string element = reader.ReadElementString();
						this.fieldProgrammRem.Text = element;
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
			this.MarkPC(this.processor.GetRegisterValue("PC"));
			this.Dirty = true;
		}

		private void HandleClockTimeElapsed(object sender)
		{
			//	Le timer demande d'exécuter l'instruction suivante.
			if (this.ips > DolphinApplication.RealMaxIps)
			{
				int count = (int) (this.ips/DolphinApplication.RealMaxIps);
				for (int i=0; i<count; i++)
				{
					if (this.ProcessorClock())
					{
						this.ProcessorBreakOut();
						return;
					}
				}
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
				this.MarkPC(pc);
				this.UpdateMemoryBank();
			}
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

			this.codeButtonAdd.Enable = (address != -1 && !isReadOnly);
			this.codeButtonSub.Enable = (address != -1 && !isReadOnly);
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

		private void HandleKeyboardButtonPressed(object sender, MessageEventArgs e)
		{
			//	Touche du clavier simulé pressée.
			MyWidgets.PushButton button = sender as MyWidgets.PushButton;

			if (button.Index == 0x08 || button.Index == 0x10)  // shift ou ctrl ?
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
				this.fieldProgrammRem.TextNavigator.SelectionBold = !this.fieldProgrammRem.TextNavigator.SelectionBold;
			}

			if (button.Name == "FontItalic")
			{
				this.fieldProgrammRem.TextNavigator.SelectionItalic = !this.fieldProgrammRem.TextNavigator.SelectionItalic;
			}

			if (button.Name == "FontUnderline")
			{
				this.fieldProgrammRem.TextNavigator.SelectionUnderline = !this.fieldProgrammRem.TextNavigator.SelectionUnderline;
			}

			this.Dirty = true;
		}

		private void HandleFieldProgrammRemTextChanged(object sender)
		{
			//	Le commentaire lié au programme est changé.
			this.Dirty = true;
		}
		#endregion


		public static readonly double MainWidth  = 800;
		public static readonly double MainHeight = 600;
		public static readonly double MainMargin = 6;

		protected static readonly double RealMaxIps = 20;
		public static readonly string ApplicationTitle = "Dauphin  ̶  Simulateur de microprocesseur";
		protected static readonly string ProgrammEmptyRem = "<br/><i>Tapez ici les commentaires sur le programme...</i>";


		protected Common.Support.ResourceManagerPool	resourceManagerPool;
		protected MyWidgets.MainPanel					mainPanel;
		protected MyWidgets.Panel						leftPanelBus;
		protected MyWidgets.Panel						leftPanelDetail;
		protected MyWidgets.Panel						leftPanelCode;
		protected MyWidgets.Panel						leftPanelQuick;
		protected MyWidgets.Panel						clockBusPanel;
		protected MyWidgets.Panel						helpPanel;
		protected IconButton							buttonNew;
		protected IconButton							buttonOpen;
		protected IconButton							buttonSave;
		protected IconButton							buttonAbout;
		protected StaticText							programmFilename;
		protected MyWidgets.PushButton					buttonModeBus;
		protected MyWidgets.PushButton					buttonModeDetail;
		protected MyWidgets.PushButton					buttonModeCode;
		protected MyWidgets.PushButton					buttonModeQuick;
		protected MyWidgets.PushButton					buttonRun;
		protected MyWidgets.PushButton					buttonStep;
		protected MyWidgets.PushButton					buttonMemoryRead;
		protected MyWidgets.PushButton					buttonMemoryWrite;
		protected MyWidgets.PushButton					buttonClock5;
		protected MyWidgets.PushButton					buttonClock4;
		protected MyWidgets.PushButton					buttonClock3;
		protected MyWidgets.PushButton					buttonClock2;
		protected MyWidgets.PushButton					buttonClock1;
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
		protected List<MyWidgets.Digit>					displayDigits;
		protected MyWidgets.Display						displayBitmap;
		protected List<MyWidgets.PushButton>			keyboardButtons;
		protected MyWidgets.PushButton					buttonReset;
		protected List<MyWidgets.TextFieldHexa>			registerFields;
		protected MyWidgets.MemoryAccessor				memoryAccessor;
		protected MyWidgets.PushButton					memoryButtonM;
		protected MyWidgets.PushButton					memoryButtonR;
		protected MyWidgets.PushButton					memoryButtonP;
		protected MyWidgets.PushButton					memoryButtonD;
		protected MyWidgets.CodeAccessor				codeAccessor;
		protected MyWidgets.PushButton					codeButtonM;
		protected MyWidgets.PushButton					codeButtonR;
		protected MyWidgets.PushButton					codeButtonAdd;
		protected MyWidgets.PushButton					codeButtonSub;
		protected List<TextField>						codeRegisters;
		protected TabBook								book;
		protected TabPage								pageProgramm;
		protected TextFieldMulti						fieldProgrammRem;

		protected Components.Memory						memory;
		protected Components.AbstractProcessor			processor;
		protected int									breakAddress;
		protected Timer									clock;
		protected double								ips;
		protected string								panelMode;
		protected string								filename;
		protected bool									firstOpenSaveDialog;
		protected bool									dirty;
		protected bool									ignoreChange;
	}
}
