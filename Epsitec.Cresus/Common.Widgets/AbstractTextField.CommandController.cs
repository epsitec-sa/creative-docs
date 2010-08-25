//	Copyright � 2003-2010, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Widgets
{
	public abstract partial class AbstractTextField
	{
		private sealed class CommandController
		{
			public CommandController(AbstractTextField host)
			{
				this.host = host;
			}


			[Command (Res.CommandIds.Copy)]
			public void CommandCopy(CommandDispatcher dispatcher, CommandEventArgs e)
			{
				string value = this.host.Selection;

				if (value == "")
				{
					value = this.host.Text;
				}

				ClipboardWriteData data = new ClipboardWriteData ();

				data.WriteTextLayout (value);
				data.WriteHtmlFragment (value);
				Clipboard.SetData (data);

				e.Executed = true;
			}

			[Command (Res.CommandIds.Cut)]
			public void CommandCut(CommandDispatcher dispatcher, CommandEventArgs e)
			{
				if (this.host.IsReadOnly)
				{
					return;
				}

				string value = this.host.Selection;

				if (value == "")
				{
					value = this.host.Text;
					this.host.SelectAll ();
				}

				ClipboardWriteData data = new ClipboardWriteData ();

				data.WriteTextLayout (value);
				data.WriteHtmlFragment (value);
				Clipboard.SetData (data);

				this.host.TextNavigator.DeleteSelection ();
				this.host.SimulateEdited ();

				e.Executed = true;
			}

			[Command (Res.CommandIds.Delete)]
			public void CommandDelete(CommandDispatcher dispatcher, CommandEventArgs e)
			{
				if (this.host.IsReadOnly)
				{
					return;
				}

				string value = this.host.Selection;

				if (value == "")
				{
					this.host.SelectAll ();
				}

				this.host.TextNavigator.DeleteSelection ();
				this.host.SimulateEdited ();

				e.Executed = true;
			}

			[Command (Res.CommandIds.SelectAll)]
			public void CommandSelectAll(CommandDispatcher dispatcher, CommandEventArgs e)
			{
				this.host.SelectAll ();

				e.Executed = true;
			}

			[Command (Res.CommandIds.Paste)]
			public void CommandPaste(CommandDispatcher dispatcher, CommandEventArgs e)
			{
				if (this.host.IsReadOnly)
				{
					return;
				}

				ClipboardReadData data = Clipboard.GetData ();

				string textLayout = data.ReadTextLayout ();
				string html        = null;

				if (textLayout != null)
				{
					html = textLayout;
				}
				else
				{
					html = data.ReadHtmlFragment ();

					if (html != null)
					{
						html = Clipboard.ConvertHtmlToSimpleXml (html);
					}
					else
					{
						html = TextConverter.ConvertToTaggedText (data.ReadText ());
					}
				}

				if ((html != null) &&
					(html.Length > 0))
				{
					if (this.host.TextFieldStyle != TextFieldStyle.Multiline)
					{
						html = html.Replace ("<br/>", " ");
					}

					if (this.host.IsFormattedText == false)
					{
						//	TODO: supprime le formatage pour ne garder que tu texte pur (oublie les <b>/<i>/<font> ... mais
						//	conserve <br/> et �l�ments &xx;)
					}

					this.host.Selection = html;
					this.host.SimulateEdited ();

					e.Executed = true;
				}
			}

			[Command (Res.CommandIds.UseDefaultValue)]
			public void CommandUseDefaultValue(CommandDispatcher dispatcher, CommandEventArgs e)
			{
				if (this.host.IsReadOnly)
				{
					return;
				}

				this.host.StartEdition ();
				this.host.navigator.ReplaceWithText (ResourceBundle.Field.Null);
			}


			private readonly AbstractTextField host;
		}
	}
}
