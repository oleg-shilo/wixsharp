// Copyright (c) .NET Foundation and contributors. All rights reserved. Licensed under the Microsoft Reciprocal License. See LICENSE.TXT file in the project root for full license information.

#include <windows.h>
#include <msiquery.h>

#include "BootstrapperEngine.h"
#include "BootstrapperApplication.h"
#include "IBootstrapperEngine.h"
#include "IBootstrapperApplication.h"

#include "balutil.h"
#include "balretry.h"

class CBalBaseBootstrapperApplication : public IBootstrapperApplication
{
public: // IUnknown
    virtual STDMETHODIMP QueryInterface(
        __in REFIID riid,
        __out LPVOID *ppvObject
        )
    {
        if (!ppvObject)
        {
            return E_INVALIDARG;
        }

        *ppvObject = NULL;

        if (::IsEqualIID(__uuidof(IBootstrapperApplication), riid))
        {
            *ppvObject = static_cast<IBootstrapperApplication*>(this);
        }
        else if (::IsEqualIID(IID_IUnknown, riid))
        {
            *ppvObject = static_cast<IUnknown*>(this);
        }
        else // no interface for requested iid
        {
            return E_NOINTERFACE;
        }

        AddRef();
        return S_OK;
    }

    virtual STDMETHODIMP_(ULONG) AddRef()
    {
        return ::InterlockedIncrement(&this->m_cReferences);
    }

    virtual STDMETHODIMP_(ULONG) Release()
    {
        long l = ::InterlockedDecrement(&this->m_cReferences);
        if (0 < l)
        {
            return l;
        }

        delete this;
        return 0;
    }

public: // IBootstrapperApplication
    virtual STDMETHODIMP OnStartup()
    {
        return S_OK;
    }

    virtual STDMETHODIMP OnShutdown(
        __inout BOOTSTRAPPER_SHUTDOWN_ACTION* /*pAction*/
        )
    {
        return S_OK;
    }

    virtual STDMETHODIMP OnSystemShutdown(
        __in DWORD dwEndSession,
        __inout BOOL* pfCancel
        )
    {
        HRESULT hr = S_OK;

        // Allow requests to shut down when critical or not applying.
        *pfCancel = !(ENDSESSION_CRITICAL & dwEndSession || !m_fApplying);

        return hr;
    }

    virtual STDMETHODIMP OnDetectBegin(
        __in BOOL /*fInstalled*/,
        __in DWORD /*cPackages*/,
        __inout BOOL* pfCancel
        )
    {
        *pfCancel |= CheckCanceled();
        return S_OK;
    }

    virtual STDMETHODIMP OnDetectForwardCompatibleBundle(
        __in_z LPCWSTR /*wzBundleId*/,
        __in BOOTSTRAPPER_RELATION_TYPE /*relationType*/,
        __in_z LPCWSTR /*wzBundleTag*/,
        __in BOOL /*fPerMachine*/,
        __in DWORD64 /*dw64Version*/,
        __inout BOOL* pfCancel,
        __inout BOOL* /*pfIgnoreBundle*/
        )
    {
        *pfCancel |= CheckCanceled();
        return S_OK;
    }

    virtual STDMETHODIMP OnDetectUpdateBegin(
        __in_z LPCWSTR /*wzUpdateLocation*/,
        __inout BOOL* pfCancel,
        __inout BOOL* /*pfSkip*/
        )
    {
        *pfCancel |= CheckCanceled();
        return S_OK;
    }

    virtual STDMETHODIMP OnDetectUpdate(
        __in_z LPCWSTR /*wzUpdateLocation*/,
        __in DWORD64 /*dw64Size*/,
        __in DWORD64 /*dw64Version*/,
        __in_z LPCWSTR /*wzTitle*/,
        __in_z LPCWSTR /*wzSummary*/,
        __in_z LPCWSTR /*wzContentType*/,
        __in_z LPCWSTR /*wzContent*/,
        __inout BOOL* pfCancel,
        __inout BOOL* /*pfStopProcessingUpdates*/
        )
    {
        *pfCancel |= CheckCanceled();
        return S_OK;
    }

    virtual STDMETHODIMP OnDetectUpdateComplete(
        __in HRESULT /*hrStatus*/,
        __inout BOOL* /*pfIgnoreError*/
        )
    {
        return S_OK;
    }

