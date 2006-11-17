//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

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
	public class Panel : Widgets.FrameBox
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="T:Panel"/> class.
		/// </summary>
		public Panel()
		{
			base.TabNavigationMode = Widgets.TabNavigationMode.ForwardTabActive;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Panel"/> class.
		/// </summary>
		/// <param name="embedder">The embedder.</param>
		public Panel(Widgets.Widget embedder)
			: this ()
		{
			this.SetEmbedder (embedder);
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
				System.Diagnostics.Debug.Assert (this.PanelMode == PanelMode.Default);
				
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

					DataObject.SetDataContext (this, this.dataSourceBinding);
				}
			}
		}

		/// <summary>
		/// Gets the metadata associated with the <see cref="DataSource"/>.
		/// </summary>
		/// <value>The metadata associated with the data source.</value>
		public virtual DataSourceMetadata		DataSourceMetadata
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

		public bool								HasValidEditionPanel
		{
			get
			{
				if ((this.PanelMode == PanelMode.Default) &&
					(this.editionPanel != null) &&
					(this.editionPanel.HasChildren))
				{
					return true;
				}
				else
				{
					return false;
				}
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
						this.SetupDataContextChangeHandler ();
						
						this.editionPanel = new Panels.EditPanel (this);
						
						this.editionPanel.ChildrenLayoutMode = this.ChildrenLayoutMode;
						this.editionPanel.ContainerLayoutMode = this.ContainerLayoutMode;
						this.editionPanel.PreferredSize = this.PreferredSize;
						this.editionPanel.Anchor = this.Anchor;
						this.editionPanel.Dock = this.Dock;
						this.editionPanel.Margins = this.Margins;
						this.editionPanel.Padding = this.Padding;
						
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

		/// <summary>
		/// Serializes the specified panel.
		/// </summary>
		/// <param name="panel">The panel.</param>
		/// <returns>An XML representation of the panel.</returns>
		public static string SerializePanel(Panel panel)
		{
			DataSource              dataSource = panel.DataSource;
			Support.ResourceManager manager    = panel.ResourceManager;
			
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			System.IO.StringWriter stringWriter = new System.IO.StringWriter (buffer);
			System.Xml.XmlTextWriter xmlWriter = new System.Xml.XmlTextWriter (stringWriter);

			using (Types.Serialization.Context context = new Types.Serialization.SerializerContext (new Types.Serialization.IO.XmlWriter (xmlWriter)))
			{
				Panel.FillSerializationContext (context, dataSource, manager);

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

		/// <summary>
		/// Deserializes the panel from an XML representation.
		/// </summary>
		/// <param name="xml">The XML representation.</param>
		/// <param name="dataSource">The data source (or <c>null</c>).</param>
		/// <param name="manager">The resource manager.</param>
		/// <returns>A live <c>Panel</c>.</returns>
		public static Panel DeserializePanel(string xml, DataSource dataSource, Support.ResourceManager manager)
		{
			System.IO.StringReader stringReader = new System.IO.StringReader (xml);
			System.Xml.XmlTextReader xmlReader = new System.Xml.XmlTextReader (stringReader);

			xmlReader.Read ();

			System.Diagnostics.Debug.Assert (xmlReader.NodeType == System.Xml.XmlNodeType.Element);
			System.Diagnostics.Debug.Assert (xmlReader.LocalName == "panel");

			Types.Serialization.Context context = new Types.Serialization.DeserializerContext (new Types.Serialization.IO.XmlReader (xmlReader));

			Panel.FillSerializationContext (context, dataSource, manager);

			Panel panel = Types.Storage.Deserialize (context) as Panel;

			System.Diagnostics.Debug.Assert (panel.DataSource == dataSource);
			System.Diagnostics.Debug.Assert (panel.ResourceManager == manager);

			return panel;
		}

		protected override bool PreProcessMessage(Widgets.Message message, Drawing.Point pos)
		{
			if (this.HasValidEditionPanel)
			{
				Widgets.Window window = this.Window;
				PanelStack panelStack = PanelStack.GetPanelStack (this);

				if ((panelStack != null) &&
					(window != null))
				{
					if (message.MessageType == Widgets.MessageType.MouseLeave)
					{
						window.MouseCursor = Widgets.MouseCursor.Default;
					}
					else if (message.IsMouseType)
					{
						window.MouseCursor = Widgets.MouseCursor.AsIBeam;
					}
					else if (message.IsKeyType)
					{
						Widgets.IFeel feel = Widgets.Feel.Factory.Active;

						if (feel.TestSelectItemKey (message))
						{
							this.StartEdition (message, null, panelStack);
						}
						else if (feel.TestNavigationKey (message))
						{
							return base.PreProcessMessage (message, pos);
						}
					}

					if (message.Button == Widgets.MouseButtons.Left)
					{
						if (message.MessageType == Widgets.MessageType.MouseDown)
						{
							this.isMouseDown = true;
						}
						else if (message.MessageType == Widgets.MessageType.MouseUp)
						{
							if (this.isMouseDown)
							{
								this.isMouseDown = false;
								this.StartEdition (message, this.FindChild (pos, ChildFindMode.Deep | ChildFindMode.SkipHidden | ChildFindMode.SkipDisabled), panelStack);
							}
						}
					}

					return false;
				}
			}
			
			return base.PreProcessMessage (message, pos);
		}

		public override Widgets.TabNavigationMode TabNavigationMode
		{
			get
			{
				if (this.HasValidEditionPanel)
				{
					return base.TabNavigationMode & ~Widgets.TabNavigationMode.ForwardToChildren & ~Widgets.TabNavigationMode.ForwardOnly;
				}
				else
				{
					return base.TabNavigationMode;
				}
			}
			set
			{
				base.TabNavigationMode = value;
			}
		}

		private void StartEdition(Widgets.Message message, Widgets.Widget focusWidget, PanelStack panelStack)
		{
			string focusWidgetName = null;

			if ((focusWidget != null) &&
				(focusWidget != this))
			{
				focusWidgetName = focusWidget.Name;
			}

			panelStack.StartEdition (this.EditionPanel, focusWidgetName);

			if (message != null)
			{
				message.Swallowed = true;
				message.Consumer  = this;
			}
		}

		protected override void PaintBackgroundImplementation(Epsitec.Common.Drawing.Graphics graphics, Epsitec.Common.Drawing.Rectangle clipRect)
		{
			base.PaintBackgroundImplementation (graphics, clipRect);

			if ((this.PaintState & Epsitec.Common.Widgets.WidgetPaintState.Focused) != 0)
			{
				Drawing.Rectangle rect = this.Client.Bounds;
				
				rect.Deflate (1.5);
				
				graphics.AddRectangle (rect);
				graphics.RenderSolid (Widgets.Adorners.Factory.Active.ColorCaption);
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.isDataContextChangeHandlerRegistered)
				{
					this.RemoveEventHandler (DataObject.DataContextProperty, this.HandleDataContextChanged);
					this.isDataContextChangeHandlerRegistered = false;
				}

				if (this.PanelMode == PanelMode.Default)
				{
					if (this.editionPanel != null)
					{
						this.editionPanel.Dispose ();
					}
				}

				this.editionPanel = null;
			}

			base.Dispose (disposing);
		}
		
		private void SyncDataContext()
		{
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

		private void SetupDataContextChangeHandler()
		{
			if (this.isDataContextChangeHandlerRegistered == false)
			{
				this.AddEventHandler (DataObject.DataContextProperty, this.HandleDataContextChanged);
				this.isDataContextChangeHandlerRegistered = true;
			}
		}

		private void HandleDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			this.dataSourceBinding = DataObject.GetDataContext (this);
			this.SyncDataContext ();
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

		public static void SetPanel(DependencyObject obj, Panel panel)
		{
			if (panel == null)
			{
				obj.ClearValue (Panel.PanelProperty);
			}
			else
			{
				obj.SetValue (Panel.PanelProperty, panel);
			}
		}

		public static Panel GetPanel(DependencyObject obj)
		{
			return (Panel) obj.GetValue (Panel.PanelProperty);
		}

		public static readonly DependencyProperty PanelProperty = DependencyProperty.RegisterAttached ("Panel", typeof (Panel), typeof (Panel));
		public static readonly DependencyProperty DataSourceMetadataProperty = DependencyProperty.RegisterReadOnly ("DataSourceMetadata", typeof (DataSourceMetadata), typeof (Panel), new DependencyPropertyMetadata (Panel.GetDataSourceMetadataValue, Panel.SetDataSourceMetadataValue).MakeReadOnlySerializable ());
		public static readonly DependencyProperty EditionPanelProperty = DependencyProperty.Register ("EditionPanel", typeof (Panels.EditPanel), typeof (Panel), new DependencyPropertyMetadata (Panel.GetEditionPanelValue, Panel.SetEditionPanelValue));
		public static readonly DependencyProperty SearchPanelProperty  = DependencyProperty.Register ("SearchPanel", typeof (Panels.EditPanel), typeof (Panel), new DependencyPropertyMetadata (Panel.GetSearchPanelValue, Panel.SetSearchPanelValue));
		
		private DataSource dataSource;
		private Binding dataSourceBinding;
		private DataSourceMetadata dataSourceMetadata;
		private Panels.EditPanel editionPanel;
		private bool isDataContextChangeHandlerRegistered;
		private bool isMouseDown;
	}
}
