//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Data.Common;

using System.Collections.Generic;
using System.IO;


namespace Epsitec.Aider.Data.Subscription
{
	internal static class SubscriptionDataReader
	{
		public static IEnumerable<Dictionary<WebSubscriptionHeader, string>> ReadWebSubscriptions(FileInfo input)
		{
			return DataReader.GetRecords (input, SubscriptionDataReader.webSubscriptionHeaders);
		}

		public static IEnumerable<Dictionary<DoctorSubscriptionHeader, string>> ReadDoctorSubscriptions(FileInfo input)
		{
			return DataReader.GetRecords (input, SubscriptionDataReader.doctorSubscriptionHeaders);
		}

		public static IEnumerable<Dictionary<ProSubscriptionHeader, string>> ReadProSubscriptions(FileInfo input)
		{
			return DataReader.GetRecords (input, SubscriptionDataReader.proSubscriptionHeaders);
		}

		public static IEnumerable<Dictionary<WebSubscriptionHeader, string>> ReadGenericSubscriptions(FileInfo input)
		{
			return DataReader.GetRecords (input, SubscriptionDataReader.genericSubscriptionHeaders);
		}

		public static IEnumerable<Dictionary<ExcelSubscriptionHeader, string>> ReadExcelSubscriptions(FileInfo input)
		{
			return DataReader.GetRecords (input, SubscriptionDataReader.excelSubscriptionHeaders);
		}

		private static readonly Dictionary<ExcelSubscriptionHeader, string> excelSubscriptionHeaders =
			new Dictionary<ExcelSubscriptionHeader, string> ()
			{
				{ ExcelSubscriptionHeader.HouseholdId, "ID Ménage" },
				{ ExcelSubscriptionHeader.Title, "Appellation" },
				{ ExcelSubscriptionHeader.Lastname, "Nom de famille" },
				{ ExcelSubscriptionHeader.Firstname, "Prénoms" },
				{ ExcelSubscriptionHeader.Street, "Adresse" },
				{ ExcelSubscriptionHeader.HouseNumber, "Numéro" },
				{ ExcelSubscriptionHeader.ZipCode, "NPA" },
				{ ExcelSubscriptionHeader.Town, "Localité" },
				{ ExcelSubscriptionHeader.Confession, "Confession" },
				{ ExcelSubscriptionHeader.Sex, "Sexe" },
				{ ExcelSubscriptionHeader.MaritalStatus, "Etat civil" },
				{ ExcelSubscriptionHeader.Comment, "Commentaire" },
				{ ExcelSubscriptionHeader.Phone1, "Tel 1" },
				{ ExcelSubscriptionHeader.Mobile, "Tel mobile" },
				{ ExcelSubscriptionHeader.EMail, "E-mail" },
				{ ExcelSubscriptionHeader.Profession, "Profession" },
				{ ExcelSubscriptionHeader.BirthDate, "Date de naissance" }
			};


		private static readonly Dictionary<WebSubscriptionHeader, string> genericSubscriptionHeaders =
			new Dictionary<WebSubscriptionHeader, string> ()
			{
				{ WebSubscriptionHeader.CorporateName, "Raison sociale" },
				{ WebSubscriptionHeader.Title, "Titre" },
				{ WebSubscriptionHeader.Lastname, "Nom" },
				{ WebSubscriptionHeader.Firstname, "Prénom" },
				{ WebSubscriptionHeader.Address, "Adresse" },
				{ WebSubscriptionHeader.PostBox, "Case postale" },
				{ WebSubscriptionHeader.ZipCode, "NPA" },
				{ WebSubscriptionHeader.Town, "Localité" },
			};

		private static readonly Dictionary<WebSubscriptionHeader, string> webSubscriptionHeaders =
			new Dictionary<WebSubscriptionHeader, string> ()
			{
				{ WebSubscriptionHeader.CorporateName, "Raison sociale" },
				{ WebSubscriptionHeader.Title, "Politesse" },
				{ WebSubscriptionHeader.Lastname, "Nom" },
				{ WebSubscriptionHeader.Firstname, "Prénom" },
				{ WebSubscriptionHeader.Address, "Adresse complète" },
				{ WebSubscriptionHeader.PostBox, "Case postale" },
				{ WebSubscriptionHeader.ZipCode, "Numéro postal" },
				{ WebSubscriptionHeader.Town, "Localité" },
				{ WebSubscriptionHeader.CountryCode, "Pays" },
				{ WebSubscriptionHeader.Comment, "Remarque" },
				{ WebSubscriptionHeader.RegionalEdition, "Région" },
				{ WebSubscriptionHeader.NbCopies, "Nombre" },
			};


		private static readonly Dictionary<DoctorSubscriptionHeader, string> doctorSubscriptionHeaders =
			new Dictionary<DoctorSubscriptionHeader, string> ()
			{
				{ DoctorSubscriptionHeader.CorporateName, "Société" },
				{ DoctorSubscriptionHeader.Title, "Civilité" },
				{ DoctorSubscriptionHeader.Lastname, "Nom" },
				{ DoctorSubscriptionHeader.Firstname, "Prénom" },
				{ DoctorSubscriptionHeader.Address1, "Adresse 1" },
				{ DoctorSubscriptionHeader.Address2, "Adresse 2" },
				{ DoctorSubscriptionHeader.ZipCode, "Npa" },
				{ DoctorSubscriptionHeader.Town, "Ville" },
			};


		private static readonly Dictionary<ProSubscriptionHeader, string> proSubscriptionHeaders =
			new Dictionary<ProSubscriptionHeader, string> ()
			{
				{ ProSubscriptionHeader.PersonInName1, "PERSON_IN_COMPANY_NAME1" },
				{ ProSubscriptionHeader.Name1, "COMPANY_NAME1" },
				{ ProSubscriptionHeader.Name2, "COMPANY_NAME2" },
				{ ProSubscriptionHeader.Name3, "COMPANY_NAME3" },
				{ ProSubscriptionHeader.Address, "STREET" },
				{ ProSubscriptionHeader.PostBox, "PO_BOX" },
				{ ProSubscriptionHeader.ZipCode, "MAIL_ZIP" },
				{ ProSubscriptionHeader.Town, "MAIL_CITY" },
			};


	}


	internal enum ExcelSubscriptionHeader
	{
		HouseholdId,
		Title,
		Lastname,
		Firstname,
		BirthDate,
		Profession,
		Street,
		HouseNumber,
		ZipCode,
		Town,
		Confession,
		Sex,
		Origin,
		MaritalStatus,
		Comment,
		Phone1,
		Mobile,
		EMail
		
	}

	internal enum WebSubscriptionHeader
	{
		CorporateName,
		Title,
		Lastname,
		Firstname,
		Address,
		PostBox,
		ZipCode,
		Town,
		CountryCode,
		Comment,
		RegionalEdition,
		NbCopies
	}


	internal enum DoctorSubscriptionHeader
	{
		CorporateName,
		Title,
		Lastname,
		Firstname,
		Address1,
		Address2,
		ZipCode,
		Town,
	}


	internal enum ProSubscriptionHeader
	{
		PersonInName1,
		Name1,
		Name2,
		Name3,
		Address,
		PostBox,
		ZipCode,
		Town
	}


}
