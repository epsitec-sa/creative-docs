using Epsitec.Common.Types;

using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.UserInterface.TileData
{


	internal sealed class HorizontalGroupData : AbstractEditionData
	{


		public FormattedText Title
		{
			get;
			set;
		}


		public IList<object> Fields
		{
			get
			{
				return this.fields;
			}
		}


		private readonly IList<object> fields = new List<object> ();


	}


}
