#pragma once
// Copyright (c) .NET Foundation and contributors. All rights reserved. Licensed under the Microsoft Reciprocal License. See LICENSE.TXT file in the project root for full license information.


DECLARE_INTERFACE_IID_(IBootstrapperEngine, IUnknown, "6480D616-27A0-44D7-905B-81512C29C2FB")
{
    STDMETHOD(GetPackageCount)(
        __out DWORD* pcPackages
        ) = 0;

    STDMETHOD(GetVariableNumeric)(
        __in_z LPCWSTR wzVariable,
        __out LONGLONG* pllValue
        ) = 0;

    STDMETHOD(GetVariableString)(
        __in_z LPCWSTR wzVariable,
        __out_ecount_opt(*pcchValue) LPWSTR wzValue,
        __inout DWORD* pcchValue
        ) = 0;

    STDMETHOD(GetVariableVersion)(
        __in_z LPCWSTR wzVariable,
        __out DWORD64* pqwValue
        ) = 0;

    STDMETHOD(FormatString)(
        __in_z LPCWSTR wzIn,
        __out_ecount_opt(*pcchOut) LPWSTR wzOut,
        __inout DWORD* pcchOut
        ) = 0;

    STDMETHOD(EscapeString)(
        __in_z LPCWSTR wzIn,
        __out_ecount_opt(*pcchOut) LPWSTR wzOut,
        __inout DWORD* pcchOut
        ) = 0;

    STDMETHOD(EvaluateCondition)(
        __in_z LPCWSTR wzCondition,
        __out BOOL* pf
        ) = 0;

    STDMETHOD(Log)(
        __in BOOTSTRAPPER_LOG_LEVEL level,
        __in_z LPCWSTR wzMessage
        ) = 0;

    STDMETHOD(SendEmbeddedError)(
        __in DWORD dwErrorCode,
        __in_z_opt LPCWSTR wzMessage,
        __in DWORD dwUIHint,
        __out int* pnResult
        ) = 0;

    STDMETHOD(SendEmbeddedProgress)(
        __in DWORD dwProgressPercentage,
        __in DWORD dwOverallProgressPercentage,
        __out int* pnResult
        ) = 0;

    STDMETHOD(SetUpdate)(
        __in_z_opt LPCWSTR wzLocalSource,
        __in_z_opt LPCWSTR wzDownloadSource,
        __in DWORD64 qwSize,
        __in BOOTSTRAPPER_UPDATE_HASH_TYPE hashType,
        __in_bcount_opt(cbHash) BYTE* rgbHash,
        __in DWORD cbHash
        ) = 0;

    STDMETHOD(SetLocalSource)(
        __in_z LPCWSTR wzPackageOrContainerId,
        __in_z_opt LPCWSTR wzPayloadId,
        __in_z LPCWSTR wzPath
        ) = 0;

    STDMETHOD(SetDownloadSource)(
        __in_z LPCWSTR wzPackageOrContainerId,
        __in_z_opt LPCWSTR wzPayloadId,
        __in_z LPCWSTR wzUrl,
        __in_z_opt LPCWSTR wzUser,
        __in_z_opt LPCWSTR wzPassword
        ) = 0;

    STDMETHOD(SetVariableNumeric)(
        __in_z LPCWSTR wzVariable,
        __in LONGLONG llValue
        ) = 0;

    STDMETHOD(SetVariableString)(
        __in_z LPCWSTR wzVariable,
        __in_z_opt LPCWSTR wzValue
        ) = 0;

    STDMETHOD(SetVariableVersion)(
        __in_z LPCWSTR wzVariable,
        __in DWORD64 qwValue
        ) = 0;

    STDMETHOD(CloseSplashScreen)() = 0;

    STDMETHOD(Detect)(
        __in_opt HWND hwndParent = NULL
        ) = 0;

    STDMETHOD(Plan)(
        __in BOOTSTRAPPER_ACTION action
        ) = 0;

    STDMETHOD(Elevate)(
        __in_opt HWND hwndParent
        ) = 0;

    STDMETHOD(Apply)(
        __in_opt HWND hwndParent
        ) = 0;

    STDMETHOD(Quit)(
        __in DWORD dwExitCode
        ) = 0;

    STDMETHOD(LaunchApprovedExe)(
        __in_opt HWND hwndParent,
        __in_z LPCWSTR wzApprovedExeForElevationId,
        __in_z_opt LPCWSTR wzArguments,
        __in DWORD dwWaitForInputIdleTimeout
        ) = 0;
};
