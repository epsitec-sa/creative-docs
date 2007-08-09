//	Copyright © 2003-2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;

namespace Epsitec.App.Dolphin.Components
{
	/// <summary>
	/// Petit processeur 8 bits bien orthogonal.
	/// </summary>
	public class TinyProcessor : AbstractProcessor
	{
		protected enum Instructions
		{
			//	r     =  A,B,X,Y
			//	r'    =  A,B
			//	ADDR  =  adresse 16 bits, 4 bits de mode (abs/rel, +{XY}) et 12 bits valeur
			//	#val  =  valeur absolue 8 bits

			Nop     = 0x00,
			Ret     = 0x01,
			Halt    = 0x02,
			SetC    = 0x04,
			ClrC    = 0x05,
			SetV    = 0x06,
			ClrV    = 0x07,

			PushR   = 0x08,		// PUSH r
			PopR    = 0x0C,		// POP r

			Jump    = 0x10,
			JumpEQ  = 0x12,
			JumpNE  = 0x13,
			JumpLO  = 0x14,
			JumpLS  = 0x15,
			JumpHI  = 0x16,
			JumpHS  = 0x17,
			JumpVC  = 0x18,
			JumpVS  = 0x19,
			JumpNC  = 0x1A,
			JumpNS  = 0x1B,

			//	0x1C..0x1F libre

			Call    = 0x20,
			CallEQ  = 0x22,
			CallNE  = 0x23,
			CallLO  = 0x24,
			CallLS  = 0x25,
			CallHI  = 0x26,
			CallHS  = 0x27,
			CallVC  = 0x28,
			CallVS  = 0x29,
			CallNC  = 0x2A,
			CallNS  = 0x2B,

			// 0x2C..02F libre
			// 0x30..03F libre

			ClrR    = 0x40,		// op r
			IncR    = 0x48,
			DecR    = 0x4C,

			SlRR    = 0x50,		// op r'
			SrRR    = 0x52,
			RlcRR   = 0x54,
			RrcRR   = 0x56,
			NotRR   = 0x58,

			ClrA    = 0x60,		// op ADDR
			IncA    = 0x61,
			DecA    = 0x62,
			SlA     = 0x64,
			SrA     = 0x65,
			RlcA    = 0x66,
			RrcA    = 0x67,
			NotA    = 0x68,

			//	0x69..0x6F libre

			MoveRR  = 0x70,		// MOVE r,r

			MoveAR  = 0x80,		// op ADDR,r
			CompAR  = 0x84,
			AddAR   = 0x88,
			SubAR   = 0x8C,

			AndARR  = 0x90,		// op ADDR,r'
			OrARR   = 0x92,
			XorARR  = 0x94,
			TestARR = 0x98,
			TClrARR = 0x9A,
			TSetARR = 0x9C,
			TInvARR = 0x9D,

			MoveRA  = 0xA0,		// op r,ADDR
			CompRA  = 0xA4,
			AddRA   = 0xA8,
			SubRA   = 0xAC,

			AndRRA  = 0xB0,		// op r',ADDR
			OrRRA   = 0xB2,
			XorRRA  = 0xB4,
			TestRRA = 0xB8,
			TClrRRA = 0xBA,
			TSetRRA = 0xBC,
			TInvRRA = 0xBD,

			MoveVR  = 0xC0,		// op #val,r
			CompVR  = 0xC4,
			AddVR   = 0xC8,
			SubVR   = 0xCC,

			AndVRR  = 0xD0,		// op #val,r'
			OrVRR   = 0xD2,
			XorVRR  = 0xD4,
			TestVRR = 0xD8,
			TClrVRR = 0xDA,
			TSetVRR = 0xDC,
			TInvVRR = 0xDD,

			//	0xE0..0xEF libre
			//	0xF0..0xFF libre
		}


		public TinyProcessor(Memory memory) : base(memory)
		{
			//	Constructeur du processeur.
		}

		public override string Name
		{
			//	Nom du processeur.
			get
			{
				return "Tiny";
			}
		}

		public override void Reset()
		{
			//	Reset du processeur pour démarrer à l'adresse 0.
			base.Reset();
			this.registerPC = Memory.RamBase;
			this.registerSP = Memory.StackBase;
			this.registerF = 0;
			this.registerA = 0;
			this.registerB = 0;
			this.registerX = 0;
			this.registerY = 0;
		}

		public override bool IsCall(out int retAddress)
		{
			//	Indique si le processeur est sur une instruction CALL.
			//	Si oui, retourne l'adresse après le CALL.
			Instructions op = (Instructions) this.memory.Read(this.registerPC);

			if (op >= Instructions.Call && op <= Instructions.CallNS)
			{
				retAddress = this.registerPC+3;
				return true;
			}
			else
			{
				retAddress = 0;
				return false;
			}
		}

