using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwoFactRegressCalc.Models
{
    public class Config
    {
        /// <summary>
        /// Путь распложению сохраняемыхё файлов 
        /// </summary>
        public string FilePath { get; set; } = App.FileKeepDefaultPath;

        /// <summary>
        /// Класс точности датчика, % - порог проверки точек калибровки
        /// </summary>
        public double AccuracyClassPercent { get; set; } = 0.01;
    }
}
