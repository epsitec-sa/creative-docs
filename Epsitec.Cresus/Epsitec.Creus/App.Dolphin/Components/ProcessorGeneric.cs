//	Copyright © 2003-2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;

namespace Epsitec.App.Dolphin.Components
{
	/// <summary>
	/// Processeur générique pas très réussi.
	/// </summary>
	public class ProcessorGeneric : AbstractProcessor
	{
		protected enum Instructions
		{
			Nop = 0x00,

			JumpAbs   = 0x01,
			JumpAbsEQ = 0x02,
			JumpAbsNE = 0x03,
			JumpAbsLO = 0x04,
			JumpAbsLS = 0x05,
			JumpAbsHI = 0x06,
			JumpAbsHS = 0x07,
			JumpAbsVC = 0x08,
			JumpAbsVS = 0x09,
			JumpAbsNC = 0x0A,
			JumpAbsNS = 0x0B,

			JumpHL    = 0x10,
			JumpRel   = 0x11,
			JumpRelEQ = 0x12,
			JumpRelNE = 0x13,
			JumpRelLO = 0x14,
			JumpRelLS = 0x15,
			JumpRelHI = 0x16,
			JumpRelHS = 0x17,
			JumpRelVC = 0x18,
			JumpRelVS = 0x19,
			JumpRelNC = 0x1A,
			JumpRelNS = 0x1B,

			Halt = 0x1F,

			CallAbs   = 0x21,
			CallAbsEQ = 0x22,
			CallAbsNE = 0x23,
			CallAbsLO = 0x24,
			CallAbsLS = 0x25,
			CallAbsHI = 0x26,
			CallAbsHS = 0x27,
			CallAbsVC = 0x28,
			CallAbsVS = 0x29,
			CallAbsNC = 0x2A,
			CallAbsNS = 0x2B,

			CallHL    = 0x30,
			CallRel   = 0x31,
			CallRelEQ = 0x32,
			CallRelNE = 0x33,
			CallRelLO = 0x34,
			CallRelLS = 0x35,
			CallRelHI = 0x36,
			CallRelHS = 0x37,
			CallRelVC = 0x38,
			CallRelVS = 0x39,
			CallRelNC = 0x3A,
			CallRelNS = 0x3B,

			Ret = 0x3F,

			MoveiA   = 0x40,
			MoveiB   = 0x41,
			MoveiHL  = 0x42,
			MoverHL  = 0x43,
			MoveBA   = 0x44,
			MoveAB   = 0x45,
			MovemA   = 0x46,
			MoveAm   = 0x47,
			MovecHLA = 0x48,
			MoveAcHL = 0x49,
			MoveHA   = 0x4A,
			MoveLA   = 0x4B,
			MoveAH   = 0x4C,
			MoveAL   = 0x4D,
			SwapAB   = 0x4E,

			AddiA  = 0x50,
			AddBA  = 0x51,
			SubiA  = 0x52,
			SubBA  = 0x53,
			AndiA  = 0x54,
			AndBA  = 0x55,
			OriA   = 0x56,
			OrBA   = 0x57,
			XoriA  = 0x58,
			XorBA  = 0x59,
			AddiHL = 0x5A,
			AddAHL = 0x5B,
			AddBHL = 0x5C,
			SubiHL = 0x5D,
			SubAHL = 0x5E,
			SubBHL = 0x5F,
			
			IncA   = 0x60,
			IncB   = 0x61,
			IncHL  = 0x62,
			DecA   = 0x64,
			DecB   = 0x65,
			DecHL  = 0x66,
			RRA    = 0x68,
			RRB    = 0x69,
			RLA    = 0x6A,
			RLB    = 0x6B,

			CompiA  = 0x70,
			CompiB  = 0x71,
			CompiHL = 0x72,
			CompBA  = 0x73,
			TestiA  = 0x74,
			TestBA  = 0x75,
			TClriA  = 0x76,
			TClrBA  = 0x77,
			TSetiA  = 0x78,
			TSetBA  = 0x79,
			SetC    = 0x7A,
			ClrC    = 0x7B,
			SetV    = 0x7C,
			ClrV    = 0x7D,

			PushA  = 0x80,
			PushB  = 0x81,
			PushHL = 0x82,
			PushF  = 0x83,
			PopA   = 0x84,
			PopB   = 0x85,
			PopHL  = 0x86,
			PopF   = 0x87,