    virtual STDMETHODIMP OnDetectRelatedBundle(
        __in_z LPCWSTR /*wzBundleId*/,
        __in BOOTSTRAPPER_RELATION_TYPE /*relationType*/,
        __in_z LPCWSTR /*wzBundleTag*/,
        __in BOOL /*fPerMachine*/,
        __in DWORD64 /*dw64Version*/,
        __in BOOTSTRAPPER_RELATED_OPERATION /*operation*/,
        __inout BOOL* pfCancel
        )
    {
        *pfCancel |= CheckCanceled();
        return S_OK;
    }

    virtual STDMETHODIMP OnDetectPackageBegin(
        __in_z LPCWSTR /*wzPackageId*/,
        __inout BOOL* pfCancel
        )
    {
        *pfCancel |= CheckCanceled();
        return S_OK;
    }

    virtual STDMETHODIMP OnDetectCompatibleMsiPackage(
        __in_z LPCWSTR /*wzPackageId*/,
        __in_z LPCWSTR /*wzCompatiblePackageId*/,
        __in DWORD64 /*dw64CompatiblePackageVersion*/,
        __inout BOOL* pfCancel
        )
    {
        *pfCancel |= CheckCanceled();
        return S_OK;
    }

    virtual STDMETHODIMP OnDetectRelatedMsiPackage(
        __in_z LPCWSTR /*wzPackageId*/,
        __in_z LPCWSTR /*wzUpgradeCode*/,
        __in_z LPCWSTR /*wzProductCode*/,
        __in BOOL /*fPerMachine*/,
        __in DWORD64 /*dw64Version*/,
        __in BOOTSTRAPPER_RELATED_OPERATION /*operation*/,
        __inout BOOL* pfCancel
        ) 
    {
        *pfCancel |= CheckCanceled();
        return S_OK;
    }

    virtual STDMETHODIMP OnDetectTargetMsiPackage(
        __in_z LPCWSTR /*wzPackageId*/,
        __in_z LPCWSTR /*wzProductCode*/,
        __in BOOTSTRAPPER_PACKAGE_STATE /*patchState*/,
        __inout BOOL* pfCancel
        )
    {
        *pfCancel |= CheckCanceled();
        return S_OK;
    }

    virtual STDMETHODIMP OnDetectMsiFeature(
        __in_z LPCWSTR /*wzPackageId*/,
        __in_z LPCWSTR /*wzFeatureId*/,
        __in BOOTSTRAPPER_FEATURE_STATE /*state*/,
        __inout BOOL* pfCancel
        )
    {
        *pfCancel |= CheckCanceled();
        return S_OK;
    }

    virtual STDMETHODIMP OnDetectPackageComplete(
        __in_z LPCWSTR /*wzPackageId*/,
        __in HRESULT /*hrStatus*/,
        __in BOOTSTRAPPER_PACKAGE_STATE /*state*/
        )
    {
        return S_OK;
    }

    virtual STDMETHODIMP OnDetectComplete(
        __in HRESULT /*hrStatus*/
        )
    {
        return S_OK;
    }

    virtual STDMETHODIMP OnPlanBegin(
        __in DWORD /*cPackages*/,
        __inout BOOL* pfCancel
        )
    {
        *pfCancel |= CheckCanceled();
        return S_OK;
    }

    virtual STDMETHODIMP OnPlanRelatedBundle(
        __in_z LPCWSTR /*wzBundleId*/,
        __in BOOTSTRAPPER_REQUEST_STATE /*recommendedState*/,
        __inout BOOTSTRAPPER_REQUEST_STATE* /*pRequestedState*/,
        __inout BOOL* pfCancel
        )
    {
        *pfCancel |= CheckCanceled();
        return S_OK;
    }

    virtual STDMETHODIMP OnPlanPackageBegin(
        __in_z LPCWSTR /*wzPackageId*/,
        __in BOOTSTRAPPER_REQUEST_STATE /*recommendedState*/,
        __inout BOOTSTRAPPER_REQUEST_STATE* /*pRequestState*/,
        __inout BOOL* pfCancel
        )
    {
        *pfCancel |= CheckCanceled();
        return S_OK;
    }

