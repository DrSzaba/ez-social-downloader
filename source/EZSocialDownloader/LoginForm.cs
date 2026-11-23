using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;

namespace EZSocialDownloader;

internal class LoginForm : Form
{
	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	private struct _003CInit_003Ec__async0 : IAsyncStateMachine
	{
		internal CoreWebView2Environment _003Cenv_003E__1;

		internal string[] _0024locvar0;

		internal int _0024locvar1;

		internal string _003Cu_003E__2;

		internal List<CoreWebView2Cookie> _003Clist_003E__3;

		internal List<CoreWebView2Cookie>.Enumerator _0024locvar2;

		internal LoginForm _0024this;

		internal AsyncTaskMethodBuilder _0024builder;

		internal int _0024PC;

		private TaskAwaiter<CoreWebView2Environment> _0024awaiter0;

		private TaskAwaiter _0024awaiter1;

		private TaskAwaiter<List<CoreWebView2Cookie>> _0024awaiter2;

		public void MoveNext()
		{
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			uint num = (uint)_0024PC;
			_0024PC = -1;
			bool flag = false;
			try
			{
				switch (num)
				{
				default:
					return;
				case 0u:
					num = 4294967293u;
					goto case 1u;
				case 1u:
				case 2u:
					try
					{
						switch (num)
						{
						default:
							Directory.CreateDirectory(Paths.BrowserDir);
							_0024awaiter0 = CoreWebView2Environment.CreateAsync(null, Paths.BrowserDir).GetAwaiter();
							if (!_0024awaiter0.IsCompleted)
							{
								_0024PC = 1;
								flag = true;
								_0024builder.AwaitUnsafeOnCompleted(ref _0024awaiter0, ref this);
								return;
							}
							goto case 1u;
						case 1u:
							_003Cenv_003E__1 = _0024awaiter0.GetResult();
							_0024awaiter1 = _0024this.web.EnsureCoreWebView2Async(_003Cenv_003E__1).GetAwaiter();
							if (!_0024awaiter1.IsCompleted)
							{
								_0024PC = 2;
								flag = true;
								_0024builder.AwaitUnsafeOnCompleted(ref _0024awaiter1, ref this);
								return;
							}
							break;
						case 2u:
							break;
						}
						_0024awaiter1.GetResult();
						_0024this.ready = true;
					}
					catch (Exception ex)
					{
						_0024this.finished = true;
						MessageBox.Show((IWin32Window)(object)_0024this, "The login window needs Microsoft Edge WebView2, which comes with Windows 11.\n\nIf you're on Windows 10, install \"WebView2 Runtime\" from Microsoft (free), then try again.\n\n(" + ex.Message + ")", ((Control)_0024this).Text, (MessageBoxButtons)0, (MessageBoxIcon)48);
						((Form)_0024this).Close();
						break;
					}
					if (_0024this.logoutMode)
					{
						num = 4294967293u;
						goto case 3u;
					}
					_0024this.web.CoreWebView2.Navigate(_0024this.site.LoginUrl);
					_0024this.poll.Start();
					break;
				case 3u:
					try
					{
						switch (num)
						{
						default:
							_0024locvar0 = _0024this.site.CookieUrls;
							_0024locvar1 = 0;
							goto IL_028c;
						case 3u:
							{
								_003Clist_003E__3 = _0024awaiter2.GetResult();
								_0024locvar2 = _003Clist_003E__3.GetEnumerator();
								try
								{
									while (_0024locvar2.MoveNext())
									{
										CoreWebView2Cookie current = _0024locvar2.Current;
										_0024this.web.CoreWebView2.CookieManager.DeleteCookie(current);
									}
								}
								finally
								{
									((IDisposable)_0024locvar2).Dispose();
								}
								_0024locvar1++;
								goto IL_028c;
							}
							IL_028c:
							if (_0024locvar1 < _0024locvar0.Length)
							{
								_003Cu_003E__2 = _0024locvar0[_0024locvar1];
								_0024awaiter2 = _0024this.web.CoreWebView2.CookieManager.GetCookiesAsync(_003Cu_003E__2).GetAwaiter();
								if (!_0024awaiter2.IsCompleted)
								{
									_0024PC = 3;
									flag = true;
									_0024builder.AwaitUnsafeOnCompleted(ref _0024awaiter2, ref this);
									return;
								}
								goto case 3u;
							}
							break;
						}
					}
					catch
					{
					}
					_0024this.finished = true;
					((Form)_0024this).Close();
					break;
				}
			}
			catch (Exception exception)
			{
				_0024PC = -1;
				_0024builder.SetException(exception);
				return;
			}
			_0024PC = -1;
			_0024builder.SetResult();
		}

		[DebuggerHidden]
		public void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			_0024builder.SetStateMachine(stateMachine);
		}
	}

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	private struct _003CGetAll_003Ec__async1 : IAsyncStateMachine
	{
		internal List<CoreWebView2Cookie> _003Call_003E__0;

		internal HashSet<string> _003Cseen_003E__0;

		internal string[] _0024locvar0;

		internal int _0024locvar1;

		internal string _003Cu_003E__1;

		internal List<CoreWebView2Cookie> _003Clist_003E__2;

		internal List<CoreWebView2Cookie>.Enumerator _0024locvar2;

		internal LoginForm _0024this;

		internal AsyncTaskMethodBuilder<List<CoreWebView2Cookie>> _0024builder;

		internal int _0024PC;

		private TaskAwaiter<List<CoreWebView2Cookie>> _0024awaiter0;

		public void MoveNext()
		{
			uint num = (uint)_0024PC;
			_0024PC = -1;
			List<CoreWebView2Cookie> result;
			try
			{
				switch (num)
				{
				default:
					return;
				case 0u:
					_003Call_003E__0 = new List<CoreWebView2Cookie>();
					_003Cseen_003E__0 = new HashSet<string>();
					_0024locvar0 = _0024this.site.CookieUrls;
					_0024locvar1 = 0;
					goto IL_017f;
				case 1u:
					{
						_003Clist_003E__2 = _0024awaiter0.GetResult();
						_0024locvar2 = _003Clist_003E__2.GetEnumerator();
						try
						{
							while (_0024locvar2.MoveNext())
							{
								CoreWebView2Cookie current = _0024locvar2.Current;
								if (_003Cseen_003E__0.Add(current.Domain + "|" + current.Path + "|" + current.Name))
								{
									_003Call_003E__0.Add(current);
								}
							}
						}
						finally
						{
							((IDisposable)_0024locvar2).Dispose();
						}
						_0024locvar1++;
						goto IL_017f;
					}
					IL_017f:
					if (_0024locvar1 < _0024locvar0.Length)
					{
						_003Cu_003E__1 = _0024locvar0[_0024locvar1];
						_0024awaiter0 = _0024this.web.CoreWebView2.CookieManager.GetCookiesAsync(_003Cu_003E__1).GetAwaiter();
						if (!_0024awaiter0.IsCompleted)
						{
							_0024PC = 1;
							_0024builder.AwaitUnsafeOnCompleted(ref _0024awaiter0, ref this);
							return;
						}
						goto case 1u;
					}
					result = _003Call_003E__0;
					break;
				}
			}
			catch (Exception exception)
			{
				_0024PC = -1;
				_0024builder.SetException(exception);
				return;
			}
			_0024PC = -1;
			_0024builder.SetResult(result);
		}

		[DebuggerHidden]
		public void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			_0024builder.SetStateMachine(stateMachine);
		}
	}

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	private struct _003CCheckLoggedIn_003Ec__async2 : IAsyncStateMachine
	{
		internal List<CoreWebView2Cookie> _003Call_003E__1;

		internal LoginForm _0024this;

		internal AsyncTaskMethodBuilder _0024builder;

		internal int _0024PC;

		private TaskAwaiter<List<CoreWebView2Cookie>> _0024awaiter0;

		public void MoveNext()
		{
			uint num = (uint)_0024PC;
			_0024PC = -1;
			bool flag = false;
			try
			{
				switch (num)
				{
				default:
					return;
				case 0u:
					if (_0024this.ready && !_0024this.finished)
					{
						num = 4294967293u;
						break;
					}
					goto end_IL_0010;
				case 1u:
					break;
				}
				try
				{
					switch (num)
					{
					default:
						_0024awaiter0 = _0024this.GetAll().GetAwaiter();
						if (!_0024awaiter0.IsCompleted)
						{
							_0024PC = 1;
							flag = true;
							_0024builder.AwaitUnsafeOnCompleted(ref _0024awaiter0, ref this);
							return;
						}
						break;
					case 1u:
						break;
					}
					_003Call_003E__1 = _0024awaiter0.GetResult();
					if (SiteInfo.HasSession(_003Call_003E__1, _0024this.site))
					{
						((Control)_0024this.info).Text = "✔  You're logged in to " + _0024this.site.Name + "!  Click DONE.";
						((Control)_0024this.info).BackColor = Color.FromArgb(20, 140, 60);
						((Control)_0024this.doneBtn).BackColor = Color.FromArgb(20, 140, 60);
					}
				}
				catch
				{
				}
				end_IL_0010:;
			}
			catch (Exception exception)
			{
				_0024PC = -1;
				_0024builder.SetException(exception);
				return;
			}
			_0024PC = -1;
			_0024builder.SetResult();
		}

		[DebuggerHidden]
		public void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			_0024builder.SetStateMachine(stateMachine);
		}
	}

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	private struct _003COnClosingAsync_003Ec__async3 : IAsyncStateMachine
	{
		internal FormClosingEventArgs e;

		internal List<CoreWebView2Cookie> _003Call_003E__1;

		internal LoginForm _0024this;

		internal AsyncVoidMethodBuilder _0024builder;

		internal int _0024PC;

		private TaskAwaiter<List<CoreWebView2Cookie>> _0024awaiter0;

		public void MoveNext()
		{
			uint num = (uint)_0024PC;
			_0024PC = -1;
			bool flag = false;
			try
			{
				switch (num)
				{
				default:
					return;
				case 0u:
					if (_0024this.finished || !_0024this.ready)
					{
						return;
					}
					((CancelEventArgs)(object)e).Cancel = true;
					_0024this.poll.Stop();
					_0024this.finished = true;
					num = 4294967293u;
					break;
				case 1u:
					break;
				}
				try
				{
					switch (num)
					{
					default:
						_0024awaiter0 = _0024this.GetAll().GetAwaiter();
						if (!_0024awaiter0.IsCompleted)
						{
							_0024PC = 1;
							flag = true;
							_0024builder.AwaitUnsafeOnCompleted(ref _0024awaiter0, ref this);
							return;
						}
						break;
					case 1u:
						break;
					}
					_003Call_003E__1 = _0024awaiter0.GetResult();
					if (SiteInfo.HasSession(_003Call_003E__1, _0024this.site))
					{
						SiteInfo.WriteNetscape(_0024this.site.CookieFile, _003Call_003E__1);
					}
				}
				catch
				{
				}
				((Form)_0024this).Close();
			}
			catch (Exception exception)
			{
				_0024PC = -1;
				_0024builder.SetException(exception);
				return;
			}
			_0024PC = -1;
			_0024builder.SetResult();
		}

		[DebuggerHidden]
		public void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			_0024builder.SetStateMachine(stateMachine);
		}
	}

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	private struct _003CLoginForm_003Ec__async4 : IAsyncStateMachine
	{
		internal LoginForm _0024this;

		internal AsyncVoidMethodBuilder _0024builder;

		internal int _0024PC;

		private TaskAwaiter _0024awaiter0;

		public void MoveNext()
		{
			uint num = (uint)_0024PC;
			_0024PC = -1;
			try
			{
				switch (num)
				{
				default:
					return;
				case 0u:
					_0024awaiter0 = _0024this.CheckLoggedIn().GetAwaiter();
					if (!_0024awaiter0.IsCompleted)
					{
						_0024PC = 1;
						_0024builder.AwaitUnsafeOnCompleted(ref _0024awaiter0, ref this);
						return;
					}
					break;
				case 1u:
					break;
				}
				_0024awaiter0.GetResult();
			}
			catch (Exception exception)
			{
				_0024PC = -1;
				_0024builder.SetException(exception);
				return;
			}
			_0024PC = -1;
			_0024builder.SetResult();
		}

		[DebuggerHidden]
		public void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			_0024builder.SetStateMachine(stateMachine);
		}
	}

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	private struct _003CLoginForm_003Ec__async5 : IAsyncStateMachine
	{
		internal LoginForm _0024this;

		internal AsyncVoidMethodBuilder _0024builder;

		internal int _0024PC;

		private TaskAwaiter _0024awaiter0;

		public void MoveNext()
		{
			uint num = (uint)_0024PC;
			_0024PC = -1;
			try
			{
				switch (num)
				{
				default:
					return;
				case 0u:
					_0024awaiter0 = _0024this.Init().GetAwaiter();
					if (!_0024awaiter0.IsCompleted)
					{
						_0024PC = 1;
						_0024builder.AwaitUnsafeOnCompleted(ref _0024awaiter0, ref this);
						return;
					}
					break;
				case 1u:
					break;
				}
				_0024awaiter0.GetResult();
			}
			catch (Exception exception)
			{
				_0024PC = -1;
				_0024builder.SetException(exception);
				return;
			}
			_0024PC = -1;
			_0024builder.SetResult();
		}

		[DebuggerHidden]
		public void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			_0024builder.SetStateMachine(stateMachine);
		}
	}

	private readonly SiteInfo site;

	private readonly bool logoutMode;

	private readonly WebView2 web;

	private readonly Label info;

	private readonly Button doneBtn;

	private readonly Timer poll;

	private bool finished;

	private bool ready;

	public LoginForm(SiteInfo site, bool logout)
	{
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Expected O, but got Unknown
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Expected O, but got Unknown
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Expected O, but got Unknown
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Expected O, but got Unknown
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Expected O, but got Unknown
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Expected O, but got Unknown
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Expected O, but got Unknown
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Expected O, but got Unknown
		this.site = site;
		logoutMode = logout;
		((Control)this).Text = ((!logout) ? "Log in to " : "Logging out of ") + site.Name + " - EZ Social Downloader";
		((Control)this).Font = new Font("Segoe UI", 10f);
		((Form)this).ClientSize = new Size(1000, 780);
		((Form)this).StartPosition = (FormStartPosition)4;
		try
		{
			((Form)this).Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
		}
		catch
		{
		}
		Panel val = new Panel();
		((Control)val).Dock = (DockStyle)1;
		((Control)val).Height = 60;
		((Control)val).BackColor = Color.FromArgb(22, 45, 80);
		Panel val2 = val;
		Label val3 = new Label();
		((Control)val3).Text = ((!logout) ? ("Log in to " + site.Name + " below. When you're logged in, click DONE.") : "Logging out...");
		((Control)val3).ForeColor = Color.White;
		((Control)val3).Font = new Font("Segoe UI", 11f, (FontStyle)1);
		((Control)val3).AutoSize = false;
		((Control)val3).Dock = (DockStyle)5;
		val3.TextAlign = (ContentAlignment)16;
		((Control)val3).Padding = new Padding(14, 0, 0, 0);
		info = val3;
		Button val4 = new Button();
		((Control)val4).Text = "DONE";
		((Control)val4).Dock = (DockStyle)4;
		((Control)val4).Width = 150;
		((ButtonBase)val4).FlatStyle = (FlatStyle)0;
		((Control)val4).BackColor = Color.FromArgb(247, 147, 30);
		((Control)val4).ForeColor = Color.White;
		((Control)val4).Font = new Font("Segoe UI", 13f, (FontStyle)1);
		doneBtn = val4;
		((ButtonBase)doneBtn).FlatAppearance.BorderSize = 0;
		((Control)doneBtn).Click += delegate
		{
			((Form)this).Close();
		};
		((Control)val2).Controls.Add((Control)(object)info);
		((Control)val2).Controls.Add((Control)(object)doneBtn);
		WebView2 webView = new WebView2();
		((Control)webView).Dock = (DockStyle)5;
		web = webView;
		((Control)this).Controls.Add((Control)(object)web);
		((Control)this).Controls.Add((Control)(object)val2);
		Timer val5 = new Timer();
		val5.Interval = 2000;
		poll = val5;
		poll.Tick += [AsyncStateMachine(typeof(_003CLoginForm_003Ec__async4))] (object P_0, EventArgs P_1) =>
		{
			_003CLoginForm_003Ec__async4 stateMachine2 = default(_003CLoginForm_003Ec__async4);
			stateMachine2._0024this = this;
			stateMachine2._0024builder = AsyncVoidMethodBuilder.Create();
			stateMachine2._0024builder.Start(ref stateMachine2);
		};
		((Form)this).Load += [AsyncStateMachine(typeof(_003CLoginForm_003Ec__async5))] (object P_0, EventArgs P_1) =>
		{
			_003CLoginForm_003Ec__async5 stateMachine = default(_003CLoginForm_003Ec__async5);
			stateMachine._0024this = this;
			stateMachine._0024builder = AsyncVoidMethodBuilder.Create();
			stateMachine._0024builder.Start(ref stateMachine);
		};
		((Form)this).FormClosing += new FormClosingEventHandler(OnClosingAsync);
	}

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(_003CInit_003Ec__async0))]
	private Task Init()
	{
		_003CInit_003Ec__async0 stateMachine = default(_003CInit_003Ec__async0);
		stateMachine._0024this = this;
		stateMachine._0024builder = AsyncTaskMethodBuilder.Create();
		ref AsyncTaskMethodBuilder _0024builder = ref stateMachine._0024builder;
		_0024builder.Start(ref stateMachine);
		return _0024builder.Task;
	}

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(_003CGetAll_003Ec__async1))]
	private Task<List<CoreWebView2Cookie>> GetAll()
	{
		_003CGetAll_003Ec__async1 stateMachine = default(_003CGetAll_003Ec__async1);
		stateMachine._0024this = this;
		stateMachine._0024builder = AsyncTaskMethodBuilder<List<CoreWebView2Cookie>>.Create();
		ref AsyncTaskMethodBuilder<List<CoreWebView2Cookie>> _0024builder = ref stateMachine._0024builder;
		_0024builder.Start(ref stateMachine);
		return _0024builder.Task;
	}

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(_003CCheckLoggedIn_003Ec__async2))]
	private Task CheckLoggedIn()
	{
		_003CCheckLoggedIn_003Ec__async2 stateMachine = default(_003CCheckLoggedIn_003Ec__async2);
		stateMachine._0024this = this;
		stateMachine._0024builder = AsyncTaskMethodBuilder.Create();
		ref AsyncTaskMethodBuilder _0024builder = ref stateMachine._0024builder;
		_0024builder.Start(ref stateMachine);
		return _0024builder.Task;
	}

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(_003COnClosingAsync_003Ec__async3))]
	private void OnClosingAsync(object sender, FormClosingEventArgs e)
	{
		_003COnClosingAsync_003Ec__async3 stateMachine = default(_003COnClosingAsync_003Ec__async3);
		stateMachine.e = e;
		stateMachine._0024this = this;
		stateMachine._0024builder = AsyncVoidMethodBuilder.Create();
		stateMachine._0024builder.Start(ref stateMachine);
	}
}
