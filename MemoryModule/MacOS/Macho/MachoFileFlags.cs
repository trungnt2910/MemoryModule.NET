using System;
using System.Collections.Generic;
using System.Text;

namespace MemoryModule.MacOS.Macho
{
    [Flags]
    enum MachoFileFlags : uint
    {
        /// <summary>
        /// the object file has no undefined references
        /// </summary>
        Noundefs = 0x1,
        /// <summary>
        /// the object file is the output of an incremental link against a base file and can't be link edited again
        /// </summary>
        Incrlink = 0x2,
        /// <summary>
        /// the object file is input for the dynamic linker and can't be staticly link edited again
        /// </summary>
        Dyldlink = 0x4,
        /// <summary>
        /// the object file's undefined references are bound by the dynamic linker when loaded.
        /// </summary>
        Bindatload = 0x8,
        /// <summary>
        /// the file has its dynamic undefined references prebound.
        /// </summary>
        Prebound = 0x10,
        /// <summary>
        /// the file has its read-only and read-write segments split
        /// </summary>
        SplitSegs = 0x20,
        /// <summary>
        /// the shared library init routine is to be run lazily via catching memory faults to its writeable segments (obsolete)
        /// </summary>
        LazyInit = 0x40,
        /// <summary>
        /// the image is using two-level name space bindings
        /// </summary>
        Twolevel = 0x80,
        /// <summary>
        /// the executable is forcing all images to use flat name space bindings
        /// </summary>
        ForceFlat = 0x100,
        /// <summary>
        /// this umbrella guarantees no multiple defintions of symbols in its sub-images so the two-level namespace hints can always be used.
        /// </summary>
        Nomultidefs = 0x200,
        /// <summary>
        /// do not have dyld notify the prebinding agent about this executable
        /// </summary>
        Nofixprebinding = 0x400,
        /// <summary>
        /// the binary is not prebound but can have its prebinding redone. only used when MH_PREBOUND is not set.
        /// </summary>
        Prebindable = 0x800,
        /// <summary>
        /// indicates that this binary binds to all two-level namespace modules of its dependent libraries. only used when MH_PREBINDABLE and MH_TWOLEVEL are both set.
        /// </summary>
        Allmodsbound = 0x1000,
        /// <summary>
        /// safe to divide up the sections into sub-sections via symbols for dead code stripping
        /// </summary>
        SubsectionsViaSymbols = 0x2000,
        /// <summary>
        /// the binary has been canonicalized via the unprebind operation
        /// </summary>
        Canonical = 0x4000,
        /// <summary>
        /// the final linked image contains external weak symbols
        /// </summary>
        WeakDefines = 0x8000,
        /// <summary>
        /// the final linked image uses weak symbols
        /// </summary>
        BindsToWeak = 0x10000,
        /// <summary>
        /// When this bit is set, all stacks in the task will be given stack execution privilege. Only used in MH_EXECUTE filetypes.
        /// </summary>
        AllowStackExecution = 0x20000,
        /// <summary>
        /// When this bit is set, the binary declares it is safe for use in processes with uid zero
        /// </summary>
        RootSafe = 0x40000,
        /// <summary>
        /// When this bit is set, the binary declares it is safe for use in processes when issetugid() is true
        /// </summary>
        SetuidSafe = 0x80000,
        /// <summary>
        /// When this bit is set on a dylib, the static linker does not need to examine dependent dylibs to see if any are re-exported
        /// </summary>
        NoReexportedDylibs = 0x100000,
        /// <summary>
        /// When this bit is set, the OS will load the main executable at a random address. Only used in MH_EXECUTE filetypes.
        /// </summary>
        Pie = 0x200000,
        /// <summary>
        /// Only for use on dylibs. When linking against a dylib that has this bit set, the static linker will automatically not create a LC_LOAD_DYLIB load command to the dylib if no symbols are being referenced from the dylib.
        /// </summary>
        DeadStrippableDylib = 0x400000,
        /// <summary>
        /// Contains a section of type S_THREAD_LOCAL_VARIABLES
        /// </summary>
        HasTlvDescriptors = 0x800000,
        /// <summary>
        /// When this bit is set, the OS will run the main executable with a non-executable heap even on platforms (e.g. i386) that don't require it. Only used in MH_EXECUTE filetypes.
        /// </summary>
        NoHeapExecution = 0x1000000,
        /// <summary>
        /// The code was linked for use in an application extension.
        /// </summary>
        AppExtensionSafe = 0x02000000,
    }
}
