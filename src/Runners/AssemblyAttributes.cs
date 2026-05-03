using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Runners.Tests")]
[assembly: SuppressMessage("Performance", "CA1873:Avoid potentially expensive logging", Justification = "There are multiple logging levels based on the sink.")]