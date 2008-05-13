//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Database;

using FirebirdSql.Data.FirebirdClient;

namespace Epsitec.Cresus.Database.Implementation
{
	/// <summary>
	/// The <c>FirebirdAbstractionFactory</c> class implements the <c>IDbAbstractionFactory</c>
	/// interface for the Firebird engine.
	/// </summary>
	internal sealed class FirebirdAbstractionFactory : IDbAbstractionFactory
	{
		public FirebirdAbstractionFactory()
		{
			DbFactory.RegisterDatabaseAbstraction (this);
		}
		
		#region IDbAbstractionFactory Members

		public IDbAbstraction CreateDatabaseAbstraction(DbAccess dbAccess)
		{
			System.Diagnostics.Debug.Assert (dbAccess.Provider == this.ProviderName);

			FirebirdAbstraction fb = new FirebirdAbstraction (dbAccess, this, EngineType.Server);
			
			return fb;
		}

		public string							ProviderName
		{
			get
			{
				return "Firebird";
			}
		}

		public ITypeConverter					TypeConverter
		{
			get
			{
				return this.typeConverter;
			}
		}

		public string[] QueryDatabaseFilePaths(DbAccess dbAccess)
		{
			return new string[]
			{
				FirebirdAbstraction.MakeDbFilePath (dbAccess, EngineType.Server)
			};
		}

		#endregion
		
		private FirebirdTypeConverter			typeConverter = new FirebirdTypeConverter ();
	}

#if false
	static class CarlosTest
	{
		static void Test()
		{
			FbConnectionStringBuilder csb = new FbConnectionStringBuilder();
			
			csb.Database = @"C:\utf8db.fdb";
			csb.DataSource = "localhost";
			csb.UserID = "sysdba";
			csb.Password = "masterkey";
			csb.Pooling = false;
			csb.Charset = "UTF8";

			FbConnection.CreateDatabase (csb.ToString (), 8192, true, false);
			
			using (FbConnection c = new FbConnection (csb.ToString ()))
			{
				c.Open ();

				using (FbCommand create = new FbCommand ("create table table_name (x char(32) character set utf8)", c))
				{
					create.ExecuteNonQuery ();
				}

				string value = "На берегу пустынных волн";

				using (FbCommand insert = new FbCommand ("insert into table_name values (@value)", c))
				{
					insert.Parameters.Add ("@value", FbDbType.Char, 32).Value = value;

					insert.ExecuteNonQuery ();
				}

				using (FbCommand select = new FbCommand ("select * from table_name", c))
				{
					using (FbDataAdapter adapter = new FbDataAdapter (select))
					{
						System.Data.ITableMapping mapping = adapter.TableMappings.Add ("table_name", "My Table");
						mapping.ColumnMappings.Add ("x", "My Text");
						System.Data.DataSet dataSet = new System.Data.DataSet ();
						adapter.MissingSchemaAction = System.Data.MissingSchemaAction.AddWithKey;
						adapter.Fill (dataSet);
					}
				}
			}
		}
	}
#endif
}
