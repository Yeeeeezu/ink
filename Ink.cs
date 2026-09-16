using System.Text;

namespace Ink;

// terminal styling: colors, gradients, spinners, progress bars, tables.
// zero dependencies. just drop Ink.cs into your project.

public static class T
{
    // ---- rgb color ----

    public static string Rgb(string text, byte r, byte g, byte b)
        => $"\x1b[38;2;{r};{g};{b}m{text}\x1b[0m";

    public static string BgRgb(string text, byte r, byte g, byte b)
        => $"\x1b[48;2;{r};{g};{b}m{text}\x1b[0m";

    public static string Hex(string text, string hex)
    {
        hex = hex.TrimStart('#');
        byte r = Convert.ToByte(hex[..2], 16);
        byte g = Convert.ToByte(hex[2..4], 16);
        byte b = Convert.ToByte(hex[4..6], 16);
        return Rgb(text, r, g, b);
    }

    // ---- named styles ----

    public static string Bold(string text) => $"\x1b[1m{text}\x1b[0m";
    public static string Dim(string text) => $"\x1b[2m{text}\x1b[0m";
    public static string Italic(string text) => $"\x1b[3m{text}\x1b[0m";
    public static string Underline(string text) => $"\x1b[4m{text}\x1b[0m";
    public static string Strike(string text) => $"\x1b[9m{text}\x1b[0m";

    // ---- gradient ----

    // interpolates RGB across the string character by character
    public static string Gradient(string text, (byte r, byte g, byte b) from, (byte r, byte g, byte b) to)
    {
        if (text.Length == 0) return text;
        var sb = new StringBuilder();
        for (int i = 0; i < text.Length; i++)
        {
            float t = text.Length == 1 ? 0f : (float)i / (text.Length - 1);
            byte r = (byte)(from.r + (to.r - from.r) * t);
            byte g = (byte)(from.g + (to.g - from.g) * t);
            byte b = (byte)(from.b + (to.b - from.b) * t);
            sb.Append($"\x1b[38;2;{r};{g};{b}m{text[i]}");
        }
        sb.Append("\x1b[0m");
        return sb.ToString();
    }

    public static string Gradient(string text, string fromHex, string toHex)
    {
        fromHex = fromHex.TrimStart('#');
        toHex = toHex.TrimStart('#');
        return Gradient(text,
            (Convert.ToByte(fromHex[..2], 16), Convert.ToByte(fromHex[2..4], 16), Convert.ToByte(fromHex[4..6], 16)),
            (Convert.ToByte(toHex[..2], 16), Convert.ToByte(toHex[2..4], 16), Convert.ToByte(toHex[4..6], 16)));
    }
}

// ---- spinner ----

public sealed class Spinner : IDisposable
{
    static readonly string[] Frames = ["⠋", "⠙", "⠹", "⠸", "⠼", "⠴", "⠦", "⠧", "⠇", "⠏"];
    static readonly string[] DotsFrames = ["   ", ".  ", ".. ", "..."];
    static readonly string[] LineFrames = ["-", "\\", "|", "/"];

    private readonly string[] frames;
    private readonly string label;
    private readonly int intervalMs;
    private readonly Thread thread;
    private volatile bool running = true;
    private volatile string currentLabel;
    private int frame;

    public enum Style { Braille, Dots, Line }

    public Spinner(string label = "working", Style style = Style.Braille, int intervalMs = 80)
    {
        this.label = label;
        this.currentLabel = label;
        this.intervalMs = intervalMs;
        this.frames = style switch
        {
            Style.Dots => DotsFrames,
            Style.Line => LineFrames,
            _ => Frames
        };

        Console.CursorVisible = false;
        thread = new Thread(Run) { IsBackground = true };
        thread.Start();
    }

    public void Update(string newLabel) => currentLabel = newLabel;

    private void Run()
    {
        while (running)
        {
            string f = T.Rgb(frames[frame % frames.Length], 130, 180, 255);
            Console.Write($"\r  {f}  {currentLabel}   ");
            frame++;
            Thread.Sleep(intervalMs);
        }
    }

    public void Done(string? message = null)
    {
        running = false;
        thread.Join();
        Console.Write($"\r  {T.Rgb("✓", 80, 220, 120)}  {message ?? currentLabel}   \n");
        Console.CursorVisible = true;
    }

