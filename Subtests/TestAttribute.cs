using Subtests.XunitExtensions;
using Xunit;
using Xunit.Sdk;

namespace Subtests;

[XunitTestCaseDiscoverer(TestTestDiscoverer.TypeName, TestTestDiscoverer.AssemblyName)]
public class TestAttribute : FactAttribute
{
    //сделано по аналогии https://github.com/xunit/samples.xunit/blob/main/PartialTrustExample/PartialTrustFactAttribute.cs
}