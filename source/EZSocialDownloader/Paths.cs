using System;
using System.IO;

namespace EZSocialDownloader;

internal static class Paths
{
	public static readonly string AppDir = AppDomain.CurrentDomain.BaseDirectory;

	public static readonly string BinDir = Path.Combine(AppDir, "bin");

	public static readonly string DataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "EZ Social Downloader");

	public static readonly string EngineDir = Path.Combine(DataDir, "engine");

	public static readonly string Engine = Path.Combine(EngineDir, "yt-dlp.exe");

	public static readonly string Settings = Path.Combine(DataDir, "settings.ini");

	public static readonly string LogFile = Path.Combine(DataDir, "log.txt");

	public static readonly string LoginDir = Path.Combine(DataDir, "logins");

	public static readonly string BrowserDir = Path.Combine(DataDir, "browser");

	public static readonly string AllCookies = Path.Combine(DataDir, "cookies-all.txt");

	public const string EngineUrl = "https://github.com/yt-dlp/yt-dlp/releases/latest/download/yt-dlp.exe";

	public static string Ffmpeg => Path.Combine(BinDir, "ffmpeg.exe");

	public static string Js => Path.Combine(BinDir, "qjs.exe");
}
