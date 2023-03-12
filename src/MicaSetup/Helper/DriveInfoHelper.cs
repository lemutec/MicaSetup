using System.IO;

namespace MicaSetup.Helper;

public static class DriveInfoHelper
{
    public static long GetAvailableFreeSpace(string path)
    {
        string driveName = Path.GetPathRoot(path);
        DriveInfo driveInfo = new(driveName);
        long availableSpace = driveInfo.AvailableFreeSpace;

        return availableSpace;
    }

    public static string GetAvailableFreeSpaceString(string path)
    {
        long availableSpace = GetAvailableFreeSpace(path);

        if (availableSpace >= 1073741824)
        {
            return string.Format("{0:0.##}GB", (double)availableSpace / 1073741824);
        }
        return string.Format("{0:0.##}MB", (double)availableSpace / 1048576);
    }
}
