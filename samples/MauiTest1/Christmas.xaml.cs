using System;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

namespace MauiTest1;

public partial class Christmas : ContentPage
{
	SKPoint? touchLocation;
	readonly Random _random = new Random();
	SKPath? _starPath;
	bool _isMoving;
	float _blinkPhase;

	public Christmas()
	{
		InitializeComponent();
	}

	void OnTouch(object sender, SKTouchEventArgs e)
	{
		if (e.ActionType == SKTouchAction.Pressed || e.ActionType == SKTouchAction.Moved)
		{
			var p = e.Location;
			_isMoving = true;

			_selected = null;
			for (int i = _ornaments.Count - 1; i >= 0; i--) // topmost
			{
				var o = _ornaments[i];
				var dx = p.X - o.P.X;
				var dy = p.Y - o.P.Y;
				if (dx * dx + dy * dy <= o.R * o.R)
				{
					_selected = o;
					break;
				}
			}
		}
		else if (e.ActionType == SKTouchAction.Released || e.ActionType == SKTouchAction.Cancelled)
		{
			_selected = null;
			_isMoving = false;
		}

		e.Handled = true;
		((SKCanvasView)sender).InvalidateSurface();
	}


	// A simple ornament structure
	record Ornament(SKPoint P, float R, SKColor Color, bool IsLit);

	List<Ornament> _ornaments = new();
	Random _rng = new(1234);
	float _t; // animation time (increase from another timer)
	Ornament? _selected;
	SKSize _lastSize; // last size stored

	void EnsureOrnaments(SKImageInfo info)
	{
		bool needsRegenerate =
			_ornaments.Count == 0 ||
			Math.Abs(_lastSize.Width - info.Width) > 1 ||
			Math.Abs(_lastSize.Height - info.Height) > 1;

		if (!needsRegenerate)
		{
			return;
		}

		_lastSize = new SKSize(info.Width, info.Height);
		_ornaments.Clear();
		_rng = new Random(1234); // reset seed for deterministic placement

		// Randomly place ornaments in the tree "silhouette" region (simple)
		var cx = info.Width * 0.5f;
		var topY = info.Height * 0.12f;
		var baseY = info.Height * 0.86f;
		var maxW = info.Width * 0.28f;

		int n = 60;
		for (int i = 0; i < n; i++)
		{
			// y between top and base of the tree
			float y = Lerp(topY + 40, baseY - 40, (float)_rng.NextDouble());
			// tree width at given y (cone)
			float t = (y - topY) / (baseY - topY);
			float halfW = Lerp(maxW * 0.05f, maxW, t);
			float x = cx + (float)((_rng.NextDouble() * 2 - 1) * halfW * 0.85);

			float r = (float)Lerp(info.Width * 0.008f, info.Width * 0.015f, _rng.NextSingle());

			var color = RandomOrnamentColor(_rng);
			_ornaments.Add(new Ornament(new SKPoint(x, y), r, color, _rng.NextSingle() > 0.4));
		}
	}

	static float Lerp(float a, float b, double t) => (float)(a + (b - a) * t);

	static SKColor RandomOrnamentColor(Random rng)
	{
		// Christmas palette
		SKColor[] colors =
		{
		new SKColor(220, 40, 40),   // red
        new SKColor(30, 180, 90),   // green
        new SKColor(30, 120, 220),  // blue
        new SKColor(235, 200, 60),  // gold
        new SKColor(210, 90, 200),  // purple
        new SKColor(240, 240, 240)  // white
    };
		return colors[rng.Next(colors.Length)];
	}

