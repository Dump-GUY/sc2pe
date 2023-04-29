# sc2pe
Simple dotnet Native AOT app that uses [[AsmResolver]](https://github.com/Washi1337/AsmResolver) to convert shellcode to PE.<br/>
Both 32-bit and 64-bit shellcode is supported -> resulting in 32-bit or 64-bit PE.<br/>
[[Releases]](https://github.com/Dump-GUY/sc2pe/releases) are compiled for both linux-x64 and win-x64.<br/>

## Description
It is compiled to Native AOT format to demonstrate how awesome it is (still, it was a pain as some dependency uses reflection that gets trimmed if we won't trick it :D). The [[release]](https://github.com/Dump-GUY/sc2pe/releases) binaries available to download are native binaries without any dotnet dependency! (so just run and enjoy)

Native AOT compiled, using dotnet SDK 8.0.100-preview.3.23178.7<br/>
Dotnet SDK 8+ was chosen because of producing much smaller binaries.<br/>

Native AOT compilation is supported from the dotnet 7 version.
If you want to use dotnet 7 for building, modify the project file to target net7.0 -> building is much easier for dotnet 7.
<br/>
<br/>
### Building on Windows using dotnet SDK 8+:
Pretty easy:
- Install or update to the latest Visual Studio (preview).
- Install dotnet 8+ SDK.
- Open .sln and restore nugets (AsmResolver.PE, CommandLineParser) -> should be automatic if you configured the nuget URL.
- VS Terminale - "dotnet publish -r win-x64 -c Release"

### Building on Linux using dotnet SDK 8+:
- I don't wanna talk about it; just use the release binaries :) (as dotnet SDK 8+ is not available via the package manager, it is a pain in the ass)<br/>

### Usage<br/>
  -p, --path            [Required. Path to shellcode file]<br/>
<br/>
  -a, --architecture    [Required. Architecture: 32 or 64 (depending on the shellcode)]<br/>
<br/>
  -o, --offset          [Optional. Start offset of the shellcode (default 0)]<br/>
<br/>
  --help                [Display this help screen]<br/>
<br/>
  --version             [Display version information]<br/>

### Example
Convert shellcode to 32-bit PE (shellcode Start Offset set to 66th byte):<br/>
sc2pe -a 32 -o 66 -p C:\shellcode.bin<br/>
