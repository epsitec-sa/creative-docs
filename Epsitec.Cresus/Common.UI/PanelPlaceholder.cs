//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.UI;

using System.Collections.Generic;

[assembly: DependencyClass (typeof (PanelPlaceholder))]

namespace Epsitec.Common.UI
{
	public class PanelPlaceholder : AbstractPlaceholder, Types.Serialization.IDeserialization
	{
		public PanelPlaceholder()
		{
		}

		public PanelPlaceholder(Widgets.Widget embedder)
			: this ()
		{
			this.SetEmbedder (embedder);
		}

		public Support.ResourceManager ResourceManager
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

		public Support.Druid PanelId
		{
			get
			{
				return (Support.Druid) this.GetValue (PanelPlaceholder.PanelIdProperty);
			}
			set
			{
				this.SetValue (PanelPlaceholder.PanelIdProperty, value);
			}
		}

		/// <summary>
		/// Refreshes the panel by reloading it from the resources and then
		/// regenerating all its widgets.
		/// </summary>
		public void RefreshPanel()
		{
			this.RecreateUserInterface ();
		}

		#region IDeserialization Members

		void Types.Serialization.IDeserialization.NotifyDeserializationStarted(Types.Serialization.Context context)
		{
			this.suspendUserInterfaceCreation++;
		}

		void Types.Serialization.IDeserialization.NotifyDeserializationCompleted(Types.Serialization.Context context)
		{
			this.suspendUserInterfaceCreation--;

			this.RecreateUserInterface ();
		}

		#endregion

		private void DisposeUserInterface()
		{
			if (this.panel != null)
			{
				this.panel.SetParent (null);
				this.panel.Dispose ();
				this.panel = null;
			}
		}

		private void CreateUserInterface()
		{
			if ((this.suspendUserInterfaceCreation == 0) &&
				(this.PanelId.IsValid))
			{
				Support.ResourceManager manager = Widgets.Helpers.VisualTree.FindResourceManager (this);

				if (manager == null)
				{
					throw new System.InvalidOperationException ("Cannot create user interface: ResourceManager is undefined (add the PanelPlaceholder into a valid Panel)");
				}

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

				this.panel = Panel.DeserializePanel (xml, null, manager);
				this.panel.SetEmbedder (this);
				this.panel.Dock = Widgets.DockStyle.Fill;
			}
		}

		private void RecreateUserInterface()
		{
			this.DisposeUserInterface ();
			this.CreateUserInterface ();
		}


		private static void NotifyPanelIdChanged(DependencyObject obj, object oldValue, object newValue)
		{
			PanelPlaceholder placeholder = (PanelPlaceholder) obj;
			
			placeholder.RecreateUserInterface ();
		}
		
		public static readonly DependencyProperty PanelIdProperty = DependencyProperty.Register ("PanelId", typeof (Support.Druid), typeof (PanelPlaceholder), new DependencyPropertyMetadata (Support.Druid.Empty, PanelPlaceholder.NotifyPanelIdChanged));

		private int suspendUserInterfaceCreation;
		private Panel panel;
	}
}
