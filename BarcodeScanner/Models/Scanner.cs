using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Xml;
using CoreScanner;

namespace BarcodeScanner.Models
{
	public class Scanner : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

		#region Props/Fields

		/// <summary>
		/// VM binding for datagrid
		/// </summary>
		public ObservableCollection<Barcode> DataGrid { get; set; } = new ObservableCollection<Barcode>();

		/// <summary>
		/// VM binding for last scanned barcode
		/// </summary>
		public string LastScanned { get; set; } = "test";

		/// <summary>
		/// VM binding for operator
		/// </summary>
		public string Operator    { get; set; } = "test";

		/// <summary>
		/// VM binding for fixture
		/// </summary>
		public int Fixture { get; set; }

		/// <summary>
		/// Captures Sync context to marshall barcode events back to UI thread
		/// </summary>
		private SynchronizationContext uiSyncContext;

		#region Barcode Scanner API Config
		private CCoreScanner cCoreScannerClass = new CCoreScanner();
		private string outXML;
		private int status        = 1;
		private bool opReady      = false;
		private bool ScanDisabled = false;

		public bool sConnected    = false;
		public string _DateTime { get; set; }
		public string _RawIN    { get; set; }
		public enum ExecCodes
		{
			//Scanner SDK Commands
			GET_VERSION               = 1000,
			REGISTER_FOR_EVENTS       = 1001,
			UNREGISTER_FOR_EVENTS     = 1002,

			//General Commands
			AIM_OFF                   = 2002,
			AIM_ON                    = 2003,
			DEVICE_PULL_TRIGGER       = 2011,
			DEVICE_RELEASE_TRIGGER    = 2012,
			SCAN_DISABLE              = 2013,
			SCAN_ENABLE               = 2014,
			SET_PARAMETER_DEFAULTS    = 2015,
			DEVICE_SET_PARAMETERS     = 2016,
			SET_PARAMETER_PERSISTANCE = 2017,
			REBOOT_SCANNER            = 2019,

			//Operational modes
			DEVICE_CAPTURE_IMAGE      = 3000,
			DEVICE_CAPTURE_BARCODE    = 3500,
			DEVICE_CAPTURE_VIDEO      = 4000,

			//Attributes
			ATTR_GETALL               = 5000,
			ATTR_GET                  = 5001,
			ATTR_SET                  = 5004,
			ATTR_STORE                = 5005,

			//Used to invoke beeps, leds, ect..
			SET_ACTION   = 6000,
			_1HighS      = 0,
			_2HighS      = 1,
			_3HighS      = 2,
			_4HighS      = 3,
			_5HighS      = 4,
			_1LowS       = 5,
			_2LowS       = 6,
			_3LowS       = 7,
			_4LowS       = 8,
			_5LowS       = 9,
			_1HighL      = 10,
			_2HighL      = 11,
			_3HighL      = 12,
			_4HighL      = 13,
			_5HighL      = 14,
			_1LowL       = 15,
			_2LowL       = 16,
			_3LowL       = 17,
			_4LowL       = 18,
			_5LowL       = 19,
			_HighLow     = 22,
			_LowHigh     = 23,
			_HLH         = 24,
			_LHL         = 25,
			_HHLL        = 26,
			_GreenLEDOff = 42,
			_GreenLEDOn  = 43,
			_RedLEDOff   = 48,
			_RedLEDOn    = 47,

			//Start(0), Stop(1)
			HostTriggerSession = 6005,

			//Event ID's
			SUBSCRIBE_BARCODE  = 1,
			SUBSCRIBE_IMAGE    = 2,
			SUBSCRIBE_VIDEO    = 4,
			SUBSCRIBE_RMD      = 8,
			SUBSCRIBE_PNP      = 16,
			SUBSCRIBE_OTHER    = 32,
		};
		#endregion

		#endregion


		/// <summary>
		/// Main constructor
		/// </summary>
		public Scanner()
		{
			Log($"Scanner Constructor run on thread ");
			uiSyncContext = SynchronizationContext.Current;
			iniScanner();
			ScannerDebug();
		}

		/// <summary>
		/// Debug Constructor
		/// </summary>
		[Conditional("DEBUG")]
		private void ScannerDebug()
		{
			DataGrid.Add(new Barcode("Serial#123456", "12/12/12 12:12", "Operator", 1));
			DataGrid.Add(new Barcode("Serial#123456", "12/12/12 12:12", "Operator", 2));
			DataGrid.Add(new Barcode("Serial#123456", "12/12/12 12:12", "Operator", 3));
			DataGrid = new ObservableCollection<Barcode>(DataGrid.Reverse());
			DataGrid.Insert(0, new Barcode("Serial#123456", "12/12/12 12:12", "Operator", 4));
			DataGrid.RemoveAt(2);
			DataGrid.Insert(0, new Barcode("Serial#123456", "12/12/12 12:12", "Operator", 2));
		}

