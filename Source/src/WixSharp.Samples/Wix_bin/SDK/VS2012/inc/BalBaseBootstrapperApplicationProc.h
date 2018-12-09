#pragma once
// Copyright (c) .NET Foundation and contributors. All rights reserved. Licensed under the Microsoft Reciprocal License. See LICENSE.TXT file in the project root for full license information.


#include <windows.h>

#include "BootstrapperEngine.h"
#include "BootstrapperApplication.h"
#include "IBootstrapperEngine.h"
#include "IBootstrapperApplication.h"

static HRESULT BalBaseBAProcOnDetectBegin(
    __in IBootstrapperApplication* pBA,
    __in BA_ONDETECTBEGIN_ARGS* pArgs,
    __inout BA_ONDETECTBEGIN_RESULTS* pResults
    )
{
    return pBA->OnDetectBegin(pArgs->fInstalled, pArgs->cPackages, &pResults->fCancel);
}

static HRESULT BalBaseBAProcOnDetectComplete(
    __in IBootstrapperApplication* pBA,
    __in BA_ONDETECTCOMPLETE_ARGS* pArgs,
    __inout BA_ONDETECTCOMPLETE_RESULTS* /*pResults*/
    )
{
    return pBA->OnDetectComplete(pArgs->hrStatus);
}

static HRESULT BalBaseBAProcOnPlanBegin(
    __in IBootstrapperApplication* pBA,
    __in BA_ONPLANBEGIN_ARGS* pArgs,
    __inout BA_ONPLANBEGIN_RESULTS* pResults
    )
{
    return pBA->OnPlanBegin(pArgs->cPackages, &pResults->fCancel);
}

static HRESULT BalBaseBAProcOnPlanComplete(
    __in IBootstrapperApplication* pBA,
    __in BA_ONPLANCOMPLETE_ARGS* pArgs,
    __inout BA_ONPLANCOMPLETE_RESULTS* /*pResults*/
    )
{
    return pBA->OnPlanComplete(pArgs->hrStatus);
}

static HRESULT BalBaseBAProcOnStartup(
    __in IBootstrapperApplication* pBA,
    __in BA_ONSTARTUP_ARGS* /*pArgs*/,
    __inout BA_ONSTARTUP_RESULTS* /*pResults*/
    )
{
    return pBA->OnStartup();
}

static HRESULT BalBaseBAProcOnShutdown(
    __in IBootstrapperApplication* pBA,
    __in BA_ONSHUTDOWN_ARGS* /*pArgs*/,
    __inout BA_ONSHUTDOWN_RESULTS* pResults
    )
{
    return pBA->OnShutdown(&pResults->action);
}

static HRESULT BalBaseBAProcOnSystemShutdown(
    __in IBootstrapperApplication* pBA,
    __in BA_ONSYSTEMSHUTDOWN_ARGS* pArgs,
    __inout BA_ONSYSTEMSHUTDOWN_RESULTS* pResults
    )
{
    return pBA->OnSystemShutdown(pArgs->dwEndSession, &pResults->fCancel);
}

static HRESULT BalBaseBAProcOnDetectForwardCompatibleBundle(
    __in IBootstrapperApplication* pBA,
    __in BA_ONDETECTFORWARDCOMPATIBLEBUNDLE_ARGS* pArgs,
    __inout BA_ONDETECTFORWARDCOMPATIBLEBUNDLE_RESULTS* pResults
    )
{
    return pBA->OnDetectForwardCompatibleBundle(pArgs->wzBundleId, pArgs->relationType, pArgs->wzBundleTag, pArgs->fPerMachine, pArgs->dw64Version, &pResults->fCancel, &pResults->fIgnoreBundle);
}

static HRESULT BalBaseBAProcOnDetectUpdateBegin(
    __in IBootstrapperApplication* pBA,
    __in BA_ONDETECTUPDATEBEGIN_ARGS* pArgs,
    __inout BA_ONDETECTUPDATEBEGIN_RESULTS* pResults
    )
{
    return pBA->OnDetectUpdateBegin(pArgs->wzUpdateLocation, &pResults->fCancel, &pResults->fSkip);
}

static HRESULT BalBaseBAProcOnDetectUpdate(
    __in IBootstrapperApplication* pBA,
    __in BA_ONDETECTUPDATE_ARGS* pArgs,
    __inout BA_ONDETECTUPDATE_RESULTS* pResults
    )
{
    return pBA->OnDetectUpdate(pArgs->wzUpdateLocation, pArgs->dw64Size, pArgs->dw64Version, pArgs->wzTitle, pArgs->wzSummary, pArgs->wzContentType, pArgs->wzContent, &pResults->fCancel, &pResults->fStopProcessingUpdates);
}

static HRESULT BalBaseBAProcOnDetectUpdateComplete(
    __in IBootstrapperApplication* pBA,
    __in BA_ONDETECTUPDATECOMPLETE_ARGS* pArgs,
    __inout BA_ONDETECTUPDATECOMPLETE_RESULTS* pResults
    )
{
    return pBA->OnDetectUpdateComplete(pArgs->hrStatus, &pResults->fIgnoreError);
}

static HRESULT BalBaseBAProcOnDetectRelatedBundle(
    __in IBootstrapperApplication* pBA,
    __in BA_ONDETECTRELATEDBUNDLE_ARGS* pArgs,
    __inout BA_ONDETECTRELATEDBUNDLE_RESULTS* pResults
    )
{
    return pBA->OnDetectRelatedBundle(pArgs->wzBundleId, pArgs->relationType, pArgs->wzBundleTag, pArgs->fPerMachine, pArgs->dw64Version, pArgs->operation, &pResults->fCancel);
}

static HRESULT BalBaseBAProcOnDetectPackageBegin(
    __in IBootstrapperApplication* pBA,
    __in BA_ONDETECTPACKAGEBEGIN_ARGS* pArgs,
    __inout BA_ONDETECTPACKAGEBEGIN_RESULTS* pResults
    )
{
    return pBA->OnDetectPackageBegin(pArgs->wzPackageId, &pResults->fCancel);
}

