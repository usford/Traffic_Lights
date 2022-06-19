using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Traffic_Lights {
    public interface TaskJobRepository {
        List<TaskJob> GetTaskJobs();
    }
}