		/// <summary>
		/// Release mode constructor
		/// </summary>
		private void iniScanner()
		{
			short[] scannerTypes = new short[1];                        // Scanner Types you are interested in
			scannerTypes[0] = 2;                                        // 1 for all scanner types, 2 for SNAPI mode only
			short numberOfScannerTypes = 1;                             // Size of the scannerTypes array
			cCoreScannerClass.Open(0, scannerTypes, numberOfScannerTypes, out status);
			Log("Created scanner class on thread ");
			ScannerDisable();
			CreateNewBarcodeEvent();
			ScannerEnable();
		}

		/// <summary>
		/// Create barcode scan event
		/// </summary>
		private void CreateNewBarcodeEvent()
		{
			//Barcode scan event
			cCoreScannerClass.BarcodeEvent += new _ICoreScannerEvents_BarcodeEventEventHandler(OnBarcodeEvent);
			// Method for Subscribe events
			string inXML = "<inArgs>" +
								"<cmdArgs>" +
									"<arg-int>1</arg-int>" +            // Number of events you want to subscribe
									"<arg-int>1</arg-int>" +            // Comma separated event IDs
								"</cmdArgs>" +
							"</inArgs>";
			cCoreScannerClass.ExecCommand(1001, ref inXML, out outXML, out status);
			Log("Created event on thread ");
			Thread.Sleep(200);
		}

		/// <summary>
		/// Barcode scan event
		/// </summary>
		private void OnBarcodeEvent(short eventType, ref string pscanData)
		{
			Log("Barcode Event Fired on thread ");
			_DateTime = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
			string dat = pscanData;

			if (dat != "")
			{
				//Decodes the xml and stores the value in _RawIN 
				DecodeXml(dat);
				Debug.WriteLine($"Barcode => {_RawIN}");

				switch (opReady)
				{
					//Operator is scanned in 
					case true:

						//Decode scan type
						switch (_RawIN[0])
						{

							//Operator sign in
							case '#':
								InvokeBeep(ExecCodes._3HighS);
								ChangeLed(ExecCodes._RedLEDOff);
								ChangeLed(ExecCodes._GreenLEDOn);

								//Post to UI thread
								uiSyncContext.Post((s) => 
								{
									Operator = _RawIN.Substring(1);
									opReady = true;
								}, null);
								break;


							//Part Scanned
							default:
								InvokeBeep(ExecCodes._1HighL);
								ChangeLed(ExecCodes._RedLEDOff);
								var bc = new Barcode(_RawIN, _DateTime, Operator, Fixture);

								//Check for duplicate serial scan from same operator 
								foreach (var i in DataGrid)
								{
									//If not same operator then update DB/Datagrid
									if (i.Serial == _RawIN && i.Operator != Operator)
									{
										//Post to UI thread
										uiSyncContext.Post((s) =>
										{
											SQLite.UpdateBarcode(bc);
											DataGrid.RemoveAt(DataGrid.IndexOf(i));
											DataGrid.Insert(0, bc);

										}, null);
										return;
									}
									//Same operator scanned barcode twice
									else if (i.Serial == _RawIN && i.Operator == Operator)
									{
										//Post to UI thread
										uiSyncContext.Post((s) =>
										{
											SQLite.DeleteBarcode(bc);
											DataGrid.RemoveAt(DataGrid.IndexOf(i));
										}, null);
										return;
									}
								}

								//Add new barcode to Datagrid 
								uiSyncContext.Post((s) =>
								{
									if (SQLite.WriteBarcode(bc))
									{
										DataGrid.Insert(0, bc);
									}
								}, null);

								break;
						}
						break;


					//No operator is currently scanned in
					case false:
						InvokeBeep(ExecCodes._3LowL);
						ChangeLed(ExecCodes._RedLEDOn);
						break;
				}
			}
		}

		/// <summary>
		/// Checks for active connection with barcode scanner in SNAPI mode
		/// </summary>
		private void CheckForConn()
		{
			short numberOfScanners;
			int[] connectedScannerIDList = new int[255];
			cCoreScannerClass.GetScanners(out numberOfScanners, connectedScannerIDList,
			out outXML, out status);

			if (outXML.ToString().Length < 100)
			{
				Thread.Sleep(500);
			}
			else
			{
				sConnected = true;
			}
		}

		/// <summary>
		/// Disables barcode scanner
		/// </summary>
		private void ScannerDisable()
		{
			ChangeLed(ExecCodes._RedLEDOn);
			ExecMiscCmd(ExecCodes.SCAN_DISABLE);
		}

