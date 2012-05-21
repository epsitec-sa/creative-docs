using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.PDF
{
	/// <summary>
	/// La classe Pattern enregistre les informations d'un pattern.
	/// </summary>
	public class Pattern
	{
		public int Page
		{
			get { return this.page; }
			set { this.page = value; }
		}

		public Objects.Abstract Object
		{
			get { return this.obj; }
			set { this.obj = value; }
		}

		public Properties.Abstract Property
		{
			get { return this.property; }
			set { this.property = value; }
		}

		public int Rank
		{
			get { return this.rank; }
			set { this.rank = value; }
		}

		public int Id
		{
			get { return this.id; }
			set { this.id = value; }
		}

		protected int					page;  // 1..n
		protected Objects.Abstract		obj;
		protected Properties.Abstract	property;
		protected int					rank;
		protected int					id;
	}
}
