namespace Epsitec.Common.Document
{
	/// <summary>
	/// La classe ContextMenuItem représente une case du menu contextuel.
	/// </summary>
	public class ContextMenuItem
	{
		public ContextMenuItem()
		{
			this.command = "";
			this.name = "";
			this.icon = "";
			this.iconActiveNo = "";
			this.iconActiveYes = "";
			this.text = "";
			this.active = false;
		}

		public string Command
		{
			get { return this.command; }
			set { this.command = value; }
		}

		public string Name
		{
			get { return this.name; }
			set { this.name = value; }
		}

		public string Icon
		{
			get { return this.icon; }
			set { this.icon = value; }
		}

		public string IconActiveNo
		{
			get { return this.iconActiveNo; }
			set { this.iconActiveNo = value; }
		}

		public string IconActiveYes
		{
			get { return this.iconActiveYes; }
			set { this.iconActiveYes = value; }
		}

		public string Text
		{
			get { return this.text; }
			set { this.text = value; }
		}

		public bool Active
		{
			get { return this.active; }
			set { this.active = value; }
		}

		protected string			command;
		protected string			name;
		protected string			icon;
		protected string			iconActiveNo;
		protected string			iconActiveYes;
		protected string			text;
		protected bool				active;
	}
}
