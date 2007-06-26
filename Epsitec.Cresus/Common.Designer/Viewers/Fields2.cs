using System.Collections.Generic;
using System.Text.RegularExpressions;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Viewers
{
	/// <summary>
	/// Permet de représenter les ressources d'un module.
	/// </summary>
	public class Fields2 : AbstractCaptions2
	{
		public Fields2(Module module, PanelsContext context, ResourceAccess access, MainWindow mainWindow) : base(module, context, access, mainWindow)
		{
			this.UpdateAll();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
			}

			base.Dispose(disposing);
		}


		public override ResourceAccess.Type ResourceType
		{
			get
			{
				return ResourceAccess.Type.Fields2;
			}
		}

	
		protected override void InitializeTable()
		{
			//	Initialise la table.
			StructuredType cultureMapType = new StructuredType();
			cultureMapType.Fields.Add("Entity", StringType.Default);
			cultureMapType.Fields.Add("Name", StringType.Default);
			cultureMapType.Fields.Add("Primary", StringType.Default);
			cultureMapType.Fields.Add("Secondary", StringType.Default);

			this.table.SourceType = cultureMapType;

			this.table.Columns.Add(new UI.ItemTableColumn("Entity", new Widgets.Layouts.GridLength(this.GetColumnWidth(0), Widgets.Layouts.GridUnitType.Proportional)));
			this.table.Columns.Add(new UI.ItemTableColumn("Name", new Widgets.Layouts.GridLength(this.GetColumnWidth(1), Widgets.Layouts.GridUnitType.Proportional)));
			this.table.Columns.Add(new UI.ItemTableColumn("Primary", new Widgets.Layouts.GridLength(this.GetColumnWidth(2), Widgets.Layouts.GridUnitType.Proportional)));
			this.table.Columns.Add(new UI.ItemTableColumn("Secondary", new Widgets.Layouts.GridLength(this.GetColumnWidth(3), Widgets.Layouts.GridUnitType.Proportional)));

			this.table.ColumnHeader.SetColumnComparer(0, Fields2.CompareTypeColumns);
			this.table.ColumnHeader.SetColumnComparer(2, this.ComparePrimary);
			this.table.ColumnHeader.SetColumnComparer(3, this.CompareSecondary);

			this.table.ColumnHeader.SetColumnText(0, "Entité");
			this.table.ColumnHeader.SetColumnText(1, "Nom");

			this.table.ColumnHeader.SetColumnSort(1, ListSortDirection.Ascending);
			this.table.ColumnHeader.SetColumnSort(0, ListSortDirection.Ascending);
		}

		protected override int PrimaryColumn
		{
			//	Retourne le rang de la colonne pour la culture principale.
			get
			{
				return 2;
			}
		}

		protected override int SecondaryColumn
		{
			//	Retourne le rang de la colonne pour la culture secondaire.
			get
			{
				return 3;
			}
		}


		protected override double GetColumnWidth(int column)
		{
			//	Retourne la largeur à utiliser pour une colonne de la liste de gauche.
			if (this.mainWindow.DisplayModeState == MainWindow.DisplayMode.Horizontal)
			{
				return Fields2.columnWidthHorizontal[column];
			}
			else
			{
				return Fields2.columnWidthVertical[column];
			}
		}

		protected override void SetColumnWidth(int column, double value)
		{
			//	Mémorise la largeur à utiliser pour une colonne de la liste de gauche.
			if (this.mainWindow.DisplayModeState == MainWindow.DisplayMode.Horizontal)
			{
				Fields2.columnWidthHorizontal[column] = value;
			}
			else
			{
				Fields2.columnWidthVertical[column] = value;
			}
		}
	
	
		private static int CompareTypeColumns(object a, object b)
		{
			CultureMap itemA = a as CultureMap;
			CultureMap itemB = b as CultureMap;

			return itemA.Prefix.CompareTo(itemB.Prefix);
		}


		private static double[]				columnWidthHorizontal = {80, 120, 100, 100};
		private static double[]				columnWidthVertical = {100, 210, 270, 270};
	}
}