    virtual STDMETHODIMP OnPlanCompatibleMsiPackageBegin(
        __in_z LPCWSTR /*wzPackageId*/,
        __in_z LPCWSTR /*wzCompatiblePackageId*/,
        __in DWORD64 /*dw64CompatiblePackageVersion*/,
        __in BOOTSTRAPPER_REQUEST_STATE /*recommendedState*/,
        __inout BOOTSTRAPPER_REQUEST_STATE* /*pRequestedState*/,
        __inout BOOL* pfCancel
        )
    {
        *pfCancel |= CheckCanceled();
        return S_OK;
    }

    virtual STDMETHODIMP OnPlanCompatibleMsiPackageComplete(
        __in_z LPCWSTR /*wzPackageId*/,
        __in_z LPCWSTR /*wzCompatiblePackageId*/,
        __in HRESULT /*hrStatus*/,
        __in BOOTSTRAPPER_PACKAGE_STATE /*state*/,
        __in BOOTSTRAPPER_REQUEST_STATE /*requested*/,
        __in BOOTSTRAPPER_ACTION_STATE /*execute*/,
        __in BOOTSTRAPPER_ACTION_STATE /*rollback*/
        )
    {
        return S_OK;
    }

    virtual STDMETHODIMP OnPlanTargetMsiPackage(
        __in_z LPCWSTR /*wzPackageId*/,
        __in_z LPCWSTR /*wzProductCode*/,
        __in BOOTSTRAPPER_REQUEST_STATE /*recommendedState*/,
        __inout BOOTSTRAPPER_REQUEST_STATE* /*pRequestedState*/,
        __inout BOOL* pfCancel
        )
    {
        *pfCancel |= CheckCanceled();
        return S_OK;
    }

    virtual STDMETHODIMP OnPlanMsiFeature(
        __in_z LPCWSTR /*wzPackageId*/,
        __in_z LPCWSTR /*wzFeatureId*/,
        __in BOOTSTRAPPER_FEATURE_STATE /*recommendedState*/,
        __inout BOOTSTRAPPER_FEATURE_STATE* /*pRequestedState*/,
        __inout BOOL* pfCancel
        )
    {
        *pfCancel |= CheckCanceled();
        return S_OK;
    }

    virtual STDMETHODIMP OnPlanPackageComplete(
        __in_z LPCWSTR /*wzPackageId*/,
        __in HRESULT /*hrStatus*/,
        __in BOOTSTRAPPER_PACKAGE_STATE /*state*/,
        __in BOOTSTRAPPER_REQUEST_STATE /*requested*/,
        __in BOOTSTRAPPER_ACTION_STATE /*execute*/,
        __in BOOTSTRAPPER_ACTION_STATE /*rollback*/
        )
    {
        return S_OK;
    }

    virtual STDMETHODIMP OnPlanComplete(
        __in HRESULT /*hrStatus*/
        )
    {
        return S_OK;
    }

    virtual STDMETHODIMP OnApplyBegin(
        __in DWORD /*dwPhaseCount*/,
        __inout BOOL* pfCancel
        )
    {
        m_fApplying = TRUE;

        m_dwProgressPercentage = 0;
        m_dwOverallProgressPercentage = 0;

        *pfCancel |= CheckCanceled();
        return S_OK;
    }

    virtual STDMETHODIMP OnElevateBegin(
        __inout BOOL* pfCancel
        )
    {
        *pfCancel |= CheckCanceled();
        return S_OK;
    }

    virtual STDMETHODIMP OnElevateComplete(
        __in HRESULT /*hrStatus*/
        )
    {
        return S_OK;
    }

    virtual STDMETHODIMP OnProgress(
        __in DWORD dwProgressPercentage,
        __in DWORD dwOverallProgressPercentage,
        __inout BOOL* pfCancel
        )
    {
        HRESULT hr = S_OK;
        int nResult = IDNOACTION;

        m_dwProgressPercentage = dwProgressPercentage;
        m_dwOverallProgressPercentage = dwOverallProgressPercentage;

        if (BOOTSTRAPPER_DISPLAY_EMBEDDED == m_display)
        {
            hr = m_pEngine->SendEmbeddedProgress(m_dwProgressPercentage, m_dwOverallProgressPercentage, &nResult);
            BalExitOnFailure(hr, "Failed to send embedded overall progress.");

            if (IDERROR == nResult)
            {
                hr = E_FAIL;
            }
            else if (IDCANCEL == nResult)
            {
                *pfCancel = TRUE;
            }
        }

    LExit:
        *pfCancel |= CheckCanceled();
        return hr;
    }

