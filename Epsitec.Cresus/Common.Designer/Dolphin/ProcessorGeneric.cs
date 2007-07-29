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

		public override string GetRegisterBitNames(string name)
		{
			//	Retourne les noms des bits d'un registre.
			if (name == "F")
			{
				return "CWLG    ";  // bits 0..7 !
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

		
		public override List<string> HelpChapters
		{
			//	Retourne la liste des chapitres.
			get
			{
				List<string> chapters = new List<string>();
				
				chapters.Add("Hexa");
				chapters.Add("Load");
				chapters.Add("Op");
				chapters.Add("Test");
				chapters.Add("Jump");
				
				return chapters;
			}
		}

		public override string HelpChapter(string chapter)
		{
			//	Retourne le texte d'un chapitre.
			System.Text.StringBuilder builder = new System.Text.StringBuilder();

			switch (chapter)
			{
				case "Hexa":
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
					break;

				case "Load":
					AbstractProcessor.HelpPutTitle(builder, "Valeur immédiate");
					AbstractProcessor.HelpPutLine(builder, "[03] [xx] :  LOAD A, #xx");
					AbstractProcessor.HelpPutLine(builder, "[04] [xx] :  LOAD B, #xx");
					AbstractProcessor.HelpPutLine(builder, "[05] [xx] [yy] :  LOAD HL, #xxyy");

					AbstractProcessor.HelpPutTitle(builder, "Registre à registre");
					AbstractProcessor.HelpPutLine(builder, "[06] : LOAD A, B");
					AbstractProcessor.HelpPutLine(builder, "[07] : LOAD B, A");

					AbstractProcessor.HelpPutTitle(builder, "Registre à mémoire");
					AbstractProcessor.HelpPutLine(builder, "[08] [hh] [ll] :  LOAD A, hhll");
					AbstractProcessor.HelpPutLine(builder, "[09] [hh] [ll] :  LOAD hhll, A");
					break;

				case "Op":
					AbstractProcessor.HelpPutTitle(builder, "Compteurs");
					AbstractProcessor.HelpPutLine(builder, "[10] :  INC A");
					AbstractProcessor.HelpPutLine(builder, "[11] :  INC B");
					AbstractProcessor.HelpPutLine(builder, "[12] :  INC HL");
					break;

				case "Test":
					AbstractProcessor.HelpPutTitle(builder, "");
					AbstractProcessor.HelpPutLine(builder, "");
					break;

				case "Jump":
					AbstractProcessor.HelpPutTitle(builder, "Absolu");
					AbstractProcessor.HelpPutLine(builder, "[01] [hh] [ll] :  JUMP hhll");
					AbstractProcessor.HelpPutLine(builder, "[02] [dd] :  JUMP' +dd");

					AbstractProcessor.HelpPutTitle(builder, "Conditionnel");
					AbstractProcessor.HelpPutLine(builder, "[20] [hh] [ll] :  JUMP,EQ hhll");
					break;
			}

			return builder.ToString();
		}

		
		protected int registerPC;  // program counter
		protected int registerSP;  // stack pointer
		protected int registerF;   // flags
		protected int registerA;   // accumulator 8 bits
		protected int registerB;   // registre auxiliaire 8 bits
		protected int registerHL;  // registre 12 bits
	}
}
