using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.Dialogs
{
	/// <summary>
	/// Summary description for SelectFromList.
	/// </summary>
	public class SelectFromList : AbstractOkCancel
	{
		public SelectFromList(string title, string caption, string[] data, string command_template, CommandDispatcher command_dispatcher) : base (title, command_template, command_dispatcher)
		{
			this.caption = caption;
			this.data    = data;
		}
		
		
		public virtual string[]					Items
		{
			get
			{
				return this.data;
			}
			set
			{
				this.data = value;
				
				if (this.list != null)
				{
					this.list.Items.Clear ();
					this.list.Items.AddRange (this.data);
					this.list.SelectedIndex = 0;
				}
			}
		}
		
		
		protected virtual double				ExtraHeight
		{
			get { return 0; }
		}
		
		protected virtual void AddExtraWidgets(Widget body)
		{
		}
		
		protected override Widget CreateBodyWidget()
		{
			Widget body   = new Widget ();
			double extra  = this.ExtraHeight;
			double height = System.Math.Max (extra + 160, 200);
			
			body.Size = new Drawing.Size (320, height);
			
			StaticText label;
			
			label        = new StaticText (body);
			label.Bounds = new Drawing.Rectangle (0, body.Height - label.DefaultHeight, body.Width, label.DefaultHeight);
			label.Text   = this.caption;
			
			this.list = new ScrollList (body);
			
			this.list.Bounds  = new Drawing.Rectangle (0, extra, body.Width, label.Bottom - 4 - extra);
			this.list.Items.AddRange (this.data);
			this.list.TabIndex       = 1;
			this.list.TabNavigation  = Widget.TabNavigationMode.ActivateOnTab;
			
			this.AddValueWidget ("elem", this.list);
			this.AddExtraWidgets (body);
			
			return body;
		}
		
		protected string[]						data;
		protected string						caption;
		protected ScrollList					list;
	}
}