    virtual STDMETHODIMP OnError(
        __in BOOTSTRAPPER_ERROR_TYPE errorType,
        __in_z LPCWSTR wzPackageId,
        __in DWORD dwCode,
        __in_z LPCWSTR /*wzError*/,
        __in DWORD /*dwUIHint*/,
        __in DWORD /*cData*/,
        __in_ecount_z_opt(cData) LPCWSTR* /*rgwzData*/,
        __in int /*nRecommendation*/,
        __inout int* pResult
        )
    {
        BalRetryErrorOccurred(wzPackageId, dwCode);

        if (CheckCanceled())
        {
            *pResult = IDCANCEL;
        }
        else if (BOOTSTRAPPER_DISPLAY_FULL == m_display)
        {
            if (BOOTSTRAPPER_ERROR_TYPE_HTTP_AUTH_SERVER == errorType || BOOTSTRAPPER_ERROR_TYPE_HTTP_AUTH_PROXY == errorType)
            {
                *pResult = IDTRYAGAIN;
            }
        }

        return S_OK;
    }

    virtual STDMETHODIMP OnRegisterBegin(
        __inout BOOL* pfCancel
        )
    {
        *pfCancel |= CheckCanceled();
        return S_OK;
    }

    virtual STDMETHODIMP OnRegisterComplete(
        __in HRESULT /*hrStatus*/
        )
    {
        return S_OK;
    }

    virtual STDMETHODIMP OnCacheBegin(
        __inout BOOL* pfCancel
        )
    {
        *pfCancel |= CheckCanceled();
        return S_OK;
    }

    virtual STDMETHODIMP OnCachePackageBegin(
        __in_z LPCWSTR /*wzPackageId*/,
        __in DWORD /*cCachePayloads*/,
        __in DWORD64 /*dw64PackageCacheSize*/,
        __inout BOOL* pfCancel
        )
    {
        *pfCancel |= CheckCanceled();
        return S_OK;
    }

    virtual STDMETHODIMP OnCacheAcquireBegin(
        __in_z LPCWSTR wzPackageOrContainerId,
        __in_z_opt LPCWSTR wzPayloadId,
        __in BOOTSTRAPPER_CACHE_OPERATION /*operation*/,
        __in_z LPCWSTR /*wzSource*/,
        __inout BOOL* pfCancel
        )
    {
        BalRetryStartPackage(BALRETRY_TYPE_CACHE, wzPackageOrContainerId, wzPayloadId);
        *pfCancel |= CheckCanceled();
        return S_OK;
    }

    virtual STDMETHODIMP OnCacheAcquireProgress(
        __in_z LPCWSTR /*wzPackageOrContainerId*/,
        __in_z_opt LPCWSTR /*wzPayloadId*/,
        __in DWORD64 /*dw64Progress*/,
        __in DWORD64 /*dw64Total*/,
        __in DWORD /*dwOverallPercentage*/,
        __inout BOOL* pfCancel
        )
    {
        HRESULT hr = S_OK;
        int nResult = IDNOACTION;

        // Send progress even though we don't update the numbers to at least give the caller an opportunity
        // to cancel.
        if (BOOTSTRAPPER_DISPLAY_EMBEDDED == m_display)
        {
            hr = m_pEngine->SendEmbeddedProgress(m_dwProgressPercentage, m_dwOverallProgressPercentage, &nResult);
            BalExitOnFailure(hr, "Failed to send embedded cache progress.");

            if (IDERROR == nResult)
            {
                hr = E_FAIL;
            }
            else if (IDCANCEL == nResult)
            {
                *pfCancel = TRUE;
            }
        }

    LExit:
        *pfCancel |= CheckCanceled();
        return hr;
    }

    virtual STDMETHODIMP OnResolveSource(
        __in_z LPCWSTR /*wzPackageOrContainerId*/,
        __in_z_opt LPCWSTR /*wzPayloadId*/,
        __in_z LPCWSTR /*wzLocalSource*/,
        __in_z_opt LPCWSTR /*wzDownloadSource*/,
        __in BOOTSTRAPPER_RESOLVESOURCE_ACTION /*recommendation*/,
        __inout BOOTSTRAPPER_RESOLVESOURCE_ACTION* /*pAction*/,
        __inout BOOL* pfCancel
        )
    {
        *pfCancel |= CheckCanceled();
        return S_OK;
    }

