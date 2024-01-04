using Xunit.Abstractions;

namespace Subtests.Examples;

public class Tests : TestBase
{
    private readonly ITestOutputHelper _testOutputHelper;

    public Tests(ITestOutputHelper testOutputHelper) => _testOutputHelper = testOutputHelper;

    [Test]
    public void Case1_Success()
    {
        _testOutputHelper.WriteCallerInfo();

        Case2_Success();
        Case4_Success();
    }

    private void Case2_Success() => Subtest(() =>
    {
        _testOutputHelper.WriteCallerInfo();

        Case3_Success();
    });

    private void Case3_Success() => Subtest(() =>
    {
        _testOutputHelper.WriteCallerInfo();
    });

    private void Case4_Success() => Subtest(() =>
    {
        _testOutputHelper.WriteCallerInfo();
    });
    
    [Test]
    public void Case5_Success()
    {
        _testOutputHelper.WriteCallerInfo();

        Subtest(name: "InlineCase1_Success", action: () =>
        {
            _testOutputHelper.WriteCallerInfo("InlineCase1");
        });
    }

    [Test]
    public void Case7_Success()
    {
        _testOutputHelper.WriteCallerInfo();

        LocalFunctionCase1_Success();

        void LocalFunctionCase1_Success() => Subtest(() =>
        {
            _testOutputHelper.WriteCallerInfo("LocalFunction1Case1");
        });
    }
}