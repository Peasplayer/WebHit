using System.Drawing.Text;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using Hitster.Networking;

namespace Hitster;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        var net = new NetworkManager("ws://127.0.0.1:9443", "Dieter");
        
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();
        Application.SetCompatibleTextRenderingDefault(true);
        Application.Run(new MenueForm());
    }

    public static FontFamily MontserratBold { get; } = GetFont("Montserrat-Bold");
    public static FontFamily MontserratMediumItalic { get; } = GetFont("Montserrat-MediumItalic");
    public static FontFamily MontserratSemiBold { get; } = GetFont("Montserrat-SemiBold");

    private static FontFamily GetFont(string name)
    {
        using (Stream fontStream = GetResource(name + ".ttf"))
        {
            byte[] fontData = new byte[fontStream.Length];
            fontStream.ReadExactly(fontData, 0, (int)fontStream.Length);

            IntPtr fontPtr = Marshal.AllocCoTaskMem(fontData.Length);
            Marshal.Copy(fontData, 0, fontPtr, fontData.Length);

            var font = new PrivateFontCollection();
            font.AddMemoryFont(fontPtr, fontData.Length);
            Marshal.FreeCoTaskMem(fontPtr);
            return font.Families[0];
        }
    }
    
    public static Stream GetResource(string name)
    {
        var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Hitster.Resources." + name);
        if (stream == null)
            throw new MissingManifestResourceException(name + " does not exist!");
        
        return stream;
    }
}