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
	public class Types2 : AbstractCaptions2
	{
		public Types2(Module module, PanelsContext context, ResourceAccess access, MainWindow mainWindow) : base (module, context, access, mainWindow)
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
				return ResourceAccess.Type.Types2;
			}
		}


		protected override void InitializeTable()
		{
			//	Initialise la table.
			StructuredType cultureMapType = new StructuredType();
			cultureMapType.Fields.Add("Name", StringType.Default);
			cultureMapType.Fields.Add("Type", StringType.Default);
			cultureMapType.Fields.Add("Primary", StringType.Default);
			cultureMapType.Fields.Add("Secondary", StringType.Default);
			cultureMapType.Fields.Add("Druid", StringType.Default);
			cultureMapType.Fields.Add("Local", StringType.Default);
			cultureMapType.Fields.Add("Identity", StringType.Default);

			this.table.SourceType = cultureMapType;

			this.table.Columns.Add(new UI.ItemTableColumn("Name", new Widgets.Layouts.GridLength(this.GetColumnWidth(0), Widgets.Layouts.GridUnitType.Proportional)));
			this.table.Columns.Add(new UI.ItemTableColumn("Type", new Widgets.Layouts.GridLength(this.GetColumnWidth(1), Widgets.Layouts.GridUnitType.Proportional)));
			this.table.Columns.Add(new UI.ItemTableColumn("Primary", new Widgets.Layouts.GridLength(this.GetColumnWidth(2), Widgets.Layouts.GridUnitType.Proportional)));
			this.table.Columns.Add(new UI.ItemTableColumn("Secondary", new Widgets.Layouts.GridLength(this.GetColumnWidth(3), Widgets.Layouts.GridUnitType.Proportional)));
			this.table.Columns.Add(new UI.ItemTableColumn("Druid", new Widgets.Layouts.GridLength(this.GetColumnWidth(4), Widgets.Layouts.GridUnitType.Proportional)));
			this.table.Columns.Add(new UI.ItemTableColumn("Local", new Widgets.Layouts.GridLength(this.GetColumnWidth(5), Widgets.Layouts.GridUnitType.Proportional)));
			this.table.Columns.Add(new UI.ItemTableColumn("Identity", new Widgets.Layouts.GridLength(this.GetColumnWidth(6), Widgets.Layouts.GridUnitType.Proportional)));

			this.table.ColumnHeader.SetColumnComparer(1, Types2.CompareTypeColumns);
			this.table.ColumnHeader.SetColumnComparer(2, this.ComparePrimary);
			this.table.ColumnHeader.SetColumnComparer(3, this.CompareSecondary);
			this.table.ColumnHeader.SetColumnComparer(4, this.CompareDruid);
			this.table.ColumnHeader.SetColumnComparer(5, this.CompareLocal);
			this.table.ColumnHeader.SetColumnComparer(6, this.CompareIdentity);

			this.table.ColumnHeader.SetColumnText(0, "Nom");
			this.table.ColumnHeader.SetColumnText(1, "Type");
			this.table.ColumnHeader.SetColumnText(4, "Druid");
			this.table.ColumnHeader.SetColumnText(5, "Local");
			this.table.ColumnHeader.SetColumnText(6, "Identité");
			
			this.table.ColumnHeader.SetColumnSort(0, ListSortDirection.Ascending);
			this.table.ColumnHeader.SetColumnSort(1, ListSortDirection.Ascending);
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
				return Types2.columnWidthHorizontal[column];
			}
			else
			{
				return Types2.columnWidthVertical[column];
			}
		}

		protected override void SetColumnWidth(int column, double value)
		{
			//	Mémorise la largeur à utiliser pour une colonne de la liste de gauche.
			if (this.mainWindow.DisplayModeState == MainWindow.DisplayMode.Horizontal)
			{
				Types2.columnWidthHorizontal[column] = value;
			}
			else
			{
				Types2.columnWidthVertical[column] = value;
			}
		}
	
	
		private static int CompareTypeColumns(object a, object b)
		{
			CultureMap itemA = a as CultureMap;
			CultureMap itemB = b as CultureMap;

			StructuredData dataA = itemA.GetCultureData(Common.Support.Resources.DefaultTwoLetterISOLanguageName);
			StructuredData dataB = itemB.GetCultureData(Common.Support.Resources.DefaultTwoLetterISOLanguageName);

			string codeA = dataA.GetValue(Support.Res.Fields.ResourceBaseType.TypeCode).ToString();
			string codeB = dataB.GetValue(Support.Res.Fields.ResourceBaseType.TypeCode).ToString();

			return codeA.CompareTo(codeB);
		}


		private static double[]				columnWidthHorizontal = {140, 60, 100, 100, 80, 50, 100};
		private static double[]				columnWidthVertical = {250, 60, 270, 270, 80, 50, 100};
	}
}
