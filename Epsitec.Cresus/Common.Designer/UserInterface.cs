using Epsitec.Common.Drawing;
using Epsitec.Common.Support;

using System.Collections.Generic;

namespace Epsitec.Common.Designer
{
	public class UserInterface
	{
		public static UI.Panel CreateEmptyPanel()
		{
			//	Crée un panneau "UI" vide pour expérimenter.

			var panel = new Epsitec.Common.UI.Panel();
			var sources = new Epsitec.Common.Types.DataSource();

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
			System.Diagnostics.Debug.Assert(manager == panel.ResourceManager);
			return UI.Panel.SerializePanel(panel);
		}

		public static UI.Panel DeserializePanel(string xml, ResourceManager manager)
		{
			UI.Panel panel = UI.Panel.DeserializePanel(xml, null, manager);
			System.Diagnostics.Debug.Assert(manager == panel.ResourceManager);
			return panel;
		}

		/// <summary>
		/// Duplicates the specified user interface object (this can be a <c>UI.Panel</c> or
		/// any <c>Widgets.Visual</c>).
		/// </summary>
		/// <param name="o">The user interface object.</param>
		/// <param name="manager">The resource manager.</param>
		/// <returns>The cloned user interface object.</returns>
		public static Widgets.Visual Duplicate(Widgets.Visual visual, ResourceManager manager)
		{
			string xml;

			{
				System.Text.StringBuilder buffer = new System.Text.StringBuilder();
				System.IO.StringWriter stringWriter = new System.IO.StringWriter(buffer);
				System.Xml.XmlTextWriter xmlWriter = new System.Xml.XmlTextWriter(stringWriter);

				using (Types.Serialization.Context context = new Types.Serialization.SerializerContext(new Types.Serialization.IO.XmlWriter(xmlWriter)))
				{
					UI.Panel.FillSerializationContext(context, null, manager);

					xmlWriter.Formatting = System.Xml.Formatting.None;
					xmlWriter.WriteStartElement("elem");

					context.ActiveWriter.WriteAttributeStrings();

					Types.Storage.Serialize(visual, context);

					xmlWriter.WriteEndElement();
					xmlWriter.Flush();
					xmlWriter.Close();

					xml = buffer.ToString();
				}
			}
			
			{
				System.IO.StringReader stringReader = new System.IO.StringReader(xml);
				System.Xml.XmlTextReader xmlReader = new System.Xml.XmlTextReader(stringReader);

				xmlReader.Read();

				System.Diagnostics.Debug.Assert(xmlReader.NodeType == System.Xml.XmlNodeType.Element);
				System.Diagnostics.Debug.Assert(xmlReader.LocalName == "elem");

				Types.Serialization.Context context = new Types.Serialization.DeserializerContext(new Types.Serialization.IO.XmlReader(xmlReader));

				UI.Panel.FillSerializationContext(context, null, manager);

				return Types.Storage.Deserialize(context) as Widgets.Visual;
			}
		}
		
		public static void RunPanel(UI.Panel panel, ResourceManager manager, Widgets.Window mainWindow, string name)
		{
			string xml = UserInterface.SerializePanel(panel, manager);
			UI.Panel clone = UserInterface.DeserializePanel(xml, manager);
			UI.PanelStack stack = new UI.PanelStack();
			Widgets.Window window = new Widgets.Window();

			window.MakeSecondaryWindow();
			window.ShowWindowIcon = false;
			window.Root.Children.Add(stack);

			stack.Dock = Widgets.DockStyle.Fill;
			clone.Dock = Widgets.DockStyle.Fill;

			clone.SetupSampleDataSource();

			stack.Children.Add(clone);

			window.Owner = mainWindow;
			window.Text = name;

			window.ForceLayout();

			double width  = Widgets.Layouts.LayoutMeasure.GetWidth(clone).Desired;
			double height = Widgets.Layouts.LayoutMeasure.GetHeight(clone).Desired;

			window.ClientSize = new Size(width, height);

			double dx = window.WindowSize.Width;
			double dy = window.WindowSize.Height;  // taille avec le cadre

			Point center = UserInterface.runPanelCenter;
			if (center.IsZero)
			{
				center = mainWindow.WindowBounds.Center;
			}
			Rectangle rect = new Rectangle(center.X-dx/2, center.Y-dy/2, dx, dy);
			window.WindowBounds = rect;

			window.ShowDialog();  // affiche le dialogue modal...
			
			UserInterface.runPanelCenter = window.WindowPlacementBounds.Center;
		}
		
		public static void RunForm(UI.Panel panel, Widgets.Window mainWindow, Size size, string name)
		{
			UI.PanelStack stack = new UI.PanelStack();
			Widgets.Window window = new Widgets.Window();

			window.MakeSecondaryWindow();
			window.ShowWindowIcon = false;
			window.Root.Children.Add(stack);

			stack.Dock = Widgets.DockStyle.Fill;
			panel.Dock = Widgets.DockStyle.Fill;
			panel.Margins = new Margins(10, 10, 10, 10);

			stack.Children.Add(panel);

			window.Owner = mainWindow;
			window.Text = name;
			window.ForceLayout();
			window.ClientSize = size;

			double dx = window.WindowSize.Width;
			double dy = window.WindowSize.Height;  // taille avec le cadre

			Point center = UserInterface.runPanelCenter;
			if (center.IsZero)
			{
				center = mainWindow.WindowBounds.Center;
			}
			Rectangle rect = new Rectangle(center.X-dx/2, center.Y-dy/2, dx, dy);
			window.WindowBounds = rect;

			window.ShowDialog();  // affiche le dialogue modal...
			
			UserInterface.runPanelCenter = window.WindowPlacementBounds.Center;
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
