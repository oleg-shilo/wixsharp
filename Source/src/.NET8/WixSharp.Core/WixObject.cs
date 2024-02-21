#region Licence...
//-----------------------------------------------------------------------------
// Date:	10/03/09	Time: 21:00
// Module:	WixSharp.cs
// Version: 0.1.12
// 
// This module contains the definition of the Wix# classes. 
//
// Written by Oleg Shilo (oshilo@gmail.com)
// Copyright (c) 2008-2009. All rights reserved.
//
// Redistribution and use of this code in source and binary forms, with or without 
// modification, are permitted provided that the following conditions are met:
// 1. Redistributions of source code must retain the above copyright notice, 
//	this list of conditions and the following disclaimer. 
// 2. Neither the name of an author nor the names of the contributors may be used 
//	to endorse or promote products derived from this software without specific 
//	prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS 
// "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT 
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR 
// A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT 
// OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
// SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED 
// TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR 
// PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF 
// LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
// NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS 
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
//	Caution: Bugs are expected!
//----------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using IO = System.IO;

namespace WixSharp
{
    /// <summary>
    /// Base class for all Wix# types (entities)
    /// </summary>
    public partial class WixObject
    {
        /// <summary>
        /// Name of the <see cref="WixObject"/>. 
        /// <para>This value is used as a <c>Name</c> for the corresponding WiX XML element.</para>
        /// </summary>
        public string Name = "";

        /// <summary>
        /// Gets or sets the <c>Id</c> value of the <see cref="WixObject"/>. 
        /// <para>This value is used as a <c>Id</c> for the corresponding WiX XML element.</para>
        /// <para>If the <see cref="Id"/> value is not specified explicitly by the user, the Wix# compiler
        /// generates it automatically insuring its uniqueness.</para>
        /// </summary>
        /// <value>The id.</value>
        public string Id
        {
            get
            {
                if (id.IsEmpty())
                {
                    if (!idMaps.ContainsKey(GetType()))
                        idMaps[GetType()] = new Dictionary<string, int>();

                    var rawName = Name.Expand();

                    if (GetType() != typeof(Dir) && GetType().BaseType != typeof(Dir))
                        rawName = IO.Path.GetFileName(Name).Expand();

                    if (!idMaps[GetType()].ContainsKey(rawName)) //this Type has already
                    {
                        idMaps[GetType()][rawName] = 0;
                        id = rawName;
                    }
                    else
                    {
                        //The Id has been already generated for this Type with the rawName
                        //so just increase the index
                        var index = idMaps[GetType()][rawName] + 1;

                        id = rawName + "." + index;
                        idMaps[GetType()][rawName] = index;
                    }
                }
                return id;
            }
            set { id = value; }
        }
        /// <summary>
        /// Backing value of <see cref="Id"/>.
        /// </summary>
        protected string id;
        
        static Dictionary<Type, Dictionary<string, int>> idMaps = new Dictionary<Type, Dictionary<string, int>>();

        /// <summary>
        /// Resets the <see cref="Id"/> generator. This method is exercised by the Wix# compiler before any 
        /// <c>Build</c> operations to ensure reproducibility of the <see cref="Id"/>s set between <c>Build()</c> 
        /// calls.
        /// </summary>
        static public void ResetIdGenerator()
        {
            idMaps.Clear();
        }
    }
}
