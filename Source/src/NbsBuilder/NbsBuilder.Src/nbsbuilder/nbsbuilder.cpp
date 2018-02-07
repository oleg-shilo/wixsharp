// nbsbuilder.cpp : Defines the entry point for the console application.
//
#include "stdafx.h"
#include "atlbase.h"
#include "utils.h"
#include "resource.h"

#include "afxres.h"

//{185F1B47-C267-4cfd-B55F-EB89DAF3B4E3}
//char markerData[] = { 0x18, 0x5F, 0x1B, 0x47, 0xC2, 0x67, 0x4c, 0xFD, 0xB5, 0x5F, 0xEB, 0x89, 0xDA, 0xF3, 0xB4, 0xE3 };

void EmbeddWinResources(wstring outFile, wstring msiFile1, wstring msiFile2, wstring regKey, bool varify);

#define IDR_CUSTOM_PRIMARY_DATA         131
#define IDR_CUSTOM_PRIMARY_NAME         132
#define IDR_CUSTOM_PREREQ_DATA          133
#define IDR_CUSTOM_PREREQ_NAME          134
#define IDR_CUSTOM_CONDITION            135
#define IDR_CUSTOM_VERIFY               136
#define IDI_nbs                         107
#define IDI_SMALL                       108

//void TestIcon();
//void InjectMainIcon(WCHAR *Where, WCHAR *What);
int _tmain(int argc, _TCHAR* argv[])
{
    _AtlBaseModule;

     //InjectMainIcon(L"E:\\Galos\\Projects\\WixSharp\\Main\\NbsBuilder\\Output\\nbs.exe", L"E:\\Galos\\Projects\\WixSharp\\Main\\NbsBuilder\\Output\\nbs1.ico");
    //string data = InputStream::ReadToEnd(L"E:\\Galos\\Projects\\WixSharp\\Main\\NbsBuilder\\Output\\nbs1.ico");
    //Resources::ReplaceIcon(L"E:\\Galos\\Projects\\WixSharp\\Main\\NbsBuilder\\Output\\nbs.exe", IDI_nbs, data);
    //Resources::ReplaceIcon(L"E:\\Galos\\Projects\\WixSharp\\Main\\NbsBuilder\\Output\\nbs.exe", IDI_SMALL, data);

    //TestIcon();
    //return 0;
    /*string data;
    data.resize(10);
    ZeroMemory((void*)data.data(), 10);
    strcpy((char*)data.data(), "test");

    Resources::ReplaceResource(
        L"P:\\Galos\\Projects\\WixSharp\\Main\\NbsBuilder\\Debug\\nbs1.exe", 
        L"CUSTOM", 134, data);

    return 0;*/

    //WCHAR val[100];
    //DWORD ttt = GetPrivateProfileStringW(L"Input", L"InstallType0", L"ffff", val,100, L"E:\\Galos\\Projects\\WixSharp\\Main\\NetStrapper\\Debug\\mbsbuilder.ini");

    //ATLASSERT(FALSE);
    printf("Building bootstrapper...\n");
    wstring curDir = Path::CurrentDirectory();

    wstring outFile, msiFile1, msiFile2, regKey;
    bool helpRequested = false;
    bool verify = true;
    
    WCHAR* lpCmdLine = GetCommandLineW();
    vector<wstring> args = Application::ParseCommandLine(lpCmdLine);
        
    for (UINT i = 1; i < args.size(); i++)
    {
        if (Utils::StartWith(args[i], L"/out:"))
        {
            outFile = Utils::Substring(args[i], wcslen(L"/out:"));
            outFile = Path::GetFullPath(outFile);
        }
        else if (Utils::StartWith(args[i], L"/first:"))
        {
             msiFile1 = Utils::Substring(args[i], wcslen(L"/first:"));
             msiFile1 = Path::GetFullPath(msiFile1);
        }
        else if (Utils::StartWith(args[i], L"/second:"))
        {
            msiFile2 = Utils::Substring(args[i], wcslen(L"/second:"));
            msiFile2 = Path::GetFullPath(msiFile2);
        }
        else if (args[i] == L"/verify:no")
        {
            verify = false;
        }
        else if (Utils::StartWith(args[i], L"/reg:"))
        {
            regKey = Utils::Substring(args[i], wcslen(L"/reg:"));
            vector<wstring> tokens = Utils::Split(L"HKLM:SOFTWARE\\Microsoft\\.NETFramework:", L':');
        }
        else if (args[i] == L"/help" || args[i] == L"/?" )
        {
            helpRequested = true;
        }
    }

    if (helpRequested || args.size() == 1)
    {   
        printf("Native Bootstrapper Builder v 1.0.0\n");
        printf("Copyright (C) 2010 Oleg Shilo. \n");
        printf("\n");
        printf("Builds simple native (Win32) bootstrapper. It alows building a bootstrapper for\n");
        printf("two deployment applications: primary setup and its prerequisite.\n");
        printf("\n");
        printf("NBSBUILDER /out:<outFile> /first:<firstMSI> /second:<secondMSI> /regkey:<reg> [/verify:<yes|no>] [/icon:<path>]\n");
        printf("\n");
        printf(" first  - the setup application to be run the first (prerequisite).\n");
        printf("\n");
        printf(" second - the setup application to be run the second (after\n");
        printf("          running prerequisite)\n");
        printf("\n");
        printf(" out    - name of the output file (bootstrapper) to produce\n");
        printf("\n");
        printf(" reg    - the registry key that indicates if firstMSI should be run.\n");
        printf("          If the 'reg' value exists in registry the firstMSI will be\n");
        printf("          considered already installed and will not run.\n");
        printf("          Registry value should comply with the following pattern.\n");
        printf("          <HKEY>:<SubKey>:ValueName>\n");
        printf("\n");
        printf("          Examples:\n");
        printf("           'HLKM:SOFTWARE\\Microsoft\\.NETFramework\\v2.0.50727:' .NET v2.0,\n");
        printf("           'HLKM:SOFTWARE\\Microsoft\\.NETFramework:' any version of .NET,\n");
        printf("           'HLKM:SOFTWARE\\MyCompany\\MiProduct:InstallDir' InstallDir value\n");
        printf("\n");
        printf(" verify - flag (yes/no) indicating if the registry key (/regkey:<reg>)\n");
        printf("          should be checked again after running prerequisite. Default: yes.\n");
       // printf("\n");
        //printf(" icon   - path to the icon file for the bootstrapper.\n");
        
        
        return 0;
    }
    if (msiFile1.length() == 0 || !Path::FileExists(msiFile1))
    {
        printf("The 'first' argument was not specified or incorrect.\n");
        return 1;
    }
    if (msiFile2.length() == 0 || !Path::FileExists(msiFile2))
    {
        printf("The 'second' argument was not specified or is incorrect.\n");
        return 1;
    }
    if (outFile.length() == 0)
    {
        printf("You to have to specify '/out:' argument (output file).\n");
        return 1;
    }
    if (regKey.length() == 0)
    {
        printf("You have to specify '/reg:' argument (refistry value for the prerequisite file).\n");
        return 1;
    }

    //wstring path = Path::GetTempDir();
    ////////////////////////////////////
    /*wstring bootstrapperFile = Path::Combine(Path::CurrentDirectory(), L"setup.exe");
    wstring launcherFile = L"E:\\Galos\\Projects\\WixSharp\\Main\\NetStrapper\\nbs\\temp\\nbs.exe";
    wstring msiFile1 = L"E:\\Galos\\Projects\\WixSharp\\Main\\NetStrapper\\nbsbuilder\\Input\\App1.exe";
    wstring msiFile2 = L"E:\\Galos\\Projects\\WixSharp\\Main\\NetStrapper\\nbsbuilder\\Input\\App2.exe";
    wstring regKey = L"1234567890";
    *////////////////////////////////////////


    EmbeddWinResources(outFile, msiFile1, msiFile2, regKey, verify);
        
    return 0;
}

