// nbs.cpp : Defines the entry point for the application.
//

#include "stdafx.h"
#include "nbs.h"
#include "ShellAPI.h"
#include "Utils.h"


void ProcessWinResources(wstring& msiFile1, wstring& msiFile2, wstring& regKey, bool& verify);


int APIENTRY _tWinMain(HINSTANCE hInstance,
                     HINSTANCE hPrevInstance,
                     LPTSTR    lpCmdLine,
                     int       nCmdShow)
{
	string data = Resources::Read(IDR_CUSTOM_CONDITION, L"CUSTOM");

	if (strcmp(data.c_str(), "HKLM:SOFTWARE\\Microsoft\\.NETFramework:$default") == 0)
	{
		MessageBox(0, L"Resources are not embedded", L"Wix# Bootstrapper", 0);
		return 1;
	}

    //ATLASSERT(FALSE);
    wstring msiFile1, msiFile2;
    wstring regKey;
    bool verify = true;

    ProcessWinResources(msiFile1, msiFile2, regKey, verify);
   
    wstring msiParams = PathGetArgsW(GetCommandLineW());

    if (!RegKey::ValueExists(regKey))
	    Shell::RunApp(msiFile1, msiParams);

    if (verify && !RegKey::ValueExists(regKey))
        return 1;
	
    Shell::RunApp(msiFile2, msiParams);
   
    return 0;
}

#define IDR_CUSTOM_VERIFY               136

void ProcessWinResources(wstring& msiFile1, wstring& msiFile2, wstring& regKey, bool &verify)
{
    wstring tempDir =  Path::Combine(Path::GetTempDir(), L"Wix#");

    if (!Path::DirectoryExists(tempDir))
        Path::CreateDirectory(tempDir);
    
    string msiData1 = Resources::Read(IDR_CUSTOM_PREREQ_DATA, L"CUSTOM"); 
    wstring fileName = Utils::DataToString(Resources::Read(IDR_CUSTOM_PREREQ_NAME, L"CUSTOM"));
    
    msiFile1 = Path::Combine(tempDir, fileName);
    OutputStream::Write(msiFile1, msiData1);

    string msiData2 = Resources::Read(IDR_CUSTOM_PRIMARY_DATA, L"CUSTOM"); 
    fileName = Utils::DataToString(Resources::Read(IDR_CUSTOM_PRIMARY_NAME, L"CUSTOM"));
    
    msiFile2 = Path::Combine(tempDir, fileName);
    OutputStream::Write(msiFile2, msiData2);
     
    regKey = Utils::DataToString(Resources::Read(IDR_CUSTOM_CONDITION, L"CUSTOM")); 

    wstring verifyValue = Utils::DataToString(Resources::Read(IDR_CUSTOM_VERIFY, L"CUSTOM")); 

    verify = verifyValue != L"no";
}