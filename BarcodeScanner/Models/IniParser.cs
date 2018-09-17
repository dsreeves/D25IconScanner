using System;
using System.Collections.Generic;
using IniParser;
using IniParser.Model;

namespace BarcodeScanner.Models
{
	public class IniParse
	{
		public IniData Data { get; set; }
		public bool ParseComplete { get; protected set; } = true;
		

		public IniParse(string f)
		{
			FileIniDataParser parser = new FileIniDataParser();
			try { Data = parser.ReadFile(f); }
			catch (Exception) { ParseComplete = false; }
		}


		private void SetData()
		{
			foreach (SectionData section in Data.Sections)
			{




				Console.WriteLine("[" + section.SectionName + "]");

				foreach (KeyData key in section.Keys)
					Console.WriteLine(key.KeyName + " = " + key.Value);
			}
			Console.ReadLine();
		}

		/// <summary>
		/// Returns K/V from ini file
		/// </summary>
		/// <param name="section">Section name to get key/values from</param>
		/// <returns></returns>
		public Dictionary<string,string> GetSection(string section)
		{
			Dictionary<string, string> rOut = new Dictionary<string, string>();
			var sd = Data.Sections.GetSectionData(section);

			foreach (KeyData k in sd.Keys)
			{
				rOut.Add(k.KeyName, k.Value);
			}
			return rOut;
		}

		/// <summary>
		/// Get known key's value
		/// </summary>
		/// <param name="section"></param>
		/// <param name="key"></param>
		/// <returns></returns>
		public string GetValue(string section, string key)
		{
			try
			{
				return Data[$"{section}"][$"{key}"];
			}
			catch (Exception)
			{
				return $"{key} does not exist in {section}";
			}
		}
	}
}
