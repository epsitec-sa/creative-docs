using System.Collections.Generic;

namespace Epsitec.Common.Designer.Undo
{
	/// <summary>
	/// Cette classe contient l'état mémorisé complet, "photographié" avant d'effectuer une action par exemple.
	/// </summary>
	public class Shapshot
	{
		public Shapshot()
		{
			this.selection = new List<int>();
		}

		public string Name
		{
			//	Description de la photographie (texte en français). En plus de décrire la photographie dans le menu,
			//	ce texte permet de distinguer deux photographies différentes.
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		public string SerializedData
		{
			//	Données sérialisées correspondant à la photographie mémorisée.
			get
			{
				return this.serializedData;
			}
			set
			{
				this.serializedData = value;
			}
		}

		public List<int> Selection
		{
			//	Etat de la sélection.
			get
			{
				return this.selection;
			}
		}


		protected string		name;
		protected string		serializedData;
		protected List<int>		selection;
	}
}
