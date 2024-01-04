using Xunit.Abstractions;

namespace Subtests.Examples;

public class AsyncAdvancedTests : TestBase
{
    private readonly ITestOutputHelper _testOutputHelper;

    public AsyncAdvancedTests(ITestOutputHelper testOutputHelper) => _testOutputHelper = testOutputHelper;
    
    [Test]
    public async Task Case1_Success()
    {
        _testOutputHelper.WriteCallerInfo();

        await Subtest(async () =>
        {
            _testOutputHelper.WriteCallerInfo("Main1");

            await Subtest(async () =>
            {
                _testOutputHelper.WriteCallerInfo("Main2");

                await Task.CompletedTask;
            });
        });

        await LocalFunctionCase1_Success();

        Task LocalFunctionCase1_Success() => Subtest(async () =>
        {
            _testOutputHelper.WriteCallerInfo("LocalFunction1Case1");

            await Subtest(name: "InlineCase1_Success", func: async () =>
            {
                _testOutputHelper.WriteCallerInfo("InlineCase1");

                await Task.CompletedTask;
            });
            
            await Task.CompletedTask;
        });

        await Case2_Success();
        await Case3_Success();
    }

    private Task Case2_Success() => Subtest(async () =>
    {
        _testOutputHelper.WriteCallerInfo();

        await Subtest(async () =>
        {
            _testOutputHelper.WriteCallerInfo("1");
            
            await Task.CompletedTask;
        });
    });

    private async Task Case3_Success()
    {
        await Subtest(async () =>
        {
            _testOutputHelper.WriteCallerInfo("1");
            
            await Task.CompletedTask;
        });
        
        await Subtest(async () =>
        {
            _testOutputHelper.WriteCallerInfo("2");
            
            await Task.CompletedTask;
        });
    }
}