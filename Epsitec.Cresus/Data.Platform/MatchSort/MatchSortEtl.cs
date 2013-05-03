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

		public void Dispose()
		{
			if (Conn != null)
			{
				MatchSortEtl.DisposeSQLiteObject (this.Command);
				this.Command = null;

				MatchSortEtl.DisposeSQLiteObject (this.InsertCommunityCommand);
				MatchSortEtl.DisposeSQLiteObject (this.InsertHouseCommand);
				MatchSortEtl.DisposeSQLiteObject (this.InsertMessengerCommand);
				MatchSortEtl.DisposeSQLiteObject (this.InsertPlaceAltCommand);
				MatchSortEtl.DisposeSQLiteObject (this.InsertPlaceCommand);
				MatchSortEtl.DisposeSQLiteObject (this.InsertStreetCommand);
				MatchSortEtl.DisposeSQLiteObject (this.HousesAtStreetCommand);
				MatchSortEtl.DisposeSQLiteObject (this.MessengerCommand);
				MatchSortEtl.DisposeSQLiteObject (this.MessengerCommandRelaxed);
				MatchSortEtl.DisposeSQLiteObject (this.Conn);
				Conn=null;
			}
		}

		private static void DisposeSQLiteObject(System.IDisposable obj)
		{
			if (obj != null)
			{
				obj.Dispose ();
			}
		}

		/// <summary>
		/// Perform ETL job on Mat[CH]sort CSV file and load content for querying in SQLite
		/// https://match.post.ch/pdf/post-match-new-sort.pdf
		/// </summary>
		public MatchSortEtl(string CsvFilePath)
		{
		   
			try
			{
				var DatabaseDirectoryPath = Epsitec.Common.Support.Globals.ExecutableDirectory;
				var DatabaseFilePath = DatabaseDirectoryPath + "\\MatchSort.sqlite";
				if (!File.Exists(DatabaseFilePath))
				{
					//CASE NO DATABASE
					SQLiteConnection.CreateFile("MatchSort.sqlite");
					this.OpenDatabase();
					this.CreateTableIfNeededAndResetDb();

					this.InsertPlaceCommand = this.BuildInsertPlace();
					this.InsertPlaceAltCommand = this.BuildInsertPlaceAlt();
					this.InsertCommunityCommand = this.BuildInsertCommunity();
					this.InsertStreetCommand = this.BuildInsertStreet();
					this.InsertHouseCommand = this.BuildInsertHouse();
					this.InsertMessengerCommand = this.BuildInsertMessenger();

					this.LoadFromDatabaseCsv(CsvFilePath);
					this.IndexAndAnalyzeDatabase();
				}
				else
				{
					//CASE CHECK FOR UPDATE
					this.OpenDatabase();

					var VersionCsv = this.GetHeaderFromCsv(CsvFilePath);
					var VersionDb = this.GetHeaderFromDatabase();

					if (VersionCsv[0] != VersionDb[0] || VersionCsv[1] != VersionDb[1])
					{
						this.CreateTableIfNeededAndResetDb();

						this.InsertPlaceCommand = this.BuildInsertPlace();
						this.InsertPlaceAltCommand = this.BuildInsertPlaceAlt();
						this.InsertCommunityCommand = this.BuildInsertCommunity();
						this.InsertStreetCommand = this.BuildInsertStreet();
						this.InsertHouseCommand = this.BuildInsertHouse();
						this.InsertMessengerCommand = this.BuildInsertMessenger();

						this.LoadFromDatabaseCsv(CsvFilePath);
						this.IndexAndAnalyzeDatabase();
					}
				}


				//Prepare and Build SQL Commands
				this.MessengerCommand = this.BuildMessengerCommand ();
				this.MessengerCommandRelaxed = this.BuildMessengerCommandRelaxed ();
				this.HousesAtStreetCommand = this.BuildHousesAtStreetCommand();

			}
			catch (Exception ex)
			{
				this.Dispose();
				throw new Exception ("Problem while loading the database", ex);
			}
			
		}

		private readonly SQLiteCommand InsertPlaceCommand;
		private readonly SQLiteCommand InsertPlaceAltCommand;
		private readonly SQLiteCommand InsertCommunityCommand;
		private readonly SQLiteCommand InsertStreetCommand;
		private readonly SQLiteCommand InsertHouseCommand;
		private readonly SQLiteCommand InsertMessengerCommand;
		private readonly SQLiteCommand HousesAtStreetCommand;
		private readonly SQLiteCommand MessengerCommand;
		private readonly SQLiteCommand MessengerCommandRelaxed;
		private SQLiteConnection Conn;
		private SQLiteCommand Command;
		private SQLiteTransaction Transaction;

		private const string CreateTableHeader = "create table if not exists new_hea (vdat number(8), zcode number(6)); delete from new_hea";
		private const string CreateTablePlace1 = "create table if not exists new_plz1 (onrp number(5) primary key, bfsnr number(5), plz_typ number(2),plz number(4),plz_zz varchar(2), gplz number(4),ort_bez_18 varchar(18),ort_bez_27 varchar(27),kanton varchar(2),sprachcode number(1),sprachcode_abw number(1),briefz_durch number(5),gilt_ab_dat date(8),plz_briefzust number(6),plz_coff varchar(1)); delete from new_plz1";
		private const string CreateTablePlace2 = "create table if not exists new_plz2 (onrp number(5),laufnummer number(3),bez_typ number(1),sprachcode number(1),ort_bez_18 varchar(18),ort_bez_27 varchar(27)); delete from new_plz2";
		private const string CreateTableCommun = "create table if not exists new_com (bfsnr number(5) primary key,gemeindename varchar(30),kanton varchar(2),agglonr number(5)); delete from new_com";
		private const string CreateTableStreet = "create table if not exists new_str (str_id number(10) primary key,onrp number(5),str_bez_k varchar(25),str_bez_l varchar(60),str_bez_2k varchar(25),str_bez_2l varchar(60),str_lok_typ number(1),str_bez_spc number(1),str_bez_coff varchar(1),str_ganzfach varchar(1),str_fach_onrp number(5)); delete from new_str";
		//todo new_stra
		private const string CreateTableHouse1 = "create table if not exists new_geb (hauskey number(13) primary key,str_id number(10),hnr number(4),hnr_a varchar(6),hnr_coff varchar(1),ganzfach varchar(1),fach_onrp number(5)); delete from new_geb";
		//todo new_geba
		private const string CreateTableDeliver = "create table if not exists new_bot_b (hauskey number(13),a_plz number(6),bbz_plz number(6),boten_bez number(4),etappen_nr number(3),lauf_nr number(6),ndepot varchar(60)); delete from new_bot_b";
		private const string IndexAll =
			"create index if not exists idx_zip on new_plz1(plz,plz_zz);" +
			"create index if not exists idx_street_k on new_str(str_bez_2k collate nocase);" +
			"create index if not exists idx_street_l on new_str(str_bez_2l collate nocase);" +
			"create index if not exists idx_hnr on new_geb(str_id,hnr,hnr_a collate nocase);" +
			"create index if not exists idx_fhk on new_bot_b(hauskey)";
		private const string AnalyseAll = "analyze new_plz1;analyze new_str;analyze new_geb;analyze new_bot_b";
		private const string SelectHeader = "select vdat,zcode from new_hea";
 

		private void OpenDatabase()
		{
			this.Conn = new SQLiteConnection("Data Source=MatchSort.sqlite;Version=3;");
			this.Conn.Open();

			//SET Journal mode in WAL
			this.Command = new SQLiteCommand(this.Conn);
			this.Command.CommandText = "PRAGMA journal_mode=WAL;PRAGMA cache_size = 10000;PRAGMA synchronous=OFF;PRAGMA count_changes=OFF;PRAGMA temp_store = 2";

			this.Command.ExecuteNonQuery();
		}

		private void CreateTableIfNeededAndResetDb()
		{
			this.Transaction = this.Conn.BeginTransaction();

			this.Command.CommandText = MatchSortEtl.CreateTableHeader;
			this.Command.ExecuteNonQuery();

			this.Command.CommandText = MatchSortEtl.CreateTablePlace1;
			this.Command.ExecuteNonQuery();

			this.Command.CommandText = MatchSortEtl.CreateTablePlace2;
			this.Command.ExecuteNonQuery();

			this.Command.CommandText = MatchSortEtl.CreateTableCommun;
			this.Command.ExecuteNonQuery();

			this.Command.CommandText = MatchSortEtl.CreateTableStreet;
			this.Command.ExecuteNonQuery();

			this.Command.CommandText = MatchSortEtl.CreateTableHouse1;
			this.Command.ExecuteNonQuery();

			this.Command.CommandText = MatchSortEtl.CreateTableDeliver;
			this.Command.ExecuteNonQuery();

			this.Transaction.Commit();
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

			var command = new SQLiteCommand(this.Conn);
			command.CommandText = sql;
			command.Parameters.Add("@1", System.Data.DbType.String);
			command.Parameters.Add("@2", System.Data.DbType.String);
			command.Parameters.Add("@3", System.Data.DbType.String);
			command.Parameters.Add("@4", System.Data.DbType.String);
			command.Parameters.Add("@5", System.Data.DbType.String);
			command.Parameters.Add("@6", System.Data.DbType.String);
			command.Parameters.Add("@7", System.Data.DbType.String);
			command.Parameters.Add("@8", System.Data.DbType.String);
			command.Parameters.Add("@9", System.Data.DbType.String);
			command.Parameters.Add("@10", System.Data.DbType.String);
			command.Parameters.Add("@11", System.Data.DbType.String);
			command.Parameters.Add("@12", System.Data.DbType.String);
			command.Parameters.Add("@13", System.Data.DbType.String);
			command.Parameters.Add("@14", System.Data.DbType.String);
			command.Parameters.Add("@15", System.Data.DbType.String);
			command.Prepare();
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

			var command = new SQLiteCommand(this.Conn);
			command.CommandText = sql;
			command.Parameters.Add("@1", System.Data.DbType.String);
			command.Parameters.Add("@2", System.Data.DbType.String);
			command.Parameters.Add("@3", System.Data.DbType.String);
			command.Parameters.Add("@4", System.Data.DbType.String);
			command.Parameters.Add("@5", System.Data.DbType.String);
			command.Parameters.Add("@6", System.Data.DbType.String);
			command.Prepare();
			return command;
		}

		private SQLiteCommand BuildInsertCommunity()
		{
			var sql = "insert into new_com ("
					+ "bfsnr,gemeindename,kanton,agglonr)"
					+ " values (@1,@2,@3,@4)";

			var command = new SQLiteCommand(this.Conn);
			command.CommandText = sql;
			command.Parameters.Add("@1", System.Data.DbType.String);
			command.Parameters.Add("@2", System.Data.DbType.String);
			command.Parameters.Add("@3", System.Data.DbType.String);
			command.Parameters.Add("@4", System.Data.DbType.String);
			command.Prepare();
			return command;
		}

		private SQLiteCommand BuildInsertStreet()
		{
			var sql = "insert into new_str ("
					+ "str_id,onrp,str_bez_k,str_bez_l,str_bez_2k,"
					+ "str_bez_2l,str_lok_typ,str_bez_spc,str_bez_coff,"
					+ "str_ganzfach,str_fach_onrp)"
					+ " values (@1,@2,@3,@4,@5,@6,@7,@8,@9,@10,@11)";

			var command = new SQLiteCommand(this.Conn);
			command.CommandText = sql;
			command.Parameters.Add("@1", System.Data.DbType.String);
			command.Parameters.Add("@2", System.Data.DbType.String);
			command.Parameters.Add("@3", System.Data.DbType.String);
			command.Parameters.Add("@4", System.Data.DbType.String);
			command.Parameters.Add("@5", System.Data.DbType.String);
			command.Parameters.Add("@6", System.Data.DbType.String);
			command.Parameters.Add("@7", System.Data.DbType.String);
			command.Parameters.Add("@8", System.Data.DbType.String);
			command.Parameters.Add("@9", System.Data.DbType.String);
			command.Parameters.Add("@10", System.Data.DbType.String);
			command.Parameters.Add("@11", System.Data.DbType.String);
			command.Prepare();
			return command;
		}

		private SQLiteCommand BuildInsertHouse()
		{
			var sql = "insert into new_geb ("
					+ "hauskey,str_id,hnr,hnr_a,hnr_coff,ganzfach,fach_onrp)"
					+ " values (@1,@2,@3,@4,@5,@6,@7)";

			var command = new SQLiteCommand(this.Conn);
			command.CommandText = sql;
			command.Parameters.Add("@1", System.Data.DbType.String);
			command.Parameters.Add("@2", System.Data.DbType.String);
			command.Parameters.Add("@3", System.Data.DbType.String);
			command.Parameters.Add("@4", System.Data.DbType.String);
			command.Parameters.Add("@5", System.Data.DbType.String);
			command.Parameters.Add("@6", System.Data.DbType.String);
			command.Parameters.Add("@7", System.Data.DbType.String);
			command.Prepare();
			return command;
		}
		private SQLiteCommand BuildInsertMessenger()
		{
			var sql = "insert into new_bot_b ("
					+ "hauskey,a_plz,bbz_plz,boten_bez,"
					+ "etappen_nr,lauf_nr,ndepot)"
					+ " values  (@1,@2,@3,@4,@5,@6,@7)";

			var command = new SQLiteCommand(this.Conn);
			command.CommandText = sql;
			command.Parameters.Add("@1", System.Data.DbType.String);
			command.Parameters.Add("@2", System.Data.DbType.String);
			command.Parameters.Add("@3", System.Data.DbType.String);
			command.Parameters.Add("@4", System.Data.DbType.String);
			command.Parameters.Add("@5", System.Data.DbType.String);
			command.Parameters.Add("@6", System.Data.DbType.String);
			command.Parameters.Add("@7", System.Data.DbType.String);
			command.Prepare();
			return command;
		}

		private void LoadFromDatabaseCsv(string CsvFilePath)
		{
			this.Transaction = this.Conn.BeginTransaction();
			var CommitIndex = 0;

			//Parse CSV and extract line fields -> INSERT
			foreach (var lineFields in File.ReadLines(CsvFilePath, Encoding.GetEncoding("Windows-1252")).Select(l => l.Replace ("' ", "'").Split(';')))
			{
				switch (lineFields[0])
				{
					case "00":

						this.Command.CommandText = "insert into new_hea (vdat,zcode) values (@1,@2)";
						this.Command.Parameters.AddWithValue("@1", lineFields[1]);
						this.Command.Parameters.AddWithValue("@2", lineFields[2]);
						this.Command.ExecuteNonQuery();
						break;

					case "01":
						this.InsertPlaceCommand.Parameters["@1"].Value = lineFields[1];
						this.InsertPlaceCommand.Parameters["@2"].Value = lineFields[2];
						this.InsertPlaceCommand.Parameters["@3"].Value = lineFields[3];
						this.InsertPlaceCommand.Parameters["@4"].Value = lineFields[4];
						this.InsertPlaceCommand.Parameters["@5"].Value = lineFields[5];
						this.InsertPlaceCommand.Parameters["@6"].Value = lineFields[6];
						this.InsertPlaceCommand.Parameters["@7"].Value = lineFields[7];
						this.InsertPlaceCommand.Parameters["@8"].Value = lineFields[8];
						this.InsertPlaceCommand.Parameters["@9"].Value = lineFields[9];
						this.InsertPlaceCommand.Parameters["@10"].Value = lineFields[10];
						this.InsertPlaceCommand.Parameters["@11"].Value = lineFields[11];
						this.InsertPlaceCommand.Parameters["@12"].Value = lineFields[12];
						this.InsertPlaceCommand.Parameters["@13"].Value = lineFields[13];
						this.InsertPlaceCommand.Parameters["@14"].Value = lineFields[14];
						this.InsertPlaceCommand.Parameters["@15"].Value = lineFields[15];
						this.InsertPlaceCommand.ExecuteNonQuery();
						break;

					case "02":
						this.InsertPlaceAltCommand.Parameters["@1"].Value = lineFields[1];
						this.InsertPlaceAltCommand.Parameters["@2"].Value = lineFields[2];
						this.InsertPlaceAltCommand.Parameters["@3"].Value = lineFields[3];
						this.InsertPlaceAltCommand.Parameters["@4"].Value = lineFields[4];
						this.InsertPlaceAltCommand.Parameters["@5"].Value = lineFields[5];
						this.InsertPlaceAltCommand.Parameters["@6"].Value = lineFields[6];
						this.InsertPlaceAltCommand.ExecuteNonQuery();
						break;

					case "03":
						this.InsertCommunityCommand.Parameters["@1"].Value = lineFields[1];
						this.InsertCommunityCommand.Parameters["@2"].Value = lineFields[2];
						this.InsertCommunityCommand.Parameters["@3"].Value = lineFields[3];
						this.InsertCommunityCommand.Parameters["@4"].Value = lineFields[4];
						this.InsertCommunityCommand.ExecuteNonQuery();
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
						this.InsertStreetCommand.ExecuteNonQuery();
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
						this.InsertHouseCommand.ExecuteNonQuery();
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
						this.InsertMessengerCommand.ExecuteNonQuery();
						break;
				}
				CommitIndex++;
				if (CommitIndex % 200000 == 0)
				{
					this.Transaction.Commit();
					this.Transaction = this.Conn.BeginTransaction();
				}
			}

			this.Transaction.Commit();
		}

		private void IndexAndAnalyzeDatabase()
		{
			this.Transaction = this.Conn.BeginTransaction();
			this.Command.CommandText = MatchSortEtl.IndexAll;
			this.Command.ExecuteNonQuery();
			this.Command.CommandText = MatchSortEtl.AnalyseAll;
			this.Command.ExecuteNonQuery();
			this.Transaction.Commit();
		}

		private string[] GetHeaderFromCsv(string CsvFilePath)
		{
			string[] result = new string[2];

			using(StreamReader reader = new StreamReader(CsvFilePath, Encoding.GetEncoding("Windows-1252"))) 
			{
				var lineFields = reader.ReadLine().Split(';');
				if (lineFields[0] == "00")
				{
					result[0] = lineFields[1];
					result[1] = lineFields[2];
					return result;
				}
				else
				{
					return null;
				}   
			}          
		}

		private string [] GetHeaderFromDatabase()
		{
			string[] result = new string[2];
			this.Command.CommandText = MatchSortEtl.SelectHeader;
			using (SQLiteDataReader dr = this.Command.ExecuteReader())
			{
				dr.Read();
				if (dr.HasRows)
				{
					result[0] = dr.GetValue(0).ToString();
					result[1] = dr.GetValue(1).ToString();
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

			var command = new SQLiteCommand(this.Conn);
			command.CommandText = sql;
			command.Parameters.Add("@street", System.Data.DbType.String);
			command.Parameters.Add("@zip", System.Data.DbType.String);
			command.Parameters.Add("@zip_addon", System.Data.DbType.String);
			command.Prepare();
			return command;
		}
		/// <summary>
		/// Get a list of houses number from a street
		/// Ex: GetHousesAtStreet("1000","06","avenue floréal")
		/// </summary>
		/// <param name="zip">zip code of street</param>
		/// <param name="zip_addon">zip code addon of street</param>
		/// <param name="street">street name</param>
		/// <returns></returns>
		public List<string> GetHousesAtStreet(string zip, string zip_addon,string street)
		{
			this.HousesAtStreetCommand.Parameters["@street"].Value = street;
			this.HousesAtStreetCommand.Parameters["@zip"].Value = zip;
			this.HousesAtStreetCommand.Parameters["@zip_addon"].Value = zip_addon;
			var result = new List<string>();
			using (var dr = this.HousesAtStreetCommand.ExecuteReader())
			{
				while (dr.Read ())
				{
					result.Add(dr.GetValue(0).ToString());
				}
			}
			return result;
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

		/// <summary>
		/// Get the Messenger number
		/// Ex: GetMessenger("1000","06","avenue floréal","10","a")
		/// </summary>
		/// <param name="zip">4 digits zip </param>
		/// <param name="zip_addon">2 digits additionnal zip </param>
		/// <param name="street">human readable street name (non case-sensitive)</param>
		/// <param name="house">house number without complement</param>
		/// <param name="house_alpha">alpha house number complement (non case-sensitive)</param>
		/// <returns></returns>
		public string GetMessenger(string zip, string zip_addon, string street, string house, string house_alpha)
		{
			if (house_alpha == null)
			{
				return this.GetMessengerRelaxed (zip, zip_addon, street, house);
			}

			this.MessengerCommand.Parameters["@zip"].Value = zip;
			this.MessengerCommand.Parameters["@zip_addon"].Value = zip_addon;
			this.MessengerCommand.Parameters["@street"].Value = street;
			this.MessengerCommand.Parameters["@house"].Value = house;
			this.MessengerCommand.Parameters["@house_alpha"].Value = house_alpha;
			using (var dr = this.MessengerCommand.ExecuteReader ())
			{
				dr.Read ();
				return dr.HasRows ? dr.GetValue (0).ToString () : null;

			}
		}
		
		private string GetMessengerRelaxed(string zip, string zip_addon, string street, string house)
		{
			this.MessengerCommandRelaxed.Parameters["@zip"].Value = zip;
			this.MessengerCommandRelaxed.Parameters["@zip_addon"].Value = zip_addon;
			this.MessengerCommandRelaxed.Parameters["@street"].Value = street;
			this.MessengerCommandRelaxed.Parameters["@house"].Value = house;
			using (var dr = this.MessengerCommandRelaxed.ExecuteReader ())
			{
				dr.Read ();
				return dr.HasRows ? dr.GetValue (0).ToString () : null;

			}
		}
	}
}