		public override void Clock()
		{
			//	Exécute une instruction du processeur.
			if (this.isHalted)
			{
				return;
			}

			if (this.registerPC < 0 || this.registerPC >= this.memory.Length)
			{
				this.Reset();
			}

			int op = this.memory.Read(this.registerPC++);
			int data, address;

			switch ((Instructions) op)
			{
				case Instructions.Nop:
					return;

				case Instructions.Ret:
					this.registerPC = this.StackPopWord();
					return;

				case Instructions.Halt:
					this.registerPC--;
					this.isHalted = true;
					return;

				case Instructions.SetC:
					this.SetFlag(TinyProcessor.FlagCarry, true);
					return;

				case Instructions.ClrC:
					this.SetFlag(TinyProcessor.FlagCarry, false);
					return;

				case Instructions.SetV:
					this.SetFlag(TinyProcessor.FlagOverflow, true);
					return;

				case Instructions.ClrV:
					this.SetFlag(TinyProcessor.FlagOverflow, false);
					return;
			}

			if (op >= (int) Instructions.PushR && op <= (int) Instructions.PushR + 0x03)  // PUSH r
			{
				int n = op & 0x03;
				this.StackPushByte(this.GetRegister(n));
				return;
			}

			if (op >= (int) Instructions.PopR && op <= (int) Instructions.PopR + 0x03)  // POP r
			{
				int n = op & 0x03;
				this.SetRegister(n, this.StackPopByte());
				return;
			}

			if (op >= (int) Instructions.Jump && op <= (int) Instructions.JumpNS)
			{
				address = this.AddressAbs;
				if (this.IsTestTrue(op))
				{
					this.registerPC = address;
				}
				return;
			}

			if (op >= (int) Instructions.Call && op <= (int) Instructions.CallNS)
			{
				address = this.AddressAbs;
				if (this.IsTestTrue(op))
				{
					this.StackPushWord(this.registerPC);
					this.registerPC = address;
				}
				return;
			}

			if (op >= (int) Instructions.ClrR && op <= (int) Instructions.ClrR + 0x0F)  // op r
			{
				int n = op & 0x03;
				Instructions i = (Instructions) (op & 0xFC);

				switch (i)
				{
					case Instructions.ClrR:
						this.SetRegister(n, 0);
						return;

					case Instructions.IncR:
						this.SetRegister(n, this.GetRegister(n)+1);
						return;

					case Instructions.DecR:
						this.SetRegister(n, this.GetRegister(n)-1);
						return;
				}
			}

			if (op >= (int) Instructions.SlRR && op <= (int) Instructions.SlRR + 0x0F)  // op r'
			{
				int n = op & 0x01;
				Instructions i = (Instructions) (op & 0xFE);

				switch (i)
				{
					case Instructions.SlRR:
						return;

					case Instructions.SrRR:
						return;

					case Instructions.RlcRR:
						return;

					case Instructions.RrcRR:
						return;

					case Instructions.NotRR:
						return;
				}
			}

			if (op >= (int) Instructions.ClrA && op <= (int) Instructions.ClrA + 0x0F)  // op ADDR
			{
				Instructions i = (Instructions) op;
				address = this.AddressAbs;

				switch (i)
				{
					case Instructions.ClrA:
						this.memory.Write(address, 0);
						return;

					case Instructions.IncA:
						this.memory.Write(address, this.memory.Read(address)+1);
						return;

					case Instructions.DecA:
						this.memory.Write(address, this.memory.Read(address)-1);
						return;

					case Instructions.SlA:
						return;

					case Instructions.SrA:
						return;

					case Instructions.RrcA:
						return;

					case Instructions.NotA:
						this.memory.Write(address, this.memory.Read(address)^0xFF);
						return;
				}
			}

			if (op >= (int) Instructions.MoveRR && op <= (int) Instructions.MoveRR + 0x0F)  // MOVE r,r
			{
				int src = op & 0x03;
				int dst = (op>>2) & 0x03;
				this.SetRegister(src, this.GetRegister(dst));
			}

			if (op >= (int) Instructions.MoveAR && op <= (int) Instructions.MoveAR + 0x0F)  // op ADDR,r
			{
				int n = op & 0x03;
				Instructions i = (Instructions) (op & 0xFC);
				address = this.AddressAbs;

				switch (i)
				{
					case Instructions.MoveAR:
						this.SetRegister(n, this.memory.Read(address));
						return;

					case Instructions.CompAR:
						return;

					case Instructions.AddAR:
						this.SetRegister(n, this.GetRegister(n)+this.memory.Read(address));
						return;

					case Instructions.SubAR:
						this.SetRegister(n, this.GetRegister(n)-this.memory.Read(address));
						return;
				}
			}

			if (op >= (int) Instructions.AndARR && op <= (int) Instructions.AndARR + 0x0F)  // op ADDR,r'
			{
				int n = op & 0x01;
				Instructions i = (Instructions) (op & 0xFE);
				address = this.AddressAbs;

				switch (i)
				{
					case Instructions.AndARR:
						this.SetRegister(n, this.GetRegister(n)&this.memory.Read(address));
						return;

					case Instructions.OrARR:
						this.SetRegister(n, this.GetRegister(n)|this.memory.Read(address));
						return;

					case Instructions.XorARR:
						this.SetRegister(n, this.GetRegister(n)^this.memory.Read(address));
						return;

					case Instructions.TestARR:
						return;

					case Instructions.TClrARR:
						return;

					case Instructions.TSetARR:
						return;

					case Instructions.TInvARR:
						return;
				}

				this.memory.Write(this.AddressAbs, this.GetRegister(n));
			}

			if (op >= (int) Instructions.MoveRA && op <= (int) Instructions.MoveRA + 0x0F)  // op r,ADDR
			{
				int n = op & 0x01;
				Instructions i = (Instructions) (op & 0xFC);
				address = this.AddressAbs;

				switch (i)
				{
					case Instructions.MoveRA:
						this.memory.Write(address, this.GetRegister(n));
						return;

					case Instructions.CompRA:
						return;

					case Instructions.AddRA:
						this.memory.Write(address, this.memory.Read(address)+this.GetRegister(n));
						return;

					case Instructions.SubRA:
						this.memory.Write(address, this.memory.Read(address)-this.GetRegister(n));
						return;
				}

				this.memory.Write(this.AddressAbs, this.GetRegister(n));
			}

			if (op >= (int) Instructions.AndRRA && op <= (int) Instructions.AndRRA + 0x0F)  // op r',ADDR
			{
				int n = op & 0x01;
				Instructions i = (Instructions) (op & 0xFE);
				address = this.AddressAbs;

				switch (i)
				{
					case Instructions.AndRRA:
						this.memory.Write(address, this.memory.Read(address)&this.GetRegister(n));
						return;

					case Instructions.OrRRA:
						this.memory.Write(address, this.memory.Read(address)|this.GetRegister(n));
						return;

					case Instructions.XorRRA:
						this.memory.Write(address, this.memory.Read(address)^this.GetRegister(n));
						return;

					case Instructions.TestRRA:
						return;

					case Instructions.TClrRRA:
						return;

					case Instructions.TSetRRA:
						return;

					case Instructions.TInvRRA:
						return;
				}

				this.memory.Write(this.AddressAbs, this.GetRegister(n));
			}

			if (op >= (int) Instructions.MoveVR && op <= (int) Instructions.MoveVR + 0x0F)  // op #val,r
			{
				int n = op & 0x03;
				Instructions i = (Instructions) (op & 0xFC);
				data = this.memory.Read(this.registerPC++);

				switch (i)
				{
					case Instructions.MoveVR:
						this.SetRegister(n, data);
						return;

					case Instructions.CompVR:
						return;

					case Instructions.AddVR:
						this.SetRegister(n, this.GetRegister(n)+data);
						return;

					case Instructions.SubVR:
						this.SetRegister(n, this.GetRegister(n)-data);
						return;
				}

				this.SetRegister(n, this.memory.Read(this.registerPC++));
			}

			if (op >= (int) Instructions.AndVRR && op <= (int) Instructions.AndVRR + 0x0F)  // op #val,r'
			{
				int n = op & 0x01;
				Instructions i = (Instructions) (op & 0xFE);
				data = this.memory.Read(this.registerPC++);

				switch (i)
				{
					case Instructions.AndVRR:
						this.SetRegister(n, this.GetRegister(n)&data);
						return;

					case Instructions.OrVRR:
						this.SetRegister(n, this.GetRegister(n)|data);
						return;

					case Instructions.XorVRR:
						this.SetRegister(n, this.GetRegister(n)^data);
						return;

					case Instructions.TestVRR:
						return;

					case Instructions.TClrVRR:
						return;

					case Instructions.TSetVRR:
						return;

					case Instructions.TInvVRR:
						return;
				}

				this.SetRegister(n, this.memory.Read(this.registerPC++));
			}
		}



