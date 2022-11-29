using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Prism.Commands;
using RevitAPITrainingLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.MobileControls;

namespace RevitAPITraining61
/*{
    public class MainViewViewModel
    {
        private ExternalCommandData _commandData;

        public MainViewViewModel(ExternalCommandData commandData)
        {
         _commandData = commandData;
        }
    }
}*/
{
    public class MainViewViewModel
    {
        private ExternalCommandData _commandData;
        public List<DuctType> DuctTypes { get; } = new List<DuctType>();
        public List<Level> Levels { get; } = new List<Level>();
        public DelegateCommand SaveCommand { get; }
        public double DuctValue { get; set; }
        //public double PV { get; set; }
        //public double WallHeight { get; set; }
        public DuctType SelectedDuctType { get; set; }
        public Level SelectedLevel { get; set; }
        public List<XYZ> Points { get; } = new List<XYZ>();


        public MainViewViewModel(ExternalCommandData commandData)
        {
            _commandData = commandData;
            DuctTypes = DuctsUtils.GetDuctTypes(commandData);
            Levels = LevelsUtils.GetLevels(commandData);
            SaveCommand = new DelegateCommand(OnSaveCommand);
            //Duct
            //Parameter value = duct.get_Parameter(BuiltInParameter.RBS_OFFSET_PARAM);
            //value.Set(UnitUtils.ConvertToInternalUnits(DuctValue, UnitTypeId.Millimeters));
            //WallHeight = 100;
            //Parameter value = duct.get_Parameter(BuiltInParameter.RBS_OFFSET_PARAM);
            //value.Set(UnitUtils.ConvertToInternalUnits(DuctValue, UnitTypeId.Millimeters));
            //ParameterValue = 0;
            DuctValue = 0;
            //PV = 0;
            Points = SelectionUtils.GetPoints (_commandData, "Выберите точки", ObjectSnapTypes.Endpoints);
        }
        private void OnSaveCommand()
        {
            UIApplication uiapp = _commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            if (Points.Count < 2 || SelectedDuctType == null || SelectedLevel == null)
            {
                return;
            }
            
            /*var points = SelectionUtils.GetPoints(_commandData, "Выберите точки", ObjectSnapTypes.Endpoints);
            //var curves = new List<Curve>();
            for (int i = 0; i < Points.Count; i++)
            {
                if (i == 0)
                    continue;

                var prevPoint = Points[i - 1];
                var currentPoint = Points[i];

                               //Curve curve = Line.CreateBound(prevPoint, currentPoint);
                //curves.Add(curve);
            }*/
            using (var ts = new Transaction(doc, "create duct"))
            {
                ts.Start();
                foreach (var point in Points)
                {
                    Duct.Create(doc, SelectedDuctType.Id, SelectedLevel.Id, UnitUtils.ConvertToInternalUnits(DuctValue, UnitTypeId.Millimeters), SelectionUtils.GetPoints);
                    //Duct.Create(doc, curve, SelectedDuctType.Id, SelectedLevel.Id, 
                    // UnitUtils.ConvertToInternalUnits(PV, UnitTypeId.Millimeters));
                    
                    Parameter value = duct.get_Parameter(BuiltInParameter.RBS_OFFSET_PARAM);
                    value.Set(UnitUtils.ConvertToInternalUnits(DuctValue, UnitTypeId.Millimeters));
                }
                ts.Commit();
            }
            RaiseCloseRequest();
        }
        public event EventHandler CloseRequest;
        private void RaiseCloseRequest()
        {
            CloseRequest?.Invoke(this, EventArgs.Empty);
        }
    }

   
}
