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

		public IEnumerable<Widgets.Visual> EditVisuals
		{
			get
			{
				if (this.PanelMode == PanelMode.Edition)
				{
					return this.Children;
				}
				else
				{
					if (this.editionVisuals == null)
					{
						this.editionVisuals = new Types.Collections.HostedDependencyObjectList<Widgets.Visual> (null);
					}

					return this.editionVisuals;
				}
			}
		}

		public IEnumerable<Widgets.Visual> DefaultVisuals
		{
			get
			{
				if (this.PanelMode == PanelMode.Default)
				{
					return this.Children;
				}
				else
				{
					if (this.defaultVisuals == null)
					{
						this.defaultVisuals = new Types.Collections.HostedDependencyObjectList<Widgets.Visual> (null);
					}

					return this.defaultVisuals;
				}
			}
		}

		public Drawing.Size EditionPreferredSize
		{
			get
			{
				if (this.PanelMode == PanelMode.Edition)
				{
					return this.PreferredSize;
				}
				else
				{
					return this.editionPreferredSize;
				}
			}
		}

		public Drawing.Size DefaultPreferredSize
		{
			get
			{
				if (this.PanelMode == PanelMode.Default)
				{
					return this.PreferredSize;
				}
				else
				{
					return this.defaultPreferredSize;
				}
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
		}

		private void HandlePanelModeChanged(PanelMode oldMode, PanelMode newMode)
		{
			switch (oldMode)
			{
				case PanelMode.None:
					return;
					
				case PanelMode.Default:
					this.defaultVisuals = new Types.Collections.HostedDependencyObjectList<Widgets.Visual> (null);
					this.defaultVisuals.AddRange (this.Children);
					this.defaultPreferredSize = this.PreferredSize;
					break;
				
				case PanelMode.Edition:
					this.editionVisuals = new Types.Collections.HostedDependencyObjectList<Widgets.Visual> (null);
					this.editionVisuals.AddRange (this.Children);
					this.editionPreferredSize = this.PreferredSize;
					break;
			}

			this.Children.Clear ();
			
			switch (newMode)
			{
				case PanelMode.Default:
					this.Children.AddRange (this.defaultVisuals);
					this.PreferredSize = this.defaultPreferredSize;
					break;

				case PanelMode.Edition:
					this.Children.AddRange (this.editionVisuals);
					this.PreferredSize = this.editionPreferredSize;
					break;
			}
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

		private static object GetEditVisualsValue(DependencyObject obj)
		{
			Panel panel = (Panel) obj;
			return panel.EditVisuals;
		}

		private static object GetDefaultVisualsValue(DependencyObject obj)
		{
			Panel panel = (Panel) obj;
			return panel.DefaultVisuals;
		}

		private static object GetEditionPreferredSizeValue(DependencyObject obj)
		{
			Panel panel = (Panel) obj;
			return panel.EditionPreferredSize;
		}

		private static void SetEditionPreferredSizeValue(DependencyObject obj, object value)
		{
			Panel panel = (Panel) obj;
			panel.editionPreferredSize = (Drawing.Size) value;
		}

		private static object GetDefaultPreferredSizeValue(DependencyObject obj)
		{
			Panel panel = (Panel) obj;
			return panel.DefaultPreferredSize;
		}

		private static void SetDefaultPreferredSizeValue(DependencyObject obj, object value)
		{
			Panel panel = (Panel) obj;
			panel.defaultPreferredSize = (Drawing.Size) value;
		}

		public static DependencyProperty DataSourceMetadataProperty = DependencyProperty.RegisterReadOnly ("DataSourceMetadata", typeof (DataSourceMetadata), typeof (Panel), new DependencyPropertyMetadata (Panel.GetDataSourceMetadataValue, Panel.SetDataSourceMetadataValue).MakeReadOnlySerializable ());
		public static DependencyProperty PanelModeProperty = DependencyProperty.Register ("PanelMode", typeof (PanelMode), typeof (Panel), new DependencyPropertyMetadata (PanelMode.Default, Panel.NotifyPanelModeChanged));
		public static DependencyProperty EditVisualsProperty = DependencyProperty.RegisterReadOnly ("EditVisuals", typeof (ICollection<DependencyObject>), typeof (Panel), new DependencyPropertyMetadata (Panel.GetEditVisualsValue).MakeReadOnlySerializable ());
		public static DependencyProperty DefaultVisualsProperty = DependencyProperty.RegisterReadOnly ("DefaultVisuals", typeof (ICollection<DependencyObject>), typeof (Panel), new DependencyPropertyMetadata (Panel.GetDefaultVisualsValue).MakeReadOnlySerializable ());
		public static DependencyProperty EditionPreferredSizeProperty = DependencyProperty.Register ("EditionPreferredSize", typeof (Drawing.Size), typeof (Panel), new DependencyPropertyMetadata (Panel.GetEditionPreferredSizeValue, Panel.SetEditionPreferredSizeValue));
		public static DependencyProperty DefaultPreferredSizeProperty = DependencyProperty.Register ("DefaultPreferredSize", typeof (Drawing.Size), typeof (Panel), new DependencyPropertyMetadata (Panel.GetDefaultPreferredSizeValue, Panel.SetDefaultPreferredSizeValue));

		private DataSource dataSource;
		private DataSourceMetadata dataSourceMetadata;
		private Types.Collections.HostedDependencyObjectList<Widgets.Visual> defaultVisuals;
		private Types.Collections.HostedDependencyObjectList<Widgets.Visual> editionVisuals;
		private Drawing.Size defaultPreferredSize;
		private Drawing.Size editionPreferredSize;
	}
}
