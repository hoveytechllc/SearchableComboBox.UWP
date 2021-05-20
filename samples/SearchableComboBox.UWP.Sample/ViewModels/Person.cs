using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchableComboBox.UWP.Sample.ViewModels
{
    public class Person
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public bool IsMatch(string filterText)
        {
            if (string.IsNullOrEmpty(filterText))
                return false;

            filterText = filterText.ToLower();

            return FirstName.ToLower().Contains(filterText)
                   || LastName.ToLower().Contains(filterText);
        }
    }
}