    virtual STDMETHODIMP OnCacheAcquireComplete(
        __in_z LPCWSTR wzPackageOrContainerId,
        __in_z_opt LPCWSTR wzPayloadId,
        __in HRESULT hrStatus,
        __in BOOTSTRAPPER_CACHEACQUIRECOMPLETE_ACTION /*recommendation*/,
        __inout BOOTSTRAPPER_CACHEACQUIRECOMPLETE_ACTION* pAction
        )
    {
        HRESULT hr = S_OK;
        BOOL fRetry = FALSE;

        if (CheckCanceled())
        {
            ExitFunction1(hr = HRESULT_FROM_WIN32(ERROR_INSTALL_USEREXIT));
        }

        hr = BalRetryEndPackage(BALRETRY_TYPE_CACHE, wzPackageOrContainerId, wzPayloadId, hrStatus, &fRetry);
        ExitOnFailure(hr, "BalRetryEndPackage for cache failed");

        if (fRetry)
        {
            *pAction = BOOTSTRAPPER_CACHEACQUIRECOMPLETE_ACTION_RETRY;
        }

    LExit:
        return hr;
    }

    virtual STDMETHODIMP OnCacheVerifyBegin(
        __in_z LPCWSTR /*wzPackageId*/,
        __in_z LPCWSTR /*wzPayloadId*/,
        __inout BOOL* pfCancel
        )
    {
        *pfCancel |= CheckCanceled();
        return S_OK;
    }

    virtual STDMETHODIMP OnCacheVerifyComplete(
        __in_z LPCWSTR /*wzPackageId*/,
        __in_z LPCWSTR /*wzPayloadId*/,
        __in HRESULT /*hrStatus*/,
        __in BOOTSTRAPPER_CACHEVERIFYCOMPLETE_ACTION /*recommendation*/,
        __inout BOOTSTRAPPER_CACHEVERIFYCOMPLETE_ACTION* pAction
        )
    {
        if (CheckCanceled())
        {
            *pAction = BOOTSTRAPPER_CACHEVERIFYCOMPLETE_ACTION_NONE;
        }

        return S_OK;
    }

    virtual STDMETHODIMP OnCachePackageComplete(
        __in_z LPCWSTR /*wzPackageId*/,
        __in HRESULT /*hrStatus*/,
        __in BOOTSTRAPPER_CACHEPACKAGECOMPLETE_ACTION /*recommendation*/,
        __inout BOOTSTRAPPER_CACHEPACKAGECOMPLETE_ACTION* pAction
        )
    {
        if (CheckCanceled())
        {
            *pAction = BOOTSTRAPPER_CACHEPACKAGECOMPLETE_ACTION_NONE;
        }

        return S_OK;
    }

    virtual STDMETHODIMP OnCacheComplete(
        __in HRESULT /*hrStatus*/
        )
    {
        return S_OK;
    }

    virtual STDMETHODIMP OnExecuteBegin(
        __in DWORD /*cExecutingPackages*/,
        __inout BOOL* pfCancel
        )
    {
        *pfCancel |= CheckCanceled();
        return S_OK;
    }

    virtual STDMETHODIMP OnExecutePackageBegin(
        __in_z LPCWSTR wzPackageId,
        __in BOOL fExecute,
        __inout BOOL* pfCancel
        )
    {
        // Only track retry on execution (not rollback).
        if (fExecute)
        {
            BalRetryStartPackage(BALRETRY_TYPE_EXECUTE, wzPackageId, NULL);
        }

        m_fRollingBack = !fExecute;
        *pfCancel |= CheckCanceled();
        return S_OK;
    }

    virtual STDMETHODIMP OnExecutePatchTarget(
        __in_z LPCWSTR /*wzPackageId*/,
        __in_z LPCWSTR /*wzTargetProductCode*/,
        __inout BOOL* pfCancel
        )
    {
        *pfCancel |= CheckCanceled();
        return S_OK;
    }

