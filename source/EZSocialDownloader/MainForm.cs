using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace EZSocialDownloader;

internal class MainForm : Form
{
	private static readonly Color Navy = Color.FromArgb(22, 45, 80);

	private static readonly Color Blue = Color.FromArgb(33, 110, 200);

	private static readonly Color Orange = Color.FromArgb(247, 147, 30);

	private static readonly Color Light = Color.FromArgb(244, 246, 250);

	private TextBox urlBox;

	private TextBox folderBox;

	private TextBox logBox;

	private ComboBox qualityBox;

	private readonly Dictionary<string, Label> siteStatus = new Dictionary<string, Label>();

	private readonly Dictionary<string, Button> siteButton = new Dictionary<string, Button>();

	// what to save: video, audio (MP3), transcript, or video + transcript
	private string mode = "video";

	private readonly List<RoundButton> modeButtons = new List<RoundButton>();

	private static readonly string[][] Modes = new string[4][]
	{
		new string[3] { "video", "🎬  Video (MP4)", "The video file" },
		new string[3] { "audio", "🎵  Audio (MP3)", "Just the sound" },
		new string[3] { "transcript", "📝  Transcript / lyrics", "Just the words (no video)" },
		new string[3] { "both", "🎬 + 📝  Video + transcript", "Video and the words" }
	};

	private Button downloadBtn;

	private Button stopBtn;

	private Button updateBtn;

	private ProgressBar bar;

	private Label statusLabel;

	private volatile bool stopRequested;

	private Process current;

	private Thread worker;

	private readonly object procLock = new object();

	private static readonly string[] Qualities = new string[4] { "Best available", "1080p", "720p", "480p" };

