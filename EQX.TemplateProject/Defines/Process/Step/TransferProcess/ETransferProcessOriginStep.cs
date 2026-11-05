using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQX.TemplateProject.Defines
{
    public enum ETransferProcessOriginStep
    {
        Start,
        XAis_Origin,
        XAis_Origin_Wait,
        YAis_Origin,
        YAis_Origin_Wait,
        ZAxis_Origin,
        ZAxis_Origin_Wait,
        End,
    }
}
