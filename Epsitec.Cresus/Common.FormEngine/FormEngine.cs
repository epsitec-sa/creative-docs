using System.Collections.Generic;

using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

namespace Epsitec.Common.FormEngine
{
	public class FormEngine
	{
		public FormEngine(ResourceManager resourceManager)
		{
			//	Constructeur.
			this.resourceManager = resourceManager;
		}

		public Widget CreateForm(Druid entityId)
		{
			//	Crée un masque de saisie pour une entité donnée.
			
			Caption        entityCaption = this.resourceManager.GetCaption (entityId);
			StructuredType entityType    = TypeRosetta.GetTypeObject (entityCaption) as StructuredType;

			StructuredData entityData = new StructuredData (entityType);

			UI.Panel root = new UI.Panel();
			
			root.ResourceManager = this.resourceManager;
			root.DataSource = new UI.DataSource ();
			root.DataSource.AddDataSource ("Data", entityData);

			Widgets.Layouts.GridLayoutEngine grid = new Widgets.Layouts.GridLayoutEngine ();

			grid.ColumnDefinitions.Add (new Widgets.Layouts.ColumnDefinition ());
			grid.ColumnDefinitions.Add (new Widgets.Layouts.ColumnDefinition (new Widgets.Layouts.GridLength (1, Widgets.Layouts.GridUnitType.Proportional), 60, double.PositiveInfinity));
			grid.ColumnDefinitions[0].RightBorder = 1;

			Widgets.Layouts.LayoutEngine.SetLayoutEngine (root, grid);
			
			int row = 0;

			foreach (string fieldId in entityType.GetFieldIds())
			{
				StructuredTypeField field = entityType.GetField(fieldId);
				Caption fieldCaption = this.resourceManager.GetCaption (field.CaptionId);

				UI.Placeholder placeholder = new Epsitec.Common.UI.Placeholder (root);
				placeholder.SetBinding (UI.Placeholder.ValueProperty, new Binding (BindingMode.TwoWay, "Data." + field.Id));
				placeholder.TabIndex = row;

				grid.RowDefinitions.Add (new Widgets.Layouts.RowDefinition ());
				grid.RowDefinitions[row].BottomBorder = 2;
				
				Widgets.Layouts.GridLayoutEngine.SetColumn (placeholder, 0);
				Widgets.Layouts.GridLayoutEngine.SetRow (placeholder, row);
				Widgets.Layouts.GridLayoutEngine.SetColumnSpan (placeholder, 2);

				row++;
			}

			return root;
		}


		ResourceManager resourceManager;
	}
}
