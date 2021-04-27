# Z64Utils

WIP Tool to parse and view various files and structures in Zelda 64 ROMs.

<img src="https://i.imgur.com/N7ROTdS.png" width=100%/>
Demo (old) : https://youtu.be/AIVDunCtSnM

# Features

## Filesystem Parser
The tool can parse the filesystem contained in a given Zelda ROM and extract/replace/open the files in it.

### ROM/RAM Address Conversion
The tool recreates the memory map of the ROM like how it would lie in virtual RAM (i.e. without any heap allocation / code relocation) and makes you able to convert addresses from one address space to another.

## Object Analyzer
The Object analyzer is capable of analyzing and finding display lists in a given "object" file (these usually contain assets such as model data, textures, skeletons, etc.) and from there, find and decode the data blocks within the object file.

## F3DZEX Disassembler
The tool contains a disassembler that can decode the [F3DZEX](https://wiki.cloudmodding.com/oot/F3DZEX2) commands issued by the game to the RSP.

## Texture Viewer
The texture viewer supports all the texture formats used by the nintendo 64 RDP (CI4, CI8, I4, I8, IA4, IA8, IA16, RGBA16, RGBA32).

## Display List Viewer
The tool contains a renderer that can process [F3DZEX](https://wiki.cloudmodding.com/oot/F3DZEX2) display lists.

## Skeleton Viewer / Animation Player
The Skeleton Viewer can parse and render skeletons and animations used in Ocarina of Time and Majora's Mask.

Note that currently only standart limbs are supported (no SkinLimb/LodLimb)

# Dependencies

Currently, the only requirement is `.NET 5.0.5` (Not to be confused with `.NET Core` or `.NET Framework`).

The general purpose download link for `.NET` is [this](https://dotnet.microsoft.com/download), and the direct download is [here](https://dotnet.microsoft.com/download/dotnet/thank-you/sdk-5.0.202-windows-x64-installer).

## Wine / Linux

Z64Utils v2.0.1 is wine-compatible (this may change in the future).

To run it, first you need to install `.NET 5.0.5` using the installer (see [Dependencies](#dependencies) for the download link).

Please keep in mind you need to use `wine64` instead of `wine` because this is a 64bits-only app.

```bash
wine64 dotnet-sdk-5.0.202-win-x64.exe
wine64 Z64Utils.exe
```
