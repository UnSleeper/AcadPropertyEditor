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
using System.Collections.ObjectModel;
using System.Data.Linq;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AcadPropertyEditor
{
    /// <summary>
    /// Interaction logic for EditWindow.xaml
    /// </summary>
    public partial class EditWindow : Window
    {
        private LayersData LayersListData;
        public EditWindow()
        {
            InitializeComponent();
            LayersListData = new LayersData();
            DataContext = LayersListData;
            LayersListData.LayersList.Add(new DataLayer
            {
                Name = "Test Name 1"
            });
            LayersListData.LayersList.Add(new DataLayer
            {
                Name = "Test Name 2"
            });
        }

        public class LayersData
        {
            public ObservableCollection<DataLayer> LayersList { get; set; } = new ObservableCollection<DataLayer>();
        }

        public class DataLayer : INotifyPropertyChanged
        {
            private string name;
            public string Name
            {
                get => name;
                set
                {
                    name = value;
                    OnPropertyChanged();
                }
            }
            public ObservableCollection<DataPoint> Points { get; set; }
            public ObservableCollection<DataLine> Lines { get; set; }
            public ObservableCollection<DataCircle> Circles { get; set; }

            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public class DataPoint
        {
            public int x { get; set; }
            public int y { get; set; }
            public int z { get; set; }
        }
        public class DataLine
        {
            public int x1 { get; set; }
            public int y1 { get; set; }
            public int z1 { get; set; }
            public int x2 { get; set; }
            public int y2 { get; set; }
            public int z2 { get; set; }
        }
        public class DataCircle
        {
            public int x { get; set; }
            public int y { get; set; }
            public int z { get; set; }
            public int r { get; set; }
        }
    }
}