	void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
	{
		var canvas = e.Surface.Canvas;
		var info = e.Info;

		if (info.Width == 0 || info.Height == 0)
		{
			return;
		}

		// Init
		EnsureOrnaments(info);

		// Clear
		canvas.Clear(new SKColor(10, 14, 24));

		// Background: night gradient
		using (var bgPaint = new SKPaint
		{
			IsAntialias = true,
			Shader = SKShader.CreateLinearGradient(
				new SKPoint(0, 0),
				new SKPoint(0, info.Height),
				new[] { new SKColor(8, 10, 18), new SKColor(20, 26, 44), new SKColor(10, 14, 24) },
				new float[] { 0f, 0.55f, 1f },
				SKShaderTileMode.Clamp)
		})
		{
			canvas.DrawRect(new SKRect(0, 0, info.Width, info.Height), bgPaint);
		}

		// Snow ground
		DrawSnowGround(canvas, info);

		// Tree geometry
		float cx = info.Width * 0.5f;
		float topY = info.Height * 0.10f;
		float baseY = info.Height * 0.86f;
		float treeH = baseY - topY;
		float maxW = info.Width * 0.32f;

		// Soft shadow behind tree
		using (var shadow = new SKPaint { IsAntialias = true, Color = new SKColor(0, 0, 0, 90), MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, info.Width * 0.02f) })
		{
			var shadowPath = new SKPath();
			shadowPath.MoveTo(cx, topY);
			shadowPath.LineTo(cx - maxW * 0.85f, baseY);
			shadowPath.LineTo(cx + maxW * 0.85f, baseY);
			shadowPath.Close();
			canvas.DrawPath(shadowPath, shadow);
		}

		// Trunk
		DrawTrunk(canvas, info, cx, baseY, treeH);

		// Draw layered branches
		DrawTreeLayers(canvas, info, cx, topY, baseY, maxW);

		// Garland lights
		if (_isMoving)
		{
			_blinkPhase += 0.3f;
		}
		else
		{
			_blinkPhase *= 0.95f;
		}
		DrawGarland(canvas, info, cx, topY, baseY, maxW, _isMoving);

		// Ornaments
		foreach (var o in _ornaments)
		{
			DrawOrnament(canvas, info, o, isSelected: _selected == o);
		}

		// Star topper
		DrawStar(canvas, info, new SKPoint(cx, topY + treeH * 0.06f), info.Width * 0.035f, twinkle: (float)Math.Sin(_t * 2.2f));

