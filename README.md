# Сабтесты
Сабтест – тест, который является частью более крупного теста. Сабтест может быть частью другого сабтеста.
```csharp
[Test]
public void Case1_Success()
{
    ...
    Case2_Success();
    Case4_Success();
}

private void Case2_Success() => Subtest(() =>
{
    ...
    Case3_Success();
});

private void Case3_Success() => Subtest(() =>
{
    ...
});

private void Case4_Success() => Subtest(() =>
{
    ...
});
```
Сабтесты отображаются в списке тестов. Можно запустить отдельный сабтест.

![Paragraph.png](Files/TestRunning.png?raw=true)  

Если тесты используют базу данных, то в метод `Subtest` следует добавить [откат к savepoint](https://github.com/AVPolyakov/SavepointHandlers).

## Инлайн сабтесты

В некоторых случаях удобно использовать инлайн сабтесты:

```csharp
[Test]
public void Case5_Success()
{
    _testOutputHelper.WriteCallerInfo();

    Subtest(name: "InlineCase1_Success", action: () =>
    {
        _testOutputHelper.WriteCallerInfo("InlineCase1");
    });
}
```

![Paragraph.png](Files/InlineTestRunning.png?raw=true)  

## Локальных функций

Для сабтестов можно использовать локальные функции:

```csharp
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
```

![Paragraph.png](Files/LocalFunctionTestRunning.png?raw=true)  


