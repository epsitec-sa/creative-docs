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
			//	r    (R)  =  A,B,X,Y
			//	r'   (S)  =  A,B
			//	ADDR (A)  =  adresse 16 bits, 12 bits valeur, bit 12 = +{X}, bit 13 = +{Y}, bit 15 = relatif
			//	#val (V)  =  valeur absolue positive 8 bits

			Nop    = 0x00,
			Ret    = 0x01,
			Halt   = 0x02,
			SetC   = 0x04,
			ClrC   = 0x05,
			SetV   = 0x06,
			ClrV   = 0x07,

			PushR  = 0x08,		// PUSH r
			PopR   = 0x0C,		// POP r

			Jump   = 0x10,
			JumpEQ = 0x12,
			JumpNE = 0x13,
			JumpLO = 0x14,
			JumpLS = 0x15,
			JumpHI = 0x16,
			JumpHS = 0x17,
			JumpVC = 0x18,
			JumpVS = 0x19,
			JumpNC = 0x1A,
			JumpNS = 0x1B,

			//	0x1C..0x1F libre

			Call   = 0x20,
			CallEQ = 0x22,
			CallNE = 0x23,
			CallLO = 0x24,
			CallLS = 0x25,
			CallHI = 0x26,
			CallHS = 0x27,
			CallVC = 0x28,
			CallVS = 0x29,
			CallNC = 0x2A,
			CallNS = 0x2B,

			ClrR   = 0x30,		// op r
			NotR   = 0x34,
			IncR   = 0x38,
			DecR   = 0x3C,

			RlS    = 0x40,		// op r'
			RrS    = 0x42,
			RlcS   = 0x44,
			RrcS   = 0x46,

			ClrA   = 0x48,		// op ADDR
			NotA   = 0x49,
			IncA   = 0x4A,
			DecA   = 0x4B,
			RlA    = 0x4C,
			RrA    = 0x4D,
			RlcA   = 0x4E,
			RrcA   = 0x4F,

			MoveRR = 0x50,		// MOVE r,r
			MoveVR = 0x60,		// MOVE #val,r
			MoveAR = 0x64,		// MOVE ADDR,r
			MoveRA = 0x68,		// MOVE r,ADDR

			CompRR = 0x70,		// COMP r,r

			AddRR  = 0x80,		// op r,r
			SubRR  = 0x90,
			AddVR  = 0xA0,		// op #val,r
			SubVR  = 0xA4,
			AddAR  = 0xB0,		// op ADDR,r
			SubAR  = 0xB4,
			AddRA  = 0xB8,		// op r,ADDR
			SubRA  = 0xBC,

			AndVS  = 0xC0,		// op #val,r'
			OrVS   = 0xC2,
			XorVS  = 0xC4,
			AndSS  = 0xC8,		// op r',r'
			OrSS   = 0xCA,
			XorSS  = 0xCC,
			AndAS  = 0xD0,		// op ADDR,r'
			OrAS   = 0xD2,
			XorAS  = 0xD4,
			AndSA  = 0xD8,		// op r',ADDR
			OrSA   = 0xDA,
			XorSA  = 0xDC,

			TestSS = 0xE0,		// op r',r'
			TSetSS = 0xE2,
			TClrSS = 0xE4,
			TInvSS = 0xE6,
			TestSA = 0xE8,		// op r',ADDR
			TSetSA = 0xEA,
			TClrSA = 0xEC,
			TInvSA = 0xEE,

			TestVS = 0xF0,		// op #val,r'
			TSetVS = 0xF2,
			TClrVS = 0xF4,
			TInvVS = 0xF6,
			TestVA = 0xF8,		// op #val,ADDR
			TSetVA = 0xF9,
			TClrVA = 0xFA,
			TInvVA = 0xFB,

			CompVR = 0xFC,		// COMP #val,r
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
						data = 0;
						this.SetRegister(n, data);
						this.SetFlagsOper(data);
						return;

					case Instructions.NotR:
						data = this.GetRegister(n) ^ 0xFF;
						this.SetRegister(n, data);
						this.SetFlagsOper(data);
						return;

					case Instructions.IncR:
						data = this.GetRegister(n) + 1;
						this.SetRegister(n, data);
						this.SetFlagsOper(data);
						return;

					case Instructions.DecR:
						data = this.GetRegister(n) - 1;
						this.SetRegister(n, data);
						this.SetFlagsOper(data);
						return;
				}
			}

			if (op >= (int) Instructions.RlS && op <= (int) Instructions.RlS + 0x07)  // op r'
			{
				int n = op & 0x01;
				Instructions i = (Instructions) (op & 0xFE);

				switch (i)
				{
					case Instructions.RlS:
						data = this.RotateLeft(this.GetRegister(n), false);
						this.SetRegister(n, data);
						this.SetFlagsOper(data);
						return;

					case Instructions.RrS:
						data = this.RotateRight(this.GetRegister(n), false);
						this.SetRegister(n, data);
						this.SetFlagsOper(data);
						return;

					case Instructions.RlcS:
						data = this.RotateLeft(this.GetRegister(n), true);
						this.SetRegister(n, data);
						this.SetFlagsOper(data);
						return;

					case Instructions.RrcS:
						data = this.RotateRight(this.GetRegister(n), true);
						this.SetRegister(n, data);
						this.SetFlagsOper(data);
						return;
				}
			}

			if (op >= (int) Instructions.ClrA && op <= (int) Instructions.ClrA + 0x07)  // op ADDR
			{
				Instructions i = (Instructions) op;
				address = this.AddressAbs;

				switch (i)
				{
					case Instructions.ClrA:
						data = 0;
						this.memory.Write(address, data);
						this.SetFlagsOper(data);
						return;

					case Instructions.NotA:
						data = this.memory.Read(address) ^ 0xFF;
						this.memory.Write(address, data);
						this.SetFlagsOper(data);
						return;

					case Instructions.IncA:
						data = this.memory.Read(address) + 1;
						this.memory.Write(address, data);
						this.SetFlagsOper(data);
						return;

					case Instructions.DecA:
						data = this.memory.Read(address) - 1;
						this.memory.Write(address, data);
						this.SetFlagsOper(data);
						return;

					case Instructions.RlA:
						data = this.RotateLeft(this.memory.Read(address), false);
						this.memory.Write(address, data);
						this.SetFlagsOper(data);
						return;

					case Instructions.RrA:
						data = this.RotateRight(this.memory.Read(address), false);
						this.memory.Write(address, data);
						this.SetFlagsOper(data);
						return;

					case Instructions.RlcA:
						data = this.RotateLeft(this.memory.Read(address), true);
						this.memory.Write(address, data);
						this.SetFlagsOper(data);
						return;

					case Instructions.RrcA:
						data = this.RotateRight(this.memory.Read(address), true);
						this.memory.Write(address, data);
						this.SetFlagsOper(data);
						return;
				}
			}

			if (op >= (int) Instructions.MoveRR && op <= (int) Instructions.MoveRR + 0x0F)  // MOVE r,r
			{
				int src = (op>>2) & 0x03;
				int dst = op & 0x03;

				data = this.GetRegister(src);
				this.SetRegister(dst, data);
				this.SetFlagsOper(data);
				return;
			}

			if (op >= (int) Instructions.MoveVR && op <= (int) Instructions.MoveVR + 0x03)  // MOVE #val,r
			{
				int n = op & 0x03;

				data = this.memory.Read(this.registerPC++);
				this.SetRegister(n, data);
				this.SetFlagsOper(data);
				return;
			}

			if (op >= (int) Instructions.MoveAR && op <= (int) Instructions.MoveAR + 0x03)  // MOVE ADDR,r
			{
				int n = op & 0x03;
				address = this.AddressAbs;

				data = this.memory.Read(address);
				this.SetRegister(n, data);
				this.SetFlagsOper(data);
				return;
			}

			if (op >= (int) Instructions.MoveRA && op <= (int) Instructions.MoveRA + 0x03)  // MOVE r,ADDR
			{
				int n = op & 0x03;
				address = this.AddressAbs;

				data = this.GetRegister(n);
				this.memory.Write(address, data);
				this.SetFlagsOper(data);
				return;
			}

			if (op >= (int) Instructions.CompRR && op <= (int) Instructions.CompRR + 0x0F)  // COMP r,r
			{
				int src = (op>>2) & 0x03;
				int dst = op & 0x03;

				this.SetFlagsCompare(this.GetRegister(dst), this.GetRegister(src));
				return;
			}

			if (op >= (int) Instructions.AddRR && op <= (int) Instructions.AddRR + 0x1F)  // op r,r
			{
				int src = (op>>2) & 0x03;
				int dst = op & 0x03;
				Instructions i = (Instructions) (op & 0xF0);
				data = this.GetRegister(src);

				switch (i)
				{
					case Instructions.AddRR:
						data = this.GetRegister(dst) + data;
						this.SetRegister(dst, data);
						this.SetFlagsOper(data);
						return;

					case Instructions.SubRR:
						data = this.GetRegister(dst) - data;
						this.SetRegister(dst, data);
						this.SetFlagsOper(data);
						return;
				}
			}

			if (op >= (int) Instructions.AddVR && op <= (int) Instructions.AddVR + 0x07)  // op #val,r
			{
				int n = op & 0x03;
				Instructions i = (Instructions) (op & 0xFC);
				data = this.memory.Read(this.registerPC++);

				switch (i)
				{
					case Instructions.AddVR:
						data = this.GetRegister(n) + data;
						this.SetRegister(n, data);
						this.SetFlagsOper(data);
						return;

					case Instructions.SubVR:
						data = this.GetRegister(n) - data;
						this.SetRegister(n, data);
						this.SetFlagsOper(data);
						return;
				}
			}

			if (op >= (int) Instructions.AddAR && op <= (int) Instructions.AddAR + 0x07)  // op ADDR,r
			{
				int n = op & 0x03;
				Instructions i = (Instructions) (op & 0xFC);
				address = this.AddressAbs;

				switch (i)
				{
					case Instructions.AddAR:
						data = this.GetRegister(n) + this.memory.Read(address);
						this.SetRegister(n, data);
						this.SetFlagsOper(data);
						return;

					case Instructions.SubAR:
						data = this.GetRegister(n) - this.memory.Read(address);
						this.SetRegister(n, data);
						this.SetFlagsOper(data);
						return;
				}
			}

			if (op >= (int) Instructions.AddRA && op <= (int) Instructions.AddRA + 0x07)  // op r,ADDR
			{
				int n = op & 0x03;
				Instructions i = (Instructions) (op & 0xFC);
				address = this.AddressAbs;

				switch (i)
				{
					case Instructions.AddRA:
						data = this.memory.Read(address) + this.GetRegister(n);
						this.memory.Write(address, data);
						this.SetFlagsOper(data);
						return;

					case Instructions.SubRA:
						data = this.memory.Read(address) - this.GetRegister(n);
						this.memory.Write(address, data);
						this.SetFlagsOper(data);
						return;
				}
			}

			if (op >= (int) Instructions.AndVS && op <= (int) Instructions.AndVS + 0x07)  // op #val,r
			{
				int n = op & 0x01;
				Instructions i = (Instructions) (op & 0xFE);
				data = this.memory.Read(this.registerPC++);

				switch (i)
				{
					case Instructions.AndVS:
						data = this.GetRegister(n) & data;
						this.SetRegister(n, data);
						this.SetFlagsOper(data);
						return;

					case Instructions.OrVS:
						data = this.GetRegister(n) | data;
						this.SetRegister(n, data);
						this.SetFlagsOper(data);
						return;

					case Instructions.XorVS:
						data = this.GetRegister(n) ^ data;
						this.SetRegister(n, data);
						this.SetFlagsOper(data);
						return;
				}
			}

			if (op >= (int) Instructions.AndSS && op <= (int) Instructions.AndSS + 0x07)  // op r',r'
			{
				int n = op & 0x01;
				int src = n;
				int dst = n ^0x01;
				Instructions i = (Instructions) (op & 0xFE);
				data = this.GetRegister(src);

				switch (i)
				{
					case Instructions.AndSS:
						data = this.GetRegister(dst) & data;
						this.SetRegister(dst, data);
						this.SetFlagsOper(data);
						return;

					case Instructions.OrSS:
						data = this.GetRegister(dst) | data;
						this.SetRegister(dst, data);
						this.SetFlagsOper(data);
						return;

					case Instructions.XorSS:
						data = this.GetRegister(dst) ^ data;
						this.SetRegister(dst, data);
						this.SetFlagsOper(data);
						return;
				}
			}

			if (op >= (int) Instructions.AndAS && op <= (int) Instructions.AndAS + 0x07)  // op ADDR,r'
			{
				int n = op & 0x01;
				Instructions i = (Instructions) (op & 0xFE);
				address = this.AddressAbs;

				switch (i)
				{
					case Instructions.AndAS:
						data = this.GetRegister(n) & this.memory.Read(address);
						this.SetRegister(n, data);
						this.SetFlagsOper(data);
						return;

					case Instructions.OrAS:
						data = this.GetRegister(n) | this.memory.Read(address);
						this.SetRegister(n, data);
						this.SetFlagsOper(data);
						return;

					case Instructions.XorAS:
						data = this.GetRegister(n) ^ this.memory.Read(address);
						this.SetRegister(n, data);
						this.SetFlagsOper(data);
						return;
				}
			}

			if (op >= (int) Instructions.AndSA && op <= (int) Instructions.AndSA + 0x07)  // op r',ADDR
			{
				int n = op & 0x01;
				Instructions i = (Instructions) (op & 0xFE);
				address = this.AddressAbs;

				switch (i)
				{
					case Instructions.AndSA:
						data = this.memory.Read(address) & this.GetRegister(n);
						this.memory.Write(address, data);
						this.SetFlagsOper(data);
						return;

					case Instructions.OrSA:
						data = this.memory.Read(address) | this.GetRegister(n);
						this.memory.Write(address, data);
						this.SetFlagsOper(data);
						return;

					case Instructions.XorSA:
						data = this.memory.Read(address) ^ this.GetRegister(n);
						this.memory.Write(address, data);
						this.SetFlagsOper(data);
						return;
				}
			}

			if (op >= (int) Instructions.TestSS && op <= (int) Instructions.TestSS + 0x07)  // op r',r'
			{
				int n = op & 0x01;
				int src = n;
				int dst = n ^0x01;
				Instructions i = (Instructions) (op & 0xFE);
				data = this.GetRegister(src);

				switch (i)
				{
					case Instructions.TestSS:
						data = (1 << (data & 0x07));
						this.SetFlag(TinyProcessor.FlagZero, (this.GetRegister(dst) & data) == 0);
						return;

					case Instructions.TSetSS:
						data = (1 << (data & 0x07));
						this.SetFlag(TinyProcessor.FlagZero, (this.GetRegister(dst) & data) == 0);
						this.SetRegister(dst, this.GetRegister(dst) | data);
						return;

					case Instructions.TClrSS:
						data = (1 << (data & 0x07));
						this.SetFlag(TinyProcessor.FlagZero, (this.GetRegister(dst) & data) == 0);
						this.SetRegister(dst, this.GetRegister(dst) & ~data);
						return;

					case Instructions.TInvSS:
						data = (1 << (data & 0x07));
						this.SetFlag(TinyProcessor.FlagZero, (this.GetRegister(dst) & data) == 0);
						this.SetRegister(dst, this.GetRegister(dst) ^ data);
						return;
				}
			}

			if (op >= (int) Instructions.TestSA && op <= (int) Instructions.TestSA + 0x07)  // op r',ADDR
			{
				int n = op & 0x01;
				Instructions i = (Instructions) (op & 0xFE);
				address = this.AddressAbs;

				switch (i)
				{
					case Instructions.TestSA:
						data = (1 << (this.GetRegister(n) & 0x07));
						this.SetFlag(TinyProcessor.FlagZero, (this.memory.Read(address) & data) == 0);
						return;

					case Instructions.TClrSA:
						data = (1 << (this.GetRegister(n) & 0x07));
						this.SetFlag(TinyProcessor.FlagZero, (this.memory.Read(address) & data) == 0);
						this.memory.Write(address, this.memory.Read(address) & ~data);
						return;

					case Instructions.TSetSA:
						data = (1 << (this.GetRegister(n) & 0x07));
						this.SetFlag(TinyProcessor.FlagZero, (this.memory.Read(address) & data) == 0);
						this.memory.Write(address, this.memory.Read(address) | data);
						return;

					case Instructions.TInvSA:
						data = (1 << (this.GetRegister(n) & 0x07));
						this.SetFlag(TinyProcessor.FlagZero, (this.memory.Read(address) & data) == 0);
						this.memory.Write(address, this.memory.Read(address) ^ data);
						return;
				}
			}

			if (op >= (int) Instructions.TestVS && op <= (int) Instructions.TestVS + 0x07)  // op #val,r'
			{
				int n = op & 0x01;
				Instructions i = (Instructions) (op & 0xFE);
				data = this.memory.Read(this.registerPC++);

				switch (i)
				{
					case Instructions.TestVS:
						data = (1 << (data & 0x07));
						this.SetFlag(TinyProcessor.FlagZero, (this.GetRegister(n) & data) == 0);
						return;

					case Instructions.TClrVS:
						data = (1 << (data & 0x07));
						this.SetFlag(TinyProcessor.FlagZero, (this.GetRegister(n) & data) == 0);
						this.SetRegister(n, this.GetRegister(n) & ~data);
						return;

					case Instructions.TSetVS:
						data = (1 << (data & 0x07));
						this.SetFlag(TinyProcessor.FlagZero, (this.GetRegister(n) & data) == 0);
						this.SetRegister(n, this.GetRegister(n) | data);
						return;

					case Instructions.TInvVS:
						data = (1 << (data & 0x07));
						this.SetFlag(TinyProcessor.FlagZero, (this.GetRegister(n) & data) == 0);
						this.SetRegister(n, this.GetRegister(n) ^ data);
						return;
				}
			}

			if (op >= (int) Instructions.TestVA && op <= (int) Instructions.TestVA + 0x03)  // op #val,ADDR
			{
				Instructions i = (Instructions) (op & 0xFF);
				data = this.memory.Read(this.registerPC++);
				address = this.AddressAbs;

				switch (i)
				{
					case Instructions.TestVA:
						data = (1 << (data & 0x07));
						this.SetFlag(TinyProcessor.FlagZero, (this.memory.Read(address) & data) == 0);
						return;

					case Instructions.TClrVA:
						data = (1 << (data & 0x07));
						this.SetFlag(TinyProcessor.FlagZero, (this.memory.Read(address) & data) == 0);
						this.memory.Write(address, this.memory.Read(address) & ~data);
						return;

					case Instructions.TSetVA:
						data = (1 << (data & 0x07));
						this.SetFlag(TinyProcessor.FlagZero, (this.memory.Read(address) & data) == 0);
						this.memory.Write(address, this.memory.Read(address) | data);
						return;

					case Instructions.TInvVA:
						data = (1 << (data & 0x07));
						this.SetFlag(TinyProcessor.FlagZero, (this.memory.Read(address) & data) == 0);
						this.memory.Write(address, this.memory.Read(address) ^ data);
						return;
				}
			}

			if (op >= (int) Instructions.CompVR && op <= (int) Instructions.CompVR + 0x03)  // COMP #val,r
			{
				int n = op & 0x03;

				data = this.memory.Read(this.registerPC++);
				this.SetFlagsCompare(this.GetRegister(n), data);
				return;
			}
		}



		protected int GetRegister(int n)
		{
			//	Retourne le contenu d'un registre A,B,X,Y.
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
			//	Modifie le contenu d'un registre A,B,X,Y.
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


		protected int RotateRight(int value, bool withCarry)
		{
			bool bit = (value & 0x01) != 0;

			value = value >> 1;

			if (withCarry)
			{
				if (this.TestFlag(TinyProcessor.FlagCarry))
				{
					value |= 0x80;
				}
			}
			else
			{
				if (bit)
				{
					value |= 0x80;
				}
			}

			this.SetFlag(TinyProcessor.FlagCarry, bit);
			return value;
		}

		protected int RotateLeft(int value, bool withCarry)
		{
			bool bit = (value & 0x80) != 0;

			value = value << 1;

			if (withCarry)
			{
				if (this.TestFlag(TinyProcessor.FlagCarry))
				{
					value |= 0x01;
				}
			}
			else
			{
				if (bit)
				{
					value |= 0x01;
				}
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


		protected int AddressAbs
		{
			//	Lit ADDR qui suit une instruction, et gère les différents modes d'adressages.
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

				if ((mode & 0x1000) != 0)  // +{X} ?
				{
					address += this.registerX;
				}

				if ((mode & 0x2000) != 0)  // +{Y} ?
				{
					address += this.registerY;
				}

				return address;
			}
		}


		protected void SetFlagsCompare(int a, int b)
		{
			this.SetFlag(TinyProcessor.FlagZero, a == b);
			this.SetFlag(TinyProcessor.FlagCarry, a >= b);
		}

		protected int SetFlagsOper(int value)
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
		//	mod	A, F
		protected static byte[] WaitKey =
		{
			(byte) Instructions.MoveAR+0, 0x0C, 0x07,	// MOVE C07,A		; lit le clavier
			(byte) Instructions.TClrVS+0, 0x07,			// TCLR A:#7		; bit full ?
			(byte) Instructions.JumpEQ, 0x8F, 0xF8,		// JUMP,EQ R8^LOOP	; non, jump loop
			(byte) Instructions.Ret,					// RET
		};

		//	Affiche des segments à choix.
		//	in	A segments à allumer
		//		B digit 0..3
		//	out	-
		//	mod	F
		protected static byte[] DisplayBinaryDigit =
		{
			(byte) Instructions.PushR+1,				// PUSH B
			(byte) Instructions.PushR+2,				// PUSH X

			(byte) Instructions.AndVS+1, 0x03,			// AND #03,B
			(byte) Instructions.MoveRR+0x6,				// MOVE B,X
			(byte) Instructions.MoveRA+0, 0x1C, 0x00,	// MOVE A,C00+{X}

			(byte) Instructions.PopR+2,					// POP X
			(byte) Instructions.PopR+1,					// POP B
			(byte) Instructions.Ret,					// RET
		};

		//	Affiche un digit hexadécimal.
		//	in	A valeur 0..15
		//		B digit 0..3
		//	out	-
		//	mod	F
		protected static byte[] DisplayHexaDigit =
		{
			(byte) Instructions.PushR+0,				// PUSH A
			(byte) Instructions.PushR+1,				// PUSH B
			(byte) Instructions.PushR+2,				// PUSH X

			(byte) Instructions.AndVS+0, 0x0F,			// AND #0F,A
			(byte) Instructions.MoveRR+0x2,				// MOVE A,X
			(byte) Instructions.MoveAR+0, 0x90, 0x0A,	// MOVE R8^TABLE+{X},A

			(byte) Instructions.AndVS+1, 0x03,			// AND #03,B
			(byte) Instructions.MoveRR+0x6,				// MOVE B,X
			(byte) Instructions.MoveRA+0, 0x1C, 0x00,	// MOVE A,C00+{X}

			(byte) Instructions.PopR+2,					// POP X
			(byte) Instructions.PopR+1,					// POP B
			(byte) Instructions.PopR+0,					// POP A
			(byte) Instructions.Ret,					// RET
														// TABLE:
			0x3F, 0x03, 0x6D, 0x67, 0x53, 0x76, 0x7E, 0x23, 0x7F, 0x77, 0x7B, 0x5E, 0x3C, 0x4F, 0x7C, 0x78,
		};

		//	Affiche un byte hexadécimal sur deux digits.
		//	in	A valeur 0..255
		//		B premier digit 0..2
		//	out	-
		//	mod	F
		protected static byte[] DisplayHexaByte =
		{
			(byte) Instructions.PushR+0,				// PUSH A
			(byte) Instructions.PushR+1,				// PUSH B

			(byte) Instructions.IncR+1,					// INC B
			(byte) Instructions.Call, 0x80, 0x06,		// CALL DisplayHexaDigit

			(byte) Instructions.RrS+0,					// RR A
			(byte) Instructions.RrS+0,					// RR A
			(byte) Instructions.RrS+0,					// RR A
			(byte) Instructions.RrS+0,					// RR A
			(byte) Instructions.DecR+1,					// DEC B
			(byte) Instructions.Call, 0x80, 0x06,		// CALL DisplayHexaDigit

			(byte) Instructions.PopR+1,					// POP B
			(byte) Instructions.PopR+0,					// POP A
			(byte) Instructions.Ret,					// RET
		};

		//	Affiche une valeur décimale.
		//	in	A valeur 0..255
		//		B premier digit 0..2
		//	out	-
		//	mod	F
		protected static byte[] DisplayDecimal =
		{
			(byte) Instructions.Ret,					// RET
		};

		//	Allume un pixel dans l'écran bitmap.
		//	in	A coordonnée X 0..31
		//		B coordonnée Y 0..23
		//	out	-
		//	mod	F
		protected static byte[] SetPixel =
		{
			(byte) Instructions.PushR+0,				// PUSH A
			(byte) Instructions.PushR+1,				// PUSH B
			(byte) Instructions.PushR+2,				// PUSH X
			(byte) Instructions.PushR+3,				// PUSH Y

			(byte) Instructions.AndVS+1, 0x1F,			// AND #1F,B
			(byte) Instructions.RlS+1,					// RL B
			(byte) Instructions.RlS+1,					// RL B
			(byte) Instructions.MoveRR+0x7,				// MOVE B,Y

			(byte) Instructions.PushR+0,				// PUSH A
			(byte) Instructions.RrS+0,					// RR A
			(byte) Instructions.RrS+0,					// RR A
			(byte) Instructions.RrS+0,					// RR A
			(byte) Instructions.AndVS+0, 0x03,			// AND #03,A
			(byte) Instructions.MoveRR+0x2,				// MOVE A,X
			(byte) Instructions.PopR+0,					// POP A

			(byte) Instructions.XorVS+0, 0x07,			// XOR #07,A
			(byte) Instructions.TSetSA+0, 0x3C, 0x80,	// TSET C80+{X}+{Y},A

			(byte) Instructions.PopR+3,					// POP Y
			(byte) Instructions.PopR+2,					// POP X
			(byte) Instructions.PopR+1,					// POP B
			(byte) Instructions.PopR+0,					// POP A
			(byte) Instructions.Ret,					// RET
		};

		//	Eteint un pixel dans l'écran bitmap.
		//	in	A coordonnée X 0..31
		//		B coordonnée Y 0..23
		//	out	-
		//	mod	F
		protected static byte[] ClrPixel =
		{
			(byte) Instructions.Ret,					// RET
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
				chapters.Add("Notation");
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

				case "Notation":
					AbstractProcessor.HelpPutTitle(builder, "Registre");
					AbstractProcessor.HelpPutLine(builder, "<b>[xx+r]      <tab/>r</b>");
					AbstractProcessor.HelpPutLine(builder, "r=0            <tab/>A");
					AbstractProcessor.HelpPutLine(builder, "r=1            <tab/>B");
					AbstractProcessor.HelpPutLine(builder, "r=2            <tab/>X");
					AbstractProcessor.HelpPutLine(builder, "r=3            <tab/>Y");
					AbstractProcessor.HelpPutLine(builder, "");
					AbstractProcessor.HelpPutLine(builder, "<b>[xx+r']     <tab/>r'</b>");
					AbstractProcessor.HelpPutLine(builder, "r'=0           <tab/>A");
					AbstractProcessor.HelpPutLine(builder, "r'=1           <tab/>B");

					AbstractProcessor.HelpPutTitle(builder, "Valeur immédiate");
					AbstractProcessor.HelpPutLine(builder, "<b>[vv]         <tab/>#val</b>");
					AbstractProcessor.HelpPutLine(builder, "vv              <tab/>Valeur positive 8 bits.");

					AbstractProcessor.HelpPutTitle(builder, "Adresse");
					AbstractProcessor.HelpPutLine(builder, "<b>[mh] [ll]    <tab/>ADDR</b>");
					AbstractProcessor.HelpPutLine(builder, "m=0             <tab/>Adresse absolue 12 bits");
					AbstractProcessor.HelpPutLine(builder, "m=1             <tab/>+{X}");
					AbstractProcessor.HelpPutLine(builder, "m=2             <tab/>+{Y}");
					AbstractProcessor.HelpPutLine(builder, "m=8             <tab/>Adresse relative 12 bits signés");
					break;

				case "Op":
					AbstractProcessor.HelpPutTitle(builder, "Transferts");
					AbstractProcessor.HelpPutLine(builder, "[50+r] [vv]     <tab/>MOVE A,r");
					AbstractProcessor.HelpPutLine(builder, "[54+r] [vv]     <tab/>MOVE B,r");
					AbstractProcessor.HelpPutLine(builder, "[58+r] [vv]     <tab/>MOVE X,r");
					AbstractProcessor.HelpPutLine(builder, "[5C+r] [vv]     <tab/>MOVE Y,r");
					AbstractProcessor.HelpPutLine(builder, "[60+r] [vv]     <tab/>MOVE #val,r");
					AbstractProcessor.HelpPutLine(builder, "[64+r] [mh] [ll]<tab/>MOVE ADDR,r");
					AbstractProcessor.HelpPutLine(builder, "[68+r] [mh] [ll]<tab/>MOVE r,ADDR");

					AbstractProcessor.HelpPutTitle(builder, "Additions");
					AbstractProcessor.HelpPutLine(builder, "[80+r]          <tab/>ADD A,r");
					AbstractProcessor.HelpPutLine(builder, "[84+r]          <tab/>ADD B,r");
					AbstractProcessor.HelpPutLine(builder, "[88+r]          <tab/>ADD X,r");
					AbstractProcessor.HelpPutLine(builder, "[8C+r]          <tab/>ADD Y,r");
					AbstractProcessor.HelpPutLine(builder, "[A0+r] [vv]     <tab/>ADD #val,r");
					AbstractProcessor.HelpPutLine(builder, "[B0+r] [mh] [ll]<tab/>ADD ADDR,r");
					AbstractProcessor.HelpPutLine(builder, "[B8+r] [mh] [ll]<tab/>ADD r,ADDR");

					AbstractProcessor.HelpPutTitle(builder, "Soustractions");
					AbstractProcessor.HelpPutLine(builder, "[90+r]          <tab/>SUB A,r");
					AbstractProcessor.HelpPutLine(builder, "[94+r]          <tab/>SUB B,r");
					AbstractProcessor.HelpPutLine(builder, "[98+r]          <tab/>SUB X,r");
					AbstractProcessor.HelpPutLine(builder, "[9C+r]          <tab/>SUB Y,r");
					AbstractProcessor.HelpPutLine(builder, "[A4+r] [vv]     <tab/>SUB #val,r");
					AbstractProcessor.HelpPutLine(builder, "[B4+r] [mh] [ll]<tab/>SUB ADDR,r");
					AbstractProcessor.HelpPutLine(builder, "[BC+r] [mh] [ll]<tab/>SUB r,ADDR");

					AbstractProcessor.HelpPutTitle(builder, "ET logique");
					AbstractProcessor.HelpPutLine(builder, "[C8]             <tab/>AND A,B");
					AbstractProcessor.HelpPutLine(builder, "[C9]             <tab/>AND B,A");
					AbstractProcessor.HelpPutLine(builder, "[C0+r'] [vv]     <tab/>AND #val,r'");
					AbstractProcessor.HelpPutLine(builder, "[D0+r'] [mh] [ll]<tab/>AND ADDR,r'");
					AbstractProcessor.HelpPutLine(builder, "[D8+r'] [mh] [ll]<tab/>AND r',ADDR");

					AbstractProcessor.HelpPutTitle(builder, "OU logique");
					AbstractProcessor.HelpPutLine(builder, "[CA]             <tab/>OR A,B");
					AbstractProcessor.HelpPutLine(builder, "[CB]             <tab/>OR B,A");
					AbstractProcessor.HelpPutLine(builder, "[C2+r'] [vv]     <tab/>OR #val,r'");
					AbstractProcessor.HelpPutLine(builder, "[D2+r'] [mh] [ll]<tab/>OR ADDR,r'");
					AbstractProcessor.HelpPutLine(builder, "[DA+r'] [mh] [ll]<tab/>OR r',ADDR");

					AbstractProcessor.HelpPutTitle(builder, "OU exclusif logique");
					AbstractProcessor.HelpPutLine(builder, "[CC]             <tab/>XOR A,B");
					AbstractProcessor.HelpPutLine(builder, "[CD]             <tab/>XOR B,A");
					AbstractProcessor.HelpPutLine(builder, "[C4+r'] [vv]     <tab/>XOR #val,r'");
					AbstractProcessor.HelpPutLine(builder, "[D4+r'] [mh] [ll]<tab/>XOR ADDR,r'");
					AbstractProcessor.HelpPutLine(builder, "[DC+r'] [mh] [ll]<tab/>XOR r',ADDR");

					AbstractProcessor.HelpPutTitle(builder, "Tester un bit");
					AbstractProcessor.HelpPutLine(builder, "[E0]             <tab/>TEST B:A");
					AbstractProcessor.HelpPutLine(builder, "[E1]             <tab/>TEST A:B");
					AbstractProcessor.HelpPutLine(builder, "[F0+r'] [vv]     <tab/>TEST r':#val");
					AbstractProcessor.HelpPutLine(builder, "[E8+r'] [mh] [ll]<tab/>TEST ADDR:r'");

					AbstractProcessor.HelpPutTitle(builder, "Tester puis mettre un bit à un");
					AbstractProcessor.HelpPutLine(builder, "[E2]             <tab/>TSET B:A");
					AbstractProcessor.HelpPutLine(builder, "[E3]             <tab/>TSET A:B");
					AbstractProcessor.HelpPutLine(builder, "[F2+r'] [vv]     <tab/>TSET r':#val");
					AbstractProcessor.HelpPutLine(builder, "[EA+r'] [mh] [ll]<tab/>TSET ADDR:r'");

					AbstractProcessor.HelpPutTitle(builder, "Tester puis mettre un bit à zéro");
					AbstractProcessor.HelpPutLine(builder, "[E4]             <tab/>TCLR B:A");
					AbstractProcessor.HelpPutLine(builder, "[E5]             <tab/>TCLR A:B");
					AbstractProcessor.HelpPutLine(builder, "[F4+r'] [vv]     <tab/>TCLR r':#val");
					AbstractProcessor.HelpPutLine(builder, "[EC+r'] [mh] [ll]<tab/>TCLR ADDR:r'");

					AbstractProcessor.HelpPutTitle(builder, "Tester puis inverser un bit");
					AbstractProcessor.HelpPutLine(builder, "[E6]             <tab/>TINV B:A");
					AbstractProcessor.HelpPutLine(builder, "[E7]             <tab/>TINV A:B");
					AbstractProcessor.HelpPutLine(builder, "[F6+r'] [vv]     <tab/>TINV r':#val");
					AbstractProcessor.HelpPutLine(builder, "[EE+r'] [mh] [ll]<tab/>TINV ADDR:r'");

					AbstractProcessor.HelpPutTitle(builder, "Comparaisons");
					AbstractProcessor.HelpPutLine(builder, "[FC+r] [vv]<tab/>COMP #val,r");

					AbstractProcessor.HelpPutTitle(builder, "Opérations unaires");
					AbstractProcessor.HelpPutLine(builder, "[30+r]          <tab/>CLR r");
					AbstractProcessor.HelpPutLine(builder, "[34+r]          <tab/>NOT r");
					AbstractProcessor.HelpPutLine(builder, "[38+r]          <tab/>INC r");
					AbstractProcessor.HelpPutLine(builder, "[3C+r]          <tab/>DEC r");
					AbstractProcessor.HelpPutLine(builder, "[48] [mh] [ll]  <tab/>CLR ADDR");
					AbstractProcessor.HelpPutLine(builder, "[49] [mh] [ll]  <tab/>NOT ADDR");
					AbstractProcessor.HelpPutLine(builder, "[4A] [mh] [ll]  <tab/>INC ADDR");
					AbstractProcessor.HelpPutLine(builder, "[4B] [mh] [ll]  <tab/>DEC ADDR");

					AbstractProcessor.HelpPutTitle(builder, "Rotations");
					AbstractProcessor.HelpPutLine(builder, "[40+r']         <tab/>RL r'");
					AbstractProcessor.HelpPutLine(builder, "[42+r']         <tab/>RR r'");
					AbstractProcessor.HelpPutLine(builder, "[44+r']         <tab/>RLC r'");
					AbstractProcessor.HelpPutLine(builder, "[46+r']         <tab/>RRC r'");
					AbstractProcessor.HelpPutLine(builder, "[4C] [mh] [ll]  <tab/>RL ADDR");
					AbstractProcessor.HelpPutLine(builder, "[4D] [mh] [ll]  <tab/>RR ADDR");
					AbstractProcessor.HelpPutLine(builder, "[4E] [mh] [ll]  <tab/>RLC ADDR");
					AbstractProcessor.HelpPutLine(builder, "[4F] [mh] [ll]  <tab/>RRC ADDR");

					AbstractProcessor.HelpPutTitle(builder, "Divers");
					AbstractProcessor.HelpPutLine(builder, "[00]            <tab/>NOP");
					AbstractProcessor.HelpPutLine(builder, "[02]            <tab/>HALT");
					break;

				case "Branch":
					AbstractProcessor.HelpPutTitle(builder, "Sauts");
					AbstractProcessor.HelpPutLine(builder, "[10] [mh] [ll]<tab/>JUMP ADDR");
					AbstractProcessor.HelpPutLine(builder, "[12] [mh] [ll]<tab/>JUMP,EQ ADDR");
					AbstractProcessor.HelpPutLine(builder, "[13] [mh] [ll]<tab/>JUMP,NE ADDR");
					AbstractProcessor.HelpPutLine(builder, "[14] [mh] [ll]<tab/>JUMP,LO ADDR");
					AbstractProcessor.HelpPutLine(builder, "[15] [mh] [ll]<tab/>JUMP,LS ADDR");
					AbstractProcessor.HelpPutLine(builder, "[16] [mh] [ll]<tab/>JUMP,HI ADDR");
					AbstractProcessor.HelpPutLine(builder, "[17] [mh] [ll]<tab/>JUMP,HS ADDR");
					AbstractProcessor.HelpPutLine(builder, "[18] [mh] [ll]<tab/>JUMP,VC ADDR");
					AbstractProcessor.HelpPutLine(builder, "[19] [mh] [ll]<tab/>JUMP,VS ADDR");
					AbstractProcessor.HelpPutLine(builder, "[1A] [mh] [ll]<tab/>JUMP,NC ADDR");
					AbstractProcessor.HelpPutLine(builder, "[1B] [mh] [ll]<tab/>JUMP,NS ADDR");

					AbstractProcessor.HelpPutTitle(builder, "Appels de routines");
					AbstractProcessor.HelpPutLine(builder, "[20] [mh] [ll]<tab/>CALL ADDR");
					AbstractProcessor.HelpPutLine(builder, "[22] [mh] [ll]<tab/>CALL,EQ ADDR");
					AbstractProcessor.HelpPutLine(builder, "[23] [mh] [ll]<tab/>CALL,NE ADDR");
					AbstractProcessor.HelpPutLine(builder, "[24] [mh] [ll]<tab/>CALL,LO ADDR");
					AbstractProcessor.HelpPutLine(builder, "[25] [mh] [ll]<tab/>CALL,LS ADDR");
					AbstractProcessor.HelpPutLine(builder, "[26] [mh] [ll]<tab/>CALL,HI ADDR");
					AbstractProcessor.HelpPutLine(builder, "[27] [mh] [ll]<tab/>CALL,HS ADDR");
					AbstractProcessor.HelpPutLine(builder, "[28] [mh] [ll]<tab/>CALL,VC ADDR");
					AbstractProcessor.HelpPutLine(builder, "[29] [mh] [ll]<tab/>CALL,VS ADDR");
					AbstractProcessor.HelpPutLine(builder, "[2A] [mh] [ll]<tab/>CALL,NC ADDR");
					AbstractProcessor.HelpPutLine(builder, "[2B] [mh] [ll]<tab/>CALL,NS ADDR");
					AbstractProcessor.HelpPutLine(builder, "");
					AbstractProcessor.HelpPutLine(builder, "[01]<tab/><tab/>RET");

					AbstractProcessor.HelpPutTitle(builder, "Utilisation de la pile");
					AbstractProcessor.HelpPutLine(builder, "[08+r]           <tab/>PUSH r");
					AbstractProcessor.HelpPutLine(builder, "[0C+r]           <tab/>POP r");

					AbstractProcessor.HelpPutTitle(builder, "Gestion des fanions");
					AbstractProcessor.HelpPutLine(builder, "[04]             <tab/>SETC");
					AbstractProcessor.HelpPutLine(builder, "[05]             <tab/>CLRC");
					AbstractProcessor.HelpPutLine(builder, "[06]             <tab/>SETV");
					AbstractProcessor.HelpPutLine(builder, "[07]             <tab/>CLRV");
					break;

				case "ROM":
					AbstractProcessor.HelpPutTitle(builder, "ROM");
					AbstractProcessor.HelpPutLine(builder, "");
					AbstractProcessor.HelpPutLine(builder, "");
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
