﻿#region Copyright & License
/*
Copyright (c) 2023, Integrated Solutions, Inc.
All rights reserved.

Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

		* Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
		* Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
		* Neither the name of the Integrated Solutions, Inc. nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
#endregion
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISI.FileExplorer.Extensions.Shell.Extensions;

namespace ISI.FileExplorer.Extensions.Shell
{
	public  class Ant
	{
		public static Guid ExecuteTargetCommandUuid = Guid.Parse("3039b9e1-83c7-4320-8980-8601c543834a");
		public const string ParameterName_BuildFileName = "BuildFileName";
		public const string ParameterName_Target = "Target";

		internal const string AntFileNameExtension = ".ant";
		internal const string BuildScriptFileName = "build.ant";

		internal static string[] GetTargetKeysFromBuildScript(string buildScriptFullName)
		{
			var targets = new HashSet<string>();

			try
			{
				var antProjectElement = System.Xml.Linq.XElement.Load(buildScriptFullName);

				foreach (var target in antProjectElement.GetElementsByLocalName("target"))
				{
					var name = target.GetAttributeByLocalName("name")?.Value ?? string.Empty;

					if (!string.IsNullOrWhiteSpace(name))
					{
						targets.Add(name);
					}
				}
			}
			catch
			{
			}

			return targets.ToArray();
		}

		internal static bool IsBuildScriptFile(string buildScriptFullName)
		{
			if (string.Equals(System.IO.Path.GetFileName(buildScriptFullName), BuildScriptFileName, StringComparison.CurrentCultureIgnoreCase))
			{
				return GetTargetKeysFromBuildScript(buildScriptFullName).Any();
			}

			return false;
		}
	}
}