		// Falling snow
		DrawSnowParticles(canvas, info, _t);
	}

	void DrawSnowGround(SKCanvas canvas, SKImageInfo info)
	{
		var groundY = info.Height * 0.86f;
		using var ground = new SKPaint
		{
			IsAntialias = true,
			Shader = SKShader.CreateLinearGradient(
				new SKPoint(0, groundY),
				new SKPoint(0, info.Height),
				new[] { new SKColor(235, 245, 255, 210), new SKColor(210, 230, 255, 255) },
				null,
				SKShaderTileMode.Clamp)
		};
		canvas.DrawRect(new SKRect(0, groundY, info.Width, info.Height), ground);

		// Relief hills
		using var hill = new SKPaint { IsAntialias = true, Color = new SKColor(255, 255, 255, 120) };
		canvas.DrawCircle(info.Width * 0.25f, groundY + info.Height * 0.10f, info.Width * 0.35f, hill);
		canvas.DrawCircle(info.Width * 0.75f, groundY + info.Height * 0.12f, info.Width * 0.42f, hill);
	}

	void DrawTreeLayers(SKCanvas canvas, SKImageInfo info, float cx, float topY, float baseY, float maxW)
	{
		int layers = 6;
		float h = baseY - topY;

		for (int i = 0; i < layers; i++)
		{
			float t = i / (float)(layers - 1);
			float yTop = Lerp(topY, topY + h * 0.58f, t);
			float yBot = Lerp(topY + h * 0.22f, baseY, t);
			float halfW = Lerp(maxW * 0.28f, maxW, t);

			// green shade per layer
			byte g = (byte)(90 + i * 12);
			var c1 = new SKColor(18, g, 38, 255);
			var c2 = new SKColor(10, (byte)(g - 10), 26, 255);

			using var paint = new SKPaint
			{
				IsAntialias = true,
				Shader = SKShader.CreateLinearGradient(
					new SKPoint(cx - halfW, yTop),
					new SKPoint(cx + halfW, yBot),
					new[] { c1, c2 },
					null,
					SKShaderTileMode.Clamp)
			};

			var path = new SKPath();
			path.MoveTo(cx, yTop);
			path.LineTo(cx - halfW, yBot);
			path.LineTo(cx + halfW, yBot);
			path.Close();

			canvas.DrawPath(path, paint);

			// needle highlight
			using var edge = new SKPaint { IsAntialias = true, Color = new SKColor(255, 255, 255, 22) };
			canvas.DrawPath(path, edge);
		}
	}

	void DrawTrunk(SKCanvas canvas, SKImageInfo info, float cx, float baseY, float treeH)
	{
		float trunkW = info.Width * 0.06f;
		float trunkH = treeH * 0.16f;
		var rect = SKRect.Create(cx - trunkW * 0.5f, baseY - trunkH * 0.15f, trunkW, trunkH);

		using var trunk = new SKPaint
		{
			IsAntialias = true,
			Shader = SKShader.CreateLinearGradient(
				new SKPoint(rect.Left, rect.Top),
				new SKPoint(rect.Right, rect.Bottom),
				new[] { new SKColor(110, 70, 35), new SKColor(70, 40, 18) },
				null,
				SKShaderTileMode.Clamp)
		};
		canvas.DrawRoundRect(rect, info.Width * 0.01f, info.Width * 0.01f, trunk);
	}

	void DrawGarland(SKCanvas canvas, SKImageInfo info, float cx, float topY, float baseY, float maxW, bool isMoving = false)
	{
		// One string of lights at the bottom of each layer, except the lowest one
		int layers = 6;
		float h = baseY - topY;

		for (int row = 0; row < layers - 1; row++)
		{
			float t = row / (float)(layers - 1);
			float yBot = Lerp(topY + h * 0.22f, baseY, t);
			float halfW = Lerp(maxW * 0.28f, maxW, t);

			var path = new SKPath();
			int seg = 48;
			for (int i = 0; i <= seg; i++)
			{
				float pathT = i / (float)seg;
				float x = Lerp(cx - halfW, cx + halfW, pathT);
				float wobble = (float)Math.Sin((pathT * 2 * Math.PI) + _t * 1.4f + row) * info.Height * 0.012f;
				float y = yBot + wobble;

				if (i == 0)
				{
					path.MoveTo(x, y);
				}
				else
				{
					path.LineTo(x, y);
				}
			}

			using var wire = new SKPaint
			{
				IsAntialias = true,
				Style = SKPaintStyle.Stroke,
				StrokeWidth = info.Width * 0.0045f,
				Color = new SKColor(30, 25, 18, 170)
			};
			canvas.DrawPath(path, wire);

			// More bulbs on wider layers
			int bulbCount = Math.Max(8, (int)(18 * (0.5f + t * 0.5f)));
			for (int i = 0; i < bulbCount; i++)
			{
				float pathT = i / (float)(bulbCount - 1);
				float x = Lerp(cx - halfW, cx + halfW, pathT);
				float wobble = (float)Math.Sin((pathT * 2 * Math.PI) + _t * 1.4f + row) * info.Height * 0.012f;
				float y = yBot + wobble;

				float r = info.Width * 0.0075f;
				float blinkSpeed = isMoving ? 8f : 3.2f;
				float phase = _t + (isMoving ? _blinkPhase : 0f);
				bool blink = ((i + row) % 2 == 0) ? (Math.Sin(phase * blinkSpeed + i) > 0) : (Math.Cos(phase * (blinkSpeed - 0.4f) + i) > 0);
				var col = blink ? new SKColor(255, 220, 120) : new SKColor(120, 110, 95);
				DrawGlowDot(canvas, new SKPoint(x, y), r, col, glow: blink);
			}
		}
	}

	void DrawGlowDot(SKCanvas canvas, SKPoint p, float r, SKColor col, bool glow)
	{
		if (glow)
		{
			using var g = new SKPaint { IsAntialias = true, Color = new SKColor(col.Red, col.Green, col.Blue, 90), MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, r * 3.2f) };
			canvas.DrawCircle(p, r * 1.2f, g);
		}

		using var paint = new SKPaint { IsAntialias = true, Color = col };
		canvas.DrawCircle(p, r, paint);
	}

	void DrawOrnament(SKCanvas canvas, SKImageInfo info, Ornament o, bool isSelected)
	{
		// glow
		if (o.IsLit || isSelected)
		{
			using var glow = new SKPaint
			{
				IsAntialias = true,
				Color = new SKColor(o.Color.Red, o.Color.Green, o.Color.Blue, (byte)(isSelected ? 130 : 90)),
				MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, o.R * (isSelected ? 6f : 4f))
			};
			canvas.DrawCircle(o.P, o.R * 1.3f, glow);
		}

		// sphere gradient
		var light = new SKPoint(o.P.X - o.R * 0.45f, o.P.Y - o.R * 0.55f);
		var dark = new SKPoint(o.P.X + o.R * 0.65f, o.P.Y + o.R * 0.75f);

		using var sphere = new SKPaint
		{
			IsAntialias = true,
			Shader = SKShader.CreateRadialGradient(
				light,
				o.R * 1.6f,
				new[] { Brighten(o.Color, 1.25f), o.Color, Darken(o.Color, 0.55f) },
				new float[] { 0f, 0.55f, 1f },
				SKShaderTileMode.Clamp)
		};
		canvas.DrawCircle(o.P, o.R, sphere);

		// highlight
		using var hi = new SKPaint { IsAntialias = true, Color = new SKColor(255, 255, 255, 160) };
		canvas.DrawCircle(light, o.R * 0.22f, hi);

		// tiny cap
		using var cap = new SKPaint { IsAntialias = true, Color = new SKColor(40, 40, 40, 200) };
		canvas.DrawRoundRect(new SKRect(o.P.X - o.R * 0.28f, o.P.Y - o.R * 1.12f, o.P.X + o.R * 0.28f, o.P.Y - o.R * 0.78f), o.R * 0.08f, o.R * 0.08f, cap);
	}

	static SKColor Brighten(SKColor c, float k) =>
		new SKColor((byte)Math.Min(255, c.Red * k), (byte)Math.Min(255, c.Green * k), (byte)Math.Min(255, c.Blue * k), c.Alpha);

	static SKColor Darken(SKColor c, float k) =>
		new SKColor((byte)(c.Red * k), (byte)(c.Green * k), (byte)(c.Blue * k), c.Alpha);

	void DrawStar(SKCanvas canvas, SKImageInfo info, SKPoint center, float r, float twinkle)
	{
		float a = 0.6f + 0.4f * (0.5f + 0.5f * twinkle); // 0.6..1.0
		var col = new SKColor(255, 225, 120, (byte)(220 * a));

		// glow
		using (var glow = new SKPaint { IsAntialias = true, Color = new SKColor(255, 220, 120, (byte)(120 * a)), MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, r * 3.5f) })
		{
			canvas.DrawCircle(center, r * 0.9f, glow);
		}

		// star path
		var path = new SKPath();
		int points = 5;
		float inner = r * 0.45f;

		for (int i = 0; i < points * 2; i++)
		{
			float rr = (i % 2 == 0) ? r : inner;
			float ang = (float)(-Math.PI / 2 + i * Math.PI / points);
			var p = new SKPoint(center.X + (float)Math.Cos(ang) * rr, center.Y + (float)Math.Sin(ang) * rr);
			if (i == 0)
			{
				path.MoveTo(p);
			}
			else
			{
				path.LineTo(p);
			}
		}
		path.Close();

		using var paint = new SKPaint { IsAntialias = true, Color = col };
		canvas.DrawPath(path, paint);

		using var edge = new SKPaint { IsAntialias = true, Style = SKPaintStyle.Stroke, StrokeWidth = info.Width * 0.0025f, Color = new SKColor(255, 255, 255, (byte)(120 * a)) };
		canvas.DrawPath(path, edge);
	}

	void DrawSnowParticles(SKCanvas canvas, SKImageInfo info, float t)
	{
		int flakes = 200;
		using var p = new SKPaint { IsAntialias = true, Color = new SKColor(255, 255, 255, 160) };

		for (int i = 0; i < flakes; i++)
		{
			// Random position (from Program.cs logic)
			float x = (float)_random.Next(0, info.Width - 25);
			float y = (float)_random.Next(0, info.Height - 25);

			// Random scale (from Program.cs logic)
			float scale = (float)((_random.NextDouble() * 2) + 0.5);

			float r = 0.7f * scale;
			byte a = (byte)(110 + (i % 6) * 20);
			p.Color = new SKColor(255, 255, 255, a);
			canvas.DrawCircle(x, y, r, p);
		}
	}
}
