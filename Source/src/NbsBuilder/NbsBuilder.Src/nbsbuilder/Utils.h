#pragma once

#include <vector>
#include "comdef.h"
#include "Shlwapi.h"

#define MAX_KEY_LENGTH 255
#define MAX_VALUE_NAME 16383

class Shell
{
public:
    static void RunApp(wstring app, wstring params)
    {
        SHELLEXECUTEINFOW sei = { sizeof(sei) };
        sei.fMask = SEE_MASK_FLAG_DDEWAIT;
        sei.nShow = SW_SHOWNORMAL;
        sei.lpFile = app.c_str();
        if (params.length() != 0)
            sei.lpParameters = params.c_str();
        sei.fMask = SEE_MASK_NOCLOSEPROCESS;  //ensures hProcess gets the process handle

        ShellExecuteExW(&sei);

        WaitForSingleObject(sei.hProcess, INFINITE);
    }
    
    static void RunApp(wstring app)
    {
        SHELLEXECUTEINFOW sei = { sizeof(sei) };
        sei.fMask = SEE_MASK_FLAG_DDEWAIT;
        sei.nShow = SW_SHOWNORMAL;
        sei.lpFile = app.c_str();
        sei.fMask = SEE_MASK_NOCLOSEPROCESS;  //ensures hProcess gets the process handle

        ShellExecuteExW(&sei);

        WaitForSingleObject(sei.hProcess, INFINITE);
    }

    static bool DelFile(wstring file)
    {
        return DeleteFileW(file.c_str()) ? true : false;
    }
};

class Utils
{
public: 
    static bool StartWith(wstring data, wstring pattern)
    {
        for(UINT i = 0; i < pattern.length(); i++)
            if (pattern[i] != data[i])
                return false;

        return true;
    }

    static wstring Substring(wstring data, int index)
    {
        wstring retval;
        retval = (data.c_str() + index);

        return retval;
    }

    static vector<wstring> Split(wstring data, WCHAR delimiter)
    {
        vector<wstring> retval;
        retval.push_back(L"");
        UINT index = 0;

        for(UINT i = 0; i < data.length(); i++)
            if (data[i] == delimiter)
            {
                if (i+1 < data.length() && data[i+1] == delimiter) //not a delimiter but a char with the same value as the delimiter
                {
                    retval[index] += data[i++]; //skip the next character
                }
                else
                {
                    retval.push_back(L"");
                    index++;
                }
            }
            else
            {
                retval[index] += data[i];
            }

        return retval;
    }

    static wstring DataToString(string str)
    {
        wstring retval;
        retval.resize(str.length()*sizeof(WCHAR));
        memcpy((void*)retval.data(), str.data(), str.length());
        return retval; 
    }

    static string StringToData(wstring data)
    {
        string retval;
        retval.resize(data.length()*sizeof(WCHAR));
        memcpy((void*)retval.data(), data.data(), retval.length());
        return retval; 
    }
};


class InputStream
{
    wstring name;
    ifstream* file;

public:
    ~InputStream()
    {
        delete file;
    }
    
    InputStream(wstring name)
    {
        this->name = name;
        this->file = new ifstream(name.c_str(), ios::in | ios::binary);
    }

    void SetOffset(long offset, bool fromEnd = false)
    {
        file->seekg(fromEnd ? -offset : offset, fromEnd ? ios::end : ios::beg);
    }

    long ReadLong()
    {
        long retval = 0;
        file->read((char*)&retval, (streamsize)sizeof(retval));
        return retval;
    }

    char ReadByte()
    {
        char retval = 0;
        file->read((char*)&retval, (streamsize)sizeof(retval));
        return retval;
    }
    
    string ReadData(long size)
    {
        string buffer;
        buffer.resize(size);
        file->read((char*)buffer.data(), (streamsize)buffer.size());

        return buffer;
    }

    wstring ReadString(long charCount)
    {
        wstring buffer;
        buffer.resize(charCount);
        file->read((char*)buffer.data(), (streamsize)charCount * sizeof(WCHAR));

        return buffer;
    }

