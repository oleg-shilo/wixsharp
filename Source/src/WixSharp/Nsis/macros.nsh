!macro REG_KEY_VALUE_EXISTS ROOT_KEY SUB_KEY NAME
    ClearErrors
    push $0

    ${If} "${NAME}" == ""
        ${If} ${RunningX64}
            SetRegView 64
            EnumRegKey $0 "${ROOT_KEY}" "${SUB_KEY}" 0
            SetRegView 32
        ${Else}
            SetErrors
        ${EndIf}

        ${If} ${Errors}
            EnumRegKey $0 "${ROOT_KEY}" "${SUB_KEY}" 0
        ${EndIf}
    ${Else}
        ${If} ${RunningX64}
            SetRegView 64
            ReadRegStr $0 "${ROOT_KEY}" "${SUB_KEY}" "${NAME}"
            SetRegView 32
        ${Else}
            SetErrors
        ${EndIf}

        ${If} ${Errors}
            ReadRegStr $0 "${ROOT_KEY}" "${SUB_KEY}" "${NAME}"
        ${EndIf}
    ${EndIf}

    pop $0
!macroend
