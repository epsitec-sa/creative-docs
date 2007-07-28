using System.Collections.Generic;

namespace Epsitec.Common.Designer.Dolphin
{
	/// <summary>
	/// Processeur générique tout simple.
	/// </summary>
	public class ProcessorGeneric : AbstractProcessor
	{
		protected enum Instructions
		{
			Nop = 0x00,
			JumpAbs = 0x01,
			JumpRel = 0x02,
			LoadAi = 0x03,
			LoadBi = 0x04,
			LoadHLi = 0x05,
			LoadAB = 0x06,
			LoadBA = 0x07,
			LoadAm = 0x08,
			LoadmA = 0x09,
			IncA = 0x10,
			IncB = 0x11,
			IncHL = 0x12,
		}


		public ProcessorGeneric(DolphinApplication.Memory memory) : base(memory)
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
			this.registerPC = 0;
			this.registerSP = this.memory.Length-1;
			this.registerF = 0;
			this.registerA = 0;
			this.registerB = 0;
			this.registerHL = 0;
		}

		public override void Clock()
		{
			//	Exécute une instruction du processeur.
			if (this.registerPC < 0 || this.registerPC >= this.memory.Length)
			{
				this.Reset();
			}

			Instructions op = (Instructions) this.memory.Read(this.registerPC++);
			int address;

			switch (op)
			{
				case Instructions.JumpAbs:
					this.registerPC = (this.memory.Read(this.registerPC++) << 8) | (this.memory.Read(this.registerPC++));
					break;

				case Instructions.JumpRel:
					this.registerPC += this.memory.Read(this.registerPC++);
					break;

				case Instructions.LoadAi:
					this.registerA = this.memory.Read(this.registerPC++);
					break;

				case Instructions.LoadBi:
					this.registerB = this.memory.Read(this.registerPC++);
					break;

				case Instructions.LoadHLi:
					this.registerHL = (this.memory.Read(this.registerPC++) << 8) | (this.memory.Read(this.registerPC++));
					break;

				case Instructions.LoadAB:
					this.registerA = this.registerB;
					break;

				case Instructions.LoadBA:
					this.registerB = this.registerA;
					break;

				case Instructions.LoadAm:
					address = (this.memory.Read(this.registerPC++) << 8) | (this.memory.Read(this.registerPC++));
					this.registerA = this.memory.Read(address);
					break;

				case Instructions.LoadmA:
					address = (this.memory.Read(this.registerPC++) << 8) | (this.memory.Read(this.registerPC++));
					this.memory.Write(address, this.registerA);
					break;

				case Instructions.IncA:
					this.registerA++;
					break;

				case Instructions.IncB:
					this.registerB++;
					break;

				case Instructions.IncHL:
					this.registerHL++;
					break;
			}
		}


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
					return DolphinApplication.TotalAddress;

				case "F":
				case "A":
				case "B":
					return DolphinApplication.TotalData;
			}

			return base.GetRegisterSize(name);
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

		
		protected int registerPC;  // program counter
		protected int registerSP;  // stack pointer
		protected int registerF;   // flags
		protected int registerA;   // accumulator 8 bits
		protected int registerB;   // registre auxiliaire 8 bits
		protected int registerHL;  // registre 12 bits
	}
}
