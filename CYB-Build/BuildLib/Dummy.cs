using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskLib
{
    public static class Dummy
    {
        /// <summary>
        /// To allow zipper.exe to be copied we need this trick, calling a dummy class contained in the zipper.exe assembly, otherwise that assembly will not be copied to output folder.
        /// (NB: this is done to avoid referencing the zipper.exe directly in the CYB-Build project)
        /// </summary>
        static Dummy()
        {
            new zipper.Dummy();
        }
    }
}
