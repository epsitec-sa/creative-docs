//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.UI;

using System.Collections.Generic;

[assembly: DependencyClass (typeof (Panel))]

namespace Epsitec.Common.UI
{
	/// <summary>
	/// The <c>Panel</c> class is used as the (local) root in a widget tree
	/// built by the dynamic user interface designer.
	/// </summary>
	public class Panel : Widgets.AbstractGroup
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
					this.dataSourceBinding = new Binding (this.dataSource);

					if (oldSource != null)
					{
						oldSource.Metadata = null;
					}
					if (newSource != null)
					{
						newSource.Metadata = this.dataSourceMetadata;
					}
					
					this.SyncDataContext ();
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
		/// Gets the panel mode for this panel.
		/// </summary>
		/// <value>The panel mode.</value>
		public virtual PanelMode				PanelMode
		{
			get
			{
				return PanelMode.Default;
			}
		}

		/// <summary>
		/// Gets the edition panel.
		/// </summary>
		/// <value>The edition panel.</value>
		public Panel							EditionPanel
		{
			get
			{
				return this.GetPanel (PanelMode.Edition);
			}
		}

		/// <summary>
		/// Gets the search panel.
		/// </summary>
		/// <value>The search panel.</value>
		public Panel							SearchPanel
		{
			get
			{
				return this.GetPanel (PanelMode.Search);
			}
		}

		/// <summary>
		/// Gets the panel for the specified panel mode.
		/// </summary>
		/// <param name="mode">The panel mode.</param>
		/// <returns>The panel for the specified panel mode.</returns>
		public virtual Panel GetPanel(PanelMode mode)
		{
			switch (mode)
			{
				case PanelMode.Edition:
					if (this.editionPanel == null)
					{
						this.editionPanel = new Panels.EditPanel (this);
						this.SyncDataContext (this.editionPanel);
					}

					return this.editionPanel;

				case PanelMode.Search:
					return null;

				case PanelMode.Default:
					return this;
			}
			
			return null;
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

		public static string SerializePanel(Panel panel)
		{
			DataSource              dataSource = panel.DataSource;
			Support.ResourceManager manager    = panel.ResourceManager;
			
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			System.IO.StringWriter stringWriter = new System.IO.StringWriter (buffer);
			System.Xml.XmlTextWriter xmlWriter = new System.Xml.XmlTextWriter (stringWriter);

			using (Types.Serialization.Context context = new Types.Serialization.SerializerContext (new Types.Serialization.IO.XmlWriter (xmlWriter)))
			{
				UI.Panel.FillSerializationContext (context, dataSource, manager);

				xmlWriter.Formatting = System.Xml.Formatting.None;
				xmlWriter.WriteStartElement ("panel");

				context.ActiveWriter.WriteAttributeStrings ();

				Types.Storage.Serialize (panel, context);

				xmlWriter.WriteEndElement ();
				xmlWriter.Flush ();
				xmlWriter.Close ();

				return buffer.ToString ();
			}
		}

		public static UI.Panel DeserializePanel(string xml, DataSource dataSource, Support.ResourceManager manager)
		{
			System.IO.StringReader stringReader = new System.IO.StringReader (xml);
			System.Xml.XmlTextReader xmlReader = new System.Xml.XmlTextReader (stringReader);

			xmlReader.Read ();

			System.Diagnostics.Debug.Assert (xmlReader.NodeType == System.Xml.XmlNodeType.Element);
			System.Diagnostics.Debug.Assert (xmlReader.LocalName == "panel");

			Types.Serialization.Context context = new Types.Serialization.DeserializerContext (new Types.Serialization.IO.XmlReader (xmlReader));

			UI.Panel.FillSerializationContext (context, dataSource, manager);

			UI.Panel panel = Types.Storage.Deserialize (context) as UI.Panel;

			System.Diagnostics.Debug.Assert (panel.DataSource == dataSource);
			System.Diagnostics.Debug.Assert (panel.ResourceManager == manager);

			return panel;
		}
		
		private void SyncDataContext()
		{
			this.SyncDataContext (this);
			this.SyncDataContext (this.editionPanel);
			//	...
		}

		private void SyncDataContext(Panel panel)
		{
			if (panel != null)
			{
				if (this.dataSourceBinding == null)
				{
					DataObject.ClearDataContext (panel);
				}
				else
				{
					DataObject.SetDataContext (panel, this.dataSourceBinding);
				}
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

		private static object GetSearchPanelValue(DependencyObject obj)
		{
			Panel panel = (Panel) obj;
			return panel.SearchPanel;
		}

		private static void SetSearchPanelValue(DependencyObject obj, object value)
		{
			Panel panel = (Panel) obj;
			//	TODO: searchPanel = ...
		}


		public static DependencyProperty DataSourceMetadataProperty = DependencyProperty.RegisterReadOnly ("DataSourceMetadata", typeof (DataSourceMetadata), typeof (Panel), new DependencyPropertyMetadata (Panel.GetDataSourceMetadataValue, Panel.SetDataSourceMetadataValue).MakeReadOnlySerializable ());
		public static DependencyProperty EditionPanelProperty = DependencyProperty.Register ("EditionPanel", typeof (Panels.EditPanel), typeof (Panel), new DependencyPropertyMetadata (Panel.GetEditionPanelValue, Panel.SetEditionPanelValue));
		public static DependencyProperty SearchPanelProperty  = DependencyProperty.Register ("SearchPanel", typeof (Panels.EditPanel), typeof (Panel), new DependencyPropertyMetadata (Panel.GetSearchPanelValue, Panel.SetSearchPanelValue));
		
		private DataSource dataSource;
		private Binding dataSourceBinding;
		private DataSourceMetadata dataSourceMetadata;
		private Panels.EditPanel editionPanel;
	}
}
