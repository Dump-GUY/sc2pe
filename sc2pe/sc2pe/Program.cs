using AsmResolver;
using AsmResolver.PE.File;
using AsmResolver.PE.File.Headers;
using CommandLine;
using CommandLine.Text;
using System.Diagnostics.CodeAnalysis;

namespace sc2pe
{
    internal class Program
    {
        public class Options
        {
            [Option('p', "path", Required = true, HelpText = "Path to shellcode file.")]
            public string? Path { get; set; }

            [Option('a', "architecture", Required = true, HelpText = "Architecture: 32 or 64 (depending on the shellcode).")]
            public uint? Architecture { get; set; }

            [Option('o', "offset", Required = false, Default = (uint)0, HelpText = "Optional. Start offset of the shellcode (default 0).")]
            public uint Offset { get; set; }

            [Usage(ApplicationAlias = "sc2pe")]
            public static IEnumerable<Example> Examples
            {
                get
                {
                    return new List<Example>()
                    {
                        new Example("Convert shellcode to 32-bit PE (shellcode Start Offset set to 66th byte)", new Options { Path = "C:\\shellcode.bin", Architecture = 32, Offset = 66 })
                    };
                }
            }
        }

        static PEFile CreatePE64(string path, uint epOffset, ulong imageBase = 0x140000000)
        {
            byte[] shellcode = File.ReadAllBytes(path);
            var pe = new PEFile();
            var text = new PESection(".text", SectionFlags.ContentCode | SectionFlags.MemoryRead | SectionFlags.MemoryExecute);
            text.Contents = new DataSegment(shellcode);
            pe.Sections.Add(text);

            pe.OptionalHeader.ImageBase = imageBase;
            pe.OptionalHeader.DllCharacteristics = 0;
            pe.FileHeader.Machine = MachineType.Amd64;
            pe.FileHeader.Characteristics = Characteristics.Image | Characteristics.LocalSymsStripped | Characteristics.LineNumsStripped | Characteristics.RelocsStripped | Characteristics.LargeAddressAware;
            pe.OptionalHeader.Magic = OptionalHeaderMagic.PE64;
            pe.UpdateHeaders();
            pe.OptionalHeader.AddressOfEntryPoint = text.Rva + epOffset;

            return pe;
        }

        static PEFile CreatePE32(string path, uint epOffset, ulong imageBase = 0x400000)
        {
            byte[] shellcode = File.ReadAllBytes(path);
            var pe = new PEFile();
            var text = new PESection(".text", SectionFlags.ContentCode | SectionFlags.MemoryRead | SectionFlags.MemoryExecute);
            text.Contents = new DataSegment(shellcode);
            pe.Sections.Add(text);

            pe.OptionalHeader.ImageBase = imageBase;
            pe.OptionalHeader.DllCharacteristics = 0;
            pe.FileHeader.Machine = MachineType.I386;
            pe.FileHeader.Characteristics = Characteristics.Image | Characteristics.LocalSymsStripped | Characteristics.LineNumsStripped | Characteristics.RelocsStripped | Characteristics.Machine32Bit;
            pe.OptionalHeader.Magic = OptionalHeaderMagic.PE32;
            pe.UpdateHeaders();
            pe.OptionalHeader.AddressOfEntryPoint = text.Rva + epOffset;

            return pe;
        }

        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(Options))]
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                   .WithParsed<Options>(o =>
                   {
                       if (!File.Exists(o.Path)) { Console.WriteLine($"Can´t find shellcode file: {o.Path}"); }

                       else 
                       {
                           if (o.Architecture == 32)
                           {
                               var pe = CreatePE32(o.Path, o.Offset);
                               pe.Write(o.Path + ".exe");
                               Console.WriteLine($"PE created: {o.Path + ".exe"}");                                
                           }
                           else if (o.Architecture == 64)
                           {
                               var pe = CreatePE64(o.Path, o.Offset);
                               pe.Write(o.Path + ".exe");
                               Console.WriteLine($"PE created: {o.Path + ".exe"}");
                           }
                           else { Console.WriteLine($"Wrong architecture selected: {o.Architecture}"); }                      
                       }
                   });            
        }
    }
}
