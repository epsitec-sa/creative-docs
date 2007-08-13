//	Copyright © 2003-2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
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

		public virtual string DessassemblyInstruction(List<int> codes)
		{
			//	Retourne le nom d'une instruction.
			return "NOP";
		}

		public virtual List<int> AssemblyInstruction(string instruction)
		{
			//	Retourne les codes d'une instruction.
			List<int> codes = new List<int>();
			codes.Add(0);
			return codes;
		}

		
		public virtual void RomInitialise(int address, int length)
		{
			//	Rempli la Rom.
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

			builder.Append("<font size=\"150%\"><b>");
			builder.Append(title);
			builder.Append("</b></font><br/><br/>");
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
