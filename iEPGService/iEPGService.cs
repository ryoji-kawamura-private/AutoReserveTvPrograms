using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using iEPG;
using iEPG.Entity;

namespace iEPGService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class iEPGService : MarshalByRefObject, IiEPGService
    {
        iEPGController _iEGPController = new iEPGController();
        public iEPGController iEPGController
        {
            get
            {
                return this._iEGPController;
            }
        }

        private Regex duplicateSpaceRegex = new Regex(" +");
        public IEnumerable<TvProgram> SearchTvProgramsByString(string searchConditionsString)
        {
            IEnumerable<FreeWordConditon> freeWordConditions;
            freeWordConditions = duplicateSpaceRegex.Replace(searchConditionsString, " ").Split(' ', '　')
                .Select(conditonString =>
                {
                    return new FreeWordConditon()
                    {
                        Id = Guid.NewGuid(),
                        FreeWord = conditonString,
                    };
                }).ToArray();
            return this.iEPGController.GetTvPrograms(freeWordConditions);
        }

        public IEnumerable<TvProgram> SearcgTvProgramsByDbCondition()
        {
            return this.iEPGController.GetTvPrograms(this.iEPGController.GetFreeWordConditions());
        }
    }
}