void EmbeddCustomResources(wstring outFile, wstring msiFile1, wstring msiFile2, wstring regKey)
{
    string launcherData = Resources::Read(IDR_CUSTOM1, L"CUSTOM"); 

    OutputStream file(outFile);
    
    //Launcher (bootstrapper)
    file.WriteData(launcherData);

    //resource marker
   /* for (int i = 0; i < sizeof(markerData); i++)
        file.WriteByte(markerData[i]);*/
    
    //First MSI 
    string data = InputStream::ReadToEnd(msiFile1);
    file.WriteLong(data.size());
    file.WriteData(data);

    wstring fileName = Path::GetFileName(msiFile1);
    file.WriteLong(fileName.size());
    file.WriteString(fileName);

    //Second MSI 
    data = InputStream::ReadToEnd(msiFile2);
    file.WriteLong(data.size());
    file.WriteData(data);

    fileName = Path::GetFileName(msiFile2);
    file.WriteLong(fileName.size());
    file.WriteString(fileName);

    //Registry key
    int t = regKey.size();
    file.WriteLong(regKey.size());
    file.WriteString(regKey);

    //Data offset
    //file.WriteLong(launcherData.length());

    printf("\n\nSuccess: bootstrapper file has been built (%S).\n", outFile.c_str());
}



