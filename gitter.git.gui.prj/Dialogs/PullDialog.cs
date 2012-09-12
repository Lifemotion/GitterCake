﻿namespace gitter.Git.Gui.Dialogs
{
	using System;
	using System.ComponentModel;

	using gitter.Framework;

	using Resources = gitter.Git.Gui.Properties.Resources;

	[ToolboxItem(false)]
	public partial class PullDialog : GitDialogBase
	{
		private readonly Repository _repository;

		public PullDialog(Repository repository)
		{
			Verify.Argument.IsNotNull(repository, "repository");

			_repository = repository;

			InitializeComponent();

			Text = Resources.StrPull;
		}

		protected override string ActionVerb
		{
			get { return Resources.StrPull; }
		}
	}
}
