using System;

namespace MicaSetup.Shell.Dialogs;

[Flags]
public enum ShellObjectChangeTypes
{
    None = 0,
    ItemRename = 0x00000001,
    ItemCreate = 0x00000002,
    ItemDelete = 0x00000004,
    DirectoryCreate = 0x00000008,
    DirectoryDelete = 0x00000010,
    MediaInsert = 0x00000020,
    MediaRemove = 0x00000040,
    DriveRemove = 0x00000080,
    DriveAdd = 0x00000100,
    NetShare = 0x00000200,
    NetUnshare = 0x00000400,
    AttributesChange = 0x00000800,
    DirectoryContentsUpdate = 0x00001000,
    Update = 0x00002000,
    ServerDisconnect = 0x00004000,
    SystemImageUpdate = 0x00008000,
    DirectoryRename = 0x00020000,
    FreeSpace = 0x00040000,
    AssociationChange = 0x08000000,
    DiskEventsMask = 0x0002381F,
    GlobalEventsMask = 0x0C0581E0,
    AllEventsMask = 0x7FFFFFFF,
    FromInterrupt = unchecked((int)0x80000000),
}
