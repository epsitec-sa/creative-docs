//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;
using Epsitec.Common.Types;

[assembly: Epsitec.Common.Types.DependencyClass (typeof (Epsitec.Common.UI.Panel))]

namespace Epsitec.Common.UI
{
	/// <summary>
	/// The <c>Panel</c> class is used as the (local) root in a widget tree
	/// built by the dynamic user interface designer.
	/// </summary>
	public class Panel : Widgets.AbstractGroup, Types.Serialization.IDeserialization
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="T:Panel"/> class.
		/// </summary>
		public Panel()
		{
		}

		/// <summary>
		/// Gets or sets the <see cref="DataSource"/> used as the root data context.
		/// </summary>
		/// <value>The data source.</value>
		public DataSource						DataSource
		{
			get
			{
				return this.dataSource;
			}
			set
			{
				DataSource oldSource = this.dataSource;
				DataSource newSource = value;
				
				if (oldSource != newSource)
				{
					this.dataSource = newSource;

					if (oldSource != null)
					{
						oldSource.Metadata = null;
					}
					if (newSource != null)
					{
						newSource.Metadata = this.dataSourceMetadata;
					}
					
					this.SyncDataContextWithDataSource ();
				}
			}
		}

		/// <summary>
		/// Gets the metadata associated with the <see cref="DataSource"/>.
		/// </summary>
		/// <value>The metadata associated with the data source.</value>
		public DataSourceMetadata				DataSourceMetadata
		{
			get
			{
				if (this.dataSourceMetadata == null)
				{
					this.dataSourceMetadata = new DataSourceMetadata ();

					if (this.dataSource != null)
					{
						this.dataSource.Metadata = this.dataSourceMetadata;
					}
				}
				
				return this.dataSourceMetadata;
			}
		}

		/// <summary>
		/// Gets or sets the associated resource manager.
		/// </summary>
		/// <value>The resource manager.</value>
		public Support.ResourceManager			ResourceManager
		{
			get
			{
				return Support.ResourceManager.GetResourceManager (this);
			}
			set
			{
				Support.ResourceManager.SetResourceManager (this, value);
			}
		}

		/// <summary>
		/// Gets the owner of this panel. This is overridden by <c>EditPanel</c>
		/// and the other specialized panels.
		/// </summary>
		/// <value>The owner (always <c>null</c> if not overridden).</value>
		public virtual Panel					Owner
		{
			get
			{
				return null;
			}
		}

		/// <summary>
		/// Gets or sets the panel mode.
		/// </summary>
		/// <value>The panel mode.</value>
		public PanelMode						PanelMode
		{
			get
			{
				return (PanelMode) this.GetValue (Panel.PanelModeProperty);
			}
			set
			{
				this.SetValue (Panel.PanelModeProperty, value);
			}
		}

		public virtual Panel					EditionPanel
		{
			get
			{
				if (this.editionPanel == null)
				{
					this.editionPanel = new Panels.EditPanel (this);
					DataObject.SetDataContext (this.editionPanel, DataObject.GetDataContext (this));
				}

				return this.editionPanel;
			}
		}
		
		/// <summary>
		/// Fills the serialization context <c>ExternalMap</c> property.
		/// </summary>
		/// <param name="context">The serialization context.</param>
		/// <param name="dataSource">The data source.</param>
		/// <param name="resourceManager">The resource manager.</param>
		public static void FillSerializationContext(Types.Serialization.Context context, DataSource dataSource, Support.ResourceManager resourceManager)
		{
			context.ExternalMap.Record (Types.Serialization.Context.WellKnownTagDataSource, dataSource);
			context.ExternalMap.Record (Types.Serialization.Context.WellKnownTagResourceManager, resourceManager);
		}

		#region IDeserialization Members

		void Types.Serialization.IDeserialization.NotifyDeserializationStarted(Types.Serialization.Context context)
		{
			this.PanelMode = PanelMode.None;
		}

		void Types.Serialization.IDeserialization.NotifyDeserializationCompleted(Types.Serialization.Context context)
		{
			this.PanelMode = PanelMode.Default;
		}

		#endregion

		private void SyncDataContextWithDataSource()
		{
			if (this.dataSource == null)
			{
				DataObject.ClearDataContext (this);
			}
			else
			{
				DataObject.SetDataContext (this, new Binding (this.dataSource));
			}
			
			if (this.editionPanel != null)
			{
				DataObject.SetDataContext (this.editionPanel, DataObject.GetDataContext (this));
			}
		}

		private void HandlePanelModeChanged(PanelMode oldMode, PanelMode newMode)
		{
		}

		private static object GetDataSourceMetadataValue(DependencyObject obj)
		{
			Panel panel = (Panel) obj;
			return panel.DataSourceMetadata;
		}

		private static void SetDataSourceMetadataValue(DependencyObject obj, object value)
		{
			Panel panel = (Panel) obj;
			panel.dataSourceMetadata = (DataSourceMetadata) value;
		}

		private static void NotifyPanelModeChanged(DependencyObject obj, object oldValue, object newValue)
		{
			Panel panel = (Panel) obj;
			panel.HandlePanelModeChanged ((PanelMode) oldValue, (PanelMode) newValue);
		}

		private static object GetEditionPanelValue(DependencyObject obj)
		{
			Panel panel = (Panel) obj;
			return panel.EditionPanel;
		}

		private static void SetEditionPanelValue(DependencyObject obj, object value)
		{
			Panel panel = (Panel) obj;
			panel.editionPanel = (Panels.EditPanel) value;
		}


		public static DependencyProperty DataSourceMetadataProperty = DependencyProperty.RegisterReadOnly ("DataSourceMetadata", typeof (DataSourceMetadata), typeof (Panel), new DependencyPropertyMetadata (Panel.GetDataSourceMetadataValue, Panel.SetDataSourceMetadataValue).MakeReadOnlySerializable ());
		public static DependencyProperty PanelModeProperty = DependencyProperty.Register ("PanelMode", typeof (PanelMode), typeof (Panel), new DependencyPropertyMetadata (PanelMode.Default, Panel.NotifyPanelModeChanged));
		public static DependencyProperty EditionPanelProperty = DependencyProperty.Register ("EditionPanel", typeof (Panels.EditPanel), typeof (Panel), new DependencyPropertyMetadata (Panel.GetEditionPanelValue, Panel.SetEditionPanelValue));
		
		private DataSource dataSource;
		private DataSourceMetadata dataSourceMetadata;
		private Panels.EditPanel editionPanel;
	}
}
