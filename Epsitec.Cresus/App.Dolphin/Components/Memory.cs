//	Copyright � 2003-2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.App.Dolphin.Components
{
	/// <summary>
	/// Memoire �mul�e du dauphin.
	/// </summary>
	public class Memory
	{
		public Memory(DolphinApplication application)
		{
			//	Alloue et initialise la m�moire du dauphin.
			this.application = application;

			int size = 1 << Memory.TotalAddress;
			this.memory = new byte[size];

			for (int i=0; i<size; i++)
			{
				this.memory[i] = 0;
			}
		}

		public int Length
		{
			//	Retourne la longueur totale de la m�moire (en fait, il serait plus juste
			//	de parler de la longueur de l'espace d'adressage).
			get
			{
				return this.memory.Length;
			}
		}

		public bool IsEmptyRam
		{
			//	Retourne true si la Ram est enti�rement vide.
			get
			{
				for (int i=Memory.RamBase; i<Memory.RamBase+Memory.RamLength; i++)
				{
					if (this.memory[i] != 0)
					{
						return false;
					}
				}

				return true;
			}
		}

		public void ClearRam()
		{
			//	Vide toute le m�moire Ram.
			for (int i=Memory.RamBase; i<Memory.RamBase+Memory.RamLength; i++)
			{
				this.memory[i] = 0;
			}
		}

		public void ClearPeriph()
		{
			//	Vide toute le m�moire p�riph�rique.
			for (int i=Memory.PeriphBase; i<Memory.PeriphBase+Memory.PeriphLength; i++)
			{
				this.memory[i] = 0;
			}

			foreach (MyWidgets.Digit digit in this.application.DisplayDigits)
			{
				digit.SegmentValue = (MyWidgets.Digit.DigitSegment) 0;
			}

			this.application.DisplayBitmap.Invalidate();
		}


		public string GetContent()
		{
			//	Retourne tout le contenu de la m�moire Ram dans une cha�ne (pour la s�rialisation).
			System.Text.StringBuilder builder = new System.Text.StringBuilder();

			//	Cherche la derni�re adresse non nulle.
			int last = 0;
			for (int i=Memory.RamBase+Memory.RamLength-1; i>=Memory.RamBase; i--)
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
			//	Initialise tout le contenu de la m�moire Ram d'apr�s une cha�ne (pour la d�s�rialisation).
			this.ClearRam();

			int i = 0;
			while (i < data.Length)
			{
				string hexa = data.Substring(i, 2);
				this.memory[Memory.RamBase+i/2] = (byte) Memory.ParseHexa(hexa);
				i += 2;
			}
		}


		public bool IsReadOnly(int address)
		{
			//	Indique si l'adresse ne permet pas l'�criture.
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
			return (address >= Memory.RamBase && address < Memory.RamBase+Memory.RamLength);
		}

		public bool IsRom(int address)
		{
			//	Indique si l'adresse est en Rom.
			return (address >= Memory.RomBase && address < Memory.RomBase+Memory.RomLength);
		}

		public bool IsPeriph(int address)
		{
			//	Indique si l'adresse est un p�riph�rique.
			return (address >= Memory.PeriphBase && address < Memory.PeriphBase+Memory.PeriphLength);
		}


		public int Read(int address)
		{
			//	Lit une valeur en m�moire et/ou dans un p�riph�rique.
			if (this.IsValid(address))  // adresse valide ?
			{
				int value = this.memory[address];

				if (address == Memory.PeriphKeyboard)  // lecture du clavier ?
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
			//	Lit une valeur en m�moire et/ou dans un p�riph�rique, pour de debug.
			//	Le bit full du clavier (par exemple) n'est pas clear�.
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
			//	Ecrit une valeur en m�moire et/ou dans un p�riph�rique et
			//	g�re l'�tat dirty.
			if (!this.IsReadOnly(address) && data != this.ReadForDebug(address))
			{
				this.Write(address, data);

				if (this.IsRam(address))  // m�moire ?
				{
					this.application.Dirty = true;
				}
			}
		}

		public void Write(int address, int data)
		{
			//	Ecrit une valeur en m�moire et/ou dans un p�riph�rique.
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

			if (this.IsPeriph(address))  // p�riph�rique ?
			{
				if (address >= Memory.PeriphFirstDigit && address <= Memory.PeriphLastDigit)  // l'un des 4 digits ?
				{
					int a = address - Memory.PeriphFirstDigit;
					this.application.DisplayDigits[a].SegmentValue = (MyWidgets.Digit.DigitSegment) this.memory[address];
				}

				if (address >= Memory.PeriphDisplay && address <= Memory.PeriphDisplay*Memory.PeriphDisplayDx/8*Memory.PeriphDisplayDy)  // �cran bitmap ?
				{
					this.application.DisplayBitmap.Invalidate();
				}
			}
		}


		public void RomInitialise(AbstractProcessor processor)
		{
			//	Initialise la Rom.
			processor.RomInitialise(Memory.RomBase);
		}

		public void WriteRom(int address, int data)
		{
			//	Ecrit une valeur en m�moire morte (pour l'initialisation de la Rom).
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


		public static readonly int TotalAddress = 12;
		public static readonly int TotalData    = 8;

		public static readonly int RamBase          = 0x000;
		public static readonly int RamLength        = 0x800;
		public static readonly int StackBase        = 0x800;

		public static readonly int RomBase          = 0x800;
		public static readonly int RomLength        = 0x400;

		public static readonly int PeriphBase       = 0xC00;
		public static readonly int PeriphLength     = 0x400;
		public static readonly int PeriphFirstDigit = 0xC00;  // digit de gauche
		public static readonly int PeriphLastDigit  = 0xC03;  // digit de droite
		public static readonly int PeriphKeyboard   = 0xC07;
		public static readonly int PeriphDisplay    = 0xC80;
		public static readonly int PeriphDisplayDx  = 32;
		public static readonly int PeriphDisplayDy  = 24;


		protected DolphinApplication application;
		protected byte[] memory;
	}
}
