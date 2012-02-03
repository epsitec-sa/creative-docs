using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;

using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.UserInterface
{

	
	internal sealed class WebTileDataItem
	{


		// This class is the webcore equivalent of Epsitec.Cresus.Core.Controllers.TileDataItem.


		public WebTileDataItem()
		{
			this.SubViewControllerMode = ViewControllerMode.Edition;
		}

		// Not really used. Should be taken care of the hardcore way !!!
		public string Name
		{
			get;
			set;
		}


		public string IconUri
		{
			get;
			set;
		}

		// Not really used. Should be taken care of the hardcore way !!!
		public bool AutoGroup
		{
			get;
			set;
		}


		public bool HideAddButton
		{
			get;
			set;
		}


		public bool HideRemoveButton
		{
			get;
			set;
		}

		// Not really used. Should be taken care of the hardcore way !!!
		public bool FullHeightStretch
		{
			get;
			set;
		}


		public TileDataType DataType
		{
			get;
			set;
		}


		public FormattedText Title
		{
			get;
			set;
		}


		public FormattedText Text
		{
			get;
			set;
		}

		// Not really used. Should be taken care of the hardcore way !!!
		public FormattedText CompactTitle
		{
			get;
			set;
		}

		// Not really used. Should be taken care of the hardcore way !!!
		public FormattedText CompactText
		{
			get;
			set;
		}


		public ViewControllerMode SubViewControllerMode
		{
			get;
			set;
		}


		public int? SubViewControllerSubTypeId
		{
			get;
			set;
		}


	}


}
