using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace zipper
{
    public class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.Error.WriteLine("usage: zipper <input file/folder> <output file>");
                Environment.ExitCode = -1;
                return;
            }

            var input = args[0];
            var output = args[1];

            try
            {
                if (File.Exists(output))
                    File.Delete(output);

                using (var zipfile = new Ionic.Zip.ZipFile(output))
                {
                    if (File.Exists(input))
                        zipfile.AddFile(input);
                    else if (Directory.Exists(input))
                        zipfile.AddDirectory(input, Path.GetFileName(input));

                    zipfile.Save();

                    Console.WriteLine($"Successfully zipped '{input}' {(Directory.Exists(input) ? "folder" : "file")} to {output}");
                }
            }
            catch (Exception exc)
            {
                Console.Error.WriteLine($"Error writing {output} file: {exc.ToString()}");
                Environment.ExitCode = -1;
            }
        }
    }
}
