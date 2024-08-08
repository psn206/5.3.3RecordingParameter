using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordingParameter
{
    [Transaction(TransactionMode.Manual)]
    public class Main : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            Document doc = uiDoc.Document;

            try
            {
                IList<Reference> selectElements = uiDoc.Selection.PickObjects(ObjectType.Element, "Выберете трубы");

                foreach (var SElement in selectElements)
                {
                    var element = doc.GetElement(SElement);
                    if (element is Pipe)
                    {
                        Parameter pipeLength = element.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH);

                        using (Transaction tr = new Transaction(doc, "add param"))
                        {
                            tr.Start();
                            var familyInstence = element as FamilyInstance;
                            Parameter parameter = element.LookupParameter("Длина с запасом");
                            parameter.Set(pipeLength.AsDouble() * 1.1);
                            tr.Commit();
                        }
                    }
                }
                TaskDialog.Show("Длина труб", "Запас добавлен");
                return Result.Succeeded;
            }
            catch
            {
                TaskDialog.Show("Длина труб", "Запас не добавлен");
                return Result.Succeeded;
            }
        }
    }
}
