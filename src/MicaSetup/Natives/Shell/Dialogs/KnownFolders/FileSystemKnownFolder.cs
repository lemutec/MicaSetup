﻿using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

#pragma warning disable CS8618

namespace MicaSetup.Shell.Dialogs;

public class FileSystemKnownFolder : ShellFileSystemFolder, IKnownFolder, IDisposable
{
    private IKnownFolderNative knownFolderNative;
    private KnownFolderSettings knownFolderSettings;

    internal FileSystemKnownFolder(IShellItem2 shellItem) : base(shellItem)
    {
    }

    internal FileSystemKnownFolder(IKnownFolderNative kf)
    {
        Debug.Assert(kf != null);
        knownFolderNative = kf!;

        var guid = new Guid(ShellIIDGuid.IShellItem2);
        knownFolderNative!.GetShellItem(0, ref guid, out nativeShellItem);
    }

    public string CanonicalName => KnownFolderSettings.CanonicalName;

    public FolderCategory Category => KnownFolderSettings.Category;

    public DefinitionOptions DefinitionOptions => KnownFolderSettings.DefinitionOptions;

    public string Description => KnownFolderSettings.Description;

    public System.IO.FileAttributes FileAttributes => KnownFolderSettings.FileAttributes;

    public Guid FolderId => KnownFolderSettings.FolderId;

    public string FolderType => KnownFolderSettings.FolderType;

    public Guid FolderTypeId => KnownFolderSettings.FolderTypeId;

    public string LocalizedName => KnownFolderSettings.LocalizedName;

    public string LocalizedNameResourceId => KnownFolderSettings.LocalizedNameResourceId;

    public Guid ParentId => KnownFolderSettings.ParentId;

    public override string ParsingName => base.ParsingName;

    public override string Path => KnownFolderSettings.Path;

    public bool PathExists => KnownFolderSettings.PathExists;

    public RedirectionCapability Redirection => KnownFolderSettings.Redirection;

    public string RelativePath => KnownFolderSettings.RelativePath;

    public string Security => KnownFolderSettings.Security;

    public string Tooltip => KnownFolderSettings.Tooltip;

    public string TooltipResourceId => KnownFolderSettings.TooltipResourceId;

    private KnownFolderSettings KnownFolderSettings
    {
        get
        {
            if (knownFolderNative == null)
            {
                if (nativeShellItem != null && base.PIDL == IntPtr.Zero)
                {
                    base.PIDL = ShellHelper.PidlFromShellItem(nativeShellItem);
                }

                if (base.PIDL != IntPtr.Zero)
                {
                    knownFolderNative = KnownFolderHelper.FromPIDL(base.PIDL);
                }

                Debug.Assert(knownFolderNative != null);
            }

            knownFolderSettings ??= new KnownFolderSettings(knownFolderNative!);

            return knownFolderSettings;
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            knownFolderSettings = null!;
        }

        if (knownFolderNative != null)
        {
            Marshal.ReleaseComObject(knownFolderNative);
            knownFolderNative = null!;
        }

        base.Dispose(disposing);
    }
}
