using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Dolphin
{
	/// <summary>
	/// Fenêtre principale de l'éditeur de ressources.
	/// </summary>
	public class DolphinApplication
	{
		public DolphinApplication(Window parentWindow)
		{
			this.parentWindow = parentWindow;

			this.memory = new Memory(this);
			this.processor = new ProcessorGeneric(this.memory);
			this.memory.RomInitialise(this.processor);
			this.ips = 1000;
		}

		public void CreateLayout()
		{
			this.mainPanel = new MainPanel(this.parentWindow.Root);
			this.mainPanel.BackColor = Color.FromBrightness(0.7);
			this.mainPanel.DrawFullFrame = true;
			this.mainPanel.DrawScrew = true;
			this.mainPanel.MinSize = new Size(DolphinApplication.MainWidth, DolphinApplication.MainHeight);
			this.mainPanel.MaxSize = new Size(DolphinApplication.MainWidth, DolphinApplication.MainHeight);
			this.mainPanel.PreferredSize = new Size(DolphinApplication.MainWidth, DolphinApplication.MainHeight);
			this.mainPanel.Margins = new Margins(DolphinApplication.MainMargin, DolphinApplication.MainMargin, DolphinApplication.MainMargin, DolphinApplication.MainMargin);
			this.mainPanel.Padding = new Margins(14, 14, 14, 14);
			this.mainPanel.Dock = DockStyle.Fill;

			Panel panelTitle = new Panel(this.mainPanel);
			panelTitle.BackColor = Color.FromBrightness(0.9);
			panelTitle.DrawFullFrame = true;
			panelTitle.PreferredHeight = 40;
			panelTitle.Margins = new Margins(0, 0, 0, 10);
			panelTitle.Dock = DockStyle.Top;

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
			title.Text = "<font size=\"200%\"><b>Dolphin microprocessor emulator </b></font>by EPSITEC";
			title.ContentAlignment = ContentAlignment.MiddleCenter;
			title.Margins = new Margins(0, 100, 0, 0);
			title.Dock = DockStyle.Fill;

			Panel all = new Panel(this.mainPanel);
			all.Dock = DockStyle.Fill;

			//	Crée les deux grandes parties gauche/droite.
			Panel leftPanel = new Panel(all);
			leftPanel.BackColor = Color.FromBrightness(0.9);
			leftPanel.DrawFullFrame = true;
			leftPanel.PreferredWidth = 510;
			leftPanel.Padding = new Margins(0, 0, 10, 10);
			leftPanel.Dock = DockStyle.Left;

			Panel rightPanel = new Panel(all);
			rightPanel.Margins = new Margins(10, 0, 0, 0);
			rightPanel.Dock = DockStyle.Fill;

			//	Crée les 3 parties de gauche.
			Panel leftHeader = new Panel(leftPanel);
			leftHeader.Margins = new Margins(0, 0, 0, 5);
			leftHeader.Dock = DockStyle.Top;

			Line sep = new Line(leftPanel);
			sep.PreferredHeight = 1;
			sep.Margins = new Margins(0, 0, 0, 3);
			sep.Dock = DockStyle.Top;

			Panel leftClock = new Panel(leftPanel);
			leftClock.PreferredWidth = 50-10;
			leftClock.Margins = new Margins(10, 0, 0, 0);
			leftClock.Dock = DockStyle.Left;

			this.leftPanelBus = new Panel(leftPanel);
			this.leftPanelBus.Dock = DockStyle.Fill;

			this.leftPanelDetail = new Panel(leftPanel);
			this.leftPanelDetail.Dock = DockStyle.Fill;
			this.leftPanelDetail.Visibility = false;

			this.leftPanelQuick = new Panel(leftPanel);
			this.leftPanelQuick.Dock = DockStyle.Fill;
			this.leftPanelQuick.Visibility = false;

			//	Crée les 2 parties de droite.
			this.helpPanel = new Panel(rightPanel);
			this.helpPanel.Margins = new Margins(0, 0, 0, 10);
			this.helpPanel.Dock = DockStyle.Fill;

			Panel kdPanel = new Panel(rightPanel);
			kdPanel.BackColor = Color.FromBrightness(0.9);
			kdPanel.DrawFullFrame = true;
			kdPanel.DrawScrew = true;
			kdPanel.PreferredHeight = 100;  // minimum qui sera étendu
			kdPanel.Margins = new Margins(0, 0, 0, 0);
			kdPanel.Padding = new Margins(12, 0, 10, 10);
			kdPanel.Dock = DockStyle.Bottom;

			//	Crée le contenu des différentes parties.
			this.CreateOptions(leftHeader);
			this.CreateClockControl(leftClock);
			this.CreateBusPanel(this.leftPanelBus);
			this.CreateDetailPanel(this.leftPanelDetail);
			this.CreateQuickPanel(this.leftPanelQuick);
			this.CreateHelp(this.helpPanel);
			this.CreateKeyboardDisplay(kdPanel);

			this.ProcessorFeedback();
			this.UpdateSave();
		}

		public bool Quit()
		{
			//	Appelé avant de quitter l'application.
			//	Retourne true s'il est possible de quitter.
			return this.AutoSave();
		}

		public MemoryAccessor MemoryAccessor
		{
			get
			{
				return this.memoryAccessor;
			}
		}

		public List<Digit> DisplayDigits
		{
			get
			{
				return this.displayDigits;
			}
		}


		protected void CreateOptions(Panel parent)
		{
			//	Crée la partie supérieure de panneau de gauche.
			RadioButton radio;

			radio = new RadioButton(parent);
			radio.Text = "Panneau de contrôle";
			radio.Name = "Bus";
			radio.Group = "Option";
			radio.ActiveState = ActiveState.Yes;
			radio.Margins = new Margins(60+10, 0, 0, 0);
			radio.PreferredWidth = 140;
			radio.Dock = DockStyle.Left;
			radio.Clicked += new MessageEventHandler(this.HandleOptionRadioClicked);
			this.panelMode = radio.Name;

			radio = new RadioButton(parent);
			radio.Text = "Intérieur des circuits";
			radio.Name = "Detail";
			radio.Group = "Option";
			radio.PreferredWidth = 140;
			radio.Dock = DockStyle.Left;
			radio.Clicked += new MessageEventHandler(this.HandleOptionRadioClicked);

			radio = new RadioButton(parent);
			radio.Text = "Rien (rapide)";
			radio.Name = "Quick";
			radio.Group = "Option";
			radio.PreferredWidth = 100;
			radio.Dock = DockStyle.Left;
			radio.Clicked += new MessageEventHandler(this.HandleOptionRadioClicked);
		}

		protected void CreateBusPanel(Panel parent)
		{
			//	Crée le panneau de gauche complet avec les bus.
			Panel top, bottom;
			this.CreateBitsPanel(parent, out top, out bottom, "Data bus");

			this.dataDigits = new List<Digit>();
			for (int i=0; i<Memory.TotalData/4; i++)
			{
				this.CreateBitDigit(top, i, this.dataDigits);
			}

			this.dataLeds = new List<Led>();
			this.dataSwitchs = new List<Switch>();
			for (int i=0; i<Memory.TotalData; i++)
			{
				this.CreateBitButton(bottom, i, Memory.TotalData, this.dataLeds, this.dataSwitchs);
				this.dataSwitchs[i].Clicked += new MessageEventHandler(this.HandleDataSwitchClicked);
			}

			//	Panneau des adresses.
			this.CreateBitsPanel(parent, out top, out bottom, "Address bus");

			this.addressDigits = new List<Digit>();
			for (int i=0; i<Memory.TotalAddress/4; i++)
			{
				this.CreateBitDigit(top, i, this.addressDigits);
			}
			
			this.addressLeds = new List<Led>();
			this.addressSwitchs = new List<Switch>();
			for (int i=0; i<Memory.TotalAddress; i++)
			{
				this.CreateBitButton(bottom, i, Memory.TotalAddress, this.addressLeds, this.addressSwitchs);
				this.addressSwitchs[i].Clicked += new MessageEventHandler(this.HandleAddressSwitchClicked);
			}

			this.AddressBits = 0;
			this.DataBits = 0;
			this.UpdateButtons();
		}

		protected void CreateQuickPanel(Panel parent)
		{
			//	Crée le panneau de gauche vide, pour le mode rapide.
			Panel panel = this.CreatePanelWithTitle(parent, "Rapide");
			panel.PreferredHeight = 400;
			panel.Dock = DockStyle.Bottom;

			StaticText label = new StaticText(panel);
			label.Text = "<i>Vide</i>";
			label.ContentAlignment = ContentAlignment.MiddleCenter;
			label.Dock = DockStyle.Fill;
		}

		protected void CreateDetailPanel(Panel parent)
		{
			//	Crée le panneau de gauche détaillé complet.
			Panel header;
			Panel memoryPanel = this.CreatePanelWithTitle(parent, "Memory", out header);
			memoryPanel.PreferredHeight = 47+21*8;  // place pour 8 adresses
			memoryPanel.Dock = DockStyle.Bottom;

			this.memoryButtonM = new PushButton(header);
			this.memoryButtonM.Text = "RAM";
			this.memoryButtonM.Name = "M";
			this.memoryButtonM.PreferredSize = new Size(36, 22);
			this.memoryButtonM.Margins = new Margins(10+17, 2, 0, 3);
			this.memoryButtonM.Dock = DockStyle.Left;
			this.memoryButtonM.Clicked += new MessageEventHandler(this.HandleMemoryButtonClicked);
			ToolTip.Default.SetToolTip(this.memoryButtonM, "Montre le début de la mémoire vive (RAM)");

			this.memoryButtonR = new PushButton(header);
			this.memoryButtonR.Text = "ROM";
			this.memoryButtonR.Name = "R";
			this.memoryButtonR.PreferredSize = new Size(36, 22);
			this.memoryButtonR.Margins = new Margins(0, 2, 0, 3);
			this.memoryButtonR.Dock = DockStyle.Left;
			this.memoryButtonR.Clicked += new MessageEventHandler(this.HandleMemoryButtonClicked);
			ToolTip.Default.SetToolTip(this.memoryButtonR, "Montre le début de la mémoire morte (ROM)");

			this.memoryButtonP = new PushButton(header);
			this.memoryButtonP.Text = "PER";
			this.memoryButtonP.Name = "P";
			this.memoryButtonP.PreferredSize = new Size(36, 22);
			this.memoryButtonP.Margins = new Margins(0, 2, 0, 3);
			this.memoryButtonP.Dock = DockStyle.Left;
			this.memoryButtonP.Clicked += new MessageEventHandler(this.HandleMemoryButtonClicked);
			ToolTip.Default.SetToolTip(this.memoryButtonP, "Montre le début de la zone des périphériques");

			this.memoryAccessor = new MemoryAccessor(memoryPanel);
			this.memoryAccessor.Memory = this.memory;
			this.memoryAccessor.Bank = "M";
			this.memoryAccessor.Margins = new Margins(10, 10, 0, 0);
			this.memoryAccessor.Dock = DockStyle.Fill;

			//	Partie pour le processeur.
			Panel processorPanel = this.CreatePanelWithTitle(parent, "Microprocessor register");
			processorPanel.PreferredHeight = 10;  // minuscule (sera étendu)
			processorPanel.Dock = DockStyle.Bottom;

			this.registerFields = new List<TextFieldHexa>();
			int index = 100;
			foreach (string name in this.processor.RegisterNames)
			{
				TextFieldHexa field = this.CreateProcessorRegister(processorPanel, name);
				field.Name = name;
				field.BitNames = this.processor.GetRegisterBitNames(name);
				field.SetTabIndex(index++);
				field.Margins = new Margins(10+17, 0, 0, 1);  // laisse la largeur d'un Scroller
				field.Dock = DockStyle.Top;
				field.HexaValueChanged += new EventHandler(this.HandleProcessorHexaValueChanged);

				this.registerFields.Add(field);
			}

			this.UpdateMemoryBank();
		}

		protected TextFieldHexa CreateProcessorRegister(Panel parent, string name)
		{
			//	Crée le widget complexe pour représenter un registre du processeur.
			TextFieldHexa field = new TextFieldHexa(parent);

			field.BitCount = this.processor.GetRegisterSize(name);
			field.Label = name;
			field.PreferredHeight = 20;
			field.Margins = new Margins(0, 0, 0, 1);

			return field;
		}

		protected void CreateClockControl(Panel parent)
		{
			//	Crée les widgets pour le contrôle de l'horloge du processeur (bouton R/S, etc.).
			this.buttonReset = new PushButton(parent);
			this.buttonReset.Text = "<font size=\"200%\"><b>R/S</b></font>";
			this.buttonReset.PreferredSize = new Size(50, 50);
			this.buttonReset.Margins = new Margins(0, 0, 10, 5);
			this.buttonReset.Dock = DockStyle.Top;
			this.buttonReset.Clicked += new MessageEventHandler(this.HandleButtonResetClicked);
			ToolTip.Default.SetToolTip(this.buttonReset, "Run/Stop");

			this.buttonClock3 = new PushButton(parent);
			this.buttonClock3.Index = 1000;
			this.buttonClock3.Text = "1000 IPS";
			this.buttonClock3.PreferredSize = new Size(50, 24);
			this.buttonClock3.Margins = new Margins(0, 0, 10, 0);
			this.buttonClock3.Dock = DockStyle.Top;
			this.buttonClock3.Clicked += new MessageEventHandler(this.HandleButtonClockClicked);
			ToolTip.Default.SetToolTip(this.buttonClock3, "1000 instructions/seconde");

			this.buttonClock2 = new PushButton(parent);
			this.buttonClock2.Index = 100;
			this.buttonClock2.Text = "100 IPS";
			this.buttonClock2.PreferredSize = new Size(50, 24);
			this.buttonClock2.Margins = new Margins(0, 0, 2, 0);
			this.buttonClock2.Dock = DockStyle.Top;
			this.buttonClock2.Clicked += new MessageEventHandler(this.HandleButtonClockClicked);
			ToolTip.Default.SetToolTip(this.buttonClock2, "100 instructions/seconde");

			this.buttonClock1 = new PushButton(parent);
			this.buttonClock1.Index = 10;
			this.buttonClock1.Text = "10 IPS";
			this.buttonClock1.PreferredSize = new Size(50, 24);
			this.buttonClock1.Margins = new Margins(0, 0, 2, 0);
			this.buttonClock1.Dock = DockStyle.Top;
			this.buttonClock1.Clicked += new MessageEventHandler(this.HandleButtonClockClicked);
			ToolTip.Default.SetToolTip(this.buttonClock1, "10 instructions/seconde");

			this.buttonClock0 = new PushButton(parent);
			this.buttonClock0.Index = 1;
			this.buttonClock0.Text = "1 IPS";
			this.buttonClock0.PreferredSize = new Size(50, 24);
			this.buttonClock0.Margins = new Margins(0, 0, 2, 0);
			this.buttonClock0.Dock = DockStyle.Top;
			this.buttonClock0.Clicked += new MessageEventHandler(this.HandleButtonClockClicked);
			ToolTip.Default.SetToolTip(this.buttonClock0, "1 instruction/seconde");

			Panel stepLabels = this.CreateSwitchHorizonalLabels(parent, "C", "S");
			stepLabels.Margins = new Margins(0, 0, 6, 0);
			stepLabels.Dock = DockStyle.Top;

			this.switchStep = new Switch(parent);
			this.switchStep.PreferredSize = new Size(50, 20);
			this.switchStep.Margins = new Margins(0, 0, 0, 5);
			this.switchStep.Dock = DockStyle.Top;
			this.switchStep.Clicked += new MessageEventHandler(this.HandleSwitchStepClicked);
			ToolTip.Default.SetToolTip(this.switchStep, "Mode Continus ou Step");

			this.buttonStep = new PushButton(parent);
			this.buttonStep.Text = "<font size=\"200%\"><b>S</b></font>";
			this.buttonStep.PreferredSize = new Size(50, 50);
			this.buttonStep.Margins = new Margins(0, 0, 0, 10);
			this.buttonStep.Dock = DockStyle.Top;
			this.buttonStep.Clicked += new MessageEventHandler(this.HandleButtonStepClicked);
			ToolTip.Default.SetToolTip(this.buttonStep, "Step");

			//	Partie inférieure gauche pour le contrôle des bus.
			this.clockBusPanel = new Panel(parent);
			this.clockBusPanel.PreferredWidth = 40;
			this.clockBusPanel.Dock = DockStyle.Bottom;

			Panel rwLabels = this.CreateSwitchHorizonalLabels(this.clockBusPanel, "R", "W");
			rwLabels.Dock = DockStyle.Top;

			this.switchDataReadWrite = new Switch(this.clockBusPanel);
			this.switchDataReadWrite.PreferredSize = new Size(50, 20);
			this.switchDataReadWrite.Margins = new Margins(0, 0, 0, 5);
			this.switchDataReadWrite.Dock = DockStyle.Top;
			this.switchDataReadWrite.Clicked += new MessageEventHandler(this.HandleSwitchDataReadWriteClicked);
			ToolTip.Default.SetToolTip(this.switchDataReadWrite, "Mode Read ou Write");

			this.buttonMemory = new PushButton(this.clockBusPanel);
			this.buttonMemory.Text = "<font size=\"200%\"><b>M</b></font>";
			this.buttonMemory.PreferredSize = new Size(50, 50);
			this.buttonMemory.Margins = new Margins(0, 0, 0, 10);
			this.buttonMemory.Dock = DockStyle.Top;
			this.buttonMemory.Pressed += new MessageEventHandler(this.HandleButtonMemoryPressed);
			this.buttonMemory.Released += new MessageEventHandler(this.HandleButtonMemoryReleased);
			ToolTip.Default.SetToolTip(this.buttonMemory, "Memory access");

			this.UpdateClockButtons();
		}

		protected Led CreateLabeledLed(Panel parent, string text)
		{
			//	Crée une led dans un gros panneau avec un texte explicatif.
			Panel panel = new Panel(parent);

			panel.BackColor = Color.FromBrightness(0.8);
			panel.DrawFullFrame = true;
			panel.PreferredWidth = parent.PreferredWidth;
			panel.PreferredHeight = 60;
			panel.Padding = new Margins(0, 0, 10, 5);
			panel.Dock = DockStyle.Top;

			Led led = new Led(panel);
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

		protected void CreateBitsPanel(Panel parent, out Panel top, out Panel bottom, string title)
		{
			//	Crée un panneau recevant des boutons (led + switch) pour des bits.
			Panel panel = this.CreatePanelWithTitle(parent, title);
			panel.Dock = DockStyle.Bottom;
			
			top = new Panel(panel);
			top.PreferredHeight = 50;
			top.Padding = new Margins(0, 20, 0, 5);
			top.Dock = DockStyle.Top;
			
			bottom = new Panel(panel);
			bottom.Dock = DockStyle.Fill;

			this.CreateSwitchVerticalLabels(bottom, "<b>0</b>", "<b>1</b>");
		}

		protected Panel CreatePanelWithTitle(Panel parent, string title)
		{
			Panel header;
			return this.CreatePanelWithTitle(parent, title, out header);
		}

		protected Panel CreatePanelWithTitle(Panel parent, string title, out Panel header)
		{
			//	Crée un panneau avec un titre en haut.
			Panel panel = new Panel(parent);
			panel.MinWidth = 400;
			panel.MaxWidth = 400;
			panel.BackColor = Color.FromBrightness(0.8);
			panel.DrawFullFrame = true;
			panel.DrawScrew = true;
			panel.PreferredHeight = 195;
			panel.Padding = new Margins(0, 0, 4, 12);
			panel.Margins = new Margins(10, 10, 10, 10);

			header = new Panel(panel);
			header.PreferredHeight = 25;
			header.Margins = new Margins(0, 0, 0, 0);
			header.Dock = DockStyle.Top;

			StaticText label = new StaticText(header);
			label.Text = string.Concat("<font size=\"150%\"><b>", title, "</b></font>");
			label.ContentAlignment = ContentAlignment.TopCenter;
			label.PreferredHeight = 25;
			label.Margins = new Margins(0, 0, 0, 0);
			label.Dock = DockStyle.Fill;

			Line sep = new Line(panel);
			sep.PreferredHeight = 1;
			sep.Margins = new Margins(0, 0, 0, 5);
			sep.Dock = DockStyle.Top;

			return panel;
		}

		protected void CreateBitDigit(Panel parent, int rank, List<Digit> digits)
		{
			//	Crée un digit pour un groupe de 4 bits.
			Digit digit = new Digit(parent);
			digit.PreferredWidth = 30;
			digit.Margins = new Margins(37+18, 37, 0, 0);
			digit.Dock = DockStyle.Right;

			digits.Add(digit);
		}

		protected void CreateBitButton(Panel parent, int rank, int total, List<Led> leds, List<Switch> switchs)
		{
			//	Crée un bouton (led + switch) pour un bit.
			Panel group = new Panel(parent);
			group.PreferredSize = new Size(24, 24);
			group.Margins = new Margins(((rank+1)%4 == 0) ? 2+18/2-1:2, 0, 0, 0);
			group.Dock = DockStyle.Right;

			StaticText label = new StaticText(group);
			label.Text = rank.ToString();
			label.ContentAlignment = ContentAlignment.MiddleCenter;
			label.PreferredWidth = 24;
			label.PreferredHeight = 20;
			label.Dock = DockStyle.Top;

			Led state = new Led(group);
			state.Index = rank;
			state.PreferredWidth = 24;
			state.PreferredHeight = 24;
			state.Dock = DockStyle.Top;

			Switch button = new Switch(group);
			button.PreferredWidth = 24;
			button.Margins = new Margins(2, 2, 5, 0);
			button.Dock = DockStyle.Fill;

			if ((rank+1)%4 == 0 && rank < total-1)
			{
				Line sep = new Line(parent);
				sep.PreferredWidth = 1;
				sep.Margins = new Margins(18/2, 0, 0, 0);
				sep.Dock = DockStyle.Right;
			}

			leds.Add(state);
			switchs.Add(button);
		}

		protected void CreateSwitchVerticalLabels(Panel parent, string no, string yes)
		{
			Panel labels = new Panel(parent);
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

		protected Panel CreateSwitchHorizonalLabels(Panel parent, string no, string yes)
		{
			Panel labels = new Panel(parent);
			labels.PreferredHeight = 20;
			labels.PreferredWidth = parent.PreferredWidth;

			StaticText label;

			label = new StaticText(labels);
			label.Text = no;
			label.ContentAlignment = ContentAlignment.MiddleLeft;
			label.PreferredHeight = 20;
			label.PreferredWidth = parent.PreferredWidth/2;
			label.Margins = new Margins(5, 0, 0, 0);
			label.Dock = DockStyle.Left;

			label = new StaticText(labels);
			label.Text = yes;
			label.ContentAlignment = ContentAlignment.MiddleRight;
			label.PreferredHeight = 20;
			label.PreferredWidth = parent.PreferredWidth/2;
			label.Margins = new Margins(0, 5, 0, 0);
			label.Dock = DockStyle.Right;

			return labels;
		}


		protected void CreateHelp(Panel parent)
		{
			//	Crée le panneau pour l'aide.
			this.book = new TabBook(parent);
			this.book.Arrows = TabBookArrows.Stretch;
			this.book.Dock = DockStyle.Fill;

			//	Crée l'onglet pour les commentaires sur le programme.
			this.pageProgramm = new TabPage();
			this.pageProgramm.TabTitle = "Programme";

			this.fieldProgrammRem = new TextFieldMulti(this.pageProgramm);
			this.fieldProgrammRem.Text = DolphinApplication.ProgrammEmptyRem;
			this.fieldProgrammRem.Dock = DockStyle.Fill;

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

		protected void CreateKeyboardDisplay(Panel parent)
		{
			//	Crée le clavier et l'affichage simulé, dans la partie de droite.
			List<Panel> lines = new List<Panel>();
			for (int y=0; y<2; y++)
			{
				Panel keyboard = new Panel(parent);
				keyboard.PreferredHeight = 50;
				keyboard.Margins = new Margins(0, 0, 0, (y==0) ? 10:2);
				keyboard.Dock = DockStyle.Bottom;

				lines.Add(keyboard);
			}

			Panel display = new Panel(parent);
			display.PreferredHeight = 60;
			display.Margins = new Margins(0, 0, 10, 10);
			display.Dock = DockStyle.Bottom;

			//	Crée les digits de l'affichage.
			this.displayDigits = new List<Digit>();
			for (int i=0; i<4; i++)
			{
				Digit digit = new Digit(display);
				digit.PreferredWidth = 40;
				digit.Dock = DockStyle.Left;

				this.displayDigits.Add(digit);
			}

			//	Crée les touches du clavier.
			this.keyboardButtons = new List<PushButton>();
			int t=0;
			for (int y=0; y<2; y++)
			{
				for (int x=0; x<5; x++)
				{
					int index = DolphinApplication.KeyboardIndex[t++];

					string xmlText = null;
					if (index < 0x08)  // touche 0..7 ?
					{
						xmlText = string.Concat("<font size=\"200%\"><b>", index.ToString(), "</b></font>");
					}
					else if (index == 0x10)
					{
						xmlText = "<b>Shift</b>";
					}
					else if (index == 0x20)
					{
						xmlText = "<b>Ctrl</b>";
					}

					PushButton button = new PushButton(lines[y]);
					button.Text = xmlText;
					button.Index = index;
					button.PreferredWidth = 50;
					button.Margins = new Margins(0, 2, 0, 0);
					button.Dock = DockStyle.Left;
					button.Pressed += new MessageEventHandler(this.HandleKeyboardButtonPressed);
					button.Released += new MessageEventHandler(this.HandleKeyboardButtonReleased);

					this.keyboardButtons.Add(button);
				}
			}
		}

		protected static int[] KeyboardIndex =
		{
			0x10, 0x00, 0x01, 0x02, 0x03,  // Shift, 0..3
			0x20, 0x04, 0x05, 0x06, 0x07,  // Ctrl,  4..7
		};

		protected PushButton SearchKey(int index)
		{
			//	Cherche une touche du clavier émulé.
			foreach (PushButton button in this.keyboardButtons)
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
			this.buttonStep.Enable = (this.buttonReset.ActiveState == ActiveState.Yes) && (this.switchStep.ActiveState == ActiveState.Yes);

			bool run = (this.buttonReset.ActiveState == ActiveState.Yes);

			this.switchDataReadWrite.Enable = !run;
			this.buttonMemory.Enable = !run;

			foreach (Switch button in this.addressSwitchs)
			{
				button.Enable = !run;
			}

			foreach (Switch button in this.dataSwitchs)
			{
				button.Enable = !run;
			}
		}

		protected void UpdateClockButtons()
		{
			//	Met à jour les boutons pour la fréquence de l'horloge.
			this.buttonClock3.ActiveState = (this.ips == 1000) ? ActiveState.Yes : ActiveState.No;
			this.buttonClock2.ActiveState = (this.ips ==  100) ? ActiveState.Yes : ActiveState.No;
			this.buttonClock1.ActiveState = (this.ips ==   10) ? ActiveState.Yes : ActiveState.No;
			this.buttonClock0.ActiveState = (this.ips ==    1) ? ActiveState.Yes : ActiveState.No;
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

		protected void SetBits(List<Digit> digits, List<Led> leds, int value)
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

		protected int GetBits(List<Switch> switchs)
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


		public void KeyboardChanged(PushButton button, bool pressed)
		{
			//	Appelé lorsqu'une touche du clavier simulé a été pressée ou relâchée.
			int keys = this.memory.Read(Memory.PeriphKeyboard);

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

			this.memory.Write(Memory.PeriphKeyboard, keys);
		}


		#region Processor
		protected void ProcessorReset()
		{
			//	Reset du processeur pour démarrer à l'adresse 0.
			this.processor.Reset();
			this.ProcessorFeedback();

			this.memory.ClearPeriph();
			this.SearchKey(0x10).ActiveState = ActiveState.No;  // relâche Shift
			this.SearchKey(0x20).ActiveState = ActiveState.No;  // relâche Ctrl
		}

		protected void ProcessorStart()
		{
			//	Démarre le processeur.
			if (this.switchStep.ActiveState == ActiveState.No)  // continue ?
			{
				if (this.clock == null)
				{
					this.clock = new Timer();
					this.ProcessorClockAdjust();
					this.clock.TimeElapsed += new EventHandler(this.HandleClockTimeElapsed);
					this.clock.Start();
				}
			}
			else  // step ?
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
		}

		protected void ProcessorClock()
		{
			//	Exécute une instruction du processeur.
			this.processor.Clock();
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

			foreach (TextFieldHexa field in this.registerFields)
			{
				field.HexaValue = this.processor.GetRegisterValue(field.Label);
			}

			this.memoryAccessor.MarkPC = pc;
			this.UpdateMemoryBank();
		}
		#endregion

		#region Serialization
		protected bool DlgOpenFilename()
		{
			//	Demande le nom du fichier à ouvrir.
			Common.Dialogs.FileOpen dlg = new Common.Dialogs.FileOpen();
			dlg.Title = "Ouverture d'un programme";
			dlg.FileName = "";
			dlg.Filters.Add("dolphin", "Programmes", "*.dolphin");
			dlg.Owner = this.parentWindow;
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
			dlg.FileName = this.filename;
			dlg.Filters.Add("dolphin", "Programmes", "*.dolphin");
			dlg.Owner = this.parentWindow;
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

			string title = "Dolphin";
			string icon = "manifest:Epsitec.Common.Dialogs.Images.Question.icon";

			string message = "Voulez-vous enregistrer le programme ?";

			if (!string.IsNullOrEmpty(this.filename))
			{
				message = string.Format("Voulez-vous enregistrer <b>{0}</b> ?", System.IO.Path.GetFileNameWithoutExtension(this.filename));
			}

			Common.Dialogs.IDialog dialog = Common.Dialogs.MessageDialog.CreateYesNoCancel(title, icon, message, null, null, null);
			dialog.Owner = this.parentWindow;
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
				this.memoryAccessor.UpdateData();
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

				this.Save();
			}

			return true;
		}

		protected void Open()
		{
			//	Ouvre un nouveau programme.
			this.Stop();

			string data = null;
			try
			{
				data = System.IO.File.ReadAllText(this.filename);
			}
			catch
			{
				data = null;
			}

			if (data != null)
			{
				this.Deserialize(data);
			}

			if (this.fieldProgrammRem.Text != DolphinApplication.ProgrammEmptyRem)
			{
				this.book.ActivePage = this.pageProgramm;
			}

			this.Dirty = false;
			this.ProcessorReset();
			this.AddressBits = 0;
			this.DataBits = 0;
			this.memoryAccessor.UpdateData();
			this.UpdateButtons();
			this.UpdateClockButtons();
			this.UpdateFilename();
		}

		protected void Save()
		{
			//	Enregistre le programme en cours.
			string data = this.Serialize();
			try
			{
				System.IO.File.WriteAllText(this.filename, data);
			}
			catch
			{
			}

			this.Dirty = false;
			this.UpdateFilename();
		}

		protected void Stop()
		{
			//	Sroppe le programme en cours.
			this.ProcessorStop();
			this.ProcessorReset();
			this.buttonReset.ActiveState = ActiveState.No;
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

		protected void Deserialize(string data)
		{
			//	Désérialise la vue à partir d'un string de données.
			System.IO.StringReader stringReader = new System.IO.StringReader(data);
			XmlTextReader reader = new XmlTextReader(stringReader);
			
			this.ReadXml(reader);

			reader.Close();
		}

		protected void WriteXml(XmlWriter writer)
		{
			//	Sérialise tout le programme.
			writer.WriteStartDocument();
			writer.WriteStartElement("Dolphin");
			
			writer.WriteElementString("ProcessorName", this.processor.Name);
			writer.WriteElementString("ProcessorIPS", this.ips.ToString(System.Globalization.CultureInfo.InvariantCulture));

			if (this.fieldProgrammRem.Text != DolphinApplication.ProgrammEmptyRem)
			{
				writer.WriteElementString("Rem", this.fieldProgrammRem.Text);
			}

			writer.WriteElementString("MemoryData", this.memory.GetContent());

			writer.WriteEndElement();
			writer.WriteEndDocument();
		}

		protected void ReadXml(XmlReader reader)
		{
			//	Désérialise tout le programme.
			this.memory.ClearRam();
			this.fieldProgrammRem.Text = DolphinApplication.ProgrammEmptyRem;

			reader.Read();
			while (true)
			{
				if (reader.NodeType == XmlNodeType.Element)
				{
					string name = reader.LocalName;
					//?string element = reader.ReadElementString();

					if (name == "ProcessorName")
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
					System.Diagnostics.Debug.Assert(reader.Name == "Dolphin");
					break;
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
				this.Open();
			}
		}

		private void HandleButtonSaveClicked(object sender, MessageEventArgs e)
		{
			//	Bouton enregistrer cliqué.
			if (this.DlgSaveFilename())
			{
				this.Save();
			}
		}

		private void HandleOptionRadioClicked(object sender, MessageEventArgs e)
		{
			//	Bouton pour une option cliqué.
			RadioButton button = sender as RadioButton;
			this.panelMode = button.Name;

			this.leftPanelBus.Visibility    = (this.panelMode == "Bus");
			this.clockBusPanel.Visibility   = (this.panelMode == "Bus");
			this.leftPanelDetail.Visibility = (this.panelMode == "Detail");
			this.leftPanelQuick.Visibility  = (this.panelMode == "Quick");
		}

		private void HandleClockTimeElapsed(object sender)
		{
			//	Le timer demande d'exécuter l'instruction suivante.
			if (this.ips > DolphinApplication.RealMaxIps)
			{
				int count = (int) (this.ips/DolphinApplication.RealMaxIps);
				for (int i=0; i<count; i++)
				{
					this.ProcessorClock();
				}
			}
			else
			{
				this.ProcessorClock();
			}

			this.ProcessorFeedback();
		}

		private void HandleButtonResetClicked(object sender, MessageEventArgs e)
		{
			//	Bouton [R/S] cliqué.
			if (this.buttonReset.ActiveState == ActiveState.No)
			{
				this.ProcessorReset();
				this.ProcessorStart();
				
				this.buttonReset.ActiveState = ActiveState.Yes;
			}
			else
			{
				this.ProcessorStop();

				this.AddressBits = this.AddressBits;
				this.DataBits = 0;
				
				this.buttonReset.ActiveState = ActiveState.No;
			}

			this.UpdateButtons();
		}

		private void HandleButtonStepClicked(object sender, MessageEventArgs e)
		{
			//	Bouton [S] cliqué.
			this.ProcessorClock();
			this.ProcessorFeedback();
		}

		private void HandleButtonClockClicked(object sender, MessageEventArgs e)
		{
			//	Bouton pour choisir la fréquence d'horloge cliqué.
			PushButton button = sender as PushButton;

			this.ips = button.Index;
			this.UpdateClockButtons();
			this.ProcessorClockAdjust();
		}

		private void HandleSwitchStepClicked(object sender, MessageEventArgs e)
		{
			//	Switch C/S basculé.
			this.switchStep.ActiveState = (this.switchStep.ActiveState == ActiveState.No) ? ActiveState.Yes : ActiveState.No;
			this.UpdateButtons();

			if (this.buttonReset.ActiveState == ActiveState.Yes)
			{
				if (this.switchStep.ActiveState == ActiveState.No)  // continue ?
				{
					this.ProcessorStart();
				}
				else  // step ?
				{
					this.ProcessorStop();
					this.ProcessorFeedback();
				}
			}
		}

		private void HandleSwitchDataReadWriteClicked(object sender, MessageEventArgs e)
		{
			//	Switch R/W basculé.
			this.switchDataReadWrite.ActiveState = (this.switchDataReadWrite.ActiveState == ActiveState.No) ? ActiveState.Yes : ActiveState.No;
		}

		private void HandleButtonMemoryPressed(object sender, MessageEventArgs e)
		{
			//	Bouton [M] pressé.
			if (this.buttonReset.ActiveState == ActiveState.Yes)
			{
				return;
			}

			if (this.switchDataReadWrite.ActiveState == ActiveState.No)  // read ?
			{
				this.DataBits = this.memory.Read(this.AddressBits);
			}
			else  // write ?
			{
				this.memory.WriteWithDirty(this.AddressBits, this.DataBits);
				this.DataBits = this.memory.Read(this.AddressBits);
			}
		}

		private void HandleButtonMemoryReleased(object sender, MessageEventArgs e)
		{
			//	Bouton [M] relâché.
			if (this.buttonReset.ActiveState == ActiveState.Yes)
			{
				return;
			}

			this.DataBits = 0;

			//	Nécessaire même en lecture, car la lecture du clavier clear le bit full !
			this.memoryAccessor.UpdateData();
		}

		private void HandleAddressSwitchClicked(object sender, MessageEventArgs e)
		{
			//	Switch d'adresse basculé.
			if (this.buttonReset.ActiveState == ActiveState.Yes)
			{
				return;
			}

			Switch button = sender as Switch;

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
			if (this.buttonReset.ActiveState == ActiveState.Yes)
			{
				return;
			}

			Switch button = sender as Switch;

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
			TextFieldHexa field = sender as TextFieldHexa;
			this.processor.SetRegisterValue(field.Name, field.HexaValue);

			if (field.Name == "PC")
			{
				this.memoryAccessor.MarkPC = this.processor.GetRegisterValue("PC");
				this.UpdateMemoryBank();
			}
		}

		private void HandleMemoryButtonClicked(object sender, MessageEventArgs e)
		{
			//	Bouton [RAM], [ROM] ou [PER] cliqué.
			PushButton button = sender as PushButton;
			this.memoryAccessor.Bank = button.Name;
			this.UpdateMemoryBank();
		}

		private void HandleKeyboardButtonPressed(object sender, MessageEventArgs e)
		{
			//	Touche du clavier simulé pressée.
			PushButton button = sender as PushButton;

			if (button.Index == 0x10 || button.Index == 0x20)  // shift ou ctrl ?
			{
				button.ActiveState = (button.ActiveState == ActiveState.Yes) ? ActiveState.No : ActiveState.Yes;
			}

			this.KeyboardChanged(button, true);
		}

		private void HandleKeyboardButtonReleased(object sender, MessageEventArgs e)
		{
			//	Touche du clavier simulé relâchée.
			PushButton button = sender as PushButton;
			this.KeyboardChanged(button, false);
		}
		#endregion


		public static readonly double MainWidth = 800;
		public static readonly double MainHeight = 600;
		public static readonly double MainMargin = 6;

		protected static readonly double RealMaxIps = 20;
		protected static readonly string ProgrammEmptyRem = "<br/><i>Tapez ici les commentaires sur le programme...</i>";


		protected Window parentWindow;
		protected MainPanel mainPanel;
		protected Panel leftPanelBus;
		protected Panel leftPanelDetail;
		protected Panel leftPanelQuick;
		protected Panel clockBusPanel;
		protected Panel helpPanel;
		protected IconButton buttonNew;
		protected IconButton buttonOpen;
		protected IconButton buttonSave;
		protected StaticText programmFilename;
		protected PushButton buttonReset;
		protected PushButton buttonStep;
		protected PushButton buttonMemory;
		protected PushButton buttonClock3;
		protected PushButton buttonClock2;
		protected PushButton buttonClock1;
		protected PushButton buttonClock0;
		protected Switch switchStep;
		protected Switch switchDataReadWrite;
		protected List<Digit> addressDigits;
		protected List<Led> addressLeds;
		protected List<Switch> addressSwitchs;
		protected List<Digit> dataDigits;
		protected List<Led> dataLeds;
		protected List<Switch> dataSwitchs;
		protected List<Digit> displayDigits;
		protected List<PushButton> keyboardButtons;
		protected List<TextFieldHexa> registerFields;
		protected MemoryAccessor memoryAccessor;
		protected PushButton memoryButtonM;
		protected PushButton memoryButtonR;
		protected PushButton memoryButtonP;
		protected TabBook book;
		protected TabPage pageProgramm;
		protected TextFieldMulti fieldProgrammRem;

		protected Memory memory;
		protected AbstractProcessor processor;
		protected Timer clock;
		protected double ips;
		protected string panelMode;
		protected string filename;
		protected bool dirty;
	}
}
