# MakeIcon.Cli

Universal Windows Icon Generator CLI for MicaSetup.

## Usage

```bash
makeicon --help
Description:

Usage:
  makeicon [options]

Options:
  --input <input>  Your input image file path.
  --type <type>    Support icon type normal,setup,uninst.
  --size <size>    Support size 256,128,64,48,32,24,16.
  --ext <ext>      Support extension png,ico.
  --version        Show version information
  -?, -h, --help   Show help and usage information
```

### Example

```bash
makeicon --input "C:\Users\ema\Desktop\1.png" --type "normal,setup,uninst" --size "256,128" --ext "png,ico"
```

and then `Favicon.png`, `Favicon.ico`, `FaviconSetup.png`, `FaviconSetup.ico`, `FaviconUninst.png`, `FaviconUninst.ico` will be created.



