using Microsoft.CodeAnalysis;
using Milochau.Core.Aws.DynamoDB.Generator.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Milochau.Core.Aws.DynamoDB.Generator.Models
{
    internal readonly struct DiagnosticInfo : IEquatable<DiagnosticInfo>
    {
        public DiagnosticDescriptor Descriptor { get; }
        public object?[] MessageArgs { get; }
        public Location? Location { get; }

        public DiagnosticInfo(DiagnosticDescriptor descriptor, Location? location, params object?[]? messageArgs)
        {
            Location? trimmedLocation = location is null ? null : GetTrimmedLocation(location);

            Descriptor = descriptor;
            Location = trimmedLocation;
            MessageArgs = messageArgs ?? Array.Empty<object?>();

            // Creates a copy of the Location instance that does not capture a reference to Compilation.
            static Location GetTrimmedLocation(Location location)
                => Location.Create(location.SourceTree?.FilePath ?? "", location.SourceSpan, location.GetLineSpan().Span);
        }

        public Diagnostic CreateDiagnostic()
            => Diagnostic.Create(Descriptor, Location, MessageArgs);

        public override readonly bool Equals(object? obj) => obj is DiagnosticInfo info && Equals(info);

        public readonly bool Equals(DiagnosticInfo other)
        {
            return Descriptor.Equals(other.Descriptor) &&
                MessageArgs.SequenceEqual(other.MessageArgs) &&
                Location == other.Location;
        }

        public override readonly int GetHashCode()
        {
            int hashCode = Descriptor.GetHashCode();
            foreach (object? messageArg in MessageArgs)
            {
                hashCode = HashHelpers.Combine(hashCode, messageArg?.GetHashCode() ?? 0);
            }

            hashCode = HashHelpers.Combine(hashCode, Location?.GetHashCode() ?? 0);
            return hashCode;
        }
    }
}
