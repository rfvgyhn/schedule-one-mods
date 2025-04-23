namespace ScheduleOneMods.DealsSummary.Tests;

public partial class CalculatorTests
{
    public partial class Calculate
    {
        [Theory]
        [MemberData(nameof(Tests))]
        public void Should_Calculate_Correct_Value(Test test)
        {
            var calc = new Calculator(test.Denominations);

            var result = calc.Calculate(test.Amount);

            Assert.Equivalent(test.Expected, result, true);
        }

        [Fact]
        public void Remaining_Should_Be_Unknown_Denomination()
        {
            var calc = new Calculator([]);
            
            var result = calc.Calculate(1);
            
            Assert.Single(result);
            Assert.Equal(Calculator.UnknownDenomination, result.Keys.First());
            Assert.Equal(1, result.Values.First());
        }
    }

    public class CalculateTotals
    {
        [Fact]
        public void Should_Calculate_Correct_Values()
        {
            Contract[] contracts =
            [
                new(new Entry(Product1, 6), new Entry(Product1, 0), new Entry(Product2, 1)),
                new(new Entry(Product1, 4)),
                new(new Entry(Product2, 2)),
            ];
            var calc = new Calculator(TestDenominations);
            Summary[] expected =
            [
                new(Product1, 10, new() { [Size1] = 5, [Size5] = 1}),
                new(Product2, 3, new() { [Size1] = 3 })
            ];

            var result = calc.CalculateTotals(contracts);

            Assert.Equivalent(expected, result, true);
        }
    }
}