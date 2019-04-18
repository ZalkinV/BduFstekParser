using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace BduFstekParser
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		internal List<ThreatEntry> threatEntries { get; set; }

		public MainWindow()
		{
			InitializeComponent();
			threatEntries = new List<ThreatEntry>();
			threatEntries.Add(new ThreatEntry() { Id = 0, Name="Угроза 1"});
			threatEntries.Add(new ThreatEntry() { Id = 1, Name = "Угроза 2" });

			listViewThreatEntries.ItemsSource = threatEntries;
		}
	}
}
