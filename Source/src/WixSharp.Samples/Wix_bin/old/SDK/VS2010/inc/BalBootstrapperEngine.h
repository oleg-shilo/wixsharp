// Copyright (c) .NET Foundation and contributors. All rights reserved. Licensed under the Microsoft Reciprocal License. See LICENSE.TXT file in the project root for full license information.

#ifdef __cplusplus
extern "C" {
#endif

// function declarations

HRESULT BalBootstrapperEngineCreate(
    __in IBootstrapperEngine* pEngine, // TODO: remove after IBootstrapperEngine is moved out of the engine.
    __in PFN_BOOTSTRAPPER_ENGINE_PROC pfnBAEngineProc,
    __in_opt LPVOID pvBAEngineProcContext,
    __out IBootstrapperEngine** ppEngineForApplication
    );

#ifdef __cplusplus
}
#endif
