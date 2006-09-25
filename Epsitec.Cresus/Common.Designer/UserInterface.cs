using System;
using System.Collections.Generic;
using System.Text;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;

namespace Epsitec.Common.Designer
{
	public class UserInterface
	{
		public static UI.Panel CreateEmptyPanel()
		{
			//	Crée un panneau "UI" vide pour expérimenter.

			UI.Panel panel = new Epsitec.Common.UI.Panel();
			UI.DataSourceCollection sources = new Epsitec.Common.UI.DataSourceCollection();

			CustomerRecord customer = new CustomerRecord();

			customer.SetValue(CustomerRecord.NameProperty, "Arnaud");
			customer.SetValue(CustomerRecord.SurameProperty, "Pierre");
			customer.SetValue(CustomerRecord.AddressProperty, "Ch. du Fontenay 6");
			customer.SetValue(CustomerRecord.PostalNumberProperty, 1400);
			customer.SetValue(CustomerRecord.CityProperty, "Yverdon-les-Bains");

			sources.AddDataSource("Customer", customer);
			
			panel.DataSource = sources;

			return panel;
		}

		public static string SerializePanel(UI.Panel panel, ResourceManager manager)
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder();
			System.IO.StringWriter stringWriter = new System.IO.StringWriter(buffer);
			System.Xml.XmlTextWriter xmlWriter = new System.Xml.XmlTextWriter(stringWriter);

			using (Types.Serialization.Context context = new Types.Serialization.SerializerContext (new Types.Serialization.IO.XmlWriter (xmlWriter)))
			{
				UI.Panel.FillSerializationContext (context, null, manager);
				
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

		public static UI.Panel DeserializePanel(string xml, ResourceManager manager)
		{
			System.IO.StringReader stringReader = new System.IO.StringReader(xml);
			System.Xml.XmlTextReader xmlReader = new System.Xml.XmlTextReader(stringReader);

			xmlReader.Read();
			
			System.Diagnostics.Debug.Assert(xmlReader.NodeType == System.Xml.XmlNodeType.Element);
			System.Diagnostics.Debug.Assert(xmlReader.LocalName == "panel");

			Types.Serialization.Context context = new Types.Serialization.DeserializerContext(new Types.Serialization.IO.XmlReader(xmlReader));

			UI.Panel.FillSerializationContext (context, null, manager);
			
			UI.Panel panel = Types.Storage.Deserialize(context) as UI.Panel;
			
			System.Diagnostics.Debug.Assert(panel.ResourceManager == manager);
			
			return panel;
		}

		public static void RunPanel(UI.Panel panel, ResourceManager manager, string name)
		{
			string xml = UserInterface.SerializePanel (panel, manager);
			UI.Panel clone = UserInterface.DeserializePanel (xml, manager);
			Widgets.Window window = new Epsitec.Common.Widgets.Window ();

			window.MakeSecondaryWindow ();
			window.ShowWindowIcon = false;
			window.Root.Children.Add (clone);

			clone.Dock = Widgets.DockStyle.Fill;
			window.Owner = panel.Window;
			window.Text = name;

			window.ForceLayout ();

			double width  = Widgets.Layouts.LayoutMeasure.GetWidth (clone).Desired;
			double height = Widgets.Layouts.LayoutMeasure.GetHeight (clone).Desired;

			window.ClientSize = new Size (width, height);

			double dx = window.WindowSize.Width;
			double dy = window.WindowSize.Height;  // taille avec le cadre

			Point center = UserInterface.runPanelCenter;
			if (center.IsZero)
			{
				center = panel.Window.WindowBounds.Center;
			}
			Rectangle rect = new Rectangle (center.X-dx/2, center.Y-dy/2, dx, dy);
			window.WindowBounds = rect;

			window.ShowDialog ();  // affiche le dialogue modal...
			
			UserInterface.runPanelCenter = window.WindowPlacementNormalBounds.Center;
		}
		
		private class CustomerRecord : Types.DependencyObject
		{
			//	Class "bidon" pour avoir au moins un source de données disponible.
			
			public CustomerRecord()
			{
			}
			
			public static readonly Types.DependencyProperty NameProperty = Types.DependencyProperty.Register("Name", typeof(string), typeof(CustomerRecord));
			public static readonly Types.DependencyProperty SurameProperty = Types.DependencyProperty.Register("Surname", typeof(string), typeof(CustomerRecord));
			public static readonly Types.DependencyProperty AddressProperty = Types.DependencyProperty.Register("Address", typeof(string), typeof(CustomerRecord));
			public static readonly Types.DependencyProperty PostalNumberProperty = Types.DependencyProperty.Register("PostalNumber", typeof(int), typeof(CustomerRecord));
			public static readonly Types.DependencyProperty CityProperty = Types.DependencyProperty.Register("City", typeof(string), typeof(CustomerRecord));

		}


		protected static Point			runPanelCenter = Point.Zero;
	}
}