	public MainForm()
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Expected O, but got Unknown
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Expected O, but got Unknown
		((Control)this).Text = "EZ Social Downloader";
		((Control)this).Font = new Font("Segoe UI", 10f);
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Control)this).BackColor = Light;
		((Form)this).ClientSize = new Size(900, 780);
		((Control)this).MinimumSize = new Size(760, 720);
		((Form)this).StartPosition = (FormStartPosition)1;
		try
		{
			((Form)this).Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
		}
		catch
		{
		}
		BuildUi();
		LoadSettings();
		RefreshLogins();
		((Form)this).Shown += delegate
		{
			EnsureEngine(forceUpdate: false);
		};
		((Form)this).FormClosing += (FormClosingEventHandler)delegate
		{
			SaveSettings();
			KillCurrent();
		};
	}

	private Button MakeButton(string text, Color back, int width, int height, float size)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Expected O, but got Unknown
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		RoundButton val = new RoundButton();
		((Control)val).Text = text;
		((Control)val).BackColor = back;
		((Control)val).ForeColor = Color.White;
		((ButtonBase)val).FlatStyle = (FlatStyle)0;
		((ButtonBase)val).FlatAppearance.BorderSize = 0;
		((Control)val).Font = new Font("Segoe UI", size, (FontStyle)1);
		((Control)val).Size = new Size(width, height);
		((Control)val).Cursor = Cursors.Hand;
		((Control)val).Margin = new Padding(0, 0, 8, 0);
		return val;
	}

	private Label MakeLabel(string text, bool bold)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Expected O, but got Unknown
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		Label val = new Label();
		((Control)val).Text = text;
		((Control)val).AutoSize = true;
		((Control)val).Font = new Font("Segoe UI", (!bold) ? 10f : 11f, (FontStyle)(bold ? 1 : 0));
		((Control)val).ForeColor = Navy;
		((Control)val).Margin = new Padding(0, 8, 8, 4);
		return val;
	}

	private void BuildUi()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Expected O, but got Unknown
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Expected O, but got Unknown
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Expected O, but got Unknown
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Expected O, but got Unknown
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Expected O, but got Unknown
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Expected O, but got Unknown
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Expected O, but got Unknown
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Expected O, but got Unknown
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Expected O, but got Unknown
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Expected O, but got Unknown
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Expected O, but got Unknown
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Expected O, but got Unknown
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Expected O, but got Unknown
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Expected O, but got Unknown
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Expected O, but got Unknown
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Expected O, but got Unknown
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Expected O, but got Unknown
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Expected O, but got Unknown
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0314: Expected O, but got Unknown
		//IL_033b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0359: Unknown result type (might be due to invalid IL or missing references)
		//IL_036f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0379: Expected O, but got Unknown
		//IL_0387: Unknown result type (might be due to invalid IL or missing references)
		//IL_0391: Expected O, but got Unknown
		//IL_039a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a4: Expected O, but got Unknown
		//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b7: Expected O, but got Unknown
		//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03da: Expected O, but got Unknown
		//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_042e: Unknown result type (might be due to invalid IL or missing references)
		//IL_047c: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ca: Expected O, but got Unknown
		//IL_04dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f3: Expected O, but got Unknown
		//IL_050e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0535: Unknown result type (might be due to invalid IL or missing references)
		//IL_053c: Expected O, but got Unknown
		//IL_0556: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d2: Expected O, but got Unknown
		//IL_05e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_061b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0622: Expected O, but got Unknown
		//IL_0653: Unknown result type (might be due to invalid IL or missing references)
		//IL_066f: Unknown result type (might be due to invalid IL or missing references)
		//IL_067d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0683: Expected O, but got Unknown
		//IL_06a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ad: Expected O, but got Unknown
		//IL_06bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d0: Expected O, but got Unknown
		//IL_06ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f7: Expected O, but got Unknown
		//IL_0707: Unknown result type (might be due to invalid IL or missing references)
		//IL_0732: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f0: Expected O, but got Unknown
		//IL_0812: Unknown result type (might be due to invalid IL or missing references)
		//IL_0827: Unknown result type (might be due to invalid IL or missing references)
		//IL_0831: Expected O, but got Unknown
		//IL_0866: Unknown result type (might be due to invalid IL or missing references)
		//IL_086d: Expected O, but got Unknown
		//IL_0894: Unknown result type (might be due to invalid IL or missing references)
		//IL_08aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b4: Expected O, but got Unknown
		//IL_08c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_08cc: Expected O, but got Unknown
		//IL_08d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_08df: Expected O, but got Unknown
		//IL_08e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e7: Expected O, but got Unknown
		//IL_08fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a0d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a14: Expected O, but got Unknown
		//IL_0a27: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a56: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ab4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aba: Expected O, but got Unknown
		//IL_0adc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0af2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0afc: Expected O, but got Unknown
		//IL_0b17: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b1e: Expected O, but got Unknown
		//IL_0b49: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b89: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b90: Expected O, but got Unknown
		//IL_0bbc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bc6: Expected O, but got Unknown
		Panel val = new Panel();
		((Control)val).Dock = (DockStyle)1;
		((Control)val).Height = 78;
		((Control)val).BackColor = Navy;
		Panel val2 = val;
		Label val3 = new Label();
		((Control)val3).Text = "EZ Social Downloader";
		((Control)val3).ForeColor = Orange;
		((Control)val3).Font = new Font("Segoe UI", 20f, (FontStyle)1);
		((Control)val3).AutoSize = true;
		((Control)val3).Location = new Point(18, 8);
		Label val4 = val3;
		val3 = new Label();
		((Control)val3).Text = "TikTok  •  Instagram Reels  •  Facebook Reels  •  YouTube  •  Shorts  •  X   —   videos, MP3s and transcripts / lyrics";
		((Control)val3).ForeColor = Color.White;
		((Control)val3).Font = new Font("Segoe UI", 9.5f);
		((Control)val3).AutoSize = true;
		((Control)val3).Location = new Point(21, 48);
		Label val5 = val3;
		((Control)val2).Controls.Add((Control)(object)val4);
		((Control)val2).Controls.Add((Control)(object)val5);
		TableLayoutPanel val6 = new TableLayoutPanel();
		((Control)val6).Dock = (DockStyle)5;
		((Control)val6).Padding = new Padding(18, 10, 18, 14);
		val6.ColumnCount = 1;
		val6.RowCount = 9;
		((Control)val6).BackColor = Light;
		TableLayoutPanel val7 = val6;
		val7.ColumnStyles.Add(new ColumnStyle((SizeType)2, 100f));
		val7.RowStyles.Add(new RowStyle((SizeType)0));
		val7.RowStyles.Add(new RowStyle((SizeType)2, 45f));
		val7.RowStyles.Add(new RowStyle((SizeType)0));
		val7.RowStyles.Add(new RowStyle((SizeType)0));
		val7.RowStyles.Add(new RowStyle((SizeType)0));
		val7.RowStyles.Add(new RowStyle((SizeType)0));
		val7.RowStyles.Add(new RowStyle((SizeType)0));
		val7.RowStyles.Add(new RowStyle((SizeType)0));
		val7.RowStyles.Add(new RowStyle((SizeType)2, 55f));
		val7.Controls.Add((Control)(object)MakeLabel("1.  Paste your links here (one per line — as many as you want):", bold: true), 0, 0);
		TextBox val8 = new TextBox();
		((TextBoxBase)val8).Multiline = true;
		val8.ScrollBars = (ScrollBars)2;
		((Control)val8).Dock = (DockStyle)5;
		((Control)val8).Font = new Font("Consolas", 10.5f);
		((TextBoxBase)val8).WordWrap = false;
		val8.AcceptsReturn = true;
		urlBox = val8;
		val7.Controls.Add((Control)(object)urlBox, 0, 1);
		FlowLayoutPanel val9 = new FlowLayoutPanel();
		((Control)val9).AutoSize = true;
		((Control)val9).Dock = (DockStyle)5;
		((Control)val9).Margin = new Padding(0, 6, 0, 0);
		FlowLayoutPanel val10 = val9;
		Button val11 = MakeButton("Paste", Blue, 110, 34, 10f);
		((Control)val11).Click += delegate
		{
			PasteClipboard();
		};
		Button val12 = MakeButton("Clear", Color.Gray, 110, 34, 10f);
		((Control)val12).Click += delegate
		{
			((TextBoxBase)urlBox).Clear();
		};
		((Control)val10).Controls.Add((Control)(object)val11);
		((Control)val10).Controls.Add((Control)(object)val12);
		val7.Controls.Add((Control)(object)val10, 0, 2);
		val6 = new TableLayoutPanel();
		val6.ColumnCount = 4;
		val6.RowCount = 5;
		((Control)val6).AutoSize = true;
		((Control)val6).Dock = (DockStyle)5;
		((Control)val6).Margin = new Padding(0, 10, 0, 0);
		((Control)val6).BackColor = Color.White;
		((Control)val6).Padding = new Padding(10, 6, 10, 8);
		TableLayoutPanel val13 = val6;
		val13.ColumnStyles.Add(new ColumnStyle((SizeType)0));
		val13.ColumnStyles.Add(new ColumnStyle((SizeType)2, 100f));
		val13.ColumnStyles.Add(new ColumnStyle((SizeType)0));
		val13.ColumnStyles.Add(new ColumnStyle((SizeType)0));
		val13.Controls.Add((Control)(object)MakeLabel("Save to:", bold: false), 0, 0);
		val8 = new TextBox();
		((Control)val8).Dock = (DockStyle)5;
		((Control)val8).Margin = new Padding(0, 6, 8, 4);
		folderBox = val8;
		val13.Controls.Add((Control)(object)folderBox, 1, 0);
		Button val14 = MakeButton("Browse", Blue, 96, 30, 9.5f);
		((Control)val14).Margin = new Padding(0, 4, 8, 4);
		((Control)val14).Click += delegate
		{
			BrowseFolder();
		};
		val13.Controls.Add((Control)(object)val14, 2, 0);
		Button val15 = MakeButton("Open folder", Blue, 116, 30, 9.5f);
		((Control)val15).Margin = new Padding(0, 4, 0, 4);
		((Control)val15).Click += delegate
		{
			OpenFolder();
		};
		val13.Controls.Add((Control)(object)val15, 3, 0);
		val13.Controls.Add((Control)(object)MakeLabel("Get:", bold: true), 0, 1);
		FlowLayoutPanel modeRow = new FlowLayoutPanel();
		((Control)modeRow).AutoSize = true;
		((Control)modeRow).Dock = (DockStyle)5;
		((Control)modeRow).Margin = new Padding(0, 4, 0, 2);
		((Control)modeRow).BackColor = Color.White;
		foreach (string[] m in Modes)
		{
			RoundButton mb = new RoundButton();
			mb.Toggle = true;
			((Control)mb).Text = m[1];
			((Control)mb).BackColor = Orange;
			((Control)mb).ForeColor = Color.FromArgb(27, 36, 51);
			((Control)mb).Font = new Font("Segoe UI", 9.5f, (FontStyle)1);
			((Control)mb).Size = new Size((m[0] == "both") ? 230 : ((m[0] == "transcript") ? 196 : 150), 38);
			((Control)mb).Margin = new Padding(0, 2, 8, 2);
			((Control)mb).Tag = m[0];
			string key = m[0];
			((Control)mb).Click += delegate
			{
				SetMode(key);
			};
			new ToolTip().SetToolTip((Control)(object)mb, m[2]);
			modeButtons.Add(mb);
			((Control)modeRow).Controls.Add((Control)(object)mb);
		}
		val13.Controls.Add((Control)(object)modeRow, 1, 1);
		val13.SetColumnSpan((Control)(object)modeRow, 3);
		val13.Controls.Add((Control)(object)MakeLabel("Quality:", bold: false), 0, 2);
		ComboBox val17 = new ComboBox();
		val17.DropDownStyle = (ComboBoxStyle)2;
		((Control)val17).Width = 160;
		((Control)val17).Margin = new Padding(0, 6, 20, 4);
		qualityBox = val17;
		qualityBox.Items.AddRange((object[])Qualities);
		val13.Controls.Add((Control)(object)qualityBox, 1, 2);
		val13.SetColumnSpan((Control)(object)qualityBox, 3);
		val13.Controls.Add((Control)(object)MakeLabel("Logins:", bold: false), 0, 3);
		val9 = new FlowLayoutPanel();
		((Control)val9).AutoSize = true;
		val9.WrapContents = false;
		((Control)val9).Margin = new Padding(0, 4, 0, 0);
		FlowLayoutPanel val19 = val9;
		SiteInfo[] all = SiteInfo.All;
		foreach (SiteInfo siteInfo in all)
		{
			val6 = new TableLayoutPanel();
			val6.ColumnCount = 1;
			val6.RowCount = 3;
			((Control)val6).AutoSize = false;
			((Control)val6).Size = new Size(134, 92);
			((Control)val6).Margin = new Padding(0, 2, 8, 2);
			((Control)val6).BackColor = Light;
			((Control)val6).Padding = new Padding(8, 4, 8, 6);
			TableLayoutPanel val20 = val6;
			val3 = new Label();
			((Control)val3).Text = siteInfo.Name;
			((Control)val3).AutoSize = true;
			((Control)val3).Font = new Font("Segoe UI", 10f, (FontStyle)1);
			((Control)val3).ForeColor = Navy;
			((Control)val3).Margin = new Padding(0, 0, 0, 2);
			Label val21 = val3;
			val3 = new Label();
			((Control)val3).Text = "Not logged in";
			((Control)val3).AutoSize = true;
			((Control)val3).Font = new Font("Segoe UI", 9f);
			((Control)val3).ForeColor = Color.DimGray;
			((Control)val3).Margin = new Padding(0, 0, 0, 4);
			Label val22 = val3;
			Button val23 = MakeButton("Log in", Blue, 104, 30, 9.5f);
			((Control)val23).Margin = new Padding(0);
			SiteInfo captured = siteInfo;
			((Control)val23).Click += delegate
			{
				LoginClicked(captured);
			};
			val20.Controls.Add((Control)(object)val21, 0, 0);
			val20.Controls.Add((Control)(object)val22, 0, 1);
			val20.Controls.Add((Control)(object)val23, 0, 2);
			siteStatus[siteInfo.Key] = val22;
			siteButton[siteInfo.Key] = val23;
			((Control)val19).Controls.Add((Control)(object)val20);
		}
		val13.Controls.Add((Control)(object)val19, 1, 3);
		val13.SetColumnSpan((Control)(object)val19, 3);
		val3 = new Label();
		((Control)val3).Text = "Log in once to each site you use. Posts that need a login (like TikTok 18+ posts) will then download.";
		((Control)val3).AutoSize = true;
		((Control)val3).ForeColor = Color.DimGray;
		((Control)val3).Margin = new Padding(0, 4, 0, 2);
		((Control)val3).Font = new Font("Segoe UI", 9f);
		Label val24 = val3;
		val13.RowCount = 5;
		val13.Controls.Add((Control)(object)val24, 1, 4);
		val13.SetColumnSpan((Control)(object)val24, 3);
		val7.Controls.Add((Control)(object)val13, 0, 3);
		val6 = new TableLayoutPanel();
		val6.ColumnCount = 3;
		val6.RowCount = 1;
		((Control)val6).AutoSize = true;
		((Control)val6).Dock = (DockStyle)5;
		((Control)val6).Margin = new Padding(0, 12, 0, 4);
		TableLayoutPanel val25 = val6;
		val25.ColumnStyles.Add(new ColumnStyle((SizeType)0));
		val25.ColumnStyles.Add(new ColumnStyle((SizeType)2, 100f));
		val25.ColumnStyles.Add(new ColumnStyle((SizeType)0));
		val9 = new FlowLayoutPanel();
		((Control)val9).AutoSize = true;
		val9.WrapContents = false;
		((Control)val9).Margin = new Padding(0);
		FlowLayoutPanel val26 = val9;
		downloadBtn = MakeButton("2.  DOWNLOAD", Orange, 220, 52, 14f);
		((Control)downloadBtn).Click += delegate
		{
			StartDownloads();
		};
		stopBtn = MakeButton("Stop", Color.FromArgb(200, 50, 50), 100, 52, 12f);
		((Control)stopBtn).Enabled = false;
		((Control)stopBtn).Click += delegate
		{
			StopDownloads();
		};
		((Control)val26).Controls.Add((Control)(object)downloadBtn);
		((Control)val26).Controls.Add((Control)(object)stopBtn);
		val25.Controls.Add((Control)(object)val26, 0, 0);
		updateBtn = MakeButton("Update engine", Navy, 150, 40, 10f);
		((Control)updateBtn).Margin = new Padding(0, 6, 0, 0);
		((Control)updateBtn).Click += delegate
		{
			EnsureEngine(forceUpdate: true);
		};
		val9 = new FlowLayoutPanel();
		((Control)val9).AutoSize = true;
		val9.WrapContents = false;
		((Control)val9).Margin = new Padding(0);
		FlowLayoutPanel val27 = val9;
		Button val28 = MakeButton("Help", Blue, 90, 40, 10f);
		((Control)val28).Margin = new Padding(0, 6, 8, 0);
		((Control)val28).Click += delegate
		{
			ShowHelp();
		};
		((Control)val27).Controls.Add((Control)(object)val28);
		((Control)val27).Controls.Add((Control)(object)updateBtn);
		val25.Controls.Add((Control)(object)val27, 2, 0);
		val7.Controls.Add((Control)(object)val25, 0, 4);
		val3 = new Label();
		((Control)val3).Text = "Ready.";
		((Control)val3).AutoSize = true;
		((Control)val3).ForeColor = Navy;
		((Control)val3).Margin = new Padding(0, 6, 0, 2);
		((Control)val3).Font = new Font("Segoe UI", 10f, (FontStyle)1);
		statusLabel = val3;
		val7.Controls.Add((Control)(object)statusLabel, 0, 5);
		ProgressBar val29 = new ProgressBar();
		((Control)val29).Dock = (DockStyle)5;
		((Control)val29).Height = 22;
		val29.Minimum = 0;
		val29.Maximum = 1000;
		((Control)val29).Margin = new Padding(0, 2, 0, 6);
		bar = val29;
		val7.Controls.Add((Control)(object)bar, 0, 6);
		val7.Controls.Add((Control)(object)MakeLabel("Activity:", bold: false), 0, 7);
		val8 = new TextBox();
		((TextBoxBase)val8).Multiline = true;
		((TextBoxBase)val8).ReadOnly = true;
		val8.ScrollBars = (ScrollBars)2;
		((Control)val8).Dock = (DockStyle)5;
		((Control)val8).Font = new Font("Consolas", 9f);
		((Control)val8).BackColor = Color.White;
		logBox = val8;
		val7.Controls.Add((Control)(object)logBox, 0, 8);
		((Control)this).Controls.Add((Control)(object)val7);
		((Control)this).Controls.Add((Control)(object)val2);
	}

	private void Ui(Action a)
	{
		if (((Control)this).IsDisposed)
		{
			return;
		}
		try
		{
			if (((Control)this).InvokeRequired)
			{
				((Control)this).BeginInvoke((Delegate)a);
			}
			else
			{
				a();
			}
		}
		catch
		{
		}
	}

	private void SetMode(string key)
	{
		if (Modes.All((string[] m) => m[0] != key))
		{
			key = "video";
		}
		mode = key;
		foreach (RoundButton mb in modeButtons)
		{
			mb.Selected = (string)((Control)mb).Tag == key;
		}
		qualityBox.Enabled = key == "video" || key == "both";
		if (downloadBtn != null)
		{
			((Control)downloadBtn).Text = (key == "transcript") ? "2.  GET TRANSCRIPT" : "2.  DOWNLOAD";
		}
	}

	private void Log(string text)
	{
		Ui(delegate
		{
			((TextBoxBase)logBox).AppendText(text + Environment.NewLine);
		});
		try
		{
			Directory.CreateDirectory(Paths.DataDir);
			File.AppendAllText(Paths.LogFile, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "  " + text + Environment.NewLine);
		}
		catch
		{
		}
	}

	private void Status(string text)
	{
		Ui(delegate
		{
			((Control)statusLabel).Text = text;
		});
	}

	private void Progress(double pct)
	{
		Ui(delegate
		{
			bar.Value = Math.Max(0, Math.Min(1000, (int)(pct * 10.0)));
		});
	}

	private void SetBusy(bool busy)
	{
		Ui(delegate
		{
			((Control)downloadBtn).Enabled = !busy;
			((Control)updateBtn).Enabled = !busy;
			((Control)stopBtn).Enabled = busy;
			((Control)downloadBtn).BackColor = ((!busy) ? Orange : Color.Silver);
		});
	}

	private void PasteClipboard()
	{
		try
		{
			string text = Clipboard.GetText();
			if (!string.IsNullOrWhiteSpace(text))
			{
				if (((TextBoxBase)urlBox).TextLength > 0 && !((Control)urlBox).Text.EndsWith("\n"))
				{
					((TextBoxBase)urlBox).AppendText(Environment.NewLine);
				}
				((TextBoxBase)urlBox).AppendText(text.Trim() + Environment.NewLine);
			}
		}
		catch
		{
		}
	}

	private void BrowseFolder()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Invalid comparison between Unknown and I4
		FolderBrowserDialog val = new FolderBrowserDialog();
		try
		{
			val.Description = "Choose where to save your videos";
			if (Directory.Exists(((Control)folderBox).Text))
			{
				val.SelectedPath = ((Control)folderBox).Text;
			}
			if ((int)((CommonDialog)val).ShowDialog((IWin32Window)(object)this) == 1)
			{
				((Control)folderBox).Text = val.SelectedPath;
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private void OpenFolder()
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Directory.CreateDirectory(((Control)folderBox).Text.Trim());
			Process.Start("explorer.exe", "\"" + ((Control)folderBox).Text.Trim() + "\"");
		}
		catch (Exception ex)
		{
			MessageBox.Show((IWin32Window)(object)this, ex.Message, ((Control)this).Text);
		}
	}

	private void LoadSettings()
	{
		string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Videos", "EZ Downloads");
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		try
		{
			if (File.Exists(Paths.Settings))
			{
				string[] array = File.ReadAllLines(Paths.Settings);
				foreach (string text2 in array)
				{
					int num = text2.IndexOf('=');
					if (num > 0)
					{
						dictionary[text2.Substring(0, num)] = text2.Substring(num + 1);
					}
				}
			}
		}
		catch
		{
		}
		((Control)folderBox).Text = ((!dictionary.TryGetValue("folder", out var value) || value.Length <= 0) ? text : value);
		qualityBox.SelectedItem = ((!dictionary.TryGetValue("quality", out value) || !Qualities.Contains(value)) ? Qualities[0] : value);
		if (dictionary.TryGetValue("mode", out value))
		{
			SetMode(value);
		}
		else
		{
			SetMode((dictionary.TryGetValue("audio", out value) && value == "1") ? "audio" : "video");
		}
	}

	private void SaveSettings()
	{
		try
		{
			Directory.CreateDirectory(Paths.DataDir);
			File.WriteAllLines(Paths.Settings, new string[4]
			{
				"folder=" + ((Control)folderBox).Text.Trim(),
				"quality=" + qualityBox.SelectedItem,
				"audio=" + ((mode != "audio") ? "0" : "1"),
				"mode=" + mode
			});
		}
		catch
		{
		}
	}

	private void EnsureEngine(bool forceUpdate)
	{
		bool exists = File.Exists(Paths.Engine) && new FileInfo(Paths.Engine).Length > 1000000;
		bool flag = exists && (DateTime.Now - File.GetLastWriteTime(Paths.Engine)).TotalDays > 7.0;
		if (exists && !forceUpdate && !flag)
		{
			Status("Ready.");
			return;
		}
		SetBusy(busy: true);
		((Control)stopBtn).Enabled = false;
		Thread thread = new Thread((ThreadStart)delegate
		{
			try
			{
				if (!exists)
				{
					InstallEngine();
				}
				else
				{
					UpdateEngine(forceUpdate);
				}
			}
			finally
			{
				SetBusy(busy: false);
				Progress(0.0);
			}
		});
		thread.IsBackground = true;
		thread.Start();
	}

	private void InstallEngine()
	{
		string text = Path.Combine(Paths.BinDir, "yt-dlp.exe");
		if (File.Exists(text) && !File.Exists(Paths.Engine))
		{
			try
			{
				Directory.CreateDirectory(Paths.EngineDir);
				File.Copy(text, Paths.Engine, overwrite: true);
				File.SetLastWriteTime(Paths.Engine, DateTime.Now.AddDays(-8.0));
				Log("Engine ready. You can start downloading.");
				UpdateEngine(announce: false);
				return;
			}
			catch (Exception ex)
			{
				Log("Couldn't copy the built-in engine: " + ex.Message);
			}
		}
		Status("Getting the download engine...");
		Log("Downloading the download engine...");
		try
		{
			Directory.CreateDirectory(Paths.EngineDir);
			string text2 = Paths.Engine + ".part";
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("https://github.com/yt-dlp/yt-dlp/releases/latest/download/yt-dlp.exe");
			httpWebRequest.UserAgent = "EZSocialDownloader/1.0";
			httpWebRequest.AllowAutoRedirect = true;
			httpWebRequest.Timeout = 60000;
			using (HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
			{
				using Stream stream = httpWebResponse.GetResponseStream();
				using FileStream fileStream = File.Create(text2);
				long contentLength = httpWebResponse.ContentLength;
				long num = 0L;
				byte[] array = new byte[81920];
				int num2;
				while ((num2 = stream.Read(array, 0, array.Length)) > 0)
				{
					fileStream.Write(array, 0, num2);
					num += num2;
					if (contentLength > 0)
					{
						Progress((double)num * 100.0 / (double)contentLength);
						Status($"Downloading engine... {(double)num * 100.0 / (double)contentLength:0}%");
					}
				}
			}
			if (new FileInfo(text2).Length < 1000000)
			{
				throw new Exception("Download was incomplete.");
			}
			if (File.Exists(Paths.Engine))
			{
				File.Delete(Paths.Engine);
			}
			File.Move(text2, Paths.Engine);
			Log("Engine ready. You can start downloading.");
			Status("Ready.");
		}
		catch (Exception ex2)
		{
			Log("Could not get the download engine: " + ex2.Message);
			Log("Check your internet connection, then click 'Update engine'.");
			Status("Engine missing — check internet, then click 'Update engine'.");
		}
	}

	private void UpdateEngine(bool announce)
	{
		Status("Checking for engine updates...");
		if (announce)
		{
			Log("Checking for engine updates...");
		}
		try
		{
			List<string> lines = new List<string>();
			int num = RunProcess(Paths.Engine, "-U", delegate(string l)
			{
				lines.Add(l);
			}, delegate(string l)
			{
				lines.Add(l);
			});
			string text = lines.LastOrDefault((string l) => l.Trim().Length > 0) ?? string.Empty;
			if (num == 0)
			{
				try
				{
					File.SetLastWriteTime(Paths.Engine, DateTime.Now);
				}
				catch
				{
				}
				if (announce || text.IndexOf("Updated", StringComparison.OrdinalIgnoreCase) >= 0)
				{
					Log("Engine: " + text.Trim());
				}
			}
			else
			{
				if (announce)
				{
					Log("Engine update failed (" + text.Trim() + "). Re-downloading...");
					InstallEngine();
					return;
				}
				Log("Couldn't check for engine updates right now — using the current engine.");
			}
		}
		catch (Exception ex)
		{
			Log("Engine update failed: " + ex.Message);
		}
		Status("Ready.");
	}

	private static List<string> ParseLinks(string text)
	{
		List<string> list = new List<string>();
		foreach (Match item2 in Regex.Matches(text, "https?://\\S+"))
		{
			string item = item2.Value.TrimEnd(')', ']', '>', '"', '\'', ',');
			if (!list.Contains(item))
			{
				list.Add(item);
			}
		}
		return list;
	}

	private static string PlatformOf(string url)
	{
		string text = url.ToLowerInvariant();
		if (text.Contains("tiktok.com"))
		{
			return "TikTok";
		}
		if (text.Contains("instagram.com"))
		{
			return "Instagram";
		}
		if (text.Contains("facebook.com") || text.Contains("fb.watch"))
		{
			return "Facebook";
		}
		if (text.Contains("youtube.com/shorts"))
		{
			return "YouTube Shorts";
		}
		if (text.Contains("youtube.com") || text.Contains("youtu.be"))
		{
			return "YouTube";
		}
		if (text.Contains("twitter.com") || text.Contains("x.com"))
		{
			return "X";
		}
		return "Link";
	}

	private static string Q(string s)
	{
		return "\"" + s.Replace("\"", "\\\"") + "\"";
	}

	private static string SubLangs(bool everything)
	{
		// English and Spanish first (TikTok uses codes like eng-US / spa-ES);
		// "everything" is the fallback when a video only has other languages.
		return everything ? "all,-live_chat" : "en.*,eng.*,es.*,spa.*";
	}

	private string BuildArgs(string url, string outDir, string quality, string mode, bool useLogin, bool allLangs = false)
	{
		bool audio = mode == "audio";
		bool transcript = mode == "transcript" || mode == "both";
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("--newline --no-colors --no-playlist --progress --windows-filenames ");
		stringBuilder.Append("--retries 5 --fragment-retries 5 --concurrent-fragments 4 ");
		stringBuilder.Append("--ffmpeg-location ").Append(Q(Paths.BinDir)).Append(' ');
		if (File.Exists(Paths.Js))
		{
			stringBuilder.Append("--js-runtimes ").Append(Q("quickjs:" + Paths.Js)).Append(' ');
		}
		string text = "[format_note!*=?watermark]";
		if (transcript)
		{
			stringBuilder.Append("--write-subs ");
			if (!allLangs)
			{
				stringBuilder.Append("--write-auto-subs ");
			}
			stringBuilder.Append("--sub-langs ").Append(Q(SubLangs(allLangs))).Append(' ');
			stringBuilder.Append("--sub-format \"vtt/srt/best\" ");
			stringBuilder.Append("-o ").Append(Q("subtitle:" + Path.Combine(TranscriptDir(outDir), "%(uploader,channel,id)s - %(title).70B [%(id)s].%(ext)s"))).Append(' ');
		}
		if (mode == "transcript")
		{
			stringBuilder.Append("--skip-download ");
		}
		else if (audio)
		{
			stringBuilder.Append("-f \"ba/b\" -x --audio-format mp3 --audio-quality 0 ");
		}
		else
		{
			string text2 = string.Empty;
			switch (quality)
			{
			case "1080p":
				text2 = "[height<=1080]";
				break;
			case "720p":
				text2 = "[height<=720]";
				break;
			case "480p":
				text2 = "[height<=480]";
				break;
			}
			string s = "bv*" + text2 + text + "+ba/b" + text2 + text + "/bv*" + text2 + "+ba/b" + text2 + "/bv*+ba/b";
			stringBuilder.Append("-f ").Append(Q(s)).Append(' ');
			stringBuilder.Append("-S \"res,vcodec:h264,acodec:aac\" --merge-output-format mp4 ");
		}
		if (useLogin && File.Exists(Paths.AllCookies))
		{
			stringBuilder.Append("--cookies ").Append(Q(Paths.AllCookies)).Append(' ');
		}
		string s2 = Path.Combine(outDir, "%(extractor_key)s", "%(uploader,channel,id)s - %(title).70B [%(id)s].%(ext)s");
		stringBuilder.Append("-o ").Append(Q(s2)).Append(' ');
		stringBuilder.Append("--progress-template \"download:PROG|%(progress._percent_str)s|%(progress._speed_str)s|%(progress._eta_str)s\" ");
		if (mode != "transcript")
		{
			stringBuilder.Append("--print \"after_move:SAVED|%(filepath)s\" ");
		}
		stringBuilder.Append("-- ").Append(Q(url));
		return stringBuilder.ToString();
	}

	private void StartDownloads()
	{
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		List<string> links = ParseLinks(((Control)urlBox).Text);
		if (links.Count == 0)
		{
			MessageBox.Show((IWin32Window)(object)this, "Paste at least one link first (it should start with http).", ((Control)this).Text);
			return;
		}
		if (!File.Exists(Paths.Engine))
		{
			MessageBox.Show((IWin32Window)(object)this, "The download engine isn't set up yet. Make sure you're online and click 'Update engine'.", ((Control)this).Text);
			return;
		}
		string outDir = ((Control)folderBox).Text.Trim();
		try
		{
			Directory.CreateDirectory(outDir);
		}
		catch (Exception ex)
		{
			MessageBox.Show((IWin32Window)(object)this, "Can't use that save folder:\n" + ex.Message, ((Control)this).Text);
			return;
		}
		SaveSettings();
		string quality = (string)qualityBox.SelectedItem;
		string mode = this.mode;
		stopRequested = false;
		SetBusy(busy: true);
		worker = new Thread((ThreadStart)delegate
		{
			RunAll(links, outDir, quality, mode);
		});
		worker.IsBackground = true;
		worker.Start();
	}

	private static bool IsLoginNeeded(string m)
	{
		m = m.ToLowerInvariant();
		return m.Contains("log in") || m.Contains("login") || m.Contains("sign in") || m.Contains("--cookies") || (m.Contains("age") && m.Contains("restrict")) || m.Contains("confirm your age") || m.Contains("private");
	}

	private void ShowHelp()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		MessageBox.Show((IWin32Window)(object)this, "HOW TO USE\n1. Copy a video link (Share > Copy link) and click Paste.\n2. Pick what you want: Video, Audio (MP3), Transcript / lyrics, or Video + transcript.\n3. Click DOWNLOAD. Files go to Videos\\EZ Downloads.\n\nTRANSCRIPTS / LYRICS\nThe words come from the video's captions, saved as a plain .txt file (and a timed .vtt/.srt file) in the Transcripts folder.\nIf a video has no captions (the creator didn't add them and the site didn't make auto-captions), there is no transcript to get.\n\nLOGINS (for posts that ask you to log in)\nClick 'Log in' under a site, log in normally in the window that opens, then click DONE.\nA green check shows the site is logged in. You only do this once per site.\nIf a site starts asking again later, click 'Log out' then 'Log in' again.\n\nIF A DOWNLOAD FAILS\nClick 'Update engine' and try again. Sites change often.", "Help", (MessageBoxButtons)0, (MessageBoxIcon)64);
	}

	private void RefreshLogins()
	{
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Expected O, but got Unknown
		SiteInfo[] all = SiteInfo.All;
		foreach (SiteInfo siteInfo in all)
		{
			bool flag = siteInfo.IsLoggedIn();
			((Control)siteStatus[siteInfo.Key]).Text = ((!flag) ? "Not logged in" : "✔ Logged in");
			((Control)siteStatus[siteInfo.Key]).ForeColor = ((!flag) ? Color.DimGray : Color.FromArgb(20, 140, 60));
			((Control)siteStatus[siteInfo.Key]).Font = new Font("Segoe UI", 9f, (FontStyle)(flag ? 1 : 0));
			((Control)siteButton[siteInfo.Key]).Text = ((!flag) ? "Log in" : "Log out");
			((Control)siteButton[siteInfo.Key]).BackColor = ((!flag) ? Blue : Color.Gray);
		}
	}

	private void LoginClicked(SiteInfo site)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Invalid comparison between Unknown and I4
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		if (!((Control)downloadBtn).Enabled)
		{
			MessageBox.Show((IWin32Window)(object)this, "Please wait until the current download finishes.", ((Control)this).Text);
			return;
		}
		bool flag = site.IsLoggedIn();
		if (!flag || (int)MessageBox.Show((IWin32Window)(object)this, "Log out of " + site.Name + " in EZ Social Downloader?", ((Control)this).Text, (MessageBoxButtons)4) == 6)
		{
			LoginForm loginForm = new LoginForm(site, flag);
			try
			{
				((Form)loginForm).ShowDialog((IWin32Window)(object)this);
			}
			finally
			{
				((IDisposable)loginForm)?.Dispose();
			}
			if (flag)
			{
				site.ForgetLogin();
			}
			RefreshLogins();
			Log(site.IsLoggedIn() ? ("Logged in to " + site.Name + ".") : ((!flag) ? ("Not logged in to " + site.Name + ".") : ("Logged out of " + site.Name + ".")));
		}
	}

	private static bool IsCookieError(string m)
	{
		m = m.ToLowerInvariant();
		return m.Contains("cookie database") || m.Contains("could not copy") || m.Contains("decrypt") || m.Contains("dpapi") || (m.Contains("could not find") && m.Contains("cookies"));
	}

	private int Attempt(string url, string outDir, string quality, string mode, bool useLogin, int i, int count, string name, List<string> errors, ref string saved, bool allLangs = false)
	{
		string s = null;
		int result;
		try
		{
			result = RunProcess(Paths.Engine, BuildArgs(url, outDir, quality, mode, useLogin, allLangs), delegate(string line)
			{
				if (line.StartsWith("PROG|"))
				{
					string[] array = line.Split(new char[1] { '|' });
					string s2 = ((array.Length <= 1) ? string.Empty : array[1].Trim().TrimEnd(new char[1] { '%' }));
					if (double.TryParse(s2, NumberStyles.Float, CultureInfo.InvariantCulture, out var result2))
					{
						Progress(result2);
						Status($"Downloading {i + 1} of {count} ({name})...  {result2:0.0}%   {((array.Length <= 2) ? string.Empty : array[2].Trim())}   left {((array.Length <= 3) ? string.Empty : array[3].Trim())}");
					}
				}
				else if (line.StartsWith("SAVED|"))
				{
					s = line.Substring(6).Trim();
				}
			}, delegate(string line)
			{
				if (line.StartsWith("ERROR"))
				{
					lock (errors)
					{
						errors.Add(line.Replace("ERROR: ", string.Empty));
					}
				}
			});
		}
		catch (Exception ex)
		{
			errors.Add(ex.Message);
			result = -1;
		}
		if (s != null)
		{
			saved = s;
		}
		return result;
	}

	private static string TranscriptDir(string outDir)
	{
		return Path.Combine(outDir, "Transcripts");
	}

	private static Dictionary<string, DateTime> Snapshot(string dir)
	{
		Dictionary<string, DateTime> d = new Dictionary<string, DateTime>(StringComparer.OrdinalIgnoreCase);
		try
		{
			if (Directory.Exists(dir))
			{
				foreach (string f in Directory.GetFiles(dir))
				{
					d[f] = File.GetLastWriteTimeUtc(f);
				}
			}
		}
		catch
		{
		}
		return d;
	}

	/// <summary>Caption files that appeared (or changed) since the snapshot -> plain .txt transcripts.</summary>
	private List<string> MakeTranscripts(string dir, Dictionary<string, DateTime> before)
	{
		List<string> made = new List<string>();
		foreach (KeyValuePair<string, DateTime> f in Snapshot(dir))
		{
			if (!Transcript.IsCaptionFile(f.Key) || (before.TryGetValue(f.Key, out var t) && t == f.Value))
			{
				continue;
			}
			try
			{
				string txt = Transcript.WriteText(f.Key);
				if (txt != null)
				{
					made.Add(txt);
				}
			}
			catch (Exception ex)
			{
				Log("   Couldn't read captions " + Path.GetFileName(f.Key) + ": " + ex.Message);
			}
		}
		return made;
	}

	private void RunAll(List<string> links, string outDir, string quality, string mode)
	{
		int num = 0;
		int num2 = 0;
		SiteInfo.MergeCookies();
		for (int i = 0; i < links.Count; i++)
		{
			if (stopRequested)
			{
				break;
			}
			string text = links[i];
			string text2 = PlatformOf(text);
			Log($"[{i + 1}/{links.Count}] {text2}: {text}");
			Status($"Downloading {i + 1} of {links.Count} ({text2})...");
			Progress(0.0);
			string saved = null;
			List<string> list = new List<string>();
			SiteInfo siteInfo = SiteInfo.ForName(text2);
			bool flag = siteInfo?.IsLoggedIn() ?? false;
			bool flag2 = flag && (text2 == "Instagram" || text2 == "Facebook" || text2 == "X");
			bool wantText = mode == "transcript" || mode == "both";
			string tdir = TranscriptDir(outDir);
			Dictionary<string, DateTime> before = Snapshot(tdir);
			if (mode == "transcript")
			{
				Status($"Getting transcript {i + 1} of {links.Count} ({text2})...");
			}
			int num3 = Attempt(text, outDir, quality, mode, flag2, i, links.Count, text2, list, ref saved);
			if (!stopRequested && num3 != 0 && !flag2 && flag && list.Any(IsLoginNeeded))
			{
				Log("   This post needs a login. Using your " + siteInfo.Name + " login...");
				list.Clear();
				num3 = Attempt(text, outDir, quality, mode, useLogin: true, i, links.Count, text2, list, ref saved);
			}
			List<string> texts = new List<string>();
			if (wantText && !stopRequested && num3 == 0)
			{
				texts = MakeTranscripts(tdir, before);
				if (texts.Count == 0)
				{
					// nothing in English / Spanish - take whatever language the video has
					List<string> list2 = new List<string>();
					string saved2 = null;
					Attempt(text, outDir, quality, "transcript", flag2 || (flag && list.Any(IsLoginNeeded)), i, links.Count, text2, list2, ref saved2, allLangs: true);
					texts = MakeTranscripts(tdir, before);
				}
				foreach (string t in texts)
				{
					Log("   Transcript: " + Path.GetFileName(t));
				}
				if (texts.Count == 0)
				{
					Log("   No transcript for this one: the video has no captions. (TikTok only has them when the creator turned on captions or TikTok made auto-captions.)");
				}
			}
			if (stopRequested)
			{
				Log("   Stopped.");
				break;
			}
			if (num3 == 0 && list.Count == 0 && mode == "transcript")
			{
				if (texts.Count > 0)
				{
					num++;
					Progress(100.0);
				}
				else
				{
					num2++;
				}
				continue;
			}
			if (num3 == 0 && list.Count == 0)
			{
				num++;
				Progress(100.0);
				Log("   Saved: " + ((saved == null) ? "done" : Path.GetFileName(saved)));
				continue;
			}
			num2++;
			string text3 = ((list.Count <= 0) ? ("unknown error (code " + num3 + ")") : list[list.Count - 1]);
			if (text3.Length > 400)
			{
				text3 = text3.Substring(0, 400) + "...";
			}
			Log("   FAILED: " + text3);
			string text4 = text3.ToLowerInvariant();
			if (IsLoginNeeded(text4) && !flag)
			{
				Log("   Tip: this post needs a login. Click 'Log in' under " + ((siteInfo == null) ? text2 : siteInfo.Name) + ", log in, then download again.");
			}
			else if (IsLoginNeeded(text4))
			{
				Log("   Tip: your " + siteInfo.Name + " login may have expired. Click 'Log out', then 'Log in' again.");
			}
			else if (text4.Contains("unsupported url"))
			{
				Log("   Tip: that link type isn't supported. Copy the link from the video's Share button.");
			}
			else
			{
				Log("   Tip: click 'Update engine' — sites change often and updates fix most failures.");
			}
		}
		Progress(0.0);
		string text5 = ((!stopRequested) ? $"Finished: {num} saved, {num2} failed." : $"Stopped. {num} saved, {num2} failed.");
		if ((mode == "transcript" || mode == "both") && Directory.Exists(TranscriptDir(outDir)))
		{
			Log("Transcripts are in: " + TranscriptDir(outDir));
		}
		Status(text5);
		Log("--- " + text5 + " ---");
		SetBusy(busy: false);
	}

	private void StopDownloads()
	{
		stopRequested = true;
		Status("Stopping...");
		KillCurrent();
	}

	private void KillCurrent()
	{
		lock (procLock)
		{
			if (current == null)
			{
				return;
			}
			try
			{
				ProcessStartInfo processStartInfo = new ProcessStartInfo("taskkill", "/T /F /PID " + current.Id);
				processStartInfo.CreateNoWindow = true;
				processStartInfo.UseShellExecute = false;
				ProcessStartInfo startInfo = processStartInfo;
				Process.Start(startInfo).WaitForExit(5000);
			}
			catch
			{
			}
		}
	}

	private int RunProcess(string exe, string args, Action<string> onOut, Action<string> onErr)
	{
		ProcessStartInfo processStartInfo = new ProcessStartInfo(exe, args);
		processStartInfo.UseShellExecute = false;
		processStartInfo.CreateNoWindow = true;
		processStartInfo.RedirectStandardOutput = true;
		processStartInfo.RedirectStandardError = true;
		processStartInfo.StandardOutputEncoding = Encoding.UTF8;
		processStartInfo.StandardErrorEncoding = Encoding.UTF8;
		processStartInfo.WorkingDirectory = Paths.DataDir;
		ProcessStartInfo processStartInfo2 = processStartInfo;
		processStartInfo2.EnvironmentVariables["PYTHONUTF8"] = "1";
		processStartInfo2.EnvironmentVariables["PYTHONIOENCODING"] = "utf-8";
		Directory.CreateDirectory(Paths.DataDir);
		Process process = new Process();
		process.StartInfo = processStartInfo2;
		using Process process2 = process;
		process2.OutputDataReceived += delegate(object s, DataReceivedEventArgs e)
		{
			if (e.Data != null)
			{
				onOut(e.Data);
			}
		};
		process2.ErrorDataReceived += delegate(object s, DataReceivedEventArgs e)
		{
			if (e.Data != null)
			{
				onErr(e.Data);
			}
		};
		process2.Start();
		lock (procLock)
		{
			current = process2;
		}
		process2.BeginOutputReadLine();
		process2.BeginErrorReadLine();
		process2.WaitForExit();
		lock (procLock)
		{
			current = null;
		}
		return process2.ExitCode;
	}
}
