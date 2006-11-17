//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets.Behaviors
{
	/// <summary>
	/// La classe CopyPasteBehavior gère le copier & coller.
	/// </summary>
	public class CopyPasteBehavior
	{
		public CopyPasteBehavior(Widget host)
		{
			this.host = host;
		}
		
		
		public Widget							Host
		{
			get
			{
				return this.host;
			}
		}
		
		
		public bool ProcessMessage(Message message, Drawing.Point pos)
		{
			if (this.host.IsEnabled)
			{
				if (message.MessageType == MessageType.KeyPress)
				{
					if (message.IsControlPressed)
					{
						switch (message.KeyCode)
						{
							case KeyCode.AlphaC:
								if (this.ProcessCopy ())
								{
									message.Consumer = this.host;
								}
								break;
                            
							case KeyCode.AlphaV:
								if (this.ProcessPaste ())
								{
									message.Consumer = this.host;
								}
								break;
							
							case KeyCode.AlphaX:
								if (this.ProcessCopy () && this.ProcessDelete ())
								{
									message.Consumer = this.host;
								}
								break;
							
							case KeyCode.AlphaA:
								if (this.ProcessSelectAll ())
								{
									message.Consumer = this.host;
								}
								break;
						}
					}
				}
			}
			
			return message.Handled;
		}
		
		
		public bool ProcessCopy()
		{
			AbstractTextField text = this.host as AbstractTextField;
			
			if (text != null)
			{
				string value = text.Selection;
				
				if (value == "")
				{
					value = text.Text;
				}
				
				Support.Clipboard.WriteData data = new Support.Clipboard.WriteData ();
				
				data.WriteTextLayout (value);
				data.WriteHtmlFragment (value);
				Support.Clipboard.SetData (data);
				
				return true;
			}
			
			return false;
		}
		
		public bool ProcessPaste()
		{
			AbstractTextField text = this.host as AbstractTextField;
			
			if (text != null)
			{
				Support.Clipboard.ReadData data = Support.Clipboard.GetData ();
				
				string text_layout = data.ReadTextLayout ();
				string html        = null;
				
				if (text_layout != null)
				{
					html = text_layout;
				}
				else
				{
					html = data.ReadHtmlFragment ();
					
					if (html != null)
					{
						html = Support.Clipboard.ConvertHtmlToSimpleXml (html);
					}
					else
					{
						html = TextLayout.ConvertToTaggedText (data.ReadText ());
					}
				}
				
				if (html != null)
				{
					if (text.TextFieldStyle != TextFieldStyle.Multi)
					{
						html = html.Replace ("<br/>", " ");
					}
					
					text.Selection = html;
					text.SimulateEdited ();
					return true;
				}
			}
			
			return false;
		}
		
		public bool ProcessDelete()
		{
			AbstractTextField text = this.host as AbstractTextField;
			
			if (text != null)
			{
				string value = text.Selection;
				
				if (value == "")
				{
					text.SelectAll ();
				}
				
				text.TextNavigator.DeleteSelection ();
				text.SimulateEdited ();
				
				return true;
			}
			
			return false;
		}
		
		public bool ProcessSelectAll()
		{
			AbstractTextField text = this.host as AbstractTextField;
			
			if (text != null)
			{
				text.SelectAll ();
				return true;
			}
			
			return false;
		}
		
		
		protected Widget						host;
	}
}
