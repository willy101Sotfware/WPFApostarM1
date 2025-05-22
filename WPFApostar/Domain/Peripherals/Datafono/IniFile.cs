using System.IO;

namespace WPFApostar.Domain.Peripherals.Datafono;

internal class IniFile
{
    private Dictionary<string, string> iniParams;

    public IniFile(string INIPath)
    {
        StreamReader streamReader = new StreamReader(INIPath);
        iniParams = new Dictionary<string, string>();
        string text;
        while ((text = streamReader.ReadLine()) != null)
        {
            if (text.Contains("="))
            {
                string[] array = text.Split(new char[1] { '=' });
                iniParams[array[0].Trim()] = array[1].Trim();
            }
        }
    }

    public string getParam(string key)
    {
        return iniParams[key];
    }
}