void EmbeddWinResources(wstring outFile, wstring msiFile1, wstring msiFile2, wstring regKey, bool verify)
{
    //Launcher (bootstrapper)
    {
        OutputStream file(outFile);
        file.WriteData(Resources::Read(IDR_CUSTOM1, L"CUSTOM"));
    }
    
    //First MSI 
    string data = InputStream::ReadToEnd(msiFile1);
    Resources::ReplaceResource(outFile, L"CUSTOM", IDR_CUSTOM_PREREQ_DATA, data);

    wstring fileName = Path::GetFileName(msiFile1);
    Resources::ReplaceResource(outFile, L"CUSTOM", IDR_CUSTOM_PREREQ_NAME, Utils::StringToData(fileName));
    
    //Second MSI 
    data = InputStream::ReadToEnd(msiFile2);
    Resources::ReplaceResource(outFile, L"CUSTOM", IDR_CUSTOM_PRIMARY_DATA, data);

    wstring fileName2 = Path::GetFileName(msiFile2);
    Resources::ReplaceResource(outFile, L"CUSTOM", IDR_CUSTOM_PRIMARY_NAME, Utils::StringToData(fileName2));
    
    //Registry key
    Resources::ReplaceResource(outFile, L"CUSTOM", IDR_CUSTOM_CONDITION, Utils::StringToData(regKey));

    //verify
    Resources::ReplaceResource(outFile, L"CUSTOM", IDR_CUSTOM_VERIFY, Utils::StringToData(verify ? L"yes" : L"no"));

    //icon
    //data = InputStream::ReadToEnd(L"E:\\cs-script\\engine\\Logo\\css_logo.ico");
    //Resources::ReplaceResource(outFile, RT_GROUP_ICON, IDI_nbs, data);
    //Resources::ReplaceResource(outFile, RT_GROUP_ICON, IDI_SMALL, data);

    printf("\nSuccess: \n");
    printf(" Bootstrapper : %S.\n", Path::GetFileName(outFile).c_str());
    printf(" Prerequisite : %S.\n", Path::GetFileName(fileName).c_str());
    printf(" RegKey value : %S\n", regKey.c_str());
    printf(" Post-verify  : %S\n", verify ? L"yes" : L"no");
    printf("\nPrerequisite will be installed if the registry key value (above) is not found at the installation time.\n\n");

}

void TestIcon()
{
    wstring             lpszFile = L"E:\\Galos\\Projects\\WixSharp\\Main\\NbsBuilder\\Output\\nbs.exe";
    wstring             lpszSourceFile = L"C:\\Windows\\System32\\notepad.exe";

     
    HRSRC hRes;         // handle/ptr. to res. info. in hExe
    HANDLE hUpdateRes;  // update resource handle
    char *lpResLock;    // pointer to resource data
    HRSRC hResLoad;     // handle to loaded resource
    BOOL result;
    HMODULE hSrcExe,hDestExe;
    int iLoop;

    //Load the source exe from where we need the icon
    hSrcExe = LoadLibrary(lpszSourceFile.c_str());
    if(hSrcExe == NULL)
        return;

    // Locate the ICON resource in the .EXE file.
    for(iLoop = 1;;iLoop++)
    {
        WCHAR str[100];
        swprintf(str, L"#%d",iLoop);
        hRes = FindResource(hSrcExe, str, RT_ICON);
        if (hRes == NULL)
            continue ;
        else if(iLoop == 10)
            return;
        else
            break;
    }

    // Load the ICON into global memory.
    hResLoad = (HRSRC)LoadResource(hSrcExe, hRes);
    if (hResLoad == NULL)
        return ;

    // Lock the ICON into global memory.
    lpResLock = (char*)LockResource(hResLoad);
    if (lpResLock == NULL)
        return ;

    hDestExe = LoadLibrary(lpszFile.c_str());
    if(hDestExe == NULL)
        return;
    // Locate the ICON resource in the .EXE file.
   /* for(iLoop = 1;;iLoop++)
    {
        WCHAR str[100];
        swprintf(str, L"#%d",iLoop);
        if (FindResource(hDestExe, str, RT_ICON) == NULL)
            continue ;
        else if(iLoop == 10)
            break;
        else
            break;
    }*/
    FreeLibrary(hDestExe);

    // Open the file to which you want to add the ICON resource.
    hUpdateRes = BeginUpdateResource(lpszFile.c_str(), FALSE);
    if (hUpdateRes == NULL)
        return ;

    result = UpdateResource(hUpdateRes,       // update resource handle
        RT_ICON,                   // change dialog box resource
        MAKEINTRESOURCE(1),
        MAKELANGID(LANG_ENGLISH, SUBLANG_ENGLISH_US),  // neutral language
        lpResLock,                   // ptr to resource info
        SizeofResource(hSrcExe, hRes)); // size of resource info.

    if (result == FALSE)
        return ;

    // Write changes then close it.
    if (!EndUpdateResource(hUpdateRes, FALSE))
    {
        return ;
    }
}
