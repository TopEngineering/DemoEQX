using System.ComponentModel;

namespace EQX.TemplateProject.Defines
{
    /// <summary>
    /// Sequences of the machine
    /// </summary>
    public enum ESequence
    {
        Stop,

        AutoRun,
        /// <summary>
        /// Move Units to non-collision positions before starting Auto Run
        /// </summary>
        Ready,

        [Description("Sequence Pick")]
        Pick,
        [Description("Sequence Place")]
        Place,
        [Description("Check Input/Output Trays")]
        CheckTray,
        [Description("Vision Inspection")]
        Inspect,
    }

    public enum ESemiSequence
    {
        None,

        [Description("Sequence Pick")]
        Pick,
        [Description("Sequence Place")]
        Place,
        [Description("Check Input/Output Trays")]
        CheckTray,
        [Description("Vision Inspection")]
        Inspect,
    }
}
