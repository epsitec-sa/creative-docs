using System.Collections.Generic;
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
		}

		public void CreateLayout()
		{
			this.mainPanel = new Panel(this.parentWindow.Root);
			this.mainPanel.BackColor = Color.FromBrightness(0.7);
			this.mainPanel.DrawFullFrame = true;
			this.mainPanel.DrawScrew = true;
			this.mainPanel.PreferredSize = new Size(DolphinApplication.MainWidth, DolphinApplication.MainHeight);
			this.mainPanel.Margins = new Margins(10, 10, 10, 10);
			this.mainPanel.Padding = new Margins(14, 14, 14, 14);
			this.mainPanel.Anchor = AnchorStyles.TopLeft;

			Panel panelTitle = new Panel(this.mainPanel);
			panelTitle.BackColor = Color.FromBrightness(0.9);
			panelTitle.DrawFullFrame = true;
			panelTitle.PreferredHeight = 40;
			panelTitle.Margins = new Margins(0, 0, 0, 10);
			panelTitle.Dock = DockStyle.Top;

			StaticText title = new StaticText(panelTitle);
			title.Text = "<font size=\"200%\"><b>Dolphin microprocessor emulator </b></font>by EPSITEC";
			title.ContentAlignment = ContentAlignment.MiddleCenter;
			title.Dock = DockStyle.Fill;

			Panel all = new Panel(this.mainPanel);
			all.Dock = DockStyle.Fill;

			this.leftPanel = new Panel(all);
			this.leftPanel.BackColor = Color.FromBrightness(0.9);
			this.leftPanel.DrawFullFrame = true;
			this.leftPanel.PreferredWidth = 500;
			this.leftPanel.Padding = new Margins(10, 10, 10, 10);
			this.leftPanel.Dock = DockStyle.Left;

			this.rightPanel = new Panel(all);
			this.rightPanel.BackColor = Color.FromBrightness(0.9);
			this.rightPanel.DrawFullFrame = true;
			this.rightPanel.Margins = new Margins(10, 0, 0, 0);
			this.rightPanel.Padding = new Margins(10, 10, 10, 10);
			this.rightPanel.Dock = DockStyle.Fill;

			this.CreateBusPanel(this.leftPanel);
			this.CreateKeyboardDisplay(this.rightPanel);
		}


		protected void CreateBusPanel(Panel parent)
		{
			Panel left = new Panel(parent);
			left.PreferredWidth = 50-10;
			left.Margins = new Margins(0, 10, 0, 0);
			left.Dock = DockStyle.Left;

			Panel right = new Panel(parent);
			right.Dock = DockStyle.Fill;

			//	Partie supérieure gauche pour le contrôle des bus.
			this.buttonReset = new PushButton(left);
			this.buttonReset.Text = "<font size=\"200%\"><b>R/S</b></font>";
			this.buttonReset.PreferredSize = new Size(50, 50);
			this.buttonReset.Margins = new Margins(0, 0, 10, 5);
			this.buttonReset.Dock = DockStyle.Top;
			this.buttonReset.Clicked += new MessageEventHandler(this.HandleButtonResetClicked);
			ToolTip.Default.SetToolTip(this.buttonReset, "Run/Stop");

			this.ledRun = this.CreateLabeledLed(left, "Run");

			Panel stepLabels = this.CreateSwitchHorizonalLabels(left, "C", "S");
			stepLabels.Margins = new Margins(0, 0, 10, 0);
			stepLabels.Dock = DockStyle.Top;

			this.switchStep = new Switch(left);
			this.switchStep.PreferredSize = new Size(50, 20);
			this.switchStep.Margins = new Margins(0, 0, 0, 5);
			this.switchStep.Dock = DockStyle.Top;
			this.switchStep.Clicked += new MessageEventHandler(this.HandleSwitchStepClicked);
			ToolTip.Default.SetToolTip(this.switchStep, "Mode Continus ou Step");

			this.buttonStep = new PushButton(left);
			this.buttonStep.Text = "<font size=\"200%\"><b>S</b></font>";
			this.buttonStep.PreferredSize = new Size(50, 50);
			this.buttonStep.Margins = new Margins(0, 0, 0, 10);
			this.buttonStep.Dock = DockStyle.Top;
			this.buttonStep.Clicked += new MessageEventHandler(this.HandleButtonStepClicked);
			ToolTip.Default.SetToolTip(this.buttonStep, "Step");

			//	Partie inférieure gauche pour le contrôle des bus.
			this.buttonMemory = new PushButton(left);
			this.buttonMemory.Text = "<font size=\"200%\"><b>M</b></font>";
			this.buttonMemory.PreferredSize = new Size(50, 50);
			this.buttonMemory.Margins = new Margins(0, 0, 0, 10);
			this.buttonMemory.Dock = DockStyle.Bottom;
			this.buttonMemory.Pressed += new MessageEventHandler(this.HandleButtonMemoryPressed);
			this.buttonMemory.Released += new MessageEventHandler(this.HandleButtonMemoryReleased);
			ToolTip.Default.SetToolTip(this.buttonMemory, "Memory access");

			this.switchDataReadWrite = new Switch(left);
			this.switchDataReadWrite.PreferredSize = new Size(50, 20);
			this.switchDataReadWrite.Margins = new Margins(0, 0, 0, 5);
			this.switchDataReadWrite.Dock = DockStyle.Bottom;
			this.switchDataReadWrite.Clicked += new MessageEventHandler(this.HandleSwitchDataReadWriteClicked);
			ToolTip.Default.SetToolTip(this.switchDataReadWrite, "Mode Read ou Write");

			Panel rwLabels = this.CreateSwitchHorizonalLabels(left, "R", "W");
			rwLabels.Dock = DockStyle.Bottom;

			//	Partie de droite pour les bus.
			Panel top, bottom;
			this.CreateBitsPanel(right, out top, out bottom, "<font size=\"150%\"><b>Data bus</b></font>");

			this.dataDigits = new List<Digit>();
			for (int i=0; i<DolphinApplication.TotalData/4; i++)
			{
				this.CreateBitDigit(top, i, this.dataDigits);
			}

			this.dataLeds = new List<Led>();
			this.dataSwitchs = new List<Switch>();
			for (int i=0; i<DolphinApplication.TotalData; i++)
			{
				this.CreateBitButton(bottom, i, DolphinApplication.TotalData, this.dataLeds, this.dataSwitchs);
				this.dataSwitchs[i].Clicked += new MessageEventHandler(this.HandleDataSwitchClicked);
			}

			//	Panneau des adresses.
			this.CreateBitsPanel(right, out top, out bottom, "<font size=\"150%\"><b>Address bus</b></font>");

			this.addressDigits = new List<Digit>();
			for (int i=0; i<DolphinApplication.TotalAddress/4; i++)
			{
				this.CreateBitDigit(top, i, this.addressDigits);
			}
			
			this.addressLeds = new List<Led>();
			this.addressSwitchs = new List<Switch>();
			for (int i=0; i<DolphinApplication.TotalAddress; i++)
			{
				this.CreateBitButton(bottom, i, DolphinApplication.TotalAddress, this.addressLeds, this.addressSwitchs);
				this.addressSwitchs[i].Clicked += new MessageEventHandler(this.HandleAddressSwitchClicked);
			}

			this.AddressBits = 0;
			this.DataBits = 0;
			this.UpdateButtons();
		}

		protected Led CreateLabeledLed(Panel parent, string text)
		{
			//	Crée une led dans un gros panneau avec un texte explicatif.
			Panel panel = new Panel(parent);

			panel.BackColor = Color.FromBrightness(0.8);
			panel.DrawFullFrame = true;
			//?panel.DrawScrew = true;
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
			Panel panel = new Panel(parent);

			panel.BackColor = Color.FromBrightness(0.8);
			panel.DrawFullFrame = true;
			panel.DrawScrew = true;
			panel.PreferredHeight = 195;
			panel.Padding = new Margins(10, 10, 2, 12);
			panel.Margins = new Margins(10, 10, 10, 10);
			panel.Dock = DockStyle.Bottom;

			StaticText label = new StaticText(panel);
			label.Text = title;
			label.ContentAlignment = ContentAlignment.TopCenter;
			label.PreferredHeight = 30;
			label.Margins = new Margins(0, 0, 0, 0);
			label.Dock = DockStyle.Top;

			Line sep = new Line(panel);
			sep.PreferredHeight = 1;
			sep.Margins = new Margins(0, 0, 0, 5);
			sep.Dock = DockStyle.Top;
			
			top = new Panel(panel);
			top.PreferredHeight = 50;
			top.Padding = new Margins(0, 20, 0, 5);
			top.Dock = DockStyle.Top;
			
			bottom = new Panel(panel);
			bottom.Dock = DockStyle.Fill;

			this.CreateSwitchVerticalLabels(bottom, "<b>0</b>", "<b>1</b>");
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


		protected void CreateKeyboardDisplay(Panel parent)
		{
			//	Crée le clavier et l'affichage simulé, dans la partie de droite.
			List<Panel> lines = new List<Panel>();
			for (int y=0; y<2; y++)
			{
				Panel keyboard = new Panel(parent);
				keyboard.PreferredHeight = 50;
				keyboard.Margins = new Margins(0, 0, 2, 2);
				keyboard.Dock = DockStyle.Bottom;

				lines.Add(keyboard);
			}

			Panel display = new Panel(parent);
			display.PreferredHeight = 60;
			display.Margins = new Margins(0, 0, 0, 20);
			display.Dock = DockStyle.Bottom;

			//	Crée les digits de l'affichage.
			this.displayDigits = new List<Digit>();
			for (int i=0; i<4; i++)
			{
				Digit digit = new Digit(display);
				digit.PreferredWidth = 40;
				digit.Margins = new Margins(1, 1, 0, 0);
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
					else if (index == 0x08)
					{
						xmlText = "<b>Shift</b>";
					}
					else if (index == 0x10)
					{
						xmlText = "<b>Ctrl</b>";
					}

					PushButton button = new PushButton(lines[y]);
					button.Text = xmlText;
					button.Index = index;
					button.PreferredWidth = 50;
					button.Margins = new Margins(2, 2, 0, 0);
					button.Dock = DockStyle.Left;
					button.Pressed += new MessageEventHandler(this.HandleKeyboardButtonPressed);
					button.Released += new MessageEventHandler(this.HandleKeyboardButtonReleased);

					this.keyboardButtons.Add(button);
				}
			}
		}

		protected static int[] KeyboardIndex =
		{
			0x08, 0x00, 0x01, 0x02, 0x03,  // Shift, 0..3
			0x10, 0x04, 0x05, 0x06, 0x07,  // Ctrl,  4..7
		};



		protected void UpdateButtons()
		{
			//	Met à jour le mode enable/disable de tous les boutons.
			this.buttonStep.Enable = (this.ledRun.ActiveState == ActiveState.Yes) && (this.switchStep.ActiveState == ActiveState.Yes);

			bool run = (this.ledRun.ActiveState == ActiveState.Yes);

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


		/// <summary>
		/// Gestion de la mémoire du système emulé.
		/// </summary>
		public class Memory
		{
			public Memory(DolphinApplication application)
			{
				//	Alloue et initialise la mémoire du dauphin.
				this.application = application;

				int size = 1 << DolphinApplication.TotalAddress;
				this.memory = new byte[size];

				for (int i=0; i<size; i++)
				{
					this.memory[i] = 0;
				}
			}

			public int Length
			{
				//	Retourne la longueur de la mémoire.
				get
				{
					return this.memory.Length;
				}
			}

			public int Read(int address)
			{
				//	Lit une valeur en mémoire et/ou dans un périphérique.
				if (address >= 0 && address < this.memory.Length)  // adresse valide ?
				{
					int value = this.memory[address];

					if (address == DolphinApplication.PeriphBase+DolphinApplication.PeriphKeyboard)  // lecture du clavier ?
					{
						if ((value & 0x80) != 0)  // bit full ?
						{
							this.memory[address] = (byte) (value & ~0x87);
						}
					}

					return value;
				}
				else  // hors de l'espace d'adressage ?
				{
					return 0xff;
				}
			}

			public void Write(int address, int data)
			{
				//	Ecrit une valeur en mémoire et/ou dans un périphérique.
				if (address >= 0 && address < this.memory.Length)  // adresse valide ?
				{
					this.memory[address] = (byte) data;
				}

				if ((address & DolphinApplication.PeriphBase) != 0)  // périphérique ?
				{
					int periph = address & ~DolphinApplication.PeriphBase;

					if (periph >= DolphinApplication.PeriphFirstDigit && periph <= DolphinApplication.PeriphLastDigit)  // l'un des 4 digits ?
					{
						int t = DolphinApplication.PeriphLastDigit-DolphinApplication.PeriphFirstDigit;
						this.application.displayDigits[t-periph].SegmentValue = (Digit.DigitSegment) this.memory[address];
					}
				}
			}

			protected DolphinApplication application;
			protected byte[] memory;
		}

		public void KeyboardChanged(PushButton button, bool pressed)
		{
			//	Appelé lorsqu'une touche du clavier simulé a été pressée ou relâchée.
			int keys = this.memory.Read(DolphinApplication.PeriphBase+DolphinApplication.PeriphKeyboard);

			if (button.Index < 0x08)
			{
				if (pressed)
				{
					keys &= ~0x87;
					keys |= button.Index;
					keys |= 0x80;  // bit full
				}
			}
			else
			{
				if (pressed)
				{
					keys |= button.Index;
				}
				else
				{
					keys &= ~button.Index;
				}
			}

			this.memory.Write(DolphinApplication.PeriphBase+DolphinApplication.PeriphKeyboard, keys);
		}


		#region Processor
		protected void ProcessorReset()
		{
			//	Reset du processeur pour démarrer à l'adresse 0.
			this.processor.Reset();
		}

		protected void ProcessorStart()
		{
			//	Démarre le processeur.
			if (this.switchStep.ActiveState == ActiveState.No)  // continue ?
			{
				if (this.clock == null)
				{
					this.clock = new Timer();
					this.clock.AutoRepeat = 0.001;  // 1000 instructions/seconde
					this.clock.TimeElapsed += new EventHandler(this.HandleClockTimeElapsed);
					this.clock.Start();
				}
			}
			else  // step ?
			{
				this.ProcessorFeedback();
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
			this.ProcessorFeedback();
			this.processor.Clock();
		}

		protected void ProcessorFeedback()
		{
			//	Feedback visuel du processeur sur les bus.
			int pc = this.processor.GetRegisterValue("PC");
			this.AddressBits = pc;
			this.DataBits = this.memory.Read(pc);
		}
		#endregion


		private void HandleClockTimeElapsed(object sender)
		{
			//	Le timer demande d'exécuter l'instruction suivante.
			this.ProcessorClock();
		}

		private void HandleButtonResetClicked(object sender, MessageEventArgs e)
		{
			//	Bouton [R/S] cliqué.
			if (this.ledRun.ActiveState == ActiveState.No)
			{
				this.ProcessorReset();
				this.ProcessorStart();
				
				this.ledRun.ActiveState = ActiveState.Yes;
			}
			else
			{
				this.ProcessorStop();

				this.AddressBits = this.AddressBits;
				this.DataBits = 0;
				
				this.ledRun.ActiveState = ActiveState.No;
			}

			this.UpdateButtons();
		}

		private void HandleButtonStepClicked(object sender, MessageEventArgs e)
		{
			//	Bouton [S] cliqué.
			this.ProcessorClock();
			this.ProcessorFeedback();
		}

		private void HandleSwitchStepClicked(object sender, MessageEventArgs e)
		{
			//	Switch C/S basculé.
			this.switchStep.ActiveState = (this.switchStep.ActiveState == ActiveState.No) ? ActiveState.Yes : ActiveState.No;
			this.UpdateButtons();

			if (this.ledRun.ActiveState == ActiveState.Yes)
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
			if (this.ledRun.ActiveState == ActiveState.Yes)
			{
				return;
			}

			if (this.switchDataReadWrite.ActiveState == ActiveState.No)  // read ?
			{
				this.DataBits = this.memory.Read(this.AddressBits);
			}
			else  // write ?
			{
				this.memory.Write(this.AddressBits, this.DataBits);
				this.DataBits = this.memory.Read(this.AddressBits);
			}
		}

		private void HandleButtonMemoryReleased(object sender, MessageEventArgs e)
		{
			//	Bouton [M] relâché.
			if (this.ledRun.ActiveState == ActiveState.Yes)
			{
				return;
			}

			this.DataBits = 0;
		}

		private void HandleAddressSwitchClicked(object sender, MessageEventArgs e)
		{
			//	Switch d'adresse basculé.
			if (this.ledRun.ActiveState == ActiveState.Yes)
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
			if (this.ledRun.ActiveState == ActiveState.Yes)
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

		private void HandleKeyboardButtonPressed(object sender, MessageEventArgs e)
		{
			//	Touche du clavier simulé pressée.
			PushButton button = sender as PushButton;
			this.KeyboardChanged(button, true);
		}

		private void HandleKeyboardButtonReleased(object sender, MessageEventArgs e)
		{
			//	Touche du clavier simulé relâchée.
			PushButton button = sender as PushButton;
			this.KeyboardChanged(button, false);
		}


		public static readonly double MainWidth = 830;
		public static readonly double MainHeight = 600;

		public static readonly int TotalAddress = 12;
		public static readonly int TotalData = 8;

		protected static readonly int PeriphBase = (1 << (DolphinApplication.TotalAddress-1));
		protected static readonly int PeriphFirstDigit = 0;
		protected static readonly int PeriphLastDigit = 3;
		protected static readonly int PeriphKeyboard = 7;

		protected Window parentWindow;
		protected Panel mainPanel;
		protected Panel leftPanel;
		protected Panel rightPanel;
		protected PushButton buttonReset;
		protected PushButton buttonStep;
		protected PushButton buttonMemory;
		protected Led ledRun;
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

		protected Memory memory;
		protected AbstractProcessor processor;
		protected Timer clock;
	}
}
