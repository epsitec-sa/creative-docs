using Epsitec.Common.Widgets;

namespace Epsitec.Common.Pictogram.Data
{
	/// <summary>
	/// La classe UndoSituation permet de m�moriser une situation compl�te.
	/// </summary>
	public class UndoSituation
	{
		public UndoSituation()
		{
		}

		// Nom de l'op�ration.
		public string Operation
		{
			get { return this.operation; }
			set { this.operation = value; }
		}

		// Liste des objets.
		public IconObjects IconObjects
		{
			get { return this.iconObjects; }
			set { this.iconObjects = value; }
		}

		// Liste des styles.
		public StylesCollection StylesCollection
		{
			get { return this.stylesCollection; }
			set { this.stylesCollection = value; }
		}

		// Objet chang�.
		public AbstractObject Object
		{
			get { return this.obj; }
			set { this.obj = value; }
		}

		// Type de la propri�t� chang�e.
		public PropertyType PropertyType
		{
			get { return this.propertyType; }
			set { this.propertyType = value; }
		}

		// Nom de l'outil s�lectionn�.
		public string SelectedTool
		{
			get { return this.selectedTool; }
			set { this.selectedTool = value; }
		}

		// Modificateur global.
		public GlobalModifierData ModifierData
		{
			get { return this.modifierData; }
			set { this.modifierData = value; }
		}

		protected string				operation;
		protected IconObjects			iconObjects;
		protected StylesCollection		stylesCollection;
		protected AbstractObject		obj;
		protected PropertyType			propertyType;
		protected string				selectedTool;
		protected GlobalModifierData	modifierData;
	}
}
