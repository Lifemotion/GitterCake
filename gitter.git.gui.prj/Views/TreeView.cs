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

namespace gitter.Git.Gui.Views
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.IO;
	using System.Drawing;
	using System.Diagnostics;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Services;
	using gitter.Framework.Controls;
	using gitter.Framework.Configuration;

	using gitter.Git.Gui.Controls;

	using Resources = gitter.Git.Gui.Properties.Resources;

	[ToolboxItem(false)]
	partial class TreeView : GitViewBase
	{
		private ITreeSource _treeSource;
		private Tree _wTree;
		private TreeDirectory _currentDirectory;

		private TreeToolbar _toolBar;

		public event EventHandler CurrentDirectoryChanged;

		private sealed class TreeMenu : ContextMenuStrip
		{
			public TreeMenu(ITreeSource treeSource, TreeDirectoryListItem item)
			{
				Verify.Argument.IsNotNull(item, "item");

				Items.Add(GuiItemFactory.GetExpandAllItem<ToolStripMenuItem>(item));
				Items.Add(GuiItemFactory.GetCollapseAllItem<ToolStripMenuItem>(item));

				if(treeSource != null)
				{
					Items.Add(new ToolStripSeparator());
					Items.Add(GuiItemFactory.GetPathHistoryItem<ToolStripMenuItem>(treeSource.Revision, item.DataContext));
					Items.Add(new ToolStripSeparator());
					Items.Add(GuiItemFactory.GetCheckoutPathItem<ToolStripMenuItem>(treeSource.Revision, item.DataContext));
				}
			}
		}

		public TreeView(IDictionary<string, object> parameters, GuiProvider gui)
			: base(Guids.TreeViewGuid, gui, parameters)
		{
			InitializeComponent();

			_splitContainer.BackColor = GitterApplication.Style.Colors.WorkArea;
			_splitContainer.Panel1.BackColor = GitterApplication.Style.Colors.Window;
			_splitContainer.Panel2.BackColor = GitterApplication.Style.Colors.Window;

			_directoryTree.Columns.ShowAll(column => column.Id == (int)ColumnId.Name);
			_directoryTree.Columns[0].SizeMode = ColumnSizeMode.Auto;
			_treeContent.Columns.ShowAll(column => column.Id == (int)ColumnId.Name || column.Id == (int)ColumnId.Size);
			_treeContent.Columns.GetById((int)ColumnId.Name).SizeMode = ColumnSizeMode.Auto;

			_directoryTree.SelectionChanged += OnDirectoryTreeSelectionChanged;
			_directoryTree.ItemContextMenuRequested +=
				(sender, e) =>
				{
					var menu = new TreeMenu(Parameters["tree"] as ITreeSource, (TreeDirectoryListItem)e.Item);
					Utility.MarkDropDownForAutoDispose(menu);
					e.ContextMenu = menu;
					e.OverrideDefaultMenu = true;
				};
			_directoryTree.PreviewKeyDown += OnKeyDown;

			_treeContent.ItemContextMenuRequested += OnContextMenuRequested;
			_treeContent.SelectionChanged += OnTreeContentSelectionChanged;
			_treeContent.PreviewKeyDown += OnKeyDown;

			Text = Resources.StrTree + " " + ((ITreeSource)parameters["tree"]).DisplayName;

			AddTopToolStrip(_toolBar = new TreeToolbar(this));
		}

		private void OnTreeContentSelectionChanged(object sender, EventArgs e)
		{
			//var rev = Parameters["tree"] as RevisionTreeSource;
			//if(rev != null)
			//{
			//    var items = _treeContent.SelectedItems;
			//    if(items.Count == 0)
			//    {
			//        ShowContextualDiffTool(null);
			//    }
			//    else
			//    {
			//        ShowContextualDiffTool(new RevisionChangesDiffSource(rev.Revision,
			//            items.Cast<IWorktreeListItem>().Select(item => item.WorktreeItem.RelativePath).ToList()));
			//    }
			//}
		}

		private void OnContextMenuRequested(object sender, ItemContextMenuRequestEventArgs e)
		{
			var rts = Parameters["tree"] as ITreeSource;
			if(rts != null)
			{
				var item = ((ITreeItemListItem)e.Item);
				var file = item.TreeItem as TreeFile;
				if(file != null)
				{
					var menu = new ContextMenuStrip();
					menu.Items.AddRange(
						new ToolStripItem[]
						{
							GuiItemFactory.GetExtractAndOpenFileItem<ToolStripMenuItem>(_wTree, file.RelativePath),
							GuiItemFactory.GetExtractAndOpenFileWithItem<ToolStripMenuItem>(_wTree, file.RelativePath),
							GuiItemFactory.GetSaveAsItem<ToolStripMenuItem>(_wTree, file.RelativePath),
							new ToolStripSeparator(),
							new ToolStripMenuItem(Resources.StrCopyToClipboard, null,
								new ToolStripItem[]
								{
									GuiItemFactory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrFileName, file.Name),
									GuiItemFactory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrRelativePath, file.RelativePath),
									GuiItemFactory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrFullPath, file.FullPath),
								}),
							new ToolStripSeparator(),
							GuiItemFactory.GetBlameItem<ToolStripMenuItem>(rts.Revision, file),
							GuiItemFactory.GetPathHistoryItem<ToolStripMenuItem>(rts.Revision, file),
							new ToolStripSeparator(),
							GuiItemFactory.GetCheckoutPathItem<ToolStripMenuItem>(rts.Revision, file),
						});
					Utility.MarkDropDownForAutoDispose(menu);
					e.ContextMenu = menu;
					e.OverrideDefaultMenu = true;
					return;
				}
				var directory = item.TreeItem as TreeDirectory;
				if(directory != null)
				{
					var menu = new ContextMenuStrip();
					menu.Items.AddRange(
						new ToolStripItem[]
						{
							new ToolStripMenuItem(Resources.StrOpen, null, (s, args) => e.Item.Activate()),
							GuiItemFactory.GetPathHistoryItem<ToolStripMenuItem>(rts.Revision, directory),
							new ToolStripSeparator(),
							GuiItemFactory.GetCheckoutPathItem<ToolStripMenuItem>(rts.Revision, directory),
						});
					Utility.MarkDropDownForAutoDispose(menu);
					e.ContextMenu = menu;
					e.OverrideDefaultMenu = true;
					return;
				}
				var commit = ((ITreeItemListItem)e.Item).TreeItem as TreeCommit;
				if(commit != null)
				{
					var menu = new ContextMenuStrip();
					menu.Items.AddRange(
						new ToolStripItem[]
						{
							GuiItemFactory.GetPathHistoryItem<ToolStripMenuItem>(rts.Revision, commit),
							new ToolStripSeparator(),
							new ToolStripMenuItem(Resources.StrCopyToClipboard, null,
								new ToolStripItem[]
								{
									GuiItemFactory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrName, commit.Name),
									GuiItemFactory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrRelativePath, commit.RelativePath),
									GuiItemFactory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrFullPath, commit.FullPath),
								}),
							new ToolStripSeparator(),
							GuiItemFactory.GetCheckoutPathItem<ToolStripMenuItem>(rts.Revision, commit),
						});
					Utility.MarkDropDownForAutoDispose(menu);
					e.ContextMenu = menu;
					e.OverrideDefaultMenu = true;
					return;
				}
			}
		}

		/// <summary>
		/// Gets a value indicating whether this instance is document.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is document; otherwise, <c>false</c>.
		/// </value>
		public override bool IsDocument
		{
			get { return true; }
		}

		public TreeDirectory CurrentDirectory
		{
			get { return _currentDirectory; }
			set
			{
				Verify.Argument.IsNotNull(value, "value");

				if(_currentDirectory != value)
				{
					var item = FindDirectoryEntry(value);
					if(item == null) throw new ArgumentException("value");
					item.FocusAndSelect();
					_currentDirectory = value;
					CurrentDirectoryChanged.Raise(this);
				}
			}
		}

		private void OnDirectoryTreeSelectionChanged(object sender, EventArgs e)
		{
			if(_directoryTree.SelectedItems.Count != 0)
			{
				var treeItem = (_directoryTree.SelectedItems[0] as TreeDirectoryListItem);
				if(_currentDirectory != treeItem.DataContext)
				{
					_currentDirectory = treeItem.DataContext;
					_treeContent.SetTree(_currentDirectory, TreeListBoxMode.ShowDirectoryContent);
					CurrentDirectoryChanged.Raise(this);
				}
			}
		}

		public override void ApplyParameters(IDictionary<string, object> parameters)
		{
			_treeSource = (ITreeSource)parameters["tree"];
			_wTree = _treeSource.GetTree();
			_currentDirectory = _wTree.Root;
			CurrentDirectoryChanged.Raise(this);
			_directoryTree.SetTree(_currentDirectory, TreeListBoxMode.ShowDirectoryTree);
			_treeContent.SetTree(_currentDirectory, TreeListBoxMode.ShowDirectoryContent);
			Text = Resources.StrTree + " " + _treeSource.DisplayName;
		}

		private TreeDirectoryListItem FindDirectoryEntry(TreeDirectory folder)
		{
			return FindDirectoryEntry((TreeDirectoryListItem)_directoryTree.Items[0], folder);
		}

		private static TreeDirectoryListItem FindDirectoryEntry(TreeDirectoryListItem root, TreeDirectory folder)
		{
			if(root.DataContext == folder) return root;
			foreach(TreeDirectoryListItem item in root.Items)
			{
				var subSearch = FindDirectoryEntry(item, folder);
				if(subSearch != null) return subSearch;
			}
			return null;
		}

		private void OnItemActivated(object sender, ItemEventArgs e)
		{
			var item = e.Item as TreeFileListItem;
			if(item != null)
			{
				var fileName = _wTree.ExtractBlobToTemporaryFile(item.DataContext.RelativePath);
				if(!string.IsNullOrWhiteSpace(fileName))
				{
					var process = Utility.CreateProcessFor(fileName);
					process.EnableRaisingEvents = true;
					process.Exited += OnFileViewerProcessExited;
					process.Start();
				}
			}
			else
			{
				var folderItem = e.Item as TreeDirectoryListItem;
				if(folderItem != null)
				{
					var directoryEntry = FindDirectoryEntry(folderItem.DataContext);
					if(directoryEntry != null)
					{
						if(directoryEntry.IsSelected)
						{
							_treeContent.SetTree(folderItem.DataContext, TreeListBoxMode.ShowDirectoryContent);
						}
						else
						{
							directoryEntry.FocusAndSelect();
						}
					}
					else
					{
						_treeContent.SetTree(folderItem.DataContext, TreeListBoxMode.ShowDirectoryContent);
					}
					_currentDirectory = folderItem.DataContext;
					CurrentDirectoryChanged.Raise(this);
				}
			}
		}

		private static void OnFileViewerProcessExited(object sender, EventArgs e)
		{
			var process = (Process)sender;
			var path = process.StartInfo.FileName;
			try
			{
				File.Delete(path);
			}
			catch(Exception exc)
			{
				LoggingService.Global.Warning(exc, "Failed to remove temporary file: '{0}'", path);
			}
			process.Dispose();
		}

		protected override void AttachToRepository(Repository repository)
		{
			ApplyParameters(Parameters);
		}

		protected override void DetachFromRepository(Repository repository)
		{
			_directoryTree.Clear();
			_treeContent.Clear();
		}

		public override Image Image
		{
			get { return CachedResources.Bitmaps["ImgFolderTree"]; }
		}

		public override void RefreshContent()
		{
			if(_wTree != null)
			{
				_wTree.Refresh();
			}
		}

		protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
		{
			OnKeyDown(this, e);
			base.OnPreviewKeyDown(e);
		}

		private void OnKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			switch(e.KeyCode)
			{
				case Keys.F5:
					RefreshContent();
					e.IsInputKey = true;
					break;
			}
		}

		protected override void SaveMoreViewTo(Section section)
		{
			base.SaveMoreViewTo(section);
			var listNode = section.GetCreateSection("TreeList");
			_treeContent.SaveViewTo(listNode);
		}

		protected override void LoadMoreViewFrom(Section section)
		{
			base.LoadMoreViewFrom(section);
			var listNode = section.TryGetSection("TreeList");
			if(listNode != null)
			{
				_treeContent.LoadViewFrom(listNode);
			}
		}
	}
}
