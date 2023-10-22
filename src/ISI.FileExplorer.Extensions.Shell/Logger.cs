#region Copyright & License
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

namespace ISI.FileExplorer.Extensions.Shell
{
	public class Logger
	{
		public static string GetLogDirectory()
		{
			var directory = @"C:\ProgramData\ISI.FileExplorer.Extensions";

			System.IO.Directory.CreateDirectory(directory);

			return directory;
		}

		public static string GetLogFileName()
		{
			return $"ISI.FileExplorer.Extensions.{DateTime.Today.Date:yyyyMMdd}.log";
		}

		public static string GetLogFullName()
		{
			var directory = @"C:\ProgramData\ISI.FileExplorer.Extensions";

			System.IO.Directory.CreateDirectory(directory);

			return System.IO.Path.Combine(GetLogDirectory(), GetLogFileName());
		}

		private static bool? _ShouldWriteLog = null;

		private static bool ShouldWriteLog()
		{
			if (_ShouldWriteLog.HasValue)
			{
				return _ShouldWriteLog.Value;
			}

			if (System.IO.File.Exists(GetLogFullName()))
			{
				_ShouldWriteLog = true;
				return true;
			}

			_ShouldWriteLog = false;

			return _ShouldWriteLog.GetValueOrDefault();
		}
		private static void AppendToLog(string log)
		{
			var logFullName = GetLogFullName();

			System.IO.File.AppendAllText(logFullName, log);
		}
		public static void AddToLog(string note)
		{
			if (ShouldWriteLog())
			{
				AppendToLog(string.Format("{0:u}\t{1}\n", DateTime.UtcNow, note));
			}
		}
		public static void AddToLog(IEnumerable<string> notes)
		{
			if (ShouldWriteLog())
			{
				AppendToLog(string.Format("{0:u}\t{1}\n", DateTime.UtcNow, string.Join("\t\n", notes)));
			}
		}



		public static void AddToLog(string section, string note)
		{
			if (ShouldWriteLog())
			{
				AppendToLog(string.Format("{0:u}\t{1}:\n\t{2}\n", DateTime.UtcNow, section, note));
			}
		}
		public static void AddToLog(string section, IEnumerable<string> notes)
		{
			if (ShouldWriteLog())
			{
				AppendToLog(string.Format("{0:u}\t{1}:\n\t{2}\n", DateTime.UtcNow, section, string.Join("\t\n", notes)));
			}
		}
		public static void AddToLog(IEnumerable<KeyValuePair<string, string>> sectionNotes)
		{
			if (ShouldWriteLog())
			{
				AppendToLog(string.Format("{0:u}\n\t{1}\n", DateTime.UtcNow, string.Join("\n\t", sectionNotes.Select(sectionNote => string.Format("{0}: {1}", sectionNote.Key, sectionNote.Value)))));
			}
		}
		public static void AddToLog(IEnumerable<KeyValuePair<string, IEnumerable<string>>> sectionNotes)
		{
			if (ShouldWriteLog())
			{
				AppendToLog(string.Format("{0:u}\n\t{1}\n", DateTime.UtcNow, string.Join("\n\t", sectionNotes.Select(sectionNote => string.Format("{0}:\n\t\t{1}", sectionNote.Key, string.Join("\n\t\t", sectionNote.Value))))));
			}
		}



		public static void AddToLog(string owner, string section, string note)
		{
			if (ShouldWriteLog())
			{
				AppendToLog(string.Format("{0:u}\t{1}\n\t{2}:\n\t\t{3}\n", DateTime.UtcNow, owner, section, note));
			}
		}
		public static void AddToLog(string owner, string section, IEnumerable<string> notes)
		{
			if (ShouldWriteLog())
			{
				AppendToLog(string.Format("{0:u}\t{1}\n\t{2}:\n\t\t{3}\n", DateTime.UtcNow, owner, section, string.Join("\t\t\n", notes)));
			}
		}
		public static void AddToLog(string owner, IEnumerable<KeyValuePair<string, string>> sectionNotes)
		{
			if (ShouldWriteLog())
			{
				AppendToLog(string.Format("{0:u}\t{1}\n\t{2}\n", DateTime.UtcNow, owner, string.Join("\n\t", sectionNotes.Select(sectionNote => string.Format("{0}: {1}", sectionNote.Key, sectionNote.Value)))));
			}
		}
		public static void AddToLog(string owner, IEnumerable<KeyValuePair<string, IEnumerable<string>>> sectionNotes)
		{
			if (ShouldWriteLog())
			{
				AppendToLog(string.Format("{0:u}\t{1}\n\t{2}\n", DateTime.UtcNow, owner, string.Join("\n\t", sectionNotes.Select(sectionNote => string.Format("{0}:\n\t\t{1}", sectionNote.Key, string.Join("\n\t\t", sectionNote.Value))))));
			}
		}
	}
}