    static string ReadToEnd(wstring fileName)
    {
        string buffer;
        string asciFileName = (LPCSTR)_bstr_t(fileName.c_str());
        ifstream file(asciFileName.c_str(), ios::in | ios::binary);
        size_t fileSize = file.seekg(0, ios::end).tellg();
        buffer.resize(fileSize);
        file.seekg(0)
            .read((char*)buffer.data(), (streamsize)buffer.size());
        
        return buffer;
    }
};

class OutputStream
{
    wstring name;
    ofstream* file;

public:
    ~OutputStream()
    {
        delete file;
    }
    
    OutputStream(wstring name)
    {
        this->name = name;
        this->file = new ofstream(name.c_str(), ios::out | ios::binary);
    }

    void SetOffset(long offset, bool fromEnd = false)
    {
        file->seekp(fromEnd ? -offset : offset, fromEnd ? ios::end : ios::beg);
    }

    void WriteData(string& data)
    {
        file->write((char*)data.data(), data.size());
    }

    void WriteString(wstring data)
    {
        long size = data.length()*sizeof(WCHAR);
        file->write((char*)data.data(), size);
    }

    void WriteLong(long data)
    {
        file->write((char*)&data, (streamsize)sizeof(data));
    }

    void WriteByte(char data)
    {
        file->write(&data, (streamsize)sizeof(data));
    }

    static void Write(wstring fileName, string data)
    {
        string asciFileName = (LPCSTR)_bstr_t(fileName.c_str());
        ofstream file(asciFileName.c_str(), ios::out | ios::binary);
        file.write((char*)data.data(), (streamsize)data.size());
    }
};

class Resources
{
    static HINSTANCE GetModuleInstance()
    {
        return GetModuleHandle(NULL);
        //return _AtlBaseModule.GetModuleInstance()
    }

public:

    static string Read(int resourceId, LPCWSTR resourceType)
    {
        string buffer;

        HINSTANCE hInstance = GetModuleInstance();


        HRSRC resInfo = ::FindResource(hInstance, MAKEINTRESOURCE( resourceId ), resourceType);
        HGLOBAL resHandle = ::LoadResource(hInstance, resInfo);
        DWORD resSize = ::SizeofResource(hInstance, resInfo);
        BYTE* pData = (BYTE*)::LockResource(resHandle);

        buffer.resize(resSize);
        memcpy((char*)buffer.data(), pData, resSize);

        UnlockResource(resHandle);	

        return buffer;
    }

    static bool ReplaceResource(wstring file, wstring resType, int resId, string data)
    {
        return ReplaceResource(file, resType, resId, MAKELANGID(LANG_ENGLISH, SUBLANG_ENGLISH_US), data);
    }
    static bool ReplaceIcon(wstring file, int resId, string data)
    {
        return ReplaceIcon(file, resId, MAKELANGID(LANG_ENGLISH, SUBLANG_ENGLISH_US), data);
    }

    static bool ReplaceResource(wstring file, wstring resType, int resId, WORD language, string data)
    {
        HANDLE h = ::BeginUpdateResourceW(file.c_str(), FALSE);
        if(h == NULL || !::UpdateResource(h, resType.c_str(), MAKEINTRESOURCE(resId), language, (LPVOID)data.data(), data.length()) )
        {
            return false;
        }
        BOOL r = ::EndUpdateResource(h, FALSE); // write changes 
        DWORD err = GetLastError();
        return true;
    }
    static bool ReplaceIcon(wstring file, int resId, WORD language, string data)
    {
        HANDLE h = ::BeginUpdateResourceW(file.c_str(), FALSE);
        if(h == NULL || !::UpdateResource(h, RT_GROUP_ICON, MAKEINTRESOURCE(resId), language, (LPVOID)data.data(), data.length()) )
        {
            return false;
        }
        BOOL r = ::EndUpdateResource(h, FALSE); // write changes 
        DWORD err = GetLastError();
        return true;
    }
};

class Path
{
public:
    static wstring Combine(wstring path1, wstring path2)
    {
        WCHAR buf[MAX_PATH*2];
        GetCurrentDirectoryW(MAX_PATH*2, buf);
            
        PathCombine(buf, path1.c_str(), path2.c_str());
        
        return wstring(buf);
    }
    
