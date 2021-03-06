#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2013  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

namespace gitter.Git.AccessLayer
{
	using System;
	using System.Collections.Generic;

	using gitter.Git.AccessLayer.CLI;

	internal sealed partial class RepositoryCLI : INotesAccessor
	{
		/// <summary>Get list of all note objects.</summary>
		/// <param name="parameters"><see cref="QueryNotesParameters"/>.</param>
		/// <returns>List of all note objects.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public IList<NoteData> QueryNotes(QueryNotesParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			if(!GitFeatures.AdvancedNotesCommands.IsAvailableFor(_gitCLI))
			{
				return new NoteData[0];
			}

			var cmd = new NotesCommand(NotesCommand.List());
			var output = _executor.ExecCommand(cmd);
			output.ThrowOnBadReturnCode();

			var res = new List<NoteData>();
			var notes = output.Output;
			if(notes.Length > 81)
			{
				var parser = new GitParser(notes);
				while(!parser.IsAtEndOfString)
				{
					var noteSHA1 = parser.ReadString(40, 1);
					var objectSHA1 = parser.ReadString(40, 1);
					res.Add(new NoteData(noteSHA1, objectSHA1, null));
				}
			}
			return res;
		}

		/// <summary>Append new note to object.</summary>
		/// <param name="parameters"><see cref="AppendNoteParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public void AppendNote(AppendNoteParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			var cmd = new NotesCommand(
				NotesCommand.Append(),
				NotesCommand.Message(parameters.Message),
				new CommandArgument(parameters.Revision));
			var output = _executor.ExecCommand(cmd);
			output.ThrowOnBadReturnCode();
		}

		/// <summary>Remove all notes for non-existing/unreachable objects.</summary>
		public void PruneNotes()
		{
			var cmd = new NotesCommand(NotesCommand.Prune());
			var output = _executor.ExecCommand(cmd);
			output.ThrowOnBadReturnCode();
		}
	}
}
