﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Snowflake.Model.Game
{
    /// <summary>
    /// Represents a Stone Platform ID.
    /// </summary>
    public struct PlatformId : IEquatable<PlatformId>, IEquatable<string>
    {
        private string PlatformIdString { get; }
        
        private PlatformId(string id)
        {
            this.PlatformIdString = id?.ToUpperInvariant() ?? "NULL_PLATFORM";
        }

        /// <inheritdoc />
        public bool Equals(PlatformId other)
        {
            return other.PlatformIdString != null && 
                   other.PlatformIdString.Equals(this.PlatformIdString, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <inheritdoc />
        public bool Equals(string other)
        {
            return other != null &&
                   other.Equals(this.PlatformIdString, StringComparison.InvariantCultureIgnoreCase);
        }
        
        /// <inheritdoc />
        public override bool Equals(object other)
        {
            return other switch {
                PlatformId p => this.Equals(p),
                string s => this.Equals(s),
                _ => false
                };
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return this.PlatformIdString;
        }
        
        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Combine(PlatformIdString);
        }

        
#pragma warning disable 1591
        public static bool operator ==(PlatformId x, PlatformId y) => x.PlatformIdString == y.PlatformIdString;
        public static bool operator !=(PlatformId x, PlatformId y) => x.PlatformIdString != y.PlatformIdString;

        public static bool operator ==(string x, PlatformId y) => x.ToUpperInvariant() == y.PlatformIdString;
        public static bool operator !=(string x, PlatformId y) => x.ToUpperInvariant() != y.PlatformIdString;
        public static bool operator ==(PlatformId x, string y) => x.PlatformIdString == y.ToUpperInvariant();
        public static bool operator !=(PlatformId x, string y) => x.PlatformIdString != y.ToUpperInvariant();
#pragma warning restore 1591

           
        /// <summary>
        /// Implicitly converts from a <see cref="string"/> to a <see cref="PlatformId"/>
        /// </summary>
        /// <param name="other">The string to convert.</param>
        /// <returns>The <see cref="PlatformId"/> the string represents.</returns>
        public static implicit operator PlatformId(string other) => new PlatformId(other);
        
        /// <summary>
        /// Implicitly converts from a <see cref="PlatformId"/> to a string.
        /// </summary>
        /// <param name="id">The <see cref="PlatformId"/> to convert as a string.</param>
        /// <returns>The <see cref="PlatformId"/> as a string.</returns>
        public static implicit operator string(PlatformId id) => id.PlatformIdString;
    }
}
