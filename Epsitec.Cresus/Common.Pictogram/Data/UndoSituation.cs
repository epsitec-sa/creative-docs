using Epsitec.Common.Widgets;

namespace Epsitec.Common.Pictogram.Data
{
	/// <summary>
	/// La classe UndoSituation permet de mémoriser une situation complète.
	/// </summary>
	public class UndoSituation
	{
		public UndoSituation()
		{
		}

		// Liste des objets.
		public IconObjects IconObjects
		{
			get { return this.iconObjects; }
			set { this.iconObjects = value; }
		}

		// Nom de l'opération.
		public string Operation
		{
			get { return this.operation; }
			set { this.operation = value; }
		}

		// Objet changé.
		public AbstractObject Object
		{
			get { return this.obj; }
			set { this.obj = value; }
		}

		// Type de la propriété changée.
		public PropertyType PropertyType
		{
			get { return this.propertyType; }
			set { this.propertyType = value; }
		}

		// Nom de l'outil sélectionné.
		public string SelectedTool
		{
			get { return this.selectedTool; }
			set { this.selectedTool = value; }
		}

		protected IconObjects			iconObjects;
		protected string				operation;
		protected AbstractObject		obj;
		protected PropertyType			propertyType;
		protected string				selectedTool;
	}
}
