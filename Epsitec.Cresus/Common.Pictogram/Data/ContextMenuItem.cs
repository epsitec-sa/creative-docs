namespace Epsitec.Common.Pictogram.Data
{
	/// <summary>
	/// La classe ContextMenuItem représente une case du menu contextuel.
	/// </summary>
	public class ContextMenuItem
	{
		public ContextMenuItem()
		{
			this.name = "";
			this.icon = "";
			this.text = "";
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

		public string Text
		{
			get { return this.text; }
			set { this.text = value; }
		}

		protected string			name;
		protected string			icon;
		protected string			text;
	}
}
