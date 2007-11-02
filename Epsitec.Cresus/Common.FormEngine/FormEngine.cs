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

			foreach (string fieldId in entityType.GetFieldIds())
			{
				StructuredTypeField field = entityType.GetField(fieldId);
				Caption fieldCaption = this.resourceManager.GetCaption (field.CaptionId);

				UI.Placeholder placeholder = new Epsitec.Common.UI.Placeholder (root);
				placeholder.Dock = DockStyle.Stacked;
				placeholder.SetBinding (UI.Placeholder.ValueProperty, new Binding (BindingMode.TwoWay, "Data." + field.Id));
			}

			return root;
		}


		ResourceManager resourceManager;
	}
}
