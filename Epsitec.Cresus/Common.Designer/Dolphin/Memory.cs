using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Dolphin
{
	/// <summary>
	/// Memoire émulée du dauphin.
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

		public void Clear()
		{
			//	Vide toute le mémoire.
			for (int i=0; i<DolphinApplication.RomBase; i++)
			{
				this.memory[i] = 0;
			}

			foreach (Digit digit in this.application.DisplayDigits)
			{
				digit.SegmentValue = (Digit.DigitSegment) 0;
			}
		}

		public string GetContent()
		{
			//	Retourne tout le contenu de la mémoire dans une chaîne (pour la sérialisation).
			System.Text.StringBuilder builder = new System.Text.StringBuilder();

			//	Cherche la dernière adresse non nulle.
			int last = 0;
			for (int i=DolphinApplication.PeriphBase-1; i>=0; i--)
			{
				if (this.memory[i] != 0)
				{
					last = i+1;
					break;
				}
			}

			for (int i=0; i<last; i++)
			{
				builder.Append(this.memory[i].ToString("X2"));
			}

			return builder.ToString();
		}

		public void PutContent(string data)
		{
			//	Initialise tout le contenu de la mémoire d'après une chaîne (pour la désérialisation).
			this.Clear();

			int i = 0;
			while (i < data.Length)
			{
				string hexa = data.Substring(i, 2);
				this.memory[i/2] = (byte) Memory.ParseHexa(hexa);
				i += 2;
			}
		}

		public bool IsReadOnly(int address)
		{
			//	Indique si l'adresse ne permet pas l'écriture.
			return !this.IsRam(address) && !this.IsPeriph(address);
		}

		public bool IsValid(int address)
		{
			//	Indique si l'adresse est valide.
			return this.IsRam(address) || this.IsRom(address) || this.IsPeriph(address);
		}

		public bool IsRam(int address)
		{
			//	Indique si l'adresse est en Ram.
			return (address >= DolphinApplication.RamBase && address < DolphinApplication.RamBase+DolphinApplication.RamLength);
		}

		public bool IsRom(int address)
		{
			//	Indique si l'adresse est en Rom.
			return (address >= DolphinApplication.RomBase && address < DolphinApplication.RomBase+DolphinApplication.RomLength);
		}

		public bool IsPeriph(int address)
		{
			//	Indique si l'adresse est un périphérique.
			return (address >= DolphinApplication.PeriphBase && address < DolphinApplication.PeriphBase+DolphinApplication.PeriphLength);
		}

		public int Read(int address)
		{
			//	Lit une valeur en mémoire et/ou dans un périphérique.
			if (this.IsValid(address))  // adresse valide ?
			{
				int value = this.memory[address];

				if (address == DolphinApplication.PeriphKeyboard)  // lecture du clavier ?
				{
					if ((value & 0x80) != 0)  // bit full ?
					{
						this.memory[address] = (byte) (value & ~0x87);  // clear le bit full et les touches 0..7
					}
				}

				return value;
			}
			else  // hors de l'espace d'adressage ?
			{
				return 0xff;
			}
		}

		public int ReadForDebug(int address)
		{
			//	Lit une valeur en mémoire et/ou dans un périphérique, pour de debug.
			//	Le bit full du clavier (par exemple) n'est pas clearé.
			if (this.IsValid(address))  // adresse valide ?
			{
				return this.memory[address];
			}
			else  // hors de l'espace d'adressage ?
			{
				return 0xff;
			}
		}

		public void WriteWithDirty(int address, int data)
		{
			//	Ecrit une valeur en mémoire et/ou dans un périphérique et
			//	gère le bit dirty.
			if (!this.IsReadOnly(address) && data != this.ReadForDebug(address))
			{
				this.Write(address, data);

				if (this.IsRam(address))  // mémoire ?
				{
					this.application.Dirty = true;
				}
			}
		}

		public void Write(int address, int data)
		{
			//	Ecrit une valeur en mémoire et/ou dans un périphérique.
			if (!this.IsReadOnly(address))  // adresse valide ?
			{
				if (this.memory[address] != (byte) data)
				{
					this.memory[address] = (byte) data;

					if (!this.application.IsEmptyPanel)
					{
						this.application.MemoryAccessor.UpdateData();
					}
				}
			}

			if (this.IsPeriph(address))  // périphérique ?
			{
				if (address >= DolphinApplication.PeriphFirstDigit && address <= DolphinApplication.PeriphLastDigit)  // l'un des 4 digits ?
				{
					int a = address - DolphinApplication.PeriphFirstDigit;
					int t = DolphinApplication.PeriphLastDigit-DolphinApplication.PeriphFirstDigit;
					this.application.DisplayDigits[t-a].SegmentValue = (Digit.DigitSegment) this.memory[address];
				}
			}
		}

		public void RomInitialise(AbstractProcessor processor)
		{
			//	Initialise la Rom.
			processor.RomInitialise(DolphinApplication.RomBase);
		}

		public void WriteRom(int address, int data)
		{
			//	Ecrit une valeur en mémoire morte (initialisation).
			this.memory[address] = (byte) data;
		}

		protected static int ParseHexa(string hexa)
		{
			int result;
			if (System.Int32.TryParse(hexa, System.Globalization.NumberStyles.AllowHexSpecifier, System.Globalization.CultureInfo.CurrentCulture, out result))
			{
				return result;
			}
			else
			{
				return 0;
			}
		}

		protected DolphinApplication application;
		protected byte[] memory;
	}
}
