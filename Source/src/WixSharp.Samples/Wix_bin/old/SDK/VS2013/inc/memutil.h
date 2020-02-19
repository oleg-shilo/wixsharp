#pragma once
// Copyright (c) .NET Foundation and contributors. All rights reserved. Licensed under the Microsoft Reciprocal License. See LICENSE.TXT file in the project root for full license information.


#ifdef __cplusplus
extern "C" {
#endif

#define ReleaseMem(p) if (p) { MemFree(p); }
#define ReleaseNullMem(p) if (p) { MemFree(p); p = NULL; }

HRESULT DAPI MemInitialize();
void DAPI MemUninitialize();

LPVOID DAPI MemAlloc(
    __in SIZE_T cbSize,
    __in BOOL fZero
    );
LPVOID DAPI MemReAlloc(
    __in LPVOID pv,
    __in SIZE_T cbSize,
    __in BOOL fZero
    );
HRESULT DAPI MemReAllocSecure(
    __in LPVOID pv,
    __in SIZE_T cbSize,
    __in BOOL fZero,
    __deref_out LPVOID* ppvNew
    );
HRESULT DAPI MemAllocArray(
    __inout LPVOID* ppvArray,
    __in SIZE_T cbArrayType,
    __in DWORD dwItemCount
    );
HRESULT DAPI MemReAllocArray(
    __inout LPVOID* ppvArray,
    __in DWORD cArray,
    __in SIZE_T cbArrayType,
    __in DWORD dwNewItemCount
    );
HRESULT DAPI MemEnsureArraySize(
    __deref_out_bcount(cArray * cbArrayType) LPVOID* ppvArray,
    __in DWORD cArray,
    __in SIZE_T cbArrayType,
    __in DWORD dwGrowthCount
    );
HRESULT DAPI MemInsertIntoArray(
    __deref_out_bcount((cExistingArray + cInsertItems) * cbArrayType) LPVOID* ppvArray,
    __in DWORD dwInsertIndex,
    __in DWORD cInsertItems,
    __in DWORD cExistingArray,
    __in SIZE_T cbArrayType,
    __in DWORD dwGrowthCount
    );
void DAPI MemRemoveFromArray(
    __inout_bcount((cExistingArray) * cbArrayType) LPVOID pvArray,
    __in DWORD dwRemoveIndex,
    __in DWORD cRemoveItems,
    __in DWORD cExistingArray,
    __in SIZE_T cbArrayType,
    __in BOOL fPreserveOrder
    );
void DAPI MemArraySwapItems(
    __inout_bcount((cExistingArray) * cbArrayType) LPVOID pvArray,
    __in DWORD dwIndex1,
    __in DWORD dwIndex2,
    __in SIZE_T cbArrayType
    );

HRESULT DAPI MemFree(
    __in LPVOID pv
    );
SIZE_T DAPI MemSize(
    __in LPCVOID pv
    );

#ifdef __cplusplus
}
#endif

