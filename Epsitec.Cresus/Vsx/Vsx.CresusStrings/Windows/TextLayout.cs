using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
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
				TextLayout.Parse (e.NewValue as string, inlines);
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

		private static IEnumerable<XNode> ToXNodes(string htmlString)
		{
			try
			{
				return XElement.Parse ("<html>" + htmlString + "</html>").Nodes();
			}
			catch
			{
				return new XText (htmlString).AsSequence();
			}
		}

		private static void Parse(string htmlString, InlineCollection inlines)
		{
			TextLayout.Parse (TextLayout.ToXNodes (htmlString), inlines);
		}

		private static void Parse(IEnumerable<XNode> nodes, InlineCollection inlines)
		{
			foreach (var node in nodes)
			{
				if (node is XText)
				{
					var text = node as XText;
					inlines.Add (new Run (text.Value));
				}
				else if (node is XElement)
				{
					var element = node as XElement;
					var name = element.Name.LocalName;

					Action<XElement, InlineCollection> factory;
					if (TextLayout.factories.TryGetValue (name, out factory))
					{
						factory (element, inlines);
					}
					else
					{
						inlines.Add (new Run (string.Format ("ERROR: tag '{0}' not implemented", name))
						{
							Foreground = Brushes.Red
						});
					}
				}
			}
		}

		private static Inline EndOfLine()
		{
			return new InlineUIContainer (
				new Border ()
				{
					CornerRadius = new CornerRadius (3),
					Padding = new Thickness (3, 0, 3, 0),
					Background = Brushes.Gray,
					Child = new TextBlock (
						new Run (TextLayout.EndOfLineSymbol)
						{
							FontFamily = TextLayout.Wingdings3,
							Foreground = Brushes.White,
						}
					)
				});
		}

		private static readonly string EndOfLineSymbol = '\u00C9'.ToString ();
		private static readonly FontFamily  Wingdings3 = new FontFamily ("Wingdings 3");

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
					var lastInline = container.LastInline;
					if (lastInline == null || lastInline is LineBreak)
					{
						container.Add (TextLayout.EndOfLine ());
					}
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
