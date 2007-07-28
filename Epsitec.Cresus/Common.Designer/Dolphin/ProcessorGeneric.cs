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
			this.registerSP = this.memory.Length;
			this.registerF = 0;
			this.registerA = 0;
			this.registerB = 0;
			this.registerC = 0;
		}

		public override void Clock()
		{
			//	Exécute une instruction du processeur.
			if (this.registerPC < 0 || this.registerPC >= this.memory.Length)
			{
				this.Reset();
			}

			Instructions op = (Instructions) this.memory.Read(this.registerPC++);

			switch (op)
			{
				case Instructions.JumpAbs:
					this.registerPC = (this.memory.Read(this.registerPC++) << 8) | (this.memory.Read(this.registerPC++));
					break;

				case Instructions.JumpRel:
					this.registerPC += this.memory.Read(this.registerPC++);
					break;
			}
		}


		public override int GetRegisterSize(string name)
		{
			//	Retourne la taille (nombre de bits) d'un registre.
			switch (name)
			{
				case "PC":
				case "SP":
					return DolphinApplication.TotalAddress;

				case "F":
				case "A":
				case "B":
				case "C":
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

				case "C":
					return this.registerC;
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

				case "C":
					this.registerC = value;
					break;
			}
		}

		
		protected int registerPC;  // program counter
		protected int registerSP;  // stack pointer
		protected int registerF;  // flags
		protected int registerA;  // accumulator
		protected int registerB;
		protected int registerC;
	}
}
