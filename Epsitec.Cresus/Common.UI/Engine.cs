//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA, 27/04/2004

namespace Epsitec.Common.UI
{
	/// <summary>
	/// La classe Engine...
	/// </summary>
	public class Engine
	{
		public Engine()
		{
		}
		
		public static void BindWidget(Types.IDataGraph graph, Common.Widgets.Widget widget, string binding)
		{
			System.Xml.XmlDocument doc = new System.Xml.XmlDocument ();
			doc.Load (new System.IO.StringReader ("<xml>" + binding + "</xml>"));
			
			Engine.BindWidget (graph, widget, doc.DocumentElement.FirstChild);
		}
		
		public static void BindWidget(Types.IDataGraph graph, Common.Widgets.Widget widget, System.Xml.XmlNode binding)
		{
			System.Xml.XmlAttribute x_path = binding.Attributes["path"];
			
			if (x_path == null)
			{
				throw new System.ArgumentException (string.Format ("Cannot bind widget; no path found in binding information ({0}).", binding.OuterXml), "binding");
			}
			
			string           path   = x_path.Value;
			Types.IDataValue source = graph.Navigate (path) as Types.IDataValue;
			
			if (source == null)
			{
				throw new System.ArgumentException (string.Format ("Cannot bind widget; invalid path found in binding information ({0}).", path), "binding");
			}
			
			Types.INamedType type = source.DataType;
			
			if (type == null)
			{
				throw new System.ArgumentException (string.Format ("Cannot bind widget; path ({0}) points to typeless data.", path), "binding");
			}
			
			if (type is Types.IString)
			{
				Binders.DataValueBinder          binder  = new Binders.DataValueBinder (source);
				Adapters.StringAdapter           adapter = new Adapters.StringAdapter (binder);
				Controllers.WidgetTextController control = new Controllers.WidgetTextController (adapter, widget);
				
				return;
			}
			
			if (type is Types.INum)
			{
				//	TODO: attache le widget à cette valeur numérique
			}
			
			if (type is Types.IEnum)
			{
				//	TODO: attache le widget à cette énumération
			}
			
			throw new System.ArgumentException (string.Format ("Cannot bind widget; path ({0}) points to data of unknown type ({1}).", path, type.Name), "binding");
		}
	}
}
