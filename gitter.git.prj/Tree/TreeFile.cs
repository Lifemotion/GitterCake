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
	using System.IO;

	using gitter.Framework;

	using gitter.Git.AccessLayer;

	using Resources = gitter.Git.Properties.Resources;

	/// <summary>Represents a file in a directory.</summary>
	public sealed class TreeFile : TreeItem
	{
		#region Data

		private ConflictType _conflictType;
		private long _size;

		#endregion

		#region .ctor

		public TreeFile(Repository repository, string relativePath, TreeDirectory parent, FileStatus status, string name)
			: base(repository, relativePath, parent, status, name)
		{
		}

		public TreeFile(Repository repository, string relativePath, TreeDirectory parent, FileStatus status, string name, long size)
			: base(repository, relativePath, parent, status, name)
		{
			_size = size;
		}

		#endregion

		public override TreeItemType ItemType
		{
			get { return TreeItemType.Blob; }
		}

		public ConflictType ConflictType
		{
			get { return _conflictType; }
			internal set { _conflictType = value; }
		}

		public long Size
		{
			get { return _size; }
		}

		public void ResolveConflict(ConflictResolution resolution)
		{
			Verify.State.IsFalse(ConflictType == Git.ConflictType.None);

			using(Repository.Monitor.BlockNotifications(
				RepositoryNotifications.IndexUpdated,
				RepositoryNotifications.WorktreeUpdated))
			{
				switch(resolution)
				{
					case ConflictResolution.DeleteFile:
						Remove(true);
						break;
					case ConflictResolution.KeepModifiedFile:
						Stage(AddFilesMode.Default);
						break;
					case ConflictResolution.UseOurs:
						UseOurs();
						Stage(AddFilesMode.Default);
						break;
					case ConflictResolution.UseTheirs:
						UseTheirs();
						Stage(AddFilesMode.Default);
						break;
					default:
						throw new ArgumentException(
							"Unknown ConflictResolution value: {0}".UseAsFormat(resolution),
							"resolution");
				}
			}
		}

		private void UseTheirs()
		{
			Repository.Accessor.CheckoutFiles(
				new CheckoutFilesParameters(RelativePath)
				{
					Mode = CheckoutFileMode.Theirs
				});
		}

		private void UseOurs()
		{
			Repository.Accessor.CheckoutFiles(
				new CheckoutFilesParameters(RelativePath)
				{
					Mode = CheckoutFileMode.Ours
				});
		}

		#region mergetool

        private void RunMergeToolExternal(IAsyncProgressMonitor monitor)
        {
            try
            {
                using (Repository.Monitor.BlockNotifications(
                    RepositoryNotifications.IndexUpdated,
                    RepositoryNotifications.WorktreeUpdated))
                {
                    Repository.Accessor.RunMergeToolExternal(
                        new RunMergeToolParameters(FullPath)
                        {
                            Monitor = monitor,
                        });
                }
            }
            finally
            {
                Repository.Status.Refresh();
            }
        }

		private void RunMergeTool(MergeTool mergeTool, IAsyncProgressMonitor monitor)
		{
			try
			{
				using(Repository.Monitor.BlockNotifications(
					RepositoryNotifications.IndexUpdated,
					RepositoryNotifications.WorktreeUpdated))
				{
					Repository.Accessor.RunMergeTool(
						new RunMergeToolParameters(RelativePath)
						{
							Monitor = monitor,
							Tool = mergeTool == null ? null : mergeTool.Name,
						});
				}
			}
			finally
			{
				Repository.Status.Refresh();
			}
		}

		public void RunMergeTool()
		{
			Verify.State.IsFalse(ConflictType == Git.ConflictType.None);

			RunMergeTool(null, null);
		}

		public void RunMergeTool(MergeTool mergeTool)
		{
			Verify.Argument.IsNotNull(mergeTool, "mergeTool");
			Verify.State.IsFalse(ConflictType == Git.ConflictType.None);

			RunMergeTool(mergeTool, null);
		}

		public IAsyncAction RunMergeToolAsync()
		{
			Verify.State.IsFalse(ConflictType == Git.ConflictType.None);

			return AsyncAction.Create(this,
				(file, mon) =>
				{
					file.RunMergeTool(null, mon);
				},
				Resources.StrRunningMergeTool,
				Resources.StrWaitingMergeTool.AddEllipsis(),
				true);
		}

        public IAsyncAction RunMergeToolExternalAsync()
        {
            Verify.State.IsFalse(ConflictType == Git.ConflictType.None);

            return AsyncAction.Create(new
            {
                File = this,
            },
                (data, mon) =>
                {
                    data.File.RunMergeToolExternal(mon);
                },
                Resources.StrRunningMergeTool,
                Resources.StrWaitingMergeTool.AddEllipsis(),
                true);
        }

		public IAsyncAction RunMergeToolAsync(MergeTool mergeTool)
		{
			Verify.State.IsFalse(ConflictType == Git.ConflictType.None);

			return AsyncAction.Create(new
				{
					File = this,
					Tool = mergeTool,
				},
				(data, mon) =>
				{
					data.File.RunMergeTool(data.Tool, mon);
				},
				Resources.StrRunningMergeTool,
				Resources.StrWaitingMergeTool.AddEllipsis(),
				true);
		}

		#endregion
	}
}
