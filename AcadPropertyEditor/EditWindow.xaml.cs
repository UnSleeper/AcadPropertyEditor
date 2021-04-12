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
using System.Windows.Media.Media3D;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using uiVisibility = System.Windows.Visibility;
using acColor = Autodesk.AutoCAD.Colors.Color;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.ApplicationServices;
using acApp = Autodesk.AutoCAD.ApplicationServices.Application;
using Line = Autodesk.AutoCAD.DatabaseServices.Line;

namespace AcadPropertyEditor
{
    /// <summary>
    /// Interaction logic for EditWindow.xaml
    /// </summary>
    public partial class EditWindow : Window
    {
        private readonly LayersViewModel _layersListData;
        public EditWindow()
        {
            InitializeComponent();
            _layersListData = new LayersViewModel();
            DataContext = _layersListData;
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
                    Rename(this.name, value);
                    name = value;
                    OnPropertyChanged();
                }
            }

            public virtual void Rename(string name, string value)
            {
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


            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            public uiVisibility IsLayer
            {
                get 
                {
                    return type == "Layer" ? uiVisibility.Visible : uiVisibility.Collapsed;
                }
            }

            public uiVisibility IsPoint
            {
                get
                {
                    return type == "Point" ? uiVisibility.Visible : uiVisibility.Collapsed;
                }
            }

            public uiVisibility IsLine
            {
                get
                {
                    return type == "Line" ? uiVisibility.Visible : uiVisibility.Collapsed;
                }
            }

            public uiVisibility IsCircle
            {
                get
                {
                    return type == "Circle" ? uiVisibility.Visible : uiVisibility.Collapsed;
                }
            }
        }

        public class LayerModel : Model, INotifyPropertyChanged
        {
            public LayerModel()
            {
                Type = "Layer";
            }

            private acColor color;
            public acColor Color
            {
                get => color;
                set
                {
                    color = value;
                    ChangeColor(Name, value);
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
            public override void Rename(string sLyrStName, string sLyrStNewName)
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
            private static void ChangeLayerVisible(string sLyrStName, bool isOff)
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
                            acLyrTblRec.IsOff = isOff;
                            acTrans.Commit();
                        }
                    }
                }
            }
            private static void ChangeColor(string sLyrStName, acColor color)
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
                            acLyrTblRec.Color = color;
                            acTrans.Commit();
                        }
                    }
                }
            }
        }
        public class PointModel : Model, INotifyPropertyChanged
        {
            private Point3D point;
            public Point3D Point
            {
                get => point;
                set
                {
                    point = value;
                    EditPoint();
                    OnPropertyChanged();
                }
            }
            public PointModel()
            {
                Name = "Точка";
                Type = "Point";
            }

            private void EditPoint()
            {
                // Get the current document and database
                Document acDoc = acApp.DocumentManager.MdiActiveDocument;
                Database acCurDb = acDoc.Database;
                using (DocumentLock acLckDoc = acDoc.LockDocument())
                {
                    // Start a transaction
                    using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
                    {
                        DBPoint curPoint = acTrans.GetObject(Id,
                            OpenMode.ForWrite) as DBPoint;
                        curPoint.Position = new Point3d(Point.X, Point.Y,Point.Z);
                        acTrans.Commit();
                    }
                }
            }
        }
        public class LineModel : Model, INotifyPropertyChanged
        {
            private Point3D startPoint;
            public Point3D StartPoint
            {
                get => startPoint;
                set
                {
                    startPoint = value;
                    EditStartPoints();
                    OnPropertyChanged();
                }
            }
            private Point3D endPoint;
            public Point3D EndPoint
            {
                get => endPoint;
                set
                {
                    endPoint = value;
                    EditEndPoints();
                    OnPropertyChanged();
                }
            }
            public LineModel()
            {
                Name = "Отрезок";
                Type = "Line";
            }

            private void EditStartPoints()
            {
                // Get the current document and database
                Document acDoc = acApp.DocumentManager.MdiActiveDocument;
                Database acCurDb = acDoc.Database;
                using (DocumentLock acLckDoc = acDoc.LockDocument())
                {
                    // Start a transaction
                    using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
                    {
                        Line curLine = acTrans.GetObject(Id,
                            OpenMode.ForWrite) as Line;
                        curLine.StartPoint = new Point3d(startPoint.X, startPoint.Y, startPoint.Z);
                        acTrans.Commit();
                    }
                }
            }
            private void EditEndPoints()
            {
                // Get the current document and database
                Document acDoc = acApp.DocumentManager.MdiActiveDocument;
                Database acCurDb = acDoc.Database;
                using (DocumentLock acLckDoc = acDoc.LockDocument())
                {
                    // Start a transaction
                    using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
                    {
                        Line curLine = acTrans.GetObject(Id,
                            OpenMode.ForWrite) as Line;
                        curLine.EndPoint = new Point3d(endPoint.X, endPoint.Y, endPoint.Z);
                        acTrans.Commit();
                    }
                }
            }
        }
        public class CircleViewMode : Model, INotifyPropertyChanged
        {
            private Point3D centerPoint;
            public Point3D CenterPoint
            {
                get => centerPoint;
                set
                {
                    centerPoint = value;
                    EditCenterPoint();
                    OnPropertyChanged();
                }
            }

            private double radius;
            public double Radius
            {
                get => radius;
                set
                {
                    radius = value;
                    EditRadius();
                    OnPropertyChanged();
                }
            }

            public CircleViewMode()
            {
                Name = "Окружность";
                Type = "Circle";
            }

            private void EditCenterPoint()
            {
                // Get the current document and database
                Document acDoc = acApp.DocumentManager.MdiActiveDocument;
                Database acCurDb = acDoc.Database;
                using (DocumentLock acLckDoc = acDoc.LockDocument())
                {
                    // Start a transaction
                    using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
                    {
                        Circle curCircle = acTrans.GetObject(Id,
                            OpenMode.ForWrite) as Circle;
                        curCircle.Center = new Point3d(centerPoint.X, centerPoint.Y, centerPoint.Z);
                        acTrans.Commit();
                    }
                }
            }
            private void EditRadius()
            {
                // Get the current document and database
                Document acDoc = acApp.DocumentManager.MdiActiveDocument;
                Database acCurDb = acDoc.Database;
                using (DocumentLock acLckDoc = acDoc.LockDocument())
                {
                    // Start a transaction
                    using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
                    {
                        Circle curCircle = acTrans.GetObject(Id,
                            OpenMode.ForWrite) as Circle;
                        curCircle.Radius = radius;
                        acTrans.Commit();
                    }
                }
            }
        }
        private void TreeViewOnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            _layersListData.SelectedModel = e.NewValue as Model;
        }
    }
}
