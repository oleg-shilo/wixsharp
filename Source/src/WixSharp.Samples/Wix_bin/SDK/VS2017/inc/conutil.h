#pragma once
// Copyright (c) .NET Foundation and contributors. All rights reserved. Licensed under the Microsoft Reciprocal License. See LICENSE.TXT file in the project root for full license information.


#ifdef __cplusplus
extern "C" {
#endif

#define ConsoleExitOnFailure(x, c, f, ...) if (FAILED(x)) { ConsoleWriteError(x, c, f, __VA_ARGS__); ExitTrace(x, f, __VA_ARGS__); goto LExit; }
#define ConsoleExitOnLastError(x, c, f, ...) { x = ::GetLastError(); x = HRESULT_FROM_WIN32(x); if (FAILED(x)) { ConsoleWriteError(x, c, f, __VA_ARGS__); ExitTrace(x, f, __VA_ARGS__); goto LExit; } }
#define ConsoleExitOnNull(p, x, e, c, f, ...) if (NULL == p) { x = e; ConsoleWriteError(x, c, f, __VA_ARGS__); ExitTrace(x, f, __VA_ARGS__); goto LExit; }


// the following macros need to go away
#define ConsoleTrace(l, f, ...) { ConsoleWriteLine(CONSOLE_COLOR_NORMAL, f, __VA_ARGS__); Trace(l, f, __VA_ARGS__); }
#define ConsoleWarning(f, ...) { ConsoleWriteLine(CONSOLE_COLOR_YELLOW, f, __VA_ARGS__); Trace(REPORT_STANDARD, f, __VA_ARGS__); }
#define ConsoleError(x, f, ...) { ConsoleWriteError(x, CONSOLE_COLOR_RED, f, __VA_ARGS__); TraceError(x, f, __VA_ARGS__); }


// enums
typedef enum CONSOLE_COLOR { CONSOLE_COLOR_NORMAL, CONSOLE_COLOR_RED, CONSOLE_COLOR_YELLOW, CONSOLE_COLOR_GREEN } CONSOLE_COLOR;

// structs

// functions
HRESULT DAPI ConsoleInitialize();
void DAPI ConsoleUninitialize();

void DAPI ConsoleGreen();
void DAPI ConsoleRed();
void DAPI ConsoleYellow();
void DAPI ConsoleNormal();

HRESULT DAPI ConsoleWrite(
    CONSOLE_COLOR cc,
    __in_z __format_string LPCSTR szFormat,
    ...
    );
HRESULT DAPI ConsoleWriteLine(
    CONSOLE_COLOR cc,
    __in_z __format_string LPCSTR szFormat,
    ...
    );
HRESULT DAPI ConsoleWriteError(
    HRESULT hrError,
    CONSOLE_COLOR cc,
    __in_z __format_string LPCSTR szFormat,
    ...
    );

HRESULT DAPI ConsoleReadW(
    __deref_out_z LPWSTR* ppwzBuffer
    );

HRESULT DAPI ConsoleReadStringA(
    __deref_out_ecount_part(cchCharBuffer,*pcchNumCharReturn) LPSTR* szCharBuffer,
    CONST DWORD cchCharBuffer,
    __out DWORD* pcchNumCharReturn
    );
HRESULT DAPI ConsoleReadStringW(
    __deref_out_ecount_part(cchCharBuffer,*pcchNumCharReturn) LPWSTR* szCharBuffer,
    CONST DWORD cchCharBuffer,
    __out DWORD* pcchNumCharReturn
    );

HRESULT DAPI ConsoleReadNonBlockingW(
    __deref_out_ecount_opt(*pcchSize) LPWSTR* ppwzBuffer,
    __out DWORD* pcchSize,
    BOOL fReadLine
    );

HRESULT DAPI ConsoleSetReadHidden(void);
HRESULT DAPI ConsoleSetReadNormal(void);

#ifdef __cplusplus
}
#endif

