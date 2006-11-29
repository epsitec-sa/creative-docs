//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.UI;

using System.Collections.Generic;

[assembly: DependencyClass (typeof (TablePlaceholder))]

namespace Epsitec.Common.UI
{
	public class TablePlaceholder : AbstractPlaceholder, Types.Serialization.IDeserialization
	{
		public TablePlaceholder()
		{
		}

		public TablePlaceholder(Widgets.Widget embedder)
			: this ()
		{
			this.SetEmbedder (embedder);
		}

		#region IDeserialization Members

		void Epsitec.Common.Types.Serialization.IDeserialization.NotifyDeserializationStarted(Epsitec.Common.Types.Serialization.Context context)
		{
			this.suspendUserInterfaceCreation++;
		}

		void Epsitec.Common.Types.Serialization.IDeserialization.NotifyDeserializationCompleted(Epsitec.Common.Types.Serialization.Context context)
		{
			this.suspendUserInterfaceCreation--;

			this.RecreateUserInterface ();
		}

		#endregion

		private void DisposeUserInterface()
		{
			if (this.table != null)
			{
				this.table.SetParent (null);
				this.table.Dispose ();
				this.table = null;
			}
		}

		private void CreateUserInterface()
		{
			if ((this.suspendUserInterfaceCreation == 0) &&
				(this.ValueType is ICollectionType))
			{
				Support.ResourceManager manager = Widgets.Helpers.VisualTree.FindResourceManager (this);

				if (manager == null)
				{
					throw new System.InvalidOperationException ("Cannot create user interface: ResourceManager is undefined (add the PanelPlaceholder into a valid Panel)");
				}

#if false
				Support.ResourceBundle bundle = manager.GetBundle (this.PanelId);

				if (bundle == null)
				{
					throw new System.InvalidOperationException (string.Format ("Cannot create user interface: bundle {0} not found", this.PanelId));
				}

				Support.ResourceBundle.Field field = bundle["Panel"];
				string xml;

				if (field.IsEmpty)
				{
					Panel cachedPanel = Panel.GetPanel (bundle);

					if (cachedPanel == null)
					{
						throw new System.InvalidOperationException (string.Format ("Cannot create user interface: panel field for bundle {0} not found", this.PanelId));
					}

					xml = Panel.SerializePanel (cachedPanel);
				}
				else
				{
					xml = field.AsString;
				}

				this.DefinePanel (Panel.DeserializePanel (xml, null, manager));
#endif
			}
		}

		private void RecreateUserInterface()
		{
			this.DisposeUserInterface ();
			this.CreateUserInterface ();
		}
		
		private void HandleVisibleColumnIdsChanged(string oldValue, string newValue)
		{
			//	TODO: ...
		}


		private static void NotifyVisibleColumnIdsChanged(DependencyObject o, object oldValue, object newValue)
		{
			TablePlaceholder placeholder = (TablePlaceholder) o;
			placeholder.HandleVisibleColumnIdsChanged ((string) oldValue, (string) newValue);
		}

		public static readonly DependencyProperty ColumnDefinitionsProperty = DependencyProperty.Register ("ColumnDefinitions", typeof (string), typeof (TablePlaceholder), new DependencyPropertyMetadata (TablePlaceholder.NotifyVisibleColumnIdsChanged));

		private ItemTable table;
		private int suspendUserInterfaceCreation;
	}
}
