using System;
using System.Text;
using System.IO;
using System.Reflection;

namespace quake_extract
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("How to use:");
                Console.WriteLine("1. Start the game once");
                Console.WriteLine(@"2. Place this executable in \dev_hdd0\game\BLUS30132\USRDIR\");
                Console.WriteLine("3. Launch the executable with the path to the install.pkg file as a parameter");
                Console.WriteLine(@"The file is at \PS3_GAME\USRDIR\install.pkg by default");
                return;
            }

            string basePath = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            using (FileStream fs = new FileStream(args[0], FileMode.Open, FileAccess.Read, FileShare.Read, 65536))
            {
                while (fs.Position < fs.Length)
                {
                    byte[] path = new byte[256];
                    byte[] size = new byte[4];

                    fs.Read(path, 0, 256);
                    string stringPath = Encoding.UTF8.GetString(path).Trim('\0');

                    if (stringPath == "\\base.cachemap")
                    {
                        break;
                    }

                    Console.WriteLine($"path = {stringPath}");

                    fs.Read(size, 0, 4);
                    int intsize = size[0] * (1 << 24) + size[1] * (1 << 16) + size[2] * (1 << 8) + size[3];
                    Console.WriteLine($"size = {intsize}");

                    byte[] data = new byte[intsize];
                    fs.Read(data, 0, intsize);

                    Directory.CreateDirectory(Path.GetDirectoryName(basePath + stringPath));
                    using (FileStream createFS = File.Create(basePath + stringPath))
                    {
                        createFS.Write(data, 0, intsize);
                    }
                }
            }
            Console.WriteLine("Extraction complete!");
        }
    }
}
