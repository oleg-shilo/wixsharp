#pragma once
// Copyright (c) .NET Foundation and contributors. All rights reserved. Licensed under the Microsoft Reciprocal License. See LICENSE.TXT file in the project root for full license information.


#include "dutil.h"


#ifdef __cplusplus
extern "C" {
#endif

#define BalExitOnFailure(x, f, ...) if (FAILED(x)) { BalLogError(x, f, __VA_ARGS__); ExitTrace(x, f, __VA_ARGS__); goto LExit; }
#define BalExitOnRootFailure(x, f, ...) if (FAILED(x)) { BalLogError(x, f, __VA_ARGS__); Dutil_RootFailure(__FILE__, __LINE__, x); ExitTrace(x, f, __VA_ARGS__); goto LExit; }
#define BalExitOnNullWithLastError(p, x, f, ...) if (NULL == p) { DWORD Dutil_er = ::GetLastError(); x = HRESULT_FROM_WIN32(Dutil_er); if (!FAILED(x)) { x = E_FAIL; } BalLogError(x, f, __VA_ARGS__); ExitTrace(x, f, __VA_ARGS__); goto LExit; }

#define FACILITY_WIX 500

const LPCWSTR BAL_MANIFEST_FILENAME = L"BootstrapperApplicationData.xml";

static const HRESULT E_WIXSTDBA_CONDITION_FAILED = MAKE_HRESULT(SEVERITY_ERROR, FACILITY_WIX, 1);

static const HRESULT E_MBAHOST_NET452_ON_WIN7RTM = MAKE_HRESULT(SEVERITY_ERROR, FACILITY_WIX, 1000);


/*******************************************************************
 BalInitialize - remembers the engine interface to enable logging and
                 other functions.

********************************************************************/
DAPI_(void) BalInitialize(
    __in IBootstrapperEngine* pEngine
    );

/*******************************************************************
 BalInitializeFromCreateArgs - convenience function to call BalBootstrapperEngineCreate
                               then pass it along to BalInitialize.

********************************************************************/
DAPI_(HRESULT) BalInitializeFromCreateArgs(
    __in const BOOTSTRAPPER_CREATE_ARGS* pArgs,
    __out IBootstrapperEngine** ppEngine
    );

/*******************************************************************
 BalUninitialize - cleans up utility layer internals.

********************************************************************/
DAPI_(void) BalUninitialize();

/*******************************************************************
 BalManifestLoad - loads the Application manifest into an XML document.

********************************************************************/
DAPI_(HRESULT) BalManifestLoad(
    __in HMODULE hUXModule,
    __out IXMLDOMDocument** ppixdManifest
    );

/*******************************************************************
BalEvaluateCondition - evaluates a condition using variables in the engine.

********************************************************************/
DAPI_(HRESULT) BalEvaluateCondition(
    __in_z LPCWSTR wzCondition,
    __out BOOL* pf
    );

/*******************************************************************
BalFormatString - formats a string using variables in the engine.

 Note: Use StrFree() to release psczOut.
********************************************************************/
DAPI_(HRESULT) BalFormatString(
    __in_z LPCWSTR wzFormat,
    __inout LPWSTR* psczOut
    );

/*******************************************************************
BalGetNumericVariable - gets a number from a variable in the engine.

 Note: Returns E_NOTFOUND if variable does not exist.
********************************************************************/
DAPI_(HRESULT) BalGetNumericVariable(
    __in_z LPCWSTR wzVariable,
    __out LONGLONG* pllValue
    );

/*******************************************************************
BalSetNumericVariable - sets a numeric variable in the engine.

********************************************************************/
DAPI_(HRESULT) BalSetNumericVariable(
    __in_z LPCWSTR wzVariable,
    __in LONGLONG llValue
    );

/*******************************************************************
BalStringVariableExists - checks if a string variable exists in the engine.

********************************************************************/
DAPI_(BOOL) BalStringVariableExists(
    __in_z LPCWSTR wzVariable
    );

/*******************************************************************
BalGetStringVariable - gets a string from a variable in the engine.

 Note: Use StrFree() to release psczValue.
********************************************************************/
DAPI_(HRESULT) BalGetStringVariable(
    __in_z LPCWSTR wzVariable,
    __inout LPWSTR* psczValue
    );

/*******************************************************************
BalSetStringVariable - sets a string variable in the engine.

********************************************************************/
DAPI_(HRESULT) BalSetStringVariable(
    __in_z LPCWSTR wzVariable,
    __in_z_opt LPCWSTR wzValue
    );

/*******************************************************************
 BalLog - logs a message with the engine.

********************************************************************/
DAPIV_(HRESULT) BalLog(
    __in BOOTSTRAPPER_LOG_LEVEL level,
    __in_z __format_string LPCSTR szFormat,
    ...
    );

/*******************************************************************
 BalLogError - logs an error message with the engine.

********************************************************************/
DAPIV_(HRESULT) BalLogError(
    __in HRESULT hr,
    __in_z __format_string LPCSTR szFormat,
    ...
    );

/*******************************************************************
BalLogId - logs a message with the engine with a string embedded in a 
           MESSAGETABLE resource.

********************************************************************/
DAPIV_(HRESULT) BalLogId(
    __in BOOTSTRAPPER_LOG_LEVEL level,
    __in DWORD dwLogId,
    __in HMODULE hModule,
    ...
    );

DAPI_(HRESULT) BalLogIdArgs(
    __in BOOTSTRAPPER_LOG_LEVEL level,
    __in DWORD dwLogId,
    __in HMODULE hModule,
    __in va_list args
    );

#ifdef __cplusplus
}
#endif
