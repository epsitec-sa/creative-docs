using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;
using System.Xml.Linq;

namespace Epsitec.Windows
{
	public class TextLayout
	{
		public static readonly DependencyProperty HtmlProperty =
			DependencyProperty.RegisterAttached ("Html", typeof (string), typeof (TextLayout), new PropertyMetadata (string.Empty, TextLayout.OnHtmlChanged));

		public static string GetHtml(DependencyObject obj)
		{
			return (string) obj.GetValue (HtmlProperty);
		}

		public static void SetHtml(DependencyObject obj, string value)
		{
			obj.SetValue (HtmlProperty, value);
		}


		private static void OnHtmlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var inlines = TextLayout.GetInlines(d);
			if (inlines != null)
			{
				var htmlString = e.NewValue as string;
				TextLayout.Parse (TextLayout.ToXElement (htmlString).Nodes (), inlines);
			}
		}

		private static InlineCollection GetInlines(DependencyObject d)
		{
			if (d is TextBlock)
			{
				return (d as TextBlock).Inlines;
			}
			else if (d is Paragraph)
			{
				return (d as Paragraph).Inlines;
			}
			else if (d is Span)
			{
				return (d as Span).Inlines;
			}
			return null;
		}

		private static XElement ToXElement(string htmlString)
		{
			return XElement.Parse ("<html>" + htmlString + "</html>");
		}

		private static void Parse(IEnumerable<XNode> nodes, InlineCollection container)
		{
			foreach (var node in nodes)
			{
				if (node is XText)
				{
					var text = node as XText;
					container.Add (new Run (text.Value));
				}
				else if (node is XElement)
				{
					var element = node as XElement;
					var name = element.Name.LocalName;

					Action<XElement, InlineCollection> factory;
					if (TextLayout.factories.TryGetValue (name, out factory))
					{
						factory (element, container);
					}
					else
					{
						throw new NotImplementedException (string.Format ("TextLayout.Html : tag '{0}' factory not implemented", name));
					}
				}
			}
		}

		private static readonly Dictionary<string, Action<XElement, InlineCollection>> factories = new Dictionary<string, Action<XElement, InlineCollection>> ()
		{
			{
				"a",
				(element, container) =>
				{
					var hyperlink = new Hyperlink ();
					hyperlink.NavigateUri = new Uri (element.Attribute ("href").Value);
					hyperlink.RequestNavigate += (sender, e) =>
					{
						Process.Start (new ProcessStartInfo (e.Uri.AbsoluteUri));
						e.Handled = true;
					};
					TextLayout.Parse (element.Nodes (), hyperlink.Inlines);
					container.Add (hyperlink);
				}
			},
			{
				"b",
				(element, container) =>
				{
					var span = new Bold ();
					TextLayout.Parse (element.Nodes (), span.Inlines);
					container.Add (span);
				}
			},
			{
				"br",
				(element, container) =>
				{
					container.Add (new LineBreak ());
				}
			},
			{
				"i",
				(element, container) =>
				{
					var span = new Italic ();
					TextLayout.Parse (element.Nodes (), span.Inlines);
					container.Add (span);
				}
			},
			{
				"u",
				(element, container) =>
				{
					var span = new Underline ();
					TextLayout.Parse (element.Nodes (), span.Inlines);
					container.Add (span);
				}
			},
		};
	}
}