			MuliA  = 0x90,
			MulBA  = 0x91,
			MuliHL = 0x92,
			MulAHL = 0x93,
			MulBHL = 0x94,
			DiviA  = 0x95,
			DivBA  = 0x96,
			DiviHL = 0x97,
			DivAHL = 0x98,
			DivBHL = 0x99,
			ModiA  = 0x9A,
			ModBA  = 0x9B,
			ModiHL = 0x9C,
			ModAHL = 0x9D,
			ModBHL = 0x9E,
		}


		public ProcessorGeneric(Memory memory) : base(memory)
		{
			//	Constructeur du processeur.
		}

		public override string Name
		{
			//	Nom du processeur.
			get
			{
				return "Generic";
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
			this.registerHL = 0;
		}

		public override bool IsCall(out int retAddress)
		{
			//	Indique si le processeur est sur une instruction CALL.
			//	Si oui, retourne l'adresse après le CALL.
			Instructions op = (Instructions) this.memory.Read(this.registerPC);

			if (op >= Instructions.CallAbs && op <= Instructions.CallAbsNS)
			{
				retAddress = this.registerPC+3;
				return true;
			}
			else if (op >= Instructions.CallRel && op <= Instructions.CallRelNS)
			{
				retAddress = this.registerPC+2;
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

			Instructions op = (Instructions) this.memory.Read(this.registerPC++);
			int data, address;

			if (op >= Instructions.JumpAbs && op <= Instructions.JumpAbsNS)
			{
				address = this.AddressAbs;
				if (this.IsTestTrue(op))
				{
					this.registerPC = address;
				}
				return;
			}

			if (op >= Instructions.JumpRel && op <= Instructions.JumpRelNS)
			{
				address = this.AddressRel;
				if (this.IsTestTrue(op))
				{
					this.registerPC = address;
				}
				return;
			}

			if (op >= Instructions.CallAbs && op <= Instructions.CallAbsNS)
			{
				address = this.AddressAbs;
				if (this.IsTestTrue(op))
				{
					this.StackPushWord(this.registerPC);
					this.registerPC = address;
				}
				return;
			}

			if (op >= Instructions.CallRel && op <= Instructions.CallRelNS)
			{
				address = this.AddressRel;
				if (this.IsTestTrue(op))
				{
					this.StackPushWord(this.registerPC);
					this.registerPC = address;
				}
				return;
			}

			switch (op)
			{
				case Instructions.JumpHL:
					this.registerPC = this.registerHL;
					break;

				case Instructions.Halt:
					this.registerPC--;
					this.isHalted = true;
					break;

				case Instructions.CallHL:
					this.StackPushWord(this.registerPC);
					this.registerPC = this.registerHL;
					break;

				case Instructions.Ret:
					this.registerPC = this.StackPopWord();
					break;

				case Instructions.MoveiA:
					this.registerA = this.memory.Read(this.registerPC++);
					break;

				case Instructions.MoveiB:
					this.registerB = this.memory.Read(this.registerPC++);
					break;

				case Instructions.MoveiHL:
					this.registerHL = this.AddressAbs;
					break;

				case Instructions.MoverHL:
					this.registerHL = this.AddressRel;
					break;

				case Instructions.MoveBA:
					this.registerA = this.registerB;
					break;

				case Instructions.MoveAB:
					this.registerB = this.registerA;
					break;

				case Instructions.SwapAB:
					data = this.registerA;
					this.registerA = this.registerB;
					this.registerB = data;
					break;

				case Instructions.MovemA:
					this.registerA = this.memory.Read(this.AddressAbs);
					break;

				case Instructions.MoveAm:
					this.memory.Write(this.AddressAbs, this.registerA);
					break;

				case Instructions.MovecHLA:
					this.registerA = this.memory.Read(this.registerHL);
					break;

				case Instructions.MoveAcHL:
					this.memory.Write(this.registerHL, this.registerA);
					break;

				case Instructions.MoveHA:
					this.registerA = (this.registerHL >> 8) & 0xff;
					break;

				case Instructions.MoveLA:
					this.registerA = this.registerHL & 0xff;
					break;

				case Instructions.MoveAH:
					this.registerHL = (this.registerA >> 8) | (this.registerHL & 0xff);
					break;

				case Instructions.MoveAL:
					this.registerHL = (this.registerA & 0xff) | ((this.registerHL >> 8) & 0xff);
					break;

				case Instructions.AddiA:
					this.registerA = this.SetFlagsOper8(this.BypeSignExtend(this.registerA) + this.ReadByte(this.registerPC++));
					break;

				case Instructions.AddBA:
					this.registerA = this.SetFlagsOper8(this.BypeSignExtend(this.registerA) + this.BypeSignExtend(this.registerB));
					break;

				case Instructions.SubiA:
					this.registerA = this.SetFlagsOper8(this.BypeSignExtend(this.registerA) - this.ReadByte(this.registerPC++));
					break;

				case Instructions.SubBA:
					this.registerA = this.SetFlagsOper8(this.BypeSignExtend(this.registerA) - this.BypeSignExtend(this.registerB));
					break;

				case Instructions.AndiA:
					this.registerA = this.SetFlagsOper8(this.registerA & this.memory.Read(this.registerPC++));
					break;

				case Instructions.AndBA:
					this.registerA = this.SetFlagsOper8(this.registerA & this.registerB);
					break;

				case Instructions.OriA:
					this.registerA = this.SetFlagsOper8(this.registerA | this.memory.Read(this.registerPC++));
					break;

				case Instructions.OrBA:
					this.registerA = this.SetFlagsOper8(this.registerA | this.registerB);
					break;

				case Instructions.XoriA:
					this.registerA = this.SetFlagsOper8(this.registerA ^ this.memory.Read(this.registerPC++));
					break;

				case Instructions.XorBA:
					this.registerA = this.SetFlagsOper8(this.registerA ^ this.registerB);
					break;

				case Instructions.AddiHL:
					this.registerHL = this.SetFlagsOper16(this.registerHL + this.ReadByte(this.registerPC++));
					break;

				case Instructions.AddAHL:
					this.registerHL = this.SetFlagsOper16(this.registerHL + this.BypeSignExtend(this.registerA));
					break;

				case Instructions.AddBHL:
					this.registerHL = this.SetFlagsOper16(this.registerHL + this.BypeSignExtend(this.registerB));
					break;

				case Instructions.SubiHL:
					this.registerHL = this.SetFlagsOper16(this.registerHL - this.ReadByte(this.registerPC++));
					break;

				case Instructions.SubAHL:
					this.registerHL = this.SetFlagsOper16(this.registerHL - this.BypeSignExtend(this.registerA));
					break;

				case Instructions.SubBHL:
					this.registerHL = this.SetFlagsOper16(this.registerHL - this.BypeSignExtend(this.registerB));
					break;

				case Instructions.CompiA:
					this.SetFlagsCompare(this.registerA, this.memory.Read(this.registerPC++));
					break;

				case Instructions.CompiB:
					this.SetFlagsCompare(this.registerB, this.memory.Read(this.registerPC++));
					break;

				case Instructions.CompiHL:
					this.SetFlagsCompare(this.registerHL, this.AddressAbs);
					break;

				case Instructions.CompBA:
					this.SetFlagsCompare(this.registerA, this.registerB);
					break;

				case Instructions.TestiA:
					data = (1 << (this.memory.Read(this.registerPC++) & 0x07));
					this.SetFlag(ProcessorGeneric.FlagZero, (this.registerA & data) == 0);
					break;

				case Instructions.TestBA:
					data = (1 << (this.registerB & 0x07));
					this.SetFlag(ProcessorGeneric.FlagZero, (this.registerA & data) == 0);
					break;

				case Instructions.TClriA:
					data = (1 << (this.memory.Read(this.registerPC++) & 0x07));
					this.SetFlag(ProcessorGeneric.FlagZero, (this.registerA & data) == 0);
					this.registerA &= ~data;
					break;

				case Instructions.TClrBA:
					data = (1 << (this.registerB & 0x07));
					this.SetFlag(ProcessorGeneric.FlagZero, (this.registerA & data) == 0);
					this.registerA &= ~data;
					break;

				case Instructions.TSetiA:
					data = (1 << (this.memory.Read(this.registerPC++) & 0x07));
					this.SetFlag(ProcessorGeneric.FlagZero, (this.registerA & data) == 0);
					this.registerA |= data;
					break;

				case Instructions.TSetBA:
					data = (1 << (this.registerB & 0x07));
					this.SetFlag(ProcessorGeneric.FlagZero, (this.registerA & data) == 0);
					this.registerA |= data;
					break;

				case Instructions.IncA:
					this.registerA = this.SetFlagsOper8(this.BypeSignExtend(this.registerA+1));
					break;

				case Instructions.IncB:
					this.registerB = this.SetFlagsOper8(this.BypeSignExtend(this.registerB+1));
					break;

				case Instructions.IncHL:
					this.registerHL = this.SetFlagsOper16(this.registerHL+1);
					break;

				case Instructions.DecA:
					this.registerA = this.SetFlagsOper8(this.BypeSignExtend(this.registerA-1));
					break;

				case Instructions.DecB:
					this.registerB = this.SetFlagsOper8(this.BypeSignExtend(this.registerB-1));
					break;

				case Instructions.DecHL:
					this.registerHL = this.SetFlagsOper16(this.registerHL-1);
					break;

				case Instructions.RRA:
					this.registerA = this.RotateRight(this.registerA);
					break;

				case Instructions.RRB:
					this.registerB = this.RotateRight(this.registerB);
					break;

				case Instructions.RLA:
					this.registerA = this.RotateLeft(this.registerA);
					break;

				case Instructions.RLB:
					this.registerB = this.RotateLeft(this.registerB);
					break;

				case Instructions.SetC:
					this.SetFlag(ProcessorGeneric.FlagCarry, true);
					break;

				case Instructions.ClrC:
					this.SetFlag(ProcessorGeneric.FlagCarry, false);
					break;

				case Instructions.SetV:
					this.SetFlag(ProcessorGeneric.FlagOverflow, true);
					break;

				case Instructions.ClrV:
					this.SetFlag(ProcessorGeneric.FlagOverflow, false);
					break;

				case Instructions.PushA:
					this.StackPushByte(this.registerA);
					break;

				case Instructions.PushB:
					this.StackPushByte(this.registerB);
					break;

				case Instructions.PushHL:
					this.StackPushWord(this.registerHL);
					break;

				case Instructions.PushF:
					this.StackPushByte(this.registerF);
					break;

				case Instructions.PopA:
					this.registerA = this.StackPopByte();
					break;

				case Instructions.PopB:
					this.registerB = this.StackPopByte();
					break;

				case Instructions.PopHL:
					this.registerHL = this.StackPopWord();
					break;

				case Instructions.PopF:
					this.registerF = this.StackPopByte();
					break;

				case Instructions.MuliA:
					this.registerA = this.SetFlagsOper8(this.BypeSignExtend(this.registerA) * this.ReadByte(this.registerPC++));
					break;

				case Instructions.MulBA:
					this.registerA = this.SetFlagsOper8(this.BypeSignExtend(this.registerA) * this.BypeSignExtend(this.registerB));
					break;

				case Instructions.MuliHL:
					this.registerHL = this.SetFlagsOper16(this.registerHL * this.ReadByte(this.registerPC++));
					break;

				case Instructions.MulAHL:
					this.registerHL = this.SetFlagsOper16(this.registerHL * this.BypeSignExtend(this.registerA));
					break;

				case Instructions.MulBHL:
					this.registerHL = this.SetFlagsOper16(this.registerHL * this.BypeSignExtend(this.registerB));
					break;

				case Instructions.DiviA:
					data = this.ReadByte(this.registerPC++);
					if (data == 0)
					{
						this.SetFlag(ProcessorGeneric.FlagOverflow, true);
					}
					else
					{
						this.registerA = this.SetFlagsOper8(this.BypeSignExtend(this.registerA) / data);
					}
					break;

				case Instructions.DivBA:
					data = this.BypeSignExtend(this.registerB);
					if (data == 0)
					{
						this.SetFlag(ProcessorGeneric.FlagOverflow, true);
					}
					else
					{
						this.registerA = this.SetFlagsOper8(this.BypeSignExtend(this.registerA) / data);
					}
					break;

				case Instructions.DiviHL:
					data = this.ReadByte(this.registerPC++);
					if (data == 0)
					{
						this.SetFlag(ProcessorGeneric.FlagOverflow, true);
					}
					else
					{
						this.registerHL = this.SetFlagsOper16(this.registerHL / data);
					}
					break;

				case Instructions.DivAHL:
					data = this.BypeSignExtend(this.registerA);
					if (data == 0)
					{
						this.SetFlag(ProcessorGeneric.FlagOverflow, true);
					}
					else
					{
						this.registerHL = this.SetFlagsOper16(this.registerHL / data);
					}
					break;

				case Instructions.DivBHL:
					data = this.BypeSignExtend(this.registerB);
					if (data == 0)
					{
						this.SetFlag(ProcessorGeneric.FlagOverflow, true);
					}
					else
					{
						this.registerHL = this.SetFlagsOper16(this.registerHL / data);
					}
					break;

				case Instructions.ModiA:
					data = this.ReadByte(this.registerPC++);
					if (data == 0)
					{
						this.SetFlag(ProcessorGeneric.FlagOverflow, true);
					}
					else
					{
						this.registerA = this.SetFlagsOper8(this.BypeSignExtend(this.registerA) % data);
					}
					break;

				case Instructions.ModBA:
					data = this.BypeSignExtend(this.registerB);
					if (data == 0)
					{
						this.SetFlag(ProcessorGeneric.FlagOverflow, true);
					}
					else
					{
						this.registerA = this.SetFlagsOper8(this.BypeSignExtend(this.registerA) % data);
					}
					break;

				case Instructions.ModiHL:
					data = this.ReadByte(this.registerPC++);
					if (data == 0)
					{
						this.SetFlag(ProcessorGeneric.FlagOverflow, true);
					}
					else
					{
						this.registerHL = this.SetFlagsOper16(this.registerHL % data);
					}
					break;

				case Instructions.ModAHL:
					data = this.BypeSignExtend(this.registerA);
					if (data == 0)
					{
						this.SetFlag(ProcessorGeneric.FlagOverflow, true);
					}
					else
					{
						this.registerHL = this.SetFlagsOper16(this.registerHL % data);
					}
					break;

				case Instructions.ModBHL:
					data = this.BypeSignExtend(this.registerB);
					if (data == 0)
					{
						this.SetFlag(ProcessorGeneric.FlagOverflow, true);
					}
					else
					{
						this.registerHL = this.SetFlagsOper16(this.registerHL % data);
					}
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

			this.SetFlag(ProcessorGeneric.FlagCarry, bit);
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

			this.SetFlag(ProcessorGeneric.FlagCarry, bit);
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
				return (this.memory.Read(this.registerPC++) << 8) | (this.memory.Read(this.registerPC++));
			}
		}

		protected int AddressRel
		{
			get
			{
				int offset = this.memory.Read(this.registerPC++);
				
				if ((offset & 0x80) != 0)  // offset négatif ?
				{
					offset = offset-0x100;
				}

				return this.registerPC + offset;
			}
		}


		protected void SetFlagsCompare(int a, int b)
		{
			this.SetFlag(ProcessorGeneric.FlagZero, a == b);
			this.SetFlag(ProcessorGeneric.FlagCarry, a >= b);
		}

		protected int SetFlagsOper8(int value)
		{
			this.SetFlag(ProcessorGeneric.FlagZero, value == 0);
			this.SetFlag(ProcessorGeneric.FlagNeg, (value & 0x80) != 0);

			if ((value & 0x80) == 0)  // valeur positive ?
			{
				this.SetFlag(ProcessorGeneric.FlagOverflow, (value & 0xffffff00) != 0);
			}
			else  // valeur négative ?
			{
				this.SetFlag(ProcessorGeneric.FlagOverflow, (value & 0xffffff00) == 0);
			}

			return value & 0xff;
		}

		protected int SetFlagsOper16(int value)
		{
			this.SetFlag(ProcessorGeneric.FlagZero, value == 0);
			this.SetFlag(ProcessorGeneric.FlagNeg, (value & 0x8000) != 0);

			if ((value & 0x8000) == 0)  // valeur positive ?
			{
				this.SetFlag(ProcessorGeneric.FlagOverflow, (value & 0xffff0000) != 0);
			}
			else  // valeur négative ?
			{
				this.SetFlag(ProcessorGeneric.FlagOverflow, (value & 0xffff0000) == 0);
			}

			return value & 0xffff;
		}

		protected bool IsTestTrue(Instructions op)
		{
			Instructions test = (op & (Instructions)0xf);

			switch (test)
			{
				case Instructions.JumpAbsEQ:
					return this.TestFlag(ProcessorGeneric.FlagZero);

				case Instructions.JumpAbsNE:
					return !this.TestFlag(ProcessorGeneric.FlagZero);

				case Instructions.JumpAbsLO:
					return !this.TestFlag(ProcessorGeneric.FlagZero) && !this.TestFlag(ProcessorGeneric.FlagCarry);

				case Instructions.JumpAbsLS:
					return this.TestFlag(ProcessorGeneric.FlagZero) || !this.TestFlag(ProcessorGeneric.FlagCarry);

				case Instructions.JumpAbsHI:
					return !this.TestFlag(ProcessorGeneric.FlagZero) && this.TestFlag(ProcessorGeneric.FlagCarry);

				case Instructions.JumpAbsHS:
					return this.TestFlag(ProcessorGeneric.FlagZero) || this.TestFlag(ProcessorGeneric.FlagCarry);

				case Instructions.JumpAbsVC:
					return !this.TestFlag(ProcessorGeneric.FlagOverflow);

				case Instructions.JumpAbsVS:
					return this.TestFlag(ProcessorGeneric.FlagOverflow);

				case Instructions.JumpAbsNC:
					return !this.TestFlag(ProcessorGeneric.FlagNeg);

				case Instructions.JumpAbsNS:
					return this.TestFlag(ProcessorGeneric.FlagNeg);
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
				yield return "HL";
			}
		}

		public override int GetRegisterSize(string name)
		{
			//	Retourne la taille (nombre de bits) d'un registre.
			switch (name)
			{
				case "PC":
				case "SP":
				case "HL":
					return Memory.TotalAddress;

				case "F":
				case "A":
				case "B":
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

				case "HL":
					return this.registerHL;
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

				case "HL":
					this.registerHL = value;
					break;
			}
		}
		#endregion


		#region Rom
		public override void RomInitialise(int address, int length)
		{
			//	Rempli la Rom.
			int indirect = address;
			address += 3*64;  // place pour 64 appels
			this.RomWrite(ref indirect, ref address, ProcessorGeneric.WaitKey);
			this.RomWrite(ref indirect, ref address, ProcessorGeneric.DisplayBinaryDigit);
			this.RomWrite(ref indirect, ref address, ProcessorGeneric.DisplayHexaDigit);
			this.RomWrite(ref indirect, ref address, ProcessorGeneric.DisplayHexaByte);
			this.RomWrite(ref indirect, ref address, ProcessorGeneric.DisplayDecimal);
			this.RomWrite(ref indirect, ref address, ProcessorGeneric.SetPixel);
			this.RomWrite(ref indirect, ref address, ProcessorGeneric.ClrPixel);
		}

		protected void RomWrite(ref int indirect, ref int address, byte[] code)
		{
			this.memory.WriteRom(indirect++, (byte) Instructions.JumpAbs);
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
			(byte) Instructions.PushF,				// PUSH F
													// LOOP:
			(byte) Instructions.MovemA, 0x0C, 0x07,	// MOVE C07,A		; lit le clavier
			(byte) Instructions.TClriA, 0x07,		// TCLR A:#7		; bit full ?
			(byte) Instructions.JumpRelEQ, 0xF9,	// JUMP,EQ R8^LOOP	; non, jump loop
			(byte) Instructions.PopF,				// POP F
			(byte) Instructions.Ret,				// RET
		};

		//	Affiche des segments à choix.
		//	in	A segments à allumer
		//		B digit 0..3
		//	out	-
		//	mod	-
		protected static byte[] DisplayBinaryDigit =
		{
			(byte) Instructions.PushF,				// PUSH F
			(byte) Instructions.PushHL,				// PUSH HL

			(byte) Instructions.SwapAB,				// SWAP A,B
			(byte) Instructions.AndiA, 0x03,		// AND #03,A
			(byte) Instructions.SwapAB,				// SWAP A,B

			(byte) Instructions.MoveiHL, 0x0C, 0x00,// MOVE #C00,HL
			(byte) Instructions.AddBHL,				// ADD B,HL
			(byte) Instructions.MoveAcHL,			// MOVE A,{HL}		; affiche les segments

			(byte) Instructions.PopHL,				// POP HL
			(byte) Instructions.PopF,				// POP F
			(byte) Instructions.Ret,				// RET
		};

		//	Affiche un digit hexadécimal.
		//	in	A valeur 0..15
		//		B digit 0..3
		//	out	-
		//	mod	-
		protected static byte[] DisplayHexaDigit =
		{
			(byte) Instructions.PushF,				// PUSH F
			(byte) Instructions.PushA,				// PUSH A
			(byte) Instructions.PushHL,				// PUSH HL

			(byte) Instructions.AndiA, 0x0F,		// AND #0F,A
			(byte) Instructions.SwapAB,				// SWAP A,B
			(byte) Instructions.AndiA, 0x03,		// AND #03,A
			(byte) Instructions.SwapAB,				// SWAP A,B

			(byte) Instructions.MoverHL, 11,		// MOVE #R8^+11		; adresse de la table
			(byte) Instructions.AddAHL,				// ADD A,HL			; HL pointe le bon digit
			(byte) Instructions.MovecHLA,			// MOVE {HL},A		; A = segments

			(byte) Instructions.MoveiHL, 0x0C, 0x00,// MOVE #C00,HL
			(byte) Instructions.AddBHL,				// ADD B,HL
			(byte) Instructions.MoveAcHL,			// MOVE A,{HL}		; affiche le digit

			(byte) Instructions.PopHL,				// POP HL
			(byte) Instructions.PopA,				// POP A
			(byte) Instructions.PopF,				// POP F
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
			(byte) Instructions.PushF,				// PUSH F
			(byte) Instructions.PushA,				// PUSH A

			(byte) Instructions.IncB,				// INC B
			(byte) Instructions.CallAbs, 0x08, 0x06,// CALL DisplayHexaDigit

			(byte) Instructions.RRA,				// RR A
			(byte) Instructions.RRA,				// RR A
			(byte) Instructions.RRA,				// RR A
			(byte) Instructions.RRA,				// RR A
			(byte) Instructions.DecB,				// DEC B
			(byte) Instructions.CallAbs, 0x08, 0x06,// CALL DisplayHexaDigit

			(byte) Instructions.PopA,				// POP A
			(byte) Instructions.PopF,				// POP F
			(byte) Instructions.Ret,				// RET
		};

		//	Affiche une valeur décimale.
		//	in	HL valeur
		//	out	-
		//	mod	-
		protected static byte[] DisplayDecimal =
		{
			(byte) Instructions.PushF,				// PUSH F
			(byte) Instructions.PushA,				// PUSH A
			(byte) Instructions.PushB,				// PUSH B
			(byte) Instructions.PushHL,				// PUSH HL

			(byte) Instructions.MoveiB, 0x03,		// MOVE #3,B

													// LOOP:
			(byte) Instructions.PushHL,				// PUSH HL
			(byte) Instructions.ModiHL, 0x0A,		// MOD #10.,HL
			(byte) Instructions.MoveLA,				// MOVE L,A
			(byte) Instructions.CallAbs, 0x08, 0x06,// CALL DisplayHexaDigit
			(byte) Instructions.PopHL,				// POP HL

			(byte) Instructions.DecB,				// DEC B
			(byte) Instructions.DiviHL, 0x0A,		// DIV #10.,HL
			(byte) Instructions.CompiHL, 0x00, 0x00,// COMP #0,HL
			(byte) Instructions.JumpRelNE, 0xF0,	// JUMP,NE R8^LOOP

													// CLEAR:
			(byte) Instructions.CompiB, 0xFF,		// COMP #0,B
			(byte) Instructions.JumpRelEQ, 0x08,	// JUMP,EQ R8^END
			(byte) Instructions.MoveiA, 0x00,		// MOVE #0,A
			(byte) Instructions.CallAbs, 0x08, 0x03,// CALL DisplayBinaryDigit
			(byte) Instructions.DecB,				// DEC B
			(byte) Instructions.JumpRel, 0xF4,		// JUMP R8^CLEAR

													// END:
			(byte) Instructions.PopHL,				// POP HL
			(byte) Instructions.PopB,				// POP B
			(byte) Instructions.PopA,				// POP A
			(byte) Instructions.PopF,				// POP F
			(byte) Instructions.Ret,				// RET
		};

		//	Allume un pixel dans l'écran bitmap.
		//	in	A coordonnée X 0..31
		//		B coordonnée Y 0..23
		//	out	-
		//	mod	-
		protected static byte[] SetPixel =
		{
			(byte) Instructions.PushF,				// PUSH F
			(byte) Instructions.PushA,				// PUSH A
			(byte) Instructions.PushB,				// PUSH B
			(byte) Instructions.PushHL,				// PUSH HL

			(byte) Instructions.MoveiHL, 0x0C, 0x80,// MOVE #C80,HL

			(byte) Instructions.SwapAB,				// SWAP A,B
			(byte) Instructions.AndiA, 0x1F,		// AND #1F,A
			(byte) Instructions.SwapAB,				// SWAP A,B
			(byte) Instructions.RLB,				// RL B
			(byte) Instructions.RLB,				// RL B
			(byte) Instructions.AddBHL,				// ADD B,HL

			(byte) Instructions.PushA,				// PUSH A
			(byte) Instructions.RRA,				// RR A
			(byte) Instructions.RRA,				// RR A
			(byte) Instructions.RRA,				// RR A
			(byte) Instructions.AndiA, 0x03,		// AND #03,A
			(byte) Instructions.AddAHL,				// ADD A,HL
			(byte) Instructions.PopA,				// POP A

			(byte) Instructions.AndiA, 0x07,		// AND #07,A
			(byte) Instructions.XoriA, 0x07,		// XOR #07,A
			(byte) Instructions.MoveAB,				// MOVE A,B

			(byte) Instructions.MovecHLA,			// MOVE {HL},A
			(byte) Instructions.TSetBA,				// TSET B:A
			(byte) Instructions.MoveAcHL,			// MOVE A,{HL}

			(byte) Instructions.PopHL,				// POP HL
			(byte) Instructions.PopB,				// POP B
			(byte) Instructions.PopA,				// POP A
			(byte) Instructions.PopF,				// POP F
			(byte) Instructions.Ret,				// RET
		};

		//	Eteint un pixel dans l'écran bitmap.
		//	in	A coordonnée X 0..31
		//		B coordonnée Y 0..23
		//	out	-
		//	mod	-
		protected static byte[] ClrPixel =
		{
			(byte) Instructions.PushF,				// PUSH F
			(byte) Instructions.PushA,				// PUSH A
			(byte) Instructions.PushB,				// PUSH B
			(byte) Instructions.PushHL,				// PUSH HL

			(byte) Instructions.MoveiHL, 0x0C, 0x80,// MOVE #C80,HL

			(byte) Instructions.SwapAB,				// SWAP A,B
			(byte) Instructions.AndiA, 0x1F,		// AND #1F,A
			(byte) Instructions.SwapAB,				// SWAP A,B
			(byte) Instructions.RLB,				// RL B
			(byte) Instructions.RLB,				// RL B
			(byte) Instructions.AddBHL,				// ADD B,HL

			(byte) Instructions.PushA,				// PUSH A
			(byte) Instructions.RRA,				// RR A
			(byte) Instructions.RRA,				// RR A
			(byte) Instructions.RRA,				// RR A
			(byte) Instructions.AndiA, 0x03,		// AND #03,A
			(byte) Instructions.AddAHL,				// ADD A,HL
			(byte) Instructions.PopA,				// POP A

			(byte) Instructions.AndiA, 0x07,		// AND #07,A
			(byte) Instructions.XoriA, 0x07,		// XOR #07,A
			(byte) Instructions.MoveAB,				// MOVE A,B

			(byte) Instructions.MovecHLA,			// MOVE {HL},A
			(byte) Instructions.TClrBA,				// TCLR B:A
			(byte) Instructions.MoveAcHL,			// MOVE A,{HL}

			(byte) Instructions.PopHL,				// POP HL
			(byte) Instructions.PopB,				// POP B
			(byte) Instructions.PopA,				// POP A
			(byte) Instructions.PopF,				// POP F
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
		protected int registerA;   // accumulator 8 bits
		protected int registerB;   // registre auxiliaire 8 bits
		protected int registerHL;  // registre 12 bits
	}
}
