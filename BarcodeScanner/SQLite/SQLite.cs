using Dapper;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace BarcodeScanner
{
	class SQLite
	{
		private static string LoadConnectionString(string id = "Default")
		{
			return ConfigurationManager.ConnectionStrings[id].ConnectionString;
		}

		public static bool QBarcodeSerial(string s)
		{
			using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
			{
				StringBuilder sb = new StringBuilder();
				string str = $"SELECT * FROM Barcode WHERE Serial =";
				sb.Append(str + '"' + s + '"');

				var output = cnn.Query<string>(sb.ToString(), new DynamicParameters());

				if (output.Count() != 0)
				{
					return false;
				}
				return true;
			}
		}

		public static bool WriteBarcode(object v)
		{
			using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
			{
				try
				{
					cnn.Execute("INSERT INTO Barcode (DateTime, Operator, Serial, Fixture) values (@_DateTime, @_Operator, @_RawIN, @_Fixture)", v);
				}
				catch (SQLiteException e)
				{
					if (e.ErrorCode.ToString() == "19")
					{
						return false;
					}
				}
				return true;

			}
		}
	}
}
