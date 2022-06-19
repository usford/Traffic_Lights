using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Traffic_Lights.Models {
    public class MenuTasksModel {
        public static List<List<ExcelTaskJobRepository.ElementInfoDB>> GetTaskTables(int indexTable1, int indexTable2) {
            var listTaskTables = new List<List<ExcelTaskJobRepository.ElementInfoDB>>();

            listTaskTables.Add(ExcelTaskJobRepository.GetElementsFromExcel(indexTable1));
            listTaskTables.Add(ExcelTaskJobRepository.GetElementsFromExcel(indexTable2));

            return listTaskTables;
        }
    }
}