    virtual STDMETHODIMP OnExecuteProgress(
        __in_z LPCWSTR /*wzPackageId*/,
        __in DWORD /*dwProgressPercentage*/,
        __in DWORD /*dwOverallProgressPercentage*/,
        __inout BOOL* pfCancel
        )
    {
        HRESULT hr = S_OK;
        int nResult = IDNOACTION;

        // Send progress even though we don't update the numbers to at least give the caller an opportunity
        // to cancel.
        if (BOOTSTRAPPER_DISPLAY_EMBEDDED == m_display)
        {
            hr = m_pEngine->SendEmbeddedProgress(m_dwProgressPercentage, m_dwOverallProgressPercentage, &nResult);
            BalExitOnFailure(hr, "Failed to send embedded execute progress.");

            if (IDERROR == nResult)
            {
                hr = E_FAIL;
            }
            else if (IDCANCEL == nResult)
            {
                *pfCancel = TRUE;
            }
        }

    LExit:
        *pfCancel |= CheckCanceled();
        return hr;
    }

    virtual STDMETHODIMP OnExecuteMsiMessage(
        __in_z LPCWSTR /*wzPackageId*/,
        __in INSTALLMESSAGE /*messageType*/,
        __in DWORD /*dwUIHint*/,
        __in_z LPCWSTR /*wzMessage*/,
        __in DWORD /*cData*/,
        __in_ecount_z_opt(cData) LPCWSTR* /*rgwzData*/,
        __in int /*nRecommendation*/,
        __inout int* pResult
        )
    {
        if (CheckCanceled())
        {
            *pResult = IDCANCEL;
        }

        return S_OK;
    }

    virtual STDMETHODIMP OnExecuteFilesInUse(
        __in_z LPCWSTR /*wzPackageId*/,
        __in DWORD /*cFiles*/,
        __in_ecount_z(cFiles) LPCWSTR* /*rgwzFiles*/,
        __in int /*nRecommendation*/,
        __inout int* pResult
        )
    {
        if (CheckCanceled())
        {
            *pResult = IDCANCEL;
        }

        return S_OK;
    }

    virtual STDMETHODIMP OnExecutePackageComplete(
        __in_z LPCWSTR wzPackageId,
        __in HRESULT hrStatus,
        __in BOOTSTRAPPER_APPLY_RESTART /*restart*/,
        __in BOOTSTRAPPER_EXECUTEPACKAGECOMPLETE_ACTION /*recommendation*/,
        __inout BOOTSTRAPPER_EXECUTEPACKAGECOMPLETE_ACTION* pAction
        )
    {
        HRESULT hr = S_OK;
        BOOL fRetry = FALSE;

        if (CheckCanceled())
        {
            ExitFunction1(hr = HRESULT_FROM_WIN32(ERROR_INSTALL_USEREXIT));
        }

        hr = BalRetryEndPackage(BALRETRY_TYPE_EXECUTE, wzPackageId, NULL, hrStatus, &fRetry);
        ExitOnFailure(hr, "BalRetryEndPackage for execute failed");

        if (fRetry)
        {
            *pAction = BOOTSTRAPPER_EXECUTEPACKAGECOMPLETE_ACTION_RETRY;
        }

    LExit:
        return hr;
    }

    virtual STDMETHODIMP OnExecuteComplete(
        __in HRESULT /*hrStatus*/
        )
    {
        return S_OK;
    }

    virtual STDMETHODIMP OnUnregisterBegin(
        __inout BOOL* pfCancel
        )
    {
        *pfCancel |= CheckCanceled();
        return S_OK;
    }

    virtual STDMETHODIMP OnUnregisterComplete(
        __in HRESULT /*hrStatus*/
        )
    {
        return S_OK;
    }

    virtual STDMETHODIMP OnApplyComplete(
        __in HRESULT /*hrStatus*/,
        __in BOOTSTRAPPER_APPLY_RESTART restart,
        __in BOOTSTRAPPER_APPLYCOMPLETE_ACTION /*recommendation*/,
        __inout BOOTSTRAPPER_APPLYCOMPLETE_ACTION* pAction
        )
    {
        HRESULT hr = S_OK;
        BOOL fRestartRequired = BOOTSTRAPPER_APPLY_RESTART_REQUIRED == restart;
        BOOL fShouldBlockRestart = BOOTSTRAPPER_DISPLAY_FULL <= m_display && BOOTSTRAPPER_RESTART_PROMPT >= m_restart;

        if (fRestartRequired && !fShouldBlockRestart)
        {
            *pAction = BOOTSTRAPPER_APPLYCOMPLETE_ACTION_RESTART;
        }

        m_fApplying = FALSE;

        return hr;
    }

