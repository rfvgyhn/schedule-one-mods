namespace ContractAggregates.Tests;

public partial class CalculatorTests
{
    private const string Product1 = "p1";
    private const string Product2 = "p2";
    private const string Size1 = "s1";
    private const string Size5 = "s5";
    private const string Size20 = "s20";
    private static readonly Dictionary<int, string> TestDenominations = new()
    {
        [1] = Size1,
        [5] = Size5,
        [20] = Size20
    };

    public partial class Calculate
    {
        public static IEnumerable<TheoryDataRow<Test>> Tests =
        [
            new(new(0, TestDenominations, [])),
            new(new(4, TestDenominations, [new(Size1, 4)])),
            new(new(5, TestDenominations, [new(Size5, 1)])),
            new(new(6, TestDenominations, [new(Size5, 1), new(Size1, 1)])),
            new(new(21, TestDenominations, [new(Size20, 1), new(Size1, 1)]))
        ];

        public record Test(
            int Amount,
            Dictionary<int, string> Denominations,
            KeyValuePair<string, int>[] Expected)
        {
            public override string ToString() => $"Amount = {Amount}";
        }
    }
}