using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Web.WebView2.Core;

namespace EZSocialDownloader;

internal class SiteInfo
{
	public string Key;

	public string Name;

	public string LoginUrl;

	public string[] CookieUrls;

	public string[] SessionCookies;

	public static readonly SiteInfo[] All = new SiteInfo[5]
	{
		new SiteInfo
		{
			Key = "tiktok",
			Name = "TikTok",
			LoginUrl = "https://www.tiktok.com/login",
			CookieUrls = new string[1] { "https://www.tiktok.com" },
			SessionCookies = new string[2] { "sessionid", "sessionid_ss" }
		},
		new SiteInfo
		{
			Key = "instagram",
			Name = "Instagram",
			LoginUrl = "https://www.instagram.com/accounts/login/",
			CookieUrls = new string[1] { "https://www.instagram.com" },
			SessionCookies = new string[1] { "sessionid" }
		},
		new SiteInfo
		{
			Key = "facebook",
			Name = "Facebook",
			LoginUrl = "https://www.facebook.com/login",
			CookieUrls = new string[1] { "https://www.facebook.com" },
			SessionCookies = new string[1] { "c_user" }
		},
		new SiteInfo
		{
			Key = "youtube",
			Name = "YouTube",
			LoginUrl = "https://accounts.google.com/ServiceLogin?service=youtube&continue=https%3A%2F%2Fwww.youtube.com%2F",
			CookieUrls = new string[3] { "https://www.youtube.com", "https://accounts.google.com", "https://www.google.com" },
			SessionCookies = new string[3] { "LOGIN_INFO", "SAPISID", "__Secure-3PAPISID" }
		},
		new SiteInfo
		{
			Key = "x",
			Name = "X",
			LoginUrl = "https://x.com/i/flow/login",
			CookieUrls = new string[2] { "https://x.com", "https://twitter.com" },
			SessionCookies = new string[1] { "auth_token" }
		}
	};

	public string CookieFile => Path.Combine(Paths.LoginDir, Key + ".txt");

	public static SiteInfo ForName(string platform)
	{
		if (platform == "YouTube Shorts")
		{
			platform = "YouTube";
		}
		return All.FirstOrDefault((SiteInfo s) => s.Name == platform);
	}

	public bool IsLoggedIn()
	{
		try
		{
			if (!File.Exists(CookieFile))
			{
				return false;
			}
			long num = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
			string[] array = File.ReadAllLines(CookieFile);
			foreach (string text in array)
			{
				string[] array2 = text.Split(new char[1] { '\t' });
				if (array2.Length >= 7 && SessionCookies.Contains(array2[5]) && array2[6].Length > 0 && long.TryParse(array2[4], out var result) && (result == 0 || result > num))
				{
					return true;
				}
			}
		}
		catch
		{
		}
		return false;
	}

	public void ForgetLogin()
	{
		try
		{
			if (File.Exists(CookieFile))
			{
				File.Delete(CookieFile);
			}
		}
		catch
		{
		}
	}

	public static void MergeCookies()
	{
		try
		{
			StringBuilder stringBuilder = new StringBuilder("# Netscape HTTP Cookie File\n");
			SiteInfo[] all = All;
			foreach (SiteInfo siteInfo in all)
			{
				if (!siteInfo.IsLoggedIn())
				{
					continue;
				}
				string[] array = File.ReadAllLines(siteInfo.CookieFile);
				foreach (string text in array)
				{
					if (text.Length > 0 && (!text.StartsWith("#") || text.StartsWith("#HttpOnly_")))
					{
						stringBuilder.Append(text).Append('\n');
					}
				}
			}
			Directory.CreateDirectory(Paths.DataDir);
			File.WriteAllText(Paths.AllCookies, stringBuilder.ToString());
		}
		catch
		{
		}
	}

	public static bool HasSession(IEnumerable<CoreWebView2Cookie> cookies, SiteInfo site)
	{
		return cookies.Any((CoreWebView2Cookie c) => site.SessionCookies.Contains(c.Name) && !string.IsNullOrEmpty(c.Value));
	}

	public static void WriteNetscape(string path, IEnumerable<CoreWebView2Cookie> cookies)
	{
		StringBuilder stringBuilder = new StringBuilder("# Netscape HTTP Cookie File\n");
		long num = (long)(DateTime.UtcNow.AddDays(30.0) - new DateTime(1970, 1, 1)).TotalSeconds;
		foreach (CoreWebView2Cookie cookie in cookies)
		{
			string text = cookie.Domain ?? string.Empty;
			if (text.Length == 0)
			{
				continue;
			}
			long num2 = num;
			if (!cookie.IsSession)
			{
				try
				{
					num2 = (long)(cookie.Expires.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds;
				}
				catch
				{
				}
				if (num2 <= 0)
				{
					num2 = num;
				}
			}
			string value = (cookie.Value ?? string.Empty).Replace("\t", " ").Replace("\n", string.Empty).Replace("\r", string.Empty);
			stringBuilder.Append((!cookie.IsHttpOnly) ? string.Empty : "#HttpOnly_").Append(text).Append('\t')
				.Append((!text.StartsWith(".")) ? "FALSE" : "TRUE")
				.Append('\t')
				.Append((!string.IsNullOrEmpty(cookie.Path)) ? cookie.Path : "/")
				.Append('\t')
				.Append((!cookie.IsSecure) ? "FALSE" : "TRUE")
				.Append('\t')
				.Append(num2)
				.Append('\t')
				.Append(cookie.Name)
				.Append('\t')
				.Append(value)
				.Append('\n');
		}
		Directory.CreateDirectory(Path.GetDirectoryName(path));
		File.WriteAllText(path, stringBuilder.ToString());
	}
}
