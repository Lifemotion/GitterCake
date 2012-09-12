﻿namespace gitter.Git
{
	using System;

	using gitter.Git.AccessLayer;

	/// <summary>Feature which is available in specified version of git.</summary>
	public sealed class VersionFeature : GitFeature
	{
		private readonly Version _version;

		public VersionFeature(string name, Version version)
			: base(name)
		{
			Verify.Argument.IsNotNull(version, "version");

			_version = version;
		}

		public Version RequiredVersion
		{
			get { return _version; }
		}

		public override bool IsAvailableFor(IGitAccessor gitAccessor)
		{
			Verify.Argument.IsNotNull(gitAccessor, "gitAccessor");

			return gitAccessor.GitVersion >= _version;
		}
	}
}
