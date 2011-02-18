//	Copyright © 2003-2010, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
						//	conserve <br/> et éléments &xx;)
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


			[Command (Res.CommandIds.Bold)]
			public void CommandBold(CommandDispatcher dispatcher, CommandEventArgs e)
			{
				if (this.host.IsReadOnly)
				{
					return;
				}

				this.host.navigator.SelectionBold = !this.host.navigator.SelectionBold;
				this.host.OnTextEdited ();
			}

			[Command (Res.CommandIds.Italic)]
			public void CommandItalic(CommandDispatcher dispatcher, CommandEventArgs e)
			{
				if (this.host.IsReadOnly)
				{
					return;
				}

				this.host.navigator.SelectionItalic = !this.host.navigator.SelectionItalic;
				this.host.OnTextEdited ();
			}

			[Command (Res.CommandIds.Underlined)]
			public void CommandUnderlined(CommandDispatcher dispatcher, CommandEventArgs e)
			{
				if (this.host.IsReadOnly)
				{
					return;
				}

				this.host.navigator.SelectionUnderline = !this.host.navigator.SelectionUnderline;
				this.host.OnTextEdited ();
			}

			[Command (Res.CommandIds.Subscript)]
			public void CommandSubscript(CommandDispatcher dispatcher, CommandEventArgs e)
			{
				if (this.host.IsReadOnly)
				{
					return;
				}

				this.host.navigator.SelectionSubscript = !this.host.navigator.SelectionSubscript;
				this.host.OnTextEdited ();
			}

			[Command (Res.CommandIds.Superscript)]
			public void CommandSuperscript(CommandDispatcher dispatcher, CommandEventArgs e)
			{
				if (this.host.IsReadOnly)
				{
					return;
				}

				this.host.navigator.SelectionSuperscript = !this.host.navigator.SelectionSuperscript;
				this.host.OnTextEdited ();
			}

			[Command (Res.CommandIds.MultilingualEdition)]
			public void CommandMultilingualEdition(CommandDispatcher dispatcher, CommandEventArgs e)
			{
				if (this.host.IsReadOnly)
				{
					return;
				}

				this.host.OnMultilingualEditionCalled ();
			}

			
			public void NotifyIsFocusedChanged(bool focused)
			{
				this.UpdateCommandStatesEnable (focused);
				this.UpdateCommandStatesActiveState ();
			}

			public void NotifyStyleChanged()
			{
				this.UpdateCommandStatesActiveState ();
			}

			
			private void UpdateCommandStatesEnable(bool focused)
			{
				var commandContext = CommandContext.GetContext (this.host);

				bool isReadOnly  = this.host.IsReadOnly;
				bool isReadWrite = !isReadOnly;
				bool canFormat   = focused && isReadWrite && this.host.IsFormattedText;

				commandContext.GetCommandState (ApplicationCommands.Copy     ).Enable = focused;
				commandContext.GetCommandState (ApplicationCommands.Cut      ).Enable = focused && isReadWrite;
				commandContext.GetCommandState (ApplicationCommands.Paste    ).Enable = focused && isReadWrite;
				commandContext.GetCommandState (ApplicationCommands.Delete   ).Enable = focused && isReadWrite;
				commandContext.GetCommandState (ApplicationCommands.SelectAll).Enable = focused;

				commandContext.GetCommandState (ApplicationCommands.Bold       ).Enable = canFormat;
				commandContext.GetCommandState (ApplicationCommands.Italic     ).Enable = canFormat;
				commandContext.GetCommandState (ApplicationCommands.Underlined ).Enable = canFormat;
				commandContext.GetCommandState (ApplicationCommands.Subscript  ).Enable = canFormat;
				commandContext.GetCommandState (ApplicationCommands.Superscript).Enable = canFormat;

				commandContext.GetCommandState (ApplicationCommands.MultilingualEdition).Enable = canFormat;
			}

			private void UpdateCommandStatesActiveState()
			{
				var commandContext = CommandContext.GetContext (this.host);

				bool isReadOnly  = this.host.IsReadOnly;
				bool isReadWrite = !isReadOnly;
				bool canFormat   = isReadWrite && this.host.IsFormattedText;

				if (canFormat)
				{
					commandContext.GetCommandState (ApplicationCommands.Bold       ).ActiveState = this.host.navigator.SelectionBold        ? ActiveState.Yes : ActiveState.No;
					commandContext.GetCommandState (ApplicationCommands.Italic     ).ActiveState = this.host.navigator.SelectionItalic      ? ActiveState.Yes : ActiveState.No;
					commandContext.GetCommandState (ApplicationCommands.Underlined ).ActiveState = this.host.navigator.SelectionUnderline   ? ActiveState.Yes : ActiveState.No;
					commandContext.GetCommandState (ApplicationCommands.Subscript  ).ActiveState = this.host.navigator.SelectionSubscript   ? ActiveState.Yes : ActiveState.No;
					commandContext.GetCommandState (ApplicationCommands.Superscript).ActiveState = this.host.navigator.SelectionSuperscript ? ActiveState.Yes : ActiveState.No;
				}
			}
			
			
			private readonly AbstractTextField host;
		}
	}
}
