//	Copyright © 2003-2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.App.Dolphin.MyWidgets
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

			this.panel = new MyWidgets.Panel(this);
			this.panel.Dock = DockStyle.Fill;

			this.fields = new List<MyWidgets.TextFieldHexa>();

			this.followPC = true;
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

				foreach (MyWidgets.TextFieldHexa field in this.fields)
				{
					field.HexaValueChanged -= new EventHandler(this.HandleFieldHexaValueChanged);
				}
			}

			base.Dispose(disposing);
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
				this.markPC = Misc.undefined;

				this.UpdateScroller();
				this.UpdateData();
				this.UpdateMarkPC();
			}
		}

		public bool IsDeferUpdateData
		{
			//	Indique s'il faut différer UpdateData, pendant l'exécution d'un programme.
			get
			{
				return this.isDeferUpdateData;
			}
			set
			{
				if (this.isDeferUpdateData != value)
				{
					this.isDeferUpdateData = value;

					if (!this.isDeferUpdateData)
					{
						this.UpdateData();
					}
				}
			}
		}

		public bool FollowPC
		{
			get
			{
				return this.followPC;
			}
			set
			{
				this.followPC = value;
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

		public void DirtyMarkPC()
		{
			//	Force le prochain MarkPC à faire son travail.
			this.markPC = Misc.undefined;
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

					if (this.followPC && (this.markPC < this.MemoryStart+this.firstAddress || this.markPC >= this.MemoryStart+this.firstAddress+this.fields.Count))
					{
						string newBank = Components.Memory.BankSearch(this.markPC);
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
						//?this.UpdateData();
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
			//	Crée tous les champs éditables, en fonction de la hauteur du widget.
			this.fields.Clear();
			this.panel.Children.Clear();

			int index = 200;
			for (int i=0; i<total; i++)
			{
				MyWidgets.TextFieldHexa field = new MyWidgets.TextFieldHexa(this.panel);
				field.Index = i;
				field.SetTabIndex(index++);
				field.MemoryAccessor = this;
				field.BitCount = Components.Memory.TotalData;
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
			//	Met à jour l'ascenseur.
			if (this.fields.Count == 0)
			{
				return;
			}

			if (this.firstAddress >= 0)
			{
				this.scroller.MinValue = (decimal) 0;
				this.scroller.MaxValue = (decimal) (this.MemoryLength - this.fields.Count);
				this.scroller.Value = (decimal) this.firstAddress;
				this.scroller.VisibleRangeRatio = (decimal) System.Math.Max((double) this.fields.Count/this.MemoryLength, 0.2);  // évite cabine trop petite
				this.scroller.LargeChange = (decimal) this.fields.Count;
				this.scroller.SmallChange = (decimal) 1;
				this.scroller.Enable = true;
			}
			else
			{
				this.scroller.Enable = false;
			}
		}

		public void UpdateData()
		{
			if (this.isDeferUpdateData)
			{
				return;
			}

			if (this.memory == null)
			{
				return;
			}

			for (int i=0; i<this.fields.Count; i++)
			{
				int address = this.MemoryStart+this.firstAddress+i;

				MyWidgets.TextFieldHexa field = this.fields[i];
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
				this.fields[i].IsBackHilite = (this.MemoryStart+this.firstAddress+i == this.markPC);
			}
		}


		protected int MemoryStart
		{
			get
			{
				return Components.Memory.BankStart(this.bank);
			}
		}

		protected int MemoryLength
		{
			get
			{
				return Components.Memory.BankLength(this.bank);
			}
		}


		protected override void ProcessMessage(Message message, Point pos)
		{
			if (message.MessageType == MessageType.MouseWheel)
			{
				int address = this.firstAddress;
				address += (message.Wheel > 0) ? -2 : 2;
				this.FirstAddress = address;
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
			MyWidgets.TextFieldHexa field = sender as MyWidgets.TextFieldHexa;

			int address = this.MemoryStart+this.firstAddress+field.Index;
			this.memory.WriteWithDirty(address, field.HexaValue);
		}


		static protected readonly double LineHeight = 20;

		protected Components.Memory					memory;
		protected string							bank;
		protected VScroller							scroller;
		protected MyWidgets.Panel					panel;
		protected List<MyWidgets.TextFieldHexa>		fields;
		protected int								firstAddress;  // relatif dans la banque
		protected int								markPC;
		protected bool								followPC;
		protected bool								isDeferUpdateData;
		protected bool								ignoreChange;
	}
}
