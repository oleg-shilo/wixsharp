#pragma once
// Copyright (c) .NET Foundation and contributors. All rights reserved. Licensed under the Microsoft Reciprocal License. See LICENSE.TXT file in the project root for full license information.


#ifdef __cplusplus
class PSCZ
{
public:
    PSCZ() : m_scz(NULL) { }

    ~PSCZ() { ReleaseNullStr(m_scz); }

    operator LPWSTR() { return m_scz; }

    operator LPCWSTR() { return m_scz; }

    operator bool() { return NULL != m_scz; }

    LPWSTR* operator &() { return &m_scz; }

    bool operator !() { return !m_scz; }

    WCHAR operator *() { return *m_scz; }

    LPWSTR Detach() { LPWSTR scz = m_scz; m_scz = NULL; return scz; }

private:
    LPWSTR m_scz;
};
#endif  //__cplusplus
