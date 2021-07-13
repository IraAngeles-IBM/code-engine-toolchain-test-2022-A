using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TenantMasterSetupService.Model
{
    public class ModuleResponse
    {
        public int module_id { get; set; }

        public string module_name { get; set; }

        public string module_type { get; set; }

        public string link { get; set; }

        public bool has_approval { get; set; }

        public string classes { get; set; }
        public int count { get; set; }
    }
}
