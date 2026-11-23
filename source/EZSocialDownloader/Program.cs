using System;
using System.Net;
using System.Windows.Forms;

namespace EZSocialDownloader;

internal static class Program
{
	[STAThread]
	private static void Main()
	{
		try
		{
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
		}
		catch
		{
		}
		Application.EnableVisualStyles();
		Application.SetCompatibleTextRenderingDefault(false);
		Application.Run((Form)(object)new MainForm());
	}
}
