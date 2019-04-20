using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.Threading;
using Excel = Microsoft.Office.Interop.Excel;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace BduFstekParser
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private DiffWindow diffWindow;

		internal List<ThreatEntry> threatEntries;
		internal ObservableCollection<ThreatEntry> threatEntriesVisible;
		private int visibleThreatCount = 15;
		private int lastVisibleEntryIndex = 0;

		private string threatSerializedFileName;
		private string threatFileName;
		private string threatFileUrl;

		public MainWindow()
		{
			InitializeComponent();

			threatEntries = new List<ThreatEntry>();
			threatEntriesVisible = new ObservableCollection<ThreatEntry>();
			listViewThreatEntries.ItemsSource = threatEntriesVisible;

			threatSerializedFileName = "ThreatEntriesData.xml";
			threatFileName = "thrlist.xlsx";
			threatFileUrl = "https://bdu.fstec.ru/documents/files/thrlist.xlsx";
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			FillThreatsListView();
			SerializeThreatEntries(threatEntries, threatSerializedFileName);

			int entriesCount = Math.Min(visibleThreatCount, threatEntries.Count);
			for (int i = 0; i < entriesCount; i++)
				threatEntriesVisible.Add(threatEntries[i]);
			lastVisibleEntryIndex = entriesCount - 1;
		}

		private void SerializeThreatEntries(List<ThreatEntry> entries, string fileName)
		{
			XmlSerializer serializer = new XmlSerializer(typeof(ThreatEntry[]));

			using (FileStream fs = new FileStream(fileName, FileMode.Create))
			{
				serializer.Serialize(fs, entries.ToArray());
			}
		}

		private List<ThreatEntry> DeserializeThreatEntries(string fileName)
		{
			XmlSerializer serializer = new XmlSerializer(typeof(ThreatEntry[]));

			List<ThreatEntry> entries;
			using (FileStream fs = new FileStream(fileName, FileMode.Open))
			{
				entries = (serializer.Deserialize(fs) as ThreatEntry[])?.ToList();
			}

			return entries;
		}

		private void FillThreatsListView()
		{
			PrepareFile();
			threatEntries = GetFileData(threatFileName);
		}

		private void PrepareFile()
		{
			if (!File.Exists(threatFileName))
			{
				MessageBoxResult userChoice = MessageBox.Show("Файл с УБИ не найден.\nСкачать его с сайта ФСТЭК?", "Проверка наличия файла", MessageBoxButton.YesNo);
				if (userChoice == MessageBoxResult.Yes)
				{
					using (WebClient webClient = new WebClient())
					{
						webClient.DownloadFile(new Uri(threatFileUrl), threatFileName);
					}
				}
			}
		}

		private List<ThreatEntry> GetFileData(string fileName)
		{
			Excel.Application excel = new Excel.Application();
			Excel.Workbook excelWorkbook = excel.Workbooks.Open(Directory.GetCurrentDirectory() + "/" + fileName, 0, true);
			Excel.Worksheet excelSheet = excelWorkbook.Sheets[1];
			Excel.Range excelRange = excelSheet.UsedRange;

			List<ThreatEntry> fileData = new List<ThreatEntry>(excelRange.Rows.Count);
			for (int iRow = 3; iRow <= excelRange.Rows.Count; iRow++)
			{
				int columnCount = excelRange.Columns.Count - 2;

				string[] rowValues = new string[columnCount];
				for (int jColumn = 1; jColumn <= columnCount; jColumn++)
				{
					rowValues[jColumn - 1] = excelSheet.Cells[iRow, jColumn].Value.ToString();
				}

				ThreatEntry newEntry = new ThreatEntry
				(
					int.Parse(rowValues[0]),
					rowValues[1],
					rowValues[2],
					rowValues[3],
					rowValues[4],
					rowValues[5] == "1",
					rowValues[6] == "1",
					rowValues[7] == "1"
				);

				fileData.Add(newEntry);
			}

			excel.Quit();

			return fileData;
		}

		private void ButtonPrevious_Click(object sender, RoutedEventArgs e)
		{
			if (lastVisibleEntryIndex < visibleThreatCount)
				return;

			int firstEntryIndex = Math.Max(0, lastVisibleEntryIndex + 1 - (threatEntriesVisible.Count + visibleThreatCount));
			int lastEntryIndex = firstEntryIndex + visibleThreatCount;

			threatEntriesVisible.Clear();

			for (int i = firstEntryIndex; i < lastEntryIndex; i++)
			{
				threatEntriesVisible.Add(threatEntries[i]);
			}
			lastVisibleEntryIndex = lastEntryIndex - 1;
		}

		private void ButtonNext_Click(object sender, RoutedEventArgs e)
		{
			if (lastVisibleEntryIndex >= threatEntries.Count - 1)
				return;

			threatEntriesVisible.Clear();

			int remainsEntries = threatEntries.Count - 1 - lastVisibleEntryIndex;
			int firstEntryIndex = lastVisibleEntryIndex + 1;
			int lastEntryIndex = firstEntryIndex + Math.Min(remainsEntries, visibleThreatCount);
			for (int i = firstEntryIndex; i < lastEntryIndex; i++)
			{
				threatEntriesVisible.Add(threatEntries[i]);
			}
			lastVisibleEntryIndex = lastEntryIndex - 1;
		}

		private void ListViewThreatEntries_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var selectedEntry = listViewThreatEntries.SelectedItem as ThreatEntry;
			textBoxThreatDescription.Text = selectedEntry?.GetFullInfo() ?? "Выберите угрозу для просмотра дополнительной информации о ней в этом окне";
		}

		private void ButtonUpdateFile_Click(object sender, RoutedEventArgs e)
		{
			string tmpFileName = ".Tmp" + threatFileName;

			using (WebClient webClient = new WebClient())
			{
				webClient.DownloadFile(new Uri(threatFileUrl), tmpFileName);
			}

			List<ThreatEntry> fetchedEntries = GetFileData(tmpFileName);

			List<EntryDiff> differences = FindDifferences(threatEntries, fetchedEntries);
			diffWindow = new DiffWindow(differences);
			diffWindow.Show();
		}

		private List<EntryDiff> FindDifferences(List<ThreatEntry> before, List<ThreatEntry> after)
		{
			Dictionary<int, ThreatEntry> oldData = before.ToDictionary((entry) => entry.Id);
			Dictionary<int, ThreatEntry> newData = after.ToDictionary((entry) => entry.Id);

			Dictionary<int, ThreatEntry> biggerData;
			Dictionary<int, ThreatEntry> smallerData;

			if (newData.Count >= oldData.Count)
			{
				biggerData = newData;
				smallerData = oldData;
			}
			else
			{
				biggerData = oldData;
				smallerData = newData;
			}

			List<EntryDiff> differences = new List<EntryDiff>();
			foreach (var entry in biggerData)
			{
				if (!smallerData.ContainsKey(entry.Key))
				{
					if (biggerData == newData)
						differences.Add(new EntryDiff(null, entry.Value));
					else
						differences.Add(new EntryDiff(entry.Value, null));
				}
				else if (!entry.Value.Equals(smallerData[entry.Key]))
				{
					if (biggerData == newData)
						differences.Add(new EntryDiff(smallerData[entry.Key], entry.Value));
					else
						differences.Add(new EntryDiff(entry.Value, smallerData[entry.Key]));
				}
			}

			return differences;
		}
	}
}