static HRESULT BalBaseBAProcOnDetectCompatiblePackage(
    __in IBootstrapperApplication* pBA,
    __in BA_ONDETECTCOMPATIBLEMSIPACKAGE_ARGS* pArgs,
    __inout BA_ONDETECTCOMPATIBLEMSIPACKAGE_RESULTS* pResults
    )
{
    return pBA->OnDetectCompatibleMsiPackage(pArgs->wzPackageId, pArgs->wzCompatiblePackageId, pArgs->dw64CompatiblePackageVersion, &pResults->fCancel);
}

static HRESULT BalBaseBAProcOnDetectRelatedMsiPackage(
    __in IBootstrapperApplication* pBA,
    __in BA_ONDETECTRELATEDMSIPACKAGE_ARGS* pArgs,
    __inout BA_ONDETECTRELATEDMSIPACKAGE_RESULTS* pResults
    )
{
    return pBA->OnDetectRelatedMsiPackage(pArgs->wzPackageId, pArgs->wzUpgradeCode, pArgs->wzProductCode, pArgs->fPerMachine, pArgs->dw64Version, pArgs->operation, &pResults->fCancel);
}

static HRESULT BalBaseBAProcOnDetectTargetMsiPackage(
    __in IBootstrapperApplication* pBA,
    __in BA_ONDETECTTARGETMSIPACKAGE_ARGS* pArgs,
    __inout BA_ONDETECTTARGETMSIPACKAGE_RESULTS* pResults
    )
{
    return pBA->OnDetectTargetMsiPackage(pArgs->wzPackageId, pArgs->wzProductCode, pArgs->patchState, &pResults->fCancel);
}

static HRESULT BalBaseBAProcOnDetectMsiFeature(
    __in IBootstrapperApplication* pBA,
    __in BA_ONDETECTMSIFEATURE_ARGS* pArgs,
    __inout BA_ONDETECTMSIFEATURE_RESULTS* pResults
    )
{
    return pBA->OnDetectMsiFeature(pArgs->wzPackageId, pArgs->wzFeatureId, pArgs->state, &pResults->fCancel);
}

static HRESULT BalBaseBAProcOnDetectPackageComplete(
    __in IBootstrapperApplication* pBA,
    __in BA_ONDETECTPACKAGECOMPLETE_ARGS* pArgs,
    __inout BA_ONDETECTPACKAGECOMPLETE_RESULTS* /*pResults*/
    )
{
    return pBA->OnDetectPackageComplete(pArgs->wzPackageId, pArgs->hrStatus, pArgs->state);
}

static HRESULT BalBaseBAProcOnPlanRelatedBundle(
    __in IBootstrapperApplication* pBA,
    __in BA_ONPLANRELATEDBUNDLE_ARGS* pArgs,
    __inout BA_ONPLANRELATEDBUNDLE_RESULTS* pResults
    )
{
    return pBA->OnPlanRelatedBundle(pArgs->wzBundleId, pArgs->recommendedState, &pResults->requestedState, &pResults->fCancel);
}

static HRESULT BalBaseBAProcOnPlanPackageBegin(
    __in IBootstrapperApplication* pBA,
    __in BA_ONPLANPACKAGEBEGIN_ARGS* pArgs,
    __inout BA_ONPLANPACKAGEBEGIN_RESULTS* pResults
    )
{
    return pBA->OnPlanPackageBegin(pArgs->wzPackageId, pArgs->recommendedState, &pResults->requestedState, &pResults->fCancel);
}

static HRESULT BalBaseBAProcOnPlanCompatibleMsiPackageBegin(
    __in IBootstrapperApplication* pBA,
    __in BA_ONPLANCOMPATIBLEMSIPACKAGEBEGIN_ARGS* pArgs,
    __inout BA_ONPLANCOMPATIBLEMSIPACKAGEBEGIN_RESULTS* pResults
    )
{
    return pBA->OnPlanCompatibleMsiPackageBegin(pArgs->wzPackageId, pArgs->wzCompatiblePackageId, pArgs->dw64CompatiblePackageVersion, pArgs->recommendedState, &pResults->requestedState, &pResults->fCancel);
}

static HRESULT BalBaseBAProcOnPlanCompatibleMsiPackageComplete(
    __in IBootstrapperApplication* pBA,
    __in BA_ONPLANCOMPATIBLEMSIPACKAGECOMPLETE_ARGS* pArgs,
    __inout BA_ONPLANCOMPATIBLEMSIPACKAGECOMPLETE_RESULTS* /*pResults*/
    )
{
    return pBA->OnPlanCompatibleMsiPackageComplete(pArgs->wzPackageId, pArgs->wzCompatiblePackageId, pArgs->hrStatus, pArgs->state, pArgs->requested, pArgs->execute, pArgs->rollback);
}

static HRESULT BalBaseBAProcOnPlanTargetMsiPackage(
    __in IBootstrapperApplication* pBA,
    __in BA_ONPLANTARGETMSIPACKAGE_ARGS* pArgs,
    __inout BA_ONPLANTARGETMSIPACKAGE_RESULTS* pResults
    )
{
    return pBA->OnPlanTargetMsiPackage(pArgs->wzPackageId, pArgs->wzProductCode, pArgs->recommendedState, &pResults->requestedState, &pResults->fCancel);
}

static HRESULT BalBaseBAProcOnPlanMsiFeature(
    __in IBootstrapperApplication* pBA,
    __in BA_ONPLANMSIFEATURE_ARGS* pArgs,
    __inout BA_ONPLANMSIFEATURE_RESULTS* pResults
    )
{
    return pBA->OnPlanMsiFeature(pArgs->wzPackageId, pArgs->wzFeatureId, pArgs->recommendedState, &pResults->requestedState, &pResults->fCancel);
}

static HRESULT BalBaseBAProcOnPlanPackageComplete(
    __in IBootstrapperApplication* pBA,
    __in BA_ONPLANPACKAGECOMPLETE_ARGS* pArgs,
    __inout BA_ONPLANPACKAGECOMPLETE_RESULTS* /*pResults*/
    )
{
    return pBA->OnPlanPackageComplete(pArgs->wzPackageId, pArgs->hrStatus, pArgs->state, pArgs->requested, pArgs->execute, pArgs->rollback);
}

