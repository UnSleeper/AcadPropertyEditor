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
using Autodesk.AutoCAD.ApplicationServices;
using acApp = Autodesk.AutoCAD.ApplicationServices.Application;

namespace AcadPropertyEditor
{
    /// <summary>
    /// Interaction logic for EditWindow.xaml
    /// </summary>
    public partial class EditWindow : Window
    {
        private readonly LayersViewModel LayersListData;
        public EditWindow()
        {
            InitializeComponent();
            LayersListData = new LayersViewModel();
            DataContext = LayersListData;
        }

        public class LayersViewModel : INotifyPropertyChanged
        {
            private Model _selectedModel;
            public Model SelectedModel
            {
                get { return _selectedModel; }
                set
                {
                    _selectedModel = value;
                    OnPropertyChanged();
                }
            }
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
                    RenameLayer(this.name, value);
                    name = value;
                    OnPropertyChanged();
                }
            }
            private string type;
            public string Type
            {
                get => type;
                set
                {
                    type = value;
                }
            }
            private bool isSelected;
            public bool IsSelected
            {
                get { return this.isSelected; }
                set
                {
                    if (value != this.isSelected)
                    {
                        this.isSelected = value;
                        OnPropertyChanged();
                    }
                }
            }
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

        }
        public static void RenameLayer(string sLyrStName, string sLyrStNewName)
        {
            // Get the current document and database
            Document acDoc = acApp.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;
            using (DocumentLock acLckDoc = acDoc.LockDocument())
            {
                // Start a transaction
                using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
                {
                    LayerTable acLyrTbl;
                    acLyrTbl = acTrans.GetObject(acCurDb.LayerTableId,
                        OpenMode.ForRead) as LayerTable;
                    if (acLyrTbl.Has(sLyrStName) == true)
                    {
                        // Open the Layer for write
                        LayerTableRecord acLyrTblRec = acTrans.GetObject(acLyrTbl[sLyrStName], OpenMode.ForWrite) as LayerTableRecord;
                        acLyrTblRec.Name = sLyrStNewName;
                        acTrans.Commit();
                    }
                }
            }
        }
        public static void ChangeLayerVisible(string sLyrStName, bool isOff)
        {
            LayersViewModel layersListData = new LayersViewModel();
            // Get the current document and database
            Document acDoc = acApp.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;
            using (DocumentLock acLckDoc = acDoc.LockDocument())
            {
                // Start a transaction
                using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
                {
                    LayerTable acLyrTbl;
                    acLyrTbl = acTrans.GetObject(acCurDb.LayerTableId,
                        OpenMode.ForRead) as LayerTable;
                    if (acLyrTbl.Has(sLyrStName) == true)
                    {
                        // Open the Layer for write
                        LayerTableRecord acLyrTblRec = acTrans.GetObject(acLyrTbl[sLyrStName], OpenMode.ForWrite) as LayerTableRecord;
                        acLyrTblRec.IsOff = isOff;
                        acTrans.Commit();
                    }
                }
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
                    ChangeLayerVisible(Name, !value);
                    visible = value;
                    OnPropertyChanged();
                }
            }
            public ObservableCollection<Model> Models { get; set; } = new ObservableCollection<Model>();
            /*public ObservableCollection<PointModel> Points { get; set; } = new ObservableCollection<PointModel>();
            public ObservableCollection<LineModel> Lines { get; set; } = new ObservableCollection<LineModel>();
            public ObservableCollection<CircleViewMode> Circles { get; set; } = new ObservableCollection<CircleViewMode>();*/

        }
        public class PointModel : Model, INotifyPropertyChanged
        {
            private ObjectId id;

            public PointModel()
            {
                Name = "Точка";
                Type = "Point";
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
            private ObjectId id;

            public LineModel()
            {
                Name = "Отрезок";
                Type = "Line";
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
        public class CircleViewMode : Model, INotifyPropertyChanged
        {
            private ObjectId id;
            public CircleViewMode()
            {
                Name = "Окружность";
                Type = "Circle";
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
        private void TreeViewOnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var editor = acApp.DocumentManager.MdiActiveDocument.Editor;
            LayersListData.SelectedModel = e.NewValue as Model;
            editor.WriteMessage("Selected:" + LayersListData.SelectedModel.Name + Environment.NewLine);
        }
    }
}
