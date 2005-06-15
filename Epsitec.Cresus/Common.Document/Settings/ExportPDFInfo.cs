using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Settings
{
	/// <summary>
	/// La classe ExportPDFInfo comtient tous les réglages secondaires pour l'impression.
	/// </summary>
	[System.Serializable()]
	public class ExportPDFInfo : ISerializable
	{
		public ExportPDFInfo(Document document)
		{
			this.document = document;
			this.Initialise();
		}

		protected void Initialise()
		{
			this.pageRange = PrintRange.All;
			this.pageFrom = 1;
			this.pageTo = 10000;
		}

		public PrintRange PageRange
		{
			get { return this.pageRange; }
			set { this.pageRange = value; }
		}

		public int PageFrom
		{
			get { return this.pageFrom; }
			set { this.pageFrom = value; }
		}

		public int PageTo
		{
			get { return this.pageTo; }
			set { this.pageTo = value; }
		}


		#region Serialization
		// Sérialise les réglages.
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("Rev", 1);
			info.AddValue("PageRange", this.pageRange);
			info.AddValue("PageFrom", this.pageFrom);
			info.AddValue("PageTo", this.pageTo);
		}

		// Constructeur qui désérialise les réglages.
		protected ExportPDFInfo(SerializationInfo info, StreamingContext context)
		{
			this.document = Document.ReadDocument;
			this.Initialise();

			int rev = 0;
			if ( Support.Serialization.Helper.FindElement(info, "Rev") )
			{
				rev = info.GetInt32("Rev");
			}

			this.pageRange = (PrintRange) info.GetValue("PageRange", typeof(PrintRange));
			this.pageFrom = info.GetInt32("PageFrom");
			this.pageTo = info.GetInt32("PageTo");
		}
		#endregion

		
		protected Document				document;
		protected PrintRange			pageRange;
		protected int					pageFrom;
		protected int					pageTo;
	}
}
