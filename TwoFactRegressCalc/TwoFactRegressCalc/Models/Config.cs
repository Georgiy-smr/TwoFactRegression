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
    }
}
