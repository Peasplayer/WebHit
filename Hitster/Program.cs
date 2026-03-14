using System.Drawing.Text;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
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
    
    // Quelle: https://medium.com/@tarakshah/this-article-explains-how-to-check-the-similarity-between-two-string-in-percentage-or-score-from-0-83e206bf6bf5
    public static double CompareStrings(string str1, string str2)
    {
        var pairs1 = WordLetterPairs(str1.ToUpper());
        var pairs2 = WordLetterPairs(str2.ToUpper());

        int intersection = 0;
        int union = pairs1.Count + pairs2.Count;

        for (int i = 0; i < pairs1.Count; i++)
        {
            for (int j = 0; j < pairs2.Count; j++)
            {
                if (pairs1[i] == pairs2[j])
                {
                    intersection++;
                    pairs2.RemoveAt(j);//Must remove the match to prevent "AAAA" from appearing to match "AA" with 100% success
                    break;
                }
            }
        }

        return (2.0 * intersection * 100) / union; //returns in percentage
        //return (2.0 * intersection) / union; //returns in score from 0 to 1
    }
    
    private static List<string> WordLetterPairs(string str)
    {
        var AllPairs = new List<string>();

        // Tokenize the string and put the tokens/words into an array
        string[] Words = Regex.Split(str, @"\s");

        // For each word
        for (int w = 0; w < Words.Length; w++)
        {
            if (!string.IsNullOrEmpty(Words[w]))
            {
                // Find the pairs of characters
                String[] PairsInWord = LetterPairs(Words[w]);

                for (int p = 0; p < PairsInWord.Length; p++)
                {
                    AllPairs.Add(PairsInWord[p]);
                }
            }
        }
        return AllPairs;
    }

    // Generates an array containing every two consecutive letters in the input string
    private static string[] LetterPairs(string str)
    {
        int numPairs = str.Length - 1;
        string[] pairs = new string[numPairs];

        for (int i = 0; i < numPairs; i++)
        {
            pairs[i] = str.Substring(i, 2);
        }
        return pairs;
    }
}