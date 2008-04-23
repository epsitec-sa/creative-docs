//	Copyright © 2003-2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
			this.instructionAddresses = new List<CodeAddress>();

			this.addressSelected = Misc.undefined;
			this.followPC = true;
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
					field.InstructionSelected -= new EventHandler(this.HandleFieldInstructionSelected);
					field.InstructionDeselected -= new EventHandler(this.HandleFieldInstructionDeselected);
					field.InstructionChanged -= new EventHandler(this.HandleFieldInstructionChanged);
					field.CodeAddressClicked -= new EventHandler(this.HandleCodeAddressClicked);
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
				this.markPC = Misc.undefined;

				if (!string.IsNullOrEmpty(this.bank))
				{
					this.UpdateData();
				}
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

		public int NextFirstAddress(int offset)
		{
			//	Retourne la prochaine première adresse affichée (relative dans la banque).
			int index = this.GetInstructionIndex(this.firstAddress) + offset;

			if (index < 0)
			{
				return 0;
			}
			else
			{
				return this.instructionAddresses[index].Address;
			}
		}

		public int AddressSelected
		{
			//	Adresse sélectionnée.
			get
			{
				return this.addressSelected;
			}
			set
			{
				if (this.addressSelected != value)
				{
					this.addressSelected = value;
					this.OnInstructionSelected();
				}
			}
		}

		public int LengthSelected
		{
			//	Longueur de l'instruction à l'adresse sélectionnée.
			get
			{
				if (this.addressSelected == Misc.undefined)
				{
					return 0;
				}
				else
				{
					return this.GetInstructionLength(this.addressSelected);
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

					if (this.followPC && !this.IsVisibleAddress(this.markPC))
					{
						string newBank = Components.Memory.BankSearch(this.markPC);
						if (newBank == null)
						{
							this.bank = newBank;
							this.FirstAddress = this.MemoryStart;
						}
						else if (this.bank == newBank)
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
						}
					}
					else
					{
						//?this.UpdateTable();
						this.UpdateMarkPC();
					}
				}
			}
		}


		protected bool IsVisibleAddress(int address)
		{
			//	Retourne true si une adresse est visible actuellement.
			int firstIndex = this.GetInstructionIndex(this.firstAddress);

			int lastAddress;
			if (firstIndex+this.fields.Count < this.instructionAddresses.Count)
			{
				lastAddress = this.instructionAddresses[firstIndex+this.fields.Count].Address;
			}
			else
			{
				lastAddress = this.MemoryLength;
			}

			return (address >= this.MemoryStart+this.firstAddress && address < this.MemoryStart+lastAddress);
		}


		protected override void UpdateClientGeometry()
		{
			//	Met à jour la géométrie.
			base.UpdateClientGeometry();

			int total = (int) (this.Client.Bounds.Height/(CodeAccessor.LineHeight-1));
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

			int index = 300;
			for (int i=0; i<total; i++)
			{
				MyWidgets.Code field = new MyWidgets.Code(this.panel);
				field.Index = i;
				field.SetTabIndex(index++);
				field.Processor = this.processor;
				field.CodeAccessor = this;
				field.PreferredHeight = CodeAccessor.LineHeight;
				field.Margins = new Margins(0, 0, 0, -1);
				field.Dock = DockStyle.Top;
				field.InstructionSelected += new EventHandler(this.HandleFieldInstructionSelected);
				field.InstructionDeselected += new EventHandler(this.HandleFieldInstructionDeselected);
				field.InstructionChanged += new EventHandler(this.HandleFieldInstructionChanged);
				field.CodeAddressClicked += new EventHandler(this.HandleCodeAddressClicked);

				this.fields.Add(field);
			}

			this.UpdateData();
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
				this.scroller.MaxValue = (decimal) (this.instructionAddresses.Count - this.fields.Count);
				this.scroller.Value = (decimal) this.GetInstructionIndex(this.firstAddress);
				this.scroller.VisibleRangeRatio = (decimal) System.Math.Max((double) this.fields.Count/this.instructionAddresses.Count, 0.1);  // évite cabine trop petite
				this.scroller.LargeChange = (decimal) this.fields.Count;
				this.scroller.SmallChange = (decimal) 1;
				this.scroller.Enable = true;
			}
			else
			{
				this.scroller.Enable = false;
			}
		}

