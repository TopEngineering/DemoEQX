using EQX.Core.Vision;

namespace EQX.TemplateProject.Defines.Vision
{
    public class VisionAlignResult
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double T { get; set; }
        public EVisionJudge Judge { get; set; } = EVisionJudge.NG;
        public string NGMessage { get; set; } = string.Empty;

        public void Reset()
        {
            X = 0;
            Y = 0;
            T = 0;
            Judge = EVisionJudge.NG;
            NGMessage = string.Empty;
        }
    }
}
