using Ink;

// colors
Console.WriteLine(T.Rgb("   rgb color", 124, 106, 247));
Console.WriteLine(T.Hex("   hex color", "#3ecf8e"));
Console.WriteLine(T.Gradient("   gradient text demo", "#7c6af7", "#3ecf8e"));
Console.WriteLine();

// styles
Console.Write("   "); Console.Write(T.Bold("bold")); Console.Write("  ");
Console.Write(T.Dim("dim")); Console.Write("  ");
Console.Write(T.Italic("italic")); Console.Write("  ");
Console.Write(T.Underline("underline")); Console.Write("  ");
Console.WriteLine(T.Strike("strike"));
Console.WriteLine();

// spinner
using (var s = new Spinner("loading things…"))
{
    Thread.Sleep(1800);
    s.Done("loaded");
}

// progress bar
Console.WriteLine();
using (var bar = new ProgressBar("progress"))
{
    for (int i = 0; i <= 20; i++)
    {
        bar.Set(i / 20.0);
        Thread.Sleep(60);
    }
}
Console.WriteLine();

// table
var t = new Table("repos");
t.Header("name", "lang", "tests");
t.Row("phantom",  "C#",    "✓");
t.Row("sift",     "python","✓");
t.Row("arena",    "C++",   "31/31");
t.Row("tally",    "ts",    "✓");
t.Print();
Console.WriteLine();

// prompt
string name = Prompt.Ask("what is your name?");
bool ok = Prompt.Confirm($"hello, {name}. continue?");
if (ok)
{
    string choice = Prompt.Select("pick one:", new[] { "alpha", "beta", "gamma" });
    Console.WriteLine(T.Rgb($"  you picked: {choice}", 124, 106, 247));
}
