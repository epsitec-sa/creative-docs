//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Epsitec.Data.Platform.MatchSort
{
	public sealed class MatchSortEtl : IDisposable
	{
		/// <summary>
		/// Perform ETL job on Mat[CH]sort CSV file and load content for querying in SQLite
		/// https://match.post.ch/pdf/post-match-new-sort.pdf
		/// </summary>
		public MatchSortEtl(string csvFilePath)
		{

			try
			{
				var DatabaseDirectoryPath = Epsitec.Common.Support.Globals.ExecutableDirectory;
				var DatabaseFilePath = System.IO.Path.Combine (DatabaseDirectoryPath, "MatchSort.sqlite");
				if (!File.Exists (DatabaseFilePath))
				{
					//CASE NO DATABASE
					SQLiteConnection.CreateFile ("MatchSort.sqlite");
					this.OpenDatabase ();
					this.CreateTableIfNeededAndResetDb ();

					this.insertPlaceCommand = this.BuildInsertPlace ();
					this.InsertPlaceAltCommand = this.BuildInsertPlaceAlt ();
					this.InsertCommunityCommand = this.BuildInsertCommunity ();
					this.InsertStreetCommand = this.BuildInsertStreet ();
					this.InsertHouseCommand = this.BuildInsertHouse ();
					this.InsertMessengerCommand = this.BuildInsertMessenger ();

					this.LoadFromDatabaseCsv (csvFilePath);
					this.IndexAndAnalyzeDatabase ();
				}
				else
				{
					//CASE CHECK FOR UPDATE
					this.OpenDatabase ();

					var VersionCsv = this.GetHeaderFromCsv (csvFilePath);
					var VersionDb = this.GetHeaderFromDatabase ();

					if ((VersionCsv[0] != VersionDb[0]) || 
						(VersionCsv[1] != VersionDb[1]))
					{
						this.CreateTableIfNeededAndResetDb ();

						this.insertPlaceCommand = this.BuildInsertPlace ();
						this.InsertPlaceAltCommand = this.BuildInsertPlaceAlt ();
						this.InsertCommunityCommand = this.BuildInsertCommunity ();
						this.InsertStreetCommand = this.BuildInsertStreet ();
						this.InsertHouseCommand = this.BuildInsertHouse ();
						this.InsertMessengerCommand = this.BuildInsertMessenger ();

						this.LoadFromDatabaseCsv (csvFilePath);
						this.IndexAndAnalyzeDatabase ();
					}
				}


				//Prepare and Build SQL Commands
				this.MessengerCommand = this.BuildMessengerCommand ();
				this.MessengerCommandRelaxed = this.BuildMessengerCommandRelaxed ();
				this.HousesAtStreetCommand = this.BuildHousesAtStreetCommand ();

			}
			catch
			{
				this.Dispose ();
				throw;
			}

		}

		/// <summary>
		/// Get the Messenger number
		/// Ex: GetMessenger("1000","06","avenue floréal","10","a")
		/// </summary>
		/// <param name="zip">4 digits zip </param>
		/// <param name="zipAddon">2 digits additionnal zip </param>
		/// <param name="street">human readable street name (non case-sensitive)</param>
		/// <param name="house">house number without complement</param>
		/// <param name="houseAlpha">alpha house number complement (non case-sensitive)</param>
		/// <returns></returns>
		public int? GetMessenger(string zip, string zipAddon, string street, string house, string houseAlpha)
		{
			if (houseAlpha == null)
			{
				return this.GetMessengerRelaxed (zip, zipAddon, street, house);
			}

			this.MessengerCommand.Parameters["@zip"].Value = zip;
			this.MessengerCommand.Parameters["@zip_addon"].Value = zipAddon;
			this.MessengerCommand.Parameters["@street"].Value = street;
			this.MessengerCommand.Parameters["@house"].Value = house;
			this.MessengerCommand.Parameters["@house_alpha"].Value = houseAlpha;

			using (var dr = this.MessengerCommand.ExecuteReader ())
			{
				dr.Read ();
				return dr.HasRows ? (int) dr.GetInt64 (0) : (int?) null;
			}
		}

		/// <summary>
		/// Get a list of houses number from a street
		/// Ex: GetHousesAtStreet("1000","06","avenue floréal")
		/// </summary>
		/// <param name="zip">zip code of street</param>
		/// <param name="zip_addon">zip code addon of street</param>
		/// <param name="street">street name</param>
		/// <returns></returns>
		public List<string> GetHousesAtStreet(string zip, string zip_addon, string street)
		{
			this.HousesAtStreetCommand.Parameters["@street"].Value = street;
			this.HousesAtStreetCommand.Parameters["@zip"].Value = zip;
			this.HousesAtStreetCommand.Parameters["@zip_addon"].Value = zip_addon;
			var result = new List<string> ();
			using (var dr = this.HousesAtStreetCommand.ExecuteReader ())
			{
				while (dr.Read ())
				{
					result.Add (dr.GetValue (0).ToString ());
				}
			}
			return result;
		}


		#region IDisposable Members

		public void Dispose()
		{
			if (this.Conn != null)
			{
				MatchSortEtl.DisposeSQLiteObject (this.Command);
				this.Command = null;

				MatchSortEtl.DisposeSQLiteObject (this.InsertCommunityCommand);
				MatchSortEtl.DisposeSQLiteObject (this.InsertHouseCommand);
				MatchSortEtl.DisposeSQLiteObject (this.InsertMessengerCommand);
				MatchSortEtl.DisposeSQLiteObject (this.InsertPlaceAltCommand);
				MatchSortEtl.DisposeSQLiteObject (this.insertPlaceCommand);
				MatchSortEtl.DisposeSQLiteObject (this.InsertStreetCommand);
				MatchSortEtl.DisposeSQLiteObject (this.HousesAtStreetCommand);
				MatchSortEtl.DisposeSQLiteObject (this.MessengerCommand);
				MatchSortEtl.DisposeSQLiteObject (this.MessengerCommandRelaxed);
				MatchSortEtl.DisposeSQLiteObject (this.Conn);
				this.Conn=null;
			}
		}

		#endregion

		private static void DisposeSQLiteObject(System.IDisposable obj)
		{
			if (obj != null)
			{
				obj.Dispose ();
			}
		}

		private void OpenDatabase()
		{
			this.Conn = new SQLiteConnection ("Data Source=MatchSort.sqlite;Version=3;");
			this.Conn.Open ();

			//SET Journal mode in WAL
			this.Command = new SQLiteCommand (this.Conn);
			this.Command.CommandText = 
				"PRAGMA journal_mode=WAL;" +
				"PRAGMA cache_size = 10000;PRAGMA synchronous=OFF;PRAGMA count_changes=OFF;PRAGMA temp_store = 2";

			this.Command.ExecuteNonQuery ();
		}

		private void CreateTableIfNeededAndResetDb()
		{
			this.Transaction = this.Conn.BeginTransaction ();

			this.Command.CommandText = Queries.CreateTableHeader;
			this.Command.ExecuteNonQuery ();

			this.Command.CommandText = Queries.CreateTablePlace1;
			this.Command.ExecuteNonQuery ();

			this.Command.CommandText = Queries.CreateTablePlace2;
			this.Command.ExecuteNonQuery ();

			this.Command.CommandText = Queries.CreateTableCommun;
			this.Command.ExecuteNonQuery ();

			this.Command.CommandText = Queries.CreateTableStreet;
			this.Command.ExecuteNonQuery ();

			this.Command.CommandText = Queries.CreateTableHouse1;
			this.Command.ExecuteNonQuery ();

			this.Command.CommandText = Queries.CreateTableDeliver;
			this.Command.ExecuteNonQuery ();

			this.Transaction.Commit ();
		}

		private SQLiteCommand BuildInsertPlace()
		{
			var sql = "insert into new_plz1 ("
					+ "onrp,bfsnr,plz_typ,plz,"
					+ "plz_zz,gplz,ort_bez_18,"
					+ "ort_bez_27,kanton,sprachcode,"
					+ "sprachcode_abw,briefz_durch,"
					+ "gilt_ab_dat,plz_briefzust,plz_coff)"
					+ " values "
					+ "(@1,@2,@3,@4,@5,@6,@7,@8,@9,@10,@11,@12,@13,@14,@15)";

			var command = new SQLiteCommand (this.Conn);
			command.CommandText = sql;
			command.Parameters.Add ("@1", System.Data.DbType.String);
			command.Parameters.Add ("@2", System.Data.DbType.String);
			command.Parameters.Add ("@3", System.Data.DbType.String);
			command.Parameters.Add ("@4", System.Data.DbType.String);
			command.Parameters.Add ("@5", System.Data.DbType.String);
			command.Parameters.Add ("@6", System.Data.DbType.String);
			command.Parameters.Add ("@7", System.Data.DbType.String);
			command.Parameters.Add ("@8", System.Data.DbType.String);
			command.Parameters.Add ("@9", System.Data.DbType.String);
			command.Parameters.Add ("@10", System.Data.DbType.String);
			command.Parameters.Add ("@11", System.Data.DbType.String);
			command.Parameters.Add ("@12", System.Data.DbType.String);
			command.Parameters.Add ("@13", System.Data.DbType.String);
			command.Parameters.Add ("@14", System.Data.DbType.String);
			command.Parameters.Add ("@15", System.Data.DbType.String);
			command.Prepare ();
			return command;
		}

		private SQLiteCommand BuildInsertPlaceAlt()
		{
			var sql = "insert into new_plz2 ("
					+ "onrp,laufnummer,bez_typ,"
					+ "sprachcode,ort_bez_18,"
					+ "ort_bez_27)"
					+ " values "
					+ "(@1,@2,@3,@4,@5,@6)";

			var command = new SQLiteCommand (this.Conn);
			command.CommandText = sql;
			command.Parameters.Add ("@1", System.Data.DbType.String);
			command.Parameters.Add ("@2", System.Data.DbType.String);
			command.Parameters.Add ("@3", System.Data.DbType.String);
			command.Parameters.Add ("@4", System.Data.DbType.String);
			command.Parameters.Add ("@5", System.Data.DbType.String);
			command.Parameters.Add ("@6", System.Data.DbType.String);
			command.Prepare ();
			return command;
		}

		private SQLiteCommand BuildInsertCommunity()
		{
			var sql = "insert into new_com ("
					+ "bfsnr,gemeindename,kanton,agglonr)"
					+ " values (@1,@2,@3,@4)";

			var command = new SQLiteCommand (this.Conn);
			command.CommandText = sql;
			command.Parameters.Add ("@1", System.Data.DbType.String);
			command.Parameters.Add ("@2", System.Data.DbType.String);
			command.Parameters.Add ("@3", System.Data.DbType.String);
			command.Parameters.Add ("@4", System.Data.DbType.String);
			command.Prepare ();
			return command;
		}

		private SQLiteCommand BuildInsertStreet()
		{
			var sql = "insert into new_str ("
					+ "str_id,onrp,str_bez_k,str_bez_l,str_bez_2k,"
					+ "str_bez_2l,str_lok_typ,str_bez_spc,str_bez_coff,"
					+ "str_ganzfach,str_fach_onrp)"
					+ " values (@1,@2,@3,@4,@5,@6,@7,@8,@9,@10,@11)";

			var command = new SQLiteCommand (this.Conn);
			command.CommandText = sql;
			command.Parameters.Add ("@1", System.Data.DbType.String);
			command.Parameters.Add ("@2", System.Data.DbType.String);
			command.Parameters.Add ("@3", System.Data.DbType.String);
			command.Parameters.Add ("@4", System.Data.DbType.String);
			command.Parameters.Add ("@5", System.Data.DbType.String);
			command.Parameters.Add ("@6", System.Data.DbType.String);
			command.Parameters.Add ("@7", System.Data.DbType.String);
			command.Parameters.Add ("@8", System.Data.DbType.String);
			command.Parameters.Add ("@9", System.Data.DbType.String);
			command.Parameters.Add ("@10", System.Data.DbType.String);
			command.Parameters.Add ("@11", System.Data.DbType.String);
			command.Prepare ();
			return command;
		}

		private SQLiteCommand BuildInsertHouse()
		{
			var sql = "insert into new_geb ("
					+ "hauskey,str_id,hnr,hnr_a,hnr_coff,ganzfach,fach_onrp)"
					+ " values (@1,@2,@3,@4,@5,@6,@7)";

			var command = new SQLiteCommand (this.Conn);
			command.CommandText = sql;
			command.Parameters.Add ("@1", System.Data.DbType.String);
			command.Parameters.Add ("@2", System.Data.DbType.String);
			command.Parameters.Add ("@3", System.Data.DbType.String);
			command.Parameters.Add ("@4", System.Data.DbType.String);
			command.Parameters.Add ("@5", System.Data.DbType.String);
			command.Parameters.Add ("@6", System.Data.DbType.String);
			command.Parameters.Add ("@7", System.Data.DbType.String);
			command.Prepare ();
			return command;
		}

		private SQLiteCommand BuildInsertMessenger()
		{
			var sql = "insert into new_bot_b ("
					+ "hauskey,a_plz,bbz_plz,boten_bez,"
					+ "etappen_nr,lauf_nr,ndepot)"
					+ " values  (@1,@2,@3,@4,@5,@6,@7)";

			var command = new SQLiteCommand (this.Conn);
			command.CommandText = sql;
			command.Parameters.Add ("@1", System.Data.DbType.String);
			command.Parameters.Add ("@2", System.Data.DbType.String);
			command.Parameters.Add ("@3", System.Data.DbType.String);
			command.Parameters.Add ("@4", System.Data.DbType.String);
			command.Parameters.Add ("@5", System.Data.DbType.String);
			command.Parameters.Add ("@6", System.Data.DbType.String);
			command.Parameters.Add ("@7", System.Data.DbType.String);
			command.Prepare ();
			return command;
		}

		private void LoadFromDatabaseCsv(string CsvFilePath)
		{
			this.Transaction = this.Conn.BeginTransaction ();
			var CommitIndex = 0;

			//Parse CSV and extract line fields -> INSERT
			foreach (var lineFields in File.ReadLines (CsvFilePath, Encoding.GetEncoding ("Windows-1252")).Select (l => l.Replace ("' ", "'").Split (';')))
			{
				switch (lineFields[0])
				{
					case "00":

						this.Command.CommandText = "insert into new_hea (vdat,zcode) values (@1,@2)";
						this.Command.Parameters.AddWithValue ("@1", lineFields[1]);
						this.Command.Parameters.AddWithValue ("@2", lineFields[2]);
						this.Command.ExecuteNonQuery ();
						break;

					case "01":
						this.insertPlaceCommand.Parameters["@1"].Value = lineFields[1];
						this.insertPlaceCommand.Parameters["@2"].Value = lineFields[2];
						this.insertPlaceCommand.Parameters["@3"].Value = lineFields[3];
						this.insertPlaceCommand.Parameters["@4"].Value = lineFields[4];
						this.insertPlaceCommand.Parameters["@5"].Value = lineFields[5];
						this.insertPlaceCommand.Parameters["@6"].Value = lineFields[6];
						this.insertPlaceCommand.Parameters["@7"].Value = lineFields[7];
						this.insertPlaceCommand.Parameters["@8"].Value = lineFields[8];
						this.insertPlaceCommand.Parameters["@9"].Value = lineFields[9];
						this.insertPlaceCommand.Parameters["@10"].Value = lineFields[10];
						this.insertPlaceCommand.Parameters["@11"].Value = lineFields[11];
						this.insertPlaceCommand.Parameters["@12"].Value = lineFields[12];
						this.insertPlaceCommand.Parameters["@13"].Value = lineFields[13];
						this.insertPlaceCommand.Parameters["@14"].Value = lineFields[14];
						this.insertPlaceCommand.Parameters["@15"].Value = lineFields[15];
						this.insertPlaceCommand.ExecuteNonQuery ();
						break;

					case "02":
						this.InsertPlaceAltCommand.Parameters["@1"].Value = lineFields[1];
						this.InsertPlaceAltCommand.Parameters["@2"].Value = lineFields[2];
						this.InsertPlaceAltCommand.Parameters["@3"].Value = lineFields[3];
						this.InsertPlaceAltCommand.Parameters["@4"].Value = lineFields[4];
						this.InsertPlaceAltCommand.Parameters["@5"].Value = lineFields[5];
						this.InsertPlaceAltCommand.Parameters["@6"].Value = lineFields[6];
						this.InsertPlaceAltCommand.ExecuteNonQuery ();
						break;

					case "03":
						this.InsertCommunityCommand.Parameters["@1"].Value = lineFields[1];
						this.InsertCommunityCommand.Parameters["@2"].Value = lineFields[2];
						this.InsertCommunityCommand.Parameters["@3"].Value = lineFields[3];
						this.InsertCommunityCommand.Parameters["@4"].Value = lineFields[4];
						this.InsertCommunityCommand.ExecuteNonQuery ();
						break;

					case "04":
						this.InsertStreetCommand.Parameters["@1"].Value = lineFields[1];
						this.InsertStreetCommand.Parameters["@2"].Value = lineFields[2];
						this.InsertStreetCommand.Parameters["@3"].Value = lineFields[3];
						this.InsertStreetCommand.Parameters["@4"].Value = lineFields[4];
						this.InsertStreetCommand.Parameters["@5"].Value = lineFields[5];
						this.InsertStreetCommand.Parameters["@6"].Value = lineFields[6];
						this.InsertStreetCommand.Parameters["@7"].Value = lineFields[7];
						this.InsertStreetCommand.Parameters["@8"].Value = lineFields[8];
						this.InsertStreetCommand.Parameters["@9"].Value = lineFields[9];
						this.InsertStreetCommand.Parameters["@10"].Value = lineFields[10];
						this.InsertStreetCommand.Parameters["@11"].Value = lineFields[11];
						this.InsertStreetCommand.ExecuteNonQuery ();
						break;

					case "05":
						//this.streetNamesAltLang.Add(compKey, new NEW_STRA (lineFields[1], lineFields[2], lineFields[3], lineFields[4], lineFields[5], lineFields[6], lineFields[7], lineFields[8], lineFields[9]));
						break;

					case "06":
						this.InsertHouseCommand.Parameters["@1"].Value = lineFields[1];
						this.InsertHouseCommand.Parameters["@2"].Value = lineFields[2];
						this.InsertHouseCommand.Parameters["@3"].Value = lineFields[3];
						this.InsertHouseCommand.Parameters["@4"].Value = lineFields[4];
						this.InsertHouseCommand.Parameters["@5"].Value = lineFields[5];
						this.InsertHouseCommand.Parameters["@6"].Value = lineFields[6];
						this.InsertHouseCommand.Parameters["@7"].Value = lineFields[7];
						this.InsertHouseCommand.ExecuteNonQuery ();
						break;

					case "07":
						//this.houseNamesAltLang.Add(compKey, new NEW_GEBA(lineFields[1], lineFields[2], lineFields[3], lineFields[4]));
						break;

					case "08":
						this.InsertMessengerCommand.Parameters["@1"].Value = lineFields[1];
						this.InsertMessengerCommand.Parameters["@2"].Value = lineFields[2];
						this.InsertMessengerCommand.Parameters["@3"].Value = lineFields[3];
						this.InsertMessengerCommand.Parameters["@4"].Value = lineFields[4];
						this.InsertMessengerCommand.Parameters["@5"].Value = lineFields[5];
						this.InsertMessengerCommand.Parameters["@6"].Value = lineFields[6];
						this.InsertMessengerCommand.Parameters["@7"].Value = lineFields[7];
						this.InsertMessengerCommand.ExecuteNonQuery ();
						break;
				}
				CommitIndex++;
				if (CommitIndex % 200000 == 0)
				{
					this.Transaction.Commit ();
					this.Transaction = this.Conn.BeginTransaction ();
				}
			}

			this.Transaction.Commit ();
		}

		private void IndexAndAnalyzeDatabase()
		{
			this.Transaction = this.Conn.BeginTransaction ();
			this.Command.CommandText = Queries.IndexAll;
			this.Command.ExecuteNonQuery ();
			this.Command.CommandText = Queries.AnalyseAll;
			this.Command.ExecuteNonQuery ();
			this.Transaction.Commit ();
		}

		private string[] GetHeaderFromCsv(string CsvFilePath)
		{
			var line = System.IO.File.ReadLines (CsvFilePath, Encoding.GetEncoding ("Windows-1252")).First ();
			var lineFields = line.Split (';');

			if (lineFields[0] == "00")
			{
				return new string[]
				{
					lineFields[1], lineFields[2]
				};
			}
			else
			{
				return null;
			}
		}

		private string[] GetHeaderFromDatabase()
		{
			string[] result = new string[2];
			this.Command.CommandText = Queries.SelectHeader;
			using (SQLiteDataReader dr = this.Command.ExecuteReader ())
			{
				dr.Read ();
				if (dr.HasRows)
				{
					result[0] = dr.GetInt64 (0).ToString ();
					result[1] = dr.GetInt64 (1).ToString ();
					return result;
				}
				else
				{
					return null;
				}

			}
		}


		public List<string> CustomQuery(string sql)
		{
			List<string> result = new List<string> ();
			this.Command.CommandText = sql;
			try
			{
				using (SQLiteDataReader dr = this.Command.ExecuteReader ())
				{
					var i = 0;
					while (dr.Read ())
					{
						var row = "";
						foreach (string k in dr.GetValues ().AllKeys)
						{
							row += String.Format ("{0}: {1}, ", k, dr.GetValues ().Get (k));
						}
						result.Add (row);
						i++;
					}


				}

			}
			catch (SQLiteException ex)
			{
				throw new Exception ("Erreur SQL: " + ex.Message.ToString ());
			}

			return result;

		}

		private SQLiteCommand BuildHousesAtStreetCommand()
		{
			var sql = "select h.hnr || h.hnr_a "
					+ "from new_geb as h "
					+ "join new_str as s on s.str_id = h.str_id "
					+ "join new_plz1 as p on p.onrp = s.onrp "
					+ "where "
					+ "s.str_bez_2l = @street collate nocase "
					+ "and p.plz = @zip "
					+ "and p.plz_zz = @zip_addon";

			var command = new SQLiteCommand (this.Conn);
			command.CommandText = sql;
			command.Parameters.Add ("@street", System.Data.DbType.String);
			command.Parameters.Add ("@zip", System.Data.DbType.String);
			command.Parameters.Add ("@zip_addon", System.Data.DbType.String);
			command.Prepare ();
			return command;
		}

		private SQLiteCommand BuildMessengerCommand()
		{
			var sql = "select b.etappen_nr "
						+ "from new_plz1 as p "
						+ "join new_str s on s.onrp = p.onrp "
						+ "join new_geb as g on g.str_id = s.str_id "
						+ "join new_bot_b as b on b.hauskey = g.hauskey "
						+ "where p.plz = @zip and p.plz_zz = @zip_addon "
						+ "and (s.str_bez_2l = @street collate nocase or s.str_bez_2k = @street collate nocase) "
						+ "and g.hnr = @house "
						+ "and g.hnr_a = @house_alpha collate nocase";

			var command = new SQLiteCommand (this.Conn);
			command.CommandText = sql;
			command.Parameters.Add ("@zip", System.Data.DbType.String);
			command.Parameters.Add ("@zip_addon", System.Data.DbType.String);
			command.Parameters.Add ("@street", System.Data.DbType.String);
			command.Parameters.Add ("@house", System.Data.DbType.String);
			command.Parameters.Add ("@house_alpha", System.Data.DbType.String);
			command.Prepare ();
			return command;
		}

		private SQLiteCommand BuildMessengerCommandRelaxed()
		{
			var sql = "select b.etappen_nr "
						+ "from new_plz1 as p "
						+ "join new_str s on s.onrp = p.onrp "
						+ "join new_geb as g on g.str_id = s.str_id "
						+ "join new_bot_b as b on b.hauskey = g.hauskey "
						+ "where p.plz = @zip and p.plz_zz = @zip_addon "
						+ "and (s.str_bez_2l = @street collate nocase or s.str_bez_2k = @street collate nocase) "
						+ "and g.hnr = @house ";

			var command = new SQLiteCommand (this.Conn);
			command.CommandText = sql;
			command.Parameters.Add ("@zip", System.Data.DbType.String);
			command.Parameters.Add ("@zip_addon", System.Data.DbType.String);
			command.Parameters.Add ("@street", System.Data.DbType.String);
			command.Parameters.Add ("@house", System.Data.DbType.String);
			command.Prepare ();
			return command;
		}

		private int? GetMessengerRelaxed(string zip, string zip_addon, string street, string house)
		{
			this.MessengerCommandRelaxed.Parameters["@zip"].Value = zip;
			this.MessengerCommandRelaxed.Parameters["@zip_addon"].Value = zip_addon;
			this.MessengerCommandRelaxed.Parameters["@street"].Value = street;
			this.MessengerCommandRelaxed.Parameters["@house"].Value = house;
			using (var dr = this.MessengerCommandRelaxed.ExecuteReader ())
			{
				dr.Read ();
				return dr.HasRows ? (int) dr.GetInt64 (0) : (int?) null;

			}
		}

		#region Queries class

		private static class Queries
		{
			public const string CreateTableHeader = "create table if not exists new_hea (vdat integer, zcode integer); delete from new_hea";
			public const string CreateTablePlace1 = "create table if not exists new_plz1 (onrp integer primary key, bfsnr integer, plz_typ integer,plz integer,plz_zz varchar(2), gplz integer,ort_bez_18 varchar(18),ort_bez_27 varchar(27),kanton varchar(2),sprachcode integer,sprachcode_abw integer,briefz_durch integer,gilt_ab_dat date(8),plz_briefzust integer,plz_coff varchar(1)); delete from new_plz1";
			public const string CreateTablePlace2 = "create table if not exists new_plz2 (onrp integer,laufnummer integer,bez_typ integer,sprachcode integer,ort_bez_18 varchar(18),ort_bez_27 varchar(27)); delete from new_plz2";
			public const string CreateTableCommun = "create table if not exists new_com (bfsnr integer primary key,gemeindename varchar(30),kanton varchar(2),agglonr integer); delete from new_com";
			public const string CreateTableStreet = "create table if not exists new_str (str_id integer primary key,onrp integer,str_bez_k varchar(25),str_bez_l varchar(60),str_bez_2k varchar(25),str_bez_2l varchar(60),str_lok_typ integer,str_bez_spc integer,str_bez_coff varchar(1),str_ganzfach varchar(1),str_fach_onrp integer); delete from new_str";
			//todo new_stra
			public const string CreateTableHouse1 = "create table if not exists new_geb (hauskey integer primary key,str_id integer,hnr integer,hnr_a varchar(6),hnr_coff varchar(1),ganzfach varchar(1),fach_onrp integer); delete from new_geb";
			//todo new_geba
			public const string CreateTableDeliver = "create table if not exists new_bot_b (hauskey integer,a_plz integer,bbz_plz integer,boten_bez integer,etappen_nr integer,lauf_nr integer,ndepot varchar(60)); delete from new_bot_b";
			public const string IndexAll =
			"create index if not exists idx_zip on new_plz1(plz,plz_zz);" +
			"create index if not exists idx_street_k on new_str(str_bez_2k collate nocase);" +
			"create index if not exists idx_street_l on new_str(str_bez_2l collate nocase);" +
			"create index if not exists idx_hnr on new_geb(str_id,hnr,hnr_a collate nocase);" +
			"create index if not exists idx_fhk on new_bot_b(hauskey)";
			public const string AnalyseAll = "analyze new_plz1;analyze new_str;analyze new_geb;analyze new_bot_b";
			public const string SelectHeader = "select vdat,zcode from new_hea";
		}

		#endregion

		private readonly SQLiteCommand			insertPlaceCommand;
		private readonly SQLiteCommand			InsertPlaceAltCommand;
		private readonly SQLiteCommand			InsertCommunityCommand;
		private readonly SQLiteCommand			InsertStreetCommand;
		private readonly SQLiteCommand			InsertHouseCommand;
		private readonly SQLiteCommand			InsertMessengerCommand;
		private readonly SQLiteCommand			HousesAtStreetCommand;
		private readonly SQLiteCommand			MessengerCommand;
		private readonly SQLiteCommand			MessengerCommandRelaxed;
		private SQLiteConnection				Conn;
		private SQLiteCommand					Command;
		private SQLiteTransaction				Transaction;
	}
}