#if false
		public void UpdateData(int address)
		{
			//	Met à jour la table des instructions suite au changement d'un seul byte.
			if (this.isDeferUpdateData)
			{
				return;
			}

			if (address < this.MemoryStart || address >= this.MemoryStart+this.MemoryLength)
			{
				return;
			}

			if (address < Components.Memory.StackBase-Components.Memory.StackMax || address >= Components.Memory.StackBase)
			{
				address = this.AdjustAddress(address-this.MemoryStart)+this.MemoryStart;
				int index = this.GetInstructionIndex(address-this.MemoryStart);

				if (this.instructionAddresses[index].Type == 1)  // sur un BYTE d'une table ?
				{
					// rien à faire !
				}
				else
				{
					int d = index;
					int tableLength = 0;
					while (address < this.MemoryStart+this.MemoryLength)
					{
						CodeAddress ca = this.GetCodeAddress(ref address, ref tableLength);

						this.instructionAddresses.Insert(index, ca);
						index++;
						d++;

						if (this.IsMatch(ca, d))
						{
							break;
						}
					}
				}
			}

			if (this.IsVisibleAddress(address))
			{
				this.UpdateTable();

				this.ignoreChange = true;
				this.UpdateScroller();
				this.ignoreChange = false;

				this.UpdateMarkPC();
			}
		}

		protected bool IsMatch(CodeAddress ca, int index)
		{
			int start = index;

			while (index < this.instructionAddresses.Count)
			{
				if (this.instructionAddresses[index].Address < ca.Address)
				{
					index++;
				}
				else if (this.instructionAddresses[index].Address > ca.Address)
				{
					return false;
				}
				else
				{
					if (this.instructionAddresses[index].IsEqual(ca))
					{
						for (int i=0; i<=index-start; i++)
						{
							this.instructionAddresses.RemoveAt(start);
						}
						return true;
					}
					else
					{
						return false;
					}
				}
			}

			return true;
		}
