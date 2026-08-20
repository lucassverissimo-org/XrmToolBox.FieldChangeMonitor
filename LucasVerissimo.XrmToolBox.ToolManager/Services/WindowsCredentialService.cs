using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

namespace LucasVerissimo.XrmToolBox.ToolManager;

internal static class WindowsCredentialService
{
    private const string CredentialTarget = "LucasVerissimo.XrmToolBox.ToolManager/NuGet";
    private const int GenericCredential = 1;
    private const int LocalMachinePersistence = 2;

    public static string? ReadNuGetApiKey()
    {
        if (!CredRead(CredentialTarget, GenericCredential, 0, out IntPtr credentialPointer))
        {
            int error = Marshal.GetLastWin32Error();
            if (error == 1168)
            {
                return null;
            }

            throw new Win32Exception(error);
        }

        try
        {
            NativeCredential credential = Marshal.PtrToStructure<NativeCredential>(
                credentialPointer
            );
            if (credential.CredentialBlob == IntPtr.Zero || credential.CredentialBlobSize == 0)
            {
                return null;
            }

            byte[] secretBytes = new byte[credential.CredentialBlobSize];
            Marshal.Copy(credential.CredentialBlob, secretBytes, 0, secretBytes.Length);
            return Encoding.Unicode.GetString(secretBytes);
        }
        finally
        {
            CredFree(credentialPointer);
        }
    }

    public static void SaveNuGetApiKey(string apiKey)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiKey);

        byte[] secretBytes = Encoding.Unicode.GetBytes(apiKey);
        if (secretBytes.Length > 5120)
        {
            throw new ArgumentException(
                "A credencial é maior que o limite do Windows.",
                nameof(apiKey)
            );
        }

        IntPtr secretPointer = Marshal.StringToCoTaskMemUni(apiKey);
        try
        {
            Marshal.Copy(secretBytes, 0, secretPointer, secretBytes.Length);
            NativeCredential credential = new()
            {
                Type = GenericCredential,
                TargetName = CredentialTarget,
                CredentialBlobSize = secretBytes.Length,
                CredentialBlob = secretPointer,
                Persist = LocalMachinePersistence,
                UserName = Environment.UserName,
            };

            if (!CredWrite(ref credential, 0))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }
        finally
        {
            Marshal.ZeroFreeCoTaskMemUnicode(secretPointer);
        }
    }

    [DllImport(
        "advapi32.dll",
        EntryPoint = "CredReadW",
        CharSet = CharSet.Unicode,
        SetLastError = true
    )]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool CredRead(string target, int type, int flags, out IntPtr credential);

    [DllImport(
        "advapi32.dll",
        EntryPoint = "CredWriteW",
        CharSet = CharSet.Unicode,
        SetLastError = true
    )]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool CredWrite(ref NativeCredential credential, int flags);

    [DllImport("advapi32.dll")]
    private static extern void CredFree(IntPtr credential);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct NativeCredential
    {
        public int Flags;
        public int Type;
        public string TargetName;
        public string? Comment;
        public long LastWritten;
        public int CredentialBlobSize;
        public IntPtr CredentialBlob;
        public int Persist;
        public int AttributeCount;
        public IntPtr Attributes;
        public string? TargetAlias;
        public string UserName;
    }
}
