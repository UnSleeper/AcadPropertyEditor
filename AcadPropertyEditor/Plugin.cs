using System;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using acColor = Autodesk.AutoCAD.Colors.Color;
using acApp = Autodesk.AutoCAD.ApplicationServices.Application;
using WinForm = System.Windows;
using static AcadPropertyEditor.EditWindow;
using System.Windows.Media;
using System.Collections.Generic;

namespace AcadPropertyEditor
{
    public class Plugin : IExtensionApplication
    {
        [CommandMethod("StartEdit")]
        public void DisplayLayerNames()
        {
            var editor = acApp.DocumentManager.MdiActiveDocument.Editor;
            editor.WriteMessage("Запускаю окно изменений.." + Environment.NewLine);
            EditWindow mainView = new EditWindow();
            Application.ShowModelessWindow(mainView);
            //if (Application.ShowModalWindow(mainView) != true) return; //Option to make the window modal to prevent changes
            mainView.DataContext = FindLayers();
        }
        public LayersViewModel FindLayers()
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
                    // Open the Layer table for read
                    LayerTable acLyrTbl;
                    acLyrTbl = acTrans.GetObject(acCurDb.LayerTableId,
                        OpenMode.ForRead) as LayerTable;
                    foreach (ObjectId acObjId in acLyrTbl)
                    {
                        LayerTableRecord acLyrTblRec;
                        acLyrTblRec = acTrans.GetObject(acObjId,
                                                        OpenMode.ForRead) as LayerTableRecord;
                        LayerModel dataLayer = new LayerModel()
                        {
                            Name = acLyrTblRec.Name,
                            Color = acLyrTblRec.Color,
                            Visible = acLyrTblRec.IsOff
                        };
                        FindEntitiesForLayer(dataLayer);

                        layersListData.LayersList.Add(dataLayer);
                    }
                    // Dispose of the transaction
                    acTrans.Commit();
                }
            }
            return layersListData;
        }

        public void FindEntitiesForLayer(LayerModel dataLayer)
        {
            Database db = HostApplicationServices.WorkingDatabase;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTableRecord ms = (BlockTableRecord)tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db), OpenMode.ForRead);

                foreach (ObjectId id in ms)
                {
                    Entity entity = (Entity)tr.GetObject(id, OpenMode.ForRead);
                    if (entity.Layer == dataLayer.Name)
                    {
                        if (entity.GetType().Name == "DBPoint") 
                        {
                            PointModel point = new PointModel()
                            {
                                Id = entity.Id
                            };
                            dataLayer.Points.Add(point);
                        }
                        if (entity.GetType().Name == "Line")
                        {
                            LineModel line = new LineModel()
                            {
                                Id = entity.Id
                            };
                            dataLayer.Lines.Add(line);
                        }
                        if (entity.GetType().Name == "Circle")
                        {
                            CircleViewMode circle = new CircleViewMode()
                            {
                                Id = entity.Id
                            };
                            dataLayer.Circles.Add(circle);
                        }
                    }
                }
                //tr.Commit();
            }
        }

        public static void RenameLayerState(string sLyrStName, string sLyrStNewName)
        {
            // Get the current document
            Document acDoc = Application.DocumentManager.MdiActiveDocument;

            LayerStateManager acLyrStMan;
            acLyrStMan = acDoc.Database.LayerStateManager;

            if (acLyrStMan.HasLayerState(sLyrStName) == true &&
                acLyrStMan.HasLayerState(sLyrStNewName) == false)
            {
                acLyrStMan.RenameLayerState(sLyrStName, sLyrStNewName);
            }
        }

        /*public static Point2d[] GetPoints(this Polyline polyline) //Workpiece for changing lines
        {
            var editor = acApp.DocumentManager.MdiActiveDocument.Editor;
            if (polyline == null)
            {
                editor.WriteMessage("An unexpected error! Attempt to call a method for a non-existent polyline.");
                return null;
            }
            if (polyline.NumberOfVertices == 0)
                return null;

            var points = new List<Point2d>();
            for (int i = 0; i < polyline.NumberOfVertices; i++)
                points.Add(polyline.GetPoint2dAt(i));

            return points.ToArray();
        }*/

        public void Initialize()
        {
            var editor = acApp.DocumentManager.MdiActiveDocument.Editor;
            editor.WriteMessage("Plugin initialization.." + Environment.NewLine);
        }

        public void Terminate()
        {

        }
    }
}