		/// <summary>
		/// Enables barcode scanner
		/// </summary>
		private void ScannerEnable()
		{
			ChangeLed(ExecCodes._GreenLEDOn);
			ExecMiscCmd(ExecCodes.SCAN_ENABLE);
		}

		/// <summary>
		/// Manually beep barcode scanner 
		/// </summary>
		/// <param name="value"></param>
		/// <param name="id">Scanner id if multi scanner app</param>
		private void InvokeBeep(ExecCodes value, int id = 1)
		{
			string inXML = "<inArgs>" +
								$"<scannerID>{id.ToString()}</scannerID>" + //Scanner ID
								"<cmdArgs>" +
									$"<arg-int>{(int)value}</arg-int>" +
								"</cmdArgs>" +
							"</inArgs>";
			cCoreScannerClass.ExecCommand((int)ExecCodes.SET_ACTION, ref inXML, out outXML, out status);
		}

		/// <summary>
		/// Change LED state/color
		/// </summary>
		/// <param name="value"></param>
		/// <param name="id">Scanner ID</param>
		private void ChangeLed(ExecCodes value, int id = 1)
		{
			string outXML;
			string inXML = "<inArgs>" +
								$"<scannerID>{id.ToString()}</scannerID>" + //Scanner ID
								"<cmdArgs>" +
									$"<arg-int>{(int)value}</arg-int>" +
								"</cmdArgs>" +
							"</inArgs>";
			cCoreScannerClass.ExecCommand((int)ExecCodes.SET_ACTION, ref inXML, out outXML, out status);
		}

		/// <summary>
		/// Misc. scanner function call.
		/// </summary>
		/// <param name="value">opcode</param>
		/// <param name="id">scanner id</param>
		private void ExecMiscCmd(ExecCodes opcode, int id = 1)
		{
			ScanDisabled = ((int)opcode == 2013) ? true : false;
			string inXML = "<inArgs>" +
								$"<scannerID>{id.ToString()}</scannerID>" +
								"</inArgs>"; ;

			cCoreScannerClass.ExecCommand((int)opcode, ref inXML, out outXML, out status);

		}

		/// <summary>
		/// Decodes the raw xml from barcode event
		/// </summary>
		/// <param name="strXml">Raw xml from scanner</param>
		private void DecodeXml(string strXml)
		{
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.LoadXml(strXml);

			string strData = String.Empty;
			string barcode = xmlDoc.DocumentElement.GetElementsByTagName("datalabel").Item(0).InnerText;
			string symbology = xmlDoc.DocumentElement.GetElementsByTagName("datatype").Item(0).InnerText;
			string[] numbers = barcode.Split(' ');

			foreach (string number in numbers)
			{
				if (String.IsNullOrEmpty(number))
				{
					break;
				}

				strData += ((char)Convert.ToInt32(number, 16)).ToString();
			}

			_RawIN = strData;
		}

		/// <summary>
		/// Output a message with the current thread ID appended
		/// </summary>
		/// <param name="message"></param>
		private static void Log(string message)
		{
			// Write line
			Debug.WriteLine($"{message} [{Thread.CurrentThread.ManagedThreadId}]");
		}

		/// <summary>
		/// Unsubscribe from barcode scanner 
		/// </summary>
		public void Disconnect()
		{
			cCoreScannerClass.Close(0, out status);
		}

		/// <summary>
		/// Get barcode attributes/info ect.
		/// </summary>
		/// <returns></returns>
		private string GetAssets()
		{
			string inXML = "<inArgs>" +
							"<scannerID>1</scannerID>" +
								// The scanner you need to get the information
								"<cmdArgs>" +
									"<arg-xml>" +
										"<attrib_list>20004,533,20007,1</attrib_list>" +
									// attribute numbers you need
									"</arg-xml>" +
								"</cmdArgs>" +
							"</inArgs>";
			cCoreScannerClass.ExecCommand((int)ExecCodes.ATTR_GET, ref inXML, out outXML, out status);
			return outXML;
		}

	}


	/// <summary>
	/// Unique Obj class for every part scanned
	/// </summary>
	public class Barcode 
	{
		public string Operator   { get;  set; } = "";
		public string Serial     { get;  set; } = "";
		public int Fixture       { get;  set; } = 1;
		public string DateTime   { get;  set; } = "";

		public Barcode(string ser, string dt, string op, int fix)
		{
			Debug.WriteLine($"Creating Barcode class instance on thread [{Thread.CurrentThread.ManagedThreadId}]");
			DateTime = dt;
			Serial = ser;
			Operator = op;
			Fixture = fix;
		}
	}
}
