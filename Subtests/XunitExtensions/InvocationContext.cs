namespace Subtests.XunitExtensions;

internal class InvocationContext
{
    private static readonly AsyncLocal<InvocationContext?> _current = new();
    
    public static InvocationContext? Current
    {
        get => _current.Value;
        set => _current.Value = value;
    }

    public TestTestCase TestTestCase { get; }
    
    public InvocationContext(TestTestCase testTestCase)
    {
        TestTestCase = testTestCase;
    }
}