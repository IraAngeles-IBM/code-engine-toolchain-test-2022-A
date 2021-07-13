using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceManagementService.Model
{
    public class AttendanceDashboardResponse
    {

        public string  title            { get; set; }
        public string  start            { get; set; }
        public string  end              { get; set; }
        public string  description      { get; set; }
        public string  backgroundColor { get; set; }
        public string  borderColor      { get; set; }
        public int      id      { get; set; }
    }
}
