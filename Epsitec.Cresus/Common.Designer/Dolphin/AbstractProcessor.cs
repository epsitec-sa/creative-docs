using System.Collections.Generic;

namespace Epsitec.Common.Designer.Dolphin
{
	/// <summary>
	/// Processeur abstrait.
	/// </summary>
	public abstract class AbstractProcessor
	{
		public AbstractProcessor(DolphinApplication.Memory memory)
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
		}

		public virtual void Clock()
		{
			//	Exécute une instruction du processeur.
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
			//	Retourne les noms des bits du registre.
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


		protected DolphinApplication.Memory memory;
	}
}
