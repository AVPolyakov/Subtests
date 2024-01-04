using Xunit.Abstractions;

namespace Subtests.Examples;

public class AsyncTests : TestBase
{
    private readonly ITestOutputHelper _testOutputHelper;

    public AsyncTests(ITestOutputHelper testOutputHelper) => _testOutputHelper = testOutputHelper;

    [Test]
    public async Task Case1_Success()
    {
        _testOutputHelper.WriteCallerInfo();

        await Case2_Success();
        await Case4_Success();
    }

    private Task Case2_Success() => Subtest(async () =>
    {
        _testOutputHelper.WriteCallerInfo();

        await Case3_Success();
    });

    private Task Case3_Success() => Subtest(async () =>
    {
        _testOutputHelper.WriteCallerInfo();

        await Task.CompletedTask;
    });

    private Task Case4_Success() => Subtest(async () =>
    {
        _testOutputHelper.WriteCallerInfo();

        await Task.CompletedTask;
    });
    
    [Test]
    public async Task Case5_Success()
    {
        _testOutputHelper.WriteCallerInfo();

        await Subtest(name: "InlineCase1_Success", func: async () =>
        {
            _testOutputHelper.WriteCallerInfo("InlineCase1");

            await Task.CompletedTask;
        });
    }

    [Test]
    public async Task Case7_Success()
    {
        _testOutputHelper.WriteCallerInfo();

        await LocalFunctionCase1_Success();

        Task LocalFunctionCase1_Success() => Subtest(async () =>
        {
            _testOutputHelper.WriteCallerInfo("LocalFunction1Case1");
            
            await Task.CompletedTask;
        });
    }
}