		protected int GetRegister(int n)
		{
			n &= 0x03;

			switch (n)
			{
				case 0:
					return this.registerA;

				case 1:
					return this.registerB;

				case 2:
					return this.registerX;

				case 3:
					return this.registerY;
			}

			return 0;
		}

		protected void SetRegister(int n, int value)
		{
			n &= 0x03;

			switch (n)
			{
				case 0:
					this.registerA = value;
					break;

				case 1:
					this.registerB = value;
					break;

				case 2:
					this.registerX = value;
					break;

				case 3:
					this.registerY = value;
					break;
			}
		}


		protected int RotateRight(int value)
		{
			bool bit = (value & 0x01) != 0;

			value = value >> 1;

			if (bit)
			{
				value |= 0x80;
			}

			this.SetFlag(TinyProcessor.FlagCarry, bit);
			return value;
		}

		protected int RotateLeft(int value)
		{
			bool bit = (value & 0x80) != 0;

			value = value << 1;

			if (bit)
			{
				value |= 0x01;
			}

			this.SetFlag(TinyProcessor.FlagCarry, bit);
			return value;
		}


		protected void StackPushWord(int value)
		{
			this.memory.Write(this.DecSP, value & 0xff);
			this.memory.Write(this.DecSP, (value >> 8) & 0xff);
		}

		protected int StackPopWord()
		{
			int value = this.memory.Read(this.IncSP) << 8;
			value |= this.memory.Read(this.IncSP);
			return value;
		}

		protected void StackPushByte(int value)
		{
			this.memory.Write(this.DecSP, value);
		}

		protected int StackPopByte()
		{
			return this.memory.Read(this.IncSP);
		}

		protected int DecSP
		{
			get
			{
				this.registerSP--;
				this.registerSP &= 0x7ff;
				return this.registerSP;
			}
		}

		protected int IncSP
		{
			get
			{
				int sp = this.registerSP;
				this.registerSP++;
				this.registerSP &= 0x7ff;
				return sp;
			}
		}


		protected int ReadByte(int address)
		{
			//	Lit un byte signé.
			return this.BypeSignExtend(this.memory.Read(address));
		}

		protected int BypeSignExtend(int value)
		{
			if ((value & 0x80) != 0)  // valeur négative ?
			{
				value = (int) ((uint) value | 0xffffff00);
			}

			return value;
		}

		protected int ReadWord(int address)
		{
			return (this.memory.Read(address+0) << 8) | (this.memory.Read(address+1));
		}

		protected void WriteWord(int address, int data)
		{
			this.memory.Write(address+0, (data >> 8) & 0xff);
			this.memory.Write(address+1, data & 0xff);
		}

		protected int AddressAbs
		{
			get
			{
				int mode = (this.memory.Read(this.registerPC++) << 8) | (this.memory.Read(this.registerPC++));
				int address = mode & 0x0FFF;

				if ((mode & 0x8000) != 0)  // relatif ?
				{
					if ((address & 0x0800) != 0)  // offset négatif ?
					{
						address = address-0x1000;
					}

					address = this.registerPC + address;
				}

				if ((mode & 0x4000) != 0)  // +{X} ou +{Y} ?
				{
					if ((mode & 0x1000) == 0)
					{
						address += this.registerX;
					}
					else
					{
						address += this.registerY;
					}
				}

				return address;
			}
		}


		protected void SetFlagsCompare(int a, int b)
		{
			this.SetFlag(TinyProcessor.FlagZero, a == b);
			this.SetFlag(TinyProcessor.FlagCarry, a >= b);
		}

		protected int SetFlagsOper8(int value)
		{
			this.SetFlag(TinyProcessor.FlagZero, value == 0);
			this.SetFlag(TinyProcessor.FlagNeg, (value & 0x80) != 0);

			if ((value & 0x80) == 0)  // valeur positive ?
			{
				this.SetFlag(TinyProcessor.FlagOverflow, (value & 0xffffff00) != 0);
			}
			else  // valeur négative ?
			{
				this.SetFlag(TinyProcessor.FlagOverflow, (value & 0xffffff00) == 0);
			}

			return value & 0xff;
		}

		protected int SetFlagsOper16(int value)
		{
			this.SetFlag(TinyProcessor.FlagZero, value == 0);
			this.SetFlag(TinyProcessor.FlagNeg, (value & 0x8000) != 0);

			if ((value & 0x8000) == 0)  // valeur positive ?
			{
				this.SetFlag(TinyProcessor.FlagOverflow, (value & 0xffff0000) != 0);
			}
			else  // valeur négative ?
			{
				this.SetFlag(TinyProcessor.FlagOverflow, (value & 0xffff0000) == 0);
			}

			return value & 0xffff;
		}

		protected bool IsTestTrue(int op)
		{
			Instructions test = (Instructions) ((int) Instructions.Jump + (op & 0x0F));

			switch (test)
			{
				case Instructions.JumpEQ:
					return this.TestFlag(TinyProcessor.FlagZero);

				case Instructions.JumpNE:
					return !this.TestFlag(TinyProcessor.FlagZero);

				case Instructions.JumpLO:
					return !this.TestFlag(TinyProcessor.FlagZero) && !this.TestFlag(TinyProcessor.FlagCarry);

				case Instructions.JumpLS:
					return this.TestFlag(TinyProcessor.FlagZero) || !this.TestFlag(TinyProcessor.FlagCarry);

				case Instructions.JumpHI:
					return !this.TestFlag(TinyProcessor.FlagZero) && this.TestFlag(TinyProcessor.FlagCarry);

				case Instructions.JumpHS:
					return this.TestFlag(TinyProcessor.FlagZero) || this.TestFlag(TinyProcessor.FlagCarry);

				case Instructions.JumpVC:
					return !this.TestFlag(TinyProcessor.FlagOverflow);

				case Instructions.JumpVS:
					return this.TestFlag(TinyProcessor.FlagOverflow);

				case Instructions.JumpNC:
					return !this.TestFlag(TinyProcessor.FlagNeg);

				case Instructions.JumpNS:
					return this.TestFlag(TinyProcessor.FlagNeg);
			}

			return true;
		}

		protected void SetFlag(int flag, bool value)
		{
			this.registerF &= ~(1 << flag);

			if (value)
			{
				this.registerF |= (1 << flag);
			}
		}

		protected bool TestFlag(int flag)
		{
			return (this.registerF & (1 << flag)) != 0;
		}


		#region Register
		public override IEnumerable<string> RegisterNames
		{
			//	Enumère tous les noms de registres.
			get
			{
				yield return "PC";
				yield return "SP";
				yield return "F";
				yield return "A";
				yield return "B";
				yield return "X";
				yield return "Y";
			}
		}

