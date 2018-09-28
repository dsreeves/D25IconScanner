using BarcodeScanner.Models;
using Dapper;
using System;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace BarcodeScanner
{
	class SQLite
	{
		/// <summary>
		/// Amount of time(m) until a barcode can be overwritten 
		/// </summary>
		//private static int TimeRng = 5;

		private static string LoadConnectionString(string id = "Default")
		{
			return ConfigurationManager.ConnectionStrings[id].ConnectionString;
		}

		/// <summary>
		/// Check for serial and return result
		/// </summary>
		/// <param name="s">Serial number to check for</param>
		/// <returns>True if none exist</returns>
		public static bool QBarcodeSerial(string s)
		{
			using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
			{
				StringBuilder sb = new StringBuilder();
				string str = $"SELECT * FROM Barcode WHERE Serial =";
				sb.Append(str + '"' + s + '"');

				var output = cnn.Query<Barcode>(sb.ToString(), new DynamicParameters());
				//var temp = output.ToList();
				//var Tdelta = DateTime.Parse(temp[0].DateTime).Subtract(DateTime.Now).Minutes;


				if (output.Count() != 0)
				{
					return false;
				}
				return true;
			}
		}

		/// <summary>
		/// Write Barcode to DB
		/// </summary>
		/// <param name="v">Barcode Object</param>
		/// <returns>True if successful</returns>
		public static bool WriteBarcode(Barcode v)
		{
			using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
			{
				try
				{
					cnn.Execute("INSERT INTO Barcode (DateTime, Operator, Serial, Fixture) values (@DateTime, @Operator, @Serial, @Fixture)", v);
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

		/// <summary>
		/// Replace Barcode in DB
		/// </summary>
		/// <param name="v"></param>
		/// <returns></returns>
		public static bool UpdateBarcode(Barcode v)
		{
			using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
			{
				try
				{
					cnn.Execute("UPDATE Barcode SET Datetime = @DateTime, Operator = @Operator, Serial = @Serial, Fixture = @Fixture WHERE Serial = @Serial", v);
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

		/// <summary>
		/// Delete record
		/// </summary>
		/// <param name="v"></param>
		/// <returns></returns>
		public static bool DeleteBarcode(Barcode v)
		{
			using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
			{
				try
				{
					cnn.Execute("DELETE FROM Barcode WHERE Serial = @Serial", v);
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
