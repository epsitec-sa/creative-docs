//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;

using System.Xml.Linq;
using System.Linq;
using System.Linq.Expressions;

namespace Epsitec.Cresus.Core.Print.Deserializers
{
	public class DeserializedPage
	{
		public DeserializedPage(DeserializedSection parentSection, int pageRank, XElement xRoot)
		{
			this.parentSection = parentSection;
			this.pageRank      = pageRank;
			this.xRoot         = xRoot;
		}


		public DeserializedSection ParentSection
		{
			get
			{
				return this.parentSection;
			}
		}

		public int PageRank
		{
			get
			{
				return this.pageRank;
			}
		}

		public XElement XRoot
		{
			get
			{
				return this.xRoot;
			}
		}


		public string ShortDescription
		{
			get
			{
				return string.Format ("<b>{0}</b> ({1} {2}×{3})", this.parentSection.ParentJob.JobFullName, this.parentSection.PrinterLogicalName, this.parentSection.PageSize.Width, this.parentSection.PageSize.Height);
			}
		}

		public string FullDescription
		{
			get
			{
				string s1 = string.Concat ("● Titre: <b>",           this.parentSection.ParentJob.JobFullName, "</b>");
				string s2 = string.Concat ("● Unité d'impression: ", this.parentSection.PrinterLogicalName);
				string s3 = string.Concat ("● Imprimante: ",         this.parentSection.ParentJob.PrinterPhysicalName);
				string s4 = string.Concat ("● Bac: ",                this.parentSection.PrinterPhysicalTray);
				string s5 = string.Concat ("● Dimensions: ",         this.parentSection.PageSize.Width, "×", this.parentSection.PageSize.Height, " mm");
				string s6 = string.Concat ("● Numéro de la page: ",  this.pageRank+1);

				return string.Join ("<br/>", s1, s2, s3, s4, s5, s6);
			}
		}

		public Bitmap Miniature
		{
			get;
			set;
		}


		private readonly DeserializedSection	parentSection;
		private readonly int					pageRank;
		private readonly XElement				xRoot;
	}
}
