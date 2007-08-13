//	Copyright © 2003-2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.App.Dolphin.MyWidgets
{
	/// <summary>
	/// Permet d'afficher et de modifier les codes des instructions.
	/// </summary>
	public class CodeAccessor : Widget
	{
		public CodeAccessor() : base()
		{
			this.scroller = new VScroller(this);
			this.scroller.IsInverted = true;
			this.scroller.Dock = DockStyle.Left;
			this.scroller.ValueChanged += new EventHandler(this.HandleScrollerValueChanged);

			this.panel = new MyWidgets.Panel(this);
			this.panel.Dock = DockStyle.Fill;

			this.fields = new List<MyWidgets.Code>();
			this.instructionAddresses = new List<int>();
		}

		public CodeAccessor(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.scroller.ValueChanged -= new EventHandler(this.HandleScrollerValueChanged);

				foreach (MyWidgets.Code field in this.fields)
				{
					field.InstructionChanged -= new EventHandler(this.HandleFieldInstructionChanged);
				}
			}

			base.Dispose(disposing);
		}



		public Components.AbstractProcessor Processor
		{
			//	Processeur émulé affichée/modifée par ce widget.
			get
			{
				return this.processor;
			}
			set
			{
				this.processor = value;
			}
		}

		public Components.Memory Memory
		{
			//	Mémoire émulée affichée/modifée par ce widget.
			get
			{
				return this.memory;
			}
			set
			{
				this.memory = value;
				this.firstAddress = 0;
				this.markPC = -1;

				this.UpdateData();
				this.UpdateScroller();
				this.UpdateMarkPC();
			}
		}

		public int FirstAddress
		{
			//	Indique la première adresse affichée (relative dans la banque).
			get
			{
				return this.firstAddress;
			}
			set
			{
				value = System.Math.Max(value, 0);
				value = System.Math.Min(value, this.MemoryLength-this.fields.Count);
				value = this.AdjustAddress(value);

				if (this.firstAddress != value)
				{
					this.firstAddress = value;

					this.UpdateTable();
					this.UpdateMarkPC();
					this.scroller.Value = (decimal) this.GetInstructionIndex(this.firstAddress);
				}
			}
		}

		public string Bank
		{
			//	Choix de la banque affichée.
			get
			{
				return this.bank;
			}
			set
			{
				this.bank = value;
				this.firstAddress = 0;

				this.UpdateData();
				this.UpdateScroller();
				this.UpdateMarkPC();
			}
		}

		public int MarkPC
		{
			//	Indique l'adresse pointée par le registre PC.
			get
			{
				return this.markPC;
			}
			set
			{
				if (this.markPC != value)
				{
					this.markPC = value;

					int firstIndex = this.GetInstructionIndex(this.firstAddress);
					int lastAddress = 0;
					if (firstIndex+this.fields.Count < this.instructionAddresses.Count)
					{
						lastAddress = this.instructionAddresses[firstIndex+this.fields.Count];
					}
					else
					{
						lastAddress = this.MemoryLength;
					}

					if (this.markPC < this.MemoryStart+this.firstAddress || this.markPC >= this.MemoryStart+lastAddress)
					{
						string newBank = this.MemoryBank(this.markPC);
						if (this.bank == newBank)
						{
							this.FirstAddress = this.markPC - this.MemoryStart;
						}
						else
						{
							this.bank = newBank;

							this.firstAddress = this.markPC - this.MemoryStart;
							if (this.firstAddress > this.MemoryLength-this.fields.Count)
							{
								this.firstAddress = this.MemoryLength-this.fields.Count;
							}

							this.UpdateData();

							this.ignoreChange = true;
							this.UpdateScroller();
							this.ignoreChange = false;

							this.UpdateMarkPC();
						}
					}
					else
					{
						this.UpdateTable();
						this.UpdateMarkPC();
					}
				}
			}
		}


		protected override void UpdateClientGeometry()
		{
			//	Met à jour la géométrie.
			base.UpdateClientGeometry();

			int total = (int) (this.Client.Bounds.Height/(CodeAccessor.LineHeight+1));
			total = System.Math.Max(total, 1);
			if (this.fields.Count != total)
			{
				this.CreateFields(total);

				this.UpdateTable();
				this.UpdateScroller();
				this.UpdateMarkPC();
			}
		}

		private void CreateFields(int total)
		{
			//	Crée tous les champs éditables, en fonction de la hauteur du widget.
			this.fields.Clear();
			this.panel.Children.Clear();

			int index = 200;
			for (int i=0; i<total; i++)
			{
				MyWidgets.Code field = new MyWidgets.Code(this.panel);
				field.Index = i;
				field.SetTabIndex(index++);
				field.Processor = this.processor;
				field.CodeAccessor = this;
				field.PreferredHeight = CodeAccessor.LineHeight;
				field.Margins = new Margins(0, 0, 0, 1);
				field.Dock = DockStyle.Top;
				field.InstructionChanged += new EventHandler(this.HandleFieldInstructionChanged);

				this.fields.Add(field);
			}
		}


		protected void UpdateScroller()
		{
			//	Met à jour l'ascenseur.
			if (this.fields.Count == 0)
			{
				return;
			}

			this.scroller.MinValue = (decimal) 0;
			this.scroller.MaxValue = (decimal) (this.instructionAddresses.Count - this.fields.Count);
			this.scroller.Value = (decimal) this.GetInstructionIndex(this.firstAddress);
			this.scroller.VisibleRangeRatio = (decimal) System.Math.Max((double) this.fields.Count/this.instructionAddresses.Count, 0.1);  // évite cabine trop petite
			this.scroller.LargeChange = (decimal) this.fields.Count;
			this.scroller.SmallChange = (decimal) 1;
		}

		public void UpdateData()
		{
			//	Met à jour la table des instructions.
			this.UpdateInstructionAddresses();
			this.UpdateTable();
		}

		protected void UpdateTable()
		{
			//	Met à jour la table des instructions.
			if (this.processor == null || this.memory == null)
			{
				return;
			}

			int address = this.MemoryStart+this.firstAddress;
			for (int i=0; i<this.fields.Count; i++)
			{
				MyWidgets.Code field = this.fields[i];

				int code = this.memory.ReadForDebug(address);
				int length = this.processor.GetInstructionLength(code);

				List<int> codes = new List<int>();
				for (int c=0; c<length; c++)
				{
					codes.Add(this.memory.ReadForDebug(address+c));
				}

				field.SetCode(address, codes);

				address += length;
			}
		}

		protected void UpdateInstructionAddresses()
		{
			//	Met à jour la table des adresses des instructions.
			if (this.processor == null || this.memory == null)
			{
				return;
			}

			this.instructionAddresses.Clear();

			int address = this.MemoryStart;
			while (address < this.MemoryStart+this.MemoryLength)
			{
				this.instructionAddresses.Add(address-this.MemoryStart);

				int code = this.memory.ReadForDebug(address);
				address += this.processor.GetInstructionLength(code);
			}
		}

		protected int AdjustAddress(int address)
		{
			//	Retourne une adresse (relative dans la banque) ajustée pour commencer sur un début d'instruction.
			for (int i=1; i<this.instructionAddresses.Count; i++)
			{
				if (address < this.instructionAddresses[i])
				{
					return this.instructionAddresses[i-1];
				}
			}

			return address;
		}

		protected int GetInstructionIndex(int address)
		{
			//	Retourne l'index de l'instruction en fonction de son adresse (relative dans la banque).
			for (int i=1; i<this.instructionAddresses.Count; i++)
			{
				if (address < this.instructionAddresses[i])
				{
					return i-1;
				}
			}

			return 0;
		}

		protected void UpdateMarkPC()
		{
			if (this.memory == null)
			{
				return;
			}

			int address = this.MemoryStart+this.firstAddress;
			for (int i=0; i<this.fields.Count; i++)
			{
				int code = this.memory.ReadForDebug(address);
				int length = this.processor.GetInstructionLength(code);

				Color color = Color.Empty;

				if (address == this.markPC)
				{
					color = Color.FromRgb(1, 0, 0);
				}

				MyWidgets.Code field = this.fields[i];
				field.BackColor = color;

				address += length;
			}
		}


		protected string MemoryBank(int address)
		{
			//	Retourne la banque à utiliser pour une adresse donnée.
			if (address >= Components.Memory.RamBase && address < Components.Memory.RamBase+Components.Memory.RamLength)
			{
				return "M";
			}

			if (address >= Components.Memory.RomBase && address < Components.Memory.RomBase+Components.Memory.RomLength)
			{
				return "R";
			}

			if (address >= Components.Memory.PeriphBase && address < Components.Memory.PeriphBase+Components.Memory.PeriphLength)
			{
				return "P";
			}
			
			if (address >= Components.Memory.DisplayBase && address < Components.Memory.DisplayBase+Components.Memory.DisplayLength)
			{
				return "D";
			}
			
			return null;
		}

		protected int MemoryLength
		{
			get
			{
				if (this.bank == "M")
				{
					return Components.Memory.RamLength;
				}
				else if (this.bank == "R")
				{
					return Components.Memory.RomLength;
				}
				else if (this.bank == "P")
				{
					return Components.Memory.PeriphLength;
				}
				else if (this.bank == "D")
				{
					return Components.Memory.DisplayLength;
				}
				else
				{
					return this.memory.Length;
				}
			}
		}

		protected int MemoryStart
		{
			get
			{
				if (this.bank == "M")
				{
					return Components.Memory.RamBase;
				}
				else if (this.bank == "R")
				{
					return Components.Memory.RomBase;
				}
				else if (this.bank == "P")
				{
					return Components.Memory.PeriphBase;
				}
				else if (this.bank == "D")
				{
					return Components.Memory.DisplayBase;
				}
				else
				{
					return 0;
				}
			}
		}


		private void HandleScrollerValueChanged(object sender)
		{
			if (this.ignoreChange)
			{
				return;
			}

			int v = (int) System.Math.Floor(this.scroller.Value+0.5M);
			int address = 0;
			if (v >= 0 && v < this.instructionAddresses.Count)
			{
				address = this.instructionAddresses[v];
			}

			this.FirstAddress = address;
		}

		private void HandleFieldInstructionChanged(object sender)
		{
			MyWidgets.Code field = sender as MyWidgets.Code;

			int address = this.MemoryStart+this.firstAddress+field.Index;
			//?this.memory.WriteWithDirty(address, field.HexaValue);
		}


		static protected readonly double LineHeight = 20;

		protected Components.AbstractProcessor		processor;
		protected Components.Memory					memory;
		protected string							bank;
		protected VScroller							scroller;
		protected MyWidgets.Panel					panel;
		protected List<MyWidgets.Code>				fields;
		protected int								firstAddress;  // relatif dans la banque
		protected int								markPC;
		protected List<int>							instructionAddresses;
		protected bool								ignoreChange;
	}
}
