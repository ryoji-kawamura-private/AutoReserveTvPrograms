using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using iEPG.Entity;

namespace iEPGService
{
    [ServiceContract]
    interface IiEPGService
    {
        [OperationContract]
        IEnumerable<TvProgram> SearchTvProgramsByString(string searchCondition);

        [OperationContract]
        IEnumerable<TvProgram> SearcgTvProgramsByDbCondition();
    }
}
