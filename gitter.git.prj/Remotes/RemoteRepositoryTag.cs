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

namespace gitter.Git
{
	using System;
	using System.Collections.Generic;

	/// <summary>Represents a tag on remote repository.</summary>
	public sealed class RemoteRepositoryTag : BaseRemoteReference
	{
		private TagType _tagType;

		internal RemoteRepositoryTag(RemoteReferencesCollection refs, string name, TagType type, string hash)
			: base(refs, name, hash)
		{
			_tagType = type;
		}

		public TagType TagType
		{
			get { return _tagType; }
			internal set { _tagType = value; }
		}

		protected override void DeleteCore()
		{
			References.RemoveTag(this);
		}

		public override ReferenceType ReferenceType
		{
			get { return ReferenceType.Tag; }
		}
	}
}
