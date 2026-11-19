using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQX.TemplateProject.Defines.Vision
{
    public enum EVisionAlignResult
    {
        OK,
        NG,
    }

    public class VisionAlignResult
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double T { get; set; }
        public EVisionAlignResult Result { get; set; }
    }
}
