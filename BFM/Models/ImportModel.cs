using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BFM.Models
{
    internal class ImportModel : INotifyPropertyChanged
    {
        private string bloomFilter;
        private string textFile;
        private string comments;
        private string lines;

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
