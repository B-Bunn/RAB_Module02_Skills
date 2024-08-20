using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;


namespace RAB_Module02_Skills
{
    [Transaction(TransactionMode.Manual)]
    public class Class1 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // this is a variable for the Revit application
            UIApplication uiapp = commandData.Application;

            // this is a variable for the current Revit model
            Document doc = uiapp.ActiveUIDocument.Document;

            UIDocument uidoc = uiapp.ActiveUIDocument;
            IList<Element> pickList = uidoc.Selection.PickElementsByRectangle("Select Elements");


            //defines list type|name of list = creates new list(empty)
            List<CurveElement> allCurves = new List<CurveElement>();
            foreach (Element elem in pickList)
            {
                if (elem is CurveElement)
                {
                    allCurves.Add(elem as CurveElement);
                }
            }

            List<CurveElement> modelCurves = new List<CurveElement>();
            foreach (Element elem in pickList)
            {
                if (elem is CurveElement)
                {
                    CurveElement curveElem = elem as CurveElement;
                    if (curveElem.CurveElementType == CurveElementType.ModelCurve)
                    {
                        //listname.Add adds to a current list
                        modelCurves.Add(elem as CurveElement);
                    }
                }
            }

            //Curve Data
            foreach (CurveElement currentCurve in modelCurves)
            {
                Curve curve = currentCurve.GeometryCurve;
                XYZ startPoint = curve.GetEndPoint(0);
                XYZ endPoint = curve.GetEndPoint(1);


                GraphicsStyle curStyle = currentCurve.LineStyle as GraphicsStyle;

                Debug.Print(curStyle.Name);
            }

            using (Transaction t = new Transaction(doc))
            {
                t.Start("Create Revit Elements");

                Level newLevel = Level.Create(doc, 20);
                Curve curCurve1 = modelCurves[0].GeometryCurve;

                Wall.Create(doc, curCurve1, newLevel.Id, false);

                FilteredElementCollector wallTypes = new FilteredElementCollector(doc);
                wallTypes.OfClass(typeof(WallType));

                //Use Brackets to select which index you want to select from a list
                Curve curCurve2 = modelCurves[1].GeometryCurve;
                WallType myWallType = GetWallTypeByName(doc, "Basic Wall");
                //Wall.Create(doc, curCurve2, myWallType.Id, newLevel.Id, 20, 0, false, false);

                FilteredElementCollector systemCollector = new FilteredElementCollector(doc);
                systemCollector.OfClass(typeof(MEPSystemType));

                MEPSystemType ductSystemType = null;
                foreach (MEPSystemType curType in systemCollector)
                {
                    if (curType.Name == "Supply Air")
                    {
                        ductSystemType = curType;
                        break;
                    }
                }

                FilteredElementCollector collector1 = new FilteredElementCollector(doc);
                collector1.OfClass(typeof(DuctType));

                Curve curCurve3 = modelCurves[2].GeometryCurve;
                Duct newDuct = Duct.Create(doc, ductSystemType.Id, collector1.FirstElementId(), newLevel.Id,
                    curCurve3.GetEndPoint(0), curCurve3.GetEndPoint(1));

                MEPSystemType pipeSystemType = null;
                foreach (MEPSystemType curType in systemCollector)
                {
                    if (curType.Name == "Domestic Hot Water")
                    {
                        pipeSystemType = curType;
                        break;
                    }
                }

                FilteredElementCollector collector2 = new FilteredElementCollector(doc);
                collector2.OfClass(typeof(PipeType));

                collector2.Dispose();

                collector2.OfClass(typeof(PipeType));



                Curve curCurve4 = modelCurves[3].GeometryCurve;
                Pipe newPipe = Pipe.Create(doc, pipeSystemType.Id, collector1.FirstElementId(), newLevel.Id,
                    curCurve4.GetEndPoint(0), curCurve4.GetEndPoint(1));

                string testString = MyFirstMethod();
                MySecondMethod();
                string testString2 = MyThirdMethod("Hello World");

                int numberValue = 5;
                string numAsString = "";

                switch (numberValue)
                {
                    case 1:
                        numAsString = "One";
                        break;

                    case 2:
                        numAsString = "Two";
                        break;

                    case 3:
                        numAsString = "Three";
                        break;
                    case 4:
                        numAsString = "Four";
                        break;
                    case 5:
                        numAsString = "Five";
                        break;

                    default:
                        numAsString = "Zero";
                        break;

                }

                Curve curve5 = modelCurves[1].GeometryCurve;
                GraphicsStyle curve5GS = modelCurves[1].LineStyle as GraphicsStyle;

                WallType wallType1 = GetWallTypeByName(doc, "Storefront");
                WallType wallType2 = GetWallTypeByName(doc, "Exterior - Brick on CMU");

                switch (curve5GS.Name)
                {
                    case "<Thin Lines>":
                        Wall.Create(doc, curve5, wallType1.Id, newLevel.Id, 20, 0, false, false);
                        break;

                    case "<Wide Lines>":
                        Wall.Create(doc, curve5, wallType2.Id, newLevel.Id, 20, 0, false, false);
                        break;

                    default:
                        Wall.Create(doc, curve5, newLevel.Id, false);
                        break;
                }

                t.Commit();
            }

            return Result.Succeeded;
        }
        internal static PushButtonData GetButtonData()
        {
            // use this method to define the properties for this command in the Revit ribbon
            string buttonInternalName = "btnCommand1";
            string buttonTitle = "Button 1";
            string? methodBase = MethodBase.GetCurrentMethod().DeclaringType?.FullName;

            if (methodBase == null)
            {
                throw new InvalidOperationException("MethodBase.GetCurrentMethod().DeclaringType?.FullName is null");
            }
            else
            {
                Common.ButtonDataClass myButtonData1 = new Common.ButtonDataClass(
                    buttonInternalName,
                    buttonTitle,
                    methodBase,
                    Properties.Resources.Blue_32,
                    Properties.Resources.Blue_16,
                    "This is a tooltip for Button 1");

                return myButtonData1.Data;
            }
        }

        internal string MyFirstMethod()
        {
            return "this is my first method";
        }

        internal void MySecondMethod() //void means no value is returned when using the method
        {
            Debug.Print("this is my second method");
        }

        internal string MyThirdMethod(string input)
        {
            return "this is my third method" + input;
        }

        internal WallType GetWallTypeByName(Document doc, string typeName)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfClass(typeof(WallType));

            foreach (WallType curType in collector)
            {
                if (curType.Name == typeName)
                {
                    return curType;
                }
            }

            return null;

        }
    }
}

