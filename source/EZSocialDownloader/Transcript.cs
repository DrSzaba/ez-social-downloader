using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace EZSocialDownloader;

/// <summary>Turns caption files (VTT / SRT / JSON / TTML) into a clean,
/// plain-text transcript - just the words, one line each, no timestamps.</summary>
internal static class Transcript
{
	public static readonly string[] CaptionExts = new string[] { ".vtt", ".srt", ".json", ".json3", ".srv1", ".srv2", ".srv3", ".ttml", ".ass", ".lrc" };

	public static bool IsCaptionFile(string path)
	{
		string ext = Path.GetExtension(path).ToLowerInvariant();
		return CaptionExts.Contains(ext);
	}

	public static List<string> Lines(string path)
	{
		string text = File.ReadAllText(path, Encoding.UTF8);
		string ext = Path.GetExtension(path).ToLowerInvariant();
		List<string> raw;
		if (ext.StartsWith(".json"))
		{
			raw = new List<string>();
			// TikTok creator captions / YouTube json3: grab every "text" / "utf8" value
			foreach (Match m in Regex.Matches(text, "\"(?:text|utf8)\"\\s*:\\s*\"((?:[^\"\\\\]|\\\\.)*)\""))
			{
				raw.Add(Regex.Unescape(m.Groups[1].Value));
			}
		}
		else if (ext == ".ttml" || ext.StartsWith(".srv"))
		{
			raw = new List<string>();
			foreach (Match m in Regex.Matches(text, "<(?:p|text)\\b[^>]*>(.*?)</(?:p|text)>", RegexOptions.Singleline))
			{
				raw.Add(Regex.Replace(m.Groups[1].Value, "<br\\s*/?>", " "));
			}
		}
		else
		{
			raw = SubtitleText(text.Replace("\r\n", "\n").Replace('\r', '\n'));
		}
		// YouTube auto-captions "roll": every line is shown again as the next
		// one scrolls in.  Only there are back-to-back repeats removed - in
		// normal captions a repeated line is real (a chorus, for example).
		bool rolling = text.Contains("<c>") || Regex.IsMatch(text, "align:start position:0%");
		List<string> outLines = new List<string>();
		foreach (string r in raw)
		{
			string s = Regex.Replace(r, "<[^>]+>", string.Empty);      // <c>, <i>, <00:00:01.000> ...
			s = Regex.Replace(s, "\\{[^}]*\\}", string.Empty);          // {\an8} style codes
			s = WebUtility.HtmlDecode(s).Replace("\\N", " ").Replace(' ', ' ');
			s = Regex.Replace(s, "\\s+", " ").Trim();
			if (s.Length == 0)
			{
				continue;
			}
			// YouTube auto-captions repeat each line as it scrolls: drop repeats
			if (rolling && outLines.Count > 0)
			{
				string prev = outLines[outLines.Count - 1];
				if (string.Equals(prev, s, StringComparison.OrdinalIgnoreCase))
				{
					continue;
				}
				if (s.StartsWith(prev + " ", StringComparison.OrdinalIgnoreCase))
				{
					outLines[outLines.Count - 1] = s;
					continue;
				}
				if (prev.EndsWith(" " + s, StringComparison.OrdinalIgnoreCase))
				{
					continue;
				}
			}
			outLines.Add(s);
		}
		return outLines;
	}

	private static List<string> SubtitleText(string text)
	{
		List<string> lines = new List<string>();
		bool skipBlock = false;
		foreach (string line0 in text.Split('\n'))
		{
			string line = line0.Trim().TrimStart('﻿');
			if (line.Length == 0)
			{
				skipBlock = false;
				continue;
			}
			if (skipBlock)
			{
				continue;
			}
			if (line.StartsWith("WEBVTT") || line.StartsWith("Kind:") || line.StartsWith("Language:"))
			{
				continue;
			}
			if (line.StartsWith("NOTE") || line == "STYLE" || line == "REGION")
			{
				skipBlock = true;
				continue;
			}
			if (line.Contains("-->"))
			{
				continue;
			}
			if (Regex.IsMatch(line, "^\\d+$"))
			{
				continue;   // SRT cue number
			}
			if (line.StartsWith("Dialogue:"))
			{
				// .ass: text is after the 9th comma
				string[] parts = line.Split(new char[] { ',' }, 10);
				if (parts.Length == 10)
				{
					lines.Add(parts[9]);
				}
				continue;
			}
			if (Regex.IsMatch(line, "^\\[\\d+:\\d+"))
			{
				line = Regex.Replace(line, "^(\\[[\\d:.]+\\])+", string.Empty);   // .lrc
			}
			lines.Add(line);
		}
		return lines;
	}

	/// <summary>Writes "name.lang.txt" next to the caption file. Returns its path, or null if the captions were empty.</summary>
	public static string WriteText(string captionPath)
	{
		List<string> lines = Lines(captionPath);
		if (lines.Count == 0)
		{
			return null;
		}
		string txt = Path.Combine(Path.GetDirectoryName(captionPath), Path.GetFileNameWithoutExtension(captionPath) + ".txt");
		File.WriteAllText(txt, string.Join(Environment.NewLine, lines) + Environment.NewLine, new UTF8Encoding(true));
		return txt;
	}
}
