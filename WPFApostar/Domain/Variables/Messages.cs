
using System.IO;
using System.Reflection;

namespace WPFApostar.Domain.Variables;
public static class AppInfo
{
    public static string APP_NAME { get { return "Apsotar"; } }
    public static string APP_DIR { get { return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty; } }
}