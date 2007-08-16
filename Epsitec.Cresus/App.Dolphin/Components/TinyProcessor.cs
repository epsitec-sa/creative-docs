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
			Call   = 0x01,
			Ret    = 0x02,
			Halt   = 0x03,
			SetC   = 0x04,
			ClrC   = 0x05,
			AddVSP = 0x06,		// ADD #val,SP
			SubVSP = 0x07,		// SUB #val,SP

			PushR  = 0x08,		// PUSH r
			PopR   = 0x0C,		// POP r

			Jump   = 0x10,
			JumpEQ = 0x12,
			JumpNE = 0x13,
			JumpLO = 0x14,
			JumpLS = 0x15,
			JumpHI = 0x16,
			JumpHS = 0x17,
			JumpCC = 0x18,
			JumpCS = 0x19,
			JumpNC = 0x1A,
			JumpNS = 0x1B,

			ClrR   = 0x20,		// op r
			NotR   = 0x24,
			IncR   = 0x28,
			DecR   = 0x2C,

			RlR    = 0x30,		// op r
			RrR    = 0x34,
			RlcR   = 0x38,
			RrcR   = 0x3C,

			MoveRR = 0x40,		// MOVE r,r
			MoveVR = 0x50,		// MOVE #val,r
			MoveAR = 0x54,		// MOVE ADDR,r
			MoveRA = 0x58,		// MOVE r,ADDR
			ExAB   = 0x5C,		// EX A,B
			ExXY   = 0x5D,		// EX X,Y
			SwapA  = 0x5E,		// SWAP A
			SwapB  = 0x5F,		// SWAP B

			CompRR = 0x60,		// COMP r,r

			CompVR = 0x70,		// COMP #val,r
			AndVR  = 0x74,		// op #val,r
			OrVR   = 0x78,
			XorVR  = 0x7C,

			AddRR  = 0x80,		// op r,r
			SubRR  = 0x90,
			AddVR  = 0xA0,		// op #val,r
			SubVR  = 0xA4,

			ClrA   = 0xA8,		// op ADDR
			NotA   = 0xA9,
			IncA   = 0xAA,
			DecA   = 0xAB,
			RlA    = 0xAC,
			RrA    = 0xAD,
			RlcA   = 0xAE,
			RrcA   = 0xAF,

			AddAR  = 0xB0,		// op ADDR,r
			SubAR  = 0xB4,
			AddRA  = 0xB8,		// op r,ADDR
			SubRA  = 0xBC,

			TestSS = 0xC0,		// op r',r'
			TSetSS = 0xC2,
			TClrSS = 0xC4,
			TNotSS = 0xC6,
			TestSA = 0xC8,		// op r',ADDR
			TSetSA = 0xCA,
			TClrSA = 0xCC,
			TNotSA = 0xCE,

			TestVS = 0xD0,		// op #val,r'
			TSetVS = 0xD2,
			TClrVS = 0xD4,
			TNotVS = 0xD6,
			TestVA = 0xD8,		// op #val,ADDR (instructions à 4 bytes non documentées)
			TSetVA = 0xD9,
			TClrVA = 0xDA,
			TNotVA = 0xDB,
			MoveVA = 0xDC,		// op #val,ADDR
			CompVA = 0xDD,
			AddVA  = 0xDE,
			SubVA  = 0xDF,
			
			AndSS  = 0xE0,		// op r',r'
			OrSS   = 0xE2,
			XorSS  = 0xE4,
			AndAS  = 0xE8,		// op ADDR,r'
			OrAS   = 0xEA,
			XorAS  = 0xEC,
			AndSA  = 0xF0,		// op r',ADDR
			OrSA   = 0xF2,
			XorSA  = 0xF4,

			CompAR = 0xF8,		// COMP ADDR,r

			Table  = 0xFF,		// début/fin de table
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

			if (op == Instructions.Call)
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

		public override int NopInstruction
		{
			//	Retourne le code de l'instruction NOP.
			get
			{
				return (int) Instructions.Nop;
			}
		}

		public override int TableInstruction
		{
			//	Retourne le code de l'instruction TABLE.
			get
			{
				return (int) Instructions.Table;
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
			//?System.Diagnostics.Debug.WriteLine(string.Format("PC={0} i={1} F={2} A={3} B={4} X={5} Y={6}", this.registerPC.ToString("X4"), op.ToString("X2"), this.registerF.ToString("X2"), this.registerA.ToString("X2"), this.registerB.ToString("X2"), this.registerX.ToString("X2"), this.registerY.ToString("X2")));

			switch ((Instructions) op)
			{
				case Instructions.Nop:
				case Instructions.Table:
					return;

				case Instructions.Call:
					address = this.AddressAbs;
					this.StackPushWord(this.registerPC);
					this.registerPC = address;
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

				case Instructions.AddVSP:
					this.registerSP += this.memory.Read(this.registerPC++);
					return;

				case Instructions.SubVSP:
					this.registerSP -= this.memory.Read(this.registerPC++);
					return;

				case Instructions.ExAB:
					data = this.registerA;
					this.registerA = this.registerB;
					this.registerB = data;
					return;

				case Instructions.ExXY:
					data = this.registerX;
					this.registerX = this.registerY;
					this.registerY = data;
					return;

				case Instructions.SwapA:
					this.registerA = ((this.registerA << 4) & 0xF0) | ((this.registerA >> 4) & 0x0F);
					return;

				case Instructions.SwapB:
					this.registerB = ((this.registerB << 4) & 0xF0) | ((this.registerB >> 4) & 0x0F);
					return;
			}

			if (op >= (int) Instructions.PushR && op < (int) Instructions.PushR+4)  // PUSH r
			{
				int n = op & 0x03;
				this.StackPushByte(this.GetRegister(n));
				return;
			}

			if (op >= (int) Instructions.PopR && op < (int) Instructions.PopR+4)  // POP r
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

			if (op >= (int) Instructions.ClrR && op < (int) Instructions.ClrR+16)  // op r
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
						data = (this.GetRegister(n) + 1) & 0xFF;
						this.SetRegister(n, data);
						this.SetFlagsOper(data);
						return;

					case Instructions.DecR:
						data = (this.GetRegister(n) - 1) & 0xFF;
						this.SetRegister(n, data);
						this.SetFlagsOper(data);
						return;
				}
			}

			if (op >= (int) Instructions.RlR && op < (int) Instructions.RlR+16)  // op r
			{
				int n = op & 0x03;
				Instructions i = (Instructions) (op & 0xFC);

				switch (i)
				{
					case Instructions.RlR:
						data = this.RotateLeft(this.GetRegister(n), false);
						this.SetRegister(n, data);
						return;

					case Instructions.RrR:
						data = this.RotateRight(this.GetRegister(n), false);
						this.SetRegister(n, data);
						return;

					case Instructions.RlcR:
						data = this.RotateLeft(this.GetRegister(n), true);
						this.SetRegister(n, data);
						return;

					case Instructions.RrcR:
						data = this.RotateRight(this.GetRegister(n), true);
						this.SetRegister(n, data);
						return;
				}
			}

			if (op >= (int) Instructions.ClrA && op < (int) Instructions.ClrA+8)  // op ADDR
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
						data = (this.memory.Read(address) + 1) & 0xFF;
						this.memory.Write(address, data);
						this.SetFlagsOper(data);
						return;

					case Instructions.DecA:
						data = (this.memory.Read(address) - 1) & 0xFF;
						this.memory.Write(address, data);
						this.SetFlagsOper(data);
						return;

					case Instructions.RlA:
						data = this.RotateLeft(this.memory.Read(address), false);
						this.memory.Write(address, data);
						return;

					case Instructions.RrA:
						data = this.RotateRight(this.memory.Read(address), false);
						this.memory.Write(address, data);
						return;

					case Instructions.RlcA:
						data = this.RotateLeft(this.memory.Read(address), true);
						this.memory.Write(address, data);
						return;

					case Instructions.RrcA:
						data = this.RotateRight(this.memory.Read(address), true);
						this.memory.Write(address, data);
						return;
				}
			}

			if (op >= (int) Instructions.MoveRR && op < (int) Instructions.MoveRR+16)  // MOVE r,r
			{
				int src = (op>>2) & 0x03;
				int dst = op & 0x03;

				data = this.GetRegister(src);
				this.SetRegister(dst, data);
				this.SetFlagsOper(data);
				return;
			}

			if (op >= (int) Instructions.MoveVR && op < (int) Instructions.MoveVR+4)  // MOVE #val,r
			{
				int n = op & 0x03;

				data = this.memory.Read(this.registerPC++);
				this.SetRegister(n, data);
				this.SetFlagsOper(data);
				return;
			}

			if (op >= (int) Instructions.MoveAR && op < (int) Instructions.MoveAR+4)  // MOVE ADDR,r
			{
				int n = op & 0x03;
				address = this.AddressAbs;

				data = this.memory.Read(address);
				this.SetRegister(n, data);
				this.SetFlagsOper(data);
				return;
			}

			if (op >= (int) Instructions.MoveRA && op < (int) Instructions.MoveRA+4)  // MOVE r,ADDR
			{
				int n = op & 0x03;
				address = this.AddressAbs;

				data = this.GetRegister(n);
				this.memory.Write(address, data);
				this.SetFlagsOper(data);
				return;
			}

			if (op >= (int) Instructions.CompRR && op < (int) Instructions.CompRR+16)  // COMP r,r
			{
				int src = (op>>2) & 0x03;
				int dst = op & 0x03;

				this.SetFlagsCompare(this.GetRegister(dst), this.GetRegister(src));
				return;
			}

			if (op >= (int) Instructions.AddRR && op < (int) Instructions.AddRR+32)  // op r,r
			{
				int src = (op>>2) & 0x03;
				int dst = op & 0x03;
				Instructions i = (Instructions) (op & 0xF0);
				data = this.GetRegister(src);

				switch (i)
				{
					case Instructions.AddRR:
						data = (this.GetRegister(dst) + data) & 0xFF;
						this.SetRegister(dst, data);
						this.SetFlagsOper(data);
						return;

					case Instructions.SubRR:
						data = (this.GetRegister(dst) - data) & 0xFF;
						this.SetRegister(dst, data);
						this.SetFlagsOper(data);
						return;
				}
			}

			if (op >= (int) Instructions.AddVR && op < (int) Instructions.AddVR+8)  // op #val,r
			{
				int n = op & 0x03;
				Instructions i = (Instructions) (op & 0xFC);
				data = this.memory.Read(this.registerPC++);

				switch (i)
				{
					case Instructions.AddVR:
						data = (this.GetRegister(n) + data) & 0xFF;
						this.SetRegister(n, data);
						this.SetFlagsOper(data);
						return;

					case Instructions.SubVR:
						data = (this.GetRegister(n) - data) & 0xFF;
						this.SetRegister(n, data);
						this.SetFlagsOper(data);
						return;
				}
			}

			if (op >= (int) Instructions.AddAR && op < (int) Instructions.AddAR+8)  // op ADDR,r
			{
				int n = op & 0x03;
				Instructions i = (Instructions) (op & 0xFC);
				address = this.AddressAbs;

				switch (i)
				{
					case Instructions.AddAR:
						data = (this.GetRegister(n) + this.memory.Read(address)) & 0xFF;
						this.SetRegister(n, data);
						this.SetFlagsOper(data);
						return;

					case Instructions.SubAR:
						data = (this.GetRegister(n) - this.memory.Read(address)) & 0xFF;
						this.SetRegister(n, data);
						this.SetFlagsOper(data);
						return;
				}
			}

			if (op >= (int) Instructions.AddRA && op < (int) Instructions.AddRA+8)  // op r,ADDR
			{
				int n = op & 0x03;
				Instructions i = (Instructions) (op & 0xFC);
				address = this.AddressAbs;

				switch (i)
				{
					case Instructions.AddRA:
						data = (this.memory.Read(address) + this.GetRegister(n)) & 0xFF;
						this.memory.Write(address, data);
						this.SetFlagsOper(data);
						return;

					case Instructions.SubRA:
						data = (this.memory.Read(address) - this.GetRegister(n)) & 0xFF;
						this.memory.Write(address, data);
						this.SetFlagsOper(data);
						return;
				}
			}

			if (op >= (int) Instructions.AndVR && op < (int) Instructions.AndVR+12)  // op #val,r
			{
				int n = op & 0x03;
				Instructions i = (Instructions) (op & 0xFC);
				data = this.memory.Read(this.registerPC++);

				switch (i)
				{
					case Instructions.AndVR:
						data = this.GetRegister(n) & data;
						this.SetRegister(n, data);
						this.SetFlagsOper(data);
						return;

					case Instructions.OrVR:
						data = this.GetRegister(n) | data;
						this.SetRegister(n, data);
						this.SetFlagsOper(data);
						return;

					case Instructions.XorVR:
						data = this.GetRegister(n) ^ data;
						this.SetRegister(n, data);
						this.SetFlagsOper(data);
						return;
				}
			}

			if (op >= (int) Instructions.AndSS && op < (int) Instructions.AndSS+8)  // op r',r'
			{
				int n = op & 0x01;
				int src = n;
				int dst = n ^ 0x01;
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

			if (op >= (int) Instructions.AndAS && op < (int) Instructions.AndAS+6)  // op ADDR,r'
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

			if (op >= (int) Instructions.AndSA && op < (int) Instructions.AndSA+6)  // op r',ADDR
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

			if (op >= (int) Instructions.TestSS && op < (int) Instructions.TestSS+8)  // op r',r'
			{
				int n = op & 0x01;
				int src = n;
				int dst = n ^ 0x01;
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

					case Instructions.TNotSS:
						data = (1 << (data & 0x07));
						this.SetFlag(TinyProcessor.FlagZero, (this.GetRegister(dst) & data) == 0);
						this.SetRegister(dst, this.GetRegister(dst) ^ data);
						return;
				}
			}

			if (op >= (int) Instructions.TestSA && op < (int) Instructions.TestSA+8)  // op r',ADDR
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

					case Instructions.TNotSA:
						data = (1 << (this.GetRegister(n) & 0x07));
						this.SetFlag(TinyProcessor.FlagZero, (this.memory.Read(address) & data) == 0);
						this.memory.Write(address, this.memory.Read(address) ^ data);
						return;
				}
			}

			if (op >= (int) Instructions.TestVS && op < (int) Instructions.TestVS+8)  // op #val,r'
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

					case Instructions.TNotVS:
						data = (1 << (data & 0x07));
						this.SetFlag(TinyProcessor.FlagZero, (this.GetRegister(n) & data) == 0);
						this.SetRegister(n, this.GetRegister(n) ^ data);
						return;
				}
			}

			if (op >= (int) Instructions.TestVA && op < (int) Instructions.TestVA+8)  // op #val,ADDR
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

					case Instructions.TNotVA:
						data = (1 << (data & 0x07));
						this.SetFlag(TinyProcessor.FlagZero, (this.memory.Read(address) & data) == 0);
						this.memory.Write(address, this.memory.Read(address) ^ data);
						return;

					case Instructions.MoveVA:
						this.memory.Write(address, data);
						this.SetFlagsOper(data);
						return;

					case Instructions.CompVA:
						this.SetFlagsCompare(this.memory.Read(address), data);
						return;

					case Instructions.AddVA:
						data = (this.memory.Read(address) + data) & 0xFF;
						this.memory.Write(address, data);
						this.SetFlagsOper(data);
						return;

					case Instructions.SubVA:
						data = (this.memory.Read(address) - data) & 0xFF;
						this.memory.Write(address, data);
						this.SetFlagsOper(data);
						return;
				}
			}

			if (op >= (int) Instructions.CompVR && op < (int) Instructions.CompVR+4)  // COMP #val,r
			{
				int n = op & 0x03;

				data = this.memory.Read(this.registerPC++);
				this.SetFlagsCompare(this.GetRegister(n), data);
				return;
			}

			if (op >= (int) Instructions.CompAR && op < (int) Instructions.CompAR+4)  // COMP ADDR,r
			{
				int n = op & 0x03;
				address = this.AddressAbs;

				data = this.memory.Read(address);
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
			this.SetFlag(TinyProcessor.FlagZero, value == 0);
			this.SetFlag(TinyProcessor.FlagNeg, (value & 0x80) != 0);

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
			this.SetFlag(TinyProcessor.FlagZero, value == 0);
			this.SetFlag(TinyProcessor.FlagNeg, (value & 0x80) != 0);

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

				if ((mode & 0x8000) != 0)  // {PC}+depl (relatif) ?
				{
					if ((address & 0x0800) != 0)  // offset négatif ?
					{
						address = address-0x1000;
					}

					address = this.registerPC + address;
				}

				if ((mode & 0x4000) != 0)  // {SP}+depl ?
				{
					if ((address & 0x0800) != 0)  // offset négatif ?
					{
						address = address-0x1000;
					}

					address = this.registerSP + address;
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
				this.SetFlag(TinyProcessor.FlagCarry, (value & 0xffffff00) != 0);
			}
			else  // valeur négative ?
			{
				this.SetFlag(TinyProcessor.FlagCarry, (value & 0xffffff00) == 0);
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

				case Instructions.JumpCC:
					return !this.TestFlag(TinyProcessor.FlagCarry);

				case Instructions.JumpCS:
					return this.TestFlag(TinyProcessor.FlagCarry);

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
				return "CZN";  // bits 0..7 !
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


		#region Code
		public override int GetInstructionLength(int code)
		{
			//	Retourne le nombre de bytes d'une instruction.
			int length = 0;

			if (code >= 0x00 && code <= 0xFF)
			{
				length = TinyProcessor.InstructionLength[code];
			}

			if (length == 0)  // instruction inconnue ?
			{
				System.Diagnostics.Debug.WriteLine(string.Format("Unknow instruction {0}", code.ToString("X2")));
				length = 1;
			}

			return length;
		}

		protected static readonly byte[] InstructionLength =
		{
			1,3,1,1,1,1,2,2, 1,1,1,1,1,1,1,1,  // 0x00
			3,0,3,3,3,3,3,3, 3,3,3,3,0,0,0,0,  // 0x10
			1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,  // 0x20
			1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,  // 0x30
			1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,  // 0x40
			2,2,2,2,3,3,3,3, 3,3,3,3,1,1,1,1,  // 0x50
			1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,  // 0x60
			2,2,2,2,2,2,2,2, 2,2,2,2,2,2,2,2,  // 0x70

			1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,  // 0x80
			1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,  // 0x90
			2,2,2,2,2,2,2,2, 3,3,3,3,3,3,3,3,  // 0xA0
			3,3,3,3,3,3,3,3, 3,3,3,3,3,3,3,3,  // 0xB0
			1,1,1,1,1,1,1,1, 3,3,3,3,3,3,3,3,  // 0xC0
			2,2,2,2,2,2,2,2, 4,4,4,4,4,4,4,4,  // 0xD0
			1,1,1,1,1,1,0,0, 3,3,3,3,3,3,0,0,  // 0xE0
			3,3,3,3,3,3,0,0, 3,3,3,3,0,0,0,1,  // 0xF0
		};

		public override string DessassemblyInstruction(List<int> codes)
		{
			//	Retourne le nom d'une instruction.
			if (codes == null || codes.Count == 0)
			{
				return null;
			}

			System.Text.StringBuilder builder = new System.Text.StringBuilder();
			int op = codes[0];
			//?System.Diagnostics.Debug.WriteLine(string.Format("i={0}", op.ToString("X2")));

			switch ((Instructions) op)
			{
				case Instructions.Nop:
					return "NOP";

				case Instructions.Call:
					builder.Append("CALL ");
					TinyProcessor.PutCodeAddress(builder, codes[1], codes[2]);
					return builder.ToString();

				case Instructions.Ret:
					return "RET";

				case Instructions.Halt:
					return "HALT";

				case Instructions.SetC:
					return "SETC";

				case Instructions.ClrC:
					return "CLRC";

				case Instructions.AddVSP:
					builder.Append("ADD ");
					TinyProcessor.PutCodeValue(builder, codes[1]);
					builder.Append(",SP");
					return builder.ToString();

				case Instructions.SubVSP:
					builder.Append("SUB ");
					TinyProcessor.PutCodeValue(builder, codes[1]);
					builder.Append(",SP");
					return builder.ToString();

				case Instructions.ExAB:
					return "EX A,B";

				case Instructions.ExXY:
					return "EX X,Y";

				case Instructions.SwapA:
					return "SWAP A";

				case Instructions.SwapB:
					return "SWAP B";

				case Instructions.Table:
					return "TABLE";
			}

			if (op >= (int) Instructions.PushR && op < (int) Instructions.PushR+4)  // PUSH
			{
				builder.Append("PUSH ");
				int n = op & 0x03;
				TinyProcessor.PutCodeRegister(builder, n);
				return builder.ToString();
			}

			if (op >= (int) Instructions.PopR && op < (int) Instructions.PopR+4)  // POP
			{
				builder.Append("POP ");
				int n = op & 0x03;
				TinyProcessor.PutCodeRegister(builder, n);
				return builder.ToString();
			}

			if (op >= (int) Instructions.Jump && op <= (int) Instructions.JumpNS)  // JUMP
			{
				switch ((Instructions) op)
				{
					case Instructions.Jump:
						builder.Append("JUMP ");
						break;

					case Instructions.JumpEQ:
						builder.Append("JUMP,EQ ");
						break;

					case Instructions.JumpNE:
						builder.Append("JUMP,NE ");
						break;

					case Instructions.JumpLO:
						builder.Append("JUMP,LO ");
						break;

					case Instructions.JumpLS:
						builder.Append("JUMP,LS ");
						break;

					case Instructions.JumpHI:
						builder.Append("JUMP,HI ");
						break;

					case Instructions.JumpHS:
						builder.Append("JUMP,HS ");
						break;

					case Instructions.JumpCC:
						builder.Append("JUMP,CC ");
						break;

					case Instructions.JumpCS:
						builder.Append("JUMP,CS ");
						break;

					case Instructions.JumpNC:
						builder.Append("JUMP,NC ");
						break;

					case Instructions.JumpNS:
						builder.Append("JUMP,NS ");
						break;

					default:
						builder.Append("JUMP ");
						break;
				}

				TinyProcessor.PutCodeAddress(builder, codes[1], codes[2]);
				return builder.ToString();
			}

			if (op >= (int) Instructions.ClrR && op < (int) Instructions.ClrR+16)  // op r
			{
				int n = op & 0x03;
				Instructions i = (Instructions) (op & 0xFC);

				switch (i)
				{
					case Instructions.ClrR:
						builder.Append("CLR ");
						break;

					case Instructions.NotR:
						builder.Append("NOT ");
						break;

					case Instructions.IncR:
						builder.Append("INC ");
						break;

					case Instructions.DecR:
						builder.Append("DEC ");
						break;

					default:
						builder.Append("? ");
						break;
				}

				TinyProcessor.PutCodeRegister(builder, n);
				return builder.ToString();
			}

			if (op >= (int) Instructions.RlR && op < (int) Instructions.RlR+16)  // op r
			{
				int n = op & 0x03;
				Instructions i = (Instructions) (op & 0xFC);

				switch (i)
				{
					case Instructions.RlR:
						builder.Append("RL ");
						break;

					case Instructions.RrR:
						builder.Append("RR ");
						break;

					case Instructions.RlcR:
						builder.Append("RLC ");
						break;

					case Instructions.RrcR:
						builder.Append("RRC ");
						break;

					default:
						builder.Append("? ");
						break;
				}

				TinyProcessor.PutCodeRegister(builder, n);
				return builder.ToString();
			}

			if (op >= (int) Instructions.MoveRR && op < (int) Instructions.MoveRR+16)  // MOVE r,r
			{
				int src = (op>>2) & 0x03;
				int dst = op & 0x03;
				builder.Append("MOVE ");
				TinyProcessor.PutCodeRegister(builder, src);
				builder.Append(",");
				TinyProcessor.PutCodeRegister(builder, dst);
				return builder.ToString();
			}

			if (op >= (int) Instructions.MoveVR && op < (int) Instructions.MoveVR+4)  // MOVE #val,r
			{
				int n = op & 0x03;
				builder.Append("MOVE ");
				TinyProcessor.PutCodeValue(builder, codes[1]);
				builder.Append(",");
				TinyProcessor.PutCodeRegister(builder, n);
				return builder.ToString();
			}

			if (op >= (int) Instructions.MoveAR && op < (int) Instructions.MoveAR+4)  // MOVE ADDR,r
			{
				int n = op & 0x03;
				builder.Append("MOVE ");
				TinyProcessor.PutCodeAddress(builder, codes[1], codes[2]);
				builder.Append(",");
				TinyProcessor.PutCodeRegister(builder, n);
				return builder.ToString();
			}

			if (op >= (int) Instructions.MoveRA && op < (int) Instructions.MoveRA+4)  // MOVE r,ADDR
			{
				int n = op & 0x03;
				builder.Append("MOVE ");
				TinyProcessor.PutCodeRegister(builder, n);
				builder.Append(",");
				TinyProcessor.PutCodeAddress(builder, codes[1], codes[2]);
				return builder.ToString();
			}

			if (op >= (int) Instructions.CompRR && op < (int) Instructions.CompRR+16)  // COMP r,r
			{
				int src = (op>>2) & 0x03;
				int dst = op & 0x03;
				builder.Append("COMP ");
				TinyProcessor.PutCodeRegister(builder, src);
				builder.Append(",");
				TinyProcessor.PutCodeRegister(builder, dst);
				return builder.ToString();
			}

			if (op >= (int) Instructions.CompVR && op < (int) Instructions.CompVR+4)  // COMP #val,r
			{
				int n = op & 0x03;
				builder.Append("COMP ");
				TinyProcessor.PutCodeValue(builder, codes[1]);
				builder.Append(",");
				TinyProcessor.PutCodeRegister(builder, n);
				return builder.ToString();
			}

			if (op >= (int) Instructions.AndVR && op <= (int) Instructions.AndVR + 11)  // op #val,r
			{
				int n = op & 0x03;
				Instructions i = (Instructions) (op & 0xFC);

				switch (i)
				{
					case Instructions.AndVR:
						builder.Append("AND ");
						break;

					case Instructions.OrVR:
						builder.Append("OR ");
						break;

					case Instructions.XorVR:
						builder.Append("XOR ");
						break;

					default:
						builder.Append("? ");
						break;
				}

				TinyProcessor.PutCodeValue(builder, codes[1]);
				builder.Append(",");
				TinyProcessor.PutCodeRegister(builder, n);
				return builder.ToString();
			}

			if (op >= (int) Instructions.AddRR && op < (int) Instructions.AddRR+32)  // op r,r
			{
				int src = (op>>2) & 0x03;
				int dst = op & 0x03;
				Instructions i = (Instructions) (op & 0xF0);

				switch (i)
				{
					case Instructions.AddRR:
						builder.Append("ADD ");
						break;

					case Instructions.SubRR:
						builder.Append("SUB ");
						break;

					default:
						builder.Append("? ");
						break;
				}

				TinyProcessor.PutCodeRegister(builder, src);
				builder.Append(",");
				TinyProcessor.PutCodeRegister(builder, dst);
				return builder.ToString();
			}

			if (op >= (int) Instructions.AddVR && op < (int) Instructions.AddVR+4)  // ADD #val,r
			{
				int n = op & 0x03;
				builder.Append("ADD ");
				TinyProcessor.PutCodeValue(builder, codes[1]);
				builder.Append(",");
				TinyProcessor.PutCodeRegister(builder, n);
				return builder.ToString();
			}

			if (op >= (int) Instructions.SubVR && op < (int) Instructions.SubVR+4)  // SUB #val,r
			{
				int n = op & 0x03;
				builder.Append("SUB ");
				TinyProcessor.PutCodeValue(builder, codes[1]);
				builder.Append(",");
				TinyProcessor.PutCodeRegister(builder, n);
				return builder.ToString();
			}

			if (op >= (int) Instructions.ClrA && op < (int) Instructions.ClrA+8)  // op ADDR
			{
				Instructions i = (Instructions) op;

				switch (i)
				{
					case Instructions.ClrA:
						builder.Append("CLR ");
						break;

					case Instructions.NotA:
						builder.Append("NOT ");
						break;

					case Instructions.IncA:
						builder.Append("INC ");
						break;

					case Instructions.DecA:
						builder.Append("DEC ");
						break;

					case Instructions.RlA:
						builder.Append("RL ");
						break;

					case Instructions.RrA:
						builder.Append("RR ");
						break;

					case Instructions.RlcA:
						builder.Append("RLC ");
						break;

					case Instructions.RrcA:
						builder.Append("RRC ");
						break;

					default:
						builder.Append("? ");
						break;
				}

				TinyProcessor.PutCodeAddress(builder, codes[1], codes[2]);
				return builder.ToString();
			}

			if (op >= (int) Instructions.AddAR && op < (int) Instructions.AddAR+8)  // op ADDR,r
			{
				int n = op & 0x03;
				Instructions i = (Instructions) (op & 0xFC);

				switch (i)
				{
					case Instructions.AddAR:
						builder.Append("ADD ");
						break;

					case Instructions.SubAR:
						builder.Append("SUB ");
						break;

					default:
						builder.Append("? ");
						break;
				}

				TinyProcessor.PutCodeAddress(builder, codes[1], codes[2]);
				builder.Append(",");
				TinyProcessor.PutCodeRegister(builder, n);
				return builder.ToString();
			}

			if (op >= (int) Instructions.AddRA && op < (int) Instructions.AddRA+8)  // op r,ADDR
			{
				int n = op & 0x03;
				Instructions i = (Instructions) (op & 0xFC);

				switch (i)
				{
					case Instructions.AddRA:
						builder.Append("ADD ");
						break;

					case Instructions.SubRA:
						builder.Append("SUB ");
						break;

					default:
						builder.Append("? ");
						break;
				}

				builder.Append("MOVE ");
				TinyProcessor.PutCodeRegister(builder, n);
				builder.Append(",");
				TinyProcessor.PutCodeAddress(builder, codes[1], codes[2]);
				return builder.ToString();
			}

			if (op >= (int) Instructions.TestSS && op < (int) Instructions.TestSS+8)  // op r',r'
			{
				int n = op & 0x01;
				int src = n;
				int dst = n ^ 0x01;
				Instructions i = (Instructions) (op & 0xFE);

				switch (i)
				{
					case Instructions.TestSS:
						builder.Append("TEST ");
						break;

					case Instructions.TSetSS:
						builder.Append("TSET ");
						break;

					case Instructions.TClrSS:
						builder.Append("TCLR ");
						break;

					case Instructions.TNotSS:
						builder.Append("TNOT ");
						break;

					default:
						builder.Append("? ");
						break;
				}

				TinyProcessor.PutCodeRegister(builder, dst);
				builder.Append(":");
				TinyProcessor.PutCodeRegister(builder, src);
				return builder.ToString();
			}

			if (op >= (int) Instructions.TestSA && op < (int) Instructions.TestSA+8)  // op r',ADDR
			{
				int n = op & 0x01;
				Instructions i = (Instructions) (op & 0xFE);

				switch (i)
				{
					case Instructions.TestSA:
						builder.Append("TEST ");
						break;

					case Instructions.TSetSA:
						builder.Append("TSET ");
						break;

					case Instructions.TClrSA:
						builder.Append("TCLR ");
						break;

					case Instructions.TNotSA:
						builder.Append("TNOT ");
						break;

					default:
						builder.Append("? ");
						break;
				}

				TinyProcessor.PutCodeAddress(builder, codes[1], codes[2]);
				builder.Append(":");
				TinyProcessor.PutCodeRegister(builder, n);
				return builder.ToString();
			}

			if (op >= (int) Instructions.TestVS && op < (int) Instructions.TestVS+8)  // op #val,r'
			{
				int n = op & 0x01;
				Instructions i = (Instructions) (op & 0xFE);

				switch (i)
				{
					case Instructions.TestVS:
						builder.Append("TEST ");
						break;

					case Instructions.TSetVS:
						builder.Append("TSET ");
						break;

					case Instructions.TClrVS:
						builder.Append("TCLR ");
						break;

					case Instructions.TNotVS:
						builder.Append("TNOT ");
						break;

					default:
						builder.Append("? ");
						break;
				}

				TinyProcessor.PutCodeRegister(builder, n);
				builder.Append(":");
				TinyProcessor.PutCodeValue(builder, codes[1]);
				return builder.ToString();
			}

			if (op >= (int) Instructions.TestVA && op < (int) Instructions.TestVA+4)  // op #val,ADDR
			{
				int n = op & 0x01;
				Instructions i = (Instructions) (op & 0xFE);

				switch (i)
				{
					case Instructions.TestVA:
						builder.Append("TEST ");
						break;

					case Instructions.TSetVA:
						builder.Append("TSET ");
						break;

					case Instructions.TClrVA:
						builder.Append("TCLR ");
						break;

					case Instructions.TNotVA:
						builder.Append("TNOT ");
						break;

					default:
						builder.Append("? ");
						break;
				}

				TinyProcessor.PutCodeAddress(builder, codes[2], codes[3]);
				builder.Append(":");
				TinyProcessor.PutCodeValue(builder, codes[1]);
				return builder.ToString();
			}

			if (op >= (int) Instructions.MoveVA && op < (int) Instructions.MoveVA+4)  // op #val,ADDR
			{
				int n = op & 0x01;
				Instructions i = (Instructions) (op & 0xFE);

				switch (i)
				{
					case Instructions.MoveVA:
						builder.Append("MOVE ");
						break;

					case Instructions.CompVA:
						builder.Append("COMP ");
						break;

					case Instructions.AddVA:
						builder.Append("ADD ");
						break;

					case Instructions.SubVA:
						builder.Append("SUB ");
						break;

					default:
						builder.Append("? ");
						break;
				}

				TinyProcessor.PutCodeValue(builder, codes[1]);
				builder.Append(",");
				TinyProcessor.PutCodeAddress(builder, codes[2], codes[3]);
				return builder.ToString();
			}

			if (op >= (int) Instructions.AndSS && op < (int) Instructions.AndSS+6)  // op r',r'
			{
				int n = op & 0x01;
				int src = n;
				int dst = n ^ 0x01;
				Instructions i = (Instructions) (op & 0xFE);

				switch (i)
				{
					case Instructions.AndSS:
						builder.Append("AND ");
						break;

					case Instructions.OrSS:
						builder.Append("OR ");
						break;

					case Instructions.XorSS:
						builder.Append("XOR ");
						break;

					default:
						builder.Append("? ");
						break;
				}

				TinyProcessor.PutCodeRegister(builder, src);
				builder.Append(",");
				TinyProcessor.PutCodeRegister(builder, dst);
				return builder.ToString();
			}

			if (op >= (int) Instructions.AndAS && op < (int) Instructions.AndAS+6)  // op ADDR,r'
			{
				int n = op & 0x01;
				Instructions i = (Instructions) (op & 0xFE);

				switch (i)
				{
					case Instructions.AndAS:
						builder.Append("AND ");
						break;

					case Instructions.OrAS:
						builder.Append("OR ");
						break;

					case Instructions.XorAS:
						builder.Append("XOR ");
						break;

					default:
						builder.Append("? ");
						break;
				}

				TinyProcessor.PutCodeAddress(builder, codes[1], codes[2]);
				builder.Append(",");
				TinyProcessor.PutCodeRegister(builder, n);
				return builder.ToString();
			}

			if (op >= (int) Instructions.AndSA && op < (int) Instructions.AndSA+6)  // op ADDR,r'
			{
				int n = op & 0x01;
				Instructions i = (Instructions) (op & 0xFE);

				switch (i)
				{
					case Instructions.AndSA:
						builder.Append("AND ");
						break;

					case Instructions.OrSA:
						builder.Append("OR ");
						break;

					case Instructions.XorSA:
						builder.Append("XOR ");
						break;

					default:
						builder.Append("? ");
						break;
				}

				TinyProcessor.PutCodeRegister(builder, n);
				builder.Append(",");
				TinyProcessor.PutCodeAddress(builder, codes[1], codes[2]);
				return builder.ToString();
			}

			if (op >= (int) Instructions.CompAR && op < (int) Instructions.CompAR+4)  // COMP ADDR,r
			{
				int n = op & 0x03;
				builder.Append("COMP ");
				TinyProcessor.PutCodeValue(builder, codes[1]);
				builder.Append(",");
				TinyProcessor.PutCodeRegister(builder, n);
				return builder.ToString();
			}

			return "?";
		}

		protected static void PutCodeRegister(System.Text.StringBuilder builder, int n)
		{
			//	Met le nom d'un registre.
			switch (n)
			{
				case 0:
					builder.Append("A");
					break;

				case 1:
					builder.Append("B");
					break;

				case 2:
					builder.Append("X");
					break;

				case 3:
					builder.Append("Y");
					break;
			}
		}

		protected static void PutCodeValue(System.Text.StringBuilder builder, int val)
		{
			//	Met une valeur immédiate hexadécimale.
			builder.Append("#");
			builder.Append(val.ToString("X2"));
			builder.Append("h");
		}

		protected static void PutCodeAddress(System.Text.StringBuilder builder, int mh, int ll)
		{
			//	Met une adresse hexadécimale.
			int mode = (mh << 8) | ll;
			int address = mode & 0x0FFF;

			if ((mode & 0x8000) != 0)  // {PC}+depl (relatif) ?
			{
				builder.Append("{PC}");
				if ((address & 0x0800) != 0)  // offset négatif ?
				{
					address = 0x1000-address;
					builder.Append("-");
					builder.Append(address.ToString("X3"));
					builder.Append("h");
				}
				else
				{
					builder.Append("+");
					builder.Append(address.ToString("X3"));
					builder.Append("h");
				}
			}
			else if ((mode & 0x4000) != 0)  // {SP}+depl ?
			{
				builder.Append("{SP}");
				if ((address & 0x0800) != 0)  // offset négatif ?
				{
					address = 0x1000-address;
					builder.Append("-");
					builder.Append(address.ToString("X3"));
					builder.Append("h");
				}
				else
				{
					builder.Append("+");
					builder.Append(address.ToString("X3"));
					builder.Append("h");
				}
			}
			else
			{
				builder.Append(address.ToString("X3"));
				builder.Append("h");
			}

			if ((mode & 0x1000) != 0)  // +{X} ?
			{
				builder.Append("+{X}");
			}

			if ((mode & 0x2000) != 0)  // +{Y} ?
			{
				builder.Append("+{Y}");
			}
		}

		public override string AssemblyInstruction(string instruction, List<int> codes)
		{
			//	Assemble les codes d'une instruction et retourne une éventuelle erreur.
			codes.Clear();
			instruction = instruction.ToUpper().Trim();

			char[] seps = {' ', ',', ':'};
			string[] words = instruction.Split(seps, System.StringSplitOptions.RemoveEmptyEntries);

			int r1=-1, r2=-1, v1=-1, v2=-1, mh1=-1, ll1=-1, mh2=-1, ll2=-1;

			if (words.Length >= 2)  // un argument ?
			{
				TinyProcessor.GetCodeRegister(words[1], out r1);
				TinyProcessor.GetCodeValue(words[1], out v1);
				TinyProcessor.GetCodeAddress(words[1], out mh1, out ll1);
			}

			if (words.Length >= 3)  // deux arguments ?
			{
				TinyProcessor.GetCodeRegister(words[2], out r2);
				TinyProcessor.GetCodeValue(words[2], out v2);
				TinyProcessor.GetCodeAddress(words[2], out mh2, out ll2);

				if (instruction.IndexOf(":") != -1)  // instruction de bit ? (TODO: faire mieux)
				{
					Misc.Swap(ref r1, ref r2);
					Misc.Swap(ref v1, ref v2);
					Misc.Swap(ref mh1, ref mh2);
					Misc.Swap(ref ll1, ref ll2);
				}
			}

			switch (words[0])
			{
				case "NOP":
					if (words.Length == 1)
					{
						codes.Add((int) Instructions.Nop);
					}
					else
					{
						return "<b>L'instuction NOP n'a aucun argument.</b>";
					}
					break;

				case "RET":
					if (words.Length == 1)
					{
						codes.Add((int) Instructions.Ret);
					}
					else
					{
						return "<b>L'instuction RET n'a aucun argument.</b>";
					}
					break;

				case "HALT":
					if (words.Length == 1)
					{
						codes.Add((int) Instructions.Halt);
					}
					else
					{
						return "<b>L'instuction HALT n'a aucun argument.</b>";
					}
					break;

				case "SETC":
					if (words.Length == 1)
					{
						codes.Add((int) Instructions.SetC);
					}
					else
					{
						return "<b>L'instuction SETC n'a aucun argument.</b>";
					}
					break;

				case "CLRC":
					if (words.Length == 1)
					{
						codes.Add((int) Instructions.ClrC);
					}
					else
					{
						return "<b>L'instuction CLRC n'a aucun argument.</b>";
					}
					break;

				case "TABLE":
					if (words.Length == 1)
					{
						codes.Add((int) Instructions.Table);
					}
					else
					{
						return "<b>L'instuction TABLE n'a aucun argument.</b>";
					}
					break;

				case "CALL":
					if (words.Length == 2 && mh1 != -1 && ll1 != -1)
					{
						codes.Add((int) Instructions.Call);
						codes.Add(mh1);
						codes.Add(ll1);
					}
					else
					{
						return TinyProcessor.GetCodeError("CALL doit être suivi d'une adresse.", "CALL", "a");
					}
					break;

				case "PUSH":
					if (words.Length == 2 && r1 != -1)
					{
						codes.Add((int) Instructions.PushR | r1);
					}
					else
					{
						return TinyProcessor.GetCodeError("PUSH doit être suivi d'un nom de registre.", "PUSH", "r");
					}
					break;

				case "POP":
					if (words.Length == 2 && r1 != -1)
					{
						codes.Add((int) Instructions.PopR | r1);
					}
					else
					{
						return TinyProcessor.GetCodeError("POP doit être suivi d'un nom de registre.", "POP", "r");
					}
					break;

				case "JUMP":
					if (words.Length == 2 && mh1 != -1 && ll1 != -1)
					{
						codes.Add((int) Instructions.Jump);
						codes.Add(mh1);
						codes.Add(ll1);
					}
					else if (words.Length == 3 && mh2 != -1 && ll2 != -1)
					{
						switch (words[1])
						{
							case "EQ":
								codes.Add((int) Instructions.JumpEQ);
								break;

							case "NE":
								codes.Add((int) Instructions.JumpNE);
								break;

							case "LO":
								codes.Add((int) Instructions.JumpLO);
								break;

							case "LS":
								codes.Add((int) Instructions.JumpLS);
								break;

							case "HI":
								codes.Add((int) Instructions.JumpHI);
								break;

							case "HS":
								codes.Add((int) Instructions.JumpHS);
								break;

							case "CC":
								codes.Add((int) Instructions.JumpCC);
								break;

							case "CS":
								codes.Add((int) Instructions.JumpCS);
								break;

							case "NC":
								codes.Add((int) Instructions.JumpNC);
								break;

							case "NS":
								codes.Add((int) Instructions.JumpNS);
								break;

							default:
								return "<b>La condition est fausse.</b><br/><br/>Les conditions possibles sont :<br/><list type=\"fix\"/>EQ, NE<br/><list type=\"fix\"/>LO, LS, HI, HS<br/><list type=\"fix\"/>CC, CS, NC, NS<br/> ";
						}

						codes.Add(mh2);
						codes.Add(ll2);
					}
					else
					{
						return TinyProcessor.GetCodeError("JUMP doit être suivi d'une adresse.", "JUMP", "a");
					}
					break;

				case "CLR":
					if (words.Length == 2 && r1 != -1)
					{
						codes.Add((int) Instructions.ClrR | r1);
					}
					else if (words.Length == 2 && mh1 != -1 && ll1 != -1)
					{
						codes.Add((int) Instructions.ClrA);
						codes.Add(mh1);
						codes.Add(ll1);
					}
					else
					{
						return TinyProcessor.GetCodeError("CLR doit être suivi d'un nom de registre ou d'une adresse.", "CLR", "r,a");
					}
					break;

				case "NOT":
					if (words.Length == 2 && r1 != -1)
					{
						codes.Add((int) Instructions.NotR | r1);
					}
					else if (words.Length == 2 && mh1 != -1 && ll1 != -1)
					{
						codes.Add((int) Instructions.NotA);
						codes.Add(mh1);
						codes.Add(ll1);
					}
					else
					{
						return TinyProcessor.GetCodeError("NOT doit être suivi d'un nom de registre ou d'une adresse.", "NOT", "r,a");
					}
					break;

				case "INC":
					if (words.Length == 2 && r1 != -1)
					{
						codes.Add((int) Instructions.IncR | r1);
					}
					else if (words.Length == 2 && mh1 != -1 && ll1 != -1)
					{
						codes.Add((int) Instructions.IncA);
						codes.Add(mh1);
						codes.Add(ll1);
					}
					else
					{
						return TinyProcessor.GetCodeError("INC doit être suivi d'un nom de registre ou d'une adresse.", "INC", "r,a");
					}
					break;

				case "DEC":
					if (words.Length == 2 && r1 != -1)
					{
						codes.Add((int) Instructions.DecR | r1);
					}
					else if (words.Length == 2 && mh1 != -1 && ll1 != -1)
					{
						codes.Add((int) Instructions.DecA);
						codes.Add(mh1);
						codes.Add(ll1);
					}
					else
					{
						return TinyProcessor.GetCodeError("DEC doit être suivi d'un nom de registre ou d'une adresse.", "DEC", "r,a");
					}
					break;

				case "RL":
					if (words.Length == 2 && r1 != -1)
					{
						codes.Add((int) Instructions.RlR | r1);
					}
					else if (words.Length == 2 && mh1 != -1 && ll1 != -1)
					{
						codes.Add((int) Instructions.RlA);
						codes.Add(mh1);
						codes.Add(ll1);
					}
					else
					{
						return TinyProcessor.GetCodeError("RL doit être suivi d'un nom de registre ou d'une adresse.", "RL", "r,a");
					}
					break;

				case "RR":
					if (words.Length == 2 && r1 != -1)
					{
						codes.Add((int) Instructions.RrR | r1);
					}
					else if (words.Length == 2 && mh1 != -1 && ll1 != -1)
					{
						codes.Add((int) Instructions.RrA);
						codes.Add(mh1);
						codes.Add(ll1);
					}
					else
					{
						return TinyProcessor.GetCodeError("RR doit être suivi d'un nom de registre ou d'une adresse.", "RR", "r,a");
					}
					break;

				case "RLC":
					if (words.Length == 2 && r1 != -1)
					{
						codes.Add((int) Instructions.RlcR | r1);
					}
					else if (words.Length == 2 && mh1 != -1 && ll1 != -1)
					{
						codes.Add((int) Instructions.RlcA);
						codes.Add(mh1);
						codes.Add(ll1);
					}
					else
					{
						return TinyProcessor.GetCodeError("RLC doit être suivi d'un nom de registre ou d'une adresse.", "RLC", "r,a");
					}
					break;

				case "RRC":
					if (words.Length == 2 && r1 != -1)
					{
						codes.Add((int) Instructions.RrcR | r1);
					}
					else if (words.Length == 2 && mh1 != -1 && ll1 != -1)
					{
						codes.Add((int) Instructions.RrcA);
						codes.Add(mh1);
						codes.Add(ll1);
					}
					else
					{
						return TinyProcessor.GetCodeError("RRC doit être suivi d'un nom de registre ou d'une adresse.", "RRC", "r,a");
					}
					break;

				case "SWAP":
					if (words.Length == 2 && r1 != -1 && r1 < 2)
					{
						codes.Add((int) Instructions.SwapA | r1);
					}
					else
					{
						return TinyProcessor.GetCodeError("SWAP doit être suivi d'un nom de registre (A ou B).", "SWAP", "s");
					}
					break;

				case "MOVE":
					if (words.Length == 3 && r1 != -1 && r2 != -1)
					{
						codes.Add((int) Instructions.MoveRR | (r1<<2) | r2);
					}
					else if (words.Length == 3 && v1 != -1 && r2 != -1)
					{
						codes.Add((int) Instructions.MoveVR | r2);
						codes.Add(v1);
					}
					else if (words.Length == 3 && mh1 != -1 && ll1 != -1 && r2 != -1)
					{
						codes.Add((int) Instructions.MoveAR | r2);
						codes.Add(mh1);
						codes.Add(ll1);
					}
					else if (words.Length == 3 && r1 != -1 && mh2 != -1 && ll2 != -1)
					{
						codes.Add((int) Instructions.MoveRA | r1);
						codes.Add(mh2);
						codes.Add(ll2);
					}
					else if (words.Length == 3 && v1 != -1 && mh2 != -1 && ll2 != -1)
					{
						codes.Add((int) Instructions.MoveVA);
						codes.Add(v1);
						codes.Add(mh2);
						codes.Add(ll2);
					}
					else
					{
						return TinyProcessor.GetCodeError("Les arguments de l'instruction MOVE sont incorrects.", "MOVE", "rr,vr,ar,ra,va");
					}
					break;

				case "ADD":
					if (words.Length == 3 && r1 != -1 && r2 != -1)
					{
						codes.Add((int) Instructions.AddRR | (r1<<2) | r2);
					}
					else if (words.Length == 3 && v1 != -1 && r2 != -1)
					{
						codes.Add((int) Instructions.AddVR | r2);
						codes.Add(v1);
					}
					else if (words.Length == 3 && mh1 != -1 && ll1 != -1 && r2 != -1)
					{
						codes.Add((int) Instructions.AddAR | r2);
						codes.Add(mh1);
						codes.Add(ll1);
					}
					else if (words.Length == 3 && r1 != -1 && mh2 != -1 && ll2 != -1)
					{
						codes.Add((int) Instructions.AddRA | r1);
						codes.Add(mh2);
						codes.Add(ll2);
					}
					else if (words.Length == 3 && v1 != -1 && mh2 != -1 && ll2 != -1)
					{
						codes.Add((int) Instructions.AddVA);
						codes.Add(v1);
						codes.Add(mh2);
						codes.Add(ll2);
					}
					else if (words.Length == 3 && v1 != -1 && words[2] == "SP")
					{
						codes.Add((int) Instructions.AddVSP);
						codes.Add(v1);
					}
					else
					{
						return TinyProcessor.GetCodeError("Les arguments de l'instruction ADD sont incorrects.", "ADD", "rr,vr,ar,ra,va");
					}
					break;

				case "SUB":
					if (words.Length == 3 && r1 != -1 && r2 != -1)
					{
						codes.Add((int) Instructions.SubRR | (r1<<2) | r2);
					}
					else if (words.Length == 3 && v1 != -1 && r2 != -1)
					{
						codes.Add((int) Instructions.SubVR | r2);
						codes.Add(v1);
					}
					else if (words.Length == 3 && mh1 != -1 && ll1 != -1 && r2 != -1)
					{
						codes.Add((int) Instructions.SubAR | r2);
						codes.Add(mh1);
						codes.Add(ll1);
					}
					else if (words.Length == 3 && r1 != -1 && mh2 != -1 && ll2 != -1)
					{
						codes.Add((int) Instructions.SubRA | r1);
						codes.Add(mh2);
						codes.Add(ll2);
					}
					else if (words.Length == 3 && v1 != -1 && mh2 != -1 && ll2 != -1)
					{
						codes.Add((int) Instructions.SubVA);
						codes.Add(v1);
						codes.Add(mh2);
						codes.Add(ll2);
					}
					else if (words.Length == 3 && v1 != -1 && words[2] == "SP")
					{
						codes.Add((int) Instructions.SubVSP);
						codes.Add(v1);
					}
					else
					{
						return TinyProcessor.GetCodeError("Les arguments de l'instruction SUB sont incorrects.", "SUB", "rr,vr,ar,ra,va");
					}
					break;

				case "COMP":
					if (words.Length == 3 && r1 != -1 && r2 != -1)
					{
						codes.Add((int) Instructions.CompRR | (r1<<2) | r2);
					}
					else if (words.Length == 3 && v1 != -1 && r2 != -1)
					{
						codes.Add((int) Instructions.CompVR | r2);
						codes.Add(v1);
					}
					else if (words.Length == 3 && mh1 != -1 && ll1 != -1 && r2 != -1)
					{
						codes.Add((int) Instructions.CompAR | r2);
						codes.Add(mh1);
						codes.Add(ll1);
					}
					else if (words.Length == 3 && v1 != -1 && mh2 != -1 && ll2 != -1)
					{
						codes.Add((int) Instructions.CompVA);
						codes.Add(v1);
						codes.Add(mh2);
						codes.Add(ll2);
					}
					else
					{
						return TinyProcessor.GetCodeError("Les arguments de l'instruction COMP sont incorrects.", "COMP", "rr,vr,ar,va");
					}
					break;

				case "AND":
					if (words.Length == 3 && r1 != -1 && r1 < 2 && r2 != -1 && r2 < 2 && r1 != r2)
					{
						codes.Add((int) Instructions.AndSS | r1);
					}
					else if (words.Length == 3 && v1 != -1 && r2 != -1)
					{
						codes.Add((int) Instructions.AndVR | r2);
						codes.Add(v1);
					}
					else if (words.Length == 3 && mh1 != -1 && ll1 != -1 && r2 != -1 && r2 < 2)
					{
						codes.Add((int) Instructions.AndAS | r2);
						codes.Add(mh1);
						codes.Add(ll1);
					}
					else if (words.Length == 3 && r1 != -1 && r1 < 2 && mh2 != -1 && ll2 != -1)
					{
						codes.Add((int) Instructions.AndSA | r1);
						codes.Add(mh2);
						codes.Add(ll2);
					}
					else
					{
						return TinyProcessor.GetCodeError("Les arguments de l'instruction AND sont incorrects.", "AND", "ss,vr,as,sa");
					}
					break;

				case "OR":
					if (words.Length == 3 && r1 != -1 && r1 < 2 && r2 != -1 && r2 < 2 && r1 != r2)
					{
						codes.Add((int) Instructions.OrSS | r1);
					}
					else if (words.Length == 3 && v1 != -1 && r2 != -1)
					{
						codes.Add((int) Instructions.OrVR | r2);
						codes.Add(v1);
					}
					else if (words.Length == 3 && mh1 != -1 && ll1 != -1 && r2 != -1 && r2 < 2)
					{
						codes.Add((int) Instructions.OrAS | r2);
						codes.Add(mh1);
						codes.Add(ll1);
					}
					else if (words.Length == 3 && r1 != -1 && r1 < 2 && mh2 != -1 && ll2 != -1)
					{
						codes.Add((int) Instructions.OrSA | r1);
						codes.Add(mh2);
						codes.Add(ll2);
					}
					else
					{
						return TinyProcessor.GetCodeError("Les arguments de l'instruction OR sont incorrects.", "OR", "ss,vr,as,sa");
					}
					break;

				case "XOR":
					if (words.Length == 3 && r1 != -1 && r1 < 2 && r2 != -1 && r2 < 2 && r1 != r2)
					{
						codes.Add((int) Instructions.XorSS | r1);
					}
					else if (words.Length == 3 && v1 != -1 && r2 != -1)
					{
						codes.Add((int) Instructions.XorVR | r2);
						codes.Add(v1);
					}
					else if (words.Length == 3 && mh1 != -1 && ll1 != -1 && r2 != -1 && r2 < 2)
					{
						codes.Add((int) Instructions.XorAS | r2);
						codes.Add(mh1);
						codes.Add(ll1);
					}
					else if (words.Length == 3 && r1 != -1 && r1 < 2 && mh2 != -1 && ll2 != -1)
					{
						codes.Add((int) Instructions.XorSA | r1);
						codes.Add(mh2);
						codes.Add(ll2);
					}
					else
					{
						return TinyProcessor.GetCodeError("Les arguments de l'instruction XOR sont incorrects.", "XOR", "ss,vr,as,sa");
					}
					break;

				case "TEST":
					if (words.Length == 3 && r1 != -1 && r1 < 2 && r2 != -1 && r2 < 2 && r1 != r2)
					{
						codes.Add((int) Instructions.TestSS | r1);
					}
					else if (words.Length == 3 && v1 != -1 && r2 != -1 && r2 < 2)
					{
						codes.Add((int) Instructions.TestVS | r2);
						codes.Add(v1);
					}
					else if (words.Length == 3 && r1 != -1 && r1 < 2 && mh2 != -1 && ll2 != -1)
					{
						codes.Add((int) Instructions.TestSA | r1);
						codes.Add(mh2);
						codes.Add(ll2);
					}
					else if (words.Length == 3 && v1 != -1 && mh2 != -1 && ll2 != -1)
					{
						codes.Add((int) Instructions.TestVA);
						codes.Add(v1);
						codes.Add(mh2);
						codes.Add(ll2);
					}
					else
					{
						return TinyProcessor.GetCodeError("Les arguments de l'instruction TEST sont incorrects.", "TEST", "tss,tvs,tsa,tva");
					}
					break;

				case "TSET":
					if (words.Length == 3 && r1 != -1 && r1 < 2 && r2 != -1 && r2 < 2 && r1 != r2)
					{
						codes.Add((int) Instructions.TSetSS | r1);
					}
					else if (words.Length == 3 && v1 != -1 && r2 != -1 && r2 < 2)
					{
						codes.Add((int) Instructions.TSetVS | r2);
						codes.Add(v1);
					}
					else if (words.Length == 3 && r1 != -1 && r1 < 2 && mh2 != -1 && ll2 != -1)
					{
						codes.Add((int) Instructions.TSetSA | r1);
						codes.Add(mh2);
						codes.Add(ll2);
					}
					else if (words.Length == 3 && v1 != -1 && mh2 != -1 && ll2 != -1)
					{
						codes.Add((int) Instructions.TSetVA);
						codes.Add(v1);
						codes.Add(mh2);
						codes.Add(ll2);
					}
					else
					{
						return TinyProcessor.GetCodeError("Les arguments de l'instruction TSET sont incorrects.", "TSET", "tss,tvs,tsa,tva");
					}
					break;

				case "TCLR":
					if (words.Length == 3 && r1 != -1 && r1 < 2 && r2 != -1 && r2 < 2 && r1 != r2)
					{
						codes.Add((int) Instructions.TClrSS | r1);
					}
					else if (words.Length == 3 && v1 != -1 && r2 != -1 && r2 < 2)
					{
						codes.Add((int) Instructions.TClrVS | r2);
						codes.Add(v1);
					}
					else if (words.Length == 3 && r1 != -1 && r1 < 2 && mh2 != -1 && ll2 != -1)
					{
						codes.Add((int) Instructions.TClrSA | r1);
						codes.Add(mh2);
						codes.Add(ll2);
					}
					else if (words.Length == 3 && v1 != -1 && mh2 != -1 && ll2 != -1)
					{
						codes.Add((int) Instructions.TClrVA);
						codes.Add(v1);
						codes.Add(mh2);
						codes.Add(ll2);
					}
					else
					{
						return TinyProcessor.GetCodeError("Les arguments de l'instruction TCLR sont incorrects.", "TCLR", "tss,tvs,tsa,tva");
					}
					break;

				case "TNOT":
					if (words.Length == 3 && r1 != -1 && r1 < 2 && r2 != -1 && r2 < 2 && r1 != r2)
					{
						codes.Add((int) Instructions.TNotSS | r1);
					}
					else if (words.Length == 3 && v1 != -1 && r2 != -1 && r2 < 2)
					{
						codes.Add((int) Instructions.TNotVS | r2);
						codes.Add(v1);
					}
					else if (words.Length == 3 && r1 != -1 && r1 < 2 && mh2 != -1 && ll2 != -1)
					{
						codes.Add((int) Instructions.TNotSA | r1);
						codes.Add(mh2);
						codes.Add(ll2);
					}
					else if (words.Length == 3 && v1 != -1 && mh2 != -1 && ll2 != -1)
					{
						codes.Add((int) Instructions.TNotVA);
						codes.Add(v1);
						codes.Add(mh2);
						codes.Add(ll2);
					}
					else
					{
						return TinyProcessor.GetCodeError("Les arguments de l'instruction TNOT sont incorrects.", "TNOT", "tss,tvs,tsa,tva");
					}
					break;

				case "EX":
					if (words.Length == 3 && r1 >= 0 && r1 <= 1 && r2 >= 0 && r2 <= 1 && r1 != r2)
					{
						codes.Add((int) Instructions.ExAB);
					}
					else if (words.Length == 3 && r1 >= 2 && r1 <= 3 && r2 >= 2 && r2 <= 3 && r1 != r2)
					{
						codes.Add((int) Instructions.ExXY);
					}
					else
					{
						return TinyProcessor.GetCodeError("Les arguments de l'instruction EX sont incorrects.", "EX", "ex");
					}
					break;

				default:
					return "<b>Instruction inconnue.</b><br/><br/>Les instructions connues sont :<br/><list type=\"fix\"/>JUMP, CALL, RET, PUSH, POP<br/><list type=\"fix\"/>MOVE, COMP, ADD, SUB, AND, OR, XOR<br/><list type=\"fix\"/>CLR, NOT, INC, DEC, RL, RR, RLC, RRC<br/><list type=\"fix\"/>TEST, TSET, TCLR, TNOT<br/><list type=\"fix\"/>NOP, CLRC, SETC, EX, SWAP, HALT<br/> ";
			}

			return null;  // ok
		}

		protected static void GetCodeRegister(string word, out int n)
		{
			//	Analyse un registre.
			n = -1;

			switch (word)
			{
				case "A":
					n = 0;
					break;

				case "B":
					n = 1;
					break;

				case "X":
					n = 2;
					break;

				case "Y":
					n = 3;
					break;
			}
		}

		protected static void GetCodeValue(string word, out int value)
		{
			//	Analyse une valeur immédiate.
			if (string.IsNullOrEmpty(word) || word[0] != '#')
			{
				value = -1;
				return;
			}

			word = word.Substring(1);  // enlève le '#' au début

			if (word.EndsWith("H"))
			{
				word = word.Substring(0, word.Length-1);  // enlève le 'H' à la fin
				value =  Misc.ParseHexa(word, -1, -1);
				return;
			}
			else if (word.EndsWith("D"))
			{
				word = word.Substring(0, word.Length-1);  // enlève le 'D' à la fin
				if (!int.TryParse(word, out value))
				{
					value = -1;
				}
				return;
			}
			else
			{
				if (!int.TryParse(word, out value))
				{
					value = -1;
				}
				return;
			}
		}

		protected static void GetCodeAddress(string word, out int mh, out int ll)
		{
			//	Analyse une adresse.
			int mode = 0;
			int address = -1;
			int rel = 0;

			mh = -1;
			ll = -1;

			if (word.StartsWith("{PC}") || word.StartsWith("{SP}"))
			{
				if (word.StartsWith("{PC}"))
				{
					mode |= 8;
				}
				else
				{
					mode |= 4;
				}

				word = word.Substring(4);  // enlève {xx}

				if (word.Length == 0)
				{
					return;
				}

				if (word[0] == '+')
				{
					word = word.Substring(1);  // enlève +
					rel = 1;
				}
				else if (word[0] == '-')
				{
					word = word.Substring(1);  // enlève -
					rel = -1;
				}
				else
				{
					return;
				}
			}

			bool action;
			do
			{
				action = false;

				if (word.EndsWith("+{X}"))
				{
					mode |= 1;
					word = word.Substring(0, word.Length-4);  // enlève +{X}
					action = true;
				}
				
				if (word.EndsWith("+{Y}"))
				{
					mode |= 2;
					word = word.Substring(0, word.Length-4);  // enlève +{Y}
					action = true;
				}
			}
			while (action);

			if (word.EndsWith("H"))
			{
				word = word.Substring(0, word.Length-1);  // enlève H
				address = Misc.ParseHexa(word, -1, -1);
			}
			else if (word.EndsWith("D"))
			{
				word = word.Substring(0, word.Length-1);  // enlève D
				if (!int.TryParse(word, out address))
				{
					address = -1;
				}
			}
			else
			{
				if (!int.TryParse(word, out address))
				{
					address = -1;
				}
			}

			if (address == -1)
			{
				return;
			}

			if (rel == -1)  // adresse relative négative ?
			{
				address = 0x1000000-address;
			}

			mh = ((mode << 4) & 0xF0) | ((address >> 8) & 0x0F);
			ll = (address & 0xFF);
		}

		protected static string GetCodeError(string message, string instruction, string types)
		{
			//	Génère un message d'erreur clair avec des exemples.
			System.Text.StringBuilder builder = new System.Text.StringBuilder();

			builder.Append("<b>");
			builder.Append(message);  // message principal en gras
			builder.Append("</b>");

			if (!string.IsNullOrEmpty(types))
			{
				string[] list = types.Split(',');

				if (list.Length == 1)
				{
					builder.Append("<br/><br/>L'instruction suivante est possible :<br/>");
				}
				else
				{
					builder.Append("<br/><br/>Les instructions suivantes sont possibles :<br/>");
				}

				bool r = false;  // r
				bool s = false;  // r'
				bool v = false;  // #val
				bool a = false;  // ADDR

				foreach(string type in list)
				{
					switch (type)
					{
						case "r":
							builder.Append("<list type=\"fix\"/>");
							builder.Append(instruction);
							builder.Append(" <i>r</i><br/>");
							r = true;
							break;

						case "s":
							builder.Append("<list type=\"fix\"/>");
							builder.Append(instruction);
							builder.Append(" <i>r'</i><br/>");
							s = true;
							break;

						case "a":
							builder.Append("<list type=\"fix\"/>");
							builder.Append(instruction);
							builder.Append(" <i>ADDR</i><br/>");
							a = true;
							break;

						case "rr":
							builder.Append("<list type=\"fix\"/>");
							builder.Append(instruction);
							builder.Append(" <i>r</i>, <i>r</i><br/>");
							r = true;
							break;

						case "ss":
							builder.Append("<list type=\"fix\"/>");
							builder.Append(instruction);
							builder.Append(" <i>r'</i>, <i>r'</i><br/>");
							s = true;
							break;

						case "vr":
							builder.Append("<list type=\"fix\"/>");
							builder.Append(instruction);
							builder.Append(" <i>#val</i>, <i>r</i><br/>");
							r = true;
							v = true;
							break;

						case "ar":
							builder.Append("<list type=\"fix\"/>");
							builder.Append(instruction);
							builder.Append(" <i>ADDR</i>, <i>r</i><br/>");
							a = true;
							r = true;
							break;

						case "ra":
							builder.Append("<list type=\"fix\"/>");
							builder.Append(instruction);
							builder.Append(" <i>r</i>, <i>ADDR</i><br/>");
							r = true;
							a = true;
							break;

						case "as":
							builder.Append("<list type=\"fix\"/>");
							builder.Append(instruction);
							builder.Append(" <i>ADDR</i>, <i>r'</i><br/>");
							a = true;
							s = true;
							break;

						case "sa":
							builder.Append("<list type=\"fix\"/>");
							builder.Append(instruction);
							builder.Append(" <i>r'</i>, <i>ADDR</i><br/>");
							s = true;
							a = true;
							break;

						case "va":
							builder.Append("<list type=\"fix\"/>");
							builder.Append(instruction);
							builder.Append(" <i>#val</i>, <i>ADDR</i><br/>");
							v = true;
							a = true;
							break;

						case "tss":
							builder.Append("<list type=\"fix\"/>");
							builder.Append(instruction);
							builder.Append(" <i>r'</i>: <i>r'</i><br/>");
							s = true;
							break;

						case "tvs":
							builder.Append("<list type=\"fix\"/>");
							builder.Append(instruction);
							builder.Append(" <i>r'</i>: <i>#val</i><br/>");
							v = true;
							s = true;
							break;

						case "tsa":
							builder.Append("<list type=\"fix\"/>");
							builder.Append(instruction);
							builder.Append(" <i>ADDR</i>: <i>r'</i><br/>");
							s = true;
							a = true;
							break;

						case "tva":
							builder.Append("<list type=\"fix\"/>");
							builder.Append(instruction);
							builder.Append(" <i>ADDR</i>: <i>#val</i><br/>");
							v = true;
							a = true;
							break;

						case "ex":
							builder.Append("<list type=\"fix\"/>");
							builder.Append(instruction);
							builder.Append(" <i>A</i>, <i>B</i> ou <i>X</i>, <i>Y</i><br/>");
							break;
					}
				}

				if (r || s || v || a)
				{
					builder.Append("<br/>");
				}

				if (r)
				{
					builder.Append("<i>r</i> = registre A, B, X, Y<br/>");
				}

				if (s)
				{
					builder.Append("<i>r'</i> = registre A, B<br/>");
				}

				if (v)
				{
					builder.Append("<i>#val</i> = valeur immédiate #12h, #C0h, #99d<br/>");
				}

				if (a)
				{
					builder.Append("<i>ADDR</i> = adresse C00h, {PC}+DAh, {PC}-3h, {SP}+2h, C80h+{X}+{Y}<br/>");
				}
			}

			builder.Append(" <br/>");  // TODO: bug, devrait être inutile, mais la dernière ligne n'est pas visible sans cela !

			return builder.ToString();
		}
		#endregion


		#region Rom
		public override void RomInitialise(int address, int length)
		{
			//	Rempli la Rom.
			int start = address;
			System.Diagnostics.Debug.Assert(TinyProcessor.CharTable.Length <= 0x100);
			this.RomWrite(0xB00, TinyProcessor.CharTable);

			int indirect = address;
			address += 3*32;  // place pour 32 appels
			this.RomWrite(ref indirect, ref address, TinyProcessor.WaitKey);			// 0x00
			this.RomWrite(ref indirect, ref address, TinyProcessor.WaitSec);			// 0x03
			this.RomWrite(ref indirect, ref address, TinyProcessor.DisplayBinaryDigit);	// 0x06
			this.RomWrite(ref indirect, ref address, TinyProcessor.DisplayHexaDigit);	// 0x09
			this.RomWrite(ref indirect, ref address, TinyProcessor.DisplayHexaByte);	// 0x0C
			this.RomWrite(ref indirect, ref address, TinyProcessor.DisplayDecimal);		// 0x0F
			this.RomWrite(ref indirect, ref address, TinyProcessor.SetPixel);			// 0x12
			this.RomWrite(ref indirect, ref address, TinyProcessor.ClrPixel);			// 0x15
			this.RomWrite(ref indirect, ref address, TinyProcessor.NotPixel);			// 0x18
			this.RomWrite(ref indirect, ref address, TinyProcessor.ClearScreen);		// 0x1B
			this.RomWrite(ref indirect, ref address, TinyProcessor.DrawChar);			// 0x1E
			System.Diagnostics.Debug.Assert(address < 0xB00);
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

		protected void RomWrite(int address, byte[] code)
		{
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

			(byte) Instructions.AndVR+1, 0x03,			// AND #03,B
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

			(byte) Instructions.AndVR+0, 0x0F,			// AND #0F,A
			(byte) Instructions.MoveRR+0x2,				// MOVE A,X
			(byte) Instructions.MoveAR+0, 0x90, 0x0B,	// MOVE R8^TABLE+{X},A

			(byte) Instructions.AndVR+1, 0x03,			// AND #03,B
			(byte) Instructions.MoveRR+0x6,				// MOVE B,X
			(byte) Instructions.MoveRA+0, 0x1C, 0x00,	// MOVE A,C00+{X}

			(byte) Instructions.PopR+2,					// POP X
			(byte) Instructions.PopR+1,					// POP B
			(byte) Instructions.PopR+0,					// POP A
			(byte) Instructions.Ret,					// RET
														// TABLE:
			(byte) Instructions.Table,
			0x3F, 0x03, 0x6D, 0x67, 0x53, 0x76, 0x7E, 0x23, 0x7F, 0x77, 0x7B, 0x5E, 0x3C, 0x4F, 0x7C, 0x78,
			(byte) Instructions.Table,
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
			(byte) Instructions.Call, 0x08, 0x0C,		// CALL DisplayHexaDigit

			(byte) Instructions.SwapA,					// SWAP A
			(byte) Instructions.DecR+1,					// DEC B
			(byte) Instructions.Call, 0x08, 0x0C,		// CALL DisplayHexaDigit

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
		//	in	X coordonnée X 0..31
		//		Y coordonnée Y 0..23
		//	out	-
		//	mod	F
		protected static byte[] SetPixel =
		{
			(byte) Instructions.PushR+1,				// PUSH B
			(byte) Instructions.PushR+2,				// PUSH X
			(byte) Instructions.PushR+3,				// PUSH Y

			(byte) Instructions.AndVR+3, 0x1F,			// AND #1F,Y
			(byte) Instructions.RlR+3,					// RL Y
			(byte) Instructions.RlR+3,					// RL Y

			(byte) Instructions.MoveRR+0x9,				// MOVE X,B
			(byte) Instructions.XorVR+1, 0x07,			// XOR #07,B

			(byte) Instructions.RrR+2,					// RR X
			(byte) Instructions.RrR+2,					// RR X
			(byte) Instructions.RrR+2,					// RR X
			(byte) Instructions.AndVR+2, 0x03,			// AND #03,X

			(byte) Instructions.TSetSA+1, 0x3C, 0x80,	// TSET C80+{X}+{Y},B

			(byte) Instructions.PopR+3,					// POP Y
			(byte) Instructions.PopR+2,					// POP X
			(byte) Instructions.PopR+1,					// POP B
			(byte) Instructions.Ret,					// RET
		};

		//	Eteint un pixel dans l'écran bitmap.
		//	in	X coordonnée X 0..31
		//		Y coordonnée Y 0..23
		//	out	-
		//	mod	F
		protected static byte[] ClrPixel =
		{
			(byte) Instructions.PushR+1,				// PUSH B
			(byte) Instructions.PushR+2,				// PUSH X
			(byte) Instructions.PushR+3,				// PUSH Y

			(byte) Instructions.AndVR+3, 0x1F,			// AND #1F,Y
			(byte) Instructions.RlR+3,					// RL Y
			(byte) Instructions.RlR+3,					// RL Y

			(byte) Instructions.MoveRR+0x9,				// MOVE X,B
			(byte) Instructions.XorVR+1, 0x07,			// XOR #07,B

			(byte) Instructions.RrR+2,					// RR X
			(byte) Instructions.RrR+2,					// RR X
			(byte) Instructions.RrR+2,					// RR X
			(byte) Instructions.AndVR+2, 0x03,			// AND #03,X

			(byte) Instructions.TClrSA+1, 0x3C, 0x80,	// TCLR C80+{X}+{Y},B

			(byte) Instructions.PopR+3,					// POP Y
			(byte) Instructions.PopR+2,					// POP X
			(byte) Instructions.PopR+1,					// POP B
			(byte) Instructions.Ret,					// RET
		};

		//	Inverse un pixel dans l'écran bitmap.
		//	in	X coordonnée X 0..31
		//		Y coordonnée Y 0..23
		//	out	-
		//	mod	F
		protected static byte[] NotPixel =
		{
			(byte) Instructions.PushR+1,				// PUSH B
			(byte) Instructions.PushR+2,				// PUSH X
			(byte) Instructions.PushR+3,				// PUSH Y

			(byte) Instructions.AndVR+3, 0x1F,			// AND #1F,Y
			(byte) Instructions.RlR+3,					// RL Y
			(byte) Instructions.RlR+3,					// RL Y

			(byte) Instructions.MoveRR+0x9,				// MOVE X,B
			(byte) Instructions.XorVR+1, 0x07,			// XOR #07,B

			(byte) Instructions.RrR+2,					// RR X
			(byte) Instructions.RrR+2,					// RR X
			(byte) Instructions.RrR+2,					// RR X
			(byte) Instructions.AndVR+2, 0x03,			// AND #03,X

			(byte) Instructions.TNotSA+1, 0x3C, 0x80,	// TNOT C80+{X}+{Y},B

			(byte) Instructions.PopR+3,					// POP Y
			(byte) Instructions.PopR+2,					// POP X
			(byte) Instructions.PopR+1,					// POP B
			(byte) Instructions.Ret,					// RET
		};

		//	Attend quelques secondes.
		//	in	A nombre de secondes à attendre (à 1000 IPS)
		//	out	-
		//	mod	F
		protected static byte[] WaitSec =
		{
			(byte) Instructions.PushR+0,				// PUSH A
			(byte) Instructions.PushR+1,				// PUSH B
														// LOOP:
			(byte) Instructions.ClrR+1,					// CLR B
			(byte) Instructions.DecR+1,					// DEC B
			(byte) Instructions.JumpNE, 0x8F, 0xFC,		// JUMP,NE -4

			(byte) Instructions.DecR+0,					// DEC A
			(byte) Instructions.JumpNE, 0x8F, 0xF7,		// JUMP,NE R8^LOOP

			(byte) Instructions.PopR+1,					// POP B
			(byte) Instructions.PopR+0,					// POP A
			(byte) Instructions.Ret,					// RET
		};

		//	Efface tout l'écran bitmap.
		//	in	-
		//	out	-
		//	mod	F
		protected static byte[] ClearScreen =
		{
			(byte) Instructions.PushR+0,				// PUSH A
			(byte) Instructions.PushR+2,				// PUSH X

			(byte) Instructions.MoveVR+0, 0x60,			// MOVE #60,A
			(byte) Instructions.ClrR+2,					// CLR X:
														// LOOP:
			(byte) Instructions.ClrA, 0x1C, 0x80,		// CLR C80+{X}
			(byte) Instructions.IncR+2,					// INC X
			(byte) Instructions.DecR+0,					// DEC A
			(byte) Instructions.JumpNE, 0x8F, 0xF8,		// JUMP,NE R8^LOOP

			(byte) Instructions.PopR+2,					// POP X
			(byte) Instructions.PopR+0,					// POP A
			(byte) Instructions.Ret,					// RET
		};

		//	Dessine un caractère dans l'écran bitmap.
		//	in	A caractère ascii
		//		X colonne 0..7
		//		Y ligne 0..3
		//	out	-
		//	mod	F
		protected static byte[] DrawChar =
		{
			(byte) Instructions.PushR+0,				// PUSH A
			(byte) Instructions.PushR+1,				// PUSH B
			(byte) Instructions.PushR+2,				// PUSH X
			(byte) Instructions.PushR+3,				// PUSH Y

			(byte) Instructions.CompVR+0, 0x20,			// COMP #20,A
			(byte) Instructions.JumpHS, 0x80, 2,		// JUMP,HS +2
			(byte) Instructions.MoveVR+0, 0x20,			// MOVE #20,A

			(byte) Instructions.CompVR+0, 0x60,			// COMP #60,A
			(byte) Instructions.JumpLS, 0x80, 2,		// JUMP,LS +2
			(byte) Instructions.MoveVR+0, 0x60,			// MOVE #60,A

			(byte) Instructions.SubVR+0, 0x20,			// SUB #20,A

			(byte) Instructions.SubVSP, 3,				// SUB #3,SP
			(byte) Instructions.MoveVR+1, 0xF0,			// MOVE #F0,B
			(byte) Instructions.MoveRA+1, 0x40, 0,		// MOVE B,{SP}+0
			(byte) Instructions.NotR+1,					// NOT B
			(byte) Instructions.MoveRA+1, 0x40, 1,		// MOVE B,{SP}+1
			(byte) Instructions.ClrA, 0x40, 2,			// CLR {SP}+2

			(byte) Instructions.ClrC,					// CLRC
			(byte) Instructions.RrcR+0,					// RRC A
			(byte) Instructions.JumpCC, 0x80, 3,		// JUMP,CC +3
			(byte) Instructions.NotA, 0x40, 0,			// NOT {SP}+0
			(byte) Instructions.MoveRR+0x1,				// MOVE A,B
			(byte) Instructions.RlR+0,					// RL A
			(byte) Instructions.RlR+0,					// RL A
			(byte) Instructions.AddRR+0x1,				// ADD A,B		// B = A*5

			(byte) Instructions.MoveRR+0xC,				// MOVE Y,A
			(byte) Instructions.AndVR+0, 0x03,			// AND #3,A
			(byte) Instructions.MoveRR+0x3,				// MOVE A,Y
			(byte) Instructions.AddRR+0xC,				// ADD Y,A
			(byte) Instructions.AddRR+0xC,				// ADD Y,A
			(byte) Instructions.RlR+0,					// RL A			// *6
			(byte) Instructions.RlR+0,					// RL A
			(byte) Instructions.RlR+0,					// RL A			// *4
			(byte) Instructions.MoveRR+0x3,				// MOVE A,Y

			(byte) Instructions.MoveRR+0x8,				// MOVE X,A
			(byte) Instructions.MoveRR+0x6,				// MOVE B,X

			(byte) Instructions.AndVR+0, 0x07,			// AND #7,A
			(byte) Instructions.ClrC,					// CLRC
			(byte) Instructions.RrcR+0,					// RRC A
			(byte) Instructions.JumpCC,	0x80, 3,		// JUMP,CC +3
			(byte) Instructions.NotA, 0x40, 1,			// NOT {SP}+1
			(byte) Instructions.AddRR+0x3,				// ADD A,Y

			(byte) Instructions.MoveAR, 0x40, 0,		// MOVE {SP}+0,A
			(byte) Instructions.OrAS+0, 0x40, 1,		// OR {SP}+1,A
			(byte) Instructions.CompVR+0, 0xFF,			// COMP #FF,A
			(byte) Instructions.JumpNE, 0x80, 3,		// JUMP,NE +3
			(byte) Instructions.NotA, 0x40, 2,			// NOT {SP}+2

			(byte) Instructions.MoveVR+1, 0x5,			// MOVE #5,B
														// LOOP:
			(byte) Instructions.MoveAR+0, 0x40, 1,		// MOVE {SP}+1,A
			(byte) Instructions.AndSA+0, 0x2C, 0x80,	// AND A,C80+{Y}
			(byte) Instructions.MoveAR+0, 0x1B, 0x01,	// MOVE CharTable+{X},A
			(byte) Instructions.AndAS+0, 0x40, 0,		// AND {SP}+0,A
			(byte) Instructions.TestVA, 0, 0x40, 2,		// TEST {SP}+2:#0
			(byte) Instructions.JumpNE, 0x80, 1,		// JUMP,NE +1
			(byte) Instructions.SwapA,					// SWAP A
			(byte) Instructions.OrSA+0, 0x2C, 0x80,		// OR A,C80+{Y}
			(byte) Instructions.IncR+2,					// INC X
			(byte) Instructions.AddVR+3, 0x04,			// ADD #4,Y
			(byte) Instructions.DecR+1,					// DEC B
			(byte) Instructions.JumpNE, 0x8F, 0xE2,		// JUMP,NE R8^LOOP

			(byte) Instructions.AddVSP, 3,				// ADD #3,SP

			(byte) Instructions.PopR+3,					// POP Y
			(byte) Instructions.PopR+2,					// POP X
			(byte) Instructions.PopR+1,					// POP B
			(byte) Instructions.PopR+0,					// POP A
			(byte) Instructions.Ret,					// RET
		};

		protected static byte[] CharTable =
		{
			(byte) Instructions.Table,
			0x04, 0x04, 0x04, 0x00, 0x04,	//  !
			0xA4, 0xAE, 0x04, 0x0E, 0x04,	// "#
			0x6A, 0xC2, 0x44, 0x68, 0xCA,	// $%
			0x44, 0xA4, 0x40, 0xA0, 0x50,	// &'
			0x44, 0x82, 0x82, 0x82, 0x44,	// ()
			0x40, 0xE4, 0x4E, 0xE4, 0x40,	// *+
			0x00, 0x00, 0x0E, 0x40, 0x80,	// ,-
			0x02, 0x02, 0x04, 0x08, 0x48,	// ./
			0x44, 0xAC, 0xA4, 0xA4, 0x4E,	// 01
			0x4C, 0xA2, 0x26, 0x42, 0xEC,	// 23
			0x2E, 0x68, 0xAC, 0xE2, 0x2C,	// 45
			0x4E, 0x82, 0xC2, 0xA4, 0x44,	// 67
			0x44, 0xAA, 0x46, 0xA2, 0x44,	// 89
			0x00, 0x44, 0x00, 0x04, 0x48,	// :;
			0x20, 0x4E, 0x80, 0x4E, 0x20,	// <=
			0x84, 0x4A, 0x22, 0x40, 0x84,	// >?
			0x44, 0xAA, 0xAE, 0x8A, 0x6A,	// @A
			0xC6, 0xA8, 0xC8, 0xA8, 0xC6,	// BC
			0xCE, 0xA8, 0xAC, 0xA8, 0xCE,	// DE
			0xE6, 0x88, 0xCA, 0x8A, 0x86,	// FG
			0xAE, 0xA4, 0xE4, 0xA4, 0xAE,	// HI
			0xEA, 0x2A, 0x2C, 0x2A, 0xCA,	// JK
			0x8A, 0x8E, 0x8A, 0x8A, 0xEA,	// LM
			0xA4, 0xEA, 0xEA, 0xEA, 0xA4,	// NO
			0xC4, 0xAA, 0xCA, 0x8E, 0x86,	// PQ
			0xC6, 0xA8, 0xC4, 0xA2, 0xAC,	// RS
			0xEA, 0x4A, 0x4A, 0x4A, 0x44,	// TU
			0xAA, 0xAA, 0xAE, 0x4E, 0x44,	// VW
			0xAA, 0xAA, 0x44, 0xA4, 0xA4,	// XY
			0xEC, 0x28, 0x48, 0x88, 0xEC,	// Z[
			0x86, 0x82, 0x42, 0x22, 0x26,	// \]
			0x40, 0xA0, 0x00, 0x00, 0x0E,	// ^_
			(byte) Instructions.Table,
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
					AbstractProcessor.HelpPutLine(builder, "[000]..[7FF]<tab/>RAM");
					AbstractProcessor.HelpPutLine(builder, "[800]..[BFF]<tab/>ROM");
					AbstractProcessor.HelpPutLine(builder, "[C00]..[C10]<tab/>Périphériques");
					AbstractProcessor.HelpPutLine(builder, "[C80]..[CDF]<tab/>Ecran bitmap");

					AbstractProcessor.HelpPutTitle(builder, "Affichage");
					AbstractProcessor.HelpPutLine(builder, "L'affichage est constitué de 4 afficheurs à 7 segments (plus un point décimal), numérotés de droite à gauche. On peut écrire une valeur pour mémoriser les digits à allumer, ou relire cette valeur.");
					AbstractProcessor.HelpPutLine(builder, "[C00]<tab/>Premier digit (celui de gauche).");
					AbstractProcessor.HelpPutLine(builder, "[C01]<tab/>Deuxième digit.");
					AbstractProcessor.HelpPutLine(builder, "[C02]<tab/>Troisième digit.");
					AbstractProcessor.HelpPutLine(builder, "[C03]<tab/>Quatrième digit (celui de droite).");
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
					AbstractProcessor.HelpPutLine(builder, "[C07]<tab/>Clavier.");
					AbstractProcessor.HelpPutLine(builder, "");
					AbstractProcessor.HelpPutLine(builder, "bits 0..2<tab/>Touches 0..7.");
					AbstractProcessor.HelpPutLine(builder, "bit 3<tab/>Touche Shift.");
					AbstractProcessor.HelpPutLine(builder, "bit 4<tab/>Touche Ctrl.");
					AbstractProcessor.HelpPutLine(builder, "bit 7<tab/>Prend la valeur 1 lorsqu'une touche 0..7 est pressée. Est automatiquement remis à zéro lorsque l'adresse [C07] est lue.");

					AbstractProcessor.HelpPutTitle(builder, "Ecran bitmap");
					AbstractProcessor.HelpPutLine(builder, "L'écran bitmap est un écran vidéo monochrome de 32 x 24 pixels. Chaque byte représente 8 pixels horizontaux, avec le bit 7 à gauche.");
					AbstractProcessor.HelpPutLine(builder, "");
					AbstractProcessor.HelpPutLine(builder, "[C80]..[C83]<tab/>1ère ligne de 32 pixels.");
					AbstractProcessor.HelpPutLine(builder, "[C84]..[C87]<tab/>2ème ligne de 32 pixels.");
					AbstractProcessor.HelpPutLine(builder, "...");
					AbstractProcessor.HelpPutLine(builder, "[CDC]..[CDF]<tab/>24ème ligne de 32 pixels.");
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
					AbstractProcessor.HelpPutLine(builder, "m=4             <tab/>{SP}+depl");
					AbstractProcessor.HelpPutLine(builder, "m=8             <tab/>{PC}+depl (± adresse relative)");
					AbstractProcessor.HelpPutLine(builder, "");
					AbstractProcessor.HelpPutLine(builder, "<b>Exemples</b>");
					AbstractProcessor.HelpPutLine(builder, "[54] [0C] [07]  <tab/>MOVE C07,A");
					AbstractProcessor.HelpPutLine(builder, "[58] [1C] [00]  <tab/>MOVE A,C00+{X}");
					AbstractProcessor.HelpPutLine(builder, "[10] [80] [10]  <tab/>JUMP {PC}+10 (saute 10 bytes)");
					AbstractProcessor.HelpPutLine(builder, "[10] [8F] [FD]  <tab/>JUMP {PC}-3 (boucle infinie)");
					AbstractProcessor.HelpPutLine(builder, "[54] [40] [02]  <tab/>MOVE {SP}+2,A");
					break;

				case "Op":
					AbstractProcessor.HelpPutTitle(builder, "Transferts");
					AbstractProcessor.HelpPutLine(builder, "[40+r]          <tab/>MOVE A,r      <tab/><tab/>(N, Z, C)");
					AbstractProcessor.HelpPutLine(builder, "[44+r]          <tab/>MOVE B,r      <tab/><tab/>(N, Z, C)");
					AbstractProcessor.HelpPutLine(builder, "[48+r]          <tab/>MOVE X,r      <tab/><tab/>(N, Z, C)");
					AbstractProcessor.HelpPutLine(builder, "[4C+r]          <tab/>MOVE Y,r      <tab/><tab/>(N, Z, C)");
					AbstractProcessor.HelpPutLine(builder, "[50+r] [vv]     <tab/>MOVE #val,r   <tab/><tab/>(N, Z, C)");
					AbstractProcessor.HelpPutLine(builder, "[54+r] [mh] [ll]<tab/>MOVE ADDR,r   <tab/><tab/>(N, Z, C)");
					AbstractProcessor.HelpPutLine(builder, "[58+r] [mh] [ll]<tab/>MOVE r,ADDR   <tab/><tab/>(N, Z, C)");
					AbstractProcessor.HelpPutLine(builder, "[DC] [vv] [mh] [ll] MOVE #val,ADDR  <tab/>(N, Z, C)");

					AbstractProcessor.HelpPutTitle(builder, "Additions");
					AbstractProcessor.HelpPutLine(builder, "[80+r]          <tab/>ADD A,r      <tab/><tab/>(N, Z, C)");
					AbstractProcessor.HelpPutLine(builder, "[84+r]          <tab/>ADD B,r      <tab/><tab/>(N, Z, C)");
					AbstractProcessor.HelpPutLine(builder, "[88+r]          <tab/>ADD X,r      <tab/><tab/>(N, Z, C)");
					AbstractProcessor.HelpPutLine(builder, "[8C+r]          <tab/>ADD Y,r      <tab/><tab/>(N, Z, C)");
					AbstractProcessor.HelpPutLine(builder, "[A0+r] [vv]     <tab/>ADD #val,r   <tab/><tab/>(N, Z, C)");
					AbstractProcessor.HelpPutLine(builder, "[B0+r] [mh] [ll]<tab/>ADD ADDR,r   <tab/><tab/>(N, Z, C)");
					AbstractProcessor.HelpPutLine(builder, "[B8+r] [mh] [ll]<tab/>ADD r,ADDR   <tab/><tab/>(N, Z, C)");
					AbstractProcessor.HelpPutLine(builder, "[DE] [vv] [mh] [ll] ADD #val,ADDR  <tab/>(N, Z, C)");

					AbstractProcessor.HelpPutTitle(builder, "Soustractions");
					AbstractProcessor.HelpPutLine(builder, "[90+r]          <tab/>SUB A,r      <tab/><tab/>(N, Z, C)");
					AbstractProcessor.HelpPutLine(builder, "[94+r]          <tab/>SUB B,r      <tab/><tab/>(N, Z, C)");
					AbstractProcessor.HelpPutLine(builder, "[98+r]          <tab/>SUB X,r      <tab/><tab/>(N, Z, C)");
					AbstractProcessor.HelpPutLine(builder, "[9C+r]          <tab/>SUB Y,r      <tab/><tab/>(N, Z, C)");
					AbstractProcessor.HelpPutLine(builder, "[A4+r] [vv]     <tab/>SUB #val,r   <tab/><tab/>(N, Z, C)");
					AbstractProcessor.HelpPutLine(builder, "[B4+r] [mh] [ll]<tab/>SUB ADDR,r   <tab/><tab/>(N, Z, C)");
					AbstractProcessor.HelpPutLine(builder, "[BC+r] [mh] [ll]<tab/>SUB r,ADDR   <tab/><tab/>(N, Z, C)");
					AbstractProcessor.HelpPutLine(builder, "[DF] [vv] [mh] [ll] SUB #val,ADDR  <tab/>(N, Z, C)");

					AbstractProcessor.HelpPutTitle(builder, "ET logique");
					AbstractProcessor.HelpPutLine(builder, "[E0]             <tab/>AND A,B      <tab/><tab/>(N, Z, C)");
					AbstractProcessor.HelpPutLine(builder, "[E1]             <tab/>AND B,A      <tab/><tab/>(N, Z, C)");
					AbstractProcessor.HelpPutLine(builder, "[74+r] [vv]      <tab/>AND #val,r   <tab/><tab/>(N, Z, C)");
					AbstractProcessor.HelpPutLine(builder, "[E8+r'] [mh] [ll]<tab/>AND ADDR,r'  <tab/><tab/>(N, Z, C)");
					AbstractProcessor.HelpPutLine(builder, "[F0+r'] [mh] [ll]<tab/>AND r',ADDR  <tab/><tab/>(N, Z, C)");

					AbstractProcessor.HelpPutTitle(builder, "OU logique");
					AbstractProcessor.HelpPutLine(builder, "[E2]             <tab/>OR A,B      <tab/><tab/>(N, Z, C)");
					AbstractProcessor.HelpPutLine(builder, "[E3]             <tab/>OR B,A      <tab/><tab/>(N, Z, C)");
					AbstractProcessor.HelpPutLine(builder, "[78+r] [vv]      <tab/>OR #val,r   <tab/><tab/>(N, Z, C)");
					AbstractProcessor.HelpPutLine(builder, "[EA+r'] [mh] [ll]<tab/>OR ADDR,r'  <tab/><tab/>(N, Z, C)");
					AbstractProcessor.HelpPutLine(builder, "[F2+r'] [mh] [ll]<tab/>OR r',ADDR  <tab/><tab/>(N, Z, C)");

					AbstractProcessor.HelpPutTitle(builder, "OU exclusif logique");
					AbstractProcessor.HelpPutLine(builder, "[E4]             <tab/>XOR A,B      <tab/><tab/>(N, Z, C)");
					AbstractProcessor.HelpPutLine(builder, "[E5]             <tab/>XOR B,A      <tab/><tab/>(N, Z, C)");
					AbstractProcessor.HelpPutLine(builder, "[7C+r] [vv]      <tab/>XOR #val,r   <tab/><tab/>(N, Z, C)");
					AbstractProcessor.HelpPutLine(builder, "[EC+r'] [mh] [ll]<tab/>XOR ADDR,r'  <tab/><tab/>(N, Z, C)");
					AbstractProcessor.HelpPutLine(builder, "[F4+r'] [mh] [ll]<tab/>XOR r',ADDR  <tab/><tab/>(N, Z, C)");

					AbstractProcessor.HelpPutTitle(builder, "Tester un bit");
					AbstractProcessor.HelpPutLine(builder, "[C0]             <tab/>TEST B:A      <tab/><tab/>(Z)");
					AbstractProcessor.HelpPutLine(builder, "[C1]             <tab/>TEST A:B      <tab/><tab/>(Z)");
					AbstractProcessor.HelpPutLine(builder, "[D0+r'] [vv]     <tab/>TEST r':#val  <tab/><tab/>(Z)");
					AbstractProcessor.HelpPutLine(builder, "[C8+r'] [mh] [ll]<tab/>TEST ADDR:r'  <tab/><tab/>(Z)");
					AbstractProcessor.HelpPutLine(builder, "[DC] [vv] [mh] [ll] TEST ADDR:#val   <tab/>(Z)");

					AbstractProcessor.HelpPutTitle(builder, "Tester puis mettre un bit à un");
					AbstractProcessor.HelpPutLine(builder, "[C2]             <tab/>TSET B:A      <tab/><tab/>(Z)");
					AbstractProcessor.HelpPutLine(builder, "[C3]             <tab/>TSET A:B      <tab/><tab/>(Z)");
					AbstractProcessor.HelpPutLine(builder, "[D2+r'] [vv]     <tab/>TSET r':#val  <tab/><tab/>(Z)");
					AbstractProcessor.HelpPutLine(builder, "[CA+r'] [mh] [ll]<tab/>TSET ADDR:r'  <tab/><tab/>(Z)");
					AbstractProcessor.HelpPutLine(builder, "[DD] [vv] [mh] [ll] TSET ADDR:#val   <tab/>(Z)");

					AbstractProcessor.HelpPutTitle(builder, "Tester puis mettre un bit à zéro");
					AbstractProcessor.HelpPutLine(builder, "[C4]             <tab/>TCLR B:A      <tab/><tab/>(Z)");
					AbstractProcessor.HelpPutLine(builder, "[C5]             <tab/>TCLR A:B      <tab/><tab/>(Z)");
					AbstractProcessor.HelpPutLine(builder, "[D4+r'] [vv]     <tab/>TCLR r':#val  <tab/><tab/>(Z)");
					AbstractProcessor.HelpPutLine(builder, "[CC+r'] [mh] [ll]<tab/>TCLR ADDR:r'  <tab/><tab/>(Z)");
					AbstractProcessor.HelpPutLine(builder, "[DE] [vv] [mh] [ll] TCLR ADDR:#val   <tab/>(Z)");

					AbstractProcessor.HelpPutTitle(builder, "Tester puis inverser un bit");
					AbstractProcessor.HelpPutLine(builder, "[C6]             <tab/>TNOT B:A      <tab/><tab/>(Z)");
					AbstractProcessor.HelpPutLine(builder, "[C7]             <tab/>TNOT A:B      <tab/><tab/>(Z)");
					AbstractProcessor.HelpPutLine(builder, "[D6+r'] [vv]     <tab/>TNOT r':#val  <tab/><tab/>(Z)");
					AbstractProcessor.HelpPutLine(builder, "[CE+r'] [mh] [ll]<tab/>TNOT ADDR:r'  <tab/><tab/>(Z)");
					AbstractProcessor.HelpPutLine(builder, "[DF] [vv] [mh] [ll] TNOT ADDR:#val   <tab/>(Z)");

					AbstractProcessor.HelpPutTitle(builder, "Comparaisons");
					AbstractProcessor.HelpPutLine(builder, "[60+r]          <tab/>COMP A,r      <tab/><tab/>(N, Z, C)");
					AbstractProcessor.HelpPutLine(builder, "[64+r]          <tab/>COMP B,r      <tab/><tab/>(N, Z, C)");
					AbstractProcessor.HelpPutLine(builder, "[68+r]          <tab/>COMP X,r      <tab/><tab/>(N, Z, C)");
					AbstractProcessor.HelpPutLine(builder, "[6C+r]          <tab/>COMP Y,r      <tab/><tab/>(N, Z, C)");
					AbstractProcessor.HelpPutLine(builder, "[70+r] [vv]     <tab/>COMP #val,r   <tab/><tab/>(N, Z, C)");
					AbstractProcessor.HelpPutLine(builder, "[F8+r] [mh] [ll]<tab/>COMP ADDR,r   <tab/><tab/>(N, Z, C)");
					AbstractProcessor.HelpPutLine(builder, "[DD] [vv] [mh] [ll] COMP #val,ADDR  <tab/>(N, Z, C)");

					AbstractProcessor.HelpPutTitle(builder, "Opérations unaires");
					AbstractProcessor.HelpPutLine(builder, "[20+r]          <tab/>CLR r");
					AbstractProcessor.HelpPutLine(builder, "[24+r]          <tab/>NOT r");
					AbstractProcessor.HelpPutLine(builder, "[28+r]          <tab/>INC r");
					AbstractProcessor.HelpPutLine(builder, "[2C+r]          <tab/>DEC r");
					AbstractProcessor.HelpPutLine(builder, "[A8] [mh] [ll]  <tab/>CLR ADDR");
					AbstractProcessor.HelpPutLine(builder, "[A9] [mh] [ll]  <tab/>NOT ADDR");
					AbstractProcessor.HelpPutLine(builder, "[AA] [mh] [ll]  <tab/>INC ADDR");
					AbstractProcessor.HelpPutLine(builder, "[AB] [mh] [ll]  <tab/>DEC ADDR");

					AbstractProcessor.HelpPutTitle(builder, "Rotations");
					AbstractProcessor.HelpPutLine(builder, "[30+r]          <tab/>RL r          <tab/><tab/>(N, Z, C)");
					AbstractProcessor.HelpPutLine(builder, "[34+r]          <tab/>RR r          <tab/><tab/>(N, Z, C)");
					AbstractProcessor.HelpPutLine(builder, "[38+r]          <tab/>RLC r         <tab/><tab/>(N, Z, C)");
					AbstractProcessor.HelpPutLine(builder, "[3C+r]          <tab/>RRC r         <tab/><tab/>(N, Z, C)");
					AbstractProcessor.HelpPutLine(builder, "[AC] [mh] [ll]  <tab/>RL ADDR       <tab/><tab/>(N, Z, C)");
					AbstractProcessor.HelpPutLine(builder, "[AD] [mh] [ll]  <tab/>RR ADDR       <tab/><tab/>(N, Z, C)");
					AbstractProcessor.HelpPutLine(builder, "[AE] [mh] [ll]  <tab/>RLC ADDR      <tab/><tab/>(N, Z, C)");
					AbstractProcessor.HelpPutLine(builder, "[AF] [mh] [ll]  <tab/>RRC ADDR      <tab/><tab/>(N, Z, C)");

					AbstractProcessor.HelpPutTitle(builder, "Divers");
					AbstractProcessor.HelpPutLine(builder, "[00]            <tab/>NOP");
					AbstractProcessor.HelpPutLine(builder, "[03]            <tab/>HALT");
					AbstractProcessor.HelpPutLine(builder, "[5C]            <tab/>EX A,B");
					AbstractProcessor.HelpPutLine(builder, "[5D]            <tab/>EX X,Y");
					AbstractProcessor.HelpPutLine(builder, "[5E]            <tab/>SWAP A");
					AbstractProcessor.HelpPutLine(builder, "[5F]            <tab/>SWAP B");
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
					AbstractProcessor.HelpPutLine(builder, "[18] [mh] [ll]<tab/>JUMP,CC ADDR");
					AbstractProcessor.HelpPutLine(builder, "[19] [mh] [ll]<tab/>JUMP,CS ADDR");
					AbstractProcessor.HelpPutLine(builder, "[1A] [mh] [ll]<tab/>JUMP,NC ADDR");
					AbstractProcessor.HelpPutLine(builder, "[1B] [mh] [ll]<tab/>JUMP,NS ADDR");

					AbstractProcessor.HelpPutTitle(builder, "Appels de routines");
					AbstractProcessor.HelpPutLine(builder, "[01] [mh] [ll]   <tab/>CALL ADDR");
					AbstractProcessor.HelpPutLine(builder, "[02]             <tab/>RET");

					AbstractProcessor.HelpPutTitle(builder, "Utilisation de la pile");
					AbstractProcessor.HelpPutLine(builder, "[08+r]           <tab/>PUSH r");
					AbstractProcessor.HelpPutLine(builder, "[0C+r]           <tab/>POP r");
					AbstractProcessor.HelpPutLine(builder, "[07] [vv]        <tab/>SUB #val,SP");
					AbstractProcessor.HelpPutLine(builder, "[06] [vv]        <tab/>ADD #val,SP");
					AbstractProcessor.HelpPutLine(builder, "[54+r] [40] [dd] <tab/>MOVE {SP}+depl,r <tab/>(N, Z, C)");
					AbstractProcessor.HelpPutLine(builder, "[58+r] [40] [dd] <tab/>MOVE r,{SP}+depl <tab/>(N, Z, C)");

					AbstractProcessor.HelpPutTitle(builder, "Gestion des fanions");
					AbstractProcessor.HelpPutLine(builder, "[04]             <tab/>SETC           <tab/><tab/>(C=1)");
					AbstractProcessor.HelpPutLine(builder, "[05]             <tab/>CLRC           <tab/><tab/>(C=0)");
					break;

				case "ROM":
					AbstractProcessor.HelpPutTitle(builder, "WaitKey");
					AbstractProcessor.HelpPutLine(builder, "Attend la pression d'une touche du clavier.");
					AbstractProcessor.HelpPutLine(builder, "[01] [08] [00]<tab/>CALL WaitKey");
					AbstractProcessor.HelpPutLine(builder, "in<tab/>-");
					AbstractProcessor.HelpPutLine(builder, "out<tab/>A touche pressée");
					AbstractProcessor.HelpPutLine(builder, "mod<tab/>A, F");

					AbstractProcessor.HelpPutTitle(builder, "WaitSec");
					AbstractProcessor.HelpPutLine(builder, "Attend un nombre de secondes. L'attente est approximativement calibrée à 1'000 IPS.");
					AbstractProcessor.HelpPutLine(builder, "[01] [08] [03]<tab/>CALL WaitSec");
					AbstractProcessor.HelpPutLine(builder, "in<tab/>A nombre de secondes");
					AbstractProcessor.HelpPutLine(builder, "out<tab/>-");
					AbstractProcessor.HelpPutLine(builder, "mod<tab/>F");

					AbstractProcessor.HelpPutTitle(builder, "DisplayBinaryDigit");
					AbstractProcessor.HelpPutLine(builder, "Affiche des segments à choix.");
					AbstractProcessor.HelpPutLine(builder, "[01] [08] [06]<tab/>CALL DisplayBinaryDigit");
					AbstractProcessor.HelpPutLine(builder, "in<tab/>A bits des segments à allumer");
					AbstractProcessor.HelpPutLine(builder, "<tab/>B digit 0..3 (de gauche à droite)");
					AbstractProcessor.HelpPutLine(builder, "out<tab/>-");
					AbstractProcessor.HelpPutLine(builder, "mod<tab/>F");

					AbstractProcessor.HelpPutTitle(builder, "DisplayHexaDigit");
					AbstractProcessor.HelpPutLine(builder, "Affiche un digit hexadécimal.");
					AbstractProcessor.HelpPutLine(builder, "[01] [08] [09]<tab/>CALL DisplayHexaDigit");
					AbstractProcessor.HelpPutLine(builder, "in<tab/>A valeur 0..15");
					AbstractProcessor.HelpPutLine(builder, "<tab/>B digit 0..3 (de gauche à droite)");
					AbstractProcessor.HelpPutLine(builder, "out<tab/>-");
					AbstractProcessor.HelpPutLine(builder, "mod<tab/>F");

					AbstractProcessor.HelpPutTitle(builder, "DisplayHexaByte");
					AbstractProcessor.HelpPutLine(builder, "Affiche un byte hexadécimal sur deux digits.");
					AbstractProcessor.HelpPutLine(builder, "[01] [08] [0C]<tab/>CALL DisplayHexaByte");
					AbstractProcessor.HelpPutLine(builder, "in<tab/>A valeur 0..255");
					AbstractProcessor.HelpPutLine(builder, "<tab/>B premier digit 0..2 (de gauche à droite)");
					AbstractProcessor.HelpPutLine(builder, "out<tab/>-");
					AbstractProcessor.HelpPutLine(builder, "mod<tab/>F");

					AbstractProcessor.HelpPutTitle(builder, "DisplayDecimal");
					AbstractProcessor.HelpPutLine(builder, "Affiche une valeur décimale sur quatre digits.");
					AbstractProcessor.HelpPutLine(builder, "[01] [08] [0F]<tab/>CALL DisplayDecimal");
					AbstractProcessor.HelpPutLine(builder, "in<tab/>A valeur 0..255");
					AbstractProcessor.HelpPutLine(builder, "out<tab/>-");
					AbstractProcessor.HelpPutLine(builder, "mod<tab/>F");

					AbstractProcessor.HelpPutTitle(builder, "ClearScreen");
					AbstractProcessor.HelpPutLine(builder, "Efface tout l'écran bitmap.");
					AbstractProcessor.HelpPutLine(builder, "[01] [08] [1B]<tab/>CALL ClearScreen");
					AbstractProcessor.HelpPutLine(builder, "in<tab/>-");
					AbstractProcessor.HelpPutLine(builder, "out<tab/>-");
					AbstractProcessor.HelpPutLine(builder, "mod<tab/>F");

					AbstractProcessor.HelpPutTitle(builder, "SetPixel");
					AbstractProcessor.HelpPutLine(builder, "Allume un pixel dans l'écran bitmap.");
					AbstractProcessor.HelpPutLine(builder, "[01] [08] [12]<tab/>CALL SetPixel");
					AbstractProcessor.HelpPutLine(builder, "in<tab/>X coordonnée colonne 0..31");
					AbstractProcessor.HelpPutLine(builder, "<tab/>Y coordonnée ligne 0..23");
					AbstractProcessor.HelpPutLine(builder, "out<tab/>-");
					AbstractProcessor.HelpPutLine(builder, "mod<tab/>F");

					AbstractProcessor.HelpPutTitle(builder, "ClrPixel");
					AbstractProcessor.HelpPutLine(builder, "Eteint un pixel dans l'écran bitmap.");
					AbstractProcessor.HelpPutLine(builder, "[01] [08] [15]<tab/>CALL ClrPixel");
					AbstractProcessor.HelpPutLine(builder, "in<tab/>X coordonnée colonne 0..31");
					AbstractProcessor.HelpPutLine(builder, "<tab/>Y coordonnée ligne 0..23");
					AbstractProcessor.HelpPutLine(builder, "out<tab/>-");
					AbstractProcessor.HelpPutLine(builder, "mod<tab/>F");

					AbstractProcessor.HelpPutTitle(builder, "NotPixel");
					AbstractProcessor.HelpPutLine(builder, "Inverse un pixel dans l'écran bitmap.");
					AbstractProcessor.HelpPutLine(builder, "[01] [08] [18]<tab/>CALL NotPixel");
					AbstractProcessor.HelpPutLine(builder, "in<tab/>X coordonnée colonne 0..31");
					AbstractProcessor.HelpPutLine(builder, "<tab/>Y coordonnée ligne 0..23");
					AbstractProcessor.HelpPutLine(builder, "out<tab/>-");
					AbstractProcessor.HelpPutLine(builder, "mod<tab/>F");

					AbstractProcessor.HelpPutTitle(builder, "DrawChar");
					AbstractProcessor.HelpPutLine(builder, "Dessine un caractère dans l'écran bitmap. Les chiffres sont codés de 30 à 39, et les lettres de 41 à 5A.");
					AbstractProcessor.HelpPutLine(builder, "[01] [08] [1E]<tab/>CALL DrawChar");
					AbstractProcessor.HelpPutLine(builder, "in<tab/>A caractère ascii");
					AbstractProcessor.HelpPutLine(builder, "<tab/>X colonne 0..7");
					AbstractProcessor.HelpPutLine(builder, "<tab/>Y ligne 0..3");
					AbstractProcessor.HelpPutLine(builder, "out<tab/>-");
					AbstractProcessor.HelpPutLine(builder, "mod<tab/>F");
					break;
			}

			return builder.ToString();
		}
		#endregion


		protected static readonly int FlagCarry    = 0;
		protected static readonly int FlagZero     = 1;
		protected static readonly int FlagNeg      = 2;

		
		protected int registerPC;  // program counter
		protected int registerSP;  // stack pointer
		protected int registerF;   // flags
		protected int registerA;   // accumulateur 8 bits
		protected int registerB;   // accumulateur 8 bits
		protected int registerX;   // pointeur 8 bits
		protected int registerY;   // pointeur 8 bits
	}
}
