//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Support
{
	/// <summary>
	/// The <c>FolderId</c> enumeration lists all special folders which might
	/// be defined by the Operating System (currently only Microsoft Windows).
	/// </summary>
	public enum FolderId
	{
		CommonAdminTools,
		CommonAltStartup,
		CommonAppData,
		CommonDesktopDirectory,
		CommonDocuments,
		CommonFavorites,
		CommonMusic,
		CommonPictures,
		CommonStartMenuPrograms,
		CommonStartMenu,
		CommonStartMenuProgramsStartup,
		CommonTemplates,
		CommonVideo,

		AdminTools,
		AltStartup,
		ApplicationData,
		CdBurnArea,
		Cookies,
		DesktopDirectory,
		Favorites,
		Fonts,
		History,
		Internet,
		InternetCache,
		LocalAppData,
		NetHood,
		PrintHood,
		Profile,
		Profiles,
		ProgramFiles,
		ProgramFilesCommon,

		StartMenu,
		StartMenuPrograms,
		StartMenuProgramsStartup,
		
		Windows,
		WindowsSystem,
		Templates,
		
		Recent,
		SendTo,

		MyDocuments,
		MyMusic,
		MyPictures,
		MyVideo,
		
		VirtualControlPanel,
		VirtualDesktop,
		VirtualMyComputer,
		VirtualMyDocuments,
		VirtualNetwork,
		VirtualPrinters,
		VirtualRecycleBin,
	}
}
