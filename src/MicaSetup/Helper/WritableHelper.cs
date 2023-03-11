using System.IO;

namespace MicaSetup.Core;

public static class WritableHelper
{
    public static bool CheckWritable(string fileName)
    {
        if (!File.Exists(fileName))
        {
            return true;
        }

        try
        {
            using FileStream fileStream = new(fileName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            return fileStream.CanWrite;
        }
        catch
        {
            return false;
        }
    }
}
