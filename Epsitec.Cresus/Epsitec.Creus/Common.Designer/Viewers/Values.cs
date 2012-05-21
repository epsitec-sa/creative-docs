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
	public class Values : AbstractCaptions
	{
		public Values(Module module, PanelsContext context, ResourceAccess access, DesignerApplication designerApplication)
			: base (module, context, access, designerApplication)
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
				return ResourceAccess.Type.Values;
			}
		}


		protected override void InitializeTable()
		{
			//	Initialise la table.
			StructuredType cultureMapType = new StructuredType();
			cultureMapType.Fields.Add("Prefix", StringType.NativeDefault);
			cultureMapType.Fields.Add("Name", StringType.NativeDefault);
			cultureMapType.Fields.Add("Primary", StringType.NativeDefault);
			cultureMapType.Fields.Add("Secondary", StringType.NativeDefault);
			cultureMapType.Fields.Add("Source", StringType.NativeDefault);
			cultureMapType.Fields.Add("Druid", StringType.NativeDefault);
			cultureMapType.Fields.Add("Local", StringType.NativeDefault);
			cultureMapType.Fields.Add("Identity", StringType.NativeDefault);
			cultureMapType.Fields.Add("PatchLevel", StringType.NativeDefault);

			this.table.SourceType = cultureMapType;

			this.table.Columns.Add(new UI.ItemTableColumn("Prefix", new Widgets.Layouts.GridLength(this.GetColumnWidth(0), Widgets.Layouts.GridUnitType.Proportional)));
			this.table.Columns.Add(new UI.ItemTableColumn("Name", new Widgets.Layouts.GridLength(this.GetColumnWidth(1), Widgets.Layouts.GridUnitType.Proportional)));
			this.table.Columns.Add(new UI.ItemTableColumn("Primary", new Widgets.Layouts.GridLength(this.GetColumnWidth(2), Widgets.Layouts.GridUnitType.Proportional)));
			this.table.Columns.Add(new UI.ItemTableColumn("Secondary", new Widgets.Layouts.GridLength(this.GetColumnWidth(3), Widgets.Layouts.GridUnitType.Proportional)));
			this.table.Columns.Add(new UI.ItemTableColumn("Source", new Widgets.Layouts.GridLength(this.GetColumnWidth(4), Widgets.Layouts.GridUnitType.Proportional)));
			this.table.Columns.Add(new UI.ItemTableColumn("Druid", new Widgets.Layouts.GridLength(this.GetColumnWidth(5), Widgets.Layouts.GridUnitType.Proportional)));
			this.table.Columns.Add(new UI.ItemTableColumn("Local", new Widgets.Layouts.GridLength(this.GetColumnWidth(6), Widgets.Layouts.GridUnitType.Proportional)));
			this.table.Columns.Add(new UI.ItemTableColumn("Identity", new Widgets.Layouts.GridLength(this.GetColumnWidth(7), Widgets.Layouts.GridUnitType.Proportional)));
			this.table.Columns.Add(new UI.ItemTableColumn("PatchLevel", new Widgets.Layouts.GridLength(this.GetColumnWidth(8), Widgets.Layouts.GridUnitType.Proportional)));

			this.table.ColumnHeader.SetColumnComparer(0, Values.CompareTypeColumns);
			this.table.ColumnHeader.SetColumnComparer(2, this.ComparePrimary);
			this.table.ColumnHeader.SetColumnComparer(3, this.CompareSecondary);
			this.table.ColumnHeader.SetColumnComparer(4, this.CompareSource);
			this.table.ColumnHeader.SetColumnComparer(5, this.CompareDruid);
			this.table.ColumnHeader.SetColumnComparer(6, this.CompareLocal);
			this.table.ColumnHeader.SetColumnComparer(7, this.CompareIdentity);
			this.table.ColumnHeader.SetColumnComparer(8, this.ComparePatchLevel);

			this.table.ColumnHeader.SetColumnText(0, Res.Strings.Viewers.Column.Prefix);
			this.table.ColumnHeader.SetColumnText(1, Res.Strings.Viewers.Column.Name);
			this.table.ColumnHeader.SetColumnText(5, Res.Strings.Viewers.Column.Druid);
			this.table.ColumnHeader.SetColumnText(6, Res.Strings.Viewers.Column.Local);
			this.table.ColumnHeader.SetColumnText(7, Res.Strings.Viewers.Column.Identity);
			this.table.ColumnHeader.SetColumnText(8, Res.Strings.Viewers.Column.PatchLevel);

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
			if (this.designerApplication.DisplayModeState == DesignerApplication.DisplayMode.Horizontal)
			{
				return Values.columnWidthHorizontal[column];
			}
			else
			{
				return Values.columnWidthVertical[column];
			}
		}

		protected override void SetColumnWidth(int column, double value)
		{
			//	Mémorise la largeur à utiliser pour une colonne de la liste de gauche.
			if (this.designerApplication.DisplayModeState == DesignerApplication.DisplayMode.Horizontal)
			{
				Values.columnWidthHorizontal[column] = value;
			}
			else
			{
				Values.columnWidthVertical[column] = value;
			}
		}
	
	
		private static int CompareTypeColumns(object a, object b)
		{
			CultureMap itemA = a as CultureMap;
			CultureMap itemB = b as CultureMap;

			return itemA.Prefix.CompareTo(itemB.Prefix);
		}

		protected override bool HasDeleteOrDuplicate
		{
			get
			{
				//	Il n'est pas possible de créer ou de supprimer une ressource, puisque cela
				//	se fait depuis l'éditeur d'entités.
				return false;
			}
		}


		private static double[]				columnWidthHorizontal = { 110, 90, 100, 100, 20, 80, 50, 100, 50 };
		private static double[]				columnWidthVertical = { 180, 130, 270, 270, 20, 80, 50, 100, 50 };
	}
}
