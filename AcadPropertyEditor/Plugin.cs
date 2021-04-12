using System;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using acApp = Autodesk.AutoCAD.ApplicationServices.Application;
using static AcadPropertyEditor.EditWindow;
using System.Windows.Media.Media3D;

namespace AcadPropertyEditor
{
    public class Plugin : IExtensionApplication
    {
        [CommandMethod("StartEdit")]
        public void DisplayLayerNames()
        {
            var editor = acApp.DocumentManager.MdiActiveDocument.Editor;
            editor.WriteMessage("Launching the change window.." + Environment.NewLine);
            EditWindow mainView = new EditWindow();
            Application.ShowModelessWindow(mainView);

            FindLayers(mainView.DataContext as LayersViewModel);
        }
        public void FindLayers(LayersViewModel layersListData)
        {
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
                            Visible = !acLyrTblRec.IsOff,
                            Id = acLyrTblRec.Id
                        };
                        FindEntitiesForLayer(dataLayer);
                        layersListData.LayersList.Add(dataLayer);
                    }
                    // Dispose of the transaction
                    acTrans.Commit();
                }
            }
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
                            DBPoint acDBPoint = entity as DBPoint;
                            PointModel point = new PointModel()
                            {
                                Id = entity.Id,
                                Point = new Point3D(acDBPoint.Position.X, acDBPoint.Position.Y, acDBPoint.Position.Z)
                            };
                            dataLayer.Models.Add(point);
                        }
                        if (entity.GetType().Name == "Line")
                        {
                            Line acLine = entity as Line;
                            LineModel line = new LineModel()
                            {
                                Id = entity.Id,
                                StartPoint = new Point3D(acLine.StartPoint.X, acLine.StartPoint.Y, acLine.StartPoint.Z),
                                EndPoint = new Point3D(acLine.EndPoint.X, acLine.EndPoint.Y, acLine.EndPoint.Z)
                        };
                            dataLayer.Models.Add(line);
                        }
                        if (entity.GetType().Name == "Circle")
                        {
                            Circle acCircle = entity as Circle;
                            CircleViewMode circle = new CircleViewMode()
                            {
                                Id = entity.Id,
                                CenterPoint = new Point3D(acCircle.Center.X, acCircle.Center.Y, acCircle.Center.Z),
                                Radius = acCircle.Radius
                            };
                            dataLayer.Models.Add(circle);
                        }
                    }
                }
            }
        }

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
