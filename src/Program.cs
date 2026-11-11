using Mono.Cecil;
using Mono.Cecil.Cil;

if (args.Length != 2)
{
    Console.Error.WriteLine("Usage: patchge <input DLL> <output DLL>");
    return 1;
}

if (Path.GetFullPath(args[0]) == Path.GetFullPath(args[1]))
{
    Console.Error.WriteLine("Input and output paths must be different.");
    return 1;
}

const string targetType = "Microsoft.WindowsAPICodePack.Taskbar.TaskbarManager";
const string targetMethod = "get_IsPlatformSupported";

using var assembly = AssemblyDefinition.ReadAssembly(args[0]);
var type = assembly.MainModule.GetType(targetType)
    ?? throw new InvalidOperationException($"Type not found: {targetType}");
var method = type.Methods.SingleOrDefault(m => m.Name == targetMethod && m.IsStatic
    && m.Parameters.Count == 0 && m.ReturnType.FullName == "System.Boolean" && m.HasBody)
    ?? throw new InvalidOperationException($"Method not found: {targetType}.{targetMethod}");

var il = method.Body.GetILProcessor();
method.Body.Instructions.Clear();
method.Body.ExceptionHandlers.Clear();
method.Body.Variables.Clear();
method.Body.InitLocals = true;
il.Append(Instruction.Create(OpCodes.Ldc_I4_0));
il.Append(Instruction.Create(OpCodes.Ret));

assembly.Write(args[1], new WriterParameters { WriteSymbols = false });
Console.WriteLine("Patched OK -> " + args[1]);
return 0;
