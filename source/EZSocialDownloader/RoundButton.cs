using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace EZSocialDownloader;

/// <summary>A smooth, pill-shaped button with hover / press shading.
/// Set Toggle = true and Selected to use it as one of a group of choices.</summary>
internal class RoundButton : Button
{
	private bool hover;
	private bool down;
	private bool selected;

	public bool Toggle { get; set; }

	/// <summary>Face colour when a toggle button is not selected.</summary>
	public Color OffBack { get; set; } = Color.White;

	public Color OffFore { get; set; } = Color.FromArgb(22, 45, 80);

	public Color OffBorder { get; set; } = Color.FromArgb(200, 208, 222);

	public bool Selected
	{
		get { return selected; }
		set { selected = value; Invalidate(); }
	}

	public RoundButton()
	{
		SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
		         ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw |
		         ControlStyles.SupportsTransparentBackColor, true);
		FlatStyle = FlatStyle.Flat;
		FlatAppearance.BorderSize = 0;
		Cursor = Cursors.Hand;
	}

	private static GraphicsPath Pill(RectangleF r)
	{
		float d = Math.Min(r.Height, r.Width);
		GraphicsPath p = new GraphicsPath();
		p.AddArc(r.X, r.Y, d, d, 90f, 180f);
		p.AddArc(r.Right - d, r.Y, d, d, 270f, 180f);
		p.CloseFigure();
		return p;
	}

	private static Color Shade(Color c, float f)
	{
		// f < 0 darker, f > 0 lighter
		if (f < 0f)
		{
			f = 1f + f;
			return Color.FromArgb(c.A, (int)(c.R * f), (int)(c.G * f), (int)(c.B * f));
		}
		return Color.FromArgb(c.A, c.R + (int)((255 - c.R) * f), c.G + (int)((255 - c.G) * f), c.B + (int)((255 - c.B) * f));
	}

	protected override void OnMouseEnter(EventArgs e) { hover = true; Invalidate(); base.OnMouseEnter(e); }

	protected override void OnMouseLeave(EventArgs e) { hover = false; down = false; Invalidate(); base.OnMouseLeave(e); }

	protected override void OnMouseDown(MouseEventArgs e) { if (e.Button == MouseButtons.Left) { down = true; Invalidate(); } base.OnMouseDown(e); }

	protected override void OnMouseUp(MouseEventArgs e) { down = false; Invalidate(); base.OnMouseUp(e); }

	protected override void OnEnabledChanged(EventArgs e) { Invalidate(); base.OnEnabledChanged(e); }

	protected override void OnTextChanged(EventArgs e) { Invalidate(); base.OnTextChanged(e); }

	protected override void OnPaint(PaintEventArgs e)
	{
		Graphics g = e.Graphics;
		Color outside = Parent != null ? Parent.BackColor : SystemColors.Control;
		g.Clear(outside);
		g.SmoothingMode = SmoothingMode.AntiAlias;
		g.PixelOffsetMode = PixelOffsetMode.HighQuality;

		bool off = Toggle && !selected;
		Color face = off ? OffBack : BackColor;
		Color fore = off ? OffFore : ForeColor;
		if (!Enabled)
		{
			face = Color.FromArgb(205, 210, 218);
			fore = Color.FromArgb(120, 128, 140);
		}
		else if (down)
		{
			face = Shade(face, -0.15f);
		}
		else if (hover)
		{
			face = off ? Color.FromArgb(234, 240, 251) : Shade(face, 0.12f);
		}

		RectangleF r = new RectangleF(1f, 1f, Width - 3f, Height - 3f);
		if (Enabled && !off)
		{
			// soft shadow under coloured buttons
			using GraphicsPath sh = Pill(new RectangleF(r.X, r.Y + 2f, r.Width, r.Height));
			using SolidBrush sb = new SolidBrush(Color.FromArgb(45, 0, 0, 0));
			g.FillPath(sb, sh);
		}
		using (GraphicsPath p = Pill(r))
		{
			using (SolidBrush b = new SolidBrush(face))
			{
				g.FillPath(b, p);
			}
			if (Enabled && !off)
			{
				// glossy top half
				RectangleF top = new RectangleF(r.X, r.Y, r.Width, r.Height / 2f);
				using LinearGradientBrush gl = new LinearGradientBrush(new RectangleF(top.X, top.Y - 1f, top.Width, top.Height + 2f), Color.FromArgb(70, 255, 255, 255), Color.FromArgb(0, 255, 255, 255), LinearGradientMode.Vertical);
				g.SetClip(p);
				g.FillRectangle(gl, top);
				g.ResetClip();
			}
			if (off || !Enabled)
			{
				using Pen pen = new Pen(off && Enabled ? OffBorder : Color.FromArgb(185, 192, 204), 1.4f);
				g.DrawPath(pen, p);
			}
			else if (Toggle && selected)
			{
				using Pen pen = new Pen(Shade(face, -0.25f), 1.6f);
				g.DrawPath(pen, p);
			}
		}

		Rectangle tr = new Rectangle(Padding.Left + 6, Padding.Top, Width - Padding.Horizontal - 12, Height - Padding.Vertical - (down ? 0 : 1));
		if (down)
		{
			tr.Offset(0, 1);
		}
		TextRenderer.DrawText(g, Text, Font, tr, fore, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak | TextFormatFlags.NoPadding);
	}
}
