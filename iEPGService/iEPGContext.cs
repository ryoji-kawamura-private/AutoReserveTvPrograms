using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace iEPGService
{
    class iEPGContext : ApplicationContext
    {
        internal ServiceHost iEPGServiceHost { get; set; }
        public iEPGContext()
        {
            this.iEPGServiceHost = new ServiceHost(typeof(iEPGService));
            this.iEPGServiceHost.Open();
        }
    }
}
