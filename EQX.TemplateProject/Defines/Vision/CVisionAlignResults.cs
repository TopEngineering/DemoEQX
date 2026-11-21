using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQX.TemplateProject.Defines.Vision
{
    public class CVisionAlignResults
    {
        public VisionAlignResult VisionAlignResult1 { get; set; }
        public VisionAlignResult VisionAlignResult2 { get; set; }

        public CVisionAlignResults()
        {
            VisionAlignResult1 = new VisionAlignResult();
            VisionAlignResult2 = new VisionAlignResult();
        }

        public void Reset()
        {
            VisionAlignResult1.Reset();
            VisionAlignResult2.Reset();
        }
    }
}
