namespace Epsitec.Common.Widgets.Helpers
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
				if (message.Type == MessageType.KeyPress)
				{
					if (message.IsCtrlPressed)
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
								if (this.ProcessCopy () && this.ProcessErase ())
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
				
				data.WriteHtmlFragment (Support.Clipboard.ConvertSimpleXmlToHtml (value));
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
				
				string html = data.ReadHtmlFragment ();
				
				if (html != null)
				{
					html = Support.Clipboard.ConvertHtmlToSimpleXml (html);
				}
				else
				{
					html = TextLayout.ConvertToTaggedText (data.ReadText ());
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
		
		public bool ProcessErase()
		{
			AbstractTextField text = this.host as AbstractTextField;
			
			if (text != null)
			{
				string value = text.Selection;
				
				if (value == "")
				{
					text.SelectAll ();
				}
				
				text.DeleteSelection ();
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
