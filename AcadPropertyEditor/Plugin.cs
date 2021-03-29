using System;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Colors;
using acadApp = Autodesk.AutoCAD.ApplicationServices.Application;
using WinForm = System.Windows;
using static AcadPropertyEditor.EditWindow;

namespace AcadPropertyEditor
{
    public class Plugin : IExtensionApplication
    {

        [CommandMethod("StartEdit")]
        public static void DisplayLayerNames()
        {
            var editor = acadApp.DocumentManager.MdiActiveDocument.Editor;
            editor.WriteMessage("Запускаю окно изменений.." + Environment.NewLine);
            EditWindow mainView = new EditWindow();
            if(Application.ShowModalWindow(mainView) != true) return;
            LayersData layersListData = new LayersData();
            layersListData = (LayersData)mainView.DataContext;
            // Get the current document and database
            Document acDoc = acadApp.DocumentManager.MdiActiveDocument;
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
                        layersListData.LayersList.Add(new DataLayer
                        {
                            Name = acLyrTblRec.Name
                        });
                    }
                    // Dispose of the transaction
                }
            }
            mainView.DataContext = layersListData;
        }

        public void Initialize()
        {
            var editor = acadApp.DocumentManager.MdiActiveDocument.Editor;
            editor.WriteMessage("Инициализация плагина.." + Environment.NewLine);

        }

        public void Terminate()
        {

        }


    }
}