    static wstring GetFullPath(wstring path)
    {
        if (PathIsRelativeW(path.c_str()))
        {
            return Path::Combine(Path::CurrentDirectory(), path);
        }
        else
        {
            return path;
        }
    }

    static wstring GetFileName(wstring path)
    {
        return PathFindFileNameW(path.c_str());
    }

    static wstring CurrentDirectory()
    {
        WCHAR dir[MAX_PATH*2];
        GetCurrentDirectoryW(MAX_PATH*2, dir); 
        
        return wstring(dir);
    }

    static bool FileExists(wstring path)
    {
        return PathFileExistsW(path.c_str()) ? true : false;
    }

    static bool DirectoryExists(wstring path)
    {
        return FileExists(path);
    }
    
    static void CreateDirectory(wstring directory)
    {
        ::CreateDirectoryW(directory.c_str(), NULL);
    }

    static wstring GetTempDir()
    {
        WCHAR buf[MAX_PATH*2];
        GetTempPathW(MAX_PATH*2, buf);
        
        return wstring(buf);
    }
};

class RegKey
{
public:
    static bool ValueExists(wstring path)
    {
        vector<wstring> tokens = Utils::Split(path, L':');
        return ValueExists(tokens[0], tokens[1], tokens[2]);
    }
    
    static bool ValueExists(wstring key, wstring path, wstring value)
    {
        HKEY hOpenedKey;
        HKEY hKey;

        if (key == L"HKCU")
            hKey = HKEY_CURRENT_USER;
        else if(key == L"HKLM")
            hKey = HKEY_LOCAL_MACHINE;
        else if(key == L"HKCR")
            hKey = HKEY_CLASSES_ROOT;
        else if(key == L"HKU")
            hKey = HKEY_USERS;

        if( RegOpenKeyExW(hKey, path.c_str(), 0, KEY_READ, &hOpenedKey) == ERROR_SUCCESS)
        {
            if (value == L"")
                return true; //default value
            else
                return QueryKeyValue(hOpenedKey, value);
        }

        return false;
    }

private:
    static bool QueryKeyValue(HKEY hKey, wstring valueName) 
    { 
        WCHAR    achClass[MAX_PATH] = L"";  // buffer for class name 
        DWORD    cchClassName = MAX_PATH;  // size of class string 
        DWORD    cSubKeys=0;               // number of subkeys 
        DWORD    cbMaxSubKey;              // longest subkey size 
        DWORD    cchMaxClass;              // longest class string 
        DWORD    cValues;              // number of values for key 
        DWORD    cchMaxValue;          // longest value name 
        DWORD    cbMaxValueData;       // longest value data 
        DWORD    cbSecurityDescriptor; // size of security descriptor 
        FILETIME ftLastWriteTime;      // last write time 
     
        DWORD i, retCode; 
     
        TCHAR  achValue[MAX_VALUE_NAME]; 
        DWORD cchValue = MAX_VALUE_NAME; 
     
        // Get the class name and the value count. 
        retCode = RegQueryInfoKeyW(
            hKey,                    // key handle 
            achClass,                // buffer for class name 
            &cchClassName,           // size of class string 
            NULL,                    // reserved 
            &cSubKeys,               // number of subkeys 
            &cbMaxSubKey,            // longest subkey size 
            &cchMaxClass,            // longest class string 
            &cValues,                // number of values for this key 
            &cchMaxValue,            // longest value name 
            &cbMaxValueData,         // longest value data 
            &cbSecurityDescriptor,   // security descriptor 
            &ftLastWriteTime);       // last write time 
     
          // Enumerate the key values. 

            printf( "\nNumber of values: %d\n", cValues);

            for (i=0, retCode=ERROR_SUCCESS; i<cValues; i++) 
            { 
                cchValue = MAX_VALUE_NAME; 
                achValue[0] = '\0'; 
                retCode = RegEnumValueW(hKey, i, 
                    achValue, 
                    &cchValue, 
                    NULL, 
                    NULL,
                    NULL,
                    NULL);
     
                if (retCode == ERROR_SUCCESS ) 
                { 
                    if (_wcsicmp(valueName.c_str(), achValue) == 0)
                    //if (valueName == achValue)
                        return true;
                    //_tprintf(TEXT("(%d) %s\n"), i+1, achValue); 
                } 
            }

            return false;
        }
};