static HRESULT BalBaseBAProcOnApplyBegin(
    __in IBootstrapperApplication* pBA,
    __in BA_ONAPPLYBEGIN_ARGS* pArgs,
    __inout BA_ONAPPLYBEGIN_RESULTS* pResults
    )
{
    return pBA->OnApplyBegin(pArgs->dwPhaseCount, &pResults->fCancel);
}

static HRESULT BalBaseBAProcOnElevateBegin(
    __in IBootstrapperApplication* pBA,
    __in BA_ONELEVATEBEGIN_ARGS* /*pArgs*/,
    __inout BA_ONELEVATEBEGIN_RESULTS* pResults
    )
{
    return pBA->OnElevateBegin(&pResults->fCancel);
}

static HRESULT BalBaseBAProcOnElevateComplete(
    __in IBootstrapperApplication* pBA,
    __in BA_ONELEVATECOMPLETE_ARGS* pArgs,
    __inout BA_ONELEVATECOMPLETE_RESULTS* /*pResults*/
    )
{
    return pBA->OnElevateComplete(pArgs->hrStatus);
}

static HRESULT BalBaseBAProcOnProgress(
    __in IBootstrapperApplication* pBA,
    __in BA_ONPROGRESS_ARGS* pArgs,
    __inout BA_ONPROGRESS_RESULTS* pResults
    )
{
    return pBA->OnProgress(pArgs->dwProgressPercentage, pArgs->dwOverallPercentage, &pResults->fCancel);
}

static HRESULT BalBaseBAProcOnError(
    __in IBootstrapperApplication* pBA,
    __in BA_ONERROR_ARGS* pArgs,
    __inout BA_ONERROR_RESULTS* pResults
    )
{
    return pBA->OnError(pArgs->errorType, pArgs->wzPackageId, pArgs->dwCode, pArgs->wzError, pArgs->dwUIHint, pArgs->cData, pArgs->rgwzData, pArgs->nRecommendation, &pResults->nResult);
}

static HRESULT BalBaseBAProcOnRegisterBegin(
    __in IBootstrapperApplication* pBA,
    __in BA_ONREGISTERBEGIN_ARGS* /*pArgs*/,
    __inout BA_ONREGISTERBEGIN_RESULTS* pResults
    )
{
    return pBA->OnRegisterBegin(&pResults->fCancel);
}

static HRESULT BalBaseBAProcOnRegisterComplete(
    __in IBootstrapperApplication* pBA,
    __in BA_ONREGISTERCOMPLETE_ARGS* pArgs,
    __inout BA_ONREGISTERCOMPLETE_RESULTS* /*pResults*/
    )
{
    return pBA->OnRegisterComplete(pArgs->hrStatus);
}

static HRESULT BalBaseBAProcOnCacheBegin(
    __in IBootstrapperApplication* pBA,
    __in BA_ONCACHEBEGIN_ARGS* /*pArgs*/,
    __inout BA_ONCACHEBEGIN_RESULTS* pResults
    )
{
    return pBA->OnCacheBegin(&pResults->fCancel);
}

static HRESULT BalBaseBAProcOnCachePackageBegin(
    __in IBootstrapperApplication* pBA,
    __in BA_ONCACHEPACKAGEBEGIN_ARGS* pArgs,
    __inout BA_ONCACHEPACKAGEBEGIN_RESULTS* pResults
    )
{
    return pBA->OnCachePackageBegin(pArgs->wzPackageId, pArgs->cCachePayloads, pArgs->dw64PackageCacheSize, &pResults->fCancel);
}

static HRESULT BalBaseBAProcOnCacheAcquireBegin(
    __in IBootstrapperApplication* pBA,
    __in BA_ONCACHEACQUIREBEGIN_ARGS* pArgs,
    __inout BA_ONCACHEACQUIREBEGIN_RESULTS* pResults
    )
{
    return pBA->OnCacheAcquireBegin(pArgs->wzPackageOrContainerId, pArgs->wzPayloadId, pArgs->operation, pArgs->wzSource, &pResults->fCancel);
}

static HRESULT BalBaseBAProcOnCacheAcquireProgress(
    __in IBootstrapperApplication* pBA,
    __in BA_ONCACHEACQUIREPROGRESS_ARGS* pArgs,
    __inout BA_ONCACHEACQUIREPROGRESS_RESULTS* pResults
    )
{
    return pBA->OnCacheAcquireProgress(pArgs->wzPackageOrContainerId, pArgs->wzPayloadId, pArgs->dw64Progress, pArgs->dw64Total, pArgs->dwOverallPercentage, &pResults->fCancel);
}

static HRESULT BalBaseBAProcOnResolveSource(
    __in IBootstrapperApplication* pBA,
    __in BA_ONRESOLVESOURCE_ARGS* pArgs,
    __inout BA_ONRESOLVESOURCE_RESULTS* pResults
    )
{
    return pBA->OnResolveSource(pArgs->wzPackageOrContainerId, pArgs->wzPayloadId, pArgs->wzLocalSource, pArgs->wzDownloadSource, pArgs->recommendation, &pResults->action, &pResults->fCancel);
}

static HRESULT BalBaseBAProcOnCacheAcquireComplete(
    __in IBootstrapperApplication* pBA,
    __in BA_ONCACHEACQUIRECOMPLETE_ARGS* pArgs,
    __inout BA_ONCACHEACQUIRECOMPLETE_RESULTS* pResults
    )
{
    return pBA->OnCacheAcquireComplete(pArgs->wzPackageOrContainerId, pArgs->wzPayloadId, pArgs->hrStatus, pArgs->recommendation, &pResults->action);
}

static HRESULT BalBaseBAProcOnCacheVerifyBegin(
    __in IBootstrapperApplication* pBA,
    __in BA_ONCACHEVERIFYBEGIN_ARGS* pArgs,
    __inout BA_ONCACHEVERIFYBEGIN_RESULTS* pResults
    )
{
    return pBA->OnCacheVerifyBegin(pArgs->wzPackageOrContainerId, pArgs->wzPayloadId, &pResults->fCancel);
}

