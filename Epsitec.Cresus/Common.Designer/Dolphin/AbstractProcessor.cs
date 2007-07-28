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


		public virtual string GetRegisterName(string name)
		{
			//	Retourne le nom d'un registre.
			return name;
		}

		public virtual int GetRegisterSize(string name)
		{
			//	Retourne la taille (nombre de bits) d'un registre.
			return 0;
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


		protected DolphinApplication.Memory memory;
	}
}
