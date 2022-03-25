using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cinema_prototype_3;


namespace cinema_prototype_3
{
    internal interface IStorable
    {
        void ProcessData(string line);
        void ReadFile(string filename);
        void UpdateFile(string filename);

    }
}