#endif

		public void UpdateData()
		{
			//	Met à jour la table des instructions.
			if (this.isDeferUpdateData)
			{
				return;
			}

			this.UpdateInstructionAddresses();
			this.UpdateTable();

			this.ignoreChange = true;
			this.UpdateScroller();
			this.ignoreChange = false;

			this.UpdateMarkPC();
		}

		protected void UpdateTable()
		{
			//	Met à jour la table des instructions.
			if (this.processor == null || this.memory == null)
			{
				return;
			}

			int index = this.GetInstructionIndex(this.firstAddress);
			for (int i=0; i<this.fields.Count; i++)
			{
				if (index < this.instructionAddresses.Count)
				{
					int address = this.MemoryStart+this.instructionAddresses[index].Address;
					int length = this.instructionAddresses[index].Length;
					bool isTable = this.instructionAddresses[index].Type == 1;
					bool isRom = this.memory.IsReadOnly(address);

					List<int> codes = new List<int>();
					for (int c=0; c<length; c++)
					{
						codes.Add(this.memory.ReadForDebug(address+c));
					}

					this.fields[i].SetCode(address, codes, isTable, isRom);
					this.fields[i].CodeAddress.ArrowClear();
					this.fields[i].Visibility = true;
				}
				else
				{
					this.fields[i].Visibility = false;
				}

				index++;
			}

			int level = 0;
			for (int i=0; i<this.fields.Count; i++)
			{
				int arrowAddress = this.fields[i].ArrowAddress;

				if (arrowAddress != Misc.undefined)
				{
					int baseAddress = this.fields[i].Address;
					int address = baseAddress;

					//	Ne montre pas les adresses qui vont trop loin !
					bool fear = false;
					if (this.memory.IsPeriph(arrowAddress) || this.memory.IsDisplay(arrowAddress))
					{
						fear = true;
					}

					if (this.memory.IsRam(baseAddress) && this.memory.IsRom(arrowAddress))
					{
						fear = true;
					}

					if (this.memory.IsRom(baseAddress) && this.memory.IsRam(arrowAddress))
					{
						fear = true;
					}

					index = this.GetInstructionIndex(address-this.MemoryStart);
					int length = this.instructionAddresses[index].Length;

					if (fear)
					{
						this.fields[i].CodeAddress.ArrowAdd(MyWidgets.CodeAddress.Address.Type.Far, baseAddress, 0, false);
						level--;
					}
					else if (address+length <= arrowAddress)  // flèche de haut en bas ?
					{
						this.fields[i].CodeAddress.ArrowAdd(MyWidgets.CodeAddress.Address.Type.StartToDown, baseAddress, level, false);

						address += this.instructionAddresses[index++].Length;

						int j = i+1;
						while (j < this.fields.Count)
						{
							address += this.instructionAddresses[index++].Length;
							if (address > arrowAddress)
							{
								address -= this.instructionAddresses[index-1].Length;
								break;
							}
							this.fields[j++].CodeAddress.ArrowAdd(MyWidgets.CodeAddress.Address.Type.Line, baseAddress, level, false);
						}

						if (j < this.fields.Count)
						{
							this.fields[j].CodeAddress.ArrowAdd(MyWidgets.CodeAddress.Address.Type.ArrowFromUp, baseAddress, level, address != arrowAddress);
						}
					}
					else if (address > arrowAddress)  // flèche de bas en haut ?
					{
						this.fields[i].CodeAddress.ArrowAdd(MyWidgets.CodeAddress.Address.Type.StartToUp, baseAddress, level, false);

						index = this.GetInstructionIndex(address-this.MemoryStart);
						address -= this.instructionAddresses[--index].Length;

						int j = i-1;
						while (j >= 0 && address > arrowAddress)
						{
							this.fields[j--].CodeAddress.ArrowAdd(MyWidgets.CodeAddress.Address.Type.Line, baseAddress, level, false);
							address -= this.instructionAddresses[--index].Length;
						}

						if (j >= 0)
						{
							this.fields[j].CodeAddress.ArrowAdd(MyWidgets.CodeAddress.Address.Type.ArrowFromDown, baseAddress, level, address != arrowAddress);
						}
					}
					else  // flèche sur soi-même ?
					{
						this.fields[i].CodeAddress.ArrowAdd(MyWidgets.CodeAddress.Address.Type.Arrow, baseAddress, level, address != arrowAddress);
					}

					level++;
				}
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
			int tableLength = 0;

			int address = this.MemoryStart;
			while (address < this.MemoryStart+this.MemoryLength)
			{
				CodeAddress ca = this.GetCodeAddress(ref address, ref tableLength);
				this.instructionAddresses.Add(ca);
			}
		}

		protected CodeAddress GetCodeAddress(ref int address, ref int tableLength)
		{
			CodeAddress ca;
			int code = this.memory.ReadForDebug(address);

			if (tableLength > 0)  // BYTE contenu dans la table ?
			{
				ca = new CodeAddress(address-this.MemoryStart, 1, 1);
				address++;
				tableLength--;
			}
			else  // instruction ?
			{
				if (code == this.processor.TableInstruction)  // instruction TABLE #val ?
				{
					ca = new CodeAddress(address-this.MemoryStart, 2, 0);
					tableLength = this.memory.ReadForDebug(address+1);
					if (tableLength == 0)
					{
						tableLength = 0x100;
					}
					address += 2;
				}
				else  // instruction normale ?
				{
					int length = this.processor.GetInstructionLength(code);
					ca = new CodeAddress(address-this.MemoryStart, length, 0);
					address += length;
				}
			}

			return ca;
		}

		protected int AdjustAddress(int address)
		{
			//	Retourne une adresse (relative dans la banque) ajustée pour commencer sur un début d'instruction.
			for (int i=1; i<this.instructionAddresses.Count; i++)
			{
				if (address < this.instructionAddresses[i].Address)
				{
					return this.instructionAddresses[i-1].Address;
				}
			}

			return address;
		}

		protected int GetInstructionIndex(int address)
		{
			//	Retourne l'index de l'instruction en fonction de son adresse (relative dans la banque).
			int count = this.instructionAddresses.Count;

			for (int i=1; i<count; i++)
			{
				if (address < this.instructionAddresses[i].Address)
				{
					return i-1;
				}
			}

			if (address == this.instructionAddresses[count-1].Address)
			{
				return count-1;
			}

			return 0;
		}

		protected int GetInstructionLength(int address)
		{
			//	Retourne la longueur de l'instruction en fonction de son adresse (relative dans la banque).
			int index = this.GetInstructionIndex(address);
			return this.instructionAddresses[index].Length;
		}

		protected void UpdateMarkPC()
		{
			if (this.memory == null)
			{
				return;
			}

			int index = this.GetInstructionIndex(this.firstAddress);
			for (int i=0; i<this.fields.Count; i++)
			{
				if (index < this.instructionAddresses.Count)
				{
					int address = this.MemoryStart+this.instructionAddresses[index].Address;
					this.fields[i].IsBackHilite = (address == this.markPC);
				}
				else
				{
					this.fields[i].IsBackHilite = false;
				}
				index++;
			}
		}


		public int MemoryStart
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


		protected int DetectAddress(Point pos)
		{
			//	Détecte la flèche visée par la souris.
			//	Retourne son adresse de base, ou Misc.undefined.
			for (int i=this.fields.Count-1; i>=0; i--)
			{
				Code code = this.fields[i];

				Point p = code.MapParentToClient(pos);
				p = code.CodeAddress.MapParentToClient(p);
				p.X -= 17;  // TODO: je n'ai décidément rien compris à ces MapTrucToMachin !

				int baseAddress = code.CodeAddress.DetectAddress(p);
				if (baseAddress != Misc.undefined)
				{
					return baseAddress;
				}
			}

			return Misc.undefined;
		}

		protected void HiliteBaseAddress(int baseAddress)
		{
			//	Met en évidence une flèche qui part d'une adresse de base donnée.
			//	Il faut mettre en évidence tous les fragments, dans différents objets CodeAddress.
			foreach (MyWidgets.Code code in this.fields)
			{
				code.CodeAddress.HiliteBaseAddress(baseAddress);
			}
		}

		protected MyWidgets.Code SearchCode(int baseAddress)
		{
			//	Cherche le Code utilisant une adresse de base donnée.
			foreach (MyWidgets.Code code in this.fields)
			{
				if (baseAddress == code.Address)
				{
					return code;
				}
			}

			return null;
		}


		protected override void ProcessMessage(Message message, Point pos)
		{
			if (message.MessageType == MessageType.MouseMove)
			{
				int address = this.DetectAddress(pos);
				if (this.addressHilited != address)
				{
					this.addressHilited = address;
					this.HiliteBaseAddress(this.addressHilited);
				}
			}

			if (message.MessageType == MessageType.MouseWheel)
			{
				int index = this.GetInstructionIndex(this.firstAddress);
				index += (message.Wheel > 0) ? -2 : 2;

				index = System.Math.Max(index, 0);
				index = System.Math.Min(index, this.instructionAddresses.Count-1);

				this.FirstAddress = this.instructionAddresses[index].Address;
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
				address = this.instructionAddresses[v].Address;
			}

			this.FirstAddress = address;
		}

		private void HandleFieldInstructionSelected(object sender)
		{
			MyWidgets.Code widget = sender as MyWidgets.Code;
			int address = Misc.undefined;

			for (int i=0; i<this.fields.Count; i++)
			{
				if (widget == fields[i])
				{
					int index = this.GetInstructionIndex(this.firstAddress);
					address = this.instructionAddresses[index+i].Address;
				}
			}

			this.AddressSelected = address;
		}

		private void HandleFieldInstructionDeselected(object sender)
		{
			this.AddressSelected = Misc.undefined;
		}

		private void HandleFieldInstructionChanged(object sender)
		{
			MyWidgets.Code widget = sender as MyWidgets.Code;

			int address;
			List<int> codes = new List<int>();
			widget.GetCode(out address, codes);

			int oldLength = this.GetInstructionLength(address);
			int maxLength = oldLength;

			for (int i=address+oldLength; i<address+codes.Count; i++)
			{
				if (this.memory.ReadForDebug(i) == 0)
				{
					maxLength++;
				}
				else
				{
					break;
				}
			}

			int shift = 0;
			if (maxLength < codes.Count)  // pas assez de place ?
			{
				shift = codes.Count-maxLength;
				this.memory.ShiftRam(address, shift);
			}

			foreach (int code in codes)
			{
				this.memory.WriteWithDirty(address++, code);  // écrit la nouvelle instruction assemblée
			}

			while (oldLength > codes.Count)  // trop de place ?
			{
				this.memory.WriteWithDirty(address++, 0);  // NOP
				oldLength--;
			}

			this.UpdateData();

			if (shift > 0)
			{
				string title = TextLayout.ConvertToSimpleText(Res.Strings.Window.Title);
				string icon = "manifest:Epsitec.Common.Dialogs.Images.Warning.icon";
				string err = string.Format(Res.Strings.Dialog.CodeAccessor.Shift.Message, shift.ToString(), address.ToString("X3"));
				Common.Dialogs.IDialog dialog = Common.Dialogs.MessageDialog.CreateOk(title, icon, err, null, null);
				dialog.OwnerWindow = this.Window;
				dialog.OpenDialog();
			}
		}

		private void HandleCodeAddressClicked(object sender)
		{
			//	Flèche cliquée.
			if (this.addressHilited == Misc.undefined)
			{
				return;
			}

			MyWidgets.Code code = this.SearchCode(this.addressHilited);
			if (code == null)
			{
				return;
			}

			int arrowAddress = code.ArrowAddress;
			if (!this.memory.IsRam(arrowAddress) && !this.memory.IsRom(arrowAddress))
			{
				return;
			}

			string newBank = Components.Memory.BankSearch(arrowAddress);
			if (this.Bank != newBank)
			{
				this.Bank = newBank;
				this.OnBankChanged();
			}
			
			this.FirstAddress = arrowAddress-this.MemoryStart;
		}


		#region EventHandler
		protected virtual void OnInstructionSelected()
		{
			//	Génère un événement pour dire qu'une cellule a été sélectionnée.
			EventHandler handler = (EventHandler) this.GetUserEventHandler("InstructionSelected");
			if (handler != null)
			{
				handler(this);
			}
		}

		public event Common.Support.EventHandler InstructionSelected
		{
			add
			{
				this.AddUserEventHandler("InstructionSelected", value);
			}
			remove
			{
				this.RemoveUserEventHandler("InstructionSelected", value);
			}
		}

		protected virtual void OnBankChanged()
		{
			//	Génère un événement pour dire qu'une cellule a été sélectionnée.
			EventHandler handler = (EventHandler) this.GetUserEventHandler("BankChanged");
			if (handler != null)
			{
				handler(this);
			}
		}

		public event Common.Support.EventHandler BankChanged
		{
			add
			{
				this.AddUserEventHandler("BankChanged", value);
			}
			remove
			{
				this.RemoveUserEventHandler("BankChanged", value);
			}
		}
		#endregion


		/// <summary>
		/// Une instance de CodeAddress regroupe les informations sur une instruction. Toutes les instructions,
		/// visibles ou cachées, ont une instance de CodeAddress.
		/// </summary>
		protected struct CodeAddress
		{
			public CodeAddress(int address, int length, int type)
			{
				this.address = address;
				this.length = (byte) length;
				this.type = (byte) type;
			}

			public int Address
			{
				get
				{
					return this.address;
				}
			}

			public int Length
			{
				get
				{
					return (int) this.length;
				}
			}

			public int Type
			{
				get
				{
					return (int) this.type;
				}
			}

			public bool IsEqual(CodeAddress o)
			{
				return this.address == o.address && this.length == o.length && this.type == o.type;
			}

			private int address;
			private byte length;
			private byte type;
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
		protected int								addressSelected;
		protected int								addressHilited;
		protected List<CodeAddress>					instructionAddresses;
		protected bool								followPC;
		protected bool								isDeferUpdateData;
		protected bool								ignoreChange;
	}
}
