using Xunit.Abstractions;

namespace Subtests.Examples;

public class AdvancedTests : TestBase
{
    private readonly ITestOutputHelper _testOutputHelper;

    public AdvancedTests(ITestOutputHelper testOutputHelper) => _testOutputHelper = testOutputHelper;
    
    [Test]
    public void Case1_Success()
    {
        _testOutputHelper.WriteCallerInfo();

        Subtest(() =>
        {
            _testOutputHelper.WriteCallerInfo("Main1");

            Subtest(() =>
            {
                _testOutputHelper.WriteCallerInfo("Main2");
            });
        });
        
        LocalFunction1();

        void LocalFunction1() => Subtest(() =>
        {
            _testOutputHelper.WriteCallerInfo("LocalFunction1");
        });

        Case2_Success();
        Case3_Success();
    }

    private void Case2_Success() => Subtest(() =>
    {
        _testOutputHelper.WriteCallerInfo();

        Subtest(() =>
        {
            _testOutputHelper.WriteCallerInfo("1");
        });
    });

    private void Case3_Success()
    {
        Subtest(() =>
        {
            _testOutputHelper.WriteCallerInfo("1");
        });
        
        Subtest(() =>
        {
            _testOutputHelper.WriteCallerInfo("2");
        });
    }
}