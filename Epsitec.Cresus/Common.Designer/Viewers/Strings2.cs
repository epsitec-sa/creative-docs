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
	public class Strings2 : Abstract
	{
		public Strings2(Module module, PanelsContext context, ResourceAccess access, MainWindow mainWindow) : base(module, context, access, mainWindow)
		{
			this.accessor = new Support.ResourceAccessors.StringResourceAccessor();
			accessor.Load(access.ResourceManager);

			StructuredType cultureMapType = new StructuredType();
			cultureMapType.Fields.Add("Name", StringType.Default);

			this.collectionView = new CollectionView(accessor.Collection);

			
			this.primaryCulture = new IconButtonMark(this);
			this.primaryCulture.Text = "?";
			this.primaryCulture.ButtonStyle = ButtonStyle.ActivableIcon;
			this.primaryCulture.SiteMark = ButtonMarkDisposition.Below;
			this.primaryCulture.MarkDimension = 5;
			this.primaryCulture.ActiveState = ActiveState.Yes;
			this.primaryCulture.AutoFocus = false;
			this.primaryCulture.Dock = DockStyle.Top;
			this.primaryCulture.Margins = new Drawing.Margins(10, 10, 10, 0);

			this.table = new UI.ItemTable(this);
			this.table.Dock = Widgets.DockStyle.Fill;
			this.table.Margins = new Drawing.Margins(10, 10, 0, 10);
			this.table.SourceType = cultureMapType;
			this.table.Items = this.collectionView;
			this.table.Columns.Add(new UI.ItemTableColumn("Name"));
			this.table.Columns.Add(new UI.ItemTableColumn("Primary"));
			this.table.Columns.Add(new UI.ItemTableColumn("Secondary"));
			this.table.HorizontalScrollMode = UI.ItemTableScrollMode.None;
			this.table.VerticalScrollMode = UI.ItemTableScrollMode.ItemBased;
			this.table.HeaderVisibility = true;
			this.table.FrameVisibility = false;
			this.table.ItemPanel.Layout = UI.ItemPanelLayout.VerticalList;
			this.table.ItemPanel.ItemSelectionMode = UI.ItemPanelSelectionMode.ExactlyOne;
			this.table.SizeChanged += this.HandleTableSizeChanged;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.table.SizeChanged -= this.HandleTableSizeChanged;
			}

			base.Dispose(disposing);
		}


		public override ResourceAccess.Type ResourceType
		{
			get
			{
				return ResourceAccess.Type.Strings2;
			}
		}

		
		public override void Update()
		{
			//	Met à jour le contenu du Viewer.
			this.UpdateArray();
			this.UpdateEdit();
			this.UpdateModificationsCulture();
			this.UpdateCommands();
		}

		protected override void UpdateArray()
		{
			//	Met à jour tout le contenu du tableau.
			//	Version spéciale pour les trois colonnes de Strings.
			this.array.TotalRows = this.access.AccessCount;

			int first = this.array.FirstVisibleRow;
			for (int i=0; i<this.array.LineCount; i++)
			{
				if (first+i < this.access.AccessCount)
				{
					this.UpdateArrayField(0, first+i, null, ResourceAccess.FieldType.Name);
					this.UpdateArrayField(1, first+i, null, ResourceAccess.FieldType.String);
					this.UpdateArrayField(2, first+i, this.secondaryCulture, (this.secondaryCulture == null) ? ResourceAccess.FieldType.None : ResourceAccess.FieldType.String);
				}
				else
				{
					this.UpdateArrayField(0, first+i, null, ResourceAccess.FieldType.None);
					this.UpdateArrayField(1, first+i, null, ResourceAccess.FieldType.None);
					this.UpdateArrayField(2, first+i, null, ResourceAccess.FieldType.None);
				}
			}

			this.array.SelectedRow = this.access.AccessIndex;
		}

		
		private void HandleTableSizeChanged(object sender, Epsitec.Common.Types.DependencyPropertyChangedEventArgs e)
		{
			UI.ItemTable table = (UI.ItemTable) sender;
			Drawing.Size size = (Drawing.Size) e.NewValue;

			double width = size.Width - table.GetPanelPadding().Width;
			table.ColumnHeader.SetColumnWidth(0, width);

			table.ItemPanel.ItemViewDefaultSize = new Epsitec.Common.Drawing.Size(width, 20);
		}




		protected Support.ResourceAccessors.StringResourceAccessor accessor;
		protected UI.ItemTable table;
		protected CollectionView collectionView;
	}
}