class Application
{
public:

    static wstring ModuleName()
    {
        WCHAR name[MAX_PATH];
        GetModuleFileNameW(NULL, name, MAX_PATH);
        return name;
    }

    static vector<wstring> ParseCommandLine(wstring data)
    {
        vector<wstring> argsList;

        int currToken = -1;
        int currQuatationChar = -1;
        int ttt = data.length();
        for (UINT i = 0 ; i < data.length(); i++)
        {
            if (currToken == -1)
            {
                if (data[i] == L'"')
                    currQuatationChar = i;
                else if (currQuatationChar != -1)
                    currToken = i;
                else if (data[i] != L'\t' && data[i] != L' ')
                    currToken = i;
            }
            else
            {
                if (data[i] == '"')
                {
                    if (currQuatationChar != -1)
                    {
                        currQuatationChar = i;
                        wstring arg;
                        arg.resize(i - currToken);
                        memcpy((void*)arg.data(), data.c_str() + currToken, arg.size()*sizeof(WCHAR));
                        currQuatationChar = -1;
                        currToken = -1;
                        argsList.push_back(arg);
                    }
                }
                else if (currQuatationChar == -1 && (data[i] == L'\t' || data[i] == L' '))
                {
                    wstring arg;
                    arg.resize(i - currToken);
                    memcpy((void*)arg.data(), data.c_str() + currToken, arg.size()*sizeof(WCHAR));
                    currToken = -1;	
                    argsList.push_back(arg);
                }
            }

            if (i == (data.length()-1) && currToken != -1)
            {
                wstring arg;
                arg.resize(i - currToken + 1);
                memcpy((void*)arg.data(), data.c_str() + currToken, arg.size()*sizeof(WCHAR));
                currToken = -1;	
                argsList.push_back(arg);
            }
        }

        if (argsList.size() == 0 && data.size() != 0)
            argsList.push_back(data);

        return argsList;
    }

    /*
    
    WCHAR* test = L"aaa";
    ATLTRACE(">[%S]\n", test);
    vector<wstring> args = Application::ParseCommandLine(test);
    for (int i = 0; i < args.size(); i++)
        ATLTRACE("[%S]\n", args[i].c_str());
    ATLTRACE("--------------\n");

    test = L"\"aaa\"";
    args = Application::ParseCommandLine(test);
    ATLTRACE(">[%S]\n", test);
    for (int i = 0; i < args.size(); i++)
        ATLTRACE("[%S]\n", args[i].c_str());
    ATLTRACE("--------------\n");

    test = L"\"aaa\" ";
    args = Application::ParseCommandLine(test);
    ATLTRACE(">[%S]\n", test);
    for (int i = 0; i < args.size(); i++)
        ATLTRACE("[%S]\n", args[i].c_str());
    ATLTRACE("--------------\n");

    test = L"aaa   bbb";
    args = Application::ParseCommandLine(test);
    ATLTRACE(">[%S]\n", test);
    for (int i = 0; i < args.size(); i++)
        ATLTRACE("[%S]\n", args[i].c_str());
    ATLTRACE("--------------\n");

    test = L"aaa   \tbbb ";
    args = Application::ParseCommandLine(test);
    ATLTRACE(">[%S]\n", test);
    for (int i = 0; i < args.size(); i++)
        ATLTRACE("[%S]\n", args[i].c_str());
    ATLTRACE("--------------\n");

    test = L" \"aaa   \tbbb\" ";
    args = Application::ParseCommandLine(test);
    ATLTRACE(">[%S]\n", test);
    for (int i = 0; i < args.size(); i++)
        ATLTRACE("[%S]\n", args[i].c_str());
    ATLTRACE("--------------\n");

    test = L" \"aaa\"   \t\"bbb\" ";
    args = Application::ParseCommandLine(test);
    ATLTRACE(">[%S]\n", test);
    for (int i = 0; i < args.size(); i++)
        ATLTRACE("[%S]\n", args[i].c_str());
    ATLTRACE("--------------\n");
    */
};