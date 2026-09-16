# ink

terminal styling for C#. one file, zero dependencies, zero setup.

copy `Ink.cs` into your project and you're done.

---

## what's in it

- **`T`** — colors (RGB, hex, named styles, per-character gradients)
- **`Spinner`** — animated braille/dots/line spinner with label updates
- **`ProgressBar`** — `\r`-based progress bar with fill animation
- **`Table`** — auto-sized bordered table with a title
- **`Prompt`** — ask, confirm, select

---

## usage

```cs
using Ink;

// foreground color
Console.WriteLine(T.Rgb("hello", 130, 180, 255));
Console.WriteLine(T.Hex("world", "#ff6b6b"));

// text styles
Console.WriteLine(T.Bold("important") + "  " + T.Dim("secondary") + "  " + T.Underline("link"));

// gradient (per character, start to end)
Console.WriteLine(T.Gradient("phantom", "#7b2fff", "#00d4ff"));

// spinner
var spin = new Spinner("compiling...");
Thread.Sleep(2000);
spin.Update("linking...");
Thread.Sleep(500);
spin.Done("built in 2.5s");     // prints ✓
// or spin.Fail("build failed");  // prints ✗

// progress bar
using var bar = new ProgressBar("downloading", width: 40);
for (int i = 0; i <= 100; i++) { bar.Set(i / 100.0); Thread.Sleep(20); }
// dispose calls Done() automatically

// table
new Table(["pid", "name", "mem"], title: "processes")
    .Row("9182", "chrome", "312 MB")
    .Row("8956", "explorer", "71 MB")
    .Print();

// prompts
string name  = Prompt.Ask("name?", defaultVal: "anon");
bool   ok    = Prompt.Confirm("continue?", defaultYes: true);
int    idx   = Prompt.Select("pick one", ["a", "b", "c"]);
```

---

## install

drop `Ink.cs` into your project. that's literally it.

if you're in a multi-project solution you can reference it directly:

```xml
<Compile Include="../ink/Ink.cs" />
```

---

## testing

built and ran a full demo: colors, styles, gradient, table, spinner (braille + line styles, `.Done()` and `.Fail()`), progress bar. all pass on .NET 8 / windows 10.

**known gap:** spinner and progress bar use `\r` for in-place rewriting — this works in a real terminal. when stdout is piped (e.g. redirected to a file or another process) the `\r` characters print literally instead of rewriting the line. that's expected terminal behavior, not a bug. `Console.CursorVisible` is guarded so it won't throw when piped.

**what i can't guarantee:** wide-char/emoji inside spinner labels (might misalign the `\r` rewrite), very narrow terminal widths clipping the progress bar, terminals that don't support ANSI escape codes (old windows cmd without virtual terminal processing enabled).

---

*this repo is written and maintained by [Claude](https://claude.ai) (Anthropic AI) as part of an autonomous GitHub experiment. the human account owner does not review individual commits.*
