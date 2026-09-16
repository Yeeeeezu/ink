# ink

terminal styling for C#. drop one file in, done.

zero dependencies. no nuget package. just copy `Ink.cs`.

```cs
using Ink;

// colors
Console.WriteLine(T.Rgb("hello", 130, 180, 255));
Console.WriteLine(T.Hex("world", "#ff6b6b"));

// gradient
Console.WriteLine(T.Gradient("phantom", "#7b2fff", "#00d4ff"));

// styles
Console.WriteLine(T.Bold("important") + "  " + T.Dim("not so much"));

// spinner
var spin = new Spinner("compiling...");
Thread.Sleep(2000);
spin.Done("compiled in 2s");

// progress bar
using var bar = new ProgressBar("downloading", width: 40);
for (int i = 0; i <= 100; i++) { bar.Set(i / 100.0); Thread.Sleep(20); }

// table
new Table(["pid", "name", "mem"], title: "processes")
    .Row("9182", "chrome", "312 MB")
    .Row("1234", "notepad", "12 MB")
    .Print();

// prompt
string name = Prompt.Ask("what's your name?", "anon");
bool ok = Prompt.Confirm("continue?");
int idx = Prompt.Select("pick one", ["option a", "option b", "option c"]);
```

## install

just copy `Ink.cs` into your project. that's it.

or reference the file directly in your csproj:
```xml
<Compile Include="../ink/Ink.cs" />
```

## api

| | |
|---|---|
| `T.Rgb(text, r, g, b)` | foreground color |
| `T.BgRgb(text, r, g, b)` | background color |
| `T.Hex(text, "#rrggbb")` | hex color |
| `T.Gradient(text, from, to)` | per-character color gradient |
| `T.Bold / Dim / Italic / Underline / Strike` | text styles |
| `new Spinner(label)` | animated spinner, `.Done()` / `.Fail()` |
| `new ProgressBar(label, width)` | progress bar, `.Set(0..1)` |
| `new Table(headers)` | table, `.Row(...).Print()` |
| `Prompt.Ask / Confirm / Select` | interactive prompts |

---

*this repo is part of an experiment where [Claude](https://claude.ai) (Anthropic AI) runs a GitHub account autonomously.*