    public void Fail(string? message = null)
    {
        running = false;
        thread.Join();
        Console.Write($"\r  {T.Rgb("✗", 255, 80, 80)}  {message ?? currentLabel}   \n");
        Console.CursorVisible = true;
    }

    public void Dispose() => Done();
}

// ---- progress bar ----

public sealed class ProgressBar : IDisposable
{
    private readonly int width;
    private readonly string label;
    private double value;

    public ProgressBar(string label = "", int width = 30)
    {
        this.label = label;
        this.width = width;
        Console.CursorVisible = false;
        Render();
    }

    public void Set(double pct)
    {
        value = Math.Clamp(pct, 0, 1);
        Render();
    }

    public void Advance(double delta) => Set(value + delta);

    private void Render()
    {
        int filled = (int)(width * value);
        string bar = new string('█', filled) + new string('░', width - filled);
        string colored = T.Rgb(bar[..filled], 80, 180, 255) + T.Dim(bar[filled..]);
        int pct = (int)(value * 100);
        Console.Write($"\r  [{colored}] {pct,3}%  {label}  ");
    }

    public void Done()
    {
        Set(1);
        Console.WriteLine();
        Console.CursorVisible = true;
    }

    public void Dispose() => Done();
}

// ---- table ----

public sealed class Table
{
    private readonly string[] headers;
    private readonly List<string[]> rows = new();
    private readonly string? title;

    public Table(string[] headers, string? title = null)
    {
        this.headers = headers;
        this.title = title;
    }

    public Table Row(params string[] cols) { rows.Add(cols); return this; }

    public void Print()
    {
        int cols = headers.Length;
        int[] widths = new int[cols];
        for (int i = 0; i < cols; i++) widths[i] = headers[i].Length;
        foreach (var row in rows)
            for (int i = 0; i < Math.Min(cols, row.Length); i++)
                widths[i] = Math.Max(widths[i], row[i].Length);

        string Pad(string s, int w) => s.PadRight(w);
        string Sep(char l, char m, char r, char h) =>
            l + string.Join(m, widths.Select(w => new string(h, w + 2))) + r;

        Console.WriteLine();
        if (title != null)
            Console.WriteLine($"  {T.Bold(title)}");

        Console.WriteLine("  " + Sep('┌', '┬', '┐', '─'));
        Console.Write("  │");
        for (int i = 0; i < cols; i++)
            Console.Write($" {T.Bold(Pad(headers[i], widths[i]))} │");
        Console.WriteLine();
        Console.WriteLine("  " + Sep('├', '┼', '┤', '─'));

        foreach (var row in rows)
        {
            Console.Write("  │");
            for (int i = 0; i < cols; i++)
            {
                string cell = i < row.Length ? row[i] : "";
                Console.Write($" {T.Dim(Pad(cell, widths[i]))} │");
            }
            Console.WriteLine();
        }

        Console.WriteLine("  " + Sep('└', '┴', '┘', '─'));
        Console.WriteLine();
    }
}

// ---- prompt ----

public static class Prompt
{
    public static string Ask(string question, string? defaultVal = null)
    {
        string hint = defaultVal != null ? T.Dim($" [{defaultVal}]") : "";
        Console.Write($"  {T.Rgb("?", 130, 180, 255)} {question}{hint}  ");
        string? input = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(input) && defaultVal != null) return defaultVal;
        return input ?? "";
    }

    public static bool Confirm(string question, bool defaultYes = true)
    {
        string hint = defaultYes ? T.Dim("[Y/n]") : T.Dim("[y/N]");
        Console.Write($"  {T.Rgb("?", 130, 180, 255)} {question} {hint}  ");
        string? input = Console.ReadLine()?.Trim().ToLowerInvariant();
        if (string.IsNullOrEmpty(input)) return defaultYes;
        return input == "y" || input == "yes";
    }

    public static int Select(string question, string[] options)
    {
        Console.WriteLine($"  {T.Rgb("?", 130, 180, 255)} {question}");
        for (int i = 0; i < options.Length; i++)
            Console.WriteLine($"    {T.Dim($"{i + 1}.")} {options[i]}");
        Console.Write("  ");

        while (true)
        {
            string? input = Console.ReadLine();
            if (int.TryParse(input, out int n) && n >= 1 && n <= options.Length)
                return n - 1;
            Console.Write($"  {T.Rgb("enter 1-" + options.Length, 255, 120, 80)}  ");
        }
    }
}
