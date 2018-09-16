using System;
using IniParser;
using IniParser.Model;

namespace BarcodeScanner.Models
{
	public class IniParser
	{
		private IniData Data;
		public IniData _data { get; private set; }
		public bool ParseComplete { get; protected set; } = true;

		//public List<string> sections = new List<string>();
		//public int numberOfSections { get; protected set; }

		public IniParser()
		{
			FileIniDataParser parser = new FileIniDataParser();
			try { Data = parser.ReadFile("Scanner.ini"); }
			catch (Exception) { ParseComplete = false; }
			_data = Data;
		}

		private void SetData()
		{
			//numberOfSections = data.Sections.Count;

			foreach (SectionData section in Data.Sections)
			{

				Console.WriteLine("[" + section.SectionName + "]");

				foreach (KeyData key in section.Keys)
					Console.WriteLine(key.KeyName + " = " + key.Value);
			}
			Console.ReadLine();
		}
	}
}
