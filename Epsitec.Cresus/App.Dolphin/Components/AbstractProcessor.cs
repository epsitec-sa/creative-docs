//	Copyright © 2003-2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;

namespace Epsitec.App.Dolphin.Components
{
	/// <summary>
	/// Processeur abstrait.
	/// </summary>
	public abstract class AbstractProcessor
	{
		public AbstractProcessor(Memory memory)
		{
			//	Constructeur du processeur.
			this.memory = memory;
			this.Reset();
		}

		public virtual string Name
		{
			//	Nom du processeur.
			get
			{
				return "";
			}
		}

		public virtual void Reset()
		{
			//	Reset du processeur.
			this.isHalted = false;
		}

		public virtual void Clock()
		{
			//	Exécute une instruction du processeur.
		}

		public virtual bool IsHalted
		{
			//	Indique si le processeur est stoppé par l'instruction Halt.
			get
			{
				return this.isHalted;
			}
		}

		public virtual bool IsCall(out int retAddress)
		{
			//	Indique si le processeur est sur une instruction CALL.
			//	Si oui, retourne l'adresse après le CALL.
			retAddress = 0;
			return false;
		}

		public virtual int NopInstruction
		{
			//	Retourne le code de l'instruction NOP.
			get
			{
				return 0x00;
			}
		}

		public virtual int TableInstruction
		{
			//	Retourne le code de l'instruction TABLE.
			get
			{
				return 0xFF;
			}
		}


		public virtual IEnumerable<string> RegisterNames
		{
			//	Enumère tous les noms de registres.
			get
			{
				yield return null;
			}
		}

		public virtual int GetRegisterSize(string name)
		{
			//	Retourne la taille (nombre de bits) d'un registre.
			return 0;
		}

		public virtual string GetRegisterBitNames(string name)
		{
			//	Retourne les noms des bits d'un registre.
			return null;
		}

		public virtual int GetRegisterValue(string name)
		{
			//	Retourne la valeur d'un registre.
			return 0xffff;
		}

		public virtual void SetRegisterValue(string name, int value)
		{
			//	Modifie la valeur d'un registre.
		}


		public virtual int GetInstructionLength(int code)
		{
			//	Retourne le nombre de bytes d'une instruction.
			return 1;
		}

		public virtual string DessassemblyInstruction(List<int> codes, int pc, out int address)
		{
			//	Retourne le nom d'une instruction.
			address = Misc.undefined;
			return "NOP";
		}

		public virtual void AssemblySplitAddr(string addr, out string text, out int mode)
		{
			//	Extrait les modes d'adressage spéciaux d'une adresse.
			text = addr;
			mode = 0;
		}

		public virtual string AssemblyCombineAddr(string text, int mode)
		{
			//	Combine les modes d'adressage spéciaux d'une adresse.
			return text;
		}

		public virtual string AssemblyPreprocess(string instruction)
		{
			//	Pré-traitement avant AssemblyInstruction.
			//	En retour, tout est en majuscule avec un espace pour séparer les arguments.
			return instruction.ToUpper().Trim();
		}

		public virtual string AssemblyInstruction(string instruction, List<int> codes)
		{
			//	Assemble les codes d'une instruction et retourne une éventuelle erreur.
			codes.Clear();
			codes.Add(0);  // NOP
			return null;  // ok
		}

		
		public virtual void RomInitialise(int address, int length)
		{
			//	Rempli la Rom.
		}

		public virtual void RomVariables(int address, Dictionary<string, int> variables)
		{
			//	Défini les variables de la Rom.
		}


		public virtual List<string> HelpChapters
		{
			//	Retourne la liste des chapitres.
			get
			{
				return null;
			}
		}

		public virtual string HelpChapter(string chapter)
		{
			//	Retourne le texte d'un chapitre.
			return null;
		}

		protected static void HelpPutTitle(System.Text.StringBuilder builder, string title)
		{
			if (builder.Length > 0)
			{
				builder.Append("<br/>");
			}

			builder.Append(Misc.Bold(Misc.FontSize(title, 150)));
			builder.Append("<br/><br/>");
		}

		protected static void HelpPutLine(System.Text.StringBuilder builder, string line)
		{
			builder.Append(line);
			builder.Append("<br/>");
		}


		protected Memory memory;
		protected bool isHalted;
	}
}
