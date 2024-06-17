using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TwoFactRegressCalc.Infrastructure.DI.Services.FileDialog
{
    public enum FileTypes
    {
        IncorrectType,
        Excel,
        XML,
        Json,
        DataBase,
        Pdf
    }
    internal class FileDialogService : IDialogService
    {
        #region Public Properties 

        /// <summary>
        /// Путь к файлу
        /// </summary>
        public string FilePath { get; set; } = null!;

        /// <summary>
        /// Путь к папке с файлом
        /// </summary>
        public string FolderPath
            => FilePath.Remove(FilePath.LastIndexOf('\\'));

        /// <summary>
        /// Наименование файла
        /// </summary>
        public string FileName { get; set; } = null!;

        /// <summary>
        /// Выбранный тип файла
        /// </summary>
        public FileTypes FileType { get; set; }

        /// <summary>
        /// Выбранный тип фильтра
        /// </summary>
        public int FilterIndex { get; set; } = 1;

        /// <summary>
        /// Фильтр 
        /// </summary>
        public string Filter { get; set; } = null!;

        /// <summary>
        /// Начальная папка
        /// </summary>
        public string? InitialDirectory
        {
            get;
            set;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Открыть файл
        /// </summary>
        /// <returns></returns>
        public bool OpenFileDialog()
        {
            var openFileDialog = new OpenFileDialog();
            if (InitialDirectory != null)
                openFileDialog.InitialDirectory = InitialDirectory;
            openFileDialog.Filter = Filter;
            // "Excel file (*.xlsx)|*.xlsx|XML file (*.xml)|*.xml|Json file (*.json)|*.json|Database file (*.db)|*.db";
            openFileDialog.FilterIndex = FilterIndex;
            if (openFileDialog.ShowDialog() != true)
                return false;

            FileName = Path.GetFileNameWithoutExtension(openFileDialog.FileName);

            FilePath = openFileDialog.FileName;
            FileType = (FileTypes)openFileDialog.FilterIndex;
            return true;
        }

        /// <summary>
        /// Сохранить файл
        /// </summary>
        /// <returns></returns>
        public bool SaveFileDialog()
        {
            var saveFileDialog = new SaveFileDialog();
            if (InitialDirectory != null)
                saveFileDialog.InitialDirectory = InitialDirectory;
            saveFileDialog.Filter = Filter;
            //"Excel file (*.xlsx)|*.xlsx|XML file (*.xml)|*.xml|Json file (*.json)|*.json|Database file (*.db)|*.db";
            saveFileDialog.FilterIndex = FilterIndex;
            if (saveFileDialog.ShowDialog() != true)
                return false;
            FilePath = saveFileDialog.FileName;
            FileType = (FileTypes)saveFileDialog.FilterIndex;
            return true;
        }



        /// <summary>
        /// Скопировать файл
        /// </summary>
        /// <returns></returns>
        public bool CopyFileDialog()
        {
            var saveFileDialog = new OpenFileDialog();
            if (InitialDirectory != null)
                saveFileDialog.InitialDirectory = InitialDirectory;
            saveFileDialog.Filter = Filter;
            //"Excel file (*.xlsx)|*.xlsx|XML file (*.xml)|*.xml|Json file (*.json)|*.json|Database file (*.db)|*.db";
            saveFileDialog.FilterIndex = FilterIndex;
            if (saveFileDialog.ShowDialog() != true)
                return false;
            FilePath = saveFileDialog.FileName;
            FileType = (FileTypes)saveFileDialog.FilterIndex;
            return true;
        }


        /// <summary>
        /// Сообщение
        /// </summary>
        /// <param name="message"></param>
        public void ShowMessage(string message)
        {
            MessageBox.Show(message);
        }

        #endregion
    }
}
