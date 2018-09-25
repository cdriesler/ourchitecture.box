﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cicero.Core.Formats;
using Cicero.Core.Logic.Parse;
using Cicero.Core.Logic.Solve;
using Rhino;
using Rhino.Geometry;
using Grasshopper.Kernel;

namespace Cicero.Core.Components
{
    public class BlackBox : GH_Component
    {
        public BlackBox() : base("Black Box", "BLKBOX", "Black Box solver.", "Cicero", "Main")
        {

        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Bounds", "B", "Square bounds to solve within.", GH_ParamAccess.item);
            pManager.AddTextParameter("Data", "D", "Data from web request.", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Drawing", "C", "Curves to convert to svg.", GH_ParamAccess.list);
            pManager.AddTextParameter("Style Pattern", "S", "Pattern for styler to read.", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Curve bounds = null;
            DA.GetData(0, ref bounds);

            string payload = null;
            DA.GetData(1, ref payload);

            //Parse geometry from text.
            Request req = Parse.RequestFromPayload(payload);

            //Do solution.
            List<BoxResult> res = Solve.Request(req, bounds);

            //Convert solution to ordered list of curves and styles.
            Solution output = Parse.SolutionFromResults(res);

            //Write solution to output.
            var outputGeometry = new List<Curve>();
            var outputStyles = new List<string>();

            for (int i = 0; i < output.OutputCurves.Count; i++)
            {
                outputGeometry.Add(output.OutputCurves[i]);
                outputStyles.Add(output.OutputStyles[i]);
            }

            DA.SetDataList(0, outputGeometry);
            DA.SetDataList(1, outputStyles);
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return null;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("d0ed5418-a0c9-466b-9713-850d1f673783"); }
        }
    }
}