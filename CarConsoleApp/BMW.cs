namespace CarConsoleApp;

/// <summary>Автомобиль с ДВС и автоматической коробкой.</summary>
public sealed class BMW : MechanicalCar, IAutomatic
{
    public BMW()
        : base("BMW", 5)
    {
    }

    public override string PowertrainDescription =>
        "Механическая тяга: бензиновый или дизельный двигатель внутреннего сгорания.";

    public string TransmissionDescription =>
        "Автоматическая коробка передач (типичный гидротрансформатор или роботизированная АКПП).";

    public override string GetDescription()
    {
        return $"{base.GetDescription()} {PowertrainDescription} {TransmissionDescription}";
    }
}
