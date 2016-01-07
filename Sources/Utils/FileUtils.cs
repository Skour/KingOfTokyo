using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class FileUtils
{
    public static void EmptyDirectory(string aPath)
    {
        Debug.Assert(Directory.Exists(aPath));

        DirectoryInfo logDir = new DirectoryInfo(aPath);

        foreach (FileInfo file in logDir.GetFiles())
        {
            file.Delete();
        }

        foreach (DirectoryInfo dir in logDir.GetDirectories())
        {
            dir.Delete(true);
        }
    }

    public static void CreateOrEmptyDirectory(string aPath)
    {
        if (Directory.Exists(aPath))
        {
            EmptyDirectory(aPath);
        }
        else
        {
            Directory.CreateDirectory(aPath);
        }
    }    
}
