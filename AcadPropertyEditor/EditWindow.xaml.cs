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
using acColor = Autodesk.AutoCAD.Colors.Color;
using Autodesk.AutoCAD.DatabaseServices;

namespace AcadPropertyEditor
{
    /// <summary>
    /// Interaction logic for EditWindow.xaml
    /// </summary>
    public partial class EditWindow : Window
    {
        private readonly LayersViewMode LayersListData;
        public EditWindow()
        {
            InitializeComponent();
            LayersListData = new LayersViewMode();
            DataContext = LayersListData;
        }

        public class LayersViewMode
        {
            public ObservableCollection<LayerModel> LayersList { get; set; } = new ObservableCollection<LayerModel>();

            public event PropertyChangedEventHandler PropertyChanged;
            public void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public class Model : INotifyPropertyChanged
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
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public class LayerModel : Model, INotifyPropertyChanged
        {
            private acColor color;
            public acColor Color
            {
                get => color;
                set
                {
                    color = value;
                    OnPropertyChanged();
                }
            }

            private bool visible;
            public bool Visible
            {
                get => visible;
                set
                {
                    visible = value;
                    OnPropertyChanged();
                }
            }

            public ObservableCollection<PointModel> Points { get; set; } = new ObservableCollection<PointModel>();
            public ObservableCollection<LineModel> Lines { get; set; } = new ObservableCollection<LineModel>();
            public ObservableCollection<CircleViewMode> Circles { get; set; } = new ObservableCollection<CircleViewMode>();

        }
        public class PointModel : Model, INotifyPropertyChanged
        {
            private ObjectId id;

            public PointModel()
            {
                Name = "Точка";
            }

            public ObjectId Id
            {
                get => id;
                set
                {
                    id = value;
                    OnPropertyChanged();
                }
            }
        }

        public class LineModel : Model, INotifyPropertyChanged
        {
            private readonly string name = "Отрезок";
            private ObjectId id;
            public ObjectId Id
            {
                get => id;
                set
                {
                    id = value;
                    OnPropertyChanged();
                }
            }
        }

        public class CircleViewMode : Model, INotifyPropertyChanged
        {
            private string name = "Окружность";
            private ObjectId id;
            public ObjectId Id
            {
                get => id;
                set
                {
                    id = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
