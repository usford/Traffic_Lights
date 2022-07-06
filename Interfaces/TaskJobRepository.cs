using System.Collections.Generic;
using Traffic_Lights.Models;

namespace Traffic_Lights.Interfaces {
    public interface TaskJobRepository {
        List<TaskJob> GetTaskJobs();
    }
}
