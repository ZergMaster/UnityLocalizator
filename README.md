# CyberCat Localizator

*[Русская версия](README.ru.md)*

A small Windows desktop tool that turns Unity localization from "hand-edit two JSON files and hope"
into a two-language table you can actually work in — and generates the C# key enum for you, so a typo
becomes a compile error instead of an empty label in the shipped build.

Built for and used on **CyberCat**, a mobile game made at a small game studio (a Mail.Ru Group
subsidiary at the time).

---

## The problem

Unity has no built-in localization workflow worth the name. On CyberCat the strings lived where they
usually do — as JSON in `StreamingAssets`, one file per language:

```
Assets/StreamingAssets/Localization/ru.json
Assets/StreamingAssets/Localization/en.json
```

Every new line of text meant opening both files by hand. That is where the day went:

- **The translator worked blind.** Handed `en.json`, they saw `reviveOffer` and an English string
  with no Russian original next to it, no screen it belongs to, no idea whether it is a button label
  (three words max) or a dialog line. Context lived in someone's head, and questions came back as
  chat messages.
- **The two files drifted.** Add a key to `ru.json`, forget `en.json`, and the English build shows a
  blank. Nothing catches it — not the compiler, not the editor. You find it in a playtest, or a
  player does.
- **One stray character broke everything.** An unescaped quote or a raw newline pasted into a value
  is invalid JSON, so the parse fails and the *whole* language dies at once, not just that string.
- **Keys were magic strings.** Game code did `Localization.Get("reviveOffer")`. Misspell it and you
  get silence at runtime — no error, no warning, just an empty label.
- **Keys were not always valid identifiers.** Spaces and Cyrillic crept in, which ruled out ever
  generating anything typed from them.

## The solution

Localizator points at a Unity project root and opens both language files as one table — key, Russian,
English, side by side — so a translator sees the original and the translation together, and a missing
counterpart is visible at a glance instead of at playtest.

Adding and editing go through a dialog that validates the key as you type (Latin letters and digits
only, never leading with a digit, spaces folded to camelCase), and writes both JSON files in one
step, escaping quotes and newlines so the files stay parseable.

On every save it also regenerates two C# files inside the Unity project — a `LocKeys` enum and a
`LocDataTexts` class with one field per key — which turns every localization key into a compile-time
symbol: autocomplete in the IDE, and a build error the moment a key is renamed or removed.

---

## What it looks like

![Localizator main window — keys with Russian and English values side by side](docs/main-window.png)

The full key table, both languages at once. Right-click any row to edit or delete it; `Path` picks
the Unity project root, `Refresh` reloads from disk and rewrites the generated C#.

![Add localization dialog](docs/add-dialog.png)

Adding a key. Invalid characters are rejected at the keystroke, and both languages are required —
you cannot create a half-translated key by accident.

---

## Getting started

1. Clone the repo and open `Localizator.sln` in Visual Studio 2019+ or JetBrains Rider (Windows,
   .NET Framework 4.7.1).
2. Build — NuGet restores the one dependency, `Newtonsoft.Json`.
3. Run `Localizator.exe`, click **Path**, and select your Unity project root (the folder containing
   `Assets/`).
4. The path is remembered between launches; the table loads immediately on the next start.
5. Add, edit or delete keys — every change writes `ru.json`, `en.json`, `LocKeys.cs` and
   `LocDataTexts.cs` straight away.

### What it expects in the Unity project

```
<project root>/
├── Assets/StreamingAssets/Localization/ru.json      ← read + written
├── Assets/StreamingAssets/Localization/en.json      ← read + written
└── Assets/Scripts/Localization/
    ├── LocKeys.cs                                   ← generated
    └── LocDataTexts.cs                              ← generated
```

JSON format — a `texts` object of flat key/value pairs:

```json
{
  "texts": {
    "startGame": "Начать игру",
    "reviveOffer": "Поднять кота за 50 монет?"
  }
}
```

Generated output:

```csharp
namespace Viktoriaplus.CyberCat.Localization {
    public enum LocKeys {
        startGame = 0,
        reviveOffer = 1,
    }
}
```

Both paths and the generated namespace are hardcoded to the CyberCat project layout — adapting the
tool to another project is a matter of editing the string constants in `MainWindow.xaml.cs`.

---

## Who used it, and what changed

Translators at the studio used it directly, instead of being sent raw JSON. Seeing the Russian
original and the English translation in the same row removed the round-trip of "what does this string
mean, where does it appear" — the question that used to come back over chat for a good share of the
strings.

For the developers, the payoff was the generated `LocKeys` enum. Once localization keys were symbols
rather than string literals, a renamed or deleted key broke the build immediately instead of shipping
as an empty label. The class of bug that used to be found in playtest — or after release — stopped
reaching playtest at all.

And the two language files stopped drifting apart, because there was no longer a way to edit one
without the other.

---

## Tech

C# · WPF · .NET Framework 4.7.1 · Newtonsoft.Json 12.0.3 · Windows only