static HRESULT BalBaseBAProcOnCacheVerifyComplete(
    __in IBootstrapperApplication* pBA,
    __in BA_ONCACHEVERIFYCOMPLETE_ARGS* pArgs,
    __inout BA_ONCACHEVERIFYCOMPLETE_RESULTS* pResults
    )
{
    return pBA->OnCacheVerifyComplete(pArgs->wzPackageOrContainerId, pArgs->wzPayloadId, pArgs->hrStatus, pArgs->recommendation, &pResults->action);
}

static HRESULT BalBaseBAProcOnCachePackageComplete(
    __in IBootstrapperApplication* pBA,
    __in BA_ONCACHEPACKAGECOMPLETE_ARGS* pArgs,
    __inout BA_ONCACHEPACKAGECOMPLETE_RESULTS* pResults
    )
{
    return pBA->OnCachePackageComplete(pArgs->wzPackageId, pArgs->hrStatus, pArgs->recommendation, &pResults->action);
}

static HRESULT BalBaseBAProcOnCacheComplete(
    __in IBootstrapperApplication* pBA,
    __in BA_ONCACHECOMPLETE_ARGS* pArgs,
    __inout BA_ONCACHECOMPLETE_RESULTS* /*pResults*/
    )
{
    return pBA->OnCacheComplete(pArgs->hrStatus);
}

static HRESULT BalBaseBAProcOnExecuteBegin(
    __in IBootstrapperApplication* pBA,
    __in BA_ONEXECUTEBEGIN_ARGS* pArgs,
    __inout BA_ONEXECUTEBEGIN_RESULTS* pResults
    )
{
    return pBA->OnExecuteBegin(pArgs->cExecutingPackages, &pResults->fCancel);
}

static HRESULT BalBaseBAProcOnExecutePackageBegin(
    __in IBootstrapperApplication* pBA,
    __in BA_ONEXECUTEPACKAGEBEGIN_ARGS* pArgs,
    __inout BA_ONEXECUTEPACKAGEBEGIN_RESULTS* pResults
    )
{
    return pBA->OnExecutePackageBegin(pArgs->wzPackageId, pArgs->fExecute, &pResults->fCancel);
}

static HRESULT BalBaseBAProcOnExecutePatchTarget(
    __in IBootstrapperApplication* pBA,
    __in BA_ONEXECUTEPATCHTARGET_ARGS* pArgs,
    __inout BA_ONEXECUTEPATCHTARGET_RESULTS* pResults
    )
{
    return pBA->OnExecutePatchTarget(pArgs->wzPackageId, pArgs->wzTargetProductCode, &pResults->fCancel);
}

static HRESULT BalBaseBAProcOnExecuteProgress(
    __in IBootstrapperApplication* pBA,
    __in BA_ONEXECUTEPROGRESS_ARGS* pArgs,
    __inout BA_ONEXECUTEPROGRESS_RESULTS* pResults
    )
{
    return pBA->OnExecuteProgress(pArgs->wzPackageId, pArgs->dwProgressPercentage, pArgs->dwOverallPercentage, &pResults->fCancel);
}

static HRESULT BalBaseBAProcOnExecuteMsiMessage(
    __in IBootstrapperApplication* pBA,
    __in BA_ONEXECUTEMSIMESSAGE_ARGS* pArgs,
    __inout BA_ONEXECUTEMSIMESSAGE_RESULTS* pResults
    )
{
    return pBA->OnExecuteMsiMessage(pArgs->wzPackageId, pArgs->messageType, pArgs->dwUIHint, pArgs->wzMessage, pArgs->cData, pArgs->rgwzData, pArgs->nRecommendation, &pResults->nResult);
}

static HRESULT BalBaseBAProcOnExecuteFilesInUse(
    __in IBootstrapperApplication* pBA,
    __in BA_ONEXECUTEFILESINUSE_ARGS* pArgs,
    __inout BA_ONEXECUTEFILESINUSE_RESULTS* pResults
    )
{
    return pBA->OnExecuteFilesInUse(pArgs->wzPackageId, pArgs->cFiles, pArgs->rgwzFiles, pArgs->nRecommendation, &pResults->nResult);
}

static HRESULT BalBaseBAProcOnExecutePackageComplete(
    __in IBootstrapperApplication* pBA,
    __in BA_ONEXECUTEPACKAGECOMPLETE_ARGS* pArgs,
    __inout BA_ONEXECUTEPACKAGECOMPLETE_RESULTS* pResults
    )
{
    return pBA->OnExecutePackageComplete(pArgs->wzPackageId, pArgs->hrStatus, pArgs->restart, pArgs->recommendation, &pResults->action);
}

static HRESULT BalBaseBAProcOnExecuteComplete(
    __in IBootstrapperApplication* pBA,
    __in BA_ONEXECUTECOMPLETE_ARGS* pArgs,
    __inout BA_ONEXECUTECOMPLETE_RESULTS* /*pResults*/
    )
{
    return pBA->OnExecuteComplete(pArgs->hrStatus);
}

static HRESULT BalBaseBAProcOnUnregisterBegin(
    __in IBootstrapperApplication* pBA,
    __in BA_ONUNREGISTERBEGIN_ARGS* /*pArgs*/,
    __inout BA_ONUNREGISTERBEGIN_RESULTS* pResults
    )
{
    return pBA->OnUnregisterBegin(&pResults->fCancel);
}

static HRESULT BalBaseBAProcOnUnregisterComplete(
    __in IBootstrapperApplication* pBA,
    __in BA_ONUNREGISTERCOMPLETE_ARGS* pArgs,
    __inout BA_ONUNREGISTERCOMPLETE_RESULTS* /*pResults*/
    )
{
    return pBA->OnUnregisterComplete(pArgs->hrStatus);
}

static HRESULT BalBaseBAProcOnApplyComplete(
    __in IBootstrapperApplication* pBA,
    __in BA_ONAPPLYCOMPLETE_ARGS* pArgs,
    __inout BA_ONAPPLYCOMPLETE_RESULTS* pResults
    )
{
    return pBA->OnApplyComplete(pArgs->hrStatus, pArgs->restart, pArgs->recommendation, &pResults->action);
}