		public override int GetRegisterSize(string name)
		{
			//	Retourne la taille (nombre de bits) d'un registre.
			switch (name)
			{
				case "PC":
				case "SP":
					return Memory.TotalAddress;

				case "F":
				case "A":
				case "B":
				case "X":
				case "Y":
					return Memory.TotalData;
			}

			return base.GetRegisterSize(name);
		}

		public override string GetRegisterBitNames(string name)
		{
			//	Retourne les noms des bits d'un registre.
			if (name == "F")
			{
				return "CZNV";  // bits 0..7 !
			}

			return null;
		}

		public override int GetRegisterValue(string name)
		{
			//	Retourne la valeur d'un registre.
			switch (name)
			{
				case "PC":
					return this.registerPC;

				case "SP":
					return this.registerSP;

				case "F":
					return this.registerF;

				case "A":
					return this.registerA;

				case "B":
					return this.registerB;

				case "X":
					return this.registerX;

				case "Y":
					return this.registerY;
			}

			return base.GetRegisterValue(name);
		}

		public override void SetRegisterValue(string name, int value)
		{
			//	Modifie la valeur d'un registre.
			switch (name)
			{
				case "PC":
					this.registerPC = value;
					break;

				case "SP":
					this.registerSP = value;
					break;

				case "F":
					this.registerF = value;
					break;

				case "A":
					this.registerA = value;
					break;

				case "B":
					this.registerB = value;
					break;

				case "X":
					this.registerX = value;
					break;

				case "Y":
					this.registerY = value;
					break;
			}
		}
		#endregion


		#region Rom
		public override void RomInitialise(int address)
		{
			//	Rempli la Rom.
			int indirect = address;
			address += 3*64;  // place pour 64 appels
			this.RomWrite(ref indirect, ref address, TinyProcessor.WaitKey);
			this.RomWrite(ref indirect, ref address, TinyProcessor.DisplayBinaryDigit);
			this.RomWrite(ref indirect, ref address, TinyProcessor.DisplayHexaDigit);
			this.RomWrite(ref indirect, ref address, TinyProcessor.DisplayHexaByte);
			this.RomWrite(ref indirect, ref address, TinyProcessor.DisplayDecimal);
			this.RomWrite(ref indirect, ref address, TinyProcessor.SetPixel);
			this.RomWrite(ref indirect, ref address, TinyProcessor.ClrPixel);
		}

		protected void RomWrite(ref int indirect, ref int address, byte[] code)
		{
			this.memory.WriteRom(indirect++, (byte) Instructions.Jump);
			this.memory.WriteRom(indirect++, (address >> 8) & 0xff);
			this.memory.WriteRom(indirect++, address & 0xff);

			foreach (byte data in code)
			{
				this.memory.WriteRom(address++, data);
			}
		}

		//	Attend la pression d'une touche du clavier simulé.
		//	in	-
		//	out	A touche pressée
		//	mod	A
		protected static byte[] WaitKey =
		{
			(byte) Instructions.Ret,				// RET
		};

		//	Affiche des segments à choix.
		//	in	A segments à allumer
		//		B digit 0..3
		//	out	-
		//	mod	-
		protected static byte[] DisplayBinaryDigit =
		{
			(byte) Instructions.Ret,				// RET
		};

		//	Affiche un digit hexadécimal.
		//	in	A valeur 0..15
		//		B digit 0..3
		//	out	-
		//	mod	-
		protected static byte[] DisplayHexaDigit =
		{
			(byte) Instructions.Ret,				// RET
													// TABLE:
			0x3F, 0x03, 0x6D, 0x67, 0x53, 0x76, 0x7E, 0x23, 0x7F, 0x77, 0x7B, 0x5E, 0x3C, 0x4F, 0x7C, 0x78,
		};

		//	Affiche un byte hexadécimal sur deux digits.
		//	in	A valeur 0..255
		//		B premier digit 0..2
		//	out	-
		//	mod	-
		protected static byte[] DisplayHexaByte =
		{
			(byte) Instructions.Ret,				// RET
		};

		//	Affiche une valeur décimale.
		//	in	HL valeur
		//	out	-
		//	mod	-
		protected static byte[] DisplayDecimal =
		{
			(byte) Instructions.Ret,				// RET
		};

		//	Allume un pixel dans l'écran bitmap.
		//	in	A coordonnée X 0..31
		//		B coordonnée Y 0..23
		//	out	-
		//	mod	-
		protected static byte[] SetPixel =
		{
			(byte) Instructions.Ret,				// RET
		};

		//	Eteint un pixel dans l'écran bitmap.
		//	in	A coordonnée X 0..31
		//		B coordonnée Y 0..23
		//	out	-
		//	mod	-
		protected static byte[] ClrPixel =
		{
			(byte) Instructions.Ret,				// RET
		};
		#endregion


		#region Chapters
		public override List<string> HelpChapters
		{
			//	Retourne la liste des chapitres.
			get
			{
				List<string> chapters = new List<string>();
				
				chapters.Add("Intro");
				chapters.Add("Data");
				chapters.Add("Op");
				chapters.Add("Branch");
				chapters.Add("ROM");
				
				return chapters;
			}
		}

