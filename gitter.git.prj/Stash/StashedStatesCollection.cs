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
	using System.Globalization;

	using gitter.Framework;

	using gitter.Git.AccessLayer;

	using Resources = gitter.Git.Properties.Resources;

	/// <summary>Repository's stash.</summary>
	public sealed class StashedStatesCollection : GitObject, IEnumerable<StashedState>
	{
		#region Events

		/// <summary>New <see cref="StashedState"/> was created.</summary>
		public event EventHandler<StashedStateEventArgs> StashedStateCreated;

		/// <summary><see cref="StashedState"/> was dropped.</summary>
		public event EventHandler<StashedStateEventArgs> StashedStateDeleted;

		/// <summary>Invokes <see cref="StashedStateCreated"/> event.</summary>
		/// <param name="state">Created stash.</param>
		private void InvokeStashedStateCreated(StashedState state)
		{
			var handler = StashedStateCreated;
			if(handler != null) handler(this, new StashedStateEventArgs(state));
		}

		/// <summary>Invokes <see cref="StashedStateDeleted"/> event.</summary>
		/// <param name="state">Dropped state.</param>
		private void InvokeStashedStateDeleted(StashedState state)
		{
			state.MarkAsDeleted();
			var handler = StashedStateDeleted;
			if(handler != null) handler(this, new StashedStateEventArgs(state));
		}

		#endregion

		#region Data

		private readonly List<StashedState> _stash;

		#endregion

		#region .ctor

		/// <summary>Create <see cref="StashedStatesCollection"/>.</summary>
		/// <param name="repository">Related <see cref="Repository"/>.</param>
		internal StashedStatesCollection(Repository repository)
			: base(repository)
		{
			_stash = new List<StashedState>();
		}

		#endregion

		#region Drop()

		internal IAsyncAction DropAsync(StashedState stashedState)
		{
			Verify.Argument.IsValidGitObject(stashedState, Repository, "stashedState");

			return AsyncAction.Create(
				stashedState,
				(state, monitor) =>
				{
					state.Drop();
				},
				Resources.StrStashDrop,
				Resources.StrPerformingStashDrop.AddEllipsis());
		}

		internal void Drop(StashedState stashedState)
		{
			Verify.Argument.IsValidGitObject(stashedState, Repository, "stashedState");

			using(Repository.Monitor.BlockNotifications(
				RepositoryNotifications.StashChanged))
			{
				Repository.Accessor.StashDrop(
					new StashDropParameters(((IRevisionPointer)stashedState).Pointer));
			}

			lock(SyncRoot)
			{
				int index = _stash.IndexOf(stashedState);
				if(index >= 0)
				{
					_stash.RemoveAt(index);
					for(int i = index; i < _stash.Count; ++i)
					{
						_stash[i].Index = i;
					}

					InvokeStashedStateDeleted(stashedState);
				}
			}
		}

		public void Drop()
		{
			Verify.State.IsTrue(_stash.Count != 0);

			using(Repository.Monitor.BlockNotifications(
				RepositoryNotifications.StashChanged))
			{
				Repository.Accessor.StashDrop(
					new StashDropParameters());
			}

			lock(SyncRoot)
			{
				var stashedState = _stash[0];
				_stash.RemoveAt(0);
				for(int i = 0; i < _stash.Count; ++i)
				{
					_stash[i].Index = i;
				}

				InvokeStashedStateDeleted(stashedState);
			}
		}

		public IAsyncAction DropAsync()
		{
			Verify.State.IsTrue(_stash.Count != 0);

			return AsyncAction.Create(
				new StashDropParameters(),
				(data, monitor) =>
				{
					using(Repository.Monitor.BlockNotifications(
						RepositoryNotifications.StashChanged))
					{
						Repository.Accessor.StashDrop(data);
					}

					lock(SyncRoot)
					{
						var stashedState = _stash[0];

						_stash.RemoveAt(0);
						for(int i = 0; i < _stash.Count; ++i)
						{
							_stash[i].Index = i;
						}

						InvokeStashedStateDeleted(stashedState);
					}
				},
				Resources.StrStashDrop,
				Resources.StrsRemovingStashedChanges.AddEllipsis());
		}

		#endregion

		#region Clear()

		public void Clear()
		{
			using(Repository.Monitor.BlockNotifications(
				RepositoryNotifications.StashChanged))
			{
				Repository.Accessor.StashClear(
					new StashClearParameters());
			}
			lock(SyncRoot)
			{
				if(_stash.Count != 0)
				{
					for(int i = _stash.Count - 1; i >= 0; --i)
					{
						var ss = _stash[i];
						_stash.RemoveAt(i);
						InvokeStashedStateDeleted(ss);
					}
				}
			}
		}

		public IAsyncAction ClearAsync()
		{
			return AsyncAction.Create(
				new
				{
					Repository = Repository,
					List = _stash,
					OnDeleted = (Action<StashedState>)InvokeStashedStateDeleted,
				},
				(data, monitor) =>
				{
					var repository = data.Repository;
					var stash = data.List;
					var onDeleted = data.OnDeleted;
					using(repository.Monitor.BlockNotifications(RepositoryNotifications.StashChanged))
					{
						repository.Accessor.StashClear(
							new StashClearParameters());
					}
					lock(stash)
					{
						if(stash.Count != 0)
						{
							for(int i = stash.Count - 1; i >= 0; --i)
							{
								var stashedState = stash[i];
								stash.RemoveAt(i);
								onDeleted(stashedState);
							}
						}
					}
				},
				Resources.StrStashClear,
				Resources.StrsRemovingStashedChanges.AddEllipsis());
		}

		#endregion

		public StashedState MostRecentState
		{
			get
			{
				lock(SyncRoot)
				{
					if(_stash.Count == 0) return null;
					return _stash[0];
				}
			}
		}

		public StashedState this[int index]
		{
			get
			{
				lock(SyncRoot)
				{
					return _stash[index];
				}
			}
		}

		public StashedState this[string name]
		{
			get
			{
				const string ExcMessage = "Invalid stash name format.";

				Verify.Argument.IsNotNull(name, "name");
				Verify.Argument.IsTrue(name.StartsWith(GitConstants.StashFullName), "name", ExcMessage);

				var sfnl = GitConstants.StashFullName.Length;
				if(name.Length == sfnl)
				{
					lock(SyncRoot)
					{
						return _stash[0];
					}
				}
				else
				{
					Verify.Argument.IsTrue(name.Length - sfnl - 3 >= 1, "name", ExcMessage);
					Verify.Argument.IsTrue(name[sfnl + 0] == '@', "name", ExcMessage);
					Verify.Argument.IsTrue(name[sfnl + 1] == '{', "name", ExcMessage);
					Verify.Argument.IsTrue(name[name.Length - 1] == '}', "name", ExcMessage);
					var s = name.Substring(sfnl + 2, name.Length - sfnl - 3);
					int index;
					Verify.Argument.IsTrue(
						int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out index),
						"name", ExcMessage);
					lock(SyncRoot)
					{
						return _stash[index];
					}
				}
			}
		}

		public int Count
		{
			get
			{
				lock(SyncRoot)
				{
					return _stash.Count;
				}
			}
		}

		/// <summary>Returns if this stash is empty.</summary>
		/// <value><c>true</c> if this stash is empty, <c>false</c> otherwise.</value>
		public bool IsEmpty
		{
			get
			{
				lock(SyncRoot)
				{
					return _stash.Count == 0;
				}
			}
		}

		/// <summary>Object used for cross-thread synchronization.</summary>
		public object SyncRoot
		{
			get { return _stash; }
		}

		/// <summary>Refresh stash content.</summary>
		public void Refresh()
		{
			var top = Repository.Accessor.QueryStashTop(
				new QueryStashTopParameters(false));
			var stash = (top == null)?
				new StashedStateData[0] : 
				Repository.Accessor.QueryStash(new QueryStashParameters());
			lock(SyncRoot)
			lock(Repository.Revisions.SyncRoot)
			{
				if(_stash.Count != 0)
				{
					if(stash.Count == 0)
					{
						for(int i = _stash.Count - 1; i >= 0; --i)
						{
							var s = _stash[i];
							_stash.RemoveAt(i);
							InvokeStashedStateDeleted(s);
						}
					}
					else
					{
						var d = new Dictionary<string, StashedState>(_stash.Count);
						foreach(var s in _stash)
						{
							d.Add(s.Revision.Hash, s);
						}
						_stash.Clear();
						foreach(var ssinfo in stash)
						{
							StashedState stashedState;
							if(!d.TryGetValue(ssinfo.Revision.SHA1, out stashedState))
							{
								stashedState = ObjectFactories.CreateStashedState(Repository, ssinfo);
								_stash.Add(stashedState);
								InvokeStashedStateCreated(stashedState);
							}
							else
							{
								ObjectFactories.UpdateStashedState(stashedState, ssinfo);
								d.Remove(ssinfo.Revision.SHA1);
								_stash.Add(stashedState);
							}
						}
						if(d.Count != 0)
						{
							foreach(var ss in d.Values)
							{
								InvokeStashedStateDeleted(ss);
							}
						}
					}
				}
				else
				{
					if(stash.Count != 0)
					{
						foreach(var stashedStateData in stash)
						{
							var stashedState = ObjectFactories.CreateStashedState(Repository, stashedStateData);
							_stash.Add(stashedState);
							InvokeStashedStateCreated(stashedState);
						}
					}
				}
			}
		}

		internal void NotifyCleared()
		{
			lock(SyncRoot)
			{
				if(_stash.Count != 0)
				{
					for(int i = _stash.Count - 1; i >= 0; --i)
					{
						var ss = _stash[i];
						_stash.RemoveAt(i);
						InvokeStashedStateDeleted(ss);
					}
				}
			}
		}

		#region Apply()

		internal IAsyncAction ApplyAsync(StashedState stashedState, bool restoreIndex)
		{
			Verify.Argument.IsValidGitObject(stashedState, Repository, "stashedState");

			return AsyncAction.Create(
				Tuple.Create(stashedState, restoreIndex),
				(data, monitor) =>
				{
					data.Item1.Apply(data.Item2);
				},
				Resources.StrStashApply,
				Resources.StrPerformingStashApply.AddEllipsis());
		}

		internal void Apply(StashedState stashedState, bool restoreIndex)
		{
			Verify.Argument.IsValidGitObject(stashedState, Repository, "stashedState");

			using(Repository.Monitor.BlockNotifications(
				RepositoryNotifications.StashChanged,
				RepositoryNotifications.WorktreeUpdated,
				RepositoryNotifications.IndexUpdated))
			{
				Repository.Accessor.StashApply(
					new StashApplyParameters(((IRevisionPointer)stashedState).Pointer, restoreIndex));
			}

			Repository.Status.Refresh();
		}

		internal void Apply(StashedState state)
		{
			Apply(state, false);
		}

		public IAsyncAction ApplyAsync(bool restoreIndex)
		{
			Verify.State.IsTrue(_stash.Count != 0);

			return AsyncAction.Create(
				new
				{
					Repository		= Repository,
					RestoreIndex	= restoreIndex,
				},
				(data, monitor) =>
				{
					data.Repository.Stash.Apply(data.RestoreIndex);
				},
				Resources.StrStashPop,
				Resources.StrPerformingStashApply.AddEllipsis());
		}

		public void Apply(bool restoreIndex)
		{
			Verify.State.IsTrue(_stash.Count != 0);

			Apply(_stash[0], restoreIndex);
		}

		public void Apply()
		{
			Apply(false);
		}

		#endregion

		#region Pop

		internal IAsyncAction PopAsync(StashedState stashedState, bool restoreIndex)
		{
			Verify.Argument.IsValidGitObject(stashedState, Repository, "stashedState");

			return AsyncAction.Create(
				new
				{
					Repository		= Repository,
					StashedState	= stashedState,
					RestoreIndex	= restoreIndex,
				},
				(data, monitor) =>
				{
					data.Repository.Stash.Pop(
						data.StashedState, data.RestoreIndex);
				},
				Resources.StrStashPop,
				Resources.StrPerformingStashPop.AddEllipsis());
		}

		internal void Pop(StashedState stashedState, bool restoreIndex)
		{
			Verify.Argument.IsValidGitObject(stashedState, Repository, "stashedState");

			using(Repository.Monitor.BlockNotifications(
				RepositoryNotifications.StashChanged,
				RepositoryNotifications.WorktreeUpdated,
				RepositoryNotifications.IndexUpdated))
			{
				Repository.Accessor.StashPop(
					new StashPopParameters(((IRevisionPointer)stashedState).Pointer, restoreIndex));
			}

			lock(SyncRoot)
			{
				_stash.RemoveAt(stashedState.Index);
				for(int i = stashedState.Index; i < _stash.Count; ++i)
				{
					--_stash[i].Index;
				}
				InvokeStashedStateDeleted(stashedState);
			}

			Repository.Status.Refresh();
		}

		internal void Pop(StashedState state)
		{
			Pop(state, false);
		}

		public IAsyncAction PopAsync(bool restoreIndex)
		{
			Verify.State.IsTrue(_stash.Count != 0);

			return AsyncAction.Create(
				Tuple.Create(Repository, restoreIndex),
				(data, monitor) =>
				{
					data.Item1.Stash.Pop(data.Item2);
				},
				Resources.StrStashPop,
				Resources.StrPerformingStashPop.AddEllipsis());
		}

		public void Pop(bool restoreIndex)
		{
			Verify.State.IsTrue(_stash.Count != 0);

			Pop(_stash[0], restoreIndex);
		}

		public void Pop()
		{
			Pop(false);
		}

		#endregion

		#region ToBranch()

		internal Branch ToBranch(StashedState stashedState, string name)
		{
			Verify.Argument.IsValidGitObject(stashedState, Repository, "stashedState");

			using(Repository.Monitor.BlockNotifications(
				RepositoryNotifications.BranchChanged,
				RepositoryNotifications.Checkout,
				RepositoryNotifications.WorktreeUpdated,
				RepositoryNotifications.IndexUpdated,
				RepositoryNotifications.StashChanged))
			{
				Repository.Accessor.StashToBranch(
					new StashToBranchParameters(((IRevisionPointer)stashedState).Pointer, name));
			}

			lock(SyncRoot)
			{
				_stash.RemoveAt(stashedState.Index);
				for(int i = stashedState.Index; i < _stash.Count; ++i)
				{
					--_stash[i].Index;
				}
				InvokeStashedStateDeleted(stashedState);
			}

			Repository.Status.Refresh();

			var branchInformation = new BranchData(name,
				stashedState.Revision.Parents[0].Hash, false, true);
			var branch = Repository.Refs.Heads.NotifyCreated(branchInformation);
			Repository.Head.Pointer = branch;

			return branch;
		}

		#endregion

		#region Save()

		public StashedState Save(bool keepIndex)
		{
			return Save(keepIndex, false, null);
		}

		public StashedState Save(bool keepIndex, bool includeUntracked, string message)
		{
			Verify.State.IsFalse(Repository.IsEmpty,
				Resources.ExcCantDoOnEmptyRepository.UseAsFormat("stash save"));

			bool created;
			using(Repository.Monitor.BlockNotifications(
				RepositoryNotifications.IndexUpdated,
				RepositoryNotifications.WorktreeUpdated,
				RepositoryNotifications.StashChanged))
			{
				created = Repository.Accessor.StashSave(
					new StashSaveParameters(message, keepIndex, includeUntracked));
			}

			if(created)
			{
				var stashTopData = Repository.Accessor.QueryStashTop(
					new QueryStashTopParameters(true));
				Revision revision;
				lock(Repository.Revisions.SyncRoot)
				{
					revision = ObjectFactories.CreateRevision(Repository, stashTopData);
				}
				var res = new StashedState(Repository, 0, revision);

				lock(SyncRoot)
				{
					_stash.Insert(0, res);
					for(int i = 1; i < _stash.Count; ++i)
					{
						++_stash[i].Index;
					}

					InvokeStashedStateCreated(res);
				}

				Repository.OnCommitCreated(res.Revision);
				Repository.Status.Refresh();

				return res;
			}
			else
			{
				return null;
			}
		}

		public IAsyncFunc<StashedState> SaveAsync(bool keepIndex, bool includeUntracked, string message)
		{
			return new AsyncFunc<Repository, StashedState>(
				Repository,
				(repository, mon) =>
				{
					return repository.Stash.Save(keepIndex, includeUntracked, message);
				},
				Resources.StrStash,
				Resources.StrPerformingStashSave.AddEllipsis());
		}

		#endregion

		#region IEnumerable<StashedState> Members

		public IEnumerator<StashedState> GetEnumerator()
		{
			return _stash.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _stash.GetEnumerator();
		}

		#endregion
	}
}
