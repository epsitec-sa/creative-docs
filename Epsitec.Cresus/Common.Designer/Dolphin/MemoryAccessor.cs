using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Dolphin
{
	/// <summary>
	/// Permet d'afficher et de modifier de la mémoire émulée.
	/// </summary>
	public class MemoryAccessor : Widget
	{
		public MemoryAccessor() : base()
		{
			this.scroller = new VScroller(this);
			this.scroller.IsInverted = true;
			this.scroller.Dock = DockStyle.Left;
			this.scroller.ValueChanged += new EventHandler(this.HandleScrollerValueChanged);

			this.panel = new Panel(this);
			this.panel.Dock = DockStyle.Fill;

			this.fields = new List<TextFieldHexa>();
		}

		public MemoryAccessor(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.scroller.ValueChanged -= new EventHandler(this.HandleScrollerValueChanged);

				foreach (TextFieldHexa field in this.fields)
				{
					field.HexaValueChanged -= new EventHandler(this.HandleFieldHexaValueChanged);
				}
			}

			base.Dispose(disposing);
		}



		public DolphinApplication.Memory Memory
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

				this.UpdateScroller();
				this.UpdateData();
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
				if (this.firstAddress != value)
				{
					this.firstAddress = value;

					this.UpdateData();
					this.UpdateMarkPC();
					this.scroller.Value = (decimal) this.firstAddress;
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

				this.UpdateScroller();
				this.UpdateData();
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

					if (this.markPC < this.MemoryStart+this.firstAddress || this.markPC >= this.MemoryStart+this.firstAddress+this.fields.Count)
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

							this.ignoreChange = true;
							this.UpdateScroller();
							this.ignoreChange = false;

							this.UpdateData();
							this.UpdateMarkPC();
						}
					}
					else
					{
						this.UpdateData();
						this.UpdateMarkPC();
					}
				}
			}
		}


		protected override void UpdateClientGeometry()
		{
			//	Met à jour la géométrie.
			base.UpdateClientGeometry();

			int total = (int) (this.Client.Bounds.Height/(MemoryAccessor.LineHeight+1));
			total = System.Math.Max(total, 1);
			if (this.fields.Count != total)
			{
				this.CreateFields(total);

				this.UpdateScroller();
				this.UpdateData();
				this.UpdateMarkPC();
			}
		}

		private void CreateFields(int total)
		{
			this.fields.Clear();
			this.panel.Children.Clear();

			int index = 200;
			for (int i=0; i<total; i++)
			{
				TextFieldHexa field = new TextFieldHexa(this.panel);
				field.Index = i;
				field.SetTabIndex(index++);
				field.BitCount = DolphinApplication.TotalData;
				field.BitNames = null;
				field.PreferredHeight = MemoryAccessor.LineHeight;
				field.Margins = new Margins(0, 0, 0, 1);
				field.Dock = DockStyle.Top;
				field.HexaValueChanged += new EventHandler(this.HandleFieldHexaValueChanged);

				this.fields.Add(field);
			}
		}


		protected void UpdateScroller()
		{
			if (this.fields.Count == 0)
			{
				return;
			}

			this.scroller.MinValue = (decimal) 0;
			this.scroller.MaxValue = (decimal) (this.MemoryLength - this.fields.Count);
			this.scroller.Value = (decimal) this.firstAddress;
			this.scroller.VisibleRangeRatio = (decimal) System.Math.Max((double) this.fields.Count/this.MemoryLength, 0.2);  // évite cabine trop petite
			this.scroller.LargeChange = (decimal) this.fields.Count;
			this.scroller.SmallChange = (decimal) 1;
		}

		public void UpdateData()
		{
			if (this.memory == null)
			{
				return;
			}

			for (int i=0; i<this.fields.Count; i++)
			{
				int address = this.MemoryStart+this.firstAddress+i;

				TextFieldHexa field = this.fields[i];
				field.Enable = !this.memory.IsReadOnly(address);
				field.Label = address.ToString("X3");
				field.HexaValue = this.memory.ReadForDebug(address);
			}
		}

		protected void UpdateMarkPC()
		{
			if (this.memory == null)
			{
				return;
			}

			for (int i=0; i<this.fields.Count; i++)
			{
				Color color = Color.Empty;

				if (this.MemoryStart+this.firstAddress+i == this.markPC)
				{
					color = Color.FromRgb(1, 0, 0);
				}

				TextFieldHexa field = this.fields[i];
				field.BackColor = color;
			}
		}


		protected string MemoryBank(int address)
		{
			//	Retourne la banque à utiliser pour une adresse donnée.
			if (address >= DolphinApplication.RamBase && address < DolphinApplication.RamBase+DolphinApplication.RamLength)
			{
				return "M";
			}

			if (address >= DolphinApplication.PeriphBase && address < DolphinApplication.PeriphBase+DolphinApplication.PeriphLength)
			{
				return "P";
			}
			
			if (address >= DolphinApplication.RomBase && address < DolphinApplication.RomBase+DolphinApplication.RomLength)
			{
				return "R";
			}

			return null;
		}

		protected int MemoryLength
		{
			get
			{
				if (this.bank == "M")
				{
					return DolphinApplication.RamLength;
				}
				else if (this.bank == "P")
				{
					return DolphinApplication.PeriphLength;
				}
				else if (this.bank == "R")
				{
					return DolphinApplication.RomLength;
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
					return DolphinApplication.RamBase;
				}
				else if (this.bank == "P")
				{
					return DolphinApplication.PeriphBase;
				}
				else if (this.bank == "R")
				{
					return DolphinApplication.RomBase;
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

			this.FirstAddress = (int) System.Math.Floor(this.scroller.Value+0.5M);
		}

		private void HandleFieldHexaValueChanged(object sender)
		{
			TextFieldHexa field = sender as TextFieldHexa;

			int address = this.MemoryStart+this.firstAddress+field.Index;
			this.memory.WriteWithDirty(address, field.HexaValue);
		}


		static protected readonly double LineHeight = 20;

		protected DolphinApplication.Memory memory;
		protected string bank;
		protected VScroller scroller;
		protected Panel panel;
		protected List<TextFieldHexa> fields;
		protected int firstAddress;  // relatif dans la banque
		protected int markPC;
		protected bool ignoreChange;
	}
}