    virtual STDMETHODIMP OnLaunchApprovedExeBegin(
        __inout BOOL* pfCancel
        )
    {
        *pfCancel |= CheckCanceled();
        return S_OK;
    }

    virtual STDMETHODIMP OnLaunchApprovedExeComplete(
        __in HRESULT /*hrStatus*/,
        __in DWORD /*dwProcessId*/
        )
    {
        return S_OK;
    }

    virtual STDMETHODIMP_(HRESULT) BAProc(
        __in BOOTSTRAPPER_APPLICATION_MESSAGE /*message*/,
        __in const LPVOID /*pvArgs*/,
        __inout LPVOID /*pvResults*/,
        __in_opt LPVOID /*pvContext*/
        )
    {
        return E_NOTIMPL;
    }

    virtual STDMETHODIMP_(void) BAProcFallback(
        __in BOOTSTRAPPER_APPLICATION_MESSAGE /*message*/,
        __in const LPVOID /*pvArgs*/,
        __inout LPVOID /*pvResults*/,
        __inout HRESULT* /*phr*/,
        __in_opt LPVOID /*pvContext*/
        )
    {
    }

protected:
    //
    // PromptCancel - prompts the user to close (if not forced).
    //
    virtual BOOL PromptCancel(
        __in HWND hWnd,
        __in BOOL fForceCancel,
        __in_z_opt LPCWSTR wzMessage,
        __in_z_opt LPCWSTR wzCaption
        )
    {
        ::EnterCriticalSection(&m_csCanceled);

        // Only prompt the user to close if we have not canceled already.
        if (!m_fCanceled)
        {
            if (fForceCancel)
            {
                m_fCanceled = TRUE;
            }
            else
            {
                m_fCanceled = (IDYES == ::MessageBoxW(hWnd, wzMessage, wzCaption, MB_YESNO | MB_ICONEXCLAMATION));
            }
        }

        ::LeaveCriticalSection(&m_csCanceled);

        return m_fCanceled;
    }

    //
    // CheckCanceled - waits if the cancel dialog is up and checks to see if the user canceled the operation.
    //
    BOOL CheckCanceled()
    {
        ::EnterCriticalSection(&m_csCanceled);
        ::LeaveCriticalSection(&m_csCanceled);
        return m_fRollingBack ? FALSE : m_fCanceled;
    }

    BOOL IsRollingBack()
    {
        return m_fRollingBack;
    }

    BOOL IsCanceled()
    {
        return m_fCanceled;
    }

    CBalBaseBootstrapperApplication(
        __in IBootstrapperEngine* pEngine,
        __in const BOOTSTRAPPER_CREATE_ARGS* pArgs,
        __in DWORD dwRetryCount = 0,
        __in DWORD dwRetryTimeout = 1000
        )
    {
        m_cReferences = 1;
        m_display = pArgs->pCommand->display;
        m_restart = pArgs->pCommand->restart;

        pEngine->AddRef();
        m_pEngine = pEngine;

        ::InitializeCriticalSection(&m_csCanceled);
        m_fCanceled = FALSE;
        m_fApplying = FALSE;
        m_fRollingBack = FALSE;

        BalRetryInitialize(dwRetryCount, dwRetryTimeout);
    }

    virtual ~CBalBaseBootstrapperApplication()
    {
        BalRetryUninitialize();
        ::DeleteCriticalSection(&m_csCanceled);

        ReleaseNullObject(m_pEngine);
    }

protected:
    CRITICAL_SECTION m_csCanceled;
    BOOL m_fCanceled;

private:
    long m_cReferences;
    BOOTSTRAPPER_DISPLAY m_display;
    BOOTSTRAPPER_RESTART m_restart;
    IBootstrapperEngine* m_pEngine;

    BOOL m_fApplying;
    BOOL m_fRollingBack;

    DWORD m_dwProgressPercentage;
    DWORD m_dwOverallProgressPercentage;
};
