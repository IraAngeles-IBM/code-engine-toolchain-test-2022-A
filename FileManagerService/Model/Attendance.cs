using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileManagerService.Model
{
    public class AttendanceLog
    {
        public string bio_id        { get; set; }
        public string date_time     { get; set; }
        public int    in_out        { get; set; }
        public string terminal_id   { get; set; }
        public int    created_by    { get; set; }
    }


}
