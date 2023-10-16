using Cmf.CLI.Core.Utilities;
using Cmf.CLI.Utilities;
using Newtonsoft.Json;
using System;
using System.IO.Abstractions;

namespace Cmf.CLI.Core.Objects
{
    /// <summary>
    /// Represents the content to be packed into a DF Package
    /// </summary>
    /// <seealso cref="System.IEquatable{Cmf.CLI.Core.Objects.RelatedPackage}" />
    public class RelatedPackage : IEquatable<RelatedPackage>
    {
        #region Public Properties

        /// <summary>
        /// Relative path to the package folder
        /// </summary>
        /// <value>
        /// The path.
        /// </value>
        public string Path { get; set; }

        /// <summary>
        /// Should trigger build before the root triggered package
        /// </summary>
        /// <value>
        /// The prebuild.
        /// </value>
        public bool PreBuild { get; set; }

        /// <summary>
        /// Should trigger build before the root triggered package
        /// </summary>
        /// <value>
        /// The prebuild.
        /// </value>
        public bool PostBuild { get; set; }

        /// <summary>
        /// Should trigger pack before the root triggered package
        /// </summary>
        /// <value>
        /// The prepack.
        /// </value>
        public bool PrePack { get; set; }

        /// <summary>
        /// Should trigger pack after the root triggered package
        /// </summary>
        /// <value>
        /// The prepack.
        /// </value>
        public bool PostPack { get; set; }

        [JsonIgnore]
        public CmfPackage CmfPackage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance was processed
        /// before or after a pack or build operation
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is set; otherwise, <c>false</c>.
        /// </value>
        [JsonIgnore]
        public bool IsProcessed { get; set; }
        #endregion

        #region Public Methods

        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        ///   <see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <see langword="false" />.
        /// </returns>
        public bool Equals(RelatedPackage other)
        {
            return other != null &&
                   Path.IgnoreCaseEquals(other.Path);
        }

        #endregion
    }
}