static HRESULT BalBaseBAProcOnLaunchApprovedExeBegin(
    __in IBootstrapperApplication* pBA,
    __in BA_ONLAUNCHAPPROVEDEXEBEGIN_ARGS* /*pArgs*/,
    __inout BA_ONLAUNCHAPPROVEDEXEBEGIN_RESULTS* pResults
    )
{
    return pBA->OnLaunchApprovedExeBegin(&pResults->fCancel);
}

static HRESULT BalBaseBAProcOnLaunchApprovedExeComplete(
    __in IBootstrapperApplication* pBA,
    __in BA_ONLAUNCHAPPROVEDEXECOMPLETE_ARGS* pArgs,
    __inout BA_ONLAUNCHAPPROVEDEXECOMPLETE_RESULTS* /*pResults*/
    )
{
    return pBA->OnLaunchApprovedExeComplete(pArgs->hrStatus, pArgs->dwProcessId);
}

/*******************************************************************
BalBaseBootstrapperApplicationProc - requires pvContext to be of type IBootstrapperApplication.
                                     Provides a default mapping between the new message based BA interface and
                                     the old COM-based BA interface.

*******************************************************************/
static HRESULT WINAPI BalBaseBootstrapperApplicationProc(
    __in BOOTSTRAPPER_APPLICATION_MESSAGE message,
    __in const LPVOID pvArgs,
    __inout LPVOID pvResults,
    __in_opt LPVOID pvContext
    )
{
    IBootstrapperApplication* pBA = reinterpret_cast<IBootstrapperApplication*>(pvContext);
    HRESULT hr = pBA->BAProc(message, pvArgs, pvResults, pvContext);
    
    if (E_NOTIMPL == hr)
    {
        switch (message)
        {
        case BOOTSTRAPPER_APPLICATION_MESSAGE_ONDETECTBEGIN:
            hr = BalBaseBAProcOnDetectBegin(pBA, reinterpret_cast<BA_ONDETECTBEGIN_ARGS*>(pvArgs), reinterpret_cast<BA_ONDETECTBEGIN_RESULTS*>(pvResults));
            break;
        case BOOTSTRAPPER_APPLICATION_MESSAGE_ONDETECTCOMPLETE:
            hr = BalBaseBAProcOnDetectComplete(pBA, reinterpret_cast<BA_ONDETECTCOMPLETE_ARGS*>(pvArgs), reinterpret_cast<BA_ONDETECTCOMPLETE_RESULTS*>(pvResults));
            break;
        case BOOTSTRAPPER_APPLICATION_MESSAGE_ONPLANBEGIN:
            hr = BalBaseBAProcOnPlanBegin(pBA, reinterpret_cast<BA_ONPLANBEGIN_ARGS*>(pvArgs), reinterpret_cast<BA_ONPLANBEGIN_RESULTS*>(pvResults));
            break;
        case BOOTSTRAPPER_APPLICATION_MESSAGE_ONPLANCOMPLETE:
            hr = BalBaseBAProcOnPlanComplete(pBA, reinterpret_cast<BA_ONPLANCOMPLETE_ARGS*>(pvArgs), reinterpret_cast<BA_ONPLANCOMPLETE_RESULTS*>(pvResults));
            break;
        case BOOTSTRAPPER_APPLICATION_MESSAGE_ONSTARTUP:
            hr = BalBaseBAProcOnStartup(pBA, reinterpret_cast<BA_ONSTARTUP_ARGS*>(pvArgs), reinterpret_cast<BA_ONSTARTUP_RESULTS*>(pvResults));
            break;
        case BOOTSTRAPPER_APPLICATION_MESSAGE_ONSHUTDOWN:
            hr = BalBaseBAProcOnShutdown(pBA, reinterpret_cast<BA_ONSHUTDOWN_ARGS*>(pvArgs), reinterpret_cast<BA_ONSHUTDOWN_RESULTS*>(pvResults));
            break;
        case BOOTSTRAPPER_APPLICATION_MESSAGE_ONSYSTEMSHUTDOWN:
            hr = BalBaseBAProcOnSystemShutdown(pBA, reinterpret_cast<BA_ONSYSTEMSHUTDOWN_ARGS*>(pvArgs), reinterpret_cast<BA_ONSYSTEMSHUTDOWN_RESULTS*>(pvResults));
            break;
        case BOOTSTRAPPER_APPLICATION_MESSAGE_ONDETECTFORWARDCOMPATIBLEBUNDLE:
            hr = BalBaseBAProcOnDetectForwardCompatibleBundle(pBA, reinterpret_cast<BA_ONDETECTFORWARDCOMPATIBLEBUNDLE_ARGS*>(pvArgs), reinterpret_cast<BA_ONDETECTFORWARDCOMPATIBLEBUNDLE_RESULTS*>(pvResults));
            break;
        case BOOTSTRAPPER_APPLICATION_MESSAGE_ONDETECTUPDATEBEGIN:
            hr = BalBaseBAProcOnDetectUpdateBegin(pBA, reinterpret_cast<BA_ONDETECTUPDATEBEGIN_ARGS*>(pvArgs), reinterpret_cast<BA_ONDETECTUPDATEBEGIN_RESULTS*>(pvResults));
            break;
        case BOOTSTRAPPER_APPLICATION_MESSAGE_ONDETECTUPDATE:
            hr = BalBaseBAProcOnDetectUpdate(pBA, reinterpret_cast<BA_ONDETECTUPDATE_ARGS*>(pvArgs), reinterpret_cast<BA_ONDETECTUPDATE_RESULTS*>(pvResults));
            break;
        case BOOTSTRAPPER_APPLICATION_MESSAGE_ONDETECTUPDATECOMPLETE:
            hr = BalBaseBAProcOnDetectUpdateComplete(pBA, reinterpret_cast<BA_ONDETECTUPDATECOMPLETE_ARGS*>(pvArgs), reinterpret_cast<BA_ONDETECTUPDATECOMPLETE_RESULTS*>(pvResults));
            break;
        case BOOTSTRAPPER_APPLICATION_MESSAGE_ONDETECTRELATEDBUNDLE:
            hr = BalBaseBAProcOnDetectRelatedBundle(pBA, reinterpret_cast<BA_ONDETECTRELATEDBUNDLE_ARGS*>(pvArgs), reinterpret_cast<BA_ONDETECTRELATEDBUNDLE_RESULTS*>(pvResults));
            break;
        case BOOTSTRAPPER_APPLICATION_MESSAGE_ONDETECTPACKAGEBEGIN:
            hr = BalBaseBAProcOnDetectPackageBegin(pBA, reinterpret_cast<BA_ONDETECTPACKAGEBEGIN_ARGS*>(pvArgs), reinterpret_cast<BA_ONDETECTPACKAGEBEGIN_RESULTS*>(pvResults));
            break;
        case BOOTSTRAPPER_APPLICATION_MESSAGE_ONDETECTCOMPATIBLEMSIPACKAGE:
            hr = BalBaseBAProcOnDetectCompatiblePackage(pBA, reinterpret_cast<BA_ONDETECTCOMPATIBLEMSIPACKAGE_ARGS*>(pvArgs), reinterpret_cast<BA_ONDETECTCOMPATIBLEMSIPACKAGE_RESULTS*>(pvResults));
            break;
        case BOOTSTRAPPER_APPLICATION_MESSAGE_ONDETECTRELATEDMSIPACKAGE:
            hr = BalBaseBAProcOnDetectRelatedMsiPackage(pBA, reinterpret_cast<BA_ONDETECTRELATEDMSIPACKAGE_ARGS*>(pvArgs), reinterpret_cast<BA_ONDETECTRELATEDMSIPACKAGE_RESULTS*>(pvResults));
            break;
        case BOOTSTRAPPER_APPLICATION_MESSAGE_ONDETECTTARGETMSIPACKAGE:
            hr = BalBaseBAProcOnDetectTargetMsiPackage(pBA, reinterpret_cast<BA_ONDETECTTARGETMSIPACKAGE_ARGS*>(pvArgs), reinterpret_cast<BA_ONDETECTTARGETMSIPACKAGE_RESULTS*>(pvResults));
            break;
        case BOOTSTRAPPER_APPLICATION_MESSAGE_ONDETECTMSIFEATURE:
            hr = BalBaseBAProcOnDetectMsiFeature(pBA, reinterpret_cast<BA_ONDETECTMSIFEATURE_ARGS*>(pvArgs), reinterpret_cast<BA_ONDETECTMSIFEATURE_RESULTS*>(pvResults));
            break;
        case BOOTSTRAPPER_APPLICATION_MESSAGE_ONDETECTPACKAGECOMPLETE:
            hr = BalBaseBAProcOnDetectPackageComplete(pBA, reinterpret_cast<BA_ONDETECTPACKAGECOMPLETE_ARGS*>(pvArgs), reinterpret_cast<BA_ONDETECTPACKAGECOMPLETE_RESULTS*>(pvResults));
            break;
        case BOOTSTRAPPER_APPLICATION_MESSAGE_ONPLANRELATEDBUNDLE:
            hr = BalBaseBAProcOnPlanRelatedBundle(pBA, reinterpret_cast<BA_ONPLANRELATEDBUNDLE_ARGS*>(pvArgs), reinterpret_cast<BA_ONPLANRELATEDBUNDLE_RESULTS*>(pvResults));
            break;
        case BOOTSTRAPPER_APPLICATION_MESSAGE_ONPLANPACKAGEBEGIN:
            hr = BalBaseBAProcOnPlanPackageBegin(pBA, reinterpret_cast<BA_ONPLANPACKAGEBEGIN_ARGS*>(pvArgs), reinterpret_cast<BA_ONPLANPACKAGEBEGIN_RESULTS*>(pvResults));
            break;
        case BOOTSTRAPPER_APPLICATION_MESSAGE_ONPLANCOMPATIBLEMSIPACKAGEBEGIN:
            hr = BalBaseBAProcOnPlanCompatibleMsiPackageBegin(pBA, reinterpret_cast<BA_ONPLANCOMPATIBLEMSIPACKAGEBEGIN_ARGS*>(pvArgs), reinterpret_cast<BA_ONPLANCOMPATIBLEMSIPACKAGEBEGIN_RESULTS*>(pvResults));
            break;
        case BOOTSTRAPPER_APPLICATION_MESSAGE_ONPLANCOMPATIBLEMSIPACKAGECOMPLETE:
            hr = BalBaseBAProcOnPlanCompatibleMsiPackageComplete(pBA, reinterpret_cast<BA_ONPLANCOMPATIBLEMSIPACKAGECOMPLETE_ARGS*>(pvArgs), reinterpret_cast<BA_ONPLANCOMPATIBLEMSIPACKAGECOMPLETE_RESULTS*>(pvResults));
            break;
        case BOOTSTRAPPER_APPLICATION_MESSAGE_ONPLANTARGETMSIPACKAGE:
            hr = BalBaseBAProcOnPlanTargetMsiPackage(pBA, reinterpret_cast<BA_ONPLANTARGETMSIPACKAGE_ARGS*>(pvArgs), reinterpret_cast<BA_ONPLANTARGETMSIPACKAGE_RESULTS*>(pvResults));
            break;
        case BOOTSTRAPPER_APPLICATION_MESSAGE_ONPLANMSIFEATURE:
            hr = BalBaseBAProcOnPlanMsiFeature(pBA, reinterpret_cast<BA_ONPLANMSIFEATURE_ARGS*>(pvArgs), reinterpret_cast<BA_ONPLANMSIFEATURE_RESULTS*>(pvResults));
            break;
        case BOOTSTRAPPER_APPLICATION_MESSAGE_ONPLANPACKAGECOMPLETE:
            hr = BalBaseBAProcOnPlanPackageComplete(pBA, reinterpret_cast<BA_ONPLANPACKAGECOMPLETE_ARGS*>(pvArgs), reinterpret_cast<BA_ONPLANPACKAGECOMPLETE_RESULTS*>(pvResults));
            break;
        case BOOTSTRAPPER_APPLICATION_MESSAGE_ONAPPLYBEGIN:
            hr = BalBaseBAProcOnApplyBegin(pBA, reinterpret_cast<BA_ONAPPLYBEGIN_ARGS*>(pvArgs), reinterpret_cast<BA_ONAPPLYBEGIN_RESULTS*>(pvResults));
            break;
        case BOOTSTRAPPER_APPLICATION_MESSAGE_ONELEVATEBEGIN:
            hr = BalBaseBAProcOnElevateBegin(pBA, reinterpret_cast<BA_ONELEVATEBEGIN_ARGS*>(pvArgs), reinterpret_cast<BA_ONELEVATEBEGIN_RESULTS*>(pvResults));
            break;
        case BOOTSTRAPPER_APPLICATION_MESSAGE_ONELEVATECOMPLETE:
            hr = BalBaseBAProcOnElevateComplete(pBA, reinterpret_cast<BA_ONELEVATECOMPLETE_ARGS*>(pvArgs), reinterpret_cast<BA_ONELEVATECOMPLETE_RESULTS*>(pvResults));
            break;
        case BOOTSTRAPPER_APPLICATION_MESSAGE_ONPROGRESS:
            hr = BalBaseBAProcOnProgress(pBA, reinterpret_cast<BA_ONPROGRESS_ARGS*>(pvArgs), reinterpret_cast<BA_ONPROGRESS_RESULTS*>(pvResults));
            break;
        case BOOTSTRAPPER_APPLICATION_MESSAGE_ONERROR:
            hr = BalBaseBAProcOnError(pBA, reinterpret_cast<BA_ONERROR_ARGS*>(pvArgs), reinterpret_cast<BA_ONERROR_RESULTS*>(pvResults));
            break;
        case BOOTSTRAPPER_APPLICATION_MESSAGE_ONREGISTERBEGIN:
            hr = BalBaseBAProcOnRegisterBegin(pBA, reinterpret_cast<BA_ONREGISTERBEGIN_ARGS*>(pvArgs), reinterpret_cast<BA_ONREGISTERBEGIN_RESULTS*>(pvResults));
            break;
        case BOOTSTRAPPER_APPLICATION_MESSAGE_ONREGISTERCOMPLETE:
            hr = BalBaseBAProcOnRegisterComplete(pBA, reinterpret_cast<BA_ONREGISTERCOMPLETE_ARGS*>(pvArgs), reinterpret_cast<BA_ONREGISTERCOMPLETE_RESULTS*>(pvResults));
            break;
        case BOOTSTRAPPER_APPLICATION_MESSAGE_ONCACHEBEGIN:
            hr = BalBaseBAProcOnCacheBegin(pBA, reinterpret_cast<BA_ONCACHEBEGIN_ARGS*>(pvArgs), reinterpret_cast<BA_ONCACHEBEGIN_RESULTS*>(pvResults));
            break;
        case BOOTSTRAPPER_APPLICATION_MESSAGE_ONCACHEPACKAGEBEGIN:
            hr = BalBaseBAProcOnCachePackageBegin(pBA, reinterpret_cast<BA_ONCACHEPACKAGEBEGIN_ARGS*>(pvArgs), reinterpret_cast<BA_ONCACHEPACKAGEBEGIN_RESULTS*>(pvResults));
            break;
        case BOOTSTRAPPER_APPLICATION_MESSAGE_ONCACHEACQUIREBEGIN:
            hr = BalBaseBAProcOnCacheAcquireBegin(pBA, reinterpret_cast<BA_ONCACHEACQUIREBEGIN_ARGS*>(pvArgs), reinterpret_cast<BA_ONCACHEACQUIREBEGIN_RESULTS*>(pvResults));
            break;
        case BOOTSTRAPPER_APPLICATION_MESSAGE_ONCACHEACQUIREPROGRESS:
            hr = BalBaseBAProcOnCacheAcquireProgress(pBA, reinterpret_cast<BA_ONCACHEACQUIREPROGRESS_ARGS*>(pvArgs), reinterpret_cast<BA_ONCACHEACQUIREPROGRESS_RESULTS*>(pvResults));
            break;
        case BOOTSTRAPPER_APPLICATION_MESSAGE_ONRESOLVESOURCE:
            hr = BalBaseBAProcOnResolveSource(pBA, reinterpret_cast<BA_ONRESOLVESOURCE_ARGS*>(pvArgs), reinterpret_cast<BA_ONRESOLVESOURCE_RESULTS*>(pvResults));
            break;
        case BOOTSTRAPPER_APPLICATION_MESSAGE_ONCACHEACQUIRECOMPLETE:
            hr = BalBaseBAProcOnCacheAcquireComplete(pBA, reinterpret_cast<BA_ONCACHEACQUIRECOMPLETE_ARGS*>(pvArgs), reinterpret_cast<BA_ONCACHEACQUIRECOMPLETE_RESULTS*>(pvResults));
            break;
        case BOOTSTRAPPER_APPLICATION_MESSAGE_ONCACHEVERIFYBEGIN:
            hr = BalBaseBAProcOnCacheVerifyBegin(pBA, reinterpret_cast<BA_ONCACHEVERIFYBEGIN_ARGS*>(pvArgs), reinterpret_cast<BA_ONCACHEVERIFYBEGIN_RESULTS*>(pvResults));
            break;
        case BOOTSTRAPPER_APPLICATION_MESSAGE_ONCACHEVERIFYCOMPLETE:
            hr = BalBaseBAProcOnCacheVerifyComplete(pBA, reinterpret_cast<BA_ONCACHEVERIFYCOMPLETE_ARGS*>(pvArgs), reinterpret_cast<BA_ONCACHEVERIFYCOMPLETE_RESULTS*>(pvResults));
            break;
        case BOOTSTRAPPER_APPLICATION_MESSAGE_ONCACHEPACKAGECOMPLETE:
            hr = BalBaseBAProcOnCachePackageComplete(pBA, reinterpret_cast<BA_ONCACHEPACKAGECOMPLETE_ARGS*>(pvArgs), reinterpret_cast<BA_ONCACHEPACKAGECOMPLETE_RESULTS*>(pvResults));
            break;
        case BOOTSTRAPPER_APPLICATION_MESSAGE_ONCACHECOMPLETE:
            hr = BalBaseBAProcOnCacheComplete(pBA, reinterpret_cast<BA_ONCACHECOMPLETE_ARGS*>(pvArgs), reinterpret_cast<BA_ONCACHECOMPLETE_RESULTS*>(pvResults));
            break;
        case BOOTSTRAPPER_APPLICATION_MESSAGE_ONEXECUTEBEGIN:
            hr = BalBaseBAProcOnExecuteBegin(pBA, reinterpret_cast<BA_ONEXECUTEBEGIN_ARGS*>(pvArgs), reinterpret_cast<BA_ONEXECUTEBEGIN_RESULTS*>(pvResults));
            break;
        case BOOTSTRAPPER_APPLICATION_MESSAGE_ONEXECUTEPACKAGEBEGIN:
            hr = BalBaseBAProcOnExecutePackageBegin(pBA, reinterpret_cast<BA_ONEXECUTEPACKAGEBEGIN_ARGS*>(pvArgs), reinterpret_cast<BA_ONEXECUTEPACKAGEBEGIN_RESULTS*>(pvResults));
            break;
        case BOOTSTRAPPER_APPLICATION_MESSAGE_ONEXECUTEPATCHTARGET:
            hr = BalBaseBAProcOnExecutePatchTarget(pBA, reinterpret_cast<BA_ONEXECUTEPATCHTARGET_ARGS*>(pvArgs), reinterpret_cast<BA_ONEXECUTEPATCHTARGET_RESULTS*>(pvResults));
            break;
        case BOOTSTRAPPER_APPLICATION_MESSAGE_ONEXECUTEPROGRESS:
            hr = BalBaseBAProcOnExecuteProgress(pBA, reinterpret_cast<BA_ONEXECUTEPROGRESS_ARGS*>(pvArgs), reinterpret_cast<BA_ONEXECUTEPROGRESS_RESULTS*>(pvResults));
            break;
        case BOOTSTRAPPER_APPLICATION_MESSAGE_ONEXECUTEMSIMESSAGE:
            hr = BalBaseBAProcOnExecuteMsiMessage(pBA, reinterpret_cast<BA_ONEXECUTEMSIMESSAGE_ARGS*>(pvArgs), reinterpret_cast<BA_ONEXECUTEMSIMESSAGE_RESULTS*>(pvResults));
            break;
        case BOOTSTRAPPER_APPLICATION_MESSAGE_ONEXECUTEFILESINUSE:
            hr = BalBaseBAProcOnExecuteFilesInUse(pBA, reinterpret_cast<BA_ONEXECUTEFILESINUSE_ARGS*>(pvArgs), reinterpret_cast<BA_ONEXECUTEFILESINUSE_RESULTS*>(pvResults));
            break;
        case BOOTSTRAPPER_APPLICATION_MESSAGE_ONEXECUTEPACKAGECOMPLETE:
            hr = BalBaseBAProcOnExecutePackageComplete(pBA, reinterpret_cast<BA_ONEXECUTEPACKAGECOMPLETE_ARGS*>(pvArgs), reinterpret_cast<BA_ONEXECUTEPACKAGECOMPLETE_RESULTS*>(pvResults));
            break;
        case BOOTSTRAPPER_APPLICATION_MESSAGE_ONEXECUTECOMPLETE:
            hr = BalBaseBAProcOnExecuteComplete(pBA, reinterpret_cast<BA_ONEXECUTECOMPLETE_ARGS*>(pvArgs), reinterpret_cast<BA_ONEXECUTECOMPLETE_RESULTS*>(pvResults));
            break;
        case BOOTSTRAPPER_APPLICATION_MESSAGE_ONUNREGISTERBEGIN:
            hr = BalBaseBAProcOnUnregisterBegin(pBA, reinterpret_cast<BA_ONUNREGISTERBEGIN_ARGS*>(pvArgs), reinterpret_cast<BA_ONUNREGISTERBEGIN_RESULTS*>(pvResults));
            break;
        case BOOTSTRAPPER_APPLICATION_MESSAGE_ONUNREGISTERCOMPLETE:
            hr = BalBaseBAProcOnUnregisterComplete(pBA, reinterpret_cast<BA_ONUNREGISTERCOMPLETE_ARGS*>(pvArgs), reinterpret_cast<BA_ONUNREGISTERCOMPLETE_RESULTS*>(pvResults));
            break;
        case BOOTSTRAPPER_APPLICATION_MESSAGE_ONAPPLYCOMPLETE:
            hr = BalBaseBAProcOnApplyComplete(pBA, reinterpret_cast<BA_ONAPPLYCOMPLETE_ARGS*>(pvArgs), reinterpret_cast<BA_ONAPPLYCOMPLETE_RESULTS*>(pvResults));
            break;
        case BOOTSTRAPPER_APPLICATION_MESSAGE_ONLAUNCHAPPROVEDEXEBEGIN:
            hr = BalBaseBAProcOnLaunchApprovedExeBegin(pBA, reinterpret_cast<BA_ONLAUNCHAPPROVEDEXEBEGIN_ARGS*>(pvArgs), reinterpret_cast<BA_ONLAUNCHAPPROVEDEXEBEGIN_RESULTS*>(pvResults));
            break;
        case BOOTSTRAPPER_APPLICATION_MESSAGE_ONLAUNCHAPPROVEDEXECOMPLETE:
            hr = BalBaseBAProcOnLaunchApprovedExeComplete(pBA, reinterpret_cast<BA_ONLAUNCHAPPROVEDEXECOMPLETE_ARGS*>(pvArgs), reinterpret_cast<BA_ONLAUNCHAPPROVEDEXECOMPLETE_RESULTS*>(pvResults));
            break;
        }
    }

    pBA->BAProcFallback(message, pvArgs, pvResults, &hr, pvContext);

    return hr;
}