		public override string HelpChapter(string chapter)
		{
			//	Retourne le texte d'un chapitre.
			System.Text.StringBuilder builder = new System.Text.StringBuilder();

			switch (chapter)
			{
				case "Intro":
					AbstractProcessor.HelpPutTitle(builder, "Binaire et hexadécimal");
					AbstractProcessor.HelpPutLine(builder, "(<i>décimal: binaire = hexa</i>)");
					AbstractProcessor.HelpPutLine(builder, "  0: 0000 = 0");
					AbstractProcessor.HelpPutLine(builder, "  1: 0001 = 1");
					AbstractProcessor.HelpPutLine(builder, "  2: 0010 = 2");
					AbstractProcessor.HelpPutLine(builder, "  3: 0011 = 3");
					AbstractProcessor.HelpPutLine(builder, "  4: 0100 = 4");
					AbstractProcessor.HelpPutLine(builder, "  5: 0101 = 5");
					AbstractProcessor.HelpPutLine(builder, "  6: 0110 = 6");
					AbstractProcessor.HelpPutLine(builder, "  7: 0111 = 7");
					AbstractProcessor.HelpPutLine(builder, "  8: 1000 = 8");
					AbstractProcessor.HelpPutLine(builder, "  9: 1001 = 9");
					AbstractProcessor.HelpPutLine(builder, "10: 1010 = A");
					AbstractProcessor.HelpPutLine(builder, "11: 1011 = B");
					AbstractProcessor.HelpPutLine(builder, "12: 1100 = C");
					AbstractProcessor.HelpPutLine(builder, "13: 1101 = D");
					AbstractProcessor.HelpPutLine(builder, "14: 1110 = E");
					AbstractProcessor.HelpPutLine(builder, "15: 1111 = F");

					AbstractProcessor.HelpPutTitle(builder, "Espace d'adressage");
					AbstractProcessor.HelpPutLine(builder, "[000]..[7FF] :<tab/>RAM");
					AbstractProcessor.HelpPutLine(builder, "[800]..[BFF] :<tab/>ROM");
					AbstractProcessor.HelpPutLine(builder, "[C00]..[C10] :<tab/>Périphériques");
					AbstractProcessor.HelpPutLine(builder, "[C80]..[CDF] :<tab/>Ecran bitmap");

					AbstractProcessor.HelpPutTitle(builder, "Affichage");
					AbstractProcessor.HelpPutLine(builder, "L'affichage est constitué de 4 afficheurs à 7 segments (plus un point décimal), numérotés de droite à gauche. On peut écrire une valeur pour mémoriser les digits à allumer, ou relire cette valeur.");
					AbstractProcessor.HelpPutLine(builder, "[C00] :<tab/>Premier digit (celui de gauche).");
					AbstractProcessor.HelpPutLine(builder, "[C01] :<tab/>Deuxième digit.");
					AbstractProcessor.HelpPutLine(builder, "[C02] :<tab/>Troisième digit.");
					AbstractProcessor.HelpPutLine(builder, "[C03] :<tab/>Quatrième digit (celui de droite).");
					AbstractProcessor.HelpPutLine(builder, "");
					AbstractProcessor.HelpPutLine(builder, "bit 0<tab/>Segment vertical supérieur droite.");
					AbstractProcessor.HelpPutLine(builder, "bit 1<tab/>Segment vertical inférieur droite.");
					AbstractProcessor.HelpPutLine(builder, "bit 2<tab/>Segment horizontal inférieur.");
					AbstractProcessor.HelpPutLine(builder, "bit 3<tab/>Segment vertical inférieur gauche.");
					AbstractProcessor.HelpPutLine(builder, "bit 4<tab/>Segment vertical supérieur gauche.");
					AbstractProcessor.HelpPutLine(builder, "bit 5<tab/>Segment horizontal supérieur.");
					AbstractProcessor.HelpPutLine(builder, "bit 6<tab/>Segment horizontal du milieu.");
					AbstractProcessor.HelpPutLine(builder, "bit 7<tab/>Point décimal.");

					AbstractProcessor.HelpPutTitle(builder, "Clavier");
					AbstractProcessor.HelpPutLine(builder, "Le clavier est constitué de 8 touches nommées 0..7, plus 2 touches super-shift.");
					AbstractProcessor.HelpPutLine(builder, "[C07] :<tab/>Clavier.");
					AbstractProcessor.HelpPutLine(builder, "");
					AbstractProcessor.HelpPutLine(builder, "bits 0..2<tab/>Touches 0..7.");
					AbstractProcessor.HelpPutLine(builder, "bit 3<tab/>Touche Shift.");
					AbstractProcessor.HelpPutLine(builder, "bit 4<tab/>Touche Ctrl.");
					AbstractProcessor.HelpPutLine(builder, "bit 7<tab/>Prend la valeur 1 lorsqu'une touche 0..7 est pressée. Est automatiquement remis à zéro lorsque l'adresse [C07] est lue.");

					AbstractProcessor.HelpPutTitle(builder, "Ecran bitmap");
					AbstractProcessor.HelpPutLine(builder, "L'écran bitmap est un écran vidéo monochrome de 32 x 24 pixels. Chaque byte représente 8 pixels horizontaux, avec le bit 7 à gauche.");
					AbstractProcessor.HelpPutLine(builder, "");
					AbstractProcessor.HelpPutLine(builder, "[C80]..[C83] :<tab/>1ère ligne de 32 pixels.");
					AbstractProcessor.HelpPutLine(builder, "[C84]..[C87] :<tab/>2ème ligne de 32 pixels.");
					AbstractProcessor.HelpPutLine(builder, "...");
					AbstractProcessor.HelpPutLine(builder, "[CDC]..[CDF] :<tab/>24ème ligne de 32 pixels.");
					break;

				case "Data":
					AbstractProcessor.HelpPutTitle(builder, "Valeur immédiate");
					AbstractProcessor.HelpPutLine(builder, "[40] [xx] :<tab/>MOVE #xx,A");
					AbstractProcessor.HelpPutLine(builder, "[41] [xx] :<tab/>MOVE #xx,B");
					AbstractProcessor.HelpPutLine(builder, "[42] [xx] [yy] :<tab/>MOVE #xxyy,HL");
					AbstractProcessor.HelpPutLine(builder, "[43] [dd] :<tab/>MOVE #R^dd,HL");

					AbstractProcessor.HelpPutTitle(builder, "Registre à registre");
					AbstractProcessor.HelpPutLine(builder, "[44] :<tab/><tab/>MOVE B,A");
					AbstractProcessor.HelpPutLine(builder, "[45] :<tab/><tab/>MOVE A,B");
					AbstractProcessor.HelpPutLine(builder, "[4A] :<tab/><tab/>MOVE H,A");
					AbstractProcessor.HelpPutLine(builder, "[4B] :<tab/><tab/>MOVE L,A");
					AbstractProcessor.HelpPutLine(builder, "[4C] :<tab/><tab/>MOVE A,H");
					AbstractProcessor.HelpPutLine(builder, "[4D] :<tab/><tab/>MOVE A,L");
					AbstractProcessor.HelpPutLine(builder, "[4E] :<tab/><tab/>SWAP A,B");

					AbstractProcessor.HelpPutTitle(builder, "Registre à mémoire");
					AbstractProcessor.HelpPutLine(builder, "[46] [hh] [ll] :<tab/>MOVE hhll,A");
					AbstractProcessor.HelpPutLine(builder, "[47] [hh] [ll] :<tab/>MOVE A,hhll");

					AbstractProcessor.HelpPutTitle(builder, "Indexé indirect");
					AbstractProcessor.HelpPutLine(builder, "[48] :<tab/><tab/>MOVE {HL},A");
					AbstractProcessor.HelpPutLine(builder, "[49] :<tab/><tab/>MOVE A,{HL}");

					AbstractProcessor.HelpPutTitle(builder, "Stack");
					AbstractProcessor.HelpPutLine(builder, "[80] :<tab/><tab/>PUSH A");
					AbstractProcessor.HelpPutLine(builder, "[81] :<tab/><tab/>PUSH B");
					AbstractProcessor.HelpPutLine(builder, "[82] :<tab/><tab/>PUSH HL");
					AbstractProcessor.HelpPutLine(builder, "[83] :<tab/><tab/>PUSH F");
					AbstractProcessor.HelpPutLine(builder, "");
					AbstractProcessor.HelpPutLine(builder, "[84] :<tab/><tab/>POP A");
					AbstractProcessor.HelpPutLine(builder, "[85] :<tab/><tab/>POP B");
					AbstractProcessor.HelpPutLine(builder, "[86] :<tab/><tab/>POP HL");
					AbstractProcessor.HelpPutLine(builder, "[87] :<tab/><tab/>POP F");
					break;

				case "Op":
					AbstractProcessor.HelpPutTitle(builder, "Opérations arithmétiques");
					AbstractProcessor.HelpPutLine(builder, "[50] [xx] :<tab/>ADD #xx,A  <tab/>(Z, N, V)");
					AbstractProcessor.HelpPutLine(builder, "[51] :<tab/><tab/>ADD B,A   <tab/>(Z, N, V)");
					AbstractProcessor.HelpPutLine(builder, "[52] [xx] :<tab/>SUB #xx,A  <tab/>(Z, N, V)");
					AbstractProcessor.HelpPutLine(builder, "[53] :<tab/><tab/>SUB B,A   <tab/>(Z, N, V)");
					AbstractProcessor.HelpPutLine(builder, "[90] [xx] :<tab/>MUL #xx,A  <tab/>(Z, N, V)");
					AbstractProcessor.HelpPutLine(builder, "[91] :<tab/><tab/>MUL B,A   <tab/>(Z, N, V)");
					AbstractProcessor.HelpPutLine(builder, "[95] [xx] :<tab/>DIV #xx,A  <tab/>(Z, N, V)");
					AbstractProcessor.HelpPutLine(builder, "[96] :<tab/><tab/>DIV B,A   <tab/>(Z, N, V)");
					AbstractProcessor.HelpPutLine(builder, "[9A] [xx] :<tab/>MOD #xx,A  <tab/>(Z, N, V)");
					AbstractProcessor.HelpPutLine(builder, "[9B] :<tab/><tab/>MOD B,A   <tab/>(Z, N, V)");
					AbstractProcessor.HelpPutLine(builder, "");
					AbstractProcessor.HelpPutLine(builder, "[5A] [xx] :<tab/>ADD #xx,HL  <tab/>(Z, N, V)");
					AbstractProcessor.HelpPutLine(builder, "[5B] :<tab/><tab/>ADD A,HL   <tab/>(Z, N, V)");
					AbstractProcessor.HelpPutLine(builder, "[5C] :<tab/><tab/>ADD B,HL   <tab/>(Z, N, V)");
					AbstractProcessor.HelpPutLine(builder, "[5D] [xx] :<tab/>SUB #xx,HL  <tab/>(Z, N, V)");
					AbstractProcessor.HelpPutLine(builder, "[5E] :<tab/><tab/>SUB A,HL   <tab/>(Z, N, V)");
					AbstractProcessor.HelpPutLine(builder, "[5F] :<tab/><tab/>SUB B,HL   <tab/>(Z, N, V)");
					AbstractProcessor.HelpPutLine(builder, "[92] [xx] :<tab/>MUL #xx,HL  <tab/>(Z, N, V)");
					AbstractProcessor.HelpPutLine(builder, "[93] :<tab/><tab/>MUL A,HL   <tab/>(Z, N, V)");
					AbstractProcessor.HelpPutLine(builder, "[94] :<tab/><tab/>MUL B,HL   <tab/>(Z, N, V)");
					AbstractProcessor.HelpPutLine(builder, "[97] [xx] :<tab/>DIV #xx,HL  <tab/>(Z, N, V)");
					AbstractProcessor.HelpPutLine(builder, "[98] :<tab/><tab/>DIV A,HL   <tab/>(Z, N, V)");
					AbstractProcessor.HelpPutLine(builder, "[99] :<tab/><tab/>DIV B,HL   <tab/>(Z, N, V)");
					AbstractProcessor.HelpPutLine(builder, "[9C] [xx] :<tab/>MOD #xx,HL  <tab/>(Z, N, V)");
					AbstractProcessor.HelpPutLine(builder, "[9D] :<tab/><tab/>MOD A,HL   <tab/>(Z, N, V)");
					AbstractProcessor.HelpPutLine(builder, "[9E] :<tab/><tab/>MOD B,HL   <tab/>(Z, N, V)");

					AbstractProcessor.HelpPutTitle(builder, "Opérations logiques");
					AbstractProcessor.HelpPutLine(builder, "[54] [xx] :<tab/>AND #xx,A   <tab/>(Z, N, V)");
					AbstractProcessor.HelpPutLine(builder, "[56] [xx] :<tab/>OR #xx,A    <tab/>(Z, N, V)");
					AbstractProcessor.HelpPutLine(builder, "[58] [xx] :<tab/>XOR #xx,A   <tab/>(Z, N, V)");
					AbstractProcessor.HelpPutLine(builder, "");
					AbstractProcessor.HelpPutLine(builder, "[55] :<tab/><tab/>AND B,A    <tab/>(Z, N, V)");
					AbstractProcessor.HelpPutLine(builder, "[57] :<tab/><tab/>OR B,A     <tab/>(Z, N, V)");
					AbstractProcessor.HelpPutLine(builder, "[59] :<tab/><tab/>XOR B,A    <tab/>(Z, N, V)");
					AbstractProcessor.HelpPutLine(builder, "");
					AbstractProcessor.HelpPutLine(builder, "[68] :<tab/><tab/>RR A       <tab/>(Z, N, V)");
					AbstractProcessor.HelpPutLine(builder, "[69] :<tab/><tab/>RR B       <tab/>(Z, N, V)");
					AbstractProcessor.HelpPutLine(builder, "[6A] :<tab/><tab/>RL A       <tab/>(Z, N, V)");
					AbstractProcessor.HelpPutLine(builder, "[6B] :<tab/><tab/>RL B       <tab/>(Z, N, V)");

					AbstractProcessor.HelpPutTitle(builder, "Comparaisons");
					AbstractProcessor.HelpPutLine(builder, "[70] [xx] :<tab/>COMP #xx,A  <tab/>(C, Z)");
					AbstractProcessor.HelpPutLine(builder, "[71] [xx] :<tab/>COMP #xx,B  <tab/>(C, Z)");
					AbstractProcessor.HelpPutLine(builder, "[72] [xx] [yy] :<tab/>COMP #xxyy,HL<tab/>(C, Z)");
					AbstractProcessor.HelpPutLine(builder, "[73] :<tab/><tab/>COMP B,A   <tab/>(C, Z)");

					AbstractProcessor.HelpPutTitle(builder, "Compteurs");
					AbstractProcessor.HelpPutLine(builder, "[60] :<tab/><tab/>INC A      <tab/>(Z, N, V)");
					AbstractProcessor.HelpPutLine(builder, "[61] :<tab/><tab/>INC B      <tab/>(Z, N, V)");
					AbstractProcessor.HelpPutLine(builder, "[62] :<tab/><tab/>INC HL     <tab/>(Z, N, V)");
					AbstractProcessor.HelpPutLine(builder, "");
					AbstractProcessor.HelpPutLine(builder, "[64] :<tab/><tab/>DEC A      <tab/>(Z, N, V)");
					AbstractProcessor.HelpPutLine(builder, "[65] :<tab/><tab/>DEC B      <tab/>(Z, N, V)");
					AbstractProcessor.HelpPutLine(builder, "[66] :<tab/><tab/>DEC HL     <tab/>(Z, N, V)");

					AbstractProcessor.HelpPutTitle(builder, "Opérations sur des bits");
					AbstractProcessor.HelpPutLine(builder, "[74] [0b] :<tab/>TEST A:#b   <tab/>(Z)");
					AbstractProcessor.HelpPutLine(builder, "[76] [0b] :<tab/>TCLR A:#b   <tab/>(Z)");
					AbstractProcessor.HelpPutLine(builder, "[78] [0b] :<tab/>TSET A:#b   <tab/>(Z)");
					AbstractProcessor.HelpPutLine(builder, "");
					AbstractProcessor.HelpPutLine(builder, "[75] :<tab/><tab/>TEST A:B   <tab/>(Z)");
					AbstractProcessor.HelpPutLine(builder, "[77] :<tab/><tab/>TCLR A:B   <tab/>(Z)");
					AbstractProcessor.HelpPutLine(builder, "[79] :<tab/><tab/>TSET A:B   <tab/>(Z)");

					AbstractProcessor.HelpPutTitle(builder, "Flags");
					AbstractProcessor.HelpPutLine(builder, "[7A] :<tab/><tab/>SETC       <tab/>(C=1)");
					AbstractProcessor.HelpPutLine(builder, "[7B] :<tab/><tab/>CLRC       <tab/>(C=0)");
					AbstractProcessor.HelpPutLine(builder, "");
					AbstractProcessor.HelpPutLine(builder, "[7C] :<tab/><tab/>SETV       <tab/>(V=1)");
					AbstractProcessor.HelpPutLine(builder, "[7D] :<tab/><tab/>CLRV       <tab/>(V=0)");

					AbstractProcessor.HelpPutTitle(builder, "Spécial");
					AbstractProcessor.HelpPutLine(builder, "[00] :<tab/><tab/>NOP");
					AbstractProcessor.HelpPutLine(builder, "[1F] :<tab/><tab/>HALT");
					break;

				case "Branch":
					AbstractProcessor.HelpPutTitle(builder, "Sauts");
					AbstractProcessor.HelpPutLine(builder, "[01] [hh] [ll] :<tab/>JUMP hhll");
					AbstractProcessor.HelpPutLine(builder, "[02] [hh] [ll] :<tab/>JUMP,EQ hhll");
					AbstractProcessor.HelpPutLine(builder, "[03] [hh] [ll] :<tab/>JUMP,NE hhll");
					AbstractProcessor.HelpPutLine(builder, "[04] [hh] [ll] :<tab/>JUMP,LO hhll");
					AbstractProcessor.HelpPutLine(builder, "[05] [hh] [ll] :<tab/>JUMP,LS hhll");
					AbstractProcessor.HelpPutLine(builder, "[06] [hh] [ll] :<tab/>JUMP,HI hhll");
					AbstractProcessor.HelpPutLine(builder, "[07] [hh] [ll] :<tab/>JUMP,HS hhll");
					AbstractProcessor.HelpPutLine(builder, "[08] [hh] [ll] :<tab/>JUMP,VC hhll");
					AbstractProcessor.HelpPutLine(builder, "[09] [hh] [ll] :<tab/>JUMP,VS hhll");
					AbstractProcessor.HelpPutLine(builder, "[0A] [hh] [ll] :<tab/>JUMP,NC hhll");
					AbstractProcessor.HelpPutLine(builder, "[0B] [hh] [ll] :<tab/>JUMP,NS hhll");
					AbstractProcessor.HelpPutLine(builder, "");
					AbstractProcessor.HelpPutLine(builder, "[11] [dd] :<tab/>JUMP R^dd");
					AbstractProcessor.HelpPutLine(builder, "[12] [dd] :<tab/>JUMP,EQ R^dd");
					AbstractProcessor.HelpPutLine(builder, "[13] [dd] :<tab/>JUMP,NE R^dd");
					AbstractProcessor.HelpPutLine(builder, "[14] [dd] :<tab/>JUMP,LO R^dd");
					AbstractProcessor.HelpPutLine(builder, "[15] [dd] :<tab/>JUMP,LS R^dd");
					AbstractProcessor.HelpPutLine(builder, "[16] [dd] :<tab/>JUMP,HI R^dd");
					AbstractProcessor.HelpPutLine(builder, "[17] [dd] :<tab/>JUMP,HS R^dd");
					AbstractProcessor.HelpPutLine(builder, "[18] [dd] :<tab/>JUMP,VC R^dd");
					AbstractProcessor.HelpPutLine(builder, "[19] [dd] :<tab/>JUMP,VS R^dd");
					AbstractProcessor.HelpPutLine(builder, "[1A] [dd] :<tab/>JUMP,NC R^dd");
					AbstractProcessor.HelpPutLine(builder, "[1B] [dd] :<tab/>JUMP,NS R^dd");
					AbstractProcessor.HelpPutLine(builder, "");
					AbstractProcessor.HelpPutLine(builder, "[10] :<tab/>JUMP {HL}");

					AbstractProcessor.HelpPutTitle(builder, "Appels de routines");
					AbstractProcessor.HelpPutLine(builder, "[21] [hh] [ll] :<tab/>CALL hhll");
					AbstractProcessor.HelpPutLine(builder, "[22] [hh] [ll] :<tab/>CALL,EQ hhll");
					AbstractProcessor.HelpPutLine(builder, "[23] [hh] [ll] :<tab/>CALL,NE hhll");
					AbstractProcessor.HelpPutLine(builder, "[24] [hh] [ll] :<tab/>CALL,LO hhll");
					AbstractProcessor.HelpPutLine(builder, "[25] [hh] [ll] :<tab/>CALL,LS hhll");
					AbstractProcessor.HelpPutLine(builder, "[26] [hh] [ll] :<tab/>CALL,HI hhll");
					AbstractProcessor.HelpPutLine(builder, "[27] [hh] [ll] :<tab/>CALL,HS hhll");
					AbstractProcessor.HelpPutLine(builder, "[28] [hh] [ll] :<tab/>CALL,VC hhll");
					AbstractProcessor.HelpPutLine(builder, "[29] [hh] [ll] :<tab/>CALL,VS hhll");
					AbstractProcessor.HelpPutLine(builder, "[2A] [hh] [ll] :<tab/>CALL,NC hhll");
					AbstractProcessor.HelpPutLine(builder, "[2B] [hh] [ll] :<tab/>CALL,NS hhll");
					AbstractProcessor.HelpPutLine(builder, "");
					AbstractProcessor.HelpPutLine(builder, "[31] [dd] :<tab/>CALL R^dd");
					AbstractProcessor.HelpPutLine(builder, "[32] [dd] :<tab/>CALL,EQ R^dd");
					AbstractProcessor.HelpPutLine(builder, "[33] [dd] :<tab/>CALL,NE R^dd");
					AbstractProcessor.HelpPutLine(builder, "[34] [dd] :<tab/>CALL,LO R^dd");
					AbstractProcessor.HelpPutLine(builder, "[35] [dd] :<tab/>CALL,LS R^dd");
					AbstractProcessor.HelpPutLine(builder, "[36] [dd] :<tab/>CALL,HI R^dd");
					AbstractProcessor.HelpPutLine(builder, "[37] [dd] :<tab/>CALL,HS R^dd");
					AbstractProcessor.HelpPutLine(builder, "[38] [dd] :<tab/>CALL,VC R^dd");
					AbstractProcessor.HelpPutLine(builder, "[39] [dd] :<tab/>CALL,VS R^dd");
					AbstractProcessor.HelpPutLine(builder, "[3A] [dd] :<tab/>CALL,NC R^dd");
					AbstractProcessor.HelpPutLine(builder, "[3B] [dd] :<tab/>CALL,NS R^dd");
					AbstractProcessor.HelpPutLine(builder, "");
					AbstractProcessor.HelpPutLine(builder, "[30] :<tab/><tab/>CALL {HL}");
					AbstractProcessor.HelpPutLine(builder, "");
					AbstractProcessor.HelpPutLine(builder, "[3F] :<tab/><tab/>Ret");
					break;

				case "ROM":
					AbstractProcessor.HelpPutTitle(builder, "WaitKey");
					AbstractProcessor.HelpPutLine(builder, "Attend la pression d'une touche du clavier.");
					AbstractProcessor.HelpPutLine(builder, "[21] [08] [00] :<tab/>CALL WaitKey");
					AbstractProcessor.HelpPutLine(builder, "in :<tab/>-");
					AbstractProcessor.HelpPutLine(builder, "out :<tab/>A touche pressée");
					AbstractProcessor.HelpPutLine(builder, "mod :<tab/>A");

					AbstractProcessor.HelpPutTitle(builder, "DisplayBinaryDigit");
					AbstractProcessor.HelpPutLine(builder, "Affiche des segments à choix.");
					AbstractProcessor.HelpPutLine(builder, "[21] [08] [03] :<tab/>CALL DisplayBinaryDigit");
					AbstractProcessor.HelpPutLine(builder, "in :<tab/>A bits des segments à allumer");
					AbstractProcessor.HelpPutLine(builder, "<tab/>B digit 0..3 (de gauche à droite)");
					AbstractProcessor.HelpPutLine(builder, "out :<tab/>-");
					AbstractProcessor.HelpPutLine(builder, "mod :<tab/>-");

					AbstractProcessor.HelpPutTitle(builder, "DisplayHexaDigit");
					AbstractProcessor.HelpPutLine(builder, "Affiche un digit hexadécimal.");
					AbstractProcessor.HelpPutLine(builder, "[21] [08] [06] :<tab/>CALL DisplayHexaDigit");
					AbstractProcessor.HelpPutLine(builder, "in :<tab/>A valeur 0..15");
					AbstractProcessor.HelpPutLine(builder, "<tab/>B digit 0..3 (de droite à gauche)");
					AbstractProcessor.HelpPutLine(builder, "out :<tab/>-");
					AbstractProcessor.HelpPutLine(builder, "mod :<tab/>-");

					AbstractProcessor.HelpPutTitle(builder, "DisplayHexaByte");
					AbstractProcessor.HelpPutLine(builder, "Affiche un byte hexadécimal sur deux digits.");
					AbstractProcessor.HelpPutLine(builder, "[21] [08] [09] :<tab/>CALL DisplayHexaByte");
					AbstractProcessor.HelpPutLine(builder, "in :<tab/>A valeur 0..255");
					AbstractProcessor.HelpPutLine(builder, "<tab/>B premier digit 0..2 (de gauche à droite)");
					AbstractProcessor.HelpPutLine(builder, "out :<tab/>-");
					AbstractProcessor.HelpPutLine(builder, "mod :<tab/>-");

					AbstractProcessor.HelpPutTitle(builder, "DisplayDecimal");
					AbstractProcessor.HelpPutLine(builder, "Affiche une valeur décimale sur quatre digits.");
					AbstractProcessor.HelpPutLine(builder, "[21] [08] [0C] :<tab/>CALL DisplayDecimal");
					AbstractProcessor.HelpPutLine(builder, "in :<tab/>HL valeur");
					AbstractProcessor.HelpPutLine(builder, "out :<tab/>-");
					AbstractProcessor.HelpPutLine(builder, "mod :<tab/>-");

					AbstractProcessor.HelpPutTitle(builder, "SetPixel");
					AbstractProcessor.HelpPutLine(builder, "Allume un pixel dans l'écran bitmap.");
					AbstractProcessor.HelpPutLine(builder, "[21] [08] [0F] :<tab/>CALL SetPixel");
					AbstractProcessor.HelpPutLine(builder, "in :<tab/>A coordonnée X 0..31");
					AbstractProcessor.HelpPutLine(builder, "<tab/>B coordonnée Y 0..23");
					AbstractProcessor.HelpPutLine(builder, "out :<tab/>-");
					AbstractProcessor.HelpPutLine(builder, "mod :<tab/>-");

					AbstractProcessor.HelpPutTitle(builder, "ClrPixel");
					AbstractProcessor.HelpPutLine(builder, "Eteint un pixel dans l'écran bitmap.");
					AbstractProcessor.HelpPutLine(builder, "[21] [08] [12] :<tab/>CALL ClrPixel");
					AbstractProcessor.HelpPutLine(builder, "in :<tab/>A coordonnée X 0..31");
					AbstractProcessor.HelpPutLine(builder, "<tab/>B coordonnée Y 0..23");
					AbstractProcessor.HelpPutLine(builder, "out :<tab/>-");
					AbstractProcessor.HelpPutLine(builder, "mod :<tab/>-");
					break;
			}

			return builder.ToString();
		}
		#endregion


		protected static readonly int FlagCarry    = 0;
		protected static readonly int FlagZero     = 1;
		protected static readonly int FlagNeg      = 2;
		protected static readonly int FlagOverflow = 3;

		
		protected int registerPC;  // program counter
		protected int registerSP;  // stack pointer
		protected int registerF;   // flags
		protected int registerA;   // accumulateur 8 bits
		protected int registerB;   // accumulateur 8 bits
		protected int registerX;   // pointeur 8 bits
		protected int registerY;   // pointeur 8 bits
	}
}
