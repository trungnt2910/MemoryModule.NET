using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MemoryModule
{
    [Flags]
    enum Page : uint
    {
        NoAccess = 0x01,
        ReadOnly = 0x02,
        ReadWrite = 0x04,
        WriteCopy = 0x08,
        Execute = 0x10,
        ExecuteRead = 0x20,
        ExecuteReadWrite = 0x40,
        ExecuteWriteCopy = 0x80,
        PAGE_GUARD = 0x100,
        PAGE_NOCACHE = 0x200,
        PAGE_WRITECOMBINE = 0x400,
        PAGE_GRAPHICS_NOACCESS = 0x0800,
        PAGE_GRAPHICS_READONLY = 0x1000,
        PAGE_GRAPHICS_READWRITE = 0x2000,
        PAGE_GRAPHICS_EXECUTE = 0x4000,
        PAGE_GRAPHICS_EXECUTE_READ = 0x8000,
        PAGE_GRAPHICS_EXECUTE_READWRITE = 0x10000,
        PAGE_GRAPHICS_COHERENT = 0x20000,
        PAGE_ENCLAVE_THREAD_CONTROL = 0x80000000,
        PAGE_REVERT_TO_FILE_MAP = 0x80000000,
        PAGE_TARGETS_NO_UPDATE = 0x40000000,
        PAGE_TARGETS_INVALID = 0x40000000,
        PAGE_ENCLAVE_UNVALIDATED = 0x20000000,
        PAGE_ENCLAVE_DECOMMIT = 0x10000000
    }
}
