//	Copyright © 2003-2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.App.Dolphin.Components
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

			int size = 1 << Memory.TotalAddress;
			this.memory = new byte[size];

			for (int i=0; i<size; i++)
			{
				this.memory[i] = 0;
			}

			this.ClearRam();
		}


		public void RomVariables(Dictionary<string, int> variables)
		{
			//	Défini les variables des périphériques.
			variables.Add("_DIGIT0", Memory.PeriphFirstDigit+0);
			variables.Add("_DIGIT1", Memory.PeriphFirstDigit+1);
			variables.Add("_DIGIT2", Memory.PeriphFirstDigit+2);
			variables.Add("_DIGIT3", Memory.PeriphFirstDigit+3);
			variables.Add("_DIGITCOUNT", Memory.PeriphLastDigit-Memory.PeriphFirstDigit+1);

			variables.Add("_KEYBOARD", Memory.PeriphKeyboard);
			variables.Add("_KEYBOARDSHIFT", 3);
			variables.Add("_KEYBOARDCTRL", 4);
			variables.Add("_KEYBOARDDOWN", 3);
			variables.Add("_KEYBOARDUP", 4);
			variables.Add("_KEYBOARDLEFT", 5);
			variables.Add("_KEYBOARDRIGHT", 6);
			variables.Add("_KEYBOARDFULL", 7);
			
			variables.Add("_BITMAP", Memory.DisplayBase);
			variables.Add("_BITMAPWIDTH", Memory.DisplayDx);
			variables.Add("_BITMAPHEIGHT", Memory.DisplayDy);
		}


		public int Length
		{
			//	Retourne la longueur totale de la mémoire (en fait, il serait plus juste
			//	de parler de la longueur de l'espace d'adressage).
			get
			{
				return this.memory.Length;
			}
		}

		public bool IsEmptyRam
		{
			//	Retourne true si la Ram est entièrement vide.
			//	On ignore quelques bytes à la fin de la mémoire pour le stack.
			get
			{
				for (int i=Memory.RamBase; i<Memory.RamBase+Memory.RamLength-Memory.StackMax; i++)
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
			//	Vide toute le mémoire Ram.
			for (int i=Memory.RamBase; i<Memory.RamBase+Memory.RamLength; i++)
			{
				this.memory[i] = 0;
			}

			this.memory[Memory.StackBase-Memory.StackMax+0] = 0xFF;  // instruction TABLE #val
			this.memory[Memory.StackBase-Memory.StackMax+1] = (byte) (Memory.StackMax-2);
		}

		public void ClearPeriph()
		{
			//	Vide toute le mémoire périphérique.
			for (int i=Memory.PeriphBase; i<Memory.PeriphBase+Memory.PeriphLength; i++)
			{
				this.memory[i] = 0;
			}

			for (int i=Memory.DisplayBase; i<Memory.DisplayBase+Memory.DisplayLength; i++)
			{
				this.memory[i] = 0;
			}

			foreach (MyWidgets.Digit digit in this.application.DisplayDigits)
			{
				digit.SegmentValue = (MyWidgets.Digit.DigitSegment) 0;
			}

			this.application.DisplayBitmap.Invalidate();
		}

		public void ClearDisplay()
		{
			//	Efface l'écran bitmap.
			for (int i=Memory.DisplayBase; i<Memory.DisplayBase+Memory.DisplayLength; i++)
			{
				this.memory[i] = 0;
			}

			this.application.DisplayBitmap.Invalidate();
		}

		public void ShiftRam(int address, int offset)
		{
			//	Décale toute la mémoire.
			System.Diagnostics.Debug.Assert(address != -1 || offset != 0);

			if (offset > 0)  // creuse un trou ?
			{
				for (int i=Memory.RamBase+Memory.RamLength-1; i>=address+offset; i--)
				{
					this.memory[i] = this.memory[i-offset];
				}

				for (int i=address; i<address+offset; i++)
				{
					this.memory[i] = 0;
				}
			}

			if (offset < 0)  // bouche un trou ?
			{
				offset = -offset;

				for (int i=address; i<Memory.RamBase+Memory.RamLength-offset; i++)
				{
					this.memory[i] = this.memory[i+offset];
				}

				for (int i=Memory.RamBase+Memory.RamLength-offset; i<Memory.RamBase+Memory.RamLength; i++)
				{
					this.memory[i] = 0;
				}
			}
		}


		public string GetContent()
		{
			//	Retourne tout le contenu de la mémoire Ram dans une chaîne (pour la sérialisation).
			System.Text.StringBuilder builder = new System.Text.StringBuilder();

			//	Cherche la dernière adresse non nulle.
			int last = 0;
			for (int i=Memory.RamBase+Memory.RamLength-Memory.StackMax-1; i>=Memory.RamBase; i--)
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
			//	Initialise tout le contenu de la mémoire Ram d'après une chaîne (pour la désérialisation).
			this.ClearRam();

			int i = 0;
			while (i < data.Length)
			{
				string hexa = data.Substring(i, 2);
				this.memory[Memory.RamBase+i/2] = (byte) Misc.ParseHexa(hexa);
				i += 2;
			}
		}


		public static string BankSearch(int address)
		{
			//	Retourne la banque à utiliser pour une adresse donnée.
			if (address >= Memory.RamBase && address < Memory.RamBase+Memory.RamLength)
			{
				return "M";
			}

			if (address >= Memory.RomBase && address < Memory.RomBase+Memory.RomLength)
			{
				return "R";
			}

			if (address >= Memory.PeriphBase && address < Memory.PeriphBase+Memory.PeriphLength)
			{
				return "P";
			}
			
			if (address >= Memory.DisplayBase && address < Memory.DisplayBase+Memory.DisplayLength)
			{
				return "D";
			}
			
			return null;
		}

		public static int BankStart(string bank)
		{
			//	Retourne le début d'une banque.
			switch (bank)
			{
				case "M":
					return Memory.RamBase;

				case "R":
					return Memory.RomBase;

				case "P":
					return Memory.PeriphBase;

				case "D":
					return Memory.DisplayBase;

				default:
					return 0;
			}
		}

		public static int BankLength(string bank)
		{
			//	Retourne la longueur d'une banque.
			switch (bank)
			{
				case "M":
					return Memory.RamLength;

				case "R":
					return Memory.RomLength;

				case "P":
					return Memory.PeriphLength;

				case "D":
					return Memory.DisplayLength;

				default:
					return 0;
			}
		}


		public bool IsReadOnly(int address)
		{
			//	Indique si l'adresse ne permet pas l'écriture.
			return !this.IsRam(address) && !this.IsPeriph(address) && !this.IsDisplay(address);
		}

		public bool IsValid(int address)
		{
			//	Indique si l'adresse est valide.
			return this.IsRam(address) || this.IsRom(address) || this.IsPeriph(address) || this.IsDisplay(address);
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
			//	Indique si l'adresse est un périphérique.
			return (address >= Memory.PeriphBase && address < Memory.PeriphBase+Memory.PeriphLength);
		}

		public bool IsDisplay(int address)
		{
			//	Indique si l'adresse est un périphérique.
			return (address >= Memory.DisplayBase && address < Memory.DisplayBase+Memory.DisplayLength);
		}


		public int Read(int address)
		{
			//	Lit une valeur en mémoire et/ou dans un périphérique.
			if (this.IsValid(address))  // adresse valide ?
			{
				int value = this.memory[address];

				if (address == Memory.PeriphKeyboard)  // lecture du clavier ?
				{
					if ((value & 0x80) != 0)  // bit full ?
					{
						//?this.memory[address] = (byte) (value & ~0x87);  // clear le bit full et les touches 0..7
						this.memory[address] = (byte) (value & ~0x80);  // clear le bit full
					}
				}

				return value;
			}
			else  // hors de l'espace d'adressage ?
			{
				return 0xFF;
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
				return 0xFF;
			}
		}


		public void WriteWithDirty(int address, int data)
		{
			//	Ecrit une valeur en mémoire et/ou dans un périphérique et
			//	gère l'état dirty.
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

					if (this.application != null && !this.application.IsEmptyPanel)
					{
						//	Les méthodes UpdateData sont différées lorsqu'on exécute plusieurs ProcessorClock
						//	par HandleClockTimeElapsed.
						this.application.MemoryAccessor.UpdateData();
						this.application.CodeAccessor.UpdateData();
					}
				}
			}

			if (this.application != null)
			{
				if (this.IsPeriph(address))  // périphérique ?
				{
					if (address >= Memory.PeriphFirstDigit && address <= Memory.PeriphLastDigit)  // l'un des 4 digits ?
					{
						int a = address - Memory.PeriphFirstDigit;
						this.application.DisplayDigits[a].SegmentValue = (MyWidgets.Digit.DigitSegment) this.memory[address];
					}
				}

				if (this.IsDisplay(address))  // écran bitmap ?
				{
					this.application.DisplayBitmap.Invalidate();
				}
			}
		}


		public void RomInitialise(AbstractProcessor processor)
		{
			//	Initialise la Rom.
			processor.RomInitialise(Memory.RomBase, Memory.RomLength);
		}

		public void WriteRom(int address, int data)
		{
			//	Ecrit une valeur en mémoire morte (pour l'initialisation de la Rom).
			this.memory[address] = (byte) data;
		}


		public static readonly int TotalAddress = 12;
		public static readonly int TotalData    = 8;

		public static readonly int RamBase          = 0x000;
		public static readonly int RamLength        = 0x800;
		public static readonly int StackBase        = 0x800;
		public static readonly int StackMax         = 0x080;

		public static readonly int RomBase          = 0x800;
		public static readonly int RomLength        = 0x400;

		public static readonly int PeriphBase       = 0xC00;
		public static readonly int PeriphLength     = 0x010;
		public static readonly int PeriphFirstDigit = 0xC00;  // digit de gauche
		public static readonly int PeriphLastDigit  = 0xC03;  // digit de droite
		public static readonly int PeriphKeyboard   = 0xC07;

		public static readonly int DisplayBase      = 0xC80;
		public static readonly int DisplayDx        = 32;
		public static readonly int DisplayDy        = 24;
		public static readonly int DisplayLength    = Memory.DisplayDx*Memory.DisplayDy/8;


		protected DolphinApplication application;
		protected byte[] memory;
	}
}
